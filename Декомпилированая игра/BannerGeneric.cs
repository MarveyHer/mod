using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class BannerGeneric<TMetaObject, TData> : BannerBase where TMetaObject : CoreSystemObject<TData> where TData : BaseSystemData
{
	protected TMetaObject meta_object;

	private bool _created;

	protected Image part_background;

	protected Image part_icon;

	protected Image part_frame;

	public bool enable_default_click = true;

	[SerializeField]
	private bool _enable_customize_click;

	public bool enable_tab_show_click;

	protected TData data => meta_object.data;

	protected virtual string tooltip_id
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	private void Start()
	{
		create();
	}

	private void create()
	{
		if (!_created)
		{
			_created = true;
			setupParts();
			setupClick();
			setupTooltip();
		}
	}

	protected virtual void setupClick()
	{
		if (!enable_default_click && !_enable_customize_click && !enable_tab_show_click)
		{
			return;
		}
		Button tButton = GetComponentInChildren<Button>();
		if (!(tButton == null))
		{
			if (enable_default_click)
			{
				tButton.onClick.AddListener(clickAction);
			}
			else if (_enable_customize_click)
			{
				tButton.onClick.AddListener(clickCustomize);
			}
			else if (enable_tab_show_click)
			{
				tButton.onClick.AddListener(clickShowTab);
			}
		}
	}

	protected virtual void clickAction()
	{
		if (!meta_object.hasDied())
		{
			if (!InputHelpers.mouseSupported)
			{
				switchOnDoubleTap();
			}
			else
			{
				showMetaWindow();
			}
		}
	}

	private void switchOnDoubleTap()
	{
		if (!Tooltip.isShowingFor(this))
		{
			tooltipAction();
		}
		else
		{
			showMetaWindow();
		}
	}

	private void showMetaWindow()
	{
		base.meta_type_asset.set_selected(meta_object);
		string tWindowId = base.meta_type_asset.window_name;
		if (ScrollWindow.isCurrentWindow(tWindowId))
		{
			ScrollWindow.get(tWindowId).showSameWindow();
		}
		else
		{
			ScrollWindow.showWindow(tWindowId);
		}
	}

	protected virtual void clickCustomize()
	{
		string tCustomizeWindowId = base.meta_asset.customize_window_id;
		if (tCustomizeWindowId == string.Empty)
		{
			Debug.LogError("var " + tCustomizeWindowId + " is not set!");
		}
		else
		{
			ScrollWindow.showWindow(tCustomizeWindowId);
		}
	}

	private void clickShowTab()
	{
		if (HotkeyLibrary.isHoldingControlForSelection())
		{
			clickAction();
			return;
		}
		base.meta_type_asset.selectAndInspect(meta_object);
		Tooltip.blockTooltips(0.5f);
		Tooltip.hideTooltipNow();
	}

	protected virtual void setupParts()
	{
		loadPartBackground();
		loadPartFrame();
		loadPartIcon();
	}

	protected virtual void loadPartFrame()
	{
		part_frame = base.transform.FindRecursive("Frame")?.GetComponent<Image>();
	}

	protected virtual void loadPartBackground()
	{
		part_background = base.transform.FindRecursive("Background")?.GetComponent<Image>();
	}

	protected virtual void loadPartIcon()
	{
		part_icon = base.transform.FindRecursive("Icon")?.GetComponent<Image>();
	}

	protected virtual void setupBanner()
	{
	}

	public override void load(NanoObject pObject)
	{
		setMetaObject(pObject);
		create();
		setupBanner();
	}

	public override NanoObject GetNanoObject()
	{
		return meta_object;
	}

	protected virtual void setupTooltip()
	{
		if (!TryGetComponent<TipButton>(out var tTipButton))
		{
			return;
		}
		tTipButton.setHoverAction(delegate
		{
			if (InputHelpers.mouseSupported)
			{
				tooltipAction();
			}
		});
	}

	protected virtual void tooltipAction()
	{
		Tooltip.show(this, tooltip_id, getTooltipData());
	}

	protected virtual TooltipData getTooltipData()
	{
		CustomDataContainer<bool> tCustomData = new CustomDataContainer<bool>();
		tCustomData["tab_banner"] = enable_tab_show_click;
		return new TooltipData
		{
			custom_data_bool = tCustomData
		};
	}

	public override void showTooltip()
	{
		tooltipAction();
	}

	protected void setMetaObject(NanoObject pObject)
	{
		meta_object = (TMetaObject)pObject;
	}

	internal virtual void normalize()
	{
		if (base.meta_asset.option_1_editable)
		{
			int tOption1Count = base.meta_asset.option_1_count();
			if (base.option_1 < 0)
			{
				base.option_1 += tOption1Count;
			}
			if (base.option_1 > tOption1Count - 1)
			{
				base.option_1 -= tOption1Count;
			}
			base.option_1 = Mathf.Clamp(base.option_1, 0, tOption1Count - 1);
		}
		if (base.meta_asset.option_2_editable)
		{
			int tOption2Count = base.meta_asset.option_2_count();
			if (base.option_2 < 0)
			{
				base.option_2 += tOption2Count;
			}
			if (base.option_2 > tOption2Count - 1)
			{
				base.option_2 -= tOption2Count;
			}
			base.option_2 = Mathf.Clamp(base.option_2, 0, tOption2Count - 1);
		}
		if (base.meta_asset.color_editable)
		{
			int tColorCount = base.meta_asset.color_count();
			if (base.color < 0)
			{
				base.color += tColorCount;
			}
			if (base.color > tColorCount - 1)
			{
				base.color -= tColorCount;
			}
			base.color = Mathf.Clamp(base.color, 0, tColorCount - 1);
		}
	}

	internal virtual void updateColor()
	{
		ColorAsset tColor = base.meta_asset.color_library().list[base.color];
		if (meta_object.updateColor(tColor))
		{
			base.meta_asset.on_new_color();
		}
	}
}
