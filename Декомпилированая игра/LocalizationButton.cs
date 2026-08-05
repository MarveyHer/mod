using System;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationButton : MonoBehaviour
{
	public Sprite button_current;

	public Sprite button_normal;

	public Sprite button_highlight;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private Image _bg_image;

	[SerializeField]
	private Button _button;

	[SerializeField]
	private Text _text_field;

	private TipButton _tip_button;

	private LocalizedText _localized_text;

	[SerializeField]
	private Text _percent;

	private GameLanguageAsset _asset;

	private bool _initialized;

	public GameLanguageAsset getAsset()
	{
		return _asset;
	}

	private void init()
	{
		if (_initialized)
		{
			return;
		}
		_initialized = true;
		_localized_text = _text_field.GetComponent<LocalizedText>();
		_tip_button = _button.GetComponent<TipButton>();
		_tip_button.hoverAction = delegate
		{
			if (InputHelpers.mouseSupported)
			{
				showTooltip();
			}
		};
		TipButton tip_button = _tip_button;
		tip_button.clickAction = (TooltipAction)Delegate.Combine(tip_button.clickAction, (TooltipAction)delegate
		{
			if (InputHelpers.mouseSupported)
			{
				changeLanguage();
			}
			else if (Tooltip.isShowingFor(this))
			{
				changeLanguage();
			}
			else
			{
				showTooltip();
			}
		});
		base.gameObject.name = _asset.id;
		if (_asset.path_icon != null)
		{
			_icon.sprite = SpriteTextureLoader.getSprite(_asset.path_icon);
			_icon.gameObject.SetActive(value: true);
			RectTransform _text_field_rect = _text_field.GetComponent<RectTransform>();
			_text_field_rect.offsetMin = new Vector2(18.5f, _text_field_rect.offsetMin.y);
			_text_field_rect.offsetMax = new Vector2(-4f, _text_field_rect.offsetMax.y);
		}
		else
		{
			_icon.gameObject.SetActive(value: false);
			RectTransform _text_field_rect2 = _text_field.GetComponent<RectTransform>();
			_text_field_rect2.offsetMin = new Vector2(4f, _text_field_rect2.offsetMin.y);
			_text_field_rect2.offsetMax = new Vector2(-4f, _text_field_rect2.offsetMax.y);
		}
		_text_field.text = _asset.name;
		_localized_text.checkSpecialLanguages(_asset);
	}

	private void showTooltip()
	{
		TooltipData tData = new TooltipData
		{
			game_language_asset = _asset
		};
		Tooltip.show(this, "game_language", tData);
	}

	private void changeLanguage()
	{
		LocalizedTextManager.instance.setLanguage(_asset.id);
		WorldLanguagesWindow.updateButtons();
	}

	internal void checkSprite()
	{
		if (LocalizedTextManager.current_language == _asset)
		{
			_bg_image.sprite = button_current;
		}
		else if (LocalizedTextManager.getCulture(base.transform.gameObject.name) == LocalizedTextManager.getCurrentCulture())
		{
			_bg_image.sprite = button_highlight;
		}
		else
		{
			_bg_image.sprite = button_normal;
		}
	}

	public void SetAsset(GameLanguageAsset pAsset, int pDone)
	{
		_asset = pAsset;
		if (pDone > 0)
		{
			if (pDone < 40)
			{
				_percent.color = Toolbox.color_negative_RGBA;
			}
			else if (pDone < 60)
			{
				_percent.color = Toolbox.color_log_warning;
			}
			else if (pDone < 80)
			{
				_percent.color = Toolbox.color_text_default;
			}
			else
			{
				_percent.color = Toolbox.color_positive_RGBA;
			}
			_percent.text = pDone + "%";
			_percent.gameObject.SetActive(value: true);
		}
		else
		{
			_percent.gameObject.SetActive(value: false);
		}
		init();
		checkSprite();
	}
}
