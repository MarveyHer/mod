using System;
using System.Threading.Tasks;
using db;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using Proyecto26;
using UnityEngine;

public class InitStuff : MonoBehaviour
{
	private static bool initiated = false;

	private static bool restinitiated = false;

	private float elapsedSeconds;

	public static float targetSeconds = 900f;

	private float checkInitTimeOut = 1f;

	private float servicesInitTimeOut = 3f;

	private float adsInitTimeOut = 8f;

	private void Start()
	{
		ThreadHelper.Initialize();
		initDB();
		initSteam();
	}

	public static void initRest()
	{
		if (restinitiated)
		{
			return;
		}
		restinitiated = true;
		try
		{
			RestClient.DefaultRequestHeaders["salt"] = RequestHelper.salt ?? "na";
			RestClient.DefaultRequestHeaders["wb-version"] = Application.version ?? "na";
			RestClient.DefaultRequestHeaders["wb-identifier"] = Application.identifier ?? "na";
			RestClient.DefaultRequestHeaders["wb-platform"] = Application.platform.ToString() ?? "na";
			RestClient.DefaultRequestHeaders["wb-language"] = LocalizedTextManager.instance.language ?? "na";
			RestClient.DefaultRequestHeaders["wb-prem"] = (Config.hasPremium ? "y" : "n");
			RestClient.DefaultRequestHeaders["wb-build"] = Config.versionCodeText ?? "na";
			RestClient.DefaultRequestHeaders["wb-gen"] = ((!Config.gen.HasValue) ? "na" : (Config.gen.Value ? "y" : "n"));
			RestClient.DefaultRequestHeaders["wb-git"] = Config.gitCodeText ?? "na";
		}
		catch (Exception ex)
		{
			Debug.Log("RestClient initialization Error");
			Debug.Log(ex.Message);
		}
	}

	public static void initOnlineServices()
	{
		if (initiated)
		{
			return;
		}
		initiated = true;
		if (Config.isEditor)
		{
			return;
		}
		try
		{
			if (Config.firebaseEnabled)
			{
				initFirebase();
			}
		}
		catch (Exception ex)
		{
			Debug.Log("Firebase Init Error");
			Debug.Log(ex.Message);
		}
		try
		{
			VersionCheck.checkVersion();
		}
		catch (Exception ex2)
		{
			Debug.Log("Version Error");
			Debug.Log(ex2.Message);
		}
		initRichPresence();
	}

	private void Update()
	{
		if (checkInitTimeOut > 0f)
		{
			checkInitTimeOut -= Time.fixedDeltaTime;
			if (checkInitTimeOut < 0f)
			{
				if (Config.firebaseEnabled)
				{
					if (!Config.firebaseChecked)
					{
						try
						{
							checkFirebase();
						}
						catch (Exception ex)
						{
							Debug.Log("Firebase Check Error");
							Debug.Log(ex.Message);
						}
					}
				}
				else
				{
					Config.firebaseChecked = true;
				}
			}
		}
		if (!Config.firebaseChecked)
		{
			return;
		}
		if (!restinitiated)
		{
			initRest();
		}
		if (servicesInitTimeOut > 0f)
		{
			servicesInitTimeOut -= Time.fixedDeltaTime;
			if (servicesInitTimeOut < 0f)
			{
				initOnlineServices();
			}
		}
		if (Config.firebaseInitiating)
		{
			return;
		}
		if (adsInitTimeOut > 0f)
		{
			adsInitTimeOut -= Time.fixedDeltaTime;
			if (adsInitTimeOut < 0f)
			{
				InitAds.initAdProviders();
			}
		}
		if (elapsedSeconds <= targetSeconds)
		{
			elapsedSeconds += Time.deltaTime;
			return;
		}
		elapsedSeconds = 0f;
		try
		{
			if (Config.hasPremium || targetSeconds != 900f)
			{
				VersionCheck.checkVersion();
			}
		}
		catch (Exception)
		{
		}
	}

	private static void checkFirebase()
	{
		Debug.Log("Firebase Starting Check");
		Config.firebaseChecked = false;
		FirebaseApp.CheckDependenciesAsync().ContinueWithOnMainThread(delegate(Task<DependencyStatus> pTask)
		{
			Debug.Log("Firebase check continuing on thread");
			DependencyStatus dependencyStatus = pTask.Result;
			ThreadHelper.ExecuteInUpdate(delegate
			{
				Debug.Log("Firebase check status");
				if (dependencyStatus != DependencyStatus.Available)
				{
					Debug.Log("Firebase is not available");
					Debug.Log(dependencyStatus);
				}
				else
				{
					Debug.Log("Firebase is available");
					Debug.Log(dependencyStatus);
				}
				Config.firebaseChecked = true;
			});
		});
	}

	private static void initFirebase()
	{
		Debug.Log("Firebase init");
		Config.firebaseInitiating = true;
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(delegate(Task<DependencyStatus> pTask)
		{
			Debug.Log("Firebase init continuing on thread");
			DependencyStatus dependencyStatus = pTask.Result;
			Debug.Log(dependencyStatus);
			ThreadHelper.ExecuteInUpdate(delegate
			{
				Debug.Log("Firebase task status");
				if (dependencyStatus == DependencyStatus.Available)
				{
					try
					{
						Config.firebaseInitiating = false;
						Config.firebase_available = true;
						FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
						Debug.Log("Firebase loaded");
						FirebaseAnalytics.LogEvent("data", "installerName", Config.iname ?? "");
						return;
					}
					catch (Exception ex)
					{
						Debug.Log("Firebase Error");
						Debug.Log(ex.Message);
						Config.authEnabled = false;
						Config.firebase_available = false;
						Config.firebaseInitiating = false;
						return;
					}
				}
				Debug.Log($"Could not resolve all Firebase dependencies: {dependencyStatus}");
				Config.authEnabled = false;
				Config.firebase_available = false;
			});
		});
	}

	private static void initDB()
	{
		GameObject obj = new GameObject("DB")
		{
			hideFlags = HideFlags.DontSave
		};
		UnityEngine.Object.DontDestroyOnLoad(obj);
		obj.AddComponent<DBManager>();
		obj.transform.SetParent(GameObject.Find("Services").transform);
	}

	private static void initRichPresence()
	{
		if (!Config.disable_steam || !Config.disable_discord)
		{
			GameObject obj = new GameObject("PowerTracker")
			{
				hideFlags = HideFlags.DontSave
			};
			UnityEngine.Object.DontDestroyOnLoad(obj);
			obj.AddComponent<PowerTracker>();
			obj.transform.SetParent(GameObject.Find("Services").transform);
		}
	}

	internal static void initSteam()
	{
		if (!Config.disable_steam)
		{
			GameObject obj = new GameObject("Steam")
			{
				hideFlags = HideFlags.DontSave
			};
			UnityEngine.Object.DontDestroyOnLoad(obj);
			obj.AddComponent<SteamSDK>();
			obj.transform.SetParent(GameObject.Find("Services").transform);
			SteamAchievements.InitAchievements();
		}
	}
}
