using UnityEngine.Device;

public static class DynamicSpritesConfig
{
	public const int EDGE_PIXEL = 1;

	public const int TEXTURE_SIZE_512 = 512;

	public const int TEXTURE_SIZE_1024 = 1024;

	public const int TEXTURE_SIZE_2048 = 2048;

	private static int _cached_texture_size;

	public static int texture_size
	{
		get
		{
			if (_cached_texture_size == 0)
			{
				_cached_texture_size = calculateTargetTextureSize();
			}
			return _cached_texture_size;
		}
	}

	private static int calculateTargetTextureSize()
	{
		int tDefaultTextureSize = ((!Config.isMobile) ? 1024 : 512);
		int tMaxTextureSize = SystemInfo.maxTextureSize;
		if (tMaxTextureSize < tDefaultTextureSize)
		{
			tDefaultTextureSize = tMaxTextureSize;
		}
		return tDefaultTextureSize;
	}
}
