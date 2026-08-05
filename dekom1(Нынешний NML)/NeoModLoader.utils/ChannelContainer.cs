using System.Diagnostics;
using System.Runtime.CompilerServices;
using FMOD;
using UnityEngine;

namespace NeoModLoader.utils;

public struct ChannelContainer
{
	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Channel _003CChannel_003Ek__BackingField;

	public Vector3 PosAndVolume;

	public Transform AttachedTo;

	public Channel Channel
	{
		[CompilerGenerated]
		readonly get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CChannel_003Ek__BackingField;
		}
		[CompilerGenerated]
		internal set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CChannel_003Ek__BackingField = value;
		}
	}

	public readonly bool Finushed
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			Channel channel = Channel;
			bool flag = default(bool);
			return (int)((Channel)(ref channel)).isPlaying(ref flag) != 0 || !flag;
		}
	}

	internal ChannelContainer(Channel channel, Transform attachedTo = null, Vector3 PosAndVolume = default(Vector3))
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		this.PosAndVolume = default(Vector3);
		Channel = channel;
		this.PosAndVolume = PosAndVolume;
		AttachedTo = attachedTo;
	}
}
