using UnityEngine;
using UnityEngine.UI;

public class ArmyListElement : WindowListElementBase<Army, ArmyData>
{
	[SerializeField]
	private Text _text_name;

	[SerializeField]
	private CountUpOnClick _amount;

	[SerializeField]
	private CountUpOnClick _age;

	[SerializeField]
	private CountUpOnClick _renown;

	[SerializeField]
	private CountUpOnClick _kills;

	[SerializeField]
	private CountUpOnClick _deaths;

	[SerializeField]
	private UiUnitAvatarElement _captain;

	[SerializeField]
	private ArmyBanner _army_banner;

	internal override void show(Army pArmy)
	{
		base.show(pArmy);
		_text_name.text = pArmy.name;
		Color tColor = pArmy.getColor().getColorText();
		_text_name.color = tColor;
		bool tHasCaptain = pArmy.hasCaptain();
		_captain.gameObject.SetActive(tHasCaptain);
		if (tHasCaptain)
		{
			_captain.show(pArmy.getCaptain());
		}
		_amount.setValue(pArmy.countUnits());
		_age.setValue(pArmy.getAge());
		_renown.setValue(pArmy.getRenown());
		_kills.setValue((int)pArmy.getTotalKills());
		_deaths.setValue((int)pArmy.getTotalDeaths());
	}

	protected override void initMonoFields()
	{
	}

	protected override void loadBanner()
	{
		_army_banner.load(meta_object);
	}

	protected override void tooltipAction()
	{
		Tooltip.show(this, "army", new TooltipData
		{
			army = meta_object
		});
	}

	protected override ActorAsset getActorAsset()
	{
		return meta_object.getActorAsset();
	}
}
