using System.Collections.Generic;
using UnityEngine.UI;

public class CityWindow : WindowMetaGeneric<City, CityData>, IBooksWindow
{
	public Image raceTopIcon1;

	public Image raceTopIcon2;

	public LocalizedText village_title;

	public override MetaType meta_type => MetaType.City;

	protected override City meta_object => SelectedMetas.selected_city;

	public List<long> getBooks()
	{
		return meta_object.getBooks();
	}

	protected override void showTopPartInformation()
	{
		base.showTopPartInformation();
		City tCity = meta_object;
		if (tCity != null)
		{
			raceTopIcon1.sprite = tCity.getSpriteIcon();
			raceTopIcon2.sprite = tCity.getSpriteIcon();
		}
	}

	public override void startShowingWindow()
	{
		base.startShowingWindow();
		AchievementLibrary.checkCityAchievements(meta_object);
	}

	private void tryShowPastRulers()
	{
		List<LeaderEntry> past_rulers = meta_object.data.past_rulers;
		if (past_rulers != null && past_rulers.Count > 1)
		{
			showStatRow("past_leaders", meta_object.data.past_rulers.Count, MetaType.None, -1L, "iconVillages", "past_rulers", getTooltipPastRulers);
		}
	}

	private TooltipData getTooltipPastRulers()
	{
		return new TooltipData
		{
			tip_name = "past_leaders",
			meta_type = MetaType.City,
			past_rulers = new ListPool<LeaderEntry>(meta_object.data.past_rulers)
		};
	}

	protected override bool onNameChange(string pInput)
	{
		if (!base.onNameChange(pInput))
		{
			return false;
		}
		foreach (Religion tReligion in World.world.religions)
		{
			if (!tReligion.isRekt() && tReligion.data.creator_city_id == meta_object.getID())
			{
				tReligion.data.creator_city_name = meta_object.data.name;
			}
		}
		foreach (Culture tCulture in World.world.cultures)
		{
			if (!tCulture.isRekt() && tCulture.data.creator_city_id == meta_object.getID())
			{
				tCulture.data.creator_city_name = meta_object.data.name;
			}
		}
		foreach (Clan tClan in World.world.clans)
		{
			if (!tClan.isRekt() && tClan.data.founder_city_id == meta_object.getID())
			{
				tClan.data.founder_city_name = meta_object.data.name;
			}
		}
		foreach (Language tLanguage in World.world.languages)
		{
			if (!tLanguage.isRekt() && tLanguage.data.creator_city_id == meta_object.getID())
			{
				tLanguage.data.creator_city_name = meta_object.data.name;
			}
		}
		foreach (Family tFamily in World.world.families)
		{
			if (!tFamily.isRekt() && tFamily.data.founder_city_id == meta_object.getID())
			{
				tFamily.data.founder_city_name = meta_object.data.name;
			}
		}
		foreach (Book tBook in World.world.books)
		{
			if (!tBook.isRekt() && tBook.data.author_city_id == meta_object.getID())
			{
				tBook.data.author_city_name = meta_object.data.name;
			}
		}
		return true;
	}

	internal override void showStatsRows()
	{
		City tCity = meta_object;
		if (tCity != null)
		{
			if (tCity.kingdom.isNeutral())
			{
				village_title.setKeyAndUpdate("village_dying");
			}
			else
			{
				village_title.setKeyAndUpdate("village");
			}
			tryShowPastNames();
			showStatRow("founded", tCity.getFoundedDate(), MetaType.None, -1L, "iconAge");
			tryToShowActor("founder", tCity.data.founder_id, tCity.data.founder_name, null, "actor_traits/iconStupid");
			tryShowPastRulers();
			tryToShowActor("village_statistics_leader", -1L, null, tCity.leader, "iconLeaders");
			if (tCity.hasLeader())
			{
				showStatRow("ruler_money", tCity.leader.money, "#43FF43", MetaType.None, -1L, pColorText: false, "iconMoney");
			}
			string tTaxText = tCity.kingdom.getTaxRateLocal().ToString("0%");
			showStatRow("tax", tTaxText, "#43FF43", MetaType.None, -1L, pColorText: false, "kingdom_traits/kingdom_trait_tax_rate_local_low");
			string tTributeText = tCity.kingdom.getTaxRateTribute().ToString("0%");
			showStatRow("tribute", tTributeText, "#43FF43", MetaType.None, -1L, pColorText: false, "kingdom_traits/kingdom_trait_tax_rate_tribute_high");
			tryToShowActor("king", -1L, null, tCity.kingdom.king, "iconKings");
			tryToShowMetaSpecies("founder_species", tCity.getFounderSpecies()?.id);
		}
	}

	public override void showMetaRows()
	{
		City tCity = meta_object;
		if (tCity != null && !tCity.kingdom.isNeutral())
		{
			meta_rows_container.tryToShowMetaClan("clan", -1L, null, tCity.leader?.clan);
			meta_rows_container.tryToShowMetaKingdom("kingdom", -1L, null, tCity.kingdom);
			meta_rows_container.tryToShowMetaAlliance("alliance", -1L, null, tCity.kingdom.getAlliance());
			meta_rows_container.tryToShowMetaCulture("culture", -1L, null, tCity.culture);
			meta_rows_container.tryToShowMetaLanguage("language", -1L, null, tCity.language);
			meta_rows_container.tryToShowMetaReligion("religion", -1L, null, tCity.religion);
			meta_rows_container.tryToShowMetaSubspecies("main_subspecies", -1L, null, tCity.getMainSubspecies());
			meta_rows_container.tryToShowMetaArmy("army", -1L, null, tCity.army);
		}
	}

	public void clickTestItemProduction()
	{
		ItemCrafting.tryToCraftRandomWeapon(meta_object.units.GetRandom(), meta_object);
		scroll_window.tabs.showTab(scroll_window.tabs.getActiveTab());
	}

	public void clickTestClearItems()
	{
		meta_object.data.equipment.clearItems();
		scroll_window.tabs.showTab(scroll_window.tabs.getActiveTab());
	}

	public void clickTestNewBook()
	{
		if (meta_object.hasLeader() && meta_object.leader.hasCulture() && meta_object.leader.hasLanguage())
		{
			World.world.books.generateNewBook(meta_object.leader);
			meta_object.forceDoChecks();
			scroll_window.tabs.showTab(scroll_window.tabs.getActiveTab());
		}
	}
}
