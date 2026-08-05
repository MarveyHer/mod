using UnityEngine;
using UnityEngine.UI;

public class ArmyBanner : BannerGeneric<Army, ArmyData>
{
	[SerializeField]
	private Image _species_icon;

	protected override MetaType meta_type => MetaType.Army;

	protected override string tooltip_id => "army";

	protected override void setupBanner()
	{
		base.setupBanner();
		Kingdom tKingdom = meta_object.getKingdom();
		part_background.sprite = tKingdom.getElementBackground();
		part_icon.sprite = tKingdom.getElementIcon();
		ColorAsset colorAsset = tKingdom.getColor();
		Color tColorMain2 = colorAsset.getColorMainSecond();
		Color tColorIcon = colorAsset.getColorBanner();
		tColorMain2 = Color.Lerp(tColorMain2, Color.black, 0.05f);
		tColorIcon = Color.Lerp(tColorIcon, Color.black, 0.05f);
		part_background.color = tColorMain2;
		part_icon.color = tColorIcon;
		_species_icon.gameObject.SetActive(value: false);
	}

	protected override TooltipData getTooltipData()
	{
		TooltipData tooltipData = base.getTooltipData();
		tooltipData.army = meta_object;
		return tooltipData;
	}
}
