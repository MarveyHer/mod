using UnityEngine;
using UnityEngine.UI;

public class KingdomListElement : WindowListElementBase<Kingdom, KingdomData>
{
	public CountUpOnClick textAge;

	public CountUpOnClick textPopulation;

	public CountUpOnClick textArmy;

	public CountUpOnClick textCities;

	public CountUpOnClick textHouses;

	public CountUpOnClick textZones;

	public Text kingdomName;

	public GameObject buttonCapital;

	public GameObject buttonKing;

	public UiUnitAvatarElement avatarLoader;

	internal override void show(Kingdom pKingdom)
	{
		base.show(pKingdom);
		kingdomName.text = pKingdom.name;
		Color tColor = pKingdom.getColor().getColorText();
		kingdomName.color = tColor;
		avatarLoader.show(pKingdom.king);
		int tZones = 0;
		int tHouses = 0;
		int tCities = 0;
		foreach (City tCity in pKingdom.getCities())
		{
			tCities++;
			tZones += tCity.zones.Count;
			tHouses += tCity.buildings.Count;
		}
		textPopulation.setValue(pKingdom.getPopulationPeople());
		textArmy.setValue(pKingdom.countTotalWarriors());
		textZones.setValue(tZones);
		textHouses.setValue(tHouses);
		textCities.setValue(tCities, "/" + pKingdom.getMaxCities());
		textAge.setValue(pKingdom.getAge());
	}

	protected override void tooltipAction()
	{
		Tooltip.show(this, "kingdom", new TooltipData
		{
			kingdom = meta_object
		});
	}

	protected override ActorAsset getActorAsset()
	{
		return meta_object.getActorAsset();
	}
}
