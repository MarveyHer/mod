using System;
using Proyecto26;
using UnityEngine;

public class GameLoadedEvent : BaseMapObject
{
	private void Awake()
	{
		LogText.log("GameLoadedEvent", "Awake", "st");
		LogText.log("GameLoadedEvent", "Awake", "en");
		setVersionData();
	}

	private void setVersionData()
	{
		TextAsset tTextAsset = Resources.Load("texts/build_info") as TextAsset;
		try
		{
			Config.versionCodeText = tTextAsset.text.Split('$')[0];
			Config.versionCodeDate = tTextAsset.text.Split('$')[1];
		}
		catch (Exception)
		{
			if (tTextAsset != null)
			{
				Config.versionCodeText = tTextAsset.text;
				Config.versionCodeDate = "";
			}
		}
		try
		{
			RestClient.DefaultRequestHeaders["wb-build"] = Config.versionCodeText ?? "na";
		}
		catch (Exception)
		{
		}
		try
		{
			TextAsset gitAsset = Resources.Load("texts/git_info") as TextAsset;
			if (gitAsset != null)
			{
				Config.gitCodeText = gitAsset.text;
			}
			try
			{
				RestClient.DefaultRequestHeaders["wb-git"] = Config.gitCodeText ?? "na";
			}
			catch (Exception)
			{
			}
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	internal override void create()
	{
		base.create();
		Config.LOAD_TIME_INIT = Time.realtimeSinceStartup;
		LogText.log("GameLoadedEvent", "create");
		LocalizedTextManager.instance.setLanguage(PlayerConfig.dict["language"].stringVal);
		if (Globals.TRAILER_MODE)
		{
			TrailerModeSettings.startEvent();
		}
		World.world.startTheGame();
		GodPower.diagnostic();
		Config.updateCrashMetadata();
	}
}
