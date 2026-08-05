using System.Collections.Generic;
using System.Collections.ObjectModel;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using HarmonyLib;
using NeoModLoader.services;
using UnityEngine;

namespace NeoModLoader.utils;

public class CustomAudioManager
{
	private static System fmodSystem;

	private static ChannelGroup SFXGroup;

	private static ChannelGroup MusicGroup;

	private static ChannelGroup UIGroup;

	internal static readonly Dictionary<string, WavContainer> AudioWavLibrary = new Dictionary<string, WavContainer>();

	private static readonly List<ChannelContainer> channels = new List<ChannelContainer>();

	public static ReadOnlyCollection<ChannelContainer> ChannelList => channels.AsReadOnly();

	[HarmonyPostfix]
	[HarmonyPatch(typeof(RuntimeManager), "Update")]
	private static void Update()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		((ChannelGroup)(ref SFXGroup)).setVolume(GetVolume(SoundType.Sound));
		((ChannelGroup)(ref MusicGroup)).setVolume(GetVolume(SoundType.Music));
		((ChannelGroup)(ref UIGroup)).setVolume(GetVolume(SoundType.UI));
		for (int i = 0; i < channels.Count; i++)
		{
			ChannelContainer channelContainer = channels[i];
			if (!UpdateChannel(channelContainer))
			{
				channels.Remove(channelContainer);
				i--;
			}
		}
	}

	public static int LoadCustomSound(float pX, float pY, string pSoundPath, Transform AttachedTo = null)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Invalid comparison between Unknown and I4
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		WavContainer wavContainer = AudioWavLibrary[pSoundPath];
		Sound val = default(Sound);
		if ((int)((System)(ref fmodSystem)).createSound(wavContainer.Path, (MODE)(wavContainer._3D ? 18 : 2), ref val) > 0)
		{
			Debug.Log((object)("Unable to play sound " + pSoundPath + "!"));
			return -1;
		}
		((Sound)(ref val)).setLoopCount(wavContainer.LoopCount);
		Channel channel = default(Channel);
		switch (wavContainer.Type)
		{
		case SoundType.Music:
			((System)(ref fmodSystem)).playSound(val, MusicGroup, false, ref channel);
			break;
		case SoundType.Sound:
			((System)(ref fmodSystem)).playSound(val, SFXGroup, false, ref channel);
			break;
		case SoundType.UI:
			((System)(ref fmodSystem)).playSound(val, UIGroup, false, ref channel);
			break;
		}
		((Channel)(ref channel)).setVolumeRamp(wavContainer.Ramp);
		((Channel)(ref channel)).setVolume(wavContainer.Volume / 100f);
		AddChannel(channel, AttachedTo);
		SetChannelPosition(channel, pX, pY);
		return channels.Count - 1;
	}

	internal static void Initialize()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Invalid comparison between Unknown and I4
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Invalid comparison between Unknown and I4
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Invalid comparison between Unknown and I4
		System studioSystem = RuntimeManager.StudioSystem;
		if ((int)((System)(ref studioSystem)).getCoreSystem(ref fmodSystem) > 0)
		{
			LogService.LogError("Failed to initialize FMOD Core System!");
			return;
		}
		if ((int)((System)(ref fmodSystem)).createChannelGroup("SFXGroup", ref SFXGroup) > 0)
		{
			LogService.LogError("Failed to create SFXGroup!");
		}
		if ((int)((System)(ref fmodSystem)).createChannelGroup("MusicGroup", ref MusicGroup) > 0)
		{
			LogService.LogError("Failed to create MusicGroup!");
		}
		if ((int)((System)(ref fmodSystem)).createChannelGroup("UIGroup", ref UIGroup) > 0)
		{
			LogService.LogError("Failed to create UIGroup!");
		}
	}

	internal static void AddChannel(Channel channel, Transform AttachedTo = null)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		ChannelContainer item = new ChannelContainer(channel, AttachedTo);
		channels.Add(item);
	}

	public static void ModifyWavData(string ID, float Volume, bool _3D, int LoopCount = 0, bool Ramp = false, SoundType Type = SoundType.Sound)
	{
		if (AudioWavLibrary.ContainsKey(ID))
		{
			AudioWavLibrary[ID] = new WavContainer(AudioWavLibrary[ID].Path, _3D, Volume, LoopCount, Ramp, Type);
		}
	}

	private static bool UpdateChannel(ChannelContainer channel)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Channel channel2 = channel.Channel;
		bool flag = default(bool);
		((Channel)(ref channel2)).isPlaying(ref flag);
		if (!flag)
		{
			return false;
		}
		if ((Object)(object)channel.AttachedTo != (Object)null)
		{
			SetChannelPosition(channel.Channel, channel.AttachedTo.position.x, channel.AttachedTo.position.y);
		}
		return true;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(MapBox), "clearWorld")]
	public static void ClearAllCustomSounds()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		foreach (ChannelContainer channel2 in channels)
		{
			Channel channel = channel2.Channel;
			((Channel)(ref channel)).stop();
		}
		channels.Clear();
	}

	public static void SetChannelPosition(Channel channel, float pX, float pY)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		VECTOR val = default(VECTOR);
		VECTOR val2 = default(VECTOR);
		((Channel)(ref channel)).get3DAttributes(ref val, ref val2);
		if (val.x != pX || val.y != pY)
		{
			VECTOR val3 = new VECTOR
			{
				x = pX,
				y = pY,
				z = 0f
			};
			((Channel)(ref channel)).set3DAttributes(ref val3, ref val2);
		}
	}

	public static float GetVolume(SoundType soundType)
	{
		float num = 1f;
		return soundType switch
		{
			SoundType.Music => num * ((float)PlayerConfig.getIntValue("volume_music") / 100f), 
			SoundType.Sound => num * ((float)PlayerConfig.getIntValue("volume_sound_effects") / 100f), 
			_ => num * ((float)PlayerConfig.getIntValue("volume_ui") / 100f), 
		} * ((float)PlayerConfig.getIntValue("volume_master_sound") / 100f);
	}
}
