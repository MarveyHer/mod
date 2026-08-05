using System;
using System.Collections.Generic;
using System.IO;
using NeoModLoader.constants;
using NeoModLoader.services;
using Newtonsoft.Json;
using UnityEngine;
using YamlDotNet.Serialization;

namespace NeoModLoader.utils;

public static class SpriteLoadUtils
{
	private class MetaFile
	{
		public TextureImporter TextureImporter;
	}

	private class NCMSSpritesSettings
	{
		public class SpecificSetting
		{
			public string Alias = "";

			public float BorderB = 0f;

			public float BorderL = 0f;

			public float BorderR = 0f;

			public float BorderT = 0f;

			public string Path = "\\";

			public float PivotX = 0.5f;

			public float PivotY = 0f;

			public float PixelsPerUnit = 1f;

			public float RectH = -1f;

			public float RectW = -1f;

			public float RectX = 0f;

			public float RectY = 0f;

			public Sprite loadFromPath(string path)
			{
				//IL_0003: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Expected O, but got Unknown
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				Texture2D val = new Texture2D(0, 0);
				((Texture)val).filterMode = (FilterMode)0;
				ImageConversion.LoadImage(val, File.ReadAllBytes(path));
				Sprite val2 = Sprite.Create(val, new Rect(RectX, RectY, (RectW < 0f) ? ((float)((Texture)val).width) : RectW, (RectH < 0f) ? ((float)((Texture)val).height) : RectH), new Vector2(PivotX, PivotY), PixelsPerUnit, 1u, (SpriteMeshType)1, new Vector4(BorderL, BorderB, BorderR, BorderT));
				((Object)val2).name = (string.IsNullOrEmpty(Alias) ? System.IO.Path.GetFileNameWithoutExtension(path) : Alias);
				return val2;
			}
		}

		public SpecificSetting Default;

		public List<SpecificSetting> Specific;

		public override string ToString()
		{
			return JsonConvert.SerializeObject((object)this);
		}
	}

	private static Dictionary<string, Sprite> singleSpriteCache;

	private static Dictionary<string, NCMSSpritesSettings> dirNCMSSettings;

	private static HashSet<string> ignoreNCMSSettingsSearchPath;

	private static NCMSSpritesSettings.SpecificSetting defaultNCMSSetting;

	private static IDeserializer deserializer;

	public static Sprite LoadSingleSprite(string path)
	{
		if (singleSpriteCache.TryGetValue(path, out var value))
		{
			return value;
		}
		Sprite val = loadSpriteSimply(path);
		singleSpriteCache[path] = val;
		return val;
	}

	public static Sprite[] LoadSprites(string path)
	{
		TextureImporter textureImporter = loadMeta(path + ".meta");
		if (textureImporter == null)
		{
			NCMSSpritesSettings.SpecificSetting specificSetting = searchUpNCMSSetting(path);
			if (specificSetting == null)
			{
				Sprite val = loadSpriteSimply(path);
				if ((Object)(object)val == (Object)null)
				{
					return Array.Empty<Sprite>();
				}
				((Object)val).name = Path.GetFileNameWithoutExtension(path);
				return (Sprite[])(object)new Sprite[1] { val };
			}
			try
			{
				Sprite val2 = specificSetting.loadFromPath(path);
				if ((Object)(object)val2 != (Object)null)
				{
					return (Sprite[])(object)new Sprite[1] { val2 };
				}
			}
			catch (Exception ex)
			{
				LogService.LogError("Failed to load sprite from " + path + " with NCMSSetting " + specificSetting.GetType().FullName);
				LogService.LogError(ex.ToString());
				return Array.Empty<Sprite>();
			}
		}
		return loadSpriteWithMeta(path, textureImporter);
	}

	private static NCMSSpritesSettings.SpecificSetting searchUpNCMSSetting(string path)
	{
		string directoryName = Path.GetDirectoryName(path);
		do
		{
			if (!ignoreNCMSSettingsSearchPath.Contains(directoryName))
			{
				if (dirNCMSSettings.ContainsKey(directoryName))
				{
					return getInternalSetting(path, dirNCMSSettings[directoryName]);
				}
				string text = Path.Combine(directoryName, "sprites.json");
				if (File.Exists(text))
				{
					NCMSSpritesSettings nCMSSpritesSettings = JsonConvert.DeserializeObject<NCMSSpritesSettings>(File.ReadAllText(text));
					if (nCMSSpritesSettings != null)
					{
						NCMSSpritesSettings nCMSSpritesSettings2 = nCMSSpritesSettings;
						if (nCMSSpritesSettings2.Default == null)
						{
							nCMSSpritesSettings2.Default = defaultNCMSSetting;
						}
						dirNCMSSettings.Add(directoryName, nCMSSpritesSettings);
						List<NCMSSpritesSettings.SpecificSetting> specific = nCMSSpritesSettings.Specific;
						if (specific != null && specific.Contains(null))
						{
							LogService.LogWarning("Here is something wrong at " + text);
						}
						return getInternalSetting(path, nCMSSpritesSettings);
					}
					LogService.LogWarning("Wrong sprite settings file at " + text);
				}
				ignoreNCMSSettingsSearchPath.Add(directoryName);
			}
			if (directoryName == Paths.ModsPath)
			{
				return defaultNCMSSetting;
			}
			directoryName = Path.GetDirectoryName(directoryName);
		}
		while (!string.IsNullOrEmpty(directoryName));
		return defaultNCMSSetting;
		static NCMSSpritesSettings.SpecificSetting getInternalSetting(string i_path, NCMSSpritesSettings settings)
		{
			if (settings.Specific == null)
			{
				return settings.Default;
			}
			foreach (NCMSSpritesSettings.SpecificSetting item in settings.Specific)
			{
				if (item != null && item.Path == Path.GetFileName(i_path))
				{
					return item;
				}
			}
			return settings.Default;
		}
	}

	private static Sprite loadSpriteSimply(string path)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		byte[] array = File.ReadAllBytes(path);
		Texture2D val = new Texture2D(0, 0);
		((Texture)val).filterMode = (FilterMode)0;
		ImageConversion.LoadImage(val, array);
		return Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f), 1f);
	}

	private static Sprite[] loadSpriteWithMeta(string path, TextureImporter textureImporter)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = new Texture2D(0, 0);
		((Texture)val).filterMode = (FilterMode)0;
		ImageConversion.LoadImage(val, File.ReadAllBytes(path));
		Sprite[] array = (Sprite[])(object)new Sprite[textureImporter.spriteSheet.sprites.Count];
		for (int i = 0; i < array.Length; i++)
		{
			SingleSpriteMetaData singleSpriteMetaData = textureImporter.spriteSheet.sprites[i];
			array[i] = Sprite.Create(val, singleSpriteMetaData.rect, singleSpriteMetaData.pivot, 1f, 0u, (SpriteMeshType)0, singleSpriteMetaData.border);
			((Object)array[i]).name = singleSpriteMetaData.name;
		}
		return array;
	}

	private static TextureImporter loadMeta(string path)
	{
		if (!File.Exists(path))
		{
			return null;
		}
		return deserializer.Deserialize<MetaFile>(File.ReadAllText(path))?.TextureImporter;
	}

	static SpriteLoadUtils()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		singleSpriteCache = new Dictionary<string, Sprite>();
		dirNCMSSettings = new Dictionary<string, NCMSSpritesSettings>();
		ignoreNCMSSettingsSearchPath = new HashSet<string>();
		defaultNCMSSetting = new NCMSSpritesSettings.SpecificSetting();
		deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
	}
}
