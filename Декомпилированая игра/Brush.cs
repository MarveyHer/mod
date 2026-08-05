using System;

public class Brush
{
	public static string getRandom()
	{
		return AssetManager.brush_library.list.GetRandom().id;
	}

	public static string getRandom(int pMinSize, int pMaxSize = 50, Predicate<BrushData> pMatch = null)
	{
		foreach (BrushData tBrush in AssetManager.brush_library.list.LoopRandom())
		{
			if ((pMatch == null || pMatch(tBrush)) && tBrush.sqr_size >= pMinSize && tBrush.sqr_size <= pMaxSize)
			{
				return tBrush.id;
			}
		}
		return "circ_1";
	}

	public static BrushData get(int pSize, string pID = "circ_")
	{
		string tID = pID + pSize;
		BrushData tAsset = AssetManager.brush_library.get(tID);
		if (tAsset != null)
		{
			return tAsset;
		}
		tAsset = AssetManager.brush_library.clone(tID, pID + "1");
		tAsset.size = pSize;
		AssetManager.brush_library.post_init();
		return tAsset;
	}

	public static BrushData get(string pID)
	{
		return AssetManager.brush_library.get(pID);
	}
}
