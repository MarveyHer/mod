using UnityEngine;
using UnityEngine.UI;

public class ButtonGraphCategory : MonoBehaviour
{
	public Sprite sprite_on;

	public Sprite sprite_off;

	public Sprite sprite_on_light;

	private Image _button_graphics;

	private Image _icon;

	public bool is_on;

	private GraphCategoriesContainer _main_container;

	private Text _text;

	private Image _colored_circle;

	private Image _background_circle;

	private TipButton _tip_button;

	private HistoryDataAsset _asset;

	private bool _initialized;

	private void Awake()
	{
		init();
	}

	public void init()
	{
		if (_initialized)
		{
			return;
		}
		_initialized = true;
		GetComponent<Button>().onClick.AddListener(switchCategory);
		_tip_button = GetComponent<TipButton>();
		_button_graphics = GetComponent<Image>();
		_icon = base.transform.FindRecursive("Icon").GetComponent<Image>();
		_main_container = GetComponentInParent<GraphCategoriesContainer>();
		_text = base.transform.FindRecursive("Title").GetComponent<Text>();
		_colored_circle = base.transform.FindRecursive("Colored Circle").GetComponent<Image>();
		_background_circle = base.transform.FindRecursive("Background Circle").GetComponent<Image>();
		_tip_button.hoverAction = delegate
		{
			if (InputHelpers.mouseSupported)
			{
				showTooltip();
			}
		};
		checkSpriteStatus();
	}

	public void setAsset(HistoryDataAsset pAsset)
	{
		if (pAsset != null)
		{
			_asset = pAsset;
			_colored_circle.color = pAsset.getColorMain();
			_icon.sprite = SpriteTextureLoader.getSprite(pAsset.path_icon);
		}
	}

	private void Update()
	{
		checkSpriteStatus();
	}

	private void checkSpriteStatus()
	{
		if (is_on)
		{
			_button_graphics.sprite = sprite_on;
			_background_circle.gameObject.SetActive(value: true);
		}
		else
		{
			_button_graphics.sprite = sprite_off;
			_background_circle.gameObject.SetActive(value: false);
		}
	}

	private void switchCategory()
	{
		if (!InputHelpers.mouseSupported && !Tooltip.isShowingFor(this))
		{
			showTooltip();
		}
		is_on = !is_on;
		_main_container.setCategoryEnabled(base.name, is_on);
	}

	private void showTooltip()
	{
		TooltipData tData = new TooltipData
		{
			tip_name = _asset.getLocaleID(),
			tip_description = _asset.getDescriptionID(),
			tip_description_2 = "graph_tip"
		};
		Tooltip.show(this, "tip", tData);
	}

	public void turnOff()
	{
		is_on = false;
	}

	public void turnOn()
	{
		is_on = true;
	}
}
