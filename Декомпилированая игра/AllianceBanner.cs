using UnityEngine;

public class AllianceBanner : BannerGeneric<Alliance, AllianceData>
{
	public Sprite frame_normal;

	public Sprite frame_forced;

	protected override MetaType meta_type => MetaType.Alliance;

	protected override string tooltip_id => "alliance";

	protected override TooltipData getTooltipData()
	{
		TooltipData tooltipData = base.getTooltipData();
		tooltipData.alliance = meta_object;
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
		if (meta_object.isNormalType())
		{
			part_frame.sprite = frame_normal;
		}
		else
		{
			part_frame.sprite = frame_forced;
		}
	}
}
