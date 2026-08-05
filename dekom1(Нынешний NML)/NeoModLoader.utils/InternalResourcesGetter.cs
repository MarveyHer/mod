using System.IO;
using System.Reflection;
using NeoModLoader.constants;
using UnityEngine;

namespace NeoModLoader.utils;

internal static class InternalResourcesGetter
{
	private static Sprite mod_icon;

	private static Sprite icon_frame;

	private static Sprite icon_reload;

	private static Sprite github_icon;

	private static Sprite window_empty_frame;

	private static Sprite window_big_close;

	private static Sprite window_vert_name_plate;

	private static string commit = "";

	private static long last_write_time;

	private static Texture2D LoadManifestTexture(string path_under_resources)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NeoModLoader.resources." + path_under_resources);
		byte[] array = new byte[manifestResourceStream.Length];
		manifestResourceStream.Read(array, 0, array.Length);
		Texture2D val = new Texture2D(0, 0);
		((Texture)val).filterMode = (FilterMode)0;
		ImageConversion.LoadImage(val, array);
		return val;
	}

	private static byte[] LoadManifestBytes(string path_under_resources)
	{
		Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NeoModLoader.resources." + path_under_resources);
		byte[] array = new byte[manifestResourceStream.Length];
		manifestResourceStream.Read(array, 0, array.Length);
		return array;
	}

	public static long GetLastWriteTime()
	{
		if (last_write_time == 0)
		{
			FileInfo fileInfo = new FileInfo(Paths.NMLModPath);
			last_write_time = fileInfo.LastWriteTimeUtc.Ticks;
		}
		return last_write_time;
	}

	public static string GetCommit()
	{
		if (string.IsNullOrEmpty(commit))
		{
			Stream manifestResourceStream = WorldBoxMod.NeoModLoaderAssembly.GetManifestResourceStream("NeoModLoader.resources.commit");
			commit = new StreamReader(manifestResourceStream).ReadToEnd().Replace("\n", "").Replace("\r", "");
			manifestResourceStream.Close();
		}
		return commit;
	}

	public static Sprite GetIcon()
	{
		if ((Object)(object)mod_icon != (Object)null)
		{
			return mod_icon;
		}
		SpriteTextureLoader.addSprite("ui/icons/neomodloader", LoadManifestBytes("logo.png"));
		mod_icon = SpriteTextureLoader.getSprite("ui/icons/neomodloader");
		((Object)mod_icon).name = "NeoModLoader";
		ResourcesPatch.PatchResource("ui/icons/neomodloader", (Object)(object)mod_icon);
		return mod_icon;
	}

	public static Sprite GetIconFrame()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)icon_frame != (Object)null)
		{
			return icon_frame;
		}
		Texture2D val = LoadManifestTexture("square_frame_only.png");
		icon_frame = Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f), 1f, 0u, (SpriteMeshType)1, new Vector4(7f, 7f, 7f, 7f));
		return icon_frame;
	}

	public static Sprite GetGitHubIcon()
	{
		if ((Object)(object)github_icon != (Object)null)
		{
			return github_icon;
		}
		SpriteTextureLoader.addSprite("ui/icons/iconGithub", LoadManifestBytes("github.png"));
		github_icon = SpriteTextureLoader.getSprite("ui/icons/iconGithub");
		((Object)github_icon).name = "iconGithub";
		return github_icon;
	}

	public static Sprite GetReloadIcon()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)icon_reload != (Object)null)
		{
			return icon_reload;
		}
		Texture2D val = LoadManifestTexture("reload.png");
		icon_reload = Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f), 1f, 0u, (SpriteMeshType)1, new Vector4(0f, 0f, 0f, 0f));
		return icon_reload;
	}

	public static Sprite GetWindowEmptyFrame()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)window_empty_frame != (Object)null)
		{
			return window_empty_frame;
		}
		Texture2D val = LoadManifestTexture("window_empty_frame.png");
		window_empty_frame = Sprite.Create(val, new Rect(0f, 0f, 216f, 252f), new Vector2(0.5f, 0.5f), 1f, 1u, (SpriteMeshType)1, new Vector4(12f, 12f, 12f, 12f));
		((Object)window_empty_frame).name = "windowEmptyFrame";
		SpriteTextureLoader._cached_sprites["ui/special/" + ((Object)window_empty_frame).name] = window_empty_frame;
		return window_empty_frame;
	}

	public static Sprite GetWindowBigCloseSliced()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)window_big_close != (Object)null)
		{
			return window_big_close;
		}
		Texture2D val = LoadManifestTexture("windowBigCloseSliced.png");
		window_big_close = Sprite.Create(val, new Rect(0f, 0f, 36f, 35f), new Vector2(0.5f, 0.5f), 1f, 1u, (SpriteMeshType)1, new Vector4(8f, 8f, 8f, 8f));
		((Object)window_big_close).name = "windowBigCloseSliced";
		SpriteTextureLoader._cached_sprites["ui/special/" + ((Object)window_big_close).name] = window_big_close;
		return window_big_close;
	}

	public static Sprite GetWindowVertNamePlate()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)window_vert_name_plate != (Object)null)
		{
			return window_vert_name_plate;
		}
		Texture2D val = LoadManifestTexture("windowVertNamePlate.png");
		window_vert_name_plate = Sprite.Create(val, new Rect(0f, 0f, 18f, 43f), new Vector2(0.5f, 0.5f), 1f, 1u, (SpriteMeshType)1, new Vector4(2f, 2f, 2f, 2f));
		((Object)window_vert_name_plate).name = "windowVertNamePlate";
		SpriteTextureLoader._cached_sprites["ui/special/" + ((Object)window_vert_name_plate).name] = window_vert_name_plate;
		return window_vert_name_plate;
	}
}
