using UnityEngine;
using UnityEngine.UI;

public class ButtonClickMaptemplate : MonoBehaviour
{
	private Button _button;

	private MapGenTemplate _template;

	private void Awake()
	{
		string tTemplateID = base.transform.name;
		_button = GetComponent<Button>();
		_button.onClick.AddListener(click);
		if (Input.mousePresent)
		{
			_button.OnHover(delegate
			{
				if (InputHelpers.mouseSupported)
				{
					showTooltip();
				}
			});
			_button.OnHoverOut(delegate
			{
				if (InputHelpers.mouseSupported)
				{
					Tooltip.hideTooltip();
				}
			});
		}
		_template = AssetManager.map_gen_templates.get(tTemplateID);
		base.transform.Find("preview_icon").GetComponent<Image>().sprite = SpriteTextureLoader.getSprite(_template.path_icon);
	}

	private void showTooltip()
	{
		Tooltip.show(_button.gameObject, "normal", new TooltipData
		{
			tip_name = _template.getLocaleID(),
			tip_description = _template.getDescriptionID()
		});
	}

	public void click()
	{
		if (!InputHelpers.mouseSupported)
		{
			if (!Tooltip.isShowingFor(_button.gameObject))
			{
				showTooltip();
				return;
			}
			Tooltip.hideTooltipNow();
		}
		Config.current_map_template = _template.id;
		ScrollWindow.showWindow("new_world_templates_2");
	}
}
