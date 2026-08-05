using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Proyecto26;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedTextManager
{
	private const string MISSING_LOCALE = "LOC_ER";

	private static List<string> _all_languages;

	private static List<string> _changed_languages;

	public static LocalizedTextManager instance;

	private static readonly Dictionary<string, string> _boat_strings = new Dictionary<string, string>();

	public static GameLanguageAsset current_language;

	private bool _lang_dirty;

	private Dictionary<string, string> _localized_text;

	private Dictionary<string, string> _localized_text_files;

	private Font _default_font;

	private Font _hindi_font;

	private Font _japanese_font;

	private Font _korean_font;

	private Font _arabic_font;

	private Font _persian_font;

	private Font _simplified_chinese_font;

	private Font _thai_font;

	public Font default_font;

	internal bool initiated;

	internal string language = "not_set";

	internal List<LocalizedText> texts;

	public static Font current_font => current_language?.font?.Invoke() ?? instance.default_font;

	public Font hindi_font
	{
		get
		{
			if ((object)_hindi_font == null)
			{
				_hindi_font = (Font)Resources.Load("Fonts/Poppins-Regular", typeof(Font));
			}
			return _hindi_font;
		}
	}

	public Font japanese_font
	{
		get
		{
			if ((object)_japanese_font == null)
			{
				_japanese_font = (Font)Resources.Load("Fonts/MPLUSRounded1c-Medium", typeof(Font));
			}
			return _japanese_font;
		}
	}

	public Font korean_font
	{
		get
		{
			if ((object)_korean_font == null)
			{
				_korean_font = (Font)Resources.Load("Fonts/NanumGothicCoding-Bold", typeof(Font));
			}
			return _korean_font;
		}
	}

	public Font persian_font
	{
		get
		{
			if ((object)_persian_font == null)
			{
				_persian_font = (Font)Resources.Load("Fonts/Vazirmatn-Bold", typeof(Font));
			}
			return _persian_font;
		}
	}

	public Font arabic_font
	{
		get
		{
			if ((object)_arabic_font == null)
			{
				_arabic_font = (Font)Resources.Load("Fonts/Tajawal-Bold", typeof(Font));
			}
			return _arabic_font;
		}
	}

	public Font simplified_chinese_font
	{
		get
		{
			if ((object)_simplified_chinese_font == null)
			{
				_simplified_chinese_font = (Font)Resources.Load("Fonts/NotoSansCJKsc-Bold", typeof(Font));
			}
			return _simplified_chinese_font;
		}
	}

	public Font thai_font
	{
		get
		{
			if ((object)_thai_font == null)
			{
				_thai_font = (Font)Resources.Load("Fonts/krubbold", typeof(Font));
			}
			return _thai_font;
		}
	}

	public static void init(string pLanguage = null)
	{
		if (instance == null)
		{
			instance = new LocalizedTextManager();
			instance.create();
			if (pLanguage == null)
			{
				pLanguage = PlayerConfig.dict["language"].stringVal;
			}
			instance.setLanguage(pLanguage);
		}
	}

	private void create()
	{
		default_font = (Font)Resources.Load("Fonts/Roboto-Bold", typeof(Font));
		instance = this;
		texts = new List<LocalizedText>();
	}

	public bool contains(string pString)
	{
		return instance._localized_text.ContainsKey(pString);
	}

	public static IEnumerable<string> getKeys()
	{
		return instance._localized_text.Keys;
	}

	public static void addTextField(LocalizedText pText)
	{
		instance.texts.Add(pText);
		instance._lang_dirty = true;
	}

	public static void removeTextField(LocalizedText pText)
	{
		if (instance != null)
		{
			instance.texts.Remove(pText);
		}
	}

	public static void updateTexts()
	{
		Debug.Log("LocalizedTextManager: total texts loaded: " + instance.texts.Count);
		foreach (LocalizedText text in instance.texts)
		{
			text.updateText();
		}
	}

	public static bool stringExists(string pKey)
	{
		if (string.IsNullOrEmpty(pKey))
		{
			return false;
		}
		if (instance._localized_text.ContainsKey(pKey))
		{
			return true;
		}
		return false;
	}

	public static string getText(string pKey, Text text = null, bool pForceEnglish = false)
	{
		if (instance.language == "boat")
		{
			return transformToBoat(pKey);
		}
		if (instance.language == "keys")
		{
			return transformToKeys(pKey);
		}
		string tResult;
		if (instance._localized_text.ContainsKey(pKey))
		{
			tResult = instance._localized_text[pKey];
		}
		else
		{
			tResult = pKey;
			if (pKey.Contains("_placeholder"))
			{
				tResult = "";
			}
			else if (AssetManager.missing_locale_keys.Add(pKey))
			{
				Debug.LogError("LocalizedTextManager: missing text: " + pKey, text);
				AssetManager.generateMissingLocalesFile();
			}
		}
		if (pKey.StartsWith("world_law_", StringComparison.Ordinal))
		{
			tResult = tResult.Replace("\n\n", "\n");
		}
		return tResult;
	}

	public static string transformToKeys(string pTextKey)
	{
		string tResult = string.Empty;
		if (instance._localized_text_files.ContainsKey(pTextKey))
		{
			tResult = instance._localized_text_files[pTextKey];
		}
		return tResult + ": " + pTextKey;
	}

	public static string transformToBoat(string pTextKey)
	{
		if (_boat_strings.ContainsKey(pTextKey))
		{
			return _boat_strings[pTextKey];
		}
		int tWords = pTextKey.Split(' ').Length + 1;
		string tResult = "";
		for (int i = 0; i < tWords; i++)
		{
			if (tResult.Length > 0)
			{
				tResult += " ";
			}
			tResult = ((!Randy.randomBool()) ? ((!Randy.randomBool()) ? ((!Randy.randomBool()) ? (tResult + "ye") : (tResult + "boat")) : (tResult + "Ahoy")) : ((!Randy.randomBool()) ? ((!Randy.randomBool()) ? (tResult + "Argh") : (tResult + "Aye")) : (tResult + "Boat")));
		}
		_boat_strings[pTextKey] = tResult;
		return _boat_strings[pTextKey];
	}

	public void loadLocalizedText(string pLocaleID)
	{
		initiated = true;
		instance._localized_text = new Dictionary<string, string>();
		instance._localized_text_files = new Dictionary<string, string>();
		string tFolderPath = "locales/" + pLocaleID;
		TextAsset[] tTextAssets;
		try
		{
			tTextAssets = Resources.LoadAll<TextAsset>(tFolderPath);
		}
		catch (Exception)
		{
			tTextAssets = Resources.LoadAll<TextAsset>("locales/en");
		}
		if (tTextAssets == null || tTextAssets.Length == 0)
		{
			tTextAssets = Resources.LoadAll<TextAsset>("locales/en");
		}
		TextAsset[] array = tTextAssets;
		foreach (TextAsset obj in array)
		{
			string tDataAsJson = obj.text;
			string tFileName = obj.name;
			Dictionary<string, string> tObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(tDataAsJson);
			foreach (string tKey in tObj.Keys)
			{
				add(tKey, tObj[tKey], pReplace: false, tFileName, pCheckForCharacters: false);
			}
		}
	}

	public static void add(string pKey, string pTranslation, bool pReplace = false, string pFileName = "", bool pCheckForCharacters = true)
	{
		pKey = pKey.Underscore();
		if (!pReplace && instance._localized_text.ContainsKey(pKey))
		{
			if (instance._localized_text[pKey] == pTranslation)
			{
				Debug.LogWarning("Skipped - already exists - " + pKey + " - exists : " + pTranslation);
				return;
			}
			Debug.LogError("Already exists - " + pKey + " - exists : " + instance._localized_text[pKey]);
			Debug.LogError("Already exists - " + pKey + " - skipped: " + pTranslation);
		}
		else
		{
			instance._localized_text[pKey] = pTranslation;
			instance._localized_text_files[pKey] = pFileName;
		}
	}

	public void setLanguage(string pLanguage)
	{
		if (!(language == pLanguage) || _lang_dirty)
		{
			_lang_dirty = false;
			Debug.Log("LOAD LANGUAGE " + pLanguage);
			string tPathCreatures = string.Concat("locales/" + pLanguage, "/creatures");
			if (pLanguage != "boat" && pLanguage != "keys" && (object)Resources.Load(tPathCreatures) == null)
			{
				pLanguage = PlayerConfig.detectLanguage();
			}
			bool tSave = false;
			if (language != "not_set")
			{
				tSave = true;
			}
			language = pLanguage;
			instance.loadLocalizedText(pLanguage);
			try
			{
				RestClient.DefaultRequestHeaders["wb-language"] = language ?? "na";
			}
			catch (Exception)
			{
			}
			current_language = AssetManager.game_language_library.get(language);
			DebugLocales.init();
			updateTexts();
			if (PlayerConfig.dict["language"].stringVal != pLanguage)
			{
				tSave = true;
			}
			PlayerConfig.dict["language"].stringVal = pLanguage;
			if (tSave)
			{
				PlayerConfig.saveData();
			}
		}
	}

	public static string langToCulture(string pLanguage = null)
	{
		if (pLanguage == null)
		{
			pLanguage = instance.language;
		}
		string tLang = pLanguage;
		if (tLang == "boat")
		{
			return "";
		}
		if (tLang == "keys")
		{
			return "";
		}
		if (tLang == "lb")
		{
			return "lb-LU";
		}
		if (tLang == "ka")
		{
			return "ka-GE";
		}
		if (tLang == "gr")
		{
			return "el-GR";
		}
		return tLang switch
		{
			"hr" => "hr-HR", 
			"by" => "be-BY", 
			"ch" => "zh-Hant", 
			"cz" => "zh-Hans", 
			"fn" => "fi-FI", 
			"ph" => "fil-PH", 
			"gr" => "fi", 
			"br" => "pt", 
			"ko" => "ko-KR", 
			"th" => "th-TH", 
			"ua" => "uk", 
			"no" => "nb-NO", 
			"lt" => "lt", 
			"vn" => "vi", 
			_ => tLang, 
		};
	}

	public static CultureInfo getCulture(string pLanguage = null)
	{
		string tCultureLang = langToCulture(pLanguage);
		if (tCultureLang != "")
		{
			try
			{
				return new CultureInfo(tCultureLang);
			}
			catch (CultureNotFoundException)
			{
				return CultureInfo.CurrentCulture;
			}
		}
		return CultureInfo.CurrentCulture;
	}

	public static CultureInfo getCurrentCulture()
	{
		return CultureInfo.CurrentCulture;
	}

	public static bool cultureSupported()
	{
		switch (instance.language)
		{
		case "boat":
		case "hi":
		case "by":
		case "ka":
		case "lb":
			return false;
		default:
			return true;
		}
	}

	public static List<string> getAllLanguages()
	{
		if (_all_languages == null)
		{
			_all_languages = new List<string>();
			TextAsset[] array = Resources.LoadAll<TextAsset>("locales");
			foreach (TextAsset tLanguage in array)
			{
				_all_languages.Add(tLanguage.name);
			}
		}
		return _all_languages;
	}

	public static List<string> getAllLanguagesWithChanges()
	{
		if (_changed_languages == null)
		{
			_changed_languages = new List<string>();
			int tMaxLoadedLanguages = 10;
			TextAsset[] array = Resources.LoadAll<TextAsset>("locales");
			foreach (TextAsset tLanguageAsset in array)
			{
				string tLanguage = tLanguageAsset.name;
				string tFolder = TesterBehScreenshotFolder.getScreenshotFolder(tLanguage);
				if (tLanguage == "boat" || tLanguage == "keys")
				{
					continue;
				}
				if (File.Exists(tFolder + "/" + tLanguage + ".json"))
				{
					string tLastJson = File.ReadAllText(tFolder + "/" + tLanguage + ".json", Encoding.UTF8);
					string tLanguageJSON = tLanguageAsset.text;
					Debug.Log(tLanguageJSON.Length + " vs " + tLastJson.Length);
					if (tLastJson == tLanguageJSON)
					{
						Debug.Log("Language " + tLanguage + " has no changes");
						continue;
					}
					Debug.Log("Language " + tLanguage + " has changes");
					File.Delete(tFolder + "/" + tLanguage + ".json");
				}
				_changed_languages.Add(tLanguage);
				if (_changed_languages.Count >= tMaxLoadedLanguages)
				{
					break;
				}
			}
		}
		return _changed_languages;
	}
}
