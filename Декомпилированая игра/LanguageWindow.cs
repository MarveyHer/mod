using System.Collections.Generic;

public class LanguageWindow : WindowMetaGeneric<Language, LanguageData>, ITraitWindow<LanguageTrait, LanguageTraitButton>, IAugmentationsWindow<ITraitsEditor<LanguageTrait>>, IBooksWindow
{
	public override MetaType meta_type => MetaType.Language;

	protected override Language meta_object => SelectedMetas.selected_language;

	protected override void showTopPartInformation()
	{
		base.showTopPartInformation();
		AchievementLibrary.multiply_spoken.checkBySignal(meta_object);
	}

	internal override void showStatsRows()
	{
		Language tLanguage = meta_object;
		tryShowPastNames();
		showStatRow("founded", tLanguage.getFoundedDate(), MetaType.None, -1L, "iconAge");
		tryToShowActor("creator", tLanguage.data.creator_id, tLanguage.data.creator_name, null, "actor_traits/iconStupid");
		tryToShowMetaClan("creators_clan", tLanguage.data.creator_clan_id, tLanguage.data.creator_clan_name);
		tryToShowMetaKingdom("origin", tLanguage.data.creator_kingdom_id, tLanguage.data.creator_kingdom_name);
		tryToShowMetaCity("birthplace", tLanguage.data.creator_city_id, tLanguage.data.creator_city_name);
		tryToShowMetaSubspecies("creator_subspecies", tLanguage.data.creator_subspecies_id, tLanguage.data.creator_subspecies_name);
		tryToShowMetaSpecies("creator_species", tLanguage.data.creator_species_id);
	}

	public List<long> getBooks()
	{
		return meta_object.books.getList();
	}

	protected override bool onNameChange(string pInput)
	{
		if (!base.onNameChange(pInput))
		{
			return false;
		}
		long tLanguageId = meta_object.getID();
		string tLanguageName = meta_object.data.name;
		foreach (Book tBook in World.world.books)
		{
			if (!tBook.isRekt() && tBook.data.language_id == tLanguageId)
			{
				tBook.data.language_name = tLanguageName;
			}
		}
		return true;
	}

	T IAugmentationsWindow<ITraitsEditor<LanguageTrait>>.GetComponentInChildren<T>(bool includeInactive)
	{
		return GetComponentInChildren<T>(includeInactive);
	}
}
