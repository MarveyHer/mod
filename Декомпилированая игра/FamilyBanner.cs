public class FamilyBanner : BannerGeneric<Family, FamilyData>
{
	protected override MetaType meta_type => MetaType.Family;

	protected override string tooltip_id => "family";

	protected override TooltipData getTooltipData()
	{
		TooltipData tooltipData = base.getTooltipData();
		tooltipData.family = meta_object;
		return tooltipData;
	}

	protected override void setupBanner()
	{
		base.setupBanner();
		part_background.sprite = meta_object.getSpriteBackground();
		part_icon.sprite = meta_object.getSpriteIcon();
		part_frame.sprite = meta_object.getSpriteFrame();
		ColorAsset tColorAsset = meta_object.getColor();
		part_background.color = tColorAsset.getColorMainSecond();
	}
}
