using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ButtonResource : MonoBehaviour
{
	public Text textAmount;

	public ResourceAsset asset;

	public static float scaleTime = 0.1f;

	private void Start()
	{
		Button component = GetComponent<Button>();
		component.onClick.AddListener(showTooltip);
		component.OnHover(showHoverTooltip);
		component.OnHoverOut(Tooltip.hideTooltip);
	}

	internal void load(ResourceAsset pAsset, int pAmount)
	{
		asset = pAsset;
		if (asset != null)
		{
			GetComponent<Image>().sprite = pAsset.getSpriteIcon();
			textAmount.text = pAmount.ToString() ?? "";
		}
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
		string tTooltipId = asset.tooltip;
		Tooltip.show(this, tTooltipId, new TooltipData
		{
			resource = asset
		});
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		base.transform.DOKill();
		base.transform.DOScale(0.8f, scaleTime).SetEase(Ease.InBack);
	}

	private void OnDestroy()
	{
		base.transform.DOKill();
	}
}
