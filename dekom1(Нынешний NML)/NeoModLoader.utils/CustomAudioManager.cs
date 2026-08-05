using System;
using System.Collections.Generic;
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

	private static readonly List<ChannelContainer> Channels = new List<ChannelContainer>();

	private static readonly Dictionary<string, ChannelContainer> DrawingSounds = new Dictionary<string, ChannelContainer>();

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
		for (int i = 0; i < Channels.Count; i++)
		{
			ChannelContainer channelContainer = Channels[i];
			if (!UpdateChannel(channelContainer))
			{
				Channels.Remove(channelContainer);
				i--;
			}
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(MusicBox), "playSound", new Type[]
	{
		typeof(string),
		typeof(float),
		typeof(float),
		typeof(bool),
		typeof(bool)
	})]
	[HarmonyPriority(0)]
	private static bool PlaySoundPatch(string pSoundPath, float pX, float pY, bool pGameViewOnly)
	{
		if (!MusicBox.sounds_on)
		{
			return true;
		}
		if (pGameViewOnly && World.world.quality_changer.isLowRes())
		{
			return true;
		}
		if (!AudioWavLibrary.ContainsKey(pSoundPath))
		{
			return true;
		}
		LoadCustomSound(pSoundPath, pX, pY);
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(MusicBox), "playDrawingSound")]
	[HarmonyPriority(0)]
	private static bool PlayDrawingSoundPatch(string pSoundPath, float pX, float pY)
	{
		if (!MusicBox.sounds_on)
		{
			return true;
		}
		if (!AudioWavLibrary.ContainsKey(pSoundPath))
		{
			return true;
		}
		LoadDrawingSound(pSoundPath, pX, pY);
		return false;
	}

	public static ChannelContainer LoadDrawingSound(string pSoundPath, float pX, float pY)
	{
		if (DrawingSounds.TryGetValue(pSoundPath, out var value) && !value.Finushed)
		{
			SetChannelPosition(value, pX, pY);
		}
		else
		{
			DrawingSounds.Remove(pSoundPath);
			value = LoadCustomSound(pSoundPath, pX, pY);
			DrawingSounds.Add(pSoundPath, value);
		}
		return value;
	}

	public static ChannelContainer LoadCustomSound(string WAVName, float pX, float pY, Transform AttachedTo = null)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		WavContainer wavContainer = AudioWavLibrary[WAVName];
		if (wavContainer.Mode == SoundMode.Basic)
		{
			AttachedTo = null;
		}
		Sound val = default(Sound);
		if ((int)((System)(ref fmodSystem)).createSound(wavContainer.Path, (MODE)((wavContainer.Mode == SoundMode.Stereo3D) ? 18 : 2), ref val) > 0)
		{
			LogService.LogError("Unable to play sound " + WAVName + "!");
			return default(ChannelContainer);
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
		AddChannel(channel, AttachedTo, (wavContainer.Mode == SoundMode.Mono3D) ? new Vector3(pX, pY, wavContainer.Volume) : default(Vector3));
		if (wavContainer.Mode == SoundMode.Stereo3D)
		{
			SetChannelPosition(channel, pX, pY);
		}
		return Channels[Channels.Count - 1];
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

	internal static void AddChannel(Channel channel, Transform AttachedTo = null, Vector3 PosAndVolume = default(Vector3))
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Channels.Add(new ChannelContainer(channel, AttachedTo, PosAndVolume));
	}

	public static void ModifyWavData(string ID, float Volume, SoundMode Mode, int LoopCount = 0, bool Ramp = false, SoundType Type = SoundType.Sound)
	{
		if (AudioWavLibrary.ContainsKey(ID))
		{
			AudioWavLibrary[ID] = new WavContainer(AudioWavLibrary[ID].Path, Mode, Volume, LoopCount, Ramp, Type);
		}
	}

	private static bool UpdateChannel(ChannelContainer channel)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (channel.Finushed)
		{
			return false;
		}
		if ((Object)(object)channel.AttachedTo != (Object)null)
		{
			SetChannelPosition(channel, channel.AttachedTo.position.x, channel.AttachedTo.position.y);
		}
		if (channel.PosAndVolume != default(Vector3))
		{
			UpdateMonoVolume(channel);
		}
		return true;
	}

	private static void UpdateMonoVolume(ChannelContainer Channel)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)Camera.main).transform.position;
		float num = Vector3.Distance(new Vector3(position.x, position.y, Camera.main.orthographicSize), Vector2.op_Implicit(new Vector2(Channel.PosAndVolume.x, Channel.PosAndVolume.y)));
		Channel channel = Channel.Channel;
		((Channel)(ref channel)).setVolume(Mathf.Clamp01(Channel.PosAndVolume.z / num));
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(MapBox), "clearWorld")]
	public static void ClearAllCustomSounds()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		foreach (ChannelContainer channel2 in Channels)
		{
			Channel channel = channel2.Channel;
			((Channel)(ref channel)).stop();
		}
		Channels.Clear();
	}

	public static void SetChannelPosition(ChannelContainer channel, float pX, float pY)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (channel.PosAndVolume == default(Vector3))
		{
			SetChannelPosition(channel.Channel, pX, pY);
			return;
		}
		channel.PosAndVolume.x = pX;
		channel.PosAndVolume.y = pY;
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

	private static float GetVolume(SoundType soundType)
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
