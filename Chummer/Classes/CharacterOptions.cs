using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Chummer.Backend.Attributes.OptionDisplayAttributes;
using Chummer.Backend.Attributes.SaveAttributes;
using Microsoft.Win32;

namespace Chummer
{
	public class CharacterOptions
	{
		#region Default Values
		private string _strFileName = "default.xml";

	    // Settings.
		private bool _blnAllow2ndMaxAttribute;
		private bool _blnAllowAttributePointsOnExceptional;
	    private bool _blnAllowExceedAttributeBP;
		private bool _blnAllowHigherStackedFoci;
	    private bool _blnAlternateArmorEncumbrance;
		private bool _blnAlternateComplexFormCost;
		private bool _blnAlternateMatrixAttribute;
	    private bool _blnAutomaticCopyProtection = true;
		private bool _blnAutomaticRegistration = true;
	    private bool _blnCalculateCommlinkResponse = true;
	    private bool _blnEnforceSkillMaximumModifiedRating;
		private bool _blnErgonomicProgramsLimit = true;
	    private bool _blnIgnoreArmorEncumbrance = true;
	    private bool _blnMayBuyQualities;
	    private bool _blnNoSingleArmorEncumbrance;
		private bool _blnPrintArcanaAlternates;
	    private bool _blnPrintLeadershipAlternates;
	    private bool _blnSpecialAttributeKarmaLimit;
	    private bool _blnUseContactPoints;
	    private int _intFreeContactsFlatNumber = 0;

	    private readonly XmlDocument _objBookDoc = new XmlDocument();
		private string _strBookXPath = "";

	    // Karma variables.
	    private int _intKarmaNuyenPer = 2000;

	    // Karma Foci variables.

	    // Default build settings.

	    #endregion
		// Sourcebook list.

	    #region Initialization, Save, and Load Methods
		public CharacterOptions()
		{
			//// Create the settings directory if it does not exist.
			//string settingsDirectoryPath = Path.Combine(Application.StartupPath, "settings");
			//if (!Directory.Exists(settingsDirectoryPath))
			//	Directory.CreateDirectory(settingsDirectoryPath);

			//// If the default.xml settings file does not exist, attempt to read the settings from the Registry (old storage format), then save them to the default.xml file.
			//string strFilePath = Path.Combine(settingsDirectoryPath, "default.xml");
			//if (!File.Exists(strFilePath))
			//{
			//	_strFileName = "default.xml";
			//	LoadFromRegistry();
			//	Save();
			//}
			//else
			//	Load("default.xml");
			//// Load the language file.
			//LanguageManager.Instance.Load(GlobalOptions.Instance.Language, this);

			//// Load the book information.
			//_objBookDoc = XmlManager.Instance.Load("books.xml");
		}

		#endregion

		#region Methods
		/// <summary>
		/// Load the Options from the Registry (which will subsequently be converted to the XML Settings File format). Registry keys are deleted once they are read since they will no longer be used.
		/// </summary>
		private void LoadFromRegistry()
		{
			// Confirm delete.
			try
			{
				ConfirmDelete = Convert.ToBoolean(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("confirmdelete").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("confirmdelete");
			}
			catch
			{
			}

			// Confirm Karama Expense.
			try
			{
				ConfirmKarmaExpense = Convert.ToBoolean(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("confirmkarmaexpense").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("confirmkarmaexpense");
			}
			catch
			{
			}

			// Print all Active Skills with a total value greater than 0 (as opposed to only printing those with a Rating higher than 0).
			try
			{
				PrintSkillsWithZeroRating = Convert.ToBoolean(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("printzeroratingskills").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("printzeroratingskills");
			}
			catch
			{
			}

			// More Lethal Gameplay.
			try
			{
				MoreLethalGameplay = Convert.ToBoolean(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("morelethalgameplay").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("morelethalgameplay");
			}
			catch
			{
			}

			// Spirit Force Based on Total MAG.
			try
			{
				SpiritForceBasedOnTotalMAG = Convert.ToBoolean(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("spiritforcebasedontotalmag").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("spiritforcebasedontotalmag");
			}
			catch
			{
			}

			// Skill Defaulting Includes Modifers.
			try
			{
				SkillDefaultingIncludesModifiers = Convert.ToBoolean(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("skilldefaultingincludesmodifiers").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("skilldefaultingincludesmodifiers");
			}
			catch
			{
			}

			// Enforce Skill Maximum Modified Rating.
			try
			{
				_blnEnforceSkillMaximumModifiedRating = Convert.ToBoolean(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("enforceskillmaximummodifiedrating").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("enforceskillmaximummodifiedrating");
			}
			catch
			{
			}

			// Cap Skill Rating.
			try
			{
				CapSkillRating = Convert.ToBoolean(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("capskillrating").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("capskillrating");
			}
			catch
			{
			}

			// Print Expenses.
			try
			{
				PrintExpenses = Convert.ToBoolean(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("printexpenses").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("printexpenses");
			}
			catch
			{
			}

			// Nuyen per Build Point
			try
			{
				NuyenPerBP = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("nuyenperbp").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("nuyenperbp");
			}
			catch
			{
			}

			// Free Contacts Multiplier Enabled
			try
			{
				FreeContactsMultiplierEnabled = Convert.ToBoolean(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("freekarmacontactsmultiplierenabled").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("freekarmacontactsmultiplierenabled");
			}
			catch
			{
			}

			// Free Contacts Multiplier Value
			try
			{
				FreeContactsMultiplier = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("freekarmacontactsmultiplier").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("freekarmacontactsmultiplier");
			}
			catch
			{
			}

			// Free Knowledge Multiplier Enabled
			try
			{
				FreeKnowledgeMultiplierEnabled = Convert.ToBoolean(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("freekarmaknowledgemultiplierenabled").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("freekarmaknowledgemultiplierenabled");
			}
			catch
			{
			}
			// Free Knowledge Multiplier Value
			try
			{
				FreeKnowledgeMultiplier = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("freekarmaknowledgemultiplier").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("freekarmaknowledgemultiplier");
			}
			catch
			{
			}

			// Karma Free Knowledge Multiplier Enabled
			try
			{
				FreeKnowledgeMultiplierEnabled = Convert.ToBoolean(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("freeknowledgemultiplierenabled").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("freeknowledgemultiplierenabled");
			}
			catch
			{
			}
			// Karma Free Knowledge
			try
			{
				FreeKarmaContacts = Convert.ToBoolean(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("freekarmacontacts").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("freekarmacontacts");
			}
			catch
			{
			}
			// Karma Free Knowledge
			try
			{
				FreeKarmaKnowledge = Convert.ToBoolean(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("freekarmaknowledge").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("freekarmaknowledge");
			}
			catch
			{
			}

			// No Single Armor Encumbrance
			try
			{
				_blnNoSingleArmorEncumbrance = Convert.ToBoolean(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("nosinglearmorencumbrance").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("nosinglearmorencumbrance");
			}
			catch
			{
			}

			// Essence Loss Reduces Maximum Only.
			try
			{
				ESSLossReducesMaximumOnly = Convert.ToBoolean(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("esslossreducesmaximumonly").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("esslossreducesmaximumonly");
			}
			catch
			{
			}

			// Allow Skill Regrouping.
			try
			{
				AllowSkillRegrouping = Convert.ToBoolean(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("allowskillregrouping").ToString());
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("allowskillregrouping");
			}
			catch
			{
			}

			// Attempt to populate the Karma values.
			try
			{
				KarmaAttribute = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmaattribute").ToString());
				KarmaQuality = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmaquality").ToString());
				KarmaSpecialization = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmaspecialization").ToString());
				KarmaNewKnowledgeSkill = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmanewknowledgeskill").ToString());
				KarmaNewActiveSkill = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmanewactiveskill").ToString());
				KarmaNewSkillGroup = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmanewskillgroup").ToString());
				KarmaImproveKnowledgeSkill = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmaimproveknowledgeskill").ToString());
				KarmaImproveActiveSkill = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmaimproveactiveskill").ToString());
				KarmaImproveSkillGroup = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmaimproveskillgroup").ToString());
				KarmaSpell = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmaspell").ToString());
				KarmaEnhancement = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmaenhancement").ToString());
				KarmaNewComplexForm = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmanewcomplexform").ToString());
				KarmaImproveComplexForm = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmaimprovecomplexform").ToString());
				_intKarmaNuyenPer = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmanuyenper").ToString());
				KarmaContact = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmacontact").ToString());
				KarmaEnemy = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmaenemy").ToString());
				KarmaCarryover = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmacarryover").ToString());
				KarmaSpirit = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmaspirit").ToString());
				KarmaManeuver = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmamaneuver").ToString());
				KarmaInitiation = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmainitiation").ToString());
				KarmaMetamagic = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmametamagic").ToString());
				KarmaComplexFormOption = Convert.ToInt32(Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("karmacomplexformoption").ToString());
				// Delete the Registry keys ones the values have been retrieve since they will no longer be used.
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmaattribute");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmaquality");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmaspecialization");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmanewknowledgeskill");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmanewactiveskill");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmanewskillgroup");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmaimproveknowledgeskill");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmaimproveactiveskill");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmaimproveskillgroup");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmaspell");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmanewcomplexform");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmaimprovecomplexform");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmanuyenper");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmacontact");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmacarryover");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmaspirit");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmamaneuver");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmainitiation");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmametamagic");
				Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("karmacomplexformoption");
			}
			catch
			{
			}

			// Retrieve the sourcebooks that are in the Registry.
			string strBookList = "";
			try
			{
				strBookList = Registry.CurrentUser.CreateSubKey("Software\\Chummer5").GetValue("books").ToString();
			}
			catch
			{
				// We were unable to get the Registry key which means the book options have not been saved yet, so create the default values.
				strBookList = "Shadowrun 5th Edition";
				RegistryKey objRegistry = Registry.CurrentUser.CreateSubKey("Software\\Chummer5");
				objRegistry.SetValue("books", strBookList);
			}
			string[] strBooks = strBookList.Split(',');

			XmlDocument objXmlDocument = new XmlDocument();
			objXmlDocument = XmlManager.Instance.Load("books.xml");

			foreach (string strBookCode in strBooks)
			{
				XmlNode objXmlBook = objXmlDocument.SelectSingleNode("/chummer/books/book[name = \"" + strBookCode + "\"]");
				try
				{
					Books.Add(objXmlBook["code"].InnerText);
				}
				catch
				{
				}
			}

			// Delete the Registry keys ones the values have been retrieve since they will no longer be used.
			Registry.CurrentUser.CreateSubKey("Software\\Chummer5").DeleteValue("books");
		}

		/// <summary>
		/// Convert a book code into the full name.
		/// </summary>
		/// <param name="strCode">Book code to convert.</param>
		public string BookFromCode(string strCode)
		{
			string strReturn = "";
			XmlNode objXmlBook = _objBookDoc.SelectSingleNode("/chummer/books/book[code = \"" + strCode + "\"]");
			try
			{
				strReturn = objXmlBook["name"].InnerText;
			}
			catch
			{
			}
			return strReturn;
		}

		/// <summary>
		/// Book code (using the translated version if applicable).
		/// </summary>
		/// <param name="strCode">Book code to search for.</param>
		public string LanguageBookShort(string strCode)
		{
			if (strCode == "")
				return "";

			string strReturn = "";
			XmlNode objXmlBook = _objBookDoc.SelectSingleNode("/chummer/books/book[code = \"" + strCode + "\"]");
			try
			{
				if (objXmlBook["altcode"] != null)
					strReturn = objXmlBook["altcode"].InnerText;
				else
					strReturn = strCode;
			}
			catch
			{
			}
			return strReturn;
		}

		/// <summary>
		/// Determine the book's original code by using the alternate code.
		/// </summary>
		/// <param name="strCode">Alternate code to look for.</param>
		public string BookFromAltCode(string strCode)
		{
			if (strCode == "")
				return "";

			XmlNode objXmlBook = _objBookDoc.SelectSingleNode("/chummer/books/book[altcode = \"" + strCode + "\"]");
			if (objXmlBook == null)
				return strCode;
			else
				return objXmlBook["code"].InnerText;
		}

		/// <summary>
		/// Book name (using the translated version if applicable).
		/// </summary>
		/// <param name="strCode">Book code to search for.</param>
		public string LanguageBookLong(string strCode)
		{
			if (strCode == "")
				return "";

			string strReturn = "";
			XmlNode objXmlBook = _objBookDoc.SelectSingleNode("/chummer/books/book[code = \"" + strCode + "\"]");
			try
			{
				if (objXmlBook["translate"] != null)
					strReturn = objXmlBook["translate"].InnerText;
				else
					strReturn = objXmlBook["name"].InnerText;
			}
			catch
			{
			}
			return strReturn;
		}

		/// <summary>
		/// Determine whether or not a given book is in use.
		/// </summary>
		/// <param name="strCode">Book code to search for.</param>
		public bool BookEnabled(string strCode)
		{
			bool blnReturn = false;
			foreach (string strBook in Books)
			{
				if (strBook == strCode)
				{
					blnReturn = true;
					break;
				}
			}
			return blnReturn;
		}

		/// <summary>
		/// XPath query used to filter items based on the user's selected source books.
		/// </summary>
		public string BookXPath()
		{
			if (_strBookXPath != "")
				return _strBookXPath;

			string strPath = "(";

			foreach (string strBook in Books)
			{
				if (strBook != "")
					strPath += "source = \"" + strBook + "\" or ";
			}
			strPath = strPath.Substring(0, strPath.Length - 4) + ")";

			if (GlobalOptions.Instance.MissionsOnly)
			{
				strPath += " and not(nomission)";
			}

			if (!GlobalOptions.Instance.Dronemods)
			{
				strPath += " and not(optionaldrone)";
			}
			_strBookXPath = strPath;
			
			return strPath;
		}

		public List<string> BookLinq()
		{
			return Books;
		}
		#endregion

		#region Rule Settings
		#region Chummer Options
		/// <summary>
		/// Whether or not confirmation messages are shown when deleting an object.
		/// </summary>
		[OptionAttributes("Chummer Options")]
		[SavePropertyAs("confirmdelete")]
		[DisplayConfiguration("Checkbox_Options_ConfirmDelete")]
		public bool ConfirmDelete { get; set; } = true;

	    /// <summary>
		/// Wehther or not confirmation messages are shown for Karma Expenses.
		/// </summary>
		[SavePropertyAs("confirmkarmaexpense")]
		[DisplayConfiguration("Checkbox_Options_ConfirmKarmaExpense")]
		public bool ConfirmKarmaExpense { get; set; } = true;

	    /// <summary>
		/// Number of Limbs a standard character has.
		/// </summary>
		[SavePropertyAs("limbcount")]
		//TODO: Handler for comboboxes
		public int LimbCount { get; set; } = 6;

	    /// <summary>
		/// Exclude a particular Limb Slot from count towards the Limb Count.
		/// </summary>
		[SavePropertyAs("excludelimbslot")]
		//TODO: Handler for comboboxes
		public string ExcludeLimbSlot { get; set; } = "";

	    /// <summary>
		/// Whether or not a backup copy of the character should be created before they are placed into Career Mode.
		/// </summary>
		[SavePropertyAs("createbackuponcareer")]
		[DisplayConfiguration("Checkbox_Options_CreateBackupOnCareer")]
		public bool CreateBackupOnCareer { get; set; }

	    /// <summary>
		/// Number of decimal places to round to when calculating Essence.
		/// </summary>
		[SavePropertyAs("essencedecimals")]
		public int EssenceDecimals { get; set; } = 2;

	    /// <summary>
		/// Default build method.
		/// </summary>
		[SavePropertyAs("buildmethod")]
		public string BuildMethod { get; set; } = "Karma";

	    /// <summary>
		/// Default number of build points.
		/// </summary>
		[SavePropertyAs("buildpoints")]
		public int BuildPoints { get; set; } = 800;

	    /// <summary>
		/// Default Availability.
		/// </summary>
		[SavePropertyAs("availability")]
		public int Availability { get; set; } = 12;

	    /// <summary>
		/// Whether Life Modules should automatically generate a character background.
		/// </summary>
		[SavePropertyAs("autobackstory")]
		public bool AutomaticBackstory { get; internal set; } = true;

	    #region Printing
		/// <summary>
		/// Whether or not all Active Skills with a total score higher than 0 should be printed.
		/// </summary>
		[OptionAttributes("Chummer Options/Printing")]
		[DisplayConfiguration("Checkbox_Options_PrintAllSkills")]
		[SavePropertyAs("printzeroratingskills")]
		public bool PrintSkillsWithZeroRating { get; set; } = true;

		/// <summary>
		/// Whether or not the Karma and Nueyn Expenses should be printed on the character sheet.
		/// </summary>
		[SavePropertyAs("printexpenses")]
		[DisplayConfiguration("Checkbox_Options_PrintExpenses")]
		public bool PrintExpenses { get; set; }

	    /// <summary>
		/// Whether or not Notes should be printed.
		/// </summary>
		[SavePropertyAs("printnotes")]
		[DisplayConfiguration("Checkbox_Options_PrintNotes")]
		public bool PrintNotes { get; set; }

	    #endregion
		#endregion
		#region Optional Rules
		/// <summary>
		/// Whether or not the More Lethal Gameplay optional rule is enabled.
		/// </summary>
		[OptionAttributes("Optional Rules")]
	    [DisplayConfiguration("Checkbox_Options_MoreLethalGameplay")]
		[SavePropertyAs("morelethalgameplay")]
		public bool MoreLethalGameplay { get; set; }

	    /// <summary>
		/// Whether or not to require licensing restricted items.
		/// </summary>
		[DisplayConfiguration("Checkbox_Options_LicenseRestricted")]
		[SavePropertyAs("licenserestricted")]
		public bool LicenseRestricted { get; set; }

	    /// <summary>
		/// Whether or not a Spirit's Maximum Force is based on the character's total MAG.
		/// </summary>
		[DisplayConfiguration("Checkbox_Options_MaxSpiritForce")]
		[SavePropertyAs("spiritforcebasedontotalmag")]
		public bool SpiritForceBasedOnTotalMAG { get; set; }

	    /// <summary>
		/// Whether or not Knucks benefit from improvements to Unarmed DV, such as Adept Powers.
		/// </summary>
		[SavePropertyAs("knucksuseunarmed")]
		[DisplayConfiguration("Checkbox_Options_Knucks")]
		public bool KnucksUseUnarmed { get; set; }

	    /// <summary>
		/// Whether or not characters may use Initiation/Submersion in Create mode.
		/// </summary>
		[SavePropertyAs("allowinitiationincreatemode")]
		[DisplayConfiguration("Checkbox_Options_AllowInitiation")]
		public bool AllowInitiationInCreateMode { get; set; }

	    /// <summary>
		/// Whether or not Defaulting on a Skill should include any Modifiers.
		/// </summary>
		[DisplayConfiguration("Checkbox_Options_DefaultIncludeModifiers")]
		[SavePropertyAs("skilldefaultingincludesmodifiers")]
		public bool SkillDefaultingIncludesModifiers { get; set; }

	    /// <summary>
		/// Whether or not Essence loss only reduces MAG/RES maximum value, not the current value.
		/// </summary>
		[SavePropertyAs("esslossreducesmaximumonly")]
		[DisplayConfiguration("Checkbox_Options_EssenceLossReducesMaximum")]
		public bool ESSLossReducesMaximumOnly { get; set; }

	    /// <summary>
		/// Whether or not characters are allowed to put points into a Skill Group again once it is broken and all Ratings are the same.
		/// </summary>
		[SavePropertyAs("allowskillregrouping")]
		[DisplayConfiguration("Checkbox_Options_SkillRegroup")]
		public bool AllowSkillRegrouping { get; set; } = true;

	    /// <summary>
		/// Allow Cyberware Essence cost discounts.
		/// </summary>
		[SavePropertyAs("allowcyberwareessdiscounts")]
		[DisplayConfiguration("Checkbox_Options_AllowCyberwareESSDiscounts")]
		public bool AllowCyberwareESSDiscounts { get; set; }

	    /// <summary>
		/// Whether or not a character's Strength affects Weapon Recoil.
		/// </summary>
		[SavePropertyAs("strengthaffectsrecoil")]
		[DisplayConfiguration("Checkbox_Options_StrengthAffectsRecoil")]
		public bool StrengthAffectsRecoil { get; set; }

	    /// <summary>
		/// Whether or not Maximum Armor Modifications is in use.
		/// </summary>
		[SavePropertyAs("maximumarmormodifications")]
		[DisplayConfiguration("Checkbox_Options_MaximumArmorModifications")]
		public bool MaximumArmorModifications { get; set; }

	    /// <summary>
		/// Whether or not Armor Suit Capacity is in use.
		/// </summary>
		[SavePropertyAs("armorsuitcapacity")]
		[DisplayConfiguration("Checkbox_Options_ArmorSuitCapacity")]
		public bool ArmorSuitCapacity { get; set; }

	    /// <summary>
		/// Whether or not Armor Degredation is allowed.
		/// </summary>
		[SavePropertyAs("armordegredation")]
		[DisplayConfiguration("Checkbox_Options_ArmorDegradation")]
		public bool ArmorDegradation { get; set; }

	    /// <summary>
		/// Whether or not the Karma cost for increasing Special Attributes is based on the shown value instead of actual value.
		/// </summary>
		[SavePropertyAs("specialkarmacostbasedonshownvalue")]
		[DisplayConfiguration("Checkbox_Options_SpecialKarmaCostBasedOnShownValue")]
		public bool SpecialKarmaCostBasedOnShownValue { get; set; }

	    /// <summary>
		/// Whether or not characters can have more than 25 BP in Positive Qualities.
		/// </summary>
		[SavePropertyAs("exceedpositivequalities")]
		[DisplayConfiguration("Checkbox_Options_ExceedPositiveQualities")]
		public bool ExceedPositiveQualities { get; set; }

	    /// <summary>
		/// Whether or not characters can have more than 25 BP in Negative Qualities.
		/// </summary>
		[SavePropertyAs("exceednegativequalities")]
		[DisplayConfiguration("Checkbox_Options_ExceedNegativeQualities")]
		public bool ExceedNegativeQualities { get; set; }

	    /// <summary>
		/// If true, the character will not receive additional BP from Negative Qualities past the initial 25
		/// </summary>
		[SavePropertyAs("exceednegativequalitieslimit")]
		[DisplayConfiguration("Checkbox_Options_ExceedNegativeQualitiesLimit")]
		public bool ExceedNegativeQualitiesLimit { get; set; }

	    /// <summary>
		/// Whether or not Restricted items have their cost multiplied.
		/// </summary>
		[SavePropertyAs("multiplyrestrictedcost")]
		[DisplayConfiguration("Checkbox_Options_MultiplyRestrictedCost")]
		public bool MultiplyRestrictedCost { get; set; }

	    /// <summary>
		/// Cost multiplier for Restricted items.
		/// </summary>
		[SavePropertyAs("restrictedcostmultiplier")]
		public int RestrictedCostMultiplier { get; set; } = 1;

	    /// <summary>
		/// Whether or not Forbidden items have their cost multiplied.
		/// </summary>
		[SavePropertyAs("multiplyforbiddencost")]
		[DisplayConfiguration("Checkbox_Options_MultiplyForbiddenCost")]
		public bool MultiplyForbiddenCost { get; set; }

	    /// <summary>
		/// Cost multiplier for Forbidden items.
		/// </summary>
		[SavePropertyAs("forbiddencostmultiplier")]
		public int ForbiddenCostMultiplier { get; set; } = 1;

	    /// <summary>
		/// Whether or not total Skill ratings are capped at 20 or 2 x natural Attribute + Rating, whichever is higher.
		/// </summary>
		[OptionAttributes("Optional Rules/SR4")]
		[SavePropertyAs("capskillrating")]
		[DisplayConfiguration("Checkbox_Options_LimitSkills")]
		public bool CapSkillRating { get; set; }

	    /// <summary>
		/// Whether to use the rules from SR4 to calculate Public Awareness.
		/// </summary>
		[SavePropertyAs("usecalculatedpublicawareness")]
		[DisplayConfiguration("Checkbox_Options_UseCalculatedPublicAwareness")]
		public bool UseCalculatedPublicAwareness { get; set; }

	    #endregion
		#region House Rules
		/// <summary>
		/// Whether or not characters can spend skill points on broken groups.
		/// </summary>
		[OptionAttributes("House Rules")]
		[SavePropertyAs("usepointsonbrokengroups")]
		[DisplayConfiguration("Checkbox_Options_PointsOnBrokenGroups")]
		public bool UsePointsOnBrokenGroups { get; set; }

	    /// <summary>
		/// Whether or not to ignore the art requirements from street grimoire.
		/// </summary>
		[SavePropertyAs("ignoreart")]
		[DisplayConfiguration("Checkbox_Options_IgnoreArt")]
		public bool IgnoreArtRequirements { get; set; }

	    /// <summary>
		/// Whether or not to use stats from Cyberlegs when calculating movement rates
		/// </summary>
		[SavePropertyAs("cyberlegmovement")]
		[DisplayConfiguration("Checkbox_Options_CyberlegMovement")]
		public bool CyberlegMovement { get; set; }

	    /// <summary>
		/// The Drone Body multiplier for maximal Armor //TODO: Link the enabled state to DroneArmorMultiplierEnabled.
		/// </summary>
		[SavePropertyAs("dronearmorflatnumber")]
		[DisplayConfiguration("Checkbox_Options_DroneArmorMultiplier")]
		public int DroneArmorMultiplier { get; set; } = 2;

	    /// <summary>
		/// Whether or not the DroneArmorMultiplier house rule is enabled. //TODO: Link DroneArmorMultiplier to the enabled state. Redundant?
		/// </summary>
		[SavePropertyAs("dronearmormultiplierenabled")]
		[DisplayConfiguration("Checkbox_Options_DroneArmorMultiplier")]
		public bool DroneArmorMultiplierEnabled { get; set; }


	    /// <summary>
		/// Whether or not Capacity limits should be enforced.
		/// </summary>
		[SavePropertyAs("enforcecapacity")]
		[DisplayConfiguration("Checkbox_Options_EnforceCapacity")]
		public bool EnforceCapacity { get; set; } = true;

	    /// <summary>
		/// Whether or not Recoil modifiers are restricted (AR 148).
		/// </summary>
		[SavePropertyAs("restrictrecoil")]
		[DisplayConfiguration("Checkbox_Options_RestrictRecoil")]
		public bool RestrictRecoil { get; set; } = true;

	    /// <summary>
		/// Whether or not characters are unresicted in the number of points they can invest in Nuyen.
		/// </summary>
		[SavePropertyAs("unrestrictednuyen")]
		[DisplayConfiguration("Checkbox_Options_UnrestrictedNuyen")]
		public bool UnrestrictedNuyen { get; set; }

	    /// <summary>
		/// Whether or not the user can change the Part of Base Weapon flag for a Weapon Accessory or Mod.
		/// </summary>
		[SavePropertyAs("alloweditpartofbaseweapon")]
		[DisplayConfiguration("Checkbox_Options_AllowEditPartOfBaseWeapon")]
		public bool AllowEditPartOfBaseWeapon { get; set; }

	    /// <summary>
		/// Whether or not the user can mark any piece of Bioware as being Transgenic.
		/// </summary>
		[SavePropertyAs("allowcustomtransgenics")]
		[DisplayConfiguration("Checkbox_Options_AllowCustomTransgenics")]
		public bool AllowCustomTransgenics { get; set; }

	    /// <summary>
		/// Whether or not the user is allowed to break Skill Groups while in Create Mode.
		/// </summary>
		[SavePropertyAs("breakskillgroupsincreatemode")]
		[DisplayConfiguration("Checkbox_Options_StrictSkillGroups")]
		public bool StrictSkillGroupsInCreateMode { get; set; }

	    /// <summary>
		/// Whether or not any Detection Spell can be taken as Extended range version.
		/// </summary>
		[SavePropertyAs("extendanydetectionspell")]
		[DisplayConfiguration("Checkbox_Options_ExtendAnyDetectionSpell")]
		public bool ExtendAnyDetectionSpell { get; set; }

	    /// <summary>
		/// Whether or not dice rolling is allowed for Skills.
		/// </summary>
		[SavePropertyAs("allowskilldicerolling")]
		[DisplayConfiguration("Checkbox_Options_AllowSkillDiceRolling")]
		public bool AllowSkillDiceRolling { get; set; }

	    /// <summary>
		/// Whether or not cyberlimbs stats are used in attribute calculation
		/// </summary>
		[SavePropertyAs("dontusecyberlimbcalculation")]
		[DisplayConfiguration("Checkbox_Options_UseCyberlimbCalculation")]
		public bool DontUseCyberlimbCalculation { get; set; }

	    /// <summary>
		/// Whether or not characters in Career Mode should pay double for qualities.
		/// </summary>
		[OptionAttributes("House Rules/Qualities")]
		[SavePropertyAs("dontdoublequalities")]
		[DisplayConfiguration("Checkbox_Options_DontDoubleQualityPurchases")]
		public bool DontDoubleQualityPurchases { get; set; }

	    /// <summary>
		/// Whether or not characters in Career Mode should pay double for removing Negative Qualities.
		/// </summary>
		[SavePropertyAs("dontdoublequalityrefunds")]
		[DisplayConfiguration("Checkbox_Options_DontDoubleNegativeQualityRefunds")]
		public bool DontDoubleQualityRefunds { get; set; }

	    /// <summary>
		/// Whether or not Obsolescent can be removed/upgraded in the same way as Obsolete.
		/// </summary>
		[SavePropertyAs("allowobsolescentupgrade")]
		[DisplayConfiguration("Checkbox_Options_AllowObsolescentUpgrade")]
		public bool AllowObsolescentUpgrade { get; set; }

	    /// <summary>
		/// Whether or not Bioware Suites can be added and created.
		/// </summary>
		[SavePropertyAs("allowbiowaresuites")]
		[DisplayConfiguration("Checkbox_Options_AllowBiowareSuites")]
		public bool AllowBiowareSuites { get; set; }

	    /// <summary>
		/// House rule: Free Spirits calculate their Power Points based on their MAG instead of EDG.
		/// </summary>
		[SavePropertyAs("freespiritpowerpointsmag")]
		[DisplayConfiguration("Checkbox_Options_FreeSpiritPowerPointsMAG")]
		public bool FreeSpiritPowerPointsMAG { get; set; }

	    /// <summary>
		/// Whether or not Technomancers can select Autosofts as Complex Forms.
		/// </summary>
		[SavePropertyAs("technomancerallowautosoft")]
		[DisplayConfiguration("Checkbox_Options_TechnomancerAllowAutosoft")]
		public bool TechnomancerAllowAutosoft { get; set; }

	    #region Character Creation
		/// <summary>
		/// The CHA multiplier to be used with the Free Contacts Option.
		/// </summary>
		/// //TODO: Link the enabled state to FreeContactsMultiplierEnabled.
		[OptionAttributes("House Rules/Character Creation")]
		[SavePropertyAs("freekarmacontactsmultiplier")]
		[DisplayConfiguration("Checkbox_Options_ContactMultiplier")]
		public int FreeContactsMultiplier { get; set; } = 3;

	    /// <summary>
		/// Whether or not characters get a flat number of BP for free Contacts.
		/// </summary>
		/// //TODO: Link FreeContactsMultiplier to the enabled state. Redundant?
		[SavePropertyAs("freecontactsmultiplierenabled")]
		[DisplayConfiguration("Checkbox_Options_ContactMultiplier")]
		public bool FreeContactsMultiplierEnabled { get; set; }

	    /// <summary>
		/// Whether or not characters in Karma build mode receive free Knowledge Skills in the same manner as Priority characters.
		/// </summary>
		[SavePropertyAs("freekarmacontacts")]
		[DisplayConfiguration("Checkbox_Options_FreeKarmaContacts")]
		public bool FreeKarmaContacts { get; set; }

	    /// <summary>
		/// Whether or not characters in Karma build mode receive free Knowledge Skills in the same manner as Priority characters.
		/// </summary>
		[SavePropertyAs("freekarmaknowledge")]
		[DisplayConfiguration("Checkbox_Options_FreeKnowledgeSkills")]
		public bool FreeKarmaKnowledge { get; set; }

	    /// <summary>
		/// Whether or not the multiplier for Free Knowledge points are used.
		/// </summary>
		[SavePropertyAs("freekarmaknowledgemultiplierenabled")]
		[DisplayConfiguration("Checkbox_Options_KnowledgeMultiplier")]
		public bool FreeKnowledgeMultiplierEnabled { get; set; }

	    /// <summary>
		/// The INT+LOG multiplier to be used with the Free Knowledge Option.
		/// </summary>
		[SavePropertyAs("freekarmaknowledgemultiplier")]
		[DisplayConfiguration("Checkbox_Options_KnowledgeMultiplier")]
		public int FreeKnowledgeMultiplier { get; set; } = 2;

	    /// <summary>
		/// Whether or not Metatypes cost Karma.
		/// </summary>
		[SavePropertyAs("metatypecostskarma")]
		[DisplayConfiguration("Checkbox_Options_MetatypeCostsKarma")]
		public bool MetatypeCostsKarma { get; set; } = true;

	    /// <summary>
		/// Mutiplier for Metatype Karma Costs.
		/// </summary>
		[SavePropertyAs("metatypecostskarma")]
		[DisplayConfiguration("Checkbox_Options_MetatypeCostsKarma")]
		public int MetatypeCostsKarmaMultiplier { get; set; } = 1;

	    /// <summary>
		/// House rule: Treat the Metatype Attribute Minimum as 1 for the purpose of calculating Karma costs.
		/// </summary>
		[SavePropertyAs("alternatemetatypeattributekarma")]
		[DisplayConfiguration("Checkbox_Options_AlternateMetatypeAttributeKarma")]
		public bool AlternateMetatypeAttributeKarma { get; set; }

	    /// <summary>
		/// Maximum amount of remaining Karma that is carried over to the character once they are created.
		/// </summary>
		[SavePropertyAs("karmacarryover")]
		[DisplayConfiguration("Label_Options_Carryover")]
		public int KarmaCarryover { get; set; } = 7;

	    /// <summary>
		/// Amount of Nuyen gained per Karma spent.
		/// </summary>
		[SavePropertyAs("nuyenperbp")]
		[DisplayConfiguration("Label_Options_Nuyen")]
		public int NuyenPerBP { get; set; } = 2000;


	    /// <summary>
		/// Whether you benefit from augmented values for contact points.
		/// </summary>
		[SavePropertyAs("usetotalvalueforcontacts")]
		[DisplayConfiguration("Checkbox_Options_UseTotalValueForFreeContacts")]
		public bool UseTotalValueForFreeContacts { get; set; }

	    /// <summary>
		/// Whether you benefit from augmented values for free knowledge points.
		/// </summary>
		[SavePropertyAs("usetotalvalueforknowledge")]
		[DisplayConfiguration("Checkbox_Options_UseTotalValueForFreeKnowledge")]
		public bool UseTotalValueForFreeKnowledge { get; set; }

	    #endregion

		#endregion

		#region Karma Costs
		/// <summary>
		/// Karma cost to improve an Attribute = New Rating X this value.
		/// </summary>
		[OptionAttributes("Karma Costs")]
		[SavePropertyAs("karmaattribute")]
		[DisplayConfiguration("Label_Options_ImproveAttribute")]
		public int KarmaAttribute { get; set; } = 5;

	    /// <summary>
		/// Karma cost to purchase a Quality = BP Cost x this value.
		/// </summary>
		[SavePropertyAs("karmaquality")]
		[DisplayConfiguration("Label_Options_Qualities")]
		public int KarmaQuality { get; set; } = 1;

	    /// <summary>
		/// Karma cost for a Contact = (Connection + Loyalty) x this value.
		/// </summary>
		[SavePropertyAs("karmacontact")]
		[DisplayConfiguration("Label_Options_KarmaContact")]
		public int KarmaContact { get; set; } = 1;

	    /// <summary>
		/// Karma cost for an Enemy = (Connection + Loyalty) x this value.
		/// </summary>
		[SavePropertyAs("karmaenemy")]
		[DisplayConfiguration("Label_Options_Enemies")]
		public int KarmaEnemy { get; set; } = 1;

	    /// <summary>
		/// Karma cost for a Combat Maneuver = this value.
		/// </summary>
		[SavePropertyAs("karmamaneuver")]
		[DisplayConfiguration("Label_Options_KarmaMartialArtManeuver")]
		public int KarmaManeuver { get; set; } = 5;

	    #region Skills
		/// <summary>
		/// Karma cost to purchase a Specialization = this value.
		/// </summary>
		[SavePropertyAs("karmaspecialization")]
		[DisplayConfiguration("Label_Options_KarmaSkillSpecialization")]
		public int KarmaSpecialization { get; set; } = 7;

	    /// <summary>
		/// Karma cost to purchase a new Knowledge Skill = this value.
		/// </summary>
		[SavePropertyAs("karmanewknowledgeskill")]
		[DisplayConfiguration("Label_Options_KarmaKnowledgeSkill")]
		public int KarmaNewKnowledgeSkill { get; set; } = 1;

	    /// <summary>
		/// Karma cost to purchase a new Active Skill = this value.
		/// </summary>
		[SavePropertyAs("karmanewactiveskill")]
		[DisplayConfiguration("Label_Options_KarmaActiveSkill")]
		public int KarmaNewActiveSkill { get; set; } = 2;

	    /// <summary>
		/// Karma cost to purchase a new Skill Group = this value.
		/// </summary>
		[SavePropertyAs("karmanewskillgroup")]
		[DisplayConfiguration("Label_Options_KarmaSkillGroup")]
		public int KarmaNewSkillGroup { get; set; } = 5;

	    /// <summary>
		/// Karma cost to improve a Knowledge Skill = New Rating x this value.
		/// </summary>
		[SavePropertyAs("karmaimproveknowledgeskill")]
		[DisplayConfiguration("Label_Options_ImproveKnowledgeSkill")]
		public int KarmaImproveKnowledgeSkill { get; set; } = 1;

	    /// <summary>
		/// Karma cost to improve an Active Skill = New Rating x this value.
		/// </summary>
		[SavePropertyAs("karmaimproveactiveskill")]
		[DisplayConfiguration("Label_Options_ImproveActiveSkill")]
		public int KarmaImproveActiveSkill { get; set; } = 2;

	    /// <summary>
		/// Karma cost to improve a Skill Group = New Rating x this value.
		/// </summary>
		[SavePropertyAs("karmaimproveskillgroup")]
		[DisplayConfiguration("Label_Options_ImproveSkillGroup")]
		public int KarmaImproveSkillGroup { get; set; } = 5;

	    #endregion
		#region Magic
		/// <summary>
		/// Karma cost for each Spell = this value.
		/// </summary>
		[SavePropertyAs("karmaspell")]
		[DisplayConfiguration("Label_Options_KarmaSpell")]
		public int KarmaSpell { get; set; } = 5;

	    /// <summary>
		/// Karma cost for each Enhancement = this value.
		/// </summary>
		[SavePropertyAs("karmaenhancement")]
		[DisplayConfiguration("String_Enhancement")]
		public int KarmaEnhancement { get; set; } = 2;

	    /// <summary>
		/// Karma cost for a Spirit = this value.regis
		/// </summary>
		[SavePropertyAs("karmaspirit")]
		[DisplayConfiguration("Label_Options_KarmaSpirit")]
		public int KarmaSpirit { get; set; } = 1;

	    /// <summary>
		/// Karma cost for a Initiation = 10 + (New Rating x this value).
		/// </summary>
		[SavePropertyAs("karmainitiation")]
		[DisplayConfiguration("Tab_Initiation")]
		public int KarmaInitiation { get; set; } = 3;

	    /// <summary>
		/// Karma cost for a Metamagic = this value.
		/// </summary>
		[SavePropertyAs("karmametamagic")]
		[DisplayConfiguration("String_Metamagic")]
		public int KarmaMetamagic { get; set; } = 15;

	    /// <summary>
		/// Karma cost to join a Group = this value.
		/// </summary>
		[SavePropertyAs("karmajoingroup")]
		[DisplayConfiguration("Label_Options_JoinGroup")]
		public int KarmaJoinGroup { get; set; } = 5;

	    /// <summary>
		/// Karma cost to leave a Group = this value.
		/// </summary>
		[SavePropertyAs("karmaleavegroup")]
		[DisplayConfiguration("Label_Options_LeaveGroup")]
		public int KarmaLeaveGroup { get; set; } = 1;

	    /// <summary>
		/// Karma cost for Alchemical Foci.
		/// </summary>
		[OptionAttributes("Karma Costs/Foci")]
		[SavePropertyAs("karmaalchemicalfocus")]
		[DisplayConfiguration("Label_Options_AlchemicalFocus")]
		public int KarmaAlchemicalFocus { get; set; } = 3;

	    /// <summary>
		/// Karma cost for Banishing Foci.
		/// </summary>
		[SavePropertyAs("karmabanishingfocus")]
		[DisplayConfiguration("Label_Options_BanishingFocus")]
		public int KarmaBanishingFocus { get; set; } = 2;

	    /// <summary>
		/// Karma cost for Binding Foci.
		/// </summary>
		[SavePropertyAs("karmabindingfocus")]
		[DisplayConfiguration("Label_Options_BindingFocus")]
		public int KarmaBindingFocus { get; set; } = 2;

	    /// <summary>
		/// Karma cost for Centering Foci.
		/// </summary>
		[SavePropertyAs("karmacenteringfocus")]
		[DisplayConfiguration("Label_Options_CenteringFocus")]
		public int KarmaCenteringFocus { get; set; } = 3;

	    /// <summary>
		/// Karma cost for Counterspelling Foci.
		/// </summary>
		[SavePropertyAs("karmacounterspellingfocus")]
		[DisplayConfiguration("Label_Options_CounterspellingFocus")]
		public int KarmaCounterspellingFocus { get; set; } = 2;

	    /// <summary>
		/// Karma cost for Disenchanting Foci.
		/// </summary>
		[SavePropertyAs("karmadisenchantingfocus")]
		[DisplayConfiguration("Label_Options_DisenchantingFocus")]
		public int KarmaDisenchantingFocus { get; set; } = 3;

	    /// <summary>
		/// Karma cost for Flexible Signature Foci.
		/// </summary>
		[SavePropertyAs("karmaflexiblesignaturefocus")]
		[DisplayConfiguration("Label_Options_FlexibleSignatureFocus")]
		public int KarmaFlexibleSignatureFocus { get; set; } = 3;

	    /// <summary>
		/// Karma cost for Masking Foci.
		/// </summary>
		[SavePropertyAs("karmamaskingfocus")]
		[DisplayConfiguration("Label_Options_MaskingFocus")]
		public int KarmaMaskingFocus { get; set; } = 3;

	    /// <summary>
		/// Karma cost for Power Foci.
		/// </summary>
		[SavePropertyAs("karmapowerfocus")]
		[DisplayConfiguration("Label_Options_PowerFocus")]
		public int KarmaPowerFocus { get; set; } = 6;

	    /// <summary>
		/// Karma cost for Qi Foci.
		/// </summary>
		[SavePropertyAs("karmaqifocus")]
		[DisplayConfiguration("Label_Options_QiFocus")]
		public int KarmaQiFocus { get; set; } = 2;

	    /// <summary>
		/// Karma cost for Ritual Spellcasting Foci.
		/// </summary>
		[SavePropertyAs("karmaritualspellcastingfocus")]
		[DisplayConfiguration("Label_Options_RitualSpellcastingFocus")]
		public int KarmaRitualSpellcastingFocus { get; set; } = 2;

	    /// <summary>
		/// Karma cost for Spellcasting Foci.
		/// </summary>
		[SavePropertyAs("karmaspellcastingfocus")]
		[DisplayConfiguration("Label_Options_SpellcastingFocus")]
		public int KarmaSpellcastingFocus { get; set; } = 2;

	    /// <summary>
		/// Karma cost for Spell Shaping Foci.
		/// </summary>
		[SavePropertyAs("karmaspellshapingfocus")]
		[DisplayConfiguration("Label_Options_SpellShapingFocus")]
		public int KarmaSpellShapingFocus { get; set; } = 3;

	    /// <summary>
		/// Karma cost for Summoning Foci.
		/// </summary>
		[SavePropertyAs("karmasummoningfocus")]
		[DisplayConfiguration("Label_Options_SummoningFocus")]
		public int KarmaSummoningFocus { get; set; } = 2;

	    /// <summary>
		/// Karma cost for Sustaining Foci.
		/// </summary>
		[SavePropertyAs("karmasustainingfocus")]
		[DisplayConfiguration("Label_Options_SustainingFocus")]
		public int KarmaSustainingFocus { get; set; } = 2;

	    /// <summary>
		/// Karma cost for Weapon Foci.
		/// </summary>
		[SavePropertyAs("karmaweaponfocus")]
		[DisplayConfiguration("Label_Options_WeaponFocus")]
		public int KarmaWeaponFocus { get; set; } = 3;

	    #endregion
		#region Complex Forms
		/// <summary>
		/// Karma cost for a new Complex Form = this value.
		/// </summary>
		[OptionAttributes("Karma Costs/Complex Forms")]
		[SavePropertyAs("karmanewcomplexform")]
		[DisplayConfiguration("Label_Options_NewComplexForm")]
		public int KarmaNewComplexForm { get; set; } = 4;

	    /// <summary>
		/// Karma cost to improve a Complex Form = New Rating x this value.
		/// </summary>
		[SavePropertyAs("karmaimprovecomplexform")]
		[DisplayConfiguration("Label_Options_ImproveComplexForm")]
		public int KarmaImproveComplexForm { get; set; } = 1;

	    /// <summary>
		/// Karma cost for Complex Form Options = Rating x this value.
		/// </summary>
		[SavePropertyAs("karmacomplexformoption")]
		[DisplayConfiguration("Label_Options_ComplexFormOptions")]
		public int KarmaComplexFormOption { get; set; } = 2;

	    /// <summary>
		/// Karma cost for Complex Form Skillsofts = Rating x this value.
		/// </summary>
		[SavePropertyAs("karmacomplexformskillsoft")]
		[DisplayConfiguration("Label_Options_ComplexFormSkillsoft")]
		public int KarmaComplexFormSkillsoft { get; set; } = 1;

	    #endregion
		#endregion
		#endregion

		/// <summary>
		/// Sourcebooks.
		/// </summary>
		public List<string> Books { get; } = new List<string>();

	    /// <summary>
		/// Setting name.
		/// </summary>
		public string Name { get; set; } = "Default Settings";

	    /// <summary>
		/// 
		/// </summary>
		public string RecentImageFolder { get; set; } = "";
	}
}