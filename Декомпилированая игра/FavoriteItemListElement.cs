using UnityEngine;
using UnityEngine.UI;

public class FavoriteItemListElement : WindowListElementBase<Item, ItemData>
{
	public Text name_text;

	public CountUpOnClick kills_text;

	public CountUpOnClick age_text;

	public CountUpOnClick owners_text;

	public CountUpOnClick damage_text;

	public CountUpOnClick armor_text;

	public CountUpOnClick durability_text;

	[SerializeField]
	private UiUnitAvatarElement _unit_avatar_element;

	[SerializeField]
	private CityBanner _banner_city;

	[SerializeField]
	private GameObject _ownerless;

	private IconOutline _outline;

	internal override void show(Item pItem)
	{
		base.show(pItem);
		clear();
		name_text.text = pItem.getName();
		name_text.color = Toolbox.makeColor(pItem.getQualityColor());
		kills_text.setValue(pItem.data.kills);
		age_text.setValue(pItem.getAge());
		damage_text.setValue((int)pItem.getFullStats()["damage"]);
		armor_text.setValue((int)pItem.getFullStats()["armor"]);
		durability_text.setValue(pItem.getDurabilityCurrent());
		if (pItem.hasActor())
		{
			_unit_avatar_element.gameObject.SetActive(value: true);
			_unit_avatar_element.show(pItem.getActor());
		}
		else if (pItem.hasCity())
		{
			_banner_city.gameObject.SetActive(value: true);
			_banner_city.load(pItem.getCity());
		}
		else
		{
			_ownerless.SetActive(value: true);
		}
	}

	protected override void tooltipAction()
	{
		Tooltip.show(this, "equipment", new TooltipData
		{
			item = meta_object
		});
	}

	private void clear()
	{
		_unit_avatar_element.gameObject.SetActive(value: false);
		_banner_city.gameObject.SetActive(value: false);
		_ownerless.SetActive(value: false);
	}
}
