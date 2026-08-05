using UnityEngine.UI;

public class CultureBanner : BannerGeneric<Culture, CultureData>
{
	protected override MetaType meta_type => MetaType.Culture;

	protected override string tooltip_id => "culture";

	protected override void loadPartBackground()
	{
		part_background = base.transform.FindRecursive("Decor").GetComponent<Image>();
	}

	protected override TooltipData getTooltipData()
	{
		TooltipData tooltipData = base.getTooltipData();
		tooltipData.culture = meta_object;
		return tooltipData;
	}

	protected override void setupBanner()
	{
		base.setupBanner();
		part_icon.sprite = meta_object.getElementSprite();
		part_background.sprite = meta_object.getDecorSprite();
		ColorAsset tColorAsset = meta_object.getColor();
		part_icon.color = tColorAsset.getColorBanner();
		part_background.color = tColorAsset.getColorMainSecond();
		part_frame.color = tColorAsset.getColorMainSecond();
	}
}
