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

	internal ChannelContainer(Channel channel, Transform attachedTo = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Channel = channel;
		AttachedTo = attachedTo;
	}
}
