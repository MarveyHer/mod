using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class MusicBoxDebug
{
	internal List<DebugMusicBoxData> list = new List<DebugMusicBoxData>();

	public void add(string pPath, float pX, float pY, EventInstance pInstance)
	{
		pX += Randy.randomFloat(-0.5f, 0.5f);
		pY += Randy.randomFloat(-0.5f, 0.5f);
		list.Add(new DebugMusicBoxData
		{
			timer = 3f,
			path = pPath,
			x = pX,
			y = pY,
			instance = pInstance
		});
	}

	public void update()
	{
		for (int i = list.Count - 1; i >= 0; i--)
		{
			DebugMusicBoxData debugMusicBoxData = list[i];
			debugMusicBoxData.timer -= Time.deltaTime;
			if (debugMusicBoxData.timer <= 0f)
			{
				list.RemoveAt(i);
			}
		}
	}

	public void clear()
	{
		list.Clear();
	}
}
