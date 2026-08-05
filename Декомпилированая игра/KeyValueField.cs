using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyValueField : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
	public Color odd_color = Toolbox.makeColor("#000000", 0f);

	public Color even_color = Toolbox.makeColor("#30322B");

	public Color highlight_color = Toolbox.makeColor("#111111");

	public Image background;

	public Image icon;

	public Image icon_secondary;

	public Text name_text;

	public Text value;

	public bool auto_odd_even_coloring;

	public UnityAction on_hover_value;

	public UnityAction on_hover_value_out;

	public UnityAction on_click_value;

	private Color _not_highlight_color;

	private LocalizedText _name_text;

	private LocalizedText _value;

	private bool _check_language = true;

	private void Awake()
	{
		Button tButton = value.GetComponent<Button>();
		if (Input.mousePresent)
		{
			tButton.OnHover(delegate
			{
				if (InputHelpers.mouseSupported)
				{
					on_hover_value?.Invoke();
				}
			});
			tButton.OnHoverOut(delegate
			{
				if (InputHelpers.mouseSupported)
				{
					on_hover_value_out?.Invoke();
				}
			});
		}
		tButton.onClick.AddListener(delegate
		{
			if (!InputHelpers.mouseSupported && !Tooltip.isShowingFor(this))
			{
				on_hover_value?.Invoke();
				EventSystem.current.SetSelectedGameObject(base.gameObject);
			}
			else
			{
				setBackgroundColor(_not_highlight_color);
				on_click_value?.Invoke();
			}
		});
		_name_text = name_text.GetComponent<LocalizedText>();
		_value = value.GetComponent<LocalizedText>();
		_check_language = _name_text != null || _value != null;
	}

	private void OnEnable()
	{
		checkLanguage();
		if (auto_odd_even_coloring)
		{
			checkOddEvenColor(base.transform.GetActiveSiblingIndex());
		}
	}

	private void OnDisable()
	{
		on_hover_value = null;
		on_hover_value_out = null;
		on_click_value = null;
	}

	private void checkLanguage()
	{
		if (_check_language)
		{
			_name_text?.checkSpecialLanguages();
			_value?.checkSpecialLanguages();
		}
	}

	public void checkOddEvenColor(int pIndex)
	{
		if (pIndex % 2 != 0)
		{
			setEvenColor();
		}
		else
		{
			setOddColor();
		}
	}

	public void OnPointerEnter(PointerEventData pData)
	{
		if (InputHelpers.mouseSupported)
		{
			_not_highlight_color = background.color;
			setHighlightColor();
		}
	}

	public void OnPointerExit(PointerEventData pData)
	{
		if (InputHelpers.mouseSupported)
		{
			setBackgroundColor(_not_highlight_color);
		}
	}

	public void OnSelect(BaseEventData pEventData)
	{
		if (!InputHelpers.mouseSupported)
		{
			_not_highlight_color = background.color;
			setHighlightColor();
		}
	}

	public void OnDeselect(BaseEventData pEventData)
	{
		if (!InputHelpers.mouseSupported)
		{
			setNotHighlightColor();
		}
	}

	public void setEvenColor()
	{
		setBackgroundColor(even_color);
	}

	public void setOddColor()
	{
		setBackgroundColor(odd_color);
	}

	public void setHighlightColor()
	{
		setBackgroundColor(highlight_color);
	}

	public void setNotHighlightColor()
	{
		setBackgroundColor(_not_highlight_color);
	}

	private void setBackgroundColor(Color pColor)
	{
		background.color = pColor;
	}

	public void setMetaForTooltip(MetaType pMetaType, long pMetaId, string pTooltipId = null, TooltipDataGetter pData = null)
	{
		on_hover_value = null;
		on_hover_value_out = Tooltip.hideTooltip;
		on_click_value = null;
		if (!pMetaType.isNone())
		{
			MetaTypeAsset tAsset = AssetManager.meta_type_library.getAsset(pMetaType);
			on_hover_value = delegate
			{
				tAsset.stat_hover(pMetaId, this);
			};
			on_click_value = delegate
			{
				tAsset.stat_click(pMetaId, this);
			};
		}
		else if (!string.IsNullOrEmpty(pTooltipId))
		{
			on_hover_value = delegate
			{
				Tooltip.show(this, pTooltipId, pData());
			};
		}
	}
}
