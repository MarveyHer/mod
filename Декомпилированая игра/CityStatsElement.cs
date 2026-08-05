using System.Collections;
using UnityEngine;

public class CityStatsElement : CityElement, IStatsElement, IRefreshElement
{
	[SerializeField]
	private CityLoyaltyElement _loyalty_element;

	private StatsIconContainer _stats_icons;

	public void setIconValue(string pName, float pMainVal, float? pMax = null, string pColor = "", bool pFloat = false, string pEnding = "", char pSeparator = '/')
	{
		_stats_icons.setIconValue(pName, pMainVal, pMax, pColor, pFloat, pEnding, pSeparator);
	}

	protected override void Awake()
	{
		_stats_icons = base.gameObject.AddOrGetComponent<StatsIconContainer>();
		base.Awake();
	}

	protected override IEnumerator showContent()
	{
		if (base.city != null && base.city.isAlive())
		{
			int tPopulationPeople = base.city.getPopulationPeople();
			int tTotalFood = base.city.countFoodTotal();
			_stats_icons.showGeneralIcons<City, CityData>(base.city);
			if (tPopulationPeople > base.city.getPopulationMaximum())
			{
				setIconValue("i_population", tPopulationPeople, base.city.getPopulationMaximum(), "#FB2C21");
			}
			else
			{
				setIconValue("i_population", tPopulationPeople, base.city.getPopulationMaximum());
			}
			setIconValue("i_territory", base.city.countZones());
			setIconValue("i_boats", base.city.countBoats());
			CityStatsElement cityStatsElement = this;
			float pMainVal = tTotalFood;
			string pColor = ((tTotalFood > tPopulationPeople * 4) ? "#43FF43" : "#FB2C21");
			cityStatsElement.setIconValue("i_food", pMainVal, null, pColor);
			setIconValue("i_farmers", base.city.jobs.countOccupied(CitizenJobLibrary.farmer));
			setIconValue("i_books", base.city.countBooks());
			int tLoyalty = base.city.getLoyalty(pForceRecalc: true);
			if (tLoyalty > 0)
			{
				setIconValue("i_loyalty", tLoyalty, null, "#43FF43");
			}
			else
			{
				setIconValue("i_loyalty", tLoyalty, null, "#FB2C21");
			}
			_loyalty_element.setCity(base.city);
			if (WorldLawLibrary.world_law_civ_army.isEnabled())
			{
				setIconValue("i_army", base.city.countWarriors(), base.city.getMaxWarriors());
			}
			else
			{
				setIconValue("i_army", base.city.countWarriors());
			}
			setIconValue("i_houses", base.city.getHouseCurrent(), base.city.getHouseLimit());
		}
		yield break;
	}
}
