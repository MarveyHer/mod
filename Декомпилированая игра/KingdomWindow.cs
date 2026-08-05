using System.Collections.Generic;
using UnityEngine.UI;

public class KingdomWindow : WindowMetaGeneric<Kingdom, KingdomData>, ITraitWindow<KingdomTrait, KingdomTraitButton>, IAugmentationsWindow<ITraitsEditor<KingdomTrait>>
{
	public Image raceTopIcon1;

	public Image raceTopIcon2;

	public NameInput mottoInput;

	public override MetaType meta_type => MetaType.Kingdom;

	protected override Kingdom meta_object => SelectedMetas.selected_kingdom;

	protected override void initNameInput()
	{
		base.initNameInput();
		mottoInput.addListener(applyInputMotto);
	}

	protected override bool onNameChange(string pInput)
	{
		if (!base.onNameChange(pInput))
		{
			return false;
		}
		long tKingdomID = meta_object.getID();
		string tKingdomName = meta_object.data.name;
		foreach (War tWar in World.world.wars)
		{
			if (!tWar.isRekt() && tWar.data.started_by_kingdom_id == tKingdomID)
			{
				tWar.data.started_by_kingdom_name = tKingdomName;
			}
		}
		foreach (Alliance tAlliance in World.world.alliances)
		{
			if (!tAlliance.isRekt() && tAlliance.data.founder_kingdom_id == tKingdomID)
			{
				tAlliance.data.founder_kingdom_name = tKingdomName;
			}
		}
		foreach (Religion tReligion in World.world.religions)
		{
			if (!tReligion.isRekt() && tReligion.data.creator_kingdom_id == tKingdomID)
			{
				tReligion.data.creator_kingdom_name = tKingdomName;
			}
		}
		foreach (Culture tCulture in World.world.cultures)
		{
			if (!tCulture.isRekt() && tCulture.data.creator_kingdom_id == tKingdomID)
			{
				tCulture.data.creator_kingdom_name = tKingdomName;
			}
		}
		foreach (Clan tClan in World.world.clans)
		{
			if (!tClan.isRekt() && tClan.data.founder_kingdom_id == tKingdomID)
			{
				tClan.data.founder_kingdom_name = tKingdomName;
			}
		}
		foreach (Language tLanguage in World.world.languages)
		{
			if (!tLanguage.isRekt() && tLanguage.data.creator_kingdom_id == tKingdomID)
			{
				tLanguage.data.creator_kingdom_name = tKingdomName;
			}
		}
		foreach (Family tFamily in World.world.families)
		{
			if (!tFamily.isRekt() && tFamily.data.founder_kingdom_id == tKingdomID)
			{
				tFamily.data.founder_kingdom_name = tKingdomName;
			}
		}
		foreach (Book tBook in World.world.books)
		{
			if (!tBook.isRekt() && tBook.data.author_kingdom_id == tKingdomID)
			{
				tBook.data.author_kingdom_name = tKingdomName;
			}
		}
		foreach (Item tItem in World.world.items)
		{
			if (!tItem.isRekt() && tItem.data.creator_kingdom_id == tKingdomID)
			{
				tItem.data.from = tKingdomName;
			}
		}
		foreach (Army tArmy in World.world.armies)
		{
			if (!tArmy.isRekt() && tArmy.getKingdom() == meta_object)
			{
				tArmy.onKingdomNameChange();
			}
		}
		return true;
	}

	private void applyInputMotto(string pInput)
	{
		if (pInput != null && meta_object != null)
		{
			meta_object.data.motto = pInput;
		}
	}

	protected override void showTopPartInformation()
	{
		base.showTopPartInformation();
		Kingdom tKingdom = meta_object;
		if (tKingdom != null)
		{
			raceTopIcon1.sprite = tKingdom.getSpriteIcon();
			raceTopIcon2.sprite = tKingdom.getSpriteIcon();
			mottoInput.setText(tKingdom.getMotto());
			mottoInput.textField.color = tKingdom.getColor().getColorText();
		}
	}

	private void tryShowPastRulers()
	{
		List<LeaderEntry> past_rulers = meta_object.data.past_rulers;
		if (past_rulers != null && past_rulers.Count > 1)
		{
			showStatRow("past_kings", meta_object.data.past_rulers.Count, MetaType.None, -1L, "iconKingdomList", "past_rulers", getTooltipPastRulers);
		}
	}

	private TooltipData getTooltipPastRulers()
	{
		return new TooltipData
		{
			tip_name = "past_kings",
			meta_type = MetaType.Kingdom,
			past_rulers = new ListPool<LeaderEntry>(meta_object.data.past_rulers)
		};
	}

	internal override void showStatsRows()
	{
		Kingdom tKingdom = meta_object;
		tryShowPastNames();
		showStatRow("founded", tKingdom.getFoundedDate(), MetaType.None, -1L, "iconAge");
		tryShowPastRulers();
		tryToShowActor("king", -1L, null, tKingdom.king, "iconKings");
		Actor tHeir = SuccessionTool.findNextHeir(tKingdom, tKingdom.king);
		tryToShowActor("heir", -1L, null, tHeir, "iconChildren");
		if (tKingdom.hasKing())
		{
			if (tKingdom.king.s_personality != null)
			{
				showStatRow("creature_statistics_personality", tKingdom.king.s_personality.getTranslatedName(), MetaType.None, -1L, "actor_traits/iconStupid");
			}
			int tKingRuleAge = Date.getYearsSince(tKingdom.data.timestamp_king_rule);
			showStatRow("kingdom_statistics_king_ruled", tKingRuleAge, MetaType.None, -1L, "iconClock");
			showStatRow("ruler_money", tKingdom.king.money, "#43FF43", MetaType.None, -1L, pColorText: false, "iconMoney");
		}
		string tTributeText = tKingdom.getTaxRateTribute().ToString("0%");
		showStatRow("tribute", tTributeText, "#43FF43", MetaType.None, -1L, pColorText: false, "kingdom_traits/kingdom_trait_tax_rate_tribute_high");
		tryToShowMetaSpecies("founder_species", tKingdom.getFounderSpecies().id);
	}

	public override void showMetaRows()
	{
		Kingdom tKingdom = meta_object;
		meta_rows_container.tryToShowMetaAlliance("alliance", -1L, null, tKingdom.getAlliance());
		meta_rows_container.tryToShowMetaCity("kingdom_statistics_capital", -1L, null, tKingdom.capital, "iconKingdom");
		meta_rows_container.tryToShowMetaClan("clan", -1L, null, tKingdom.king?.clan);
		meta_rows_container.tryToShowMetaCulture("culture", -1L, null, tKingdom.culture);
		meta_rows_container.tryToShowMetaLanguage("language", -1L, null, tKingdom.language);
		meta_rows_container.tryToShowMetaReligion("religion", -1L, null, tKingdom.religion);
		meta_rows_container.tryToShowMetaSubspecies("main_subspecies", -1L, null, tKingdom.getMainSubspecies());
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		mottoInput.inputField.DeactivateInputField();
	}

	public void clickCapital()
	{
		SelectedMetas.selected_city = meta_object.capital;
		ScrollWindow.showWindow("city");
	}

	T IAugmentationsWindow<ITraitsEditor<KingdomTrait>>.GetComponentInChildren<T>(bool includeInactive)
	{
		return GetComponentInChildren<T>(includeInactive);
	}
}
