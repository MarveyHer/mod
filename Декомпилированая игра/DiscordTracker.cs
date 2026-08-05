using System;
using Discord;
using Proyecto26;
using UnityEngine;

public class DiscordTracker : MonoBehaviour, IRichTracker
{
	private const long DISCORD_GAME_ID = 816251591299432468L;

	private const ulong DISCORD_FLAGS = 1uL;

	private static global::Discord.Discord _discord;

	private static ActivityManager _activity_manager;

	private bool _initiated;

	private static DiscordTracker _instance;

	private static Activity _activity;

	private static bool _have_user = false;

	private static int _user_tries = 10;

	private static float _timer = 10f;

	private void Start()
	{
		if (_initiated)
		{
			return;
		}
		_initiated = true;
		bool tDestroy = false;
		try
		{
			_instance = this;
			_discord = new global::Discord.Discord(816251591299432468L, 1uL);
			_activity_manager = _discord.GetActivityManager();
			_activity = new Activity
			{
				State = LocalizedTextManager.getText("discord_browsing"),
				Assets = 
				{
					LargeImage = "worldboxlogo",
					LargeText = "WorldBox"
				},
				Timestamps = 
				{
					Start = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds
				},
				Instance = true
			};
			_activity_manager?.UpdateActivity(_activity, delegate(Result pRes)
			{
				if (pRes != Result.Ok)
				{
					Debug.Log("Disabling Discord");
					Debug.Log(pRes);
					UnityEngine.Object.Destroy(_instance);
				}
			});
		}
		catch (ResultException message)
		{
			Debug.Log("Disabling Discord Integration (Discord not running, or game not run as Administrator)");
			Debug.Log(message);
			tDestroy = true;
		}
		catch (Exception message2)
		{
			Debug.Log("Disabling Discord Integration (Discord not running, or game not run as Administrator)");
			Debug.Log(message2);
			tDestroy = true;
		}
		if (tDestroy)
		{
			UnityEngine.Object.Destroy(_instance);
		}
	}

	private static void tryGetUser()
	{
		try
		{
			_user_tries--;
			User tUser = _discord.GetUserManager().GetCurrentUser();
			string tUserID = tUser.Id.ToString();
			if (!string.IsNullOrEmpty(tUserID))
			{
				Config.discordId = tUserID;
				RestClient.DefaultRequestHeaders["wb-dsc"] = tUserID;
				_have_user = true;
				Debug.Log("D:" + Config.discordId);
			}
			else
			{
				Debug.Log("D:nf");
			}
			string tUsername = tUser.Username;
			if (!string.IsNullOrEmpty(tUsername))
			{
				Config.discordName = tUsername;
			}
			string tUserDiscriminator = tUser.Discriminator;
			if (!string.IsNullOrEmpty(tUserDiscriminator))
			{
				Config.discordDiscriminator = tUserDiscriminator;
			}
			VersionCheck.checkVersion();
		}
		catch (Exception)
		{
			Debug.Log("D:F");
		}
	}

	private void Update()
	{
		if (!_initiated)
		{
			return;
		}
		try
		{
			_discord.RunCallbacks();
		}
		catch (Exception message)
		{
			Debug.Log("Disabling Discord");
			Debug.Log(message);
			UnityEngine.Object.Destroy(_instance);
			return;
		}
		if (_timer > 0f)
		{
			_timer -= Time.deltaTime;
			return;
		}
		_timer = 10f;
		try
		{
			if (!_have_user && _user_tries > 0)
			{
				tryGetUser();
			}
			updateDetails(PowerTracker.activeStat);
		}
		catch (Exception message2)
		{
			Debug.Log("Disabling Discord");
			Debug.Log(message2);
			UnityEngine.Object.Destroy(_instance);
		}
	}

	private void OnDisable()
	{
		_discord?.Dispose();
	}

	private void OnDestroy()
	{
		_instance = null;
		_activity_manager = null;
		PowerTracker.discordTracker = null;
	}

	public void trackViewing(string pString)
	{
		if (_instance == null)
		{
			return;
		}
		if (pString != "" && LocalizedTextManager.stringExists(pString))
		{
			pString = LocalizedTextManager.getText("discord_viewing").Replace("$window$", LocalizedTextManager.getText(pString));
		}
		else
		{
			if (pString != "")
			{
				Debug.Log("Missing translation for " + pString);
			}
			pString = LocalizedTextManager.getText("discord_browsing");
		}
		trackActivity(pString);
	}

	public void trackWatching()
	{
		if (!(_instance == null))
		{
			trackActivity(LocalizedTextManager.getText("discord_watching"));
		}
	}

	public void trackUsing(string pPower)
	{
		if (!(_instance == null))
		{
			trackActivity(LocalizedTextManager.getText("discord_using").Replace("$power$", LocalizedTextManager.getText(pPower)));
		}
	}

	public void updateUsing(int pAmount, string pPower = "")
	{
		trackActivity(LocalizedTextManager.getText(pPower) + " (" + pAmount + ")");
	}

	public void inspectKingdom(string pKingdom)
	{
		trackActivity(LocalizedTextManager.getText("village_statistics_kingdom") + ": " + pKingdom);
	}

	public void inspectVillage(string pVillage)
	{
		trackActivity(LocalizedTextManager.getText("village") + ": " + pVillage);
	}

	public void inspectUnit(string pUnit)
	{
		trackActivity("inspect".Localize() + ": " + pUnit);
	}

	public void spectatingUnit(string pUnit)
	{
		trackActivity(LocalizedTextManager.getText("tip_following_unit").Replace("$name$", pUnit));
	}

	public void trackActivity(string pState = "")
	{
		if (!(_instance == null))
		{
			_activity.State = pState;
			_activity_manager?.UpdateActivity(_activity, delegate
			{
			});
		}
	}

	public void updateDetails(StatisticsAsset pStat)
	{
		if (!(_instance == null))
		{
			string tKey = pStat.getLocaleID();
			if (!string.IsNullOrEmpty(tKey))
			{
				_activity.Details = LocalizedTextManager.getText(tKey) + ": " + pStat.last_value;
			}
			else
			{
				_activity.Details = pStat.last_value;
			}
			_activity_manager?.UpdateActivity(_activity, delegate
			{
			});
		}
	}
}
