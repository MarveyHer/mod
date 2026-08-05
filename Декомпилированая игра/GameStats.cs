using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class GameStats : MonoBehaviour
{
	internal GameStatsData data;

	private string dataPath;

	private WorldTimer saveTimer;

	private void Start()
	{
		dataPath = Application.persistentDataPath + "/stats.json";
		loadData();
		if (data == null)
		{
			data = new GameStatsData();
		}
		else
		{
			checkDataForErrors();
		}
		saveTimer = new WorldTimer(30f, saveData);
		data.gameLaunches++;
	}

	internal bool goodForAds()
	{
		return true;
	}

	private void saveData()
	{
		string tWhat = "Stats";
		bool hasError = false;
		string tPathSaveData = dataPath;
		string tTempPath = dataPath + ".tmp";
		try
		{
			if (!Directory.Exists(Application.persistentDataPath))
			{
				Directory.CreateDirectory(Application.persistentDataPath);
			}
		}
		catch (Exception message)
		{
			WorldTip.showNow("Error creating directory to save stats in! Check console for details", pTranslate: false, "top");
			Debug.Log("Error creating directory: " + Application.persistentDataPath);
			Debug.Log(message);
		}
		try
		{
			using FileStream fs = new FileStream(tTempPath, FileMode.Create, FileAccess.Write);
			using StreamWriter sw = new StreamWriter(fs);
			using JsonWriter writer = new JsonTextWriter(sw);
			new JsonSerializer().Serialize(writer, data);
		}
		catch (IOException ex)
		{
			if (Toolbox.IsDiskFull(ex))
			{
				WorldTip.showNow("Error saving " + tWhat + " : Disk full!", pTranslate: false, "top");
			}
			else
			{
				Debug.Log("Could not save " + tWhat + " due to hard drive / IO Error : ");
				Debug.Log(ex);
				WorldTip.showNow("Error saving " + tWhat + " due to IOError! Check console for details", pTranslate: false, "top");
			}
			hasError = true;
		}
		catch (Exception message2)
		{
			Debug.Log("Could not save " + tWhat + " due to error : ");
			Debug.Log(message2);
			WorldTip.showNow("Error saving " + tWhat + "! Check console for errors", pTranslate: false, "top");
			hasError = true;
		}
		if (hasError)
		{
			if (File.Exists(tTempPath))
			{
				File.Delete(tTempPath);
			}
		}
		else
		{
			Toolbox.MoveSafely(tTempPath, tPathSaveData);
		}
		AchievementLibrary.life_is_a_sim.check();
	}

	private void checkDataForErrors()
	{
		if (double.IsNaN(data.gameTime) || double.IsInfinity(data.gameTime) || data.gameTime < 0.0)
		{
			Debug.Log(data.gameTime);
			Debug.LogError("Game time is NaN or Infinity! Resetting to 0");
			data.gameTime = 0.0;
		}
		if (data.creaturesBorn < 0)
		{
			data.creaturesBorn = Math.Max(0L, data.creaturesDied - data.creaturesCreated);
		}
	}

	private void loadData()
	{
		if (!File.Exists(dataPath))
		{
			return;
		}
		try
		{
			using FileStream fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read);
			using StreamReader sr = new StreamReader(fs);
			using JsonReader reader = new JsonTextReader(sr);
			JsonSerializer serializer = new JsonSerializer();
			data = serializer.Deserialize<GameStatsData>(reader);
		}
		catch (Exception message)
		{
			Debug.Log("exception caught when loading stats");
			Debug.LogError(message);
		}
		if (data == null)
		{
			Debug.LogError("(!) stats not has been loaded");
		}
	}

	public void updateStats(float pTime)
	{
		data.gameTime += pTime;
		saveTimer.update();
	}
}
