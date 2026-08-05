public class LanguageBanner : BannerGeneric<Language, LanguageData>
{
	protected override MetaType meta_type => MetaType.Language;

	protected override string tooltip_id => "language";

	protected override TooltipData getTooltipData()
	{
		TooltipData tooltipData = base.getTooltipData();
		tooltipData.language = meta_object;
		return tooltipData;
	}

	protected override void setupBanner()
	{
		base.setupBanner();
		part_background.sprite = meta_object.getBackgroundSprite();
		part_icon.sprite = meta_object.getIconSprite();
		ColorAsset tColorAsset = meta_object.getColor();
		part_background.color = tColorAsset.getColorMainSecond();
		part_icon.color = tColorAsset.getColorBanner();
	}
}
