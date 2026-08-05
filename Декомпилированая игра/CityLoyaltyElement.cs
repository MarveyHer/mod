using UnityEngine;
using UnityEngine.UI;

public class CityLoyaltyElement : MonoBehaviour
{
	private City _city;

	private TooltipData _tooltip_data;

	public void setCity(City pCity)
	{
		_city = pCity;
	}

	private void Start()
	{
		Button component = GetComponent<Button>();
		component.onClick.AddListener(showTooltip);
		component.OnHover(showHoverTooltip);
		component.OnHoverOut(Tooltip.hideTooltip);
	}

	private void showHoverTooltip()
	{
		if (Config.tooltips_active)
		{
			showTooltip();
		}
	}

	private void showTooltip()
	{
		_tooltip_data = new TooltipData
		{
			city = _city
		};
		Tooltip.show(base.gameObject, "loyalty", _tooltip_data);
	}
}
