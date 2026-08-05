public class CustomTextureAtlas
{
	private static int width = 1202;

	private static int height = 2021;

	public static bool filesExists()
	{
		return true;
	}

	public static void createUnityBin()
	{
	}

	private static void save(string pData)
	{
	}

	internal static void delete(string pTexture)
	{
	}

	public static string createTextureID(string pString)
	{
		string tTexturePreID = width.ToString() + height;
		return Toolbox.textureID(pString, tTexturePreID);
	}
}
