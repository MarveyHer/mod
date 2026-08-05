using UnityEngine;

namespace NeoModLoader.constants;

public static class Others
{
	internal const long confirmed_compile_time = 100000000L;

	internal const string harmony_id = "wbom.nml";

	public static bool unity_player_enabled { get; internal set; }

	public static bool is_editor
	{
		get
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Invalid comparison between Unknown and I4
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Invalid comparison between Unknown and I4
			if (unity_player_enabled)
			{
				RuntimePlatform platform = Application.platform;
				if (1 == 0)
				{
				}
				bool result = (int)platform == 0 || (int)platform == 7 || (int)platform == 16;
				if (1 == 0)
				{
				}
				return result;
			}
			return false;
		}
	}
}
