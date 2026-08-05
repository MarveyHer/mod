using System.Collections.Generic;

public class ClanWindow : WindowMetaGeneric<Clan, ClanData>, ITraitWindow<ClanTrait, ClanTraitButton>, IAugmentationsWindow<ITraitsEditor<ClanTrait>>
{
	public NameInput nameInput;

	public NameInput mottoInput;

	public override MetaType meta_type => MetaType.Clan;

	protected override Clan meta_object => SelectedMetas.selected_clan;

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
		long tClanId = meta_object.getID();
		string tClanName = meta_object.data.name;
		foreach (Culture tCulture in World.world.cultures)
		{
			if (!tCulture.isRekt() && tCulture.data.creator_clan_id == tClanId)
			{
				tCulture.data.creator_clan_name = tClanName;
			}
		}
		foreach (Religion tReligion in World.world.religions)
		{
			if (!tReligion.isRekt() && tReligion.data.creator_clan_id == tClanId)
			{
				tReligion.data.creator_clan_name = tClanName;
			}
		}
		foreach (Language tLanguage in World.world.languages)
		{
			if (!tLanguage.isRekt() && tLanguage.data.creator_clan_id == tClanId)
			{
				tLanguage.data.creator_clan_name = tClanName;
			}
		}
		foreach (Book tBook in World.world.books)
		{
			if (!tBook.isRekt() && tBook.data.author_clan_id == tClanId)
			{
				tBook.data.author_clan_name = tClanName;
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
		Clan tClan = meta_object;
		if (tClan != null)
		{
			mottoInput.setText(tClan.getMotto());
			mottoInput.textField.color = tClan.getColor().getColorText();
		}
	}

	internal override void showStatsRows()
	{
		Clan tClan = meta_object;
		tryShowPastNames();
		showStatRow("founded", tClan.getFoundedDate(), MetaType.None, -1L, "iconAge");
		tryToShowActor("clan_founder", tClan.data.founder_actor_id, tClan.data.founder_actor_name, null, "actor_traits/iconStupid");
		tryToShowActor("clan_chief_title", -1L, null, tClan.getChief(), "iconClan");
		tryShowPastChiefs();
		Actor tNextChief = tClan.getNextChief();
		tryToShowActor("clan_heir", -1L, null, tNextChief, "iconClanList");
		tryToShowMetaCulture("culture", -1L, null, tClan.getClanCulture());
		tryToShowMetaKingdom("origin", tClan.data.founder_kingdom_id, tClan.data.founder_kingdom_name);
		tryToShowMetaCity("birthplace", tClan.data.founder_city_id, tClan.data.founder_city_name);
		tryToShowMetaSubspecies("original_subspecies", tClan.data.creator_subspecies_id, tClan.data.creator_subspecies_name);
		tryToShowMetaSpecies("species", tClan.data.creator_species_id);
	}

	private void tryShowPastChiefs()
	{
		List<LeaderEntry> past_chiefs = meta_object.data.past_chiefs;
		if (past_chiefs != null && past_chiefs.Count > 1)
		{
			showStatRow("past_chiefs", meta_object.data.past_chiefs.Count, MetaType.None, -1L, "iconCaptain", "past_rulers", getTooltipPastChiefs);
		}
	}

	private TooltipData getTooltipPastChiefs()
	{
		return new TooltipData
		{
			tip_name = "past_chiefs",
			meta_type = MetaType.Clan,
			past_rulers = new ListPool<LeaderEntry>(meta_object.data.past_chiefs)
		};
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		mottoInput.inputField.DeactivateInputField();
	}

	public void debugClearExpLevel()
	{
		OnEnable();
	}

	T IAugmentationsWindow<ITraitsEditor<ClanTrait>>.GetComponentInChildren<T>(bool includeInactive)
	{
		return GetComponentInChildren<T>(includeInactive);
	}
}
