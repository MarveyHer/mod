using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.UnityConverters.Math;
using UnityEngine;

public abstract class BaseAssetLibrary
{
	private const string ERROR_COLOR_WHITE = "#FFFFFF";

	private const string ERROR_COLOR_RED = "#FF3232";

	private const string ERROR_COLOR_YELLOW = "#FFF832";

	private const string ERROR_COLOR_MAIN = "#D2B7FF";

	[JsonProperty(Order = -1)]
	public string id = "ASSET_LIBRARY";

	protected static int _latest_hash = 1;

	private static JsonSerializer _json_serializer_internal = null;

	private static JsonSerializer _json_serializer
	{
		get
		{
			if (_json_serializer_internal == null)
			{
				_json_serializer_internal = JsonSerializer.Create(new JsonSerializerSettings
				{
					DefaultValueHandling = DefaultValueHandling.Ignore,
					Formatting = Formatting.Indented,
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
					Culture = CultureInfo.InvariantCulture,
					ContractResolver = new OrderedContractResolver(),
					Converters = 
					{
						(JsonConverter)new DelegateConverter(),
						(JsonConverter)new StringEnumConverter(),
						(JsonConverter)new Color32Converter(),
						(JsonConverter)new ColorConverter(),
						(JsonConverter)new Vector2Converter(),
						(JsonConverter)new Vector2IntConverter(),
						(JsonConverter)new Vector3Converter(),
						(JsonConverter)new Vector3IntConverter(),
						(JsonConverter)new Vector4Converter()
					}
				});
			}
			return _json_serializer_internal;
		}
	}

	public virtual int total_items => 0;

	public virtual void init()
	{
		_ = Application.version;
	}

	public void exportAssets()
	{
		if (id.StartsWith("beh") || id.StartsWith("debug"))
		{
			return;
		}
		string tPath = "GenAssets/wbassets" + "/" + id + ".json";
		try
		{
			using FileStream fs = new FileStream(tPath, FileMode.Create, FileAccess.Write, FileShare.None);
			using StreamWriter sw = new StreamWriter(fs)
			{
				NewLine = "\n"
			};
			using JsonTextWriter writer = new JsonTextWriter(sw)
			{
				Formatting = Formatting.Indented,
				Indentation = 4,
				IndentChar = ' '
			};
			_json_serializer.Serialize(writer, this);
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to export assets to " + tPath + ": " + ex.Message);
		}
	}

	public void importAssets()
	{
		string tPath = "GenAssets/wbassets" + "/" + id + ".json";
		if (!File.Exists(tPath))
		{
			Debug.LogError("File not found: " + tPath);
			return;
		}
		using FileStream fs = new FileStream(tPath, FileMode.Open, FileAccess.Read, FileShare.Read);
		using StreamReader sr = new StreamReader(fs);
		using JsonTextReader reader = new JsonTextReader(sr);
		_json_serializer.Populate(reader, this);
	}

	public virtual void post_init()
	{
	}

	public virtual void linkAssets()
	{
	}

	public virtual void editorDiagnostic()
	{
		editorDiagnosticLocales();
	}

	public virtual void editorDiagnosticLocales()
	{
	}

	public virtual void checkLocale(Asset pAsset, string pLocaleID)
	{
		if (!string.IsNullOrEmpty(pLocaleID) && !LocalizedTextManager.stringExists(pLocaleID))
		{
			logAssetError("<e>" + pAsset.id + "</e>: Missing translation key", pLocaleID);
			AssetManager.missing_locale_keys.Add(pLocaleID);
		}
	}

	internal bool hasSpriteInResources(string pPath)
	{
		if (SpriteTextureLoader.getSprite(pPath) == null)
		{
			return false;
		}
		return true;
	}

	internal bool hasSpriteInResourcesDebug(string pPath)
	{
		string tPath = Path.Combine(Path.Combine("Assets/Resources", pPath).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
		if (File.Exists(tPath + ".png"))
		{
			return true;
		}
		if (Directory.Exists(tPath) && Directory.GetFiles(tPath, "*.png", SearchOption.TopDirectoryOnly).Length != 0)
		{
			return true;
		}
		return false;
	}

	protected void logErrorOpposites(string pMainTraitID, string pOppositeTraitID)
	{
		Debug.LogError("<color=#FF3232>[" + pMainTraitID + "]</color> has opposite <color=#FFF832>[" + pOppositeTraitID + "]</color>, but <color=#FFF832>[" + pOppositeTraitID + "]</color> doesn't have opposite <color=#FF3232>[" + pMainTraitID + "]</color>");
	}

	private static string formatLog(string pMessage, string pRightPart = null)
	{
		if (pMessage.Contains("<"))
		{
			pMessage = pMessage.Replace("<e>", "<b><color=#FFFFFF>");
			pMessage = pMessage.Replace("</e>", "</color></b>");
		}
		string tStringLog = "<color=#D2B7FF>" + pMessage.Trim() + "</color>";
		if (!string.IsNullOrEmpty(pRightPart))
		{
			tStringLog = tStringLog + " : <b><color=#FFF832>" + pRightPart.Trim() + "</color></b>";
		}
		return tStringLog;
	}

	public static void logAssetLog(string pMessage, string pRightPart = null)
	{
		Debug.Log(formatLog(pMessage, pRightPart));
	}

	public static void logAssetError(string pMessage, string pRightPart = null)
	{
		Debug.LogError(formatLog(pMessage, pRightPart));
	}

	public virtual IEnumerable<Asset> getList()
	{
		yield break;
	}
}
