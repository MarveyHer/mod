using System;
using System.Collections.Generic;
using UnityEngine;

public class BrushLibrary : AssetLibrary<BrushData>
{
	[NonSerialized]
	private static readonly List<string> _available_brushes = new List<string>();

	public override void init()
	{
		base.init();
		add(new BrushData
		{
			id = "circ_0",
			size = 0,
			group = BrushGroup.Circles,
			fast_spawn = true,
			continuous = true,
			localized_key = "brush_circ",
			generate_action = delegate(BrushData pAsset)
			{
				pAsset.pos = new BrushPixelData[1]
				{
					new BrushPixelData(0, 0, 0)
				};
			}
		});
		t.show_in_brush_window = true;
		clone("circ_1", "circ_0");
		t.size = 1;
		t.fast_spawn = false;
		t.show_in_brush_window = true;
		t.generate_action = delegate(BrushData pAsset)
		{
			int size = pAsset.size;
			int i = 0;
			int num = size;
			int num2 = 1 - size;
			HashSet<BrushPixelData> hashSet = new HashSet<BrushPixelData>();
			for (; i <= num; i++)
			{
				for (int j = -i; j <= i; j++)
				{
					int pDist = (int)Toolbox.Dist(0, 0, j, num);
					hashSet.Add(new BrushPixelData(j, num, pDist));
					hashSet.Add(new BrushPixelData(j, -num, pDist));
				}
				for (int k = -num; k <= num; k++)
				{
					int pDist2 = (int)Toolbox.Dist(0, 0, i, k);
					hashSet.Add(new BrushPixelData(k, i, pDist2));
					hashSet.Add(new BrushPixelData(k, -i, pDist2));
				}
				if (num2 < 0)
				{
					num2 += 2 * i + 3;
				}
				else
				{
					num2 += 2 * (i - num) + 5;
					num--;
				}
			}
			pAsset.pos = hashSet.ToArray();
		};
		clone("circ_2", "circ_1");
		t.size = 2;
		t.show_in_brush_window = true;
		clone("circ_3", "circ_1");
		t.size = 3;
		t.show_in_brush_window = true;
		clone("circ_4", "circ_1");
		t.size = 4;
		t.continuous = false;
		t.show_in_brush_window = true;
		clone("circ_5", "circ_4");
		t.drops = 2;
		t.size = 5;
		t.show_in_brush_window = true;
		clone("circ_6", "circ_5");
		t.size = 6;
		t.show_in_brush_window = true;
		clone("circ_7", "circ_5");
		t.size = 7;
		t.show_in_brush_window = true;
		clone("circ_8", "circ_5");
		t.size = 8;
		clone("circ_9", "circ_5");
		t.size = 9;
		clone("circ_10", "circ_5");
		t.size = 10;
		t.show_in_brush_window = true;
		t.ui_scale = new Vector2(0.9f, 0.9f);
		clone("circ_11", "circ_5");
		t.size = 11;
		clone("circ_12", "circ_5");
		t.size = 12;
		clone("circ_15", "circ_5");
		t.size = 15;
		t.show_in_brush_window = true;
		t.ui_scale = new Vector2(0.9f, 0.9f);
		clone("circ_20", "circ_5");
		t.size = 20;
		clone("circ_30", "circ_5");
		t.size = 30;
		clone("circ_70", "circ_5");
		t.drops = 3;
		t.size = 70;
		add(new BrushData
		{
			id = "sqr_0",
			size = 0,
			group = BrushGroup.Squares,
			continuous = true,
			fast_spawn = true,
			localized_key = "brush_sqr",
			generate_action = delegate(BrushData pAsset)
			{
				pAsset.pos = new BrushPixelData[1]
				{
					new BrushPixelData(0, 0, 0)
				};
			}
		});
		add(new BrushData
		{
			id = "sqr_1",
			size = 1,
			group = BrushGroup.Squares,
			continuous = true,
			fast_spawn = false,
			localized_key = "brush_sqr"
		});
		t.show_in_brush_window = true;
		t.generate_action = delegate(BrushData pAsset)
		{
			int size = pAsset.size;
			Vector2Int vector2Int = new Vector2Int(size / 2, size / 2);
			using ListPool<BrushPixelData> listPool = new ListPool<BrushPixelData>();
			for (int i = -size; i <= size; i++)
			{
				for (int j = -size; j <= size; j++)
				{
					int pDist = (int)Toolbox.Dist(i, j, vector2Int.x, vector2Int.y);
					listPool.Add(new BrushPixelData(i, j, pDist));
				}
			}
			pAsset.pos = listPool.ToArray();
		};
		clone("sqr_2", "sqr_1");
		t.size = 2;
		t.continuous = false;
		t.show_in_brush_window = true;
		clone("sqr_3", "sqr_1");
		t.size = 3;
		t.continuous = false;
		clone("sqr_4", "sqr_1");
		t.size = 4;
		t.continuous = false;
		t.show_in_brush_window = true;
		clone("sqr_5", "sqr_1");
		t.size = 5;
		t.continuous = false;
		clone("sqr_10", "sqr_1");
		t.size = 10;
		t.drops = 2;
		t.continuous = false;
		t.show_in_brush_window = true;
		t.ui_scale = new Vector2(0.8f, 0.8f);
		clone("sqr_15", "sqr_10");
		t.size = 15;
		t.continuous = false;
		t.show_in_brush_window = true;
		t.ui_scale = new Vector2(0.8f, 0.8f);
		add(new BrushData
		{
			id = "diamond_1",
			continuous = true,
			group = BrushGroup.Diamonds,
			localized_key = "brush_diamond",
			size = 1,
			fast_spawn = false
		});
		t.show_in_brush_window = true;
		t.generate_action = delegate(BrushData pAsset)
		{
			string text = "brush_" + pAsset.id;
			Texture2D texture2D = Resources.Load<Texture2D>("ui/Icons/brushes/" + text);
			int width = texture2D.width;
			int height = texture2D.height;
			_ = width / 2;
			Vector2Int vector2Int = new Vector2Int(width / 2, height / 2);
			using ListPool<BrushPixelData> listPool = new ListPool<BrushPixelData>();
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (!(texture2D.GetPixel(i, j) != Color.white))
					{
						int num = vector2Int.x - i;
						int num2 = vector2Int.y - j;
						int pDist = (int)Toolbox.Dist(num, num2, vector2Int.x, vector2Int.y);
						listPool.Add(new BrushPixelData(num, num2, pDist));
					}
				}
			}
			pAsset.pos = listPool.ToArray();
		};
		clone("diamond_2", "diamond_1");
		t.size = 2;
		t.show_in_brush_window = true;
		t.ui_scale = new Vector2(0.8f, 0.8f);
		clone("diamond_4", "diamond_1");
		t.size = 4;
		t.continuous = false;
		t.show_in_brush_window = true;
		t.ui_scale = new Vector2(0.7f, 0.7f);
		clone("diamond_5", "diamond_1");
		t.drops = 2;
		t.size = 5;
		t.continuous = false;
		t.show_in_brush_window = true;
		t.ui_scale = new Vector2(0.7f, 0.7f);
		clone("diamond_7", "diamond_5");
		t.size = 7;
		t.continuous = false;
		t.show_in_brush_window = true;
		t.ui_scale = new Vector2(0.9f, 0.9f);
		add(new BrushData
		{
			id = "special_1",
			continuous = true,
			group = BrushGroup.Special,
			localized_key = "brush_special",
			size = 1,
			fast_spawn = false
		});
		t.generate_action = get("diamond_1").generate_action;
		t.show_in_brush_window = true;
		t.ui_size = new Vector2(14f, 14f);
		clone("special_2", "special_1");
		t.size = 2;
		t.show_in_brush_window = true;
		t.ui_size = new Vector2(17.93f, 17.93f);
		clone("special_3", "special_1");
		t.size = 3;
		t.continuous = false;
		t.show_in_brush_window = true;
		t.ui_size = new Vector2(21.4f, 21.4f);
		clone("special_4", "special_1");
		t.drops = 2;
		t.size = 4;
		t.continuous = false;
		t.show_in_brush_window = true;
		t.ui_size = new Vector2(23.55f, 23.55f);
		clone("special_5", "special_4");
		t.drops = 2;
		t.size = 5;
		t.continuous = false;
		t.show_in_brush_window = true;
		t.ui_size = new Vector2(28.28f, 28.28f);
	}

	public override void post_init()
	{
		base.post_init();
		foreach (BrushData tAsset in list)
		{
			if (tAsset.show_in_brush_window)
			{
				_available_brushes.Add(tAsset.id);
			}
			BrushPixelData[] pos = tAsset.pos;
			if (pos != null && pos.Length != 0)
			{
				continue;
			}
			tAsset.generate_action(tAsset);
			int tMinX = int.MaxValue;
			int tMaxX = int.MinValue;
			int tMinY = int.MaxValue;
			int tMaxY = int.MinValue;
			for (int i = 0; i < tAsset.pos.Length; i++)
			{
				BrushPixelData tPixel = tAsset.pos[i];
				if (tPixel.x < tMinX)
				{
					tMinX = tPixel.x;
				}
				if (tPixel.x > tMaxX)
				{
					tMaxX = tPixel.x;
				}
				if (tPixel.y < tMinY)
				{
					tMinY = tPixel.y;
				}
				if (tPixel.y > tMaxY)
				{
					tMaxY = tPixel.y;
				}
			}
			tAsset.width = Mathf.Abs(tMaxX - tMinX) + 1;
			tAsset.height = Mathf.Abs(tMaxY - tMinY) + 1;
			tAsset.sqr_size = tAsset.width * tAsset.height;
		}
	}

	public override void linkAssets()
	{
		base.linkAssets();
		foreach (BrushData item in list)
		{
			shuffleBrush(item);
		}
	}

	public static void shuffleBrush(BrushData pAsset)
	{
		pAsset.pos.Shuffle();
		int tIndexOfCenter = 0;
		for (int i = 0; i < pAsset.pos.Length; i++)
		{
			BrushPixelData tPixel = pAsset.pos[i];
			if (tPixel.x == 0 && tPixel.y == 0)
			{
				tIndexOfCenter = i;
				break;
			}
		}
		BrushPixelData tFirstElement = pAsset.pos[0];
		BrushPixelData tCenterElement = pAsset.pos[tIndexOfCenter];
		pAsset.pos[0] = tCenterElement;
		pAsset.pos[tIndexOfCenter] = tFirstElement;
	}

	public override void editorDiagnosticLocales()
	{
		base.editorDiagnosticLocales();
		foreach (BrushData tAsset in list)
		{
			checkLocale(tAsset, tAsset.getLocaleID());
		}
	}

	public override BrushData clone(string pNew, string pFrom)
	{
		BrushData brushData = base.clone(pNew, pFrom);
		brushData.show_in_brush_window = false;
		return brushData;
	}

	internal static void nextBrush()
	{
		Config.current_brush = getPrevious(Config.current_brush);
	}

	internal static void previousBrush()
	{
		Config.current_brush = getNext(Config.current_brush);
	}

	private static string getNext(string pBrushName)
	{
		bool tNext = false;
		for (int i = 0; i < _available_brushes.Count; i++)
		{
			string tKey = _available_brushes[i];
			if (tKey == pBrushName)
			{
				tNext = true;
			}
			else if (tNext)
			{
				return tKey;
			}
		}
		return pBrushName;
	}

	private static string getPrevious(string pBrushName)
	{
		bool tNext = false;
		for (int i = _available_brushes.Count - 1; i >= 0; i--)
		{
			string tKey = _available_brushes[i];
			if (tKey == pBrushName)
			{
				tNext = true;
			}
			else if (tNext)
			{
				return tKey;
			}
		}
		return pBrushName;
	}
}
