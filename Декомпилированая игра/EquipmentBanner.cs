using UnityEngine;

public class EquipmentBanner : BannerGeneric<Item, ItemData>
{
	[SerializeField]
	private IconOutline _outline;

	[SerializeField]
	private Sprite _frame_sprite_legendary;

	[SerializeField]
	private Sprite _frame_sprite_epic;

	protected override MetaType meta_type => MetaType.Item;

	protected override string tooltip_id => "equipment";

	protected override TooltipData getTooltipData()
	{
		TooltipData tooltipData = base.getTooltipData();
		tooltipData.item = meta_object;
		return tooltipData;
	}

	protected override void setupBanner()
	{
		base.setupBanner();
		Item tItem = meta_object;
		Rarity tItemQuality = tItem.getQuality();
		part_icon.sprite = tItem.getSprite();
		bool tFrameActive = true;
		switch (tItemQuality)
		{
		case Rarity.R3_Legendary:
			part_frame.sprite = _frame_sprite_legendary;
			break;
		case Rarity.R2_Epic:
			part_frame.sprite = _frame_sprite_epic;
			break;
		default:
			tFrameActive = false;
			break;
		}
		part_frame.gameObject.SetActive(tFrameActive);
		if (tItemQuality == Rarity.R3_Legendary)
		{
			showOutline();
		}
		else
		{
			_outline.gameObject.SetActive(value: false);
		}
	}

	private void showOutline()
	{
		_outline.show(RarityLibrary.legendary.color_container);
	}
}
