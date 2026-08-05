using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using NeoModLoader.services;
using Newtonsoft.Json;

namespace NeoModLoader.General;

public static class LM
{
	private static Dictionary<string, Dictionary<string, string>> locales = new Dictionary<string, Dictionary<string, string>>();

	private static readonly Dictionary<string, string> str2esc = new Dictionary<string, string>
	{
		{ "\\n", "\n" },
		{ "\\r", "\r" },
		{ "\\t", "\t" },
		{ "\\b", "\b" },
		{ "\\f", "\f" },
		{ "\\\"", "\"" },
		{ "\\'", "'" },
		{ "\\\\", "\\" },
		{ "\\0", "\0" }
	};

	[MethodImpl(MethodImplOptions.Synchronized | MethodImplOptions.AggressiveInlining)]
	public static string Get(string key)
	{
		return LocalizedTextManager.getText(key);
	}

	public static bool Has(string key, string lang = "")
	{
		Dictionary<string, string> value;
		return string.IsNullOrEmpty(lang) ? LocalizedTextManager.instance._localized_text.ContainsKey(key) : (locales.TryGetValue(lang, out value) && value.ContainsKey(key));
	}

	public static void LoadLocales(string pFilePath, char pSep = ',')
	{
		if (pFilePath.ToLower().EndsWith(".csv"))
		{
			Dictionary<string, Dictionary<string, string>> dictionary = null;
			try
			{
				dictionary = ParseCSV(File.ReadAllText(pFilePath), pSep);
			}
			catch (Exception ex)
			{
				LogService.LogWarning("Failed to load locale file at " + pFilePath + " as csv: " + ex.Message);
				return;
			}
			if (dictionary != null)
			{
				foreach (string key in dictionary.Keys)
				{
					Dictionary<string, string> dictionary2 = dictionary[key];
					foreach (string key2 in dictionary2.Keys)
					{
						Add(key, key2, dictionary2[key2]);
					}
				}
				return;
			}
			LogService.LogWarning("Failed to load locale file at " + pFilePath + " as csv");
		}
		else
		{
			LogService.LogWarning("Unsupported locale file type of path: " + pFilePath);
		}
	}

	public static void LoadLocales(Stream pStream, char pSep = ',')
	{
		string text = new StreamReader(pStream).ReadToEnd();
		Dictionary<string, Dictionary<string, string>> dictionary = null;
		try
		{
			dictionary = ParseCSV(text, pSep);
		}
		catch (Exception ex)
		{
			LogService.LogWarning("Failed to load locale text \"" + text + "\" as csv: " + ex.Message);
			return;
		}
		if (dictionary == null)
		{
			LogService.LogWarning("Failed to load locale text \"" + text + "\" as csv");
			return;
		}
		foreach (string key in dictionary.Keys)
		{
			Dictionary<string, string> dictionary2 = dictionary[key];
			foreach (string key2 in dictionary2.Keys)
			{
				Add(key, key2, dictionary2[key2]);
			}
		}
	}

	private static Dictionary<string, Dictionary<string, string>> ParseCSV(string pText, char sep)
	{
		pText = pText.Replace("\r\n", "\n");
		string[] array = pText.Split('\n');
		if (array.Length < 2)
		{
			return null;
		}
		if (string.IsNullOrEmpty(array[0].Trim()))
		{
			return null;
		}
		if (!array[0].Contains(sep))
		{
			return null;
		}
		string[] array2 = array[0].Split(sep);
		Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
		for (int i = 1; i < array2.Length; i++)
		{
			dictionary[array2[i]] = new Dictionary<string, string>();
		}
		for (int j = 1; j < array.Length; j++)
		{
			if (string.IsNullOrEmpty(array[j].Trim()) || !array[j].Contains(sep))
			{
				continue;
			}
			string[] array3 = str2esc.Keys.Aggregate(array[j], (string current, string key) => current.Replace(key, str2esc[key])).Split(sep);
			string text = array3[0];
			if (!string.IsNullOrEmpty(text))
			{
				if (array3.Length > array2.Length)
				{
					throw new Exception($"Line {j} has more ',' than its head.");
				}
				for (int num = 1; num < array3.Length; num++)
				{
					dictionary[array2[num]][text] = array3[num];
				}
			}
		}
		return dictionary;
	}

	public static void LoadLocale(string pLanguage, Stream pStream)
	{
		string text = new StreamReader(pStream).ReadToEnd();
		Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
		if (dictionary == null)
		{
			throw new FormatException("Failed to load locale file for stream as json");
		}
		foreach (var (key, value) in dictionary.Select((KeyValuePair<string, string> pair) => (key: pair.Key, value: pair.Value)))
		{
			Add(pLanguage, key, value);
		}
	}

	public static void LoadLocale(string pLanguage, string pFilePath)
	{
		if (pFilePath.ToLower().EndsWith(".json"))
		{
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(pFilePath));
			if (dictionary == null)
			{
				throw new FormatException("Failed to load locale file at " + pFilePath + " as json");
			}
			{
				foreach (var (key, value) in dictionary.Select((KeyValuePair<string, string> pair) => (key: pair.Key, value: pair.Value)))
				{
					Add(pLanguage, key, value);
				}
				return;
			}
		}
		LogService.LogWarning("Unsupported locale file type of path: " + pFilePath);
	}

	[MethodImpl(MethodImplOptions.Synchronized | MethodImplOptions.AggressiveInlining)]
	public static void AddToCurrentLocale(string key, string value)
	{
		LocalizedTextManager.instance._localized_text[key] = value;
		Add(LocalizedTextManager.instance.language, key, value);
	}

	[MethodImpl(MethodImplOptions.Synchronized | MethodImplOptions.AggressiveInlining)]
	public static void Add(string language, string key, string value)
	{
		if (!locales.ContainsKey(language))
		{
			locales[language] = new Dictionary<string, string>();
		}
		locales[language][key] = value;
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public static void ApplyLocale(string language, bool pUpdateTexts = true)
	{
		if (!locales.ContainsKey(language))
		{
			locales[language] = new Dictionary<string, string>();
		}
		foreach (var (key, value) in locales[language].Select((KeyValuePair<string, string> pair) => (key: pair.Key, value: pair.Value)))
		{
			LocalizedTextManager.instance._localized_text[key] = value;
		}
		foreach (string item in locales["en"].Keys.Where((string key2) => !LocalizedTextManager.instance._localized_text.ContainsKey(key2)))
		{
			LocalizedTextManager.instance._localized_text[item] = locales["en"][item];
		}
		LocalizedTextManager.updateTexts();
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public static void ApplyLocale(bool pUpdateTexts = true)
	{
		if (!locales.ContainsKey(LocalizedTextManager.instance.language))
		{
			locales[LocalizedTextManager.instance.language] = new Dictionary<string, string>();
		}
		foreach (var (key, value) in locales[LocalizedTextManager.instance.language].Select((KeyValuePair<string, string> pair) => (key: pair.Key, value: pair.Value)))
		{
			LocalizedTextManager.instance._localized_text[key] = value;
		}
		foreach (string item in locales["en"].Keys.Where((string key2) => !LocalizedTextManager.instance._localized_text.ContainsKey(key2)))
		{
			LocalizedTextManager.instance._localized_text[item] = locales["en"][item];
		}
		if (pUpdateTexts)
		{
			LocalizedTextManager.updateTexts();
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(LocalizedTextManager), "setLanguage")]
	internal static void setLanguagePostfix(string pLanguage)
	{
		ApplyLocale(pLanguage);
	}
}
