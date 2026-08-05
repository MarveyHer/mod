using System;
using Beebyte.Obfuscator;
using com.unity3d.mediation;
using Unity.Services.LevelPlay;
using UnityEngine;

[ObfuscateLiterals]
public class IronSourceMobileAdsLoader : MonoBehaviour
{
	private const string APP_KEY = "unexpected_platform";

	private static IronSourceMobileAdsLoader instance;

	internal static bool initialized;

	public static void initAds()
	{
		if (!(instance != null))
		{
			GameObject obj = new GameObject("IronSourceMobileAdsLoader")
			{
				hideFlags = HideFlags.DontSave
			};
			UnityEngine.Object.DontDestroyOnLoad(obj);
			obj.transform.SetParent(GameObject.Find("Services").transform);
			instance = obj.AddComponent<IronSourceMobileAdsLoader>();
		}
	}

	internal void Start()
	{
		if (DebugConfig.isOn(DebugOption.TestAds))
		{
			Config.testAds = true;
		}
		if (!Config.isMobile || Config.hasPremium)
		{
			return;
		}
		try
		{
			log("Initializing");
			com.unity3d.mediation.LevelPlayAdFormat[] tLegacyAdFormats = new com.unity3d.mediation.LevelPlayAdFormat[1] { com.unity3d.mediation.LevelPlayAdFormat.REWARDED };
			Unity.Services.LevelPlay.LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
			Unity.Services.LevelPlay.LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
			Unity.Services.LevelPlay.LevelPlay.Init("unexpected_platform", null, tLegacyAdFormats);
			log("Version " + Unity.Services.LevelPlay.LevelPlay.UnityVersion);
		}
		catch (Exception message)
		{
			log("Could not initialize ads");
			Debug.Log(message);
		}
	}

	private void OnApplicationPause(bool isPaused)
	{
		log("OnApplicationPause = " + isPaused);
	}

	private void SdkInitializationCompletedEvent(Unity.Services.LevelPlay.LevelPlayConfiguration pConfig)
	{
		ThreadHelper.ExecuteInUpdate(delegate
		{
			log("Initialized");
			initialized = true;
			Config.adsInitialized = true;
		});
	}

	private void SdkInitializationFailedEvent(Unity.Services.LevelPlay.LevelPlayInitError pConfig)
	{
		ThreadHelper.ExecuteInUpdate(delegate
		{
			log("Failed to initialize ads");
			initialized = false;
		});
	}

	private static void log(string pLog)
	{
		Debug.Log(GetColor() + " <color=#abe0c3>" + pLog + "</color>");
	}

	public static string GetColor()
	{
		return "[<color=#abe0c3>IS</color>]";
	}
}
