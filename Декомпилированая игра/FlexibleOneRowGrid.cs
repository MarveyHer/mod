using System.Collections.Generic;
using LayoutGroupExt;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class FlexibleOneRowGrid : MonoBehaviour, ILayoutController
{
	public bool debug;

	public int bonus_spacing_x;

	private RectTransform _grid_rect;

	private GridLayoutGroup _grid;

	private GridLayoutGroupExtended _grid_extended;

	private bool _is_extended;

	private bool _initialized;

	private void Awake()
	{
		init();
	}

	private void init()
	{
		if (!_initialized)
		{
			_initialized = true;
			if (this.HasComponent<GridLayoutGroup>())
			{
				_grid = GetComponent<GridLayoutGroup>();
				_grid_rect = _grid.GetComponent<RectTransform>();
			}
			else
			{
				_grid_extended = GetComponent<GridLayoutGroupExtended>();
				_grid_rect = _grid_extended.GetComponent<RectTransform>();
				_is_extended = true;
			}
		}
	}

	public void SetLayoutHorizontal()
	{
		if (debug || Application.isPlaying)
		{
			init();
			float tCellSize = (_is_extended ? _grid_extended.cellSize.x : _grid.cellSize.x);
			float tGridSize = _grid_rect.rect.width;
			float tChildren = calculateChildren();
			float tSpacingX = 0f;
			float tCurrentWidth = tCellSize * tChildren + (float)bonus_spacing_x * (tChildren - 1f);
			if (tCurrentWidth < tGridSize)
			{
				tSpacingX = bonus_spacing_x;
			}
			else
			{
				tCurrentWidth = tCellSize * tChildren;
				tSpacingX = (tGridSize - tCurrentWidth) / (tChildren - 1f);
			}
			if (_is_extended)
			{
				_grid_extended.spacing = new Vector2(tSpacingX, 0f);
			}
			else
			{
				_grid.spacing = new Vector2(tSpacingX, 0f);
			}
		}
	}

	public float calculateChildren()
	{
		List<Component> tToIgnoreList = CollectionPool<List<Component>, Component>.Get();
		int tChildren = 0;
		int i = 0;
		for (int tLen = _grid_rect.childCount; i < tLen; i++)
		{
			RectTransform tChild = _grid_rect.GetChild(i) as RectTransform;
			if (tChild == null || !tChild.gameObject.activeInHierarchy)
			{
				continue;
			}
			if (!tChild.HasComponent<ILayoutIgnorer>())
			{
				tChildren++;
				continue;
			}
			tChild.GetComponents(typeof(ILayoutIgnorer), tToIgnoreList);
			for (int j = 0; j < tToIgnoreList.Count; j++)
			{
				if (!((ILayoutIgnorer)tToIgnoreList[j]).ignoreLayout)
				{
					tChildren++;
					break;
				}
			}
			tToIgnoreList.Clear();
		}
		CollectionPool<List<Component>, Component>.Release(tToIgnoreList);
		return tChildren;
	}

	public void SetLayoutVertical()
	{
	}
}
