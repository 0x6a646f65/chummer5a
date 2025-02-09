/*  This file is part of Chummer5a.
 *
 *  Chummer5a is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Chummer5a is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Chummer5a.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  You can obtain the full source code for Chummer5a at
 *  https://github.com/chummer5a/chummer5a
 */
 using System;
using System.Collections.Generic;
 using System.Globalization;
 using System.Linq;
using System.Text;
using System.Windows.Forms;
 using System.Xml;
 using System.Xml.XPath;
 using Chummer.Backend.Equipment;

namespace Chummer
{
    public partial class frmSelectWeaponAccessory : Form
    {
        private string _strSelectedAccessory;
        private decimal _decMarkup;
        private int _intSelectedRating;

        private bool _blnLoading = true;
        private readonly List<string> _lstAllowedMounts = new List<string>();
        private Weapon _objParentWeapon;
        private bool _blnIsParentWeaponBlackMarketAllowed;
        private bool _blnAddAgain;

        private readonly XPathNavigator _xmlBaseChummerNode;
        private readonly Character _objCharacter;
        private bool _blnBlackMarketDiscount;
        private readonly HashSet<string> _setBlackMarketMaps;

        #region Control Events
        public frmSelectWeaponAccessory(Character objCharacter)
        {
            InitializeComponent();
            LanguageManager.TranslateWinForm(GlobalOptions.Language, this);
            lblMarkupLabel.Visible = objCharacter.Created;
            nudMarkup.Visible = objCharacter.Created;
            lblMarkupPercentLabel.Visible = objCharacter.Created;
            _objCharacter = objCharacter;
            // Load the Weapon information.
            XmlDocument objXmlDocument = XmlManager.Load("weapons.xml");
            _xmlBaseChummerNode = objXmlDocument.GetFastNavigator().SelectSingleNode("/chummer");
            _setBlackMarketMaps = _objCharacter.GenerateBlackMarketMappings(objXmlDocument);
        }

        private void frmSelectWeaponAccessory_Load(object sender, EventArgs e)
        {
            if (_objCharacter.Created)
            {
                chkHideOverAvailLimit.Visible = false;
                chkHideOverAvailLimit.Checked = false;
            }
            else
            {
                chkHideOverAvailLimit.Text = string.Format(chkHideOverAvailLimit.Text, _objCharacter.MaximumAvailability.ToString(GlobalOptions.CultureInfo));
                chkHideOverAvailLimit.Checked = _objCharacter.Options.HideItemsOverAvailLimit;
            }

            chkBlackMarketDiscount.Visible = _objCharacter.BlackMarketDiscount;

            _blnLoading = false;
            BuildAccessoryList();
        }

        /// <summary>
        /// Build the list of available weapon accessories.
        /// </summary>
        private void BuildAccessoryList()
        {
            List<ListItem> lstAccessories = new List<ListItem>();

            // Populate the Accessory list.
            StringBuilder strMount = new StringBuilder("contains(mount, \"Internal\") or contains(mount, \"None\") or mount = \"\"");
            foreach (string strAllowedMount in _lstAllowedMounts)
            {
                if (!string.IsNullOrEmpty(strAllowedMount))
                    strMount.Append(" or contains(mount, \"" + strAllowedMount + "\")");
            }
            strMount.Append(CommonFunctions.GenerateSearchXPath(txtSearch.Text));
            XPathNavigator xmlParentWeaponDataNode = _xmlBaseChummerNode.SelectSingleNode("weapons/weapon[id = \"" + _objParentWeapon.SourceIDString + "\"]");
            foreach (XPathNavigator objXmlAccessory in _xmlBaseChummerNode.Select("accessories/accessory[(" + strMount + ") and (" + _objCharacter.Options.BookXPath() + ")]"))
            {
                string strId = objXmlAccessory.SelectSingleNode("id")?.Value;
                if (string.IsNullOrEmpty(strId))
                    continue;

                XPathNavigator xmlExtraMountNode = objXmlAccessory.SelectSingleNode("extramount");
                if (xmlExtraMountNode != null)
                {
                    if (_lstAllowedMounts.Count > 1)
                    {
                        foreach (string strItem in xmlExtraMountNode.Value.Split('/'))
                        {
                            if (!string.IsNullOrEmpty(strItem) && _lstAllowedMounts.All(strAllowedMount => strAllowedMount != strItem))
                            {
                                goto NextItem;
                            }
                        }
                    }
                }

                if (!objXmlAccessory.RequirementsMet(_objCharacter, _objParentWeapon, string.Empty, string.Empty)) continue;

                XPathNavigator xmlTestNode = objXmlAccessory.SelectSingleNode("forbidden/weapondetails");
                if (xmlTestNode != null)
                {
                    // Assumes topmost parent is an AND node
                    if (xmlParentWeaponDataNode.ProcessFilterOperationNode(xmlTestNode, false))
                    {
                        continue;
                    }
                }
                xmlTestNode = objXmlAccessory.SelectSingleNode("required/weapondetails");
                if (xmlTestNode != null)
                {
                    // Assumes topmost parent is an AND node
                    if (!xmlParentWeaponDataNode.ProcessFilterOperationNode(xmlTestNode, false))
                    {
                        continue;
                    }
                }

                decimal decCostMultiplier = 1 + (nudMarkup.Value / 100.0m);
                if (_blnIsParentWeaponBlackMarketAllowed)
                    decCostMultiplier *= 0.9m;
                if ((!chkHideOverAvailLimit.Checked || SelectionShared.CheckAvailRestriction(objXmlAccessory, _objCharacter) &&
                     (chkFreeItem.Checked || !chkShowOnlyAffordItems.Checked ||
                      SelectionShared.CheckNuyenRestriction(objXmlAccessory, _objCharacter.Nuyen, decCostMultiplier))))
                {
                    lstAccessories.Add(new ListItem(strId, objXmlAccessory.SelectSingleNode("translate")?.Value ?? objXmlAccessory.SelectSingleNode("name")?.Value ?? LanguageManager.GetString("String_Unknown", GlobalOptions.Language)));
                }
                NextItem:;
            }
            
            lstAccessories.Sort(CompareListItems.CompareNames);
            string strOldSelected = lstAccessory.SelectedValue?.ToString();
            _blnLoading = true;
            lstAccessory.BeginUpdate();
            lstAccessory.ValueMember = "Value";
            lstAccessory.DisplayMember = "Name";
            lstAccessory.DataSource = lstAccessories;
            _blnLoading = false;
            if (!string.IsNullOrEmpty(strOldSelected))
                lstAccessory.SelectedValue = strOldSelected;
            else
                lstAccessory.SelectedIndex = -1;
            lstAccessory.EndUpdate();
        }

        private void lstAccessory_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateGearInfo();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            _blnAddAgain = false;
            AcceptForm();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void lstAccessory_DoubleClick(object sender, EventArgs e)
        {
            _blnAddAgain = false;
            AcceptForm();
        }

        private void cmdOKAdd_Click(object sender, EventArgs e)
        {
            _blnAddAgain = true;
            AcceptForm();
        }

        private void chkFreeItem_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowOnlyAffordItems.Checked)
                BuildAccessoryList();
            UpdateGearInfo();
        }

        private void chkBlackMarketDiscount_CheckedChanged(object sender, EventArgs e)
        {
            UpdateGearInfo();
        }

        private void nudMarkup_ValueChanged(object sender, EventArgs e)
        {
            if (chkShowOnlyAffordItems.Checked  && !chkFreeItem.Checked)
                BuildAccessoryList();
            UpdateGearInfo();
        }

        private void nudRating_ValueChanged(object sender, EventArgs e)
        {
            UpdateGearInfo();
        }

        private void cboMount_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMountFields(true);
            if (!string.IsNullOrEmpty(_objParentWeapon.DoubledCostModificationSlots))
                UpdateGearInfo(false);
        }

        private void cboExtraMount_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMountFields(false);
            if (!string.IsNullOrEmpty(_objParentWeapon.DoubledCostModificationSlots))
                UpdateGearInfo(false);
        }
        private void chkHideOverAvailLimit_CheckedChanged(object sender, EventArgs e)
        {
            BuildAccessoryList();
        }
        private void chkShowOnlyAffordItems_CheckedChanged(object sender, EventArgs e)
        {
            BuildAccessoryList();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Whether or not the user wants to add another item after this one.
        /// </summary>
        public bool AddAgain => _blnAddAgain;

        /// <summary>
        /// Name of Accessory that was selected in the dialogue.
        /// </summary>
        public string SelectedAccessory => _strSelectedAccessory;

        /// <summary>
        /// Mount that was selected in the dialogue.
        /// </summary>
        public Tuple<string, string> SelectedMount => new Tuple<string, string>(cboMount.SelectedItem?.ToString(), cboExtraMount.SelectedItem?.ToString());

        /// <summary>
        /// Rating of the Accessory.
        /// </summary>
        public int SelectedRating => _intSelectedRating;

        /// <summary>
        /// GUID of the current weapon for which the accessory is being selected
        /// </summary>
        public Weapon ParentWeapon
        {
            set
            {
                _objParentWeapon = value;
                _lstAllowedMounts.Clear();
                foreach (XPathNavigator objXmlMount in _xmlBaseChummerNode.Select("weapons/weapon[id = \"" + value.SourceIDString + "\"]/accessorymounts/mount"))
                {
                    string strLoopMount = objXmlMount.Value;
                    // Run through the Weapon's currenct Accessories and filter out any used up Mount points.
                    if (!_objParentWeapon.WeaponAccessories.Any(objMod =>
                        objMod.Mount == strLoopMount || objMod.ExtraMount == strLoopMount))
                    {
                        _lstAllowedMounts.Add(strLoopMount);
                    }
                }

                //TODO: Accessories don't use a category mapping, so we use parent weapon's category instead.
                if (_objCharacter.BlackMarketDiscount)
                {
                    string strCategory = value.GetNode()?.SelectSingleNode("category")?.InnerText ?? string.Empty;
                    _blnIsParentWeaponBlackMarketAllowed = !string.IsNullOrEmpty(strCategory) && _setBlackMarketMaps.Contains(strCategory);
                }
                else
                {
                    _blnIsParentWeaponBlackMarketAllowed = false;
                }
            }
        }

        /// <summary>
        /// Whether or not the item should be added for free.
        /// </summary>
        public bool FreeCost => chkFreeItem.Checked;

        /// <summary>
        /// Whether or not the selected Vehicle is used.
        /// </summary>
        public bool BlackMarketDiscount => _blnBlackMarketDiscount;

        /// <summary>
        /// Markup percentage.
        /// </summary>
        public decimal Markup => _decMarkup;

        #endregion

        #region Methods
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            BuildAccessoryList();
        }

        private void UpdateMountFields(bool boolChangeExtraMountFirst)
        {
            if ((cboMount.SelectedItem.ToString() != "None") && cboExtraMount.SelectedItem != null && (cboExtraMount.SelectedItem.ToString() != "None")
                && (cboMount.SelectedItem.ToString() == cboExtraMount.SelectedItem.ToString()))
            {
                if (boolChangeExtraMountFirst)
                    cboExtraMount.SelectedIndex = 0;
                else
                    cboMount.SelectedIndex = 0;
                while (cboMount.SelectedItem.ToString() != "None" && cboExtraMount.SelectedItem.ToString() != "None" && cboMount.SelectedItem.ToString() == cboExtraMount.SelectedItem.ToString())
                {
                    if (boolChangeExtraMountFirst)
                        cboExtraMount.SelectedIndex += 1;
                    else
                        cboMount.SelectedIndex += 1;
                }
            }
        }

        private void UpdateGearInfo(bool blnUpdateMountCBOs = true)
        {
            if (_blnLoading)
                return;

            XPathNavigator xmlAccessory = null;
            string strSelectedId = lstAccessory.SelectedValue?.ToString();
            // Retrieve the information for the selected Accessory.
            if (!string.IsNullOrEmpty(strSelectedId))
                xmlAccessory = _xmlBaseChummerNode.SelectSingleNode("accessories/accessory[id = \"" + strSelectedId + "\"]");
            if (xmlAccessory == null)
            {
                lblRC.Visible = false;
                lblRCLabel.Visible = false;
                nudRating.Enabled = false;
                nudRating.Visible = false;
                lblRatingLabel.Visible = false;
                lblRatingNALabel.Visible = false;
                lblMountLabel.Visible = false;
                cboMount.Visible = false;
                cboMount.Items.Clear();
                lblExtraMountLabel.Visible = false;
                cboExtraMount.Visible = false;
                cboExtraMount.Items.Clear();
                lblAvailLabel.Visible = false;
                lblAvail.Text = string.Empty;
                lblCostLabel.Visible = false;
                lblCost.Text = string.Empty;
                lblTestLabel.Visible = false;
                lblTest.Text = string.Empty;
                lblSourceLabel.Visible = false;
                lblSource.Text = string.Empty;
                lblSource.SetToolTip(string.Empty);
                return;
            }

            string strRC = xmlAccessory.SelectSingleNode("rc")?.Value;
            if (!string.IsNullOrEmpty(strRC))
            {
                lblRC.Visible = true;
                lblRCLabel.Visible = true;
                lblRC.Text = strRC;
            }
            else
            {
                lblRC.Visible = false;
                lblRCLabel.Visible = false;
            }
            if (int.TryParse(xmlAccessory.SelectSingleNode("rating")?.Value, out int intMaxRating) && intMaxRating > 0)
            {
                nudRating.Maximum = intMaxRating;
                if (chkHideOverAvailLimit.Checked)
                {
                    while (nudRating.Maximum > nudRating.Minimum && !SelectionShared.CheckAvailRestriction(xmlAccessory, _objCharacter, decimal.ToInt32(nudRating.Maximum)))
                    {
                        nudRating.Maximum -= 1;
                    }
                }
                if (chkShowOnlyAffordItems.Checked && !chkFreeItem.Checked)
                {
                    decimal decCostMultiplier = 1 + (nudMarkup.Value / 100.0m);
                    if (_setBlackMarketMaps.Contains(xmlAccessory.SelectSingleNode("category")?.Value))
                        decCostMultiplier *= 0.9m;
                    while (nudRating.Maximum > nudRating.Minimum && !SelectionShared.CheckNuyenRestriction(xmlAccessory, _objCharacter.Nuyen, decCostMultiplier, decimal.ToInt32(nudRating.Maximum)))
                    {
                        nudRating.Maximum -= 1;
                    }
                }
                nudRating.Enabled = nudRating.Maximum != nudRating.Minimum;
                nudRating.Visible = true;
                lblRatingLabel.Visible = true;
                lblRatingNALabel.Visible = false;
            }
            else
            {
                lblRatingNALabel.Visible = true;
                nudRating.Enabled = false;
                nudRating.Visible = false;
                lblRatingLabel.Visible = true;
            }

            if (blnUpdateMountCBOs)
            {
                string[] astrDataMounts = xmlAccessory.SelectSingleNode("mount")?.Value.Split('/');
                List<string> strMounts = new List<string>();
                if (astrDataMounts != null)
                {
                    strMounts.AddRange(astrDataMounts);
                }

                strMounts.Add("None");

                List<string> strAllowed = new List<string>(_lstAllowedMounts) {"None"};
                cboMount.Visible = true;
                cboMount.Items.Clear();
                foreach (string strCurrentMount in strMounts)
                {
                    if (!string.IsNullOrEmpty(strCurrentMount))
                    {
                        foreach (string strAllowedMount in strAllowed)
                        {
                            if (strCurrentMount == strAllowedMount)
                            {
                                cboMount.Items.Add(strCurrentMount);
                            }
                        }
                    }
                }

                cboMount.Enabled = cboMount.Items.Count > 1;
                cboMount.SelectedIndex = 0;
                lblMountLabel.Visible = true;

                List<string> strExtraMounts = new List<string>();
                string strExtraMount = xmlAccessory.SelectSingleNode("extramount")?.Value;
                if (!string.IsNullOrEmpty(strExtraMount))
                {
                    foreach (string strItem in strExtraMount.Split('/'))
                    {
                        strExtraMounts.Add(strItem);
                    }
                }

                strExtraMounts.Add("None");

                cboExtraMount.Items.Clear();
                foreach (string strCurrentMount in strExtraMounts)
                {
                    if (!string.IsNullOrEmpty(strCurrentMount))
                    {
                        foreach (string strAllowedMount in strAllowed)
                        {
                            if (strCurrentMount == strAllowedMount)
                            {
                                cboExtraMount.Items.Add(strCurrentMount);
                            }
                        }
                    }
                }

                cboExtraMount.Enabled = cboExtraMount.Items.Count > 1;
                cboExtraMount.SelectedIndex = 0;
                if (cboMount.SelectedItem.ToString() != "None" && cboExtraMount.SelectedItem.ToString() != "None"
                                                               && cboMount.SelectedItem.ToString() == cboExtraMount.SelectedItem.ToString())
                    cboExtraMount.SelectedIndex += 1;
                cboExtraMount.Visible = cboExtraMount.Enabled && cboExtraMount.SelectedItem.ToString() != "None";
                lblExtraMountLabel.Visible = cboExtraMount.Visible;
            }

            // Avail.
            // If avail contains "F" or "R", remove it from the string so we can use the expression.
            lblAvail.Text = new AvailabilityValue(Convert.ToInt32(nudRating.Value), xmlAccessory.SelectSingleNode("avail")?.Value).ToString();
            lblAvailLabel.Visible = !string.IsNullOrEmpty(lblAvail.Text);

            if (!chkFreeItem.Checked)
            {
                string strCost = "0";
                if (xmlAccessory.TryGetStringFieldQuickly("cost", ref strCost))
                    strCost = strCost.CheapReplace("Weapon Cost", () => _objParentWeapon.OwnCost.ToString(GlobalOptions.InvariantCultureInfo))
                        .CheapReplace("Weapon Total Cost", () => _objParentWeapon.MultipliableCost(null).ToString(GlobalOptions.InvariantCultureInfo))
                        .Replace("Rating", nudRating.Value.ToString(GlobalOptions.CultureInfo));
                if (strCost.StartsWith("Variable("))
                {
                    decimal decMin;
                    decimal decMax = decimal.MaxValue;
                    strCost = strCost.TrimStartOnce("Variable(", true).TrimEndOnce(')');
                    if (strCost.Contains('-'))
                    {
                        string[] strValues = strCost.Split('-');
                        decimal.TryParse(strValues[0], NumberStyles.Any, GlobalOptions.InvariantCultureInfo, out decMin);
                        decimal.TryParse(strValues[1], NumberStyles.Any, GlobalOptions.InvariantCultureInfo, out decMax);
                    }
                    else
                        decimal.TryParse(strCost.FastEscape('+'), NumberStyles.Any, GlobalOptions.InvariantCultureInfo, out decMin);

                    if (decMax == decimal.MaxValue)
                    {
                        lblCost.Text = decMin.ToString(_objCharacter.Options.NuyenFormat, GlobalOptions.CultureInfo) + "¥+";
                    }
                    else
                        lblCost.Text = decMin.ToString(_objCharacter.Options.NuyenFormat, GlobalOptions.CultureInfo) + " - " + decMax.ToString(_objCharacter.Options.NuyenFormat, GlobalOptions.CultureInfo) + '¥';

                    lblTest.Text = _objCharacter.AvailTest(decMax, lblAvail.Text);
                }
                else
                {
                    object objProcess = CommonFunctions.EvaluateInvariantXPath(strCost, out bool blnIsSuccess);
                    decimal decCost = blnIsSuccess ? Convert.ToDecimal(objProcess, GlobalOptions.InvariantCultureInfo) : 0;

                    // Apply any markup.
                    decCost *= 1 + (nudMarkup.Value / 100.0m);

                    if (chkBlackMarketDiscount.Checked)
                        decCost *= 0.9m;
                    decCost *= _objParentWeapon.AccessoryMultiplier;
                    if (!string.IsNullOrEmpty(_objParentWeapon.DoubledCostModificationSlots))
                    {
                        string[] astrParentDoubledCostModificationSlots = _objParentWeapon.DoubledCostModificationSlots.Split('/');
                        if (astrParentDoubledCostModificationSlots.Contains(cboMount.SelectedItem?.ToString()) ||
                            astrParentDoubledCostModificationSlots.Contains(cboExtraMount.SelectedItem?.ToString()))
                        {
                            decCost *= 2;
                        }
                    }

                    lblCost.Text = decCost.ToString(_objCharacter.Options.NuyenFormat, GlobalOptions.CultureInfo) + '¥';
                    lblTest.Text = _objCharacter.AvailTest(decCost, lblAvail.Text);
                }
            }
            else
            {
                lblCost.Text = (0.0m).ToString(_objCharacter.Options.NuyenFormat, GlobalOptions.CultureInfo) + '¥';
                lblTest.Text = _objCharacter.AvailTest(0, lblAvail.Text);
            }
            lblCostLabel.Visible = !string.IsNullOrEmpty(lblCost.Text);
            lblTestLabel.Visible = !string.IsNullOrEmpty(lblTest.Text);
            chkBlackMarketDiscount.Checked = _blnIsParentWeaponBlackMarketAllowed;
            string strSource = xmlAccessory.SelectSingleNode("source")?.Value ?? LanguageManager.GetString("String_Unknown", GlobalOptions.Language);
            string strPage = xmlAccessory.SelectSingleNode("altpage")?.Value ?? xmlAccessory.SelectSingleNode("page")?.Value ?? LanguageManager.GetString("String_Unknown", GlobalOptions.Language);
            string strSpaceCharacter = LanguageManager.GetString("String_Space", GlobalOptions.Language);
            lblSource.Text = CommonFunctions.LanguageBookShort(strSource, GlobalOptions.Language) + strSpaceCharacter + strPage;
            lblSource.SetToolTip(CommonFunctions.LanguageBookLong(strSource, GlobalOptions.Language) + strSpaceCharacter + LanguageManager.GetString("String_Page", GlobalOptions.Language) + strSpaceCharacter + strPage);
            lblSourceLabel.Visible = !string.IsNullOrEmpty(lblSource.Text);
        }
        /// <summary>
        /// Accept the selected item and close the form.
        /// </summary>
        private void AcceptForm()
        {
            string strSelectedId = lstAccessory.SelectedValue?.ToString();
            if (!string.IsNullOrEmpty(strSelectedId))
            {
                _strSelectedAccessory = strSelectedId;
                _decMarkup = nudMarkup.Value;
                _intSelectedRating = nudRating.Visible ? decimal.ToInt32(nudRating.Value) : 0;
                _blnBlackMarketDiscount = chkBlackMarketDiscount.Checked;
                DialogResult = DialogResult.OK;
            }
        }

        private void OpenSourceFromLabel(object sender, EventArgs e)
        {
            CommonFunctions.OpenPDFFromControl(sender, e);
        }
        #endregion
    }
}
