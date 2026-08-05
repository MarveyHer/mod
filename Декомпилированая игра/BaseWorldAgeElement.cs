using System;
using UnityEngine;
using UnityEngine.UI;

public class BaseWorldAgeElement : MonoBehaviour
{
	[SerializeField]
	protected Button button;

	[SerializeField]
	protected TipButton _tip_button;

	[SerializeField]
	protected Image _icon;

	protected WorldAgeAsset asset;

	protected WorldAgeElementAction click_callback;

	private void Awake()
	{
		prepare();
	}

	protected virtual void prepare()
	{
		button.onClick.AddListener(delegate
		{
			click_callback?.Invoke(this);
		});
	}

	public WorldAgeAsset getAsset()
	{
		return asset;
	}

	public virtual void setAge(WorldAgeAsset pAsset)
	{
		asset = pAsset;
		_icon.sprite = asset.getSprite();
		_tip_button.type = "world_age";
		_tip_button.textOnClick = pAsset.id;
	}

	public void setIconActiveColor(bool pState)
	{
		float tColorValue = ((!pState) ? 0.55f : 1f);
		Color tColor = new Color(tColorValue, tColorValue, tColorValue);
		_icon.color = tColor;
	}

	public void addClickCallback(WorldAgeElementAction pAction)
	{
		click_callback = (WorldAgeElementAction)Delegate.Combine(click_callback, pAction);
	}

	public void removeClickCallback(WorldAgeElementAction pAction)
	{
		click_callback = (WorldAgeElementAction)Delegate.Remove(click_callback, pAction);
	}

	public WorldAgeElementAction getClickCallback()
	{
		return click_callback;
	}

	public void clearClickCallbacks()
	{
		click_callback = null;
	}

	public Button getButton()
	{
		return button;
	}
}
