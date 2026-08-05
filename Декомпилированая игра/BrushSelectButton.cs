using UnityEngine;
using UnityEngine.UI;

public class BrushSelectButton : MonoBehaviour
{
	public Image icon;

	private BrushData _brush_asset;

	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(delegate
		{
			Config.current_brush = _brush_asset.id;
			ScrollWindow.hideAllEvent();
		});
	}

	public void setup(BrushData pBrushData)
	{
		_brush_asset = pBrushData;
		base.gameObject.name = _brush_asset.id;
		_brush_asset.setupImage(icon);
		GetComponent<TipButton>().textOnClick = _brush_asset.getLocaleID();
	}
}
