using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MetaSwitchButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Button button;

	public Text meta_name;

	public Transform banner_parent;

	private IBanner _banner;

	private MultiBannerPool _pool;

	private MetaSwitchManager.Direction _direction;

	public void init(MetaSwitchManager.Direction pDirection, SwitchWindowsAction pAction)
	{
		_direction = pDirection;
		_pool = new MultiBannerPool(banner_parent);
		button.onClick.AddListener(delegate
		{
			pAction(_direction);
		});
	}

	public void setBanner(IBanner pBanner)
	{
		_banner = pBanner;
	}

	public MultiBannerPool getPool()
	{
		return _pool;
	}

	public void clear()
	{
		_pool.clear();
	}

	public void OnPointerEnter(PointerEventData pEventData)
	{
		if (InputHelpers.mouseSupported)
		{
			showTooltip();
		}
	}

	public void OnPointerExit(PointerEventData pEventData)
	{
		if (InputHelpers.mouseSupported)
		{
			Tooltip.hideTooltip();
		}
	}

	private void showTooltip()
	{
		if (MetaSwitchManager.isSwitcherEnabled())
		{
			_banner.meta_type_asset.stat_hover(_banner.GetNanoObject().getID(), this);
		}
	}
}
