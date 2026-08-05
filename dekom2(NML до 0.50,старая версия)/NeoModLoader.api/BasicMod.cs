using System.IO;
using NeoModLoader.constants;
using NeoModLoader.services;
using UnityEngine;

namespace NeoModLoader.api;

public abstract class BasicMod<T> : MonoBehaviour, IMod, ILocalizable, IConfigurable, IFeatureLoadManaged, IStagedLoad where T : BasicMod<T>
{
	private ModConfig _config = null;

	private ModDeclare _declare = null;

	private bool _isLoaded;

	private Transform _prefab_library;

	public static T Instance { get; private set; }

	public static T I => Instance;

	public Transform PrefabLibrary
	{
		get
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)_prefab_library == (Object)null)
			{
				_prefab_library = ((Component)this).transform.Find("PrefabLibrary");
				if ((Object)(object)_prefab_library == (Object)null)
				{
					_prefab_library = new GameObject("PrefabLibrary").transform;
					_prefab_library.SetParent(((Component)this).transform);
				}
			}
			return _prefab_library;
		}
	}

	public IModFeatureManager ModFeatureManager { get; private set; }

	public ModConfig GetConfig()
	{
		return _config;
	}

	public string GetLocaleFilesDirectory(ModDeclare pModDeclare)
	{
		return Path.Combine(pModDeclare.FolderPath, "Locales");
	}

	public GameObject GetGameObject()
	{
		return ((Component)this).gameObject;
	}

	public virtual string GetUrl()
	{
		return string.IsNullOrEmpty(_declare.RepoUrl) ? "https://github.com/WorldBoxOpenMods" : _declare.RepoUrl;
	}

	public void OnLoad(ModDeclare pModDecl, GameObject pGameObject)
	{
		if (!_isLoaded)
		{
			_declare = pModDecl;
			Instance = (T)this;
			ModFeatureManager = new ModFeatureManager<T>(this);
			if (_config == null)
			{
				_config = LoadConfig();
			}
			LogInfo("OnLoad");
			OnModLoad();
			ModFeatureManager.InstantiateFeatures();
			LogInfo("Loaded");
			_isLoaded = true;
		}
	}

	public virtual void Init()
	{
		ModFeatureManager.Init();
	}

	public virtual void PostInit()
	{
		ModFeatureManager.PostInit();
	}

	public ModDeclare GetDeclaration()
	{
		return _declare;
	}

	public static GameObject NewPrefab(string name)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		GameObject val = new GameObject(name);
		val.transform.SetParent(Instance.PrefabLibrary);
		return val;
	}

	private ModConfig LoadConfig()
	{
		ModConfig modConfig = new ModConfig(Path.Combine(Paths.ModsConfigPath, _declare.UID + ".config"), pIsPersistent: true);
		string path = Path.Combine(_declare.FolderPath, Paths.ModDefaultConfigFileName);
		if (!File.Exists(path))
		{
			return modConfig;
		}
		ModConfig pDefaultConfig = new ModConfig(Path.Combine(_declare.FolderPath, Paths.ModDefaultConfigFileName));
		modConfig.MergeWith(pDefaultConfig);
		return modConfig;
	}

	protected abstract void OnModLoad();

	public static void LogInfo(string message)
	{
		LogService.LogInfo("[" + Instance._declare.Name + "]: " + message);
	}

	public static void LogWarning(string message)
	{
		LogService.LogWarning("[" + Instance._declare.Name + "]: " + message);
	}

	public static void LogError(string message)
	{
		LogService.LogError("[" + Instance._declare.Name + "]: " + message);
	}
}
