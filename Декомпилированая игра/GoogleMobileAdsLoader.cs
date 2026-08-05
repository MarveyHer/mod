using System;
using Beebyte.Obfuscator;
using GoogleMobileAds.Api;
using UnityEngine;

[ObfuscateLiterals]
public class GoogleMobileAdsLoader : MonoBehaviour
{
	private static GoogleMobileAdsLoader instance;

	internal static bool initialized;

	public static void initAds()
	{
		if (!(instance != null) && shouldLoad())
		{
			GameObject obj = new GameObject("GoogleMobileAdsLoader")
			{
				hideFlags = HideFlags.DontSave
			};
			UnityEngine.Object.DontDestroyOnLoad(obj);
			obj.transform.SetParent(GameObject.Find("Services").transform);
			instance = obj.AddComponent<GoogleMobileAdsLoader>();
		}
	}

	public void Start()
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
			string tRegion = PreciseLocale.GetRegion();
			if (tRegion.ToLower().Contains("us") || tRegion.ToLower().Contains("gb"))
			{
				GoogleInterstitialAd.default_current = 1;
				GoogleRewardAd.default_current = 1;
			}
			string tCurrency = PreciseLocale.GetCurrencyCode();
			if (tCurrency == "USD" || tCurrency == "GBP")
			{
				GoogleInterstitialAd.default_current = 1;
				GoogleRewardAd.default_current = 1;
			}
		}
		catch (Exception)
		{
		}
		try
		{
			log("Initializing");
			MobileAds.DisableMediationInitialization();
			MobileAds.Initialize(delegate
			{
				ThreadHelper.ExecuteInUpdate(delegate
				{
					log("Initialized");
					initialized = true;
					Config.adsInitialized = true;
				});
			});
		}
		catch (Exception message)
		{
			log("Could not initialize ads");
			Debug.Log(message);
		}
	}

	private static void log(string pLog)
	{
		Debug.Log(GetColor() + " <color=#fbbc04>" + pLog + "</color>");
	}

	public static string GetColor()
	{
		return "[<color=#ea4335>A</color><color=#fbbc04>D</color><color=#4285f4>M</color>]";
	}

	public static bool shouldLoad()
	{
		return false;
	}
}
