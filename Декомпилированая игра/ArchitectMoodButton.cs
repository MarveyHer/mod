using System;
using UnityEngine;
using UnityEngine.UI;

public class ArchitectMoodButton : MonoBehaviour
{
	[SerializeField]
	protected Button button;

	[SerializeField]
	protected TipButton _tip_button;

	[SerializeField]
	protected Image _icon;

	[SerializeField]
	private Image _selected;

	private ArchitectMood _asset;

	private ArchitectMoodAction _click_callback;

	private void Awake()
	{
		button.onClick.AddListener(delegate
		{
			_click_callback?.Invoke(this);
		});
	}

	public ArchitectMood getAsset()
	{
		return _asset;
	}

	public virtual void setAsset(ArchitectMood pAsset)
	{
		_asset = pAsset;
		_icon.sprite = _asset.getSprite();
		_tip_button.textOnClick = pAsset.getLocaleID();
	}

	public void toggleSelectedButton(bool pState)
	{
		if (_selected != null)
		{
			_selected.color = Toolbox.makeColor(_asset.color_main);
			_selected.enabled = pState;
		}
	}

	public void setIconActiveColor(bool pState)
	{
		float tColorValue = ((!pState) ? 0.55f : 1f);
		Color tColor = new Color(tColorValue, tColorValue, tColorValue);
		_icon.color = tColor;
	}

	public void addClickCallback(ArchitectMoodAction pAction)
	{
		_click_callback = (ArchitectMoodAction)Delegate.Combine(_click_callback, pAction);
	}
}
