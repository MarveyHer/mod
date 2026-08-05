using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AchievementButton : MonoBehaviour
{
	private Achievement _achievement;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private Image _background_completed;

	[SerializeField]
	private Image _background_legendary;

	[SerializeField]
	private GameObject _background_default;

	[SerializeField]
	private GameObject _icon_medal;

	public void Load(Achievement pAchievement)
	{
		_achievement = pAchievement;
		Sprite tNewSprite = _achievement.getIcon();
		if (tNewSprite != null)
		{
			_icon.sprite = tNewSprite;
			if (!AchievementLibrary.isUnlocked(_achievement))
			{
				_icon.color = Color.black;
				_background_default.SetActive(value: true);
				_background_completed.GetComponent<Image>().enabled = false;
				_icon_medal.SetActive(value: false);
			}
		}
		if (pAchievement.unlocks_something)
		{
			_background_legendary.gameObject.SetActive(value: true);
		}
		else
		{
			_background_legendary.gameObject.SetActive(value: false);
		}
		base.name = _achievement.id;
	}

	private void Start()
	{
		base.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
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
		Tooltip.show(this, "achievement", new TooltipData
		{
			achievement = _achievement
		});
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		base.transform.DOKill();
		base.transform.DOScale(0.8f, 0.1f).SetEase(Ease.InBack);
	}
}
