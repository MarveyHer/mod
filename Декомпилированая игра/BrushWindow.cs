using UnityEngine;

public class BrushWindow : MonoBehaviour
{
	public Transform circles;

	public Transform squares;

	public Transform diamonds;

	public Transform special;

	public BrushSelectButton button_prefab;

	public void Awake()
	{
		foreach (BrushData tBrushData in AssetManager.brush_library.list)
		{
			if (tBrushData.show_in_brush_window)
			{
				Transform tParent = null;
				switch (tBrushData.group)
				{
				case BrushGroup.Circles:
					tParent = circles;
					break;
				case BrushGroup.Squares:
					tParent = squares;
					break;
				case BrushGroup.Diamonds:
					tParent = diamonds;
					break;
				case BrushGroup.Special:
					tParent = special;
					break;
				default:
					continue;
				}
				Object.Instantiate(button_prefab, tParent).setup(tBrushData);
			}
		}
	}

	public void selectBrush(GameObject pObject)
	{
		Config.current_brush = pObject.transform.name;
		GetComponent<ScrollWindow>().clickHide();
	}
}
