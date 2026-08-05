using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColorElement : MonoBehaviour
{
	public Button button;

	public Image selection;

	public Image outer;

	public Image inner;

	public int index;

	public MetaCustomizationAsset asset;

	public void setColor(Color pOuter, Color pInner)
	{
		outer.color = pOuter;
		inner.color = pInner;
	}

	public void setSelected(bool pSelected)
	{
		selection.enabled = pSelected;
	}

	public void setAction(UnityAction pAction)
	{
		button.onClick.AddListener(pAction);
	}

	public void showTooltip()
	{
		CustomDataContainer<int> tCustomDataInt = new CustomDataContainer<int>();
		tCustomDataInt["color_count"] = asset.color_count();
		tCustomDataInt["color_current"] = index + 1;
		TooltipData tData = new TooltipData
		{
			tip_name = asset.color_locale,
			custom_data_int = tCustomDataInt
		};
		Tooltip.show(base.gameObject, "color_counter", tData);
	}
}
