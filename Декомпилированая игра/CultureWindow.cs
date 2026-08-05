using System.Collections.Generic;

public class CultureWindow : WindowMetaGeneric<Culture, CultureData>, ITraitWindow<CultureTrait, CultureTraitButton>, IAugmentationsWindow<ITraitsEditor<CultureTrait>>, IBooksWindow
{
	public StatBar experienceBar;

	public override MetaType meta_type => MetaType.Culture;

	protected override Culture meta_object => SelectedMetas.selected_culture;

	public void testDebugNewBook()
	{
		meta_object.testDebugNewBook();
		startShowingWindow();
		scroll_window.tabs.showTab(scroll_window.tabs.getActiveTab());
	}

	public List<long> getBooks()
	{
		return meta_object.books.getList();
	}

	protected override void showTopPartInformation()
	{
		base.showTopPartInformation();
		_ = meta_object;
	}

	internal override void showStatsRows()
	{
		Culture tCulture = meta_object;
		tryShowPastNames();
		showStatRow("founded", tCulture.getFoundedDate(), MetaType.None, -1L);
		tryToShowActor("founder", tCulture.data.creator_id, tCulture.data.creator_name, null, "actor_traits/iconStupid");
		tryToShowMetaClan("founder_clan", tCulture.data.creator_clan_id, tCulture.data.creator_clan_name);
		tryToShowMetaKingdom("origin", tCulture.data.creator_kingdom_id, tCulture.data.creator_kingdom_name);
		tryToShowMetaCity("birthplace", tCulture.data.creator_city_id, tCulture.data.creator_city_name);
		tryToShowMetaSubspecies("founder_subspecies", tCulture.data.creator_subspecies_id, tCulture.data.creator_subspecies_name);
		tryToShowMetaSpecies("founder_species", tCulture.data.creator_species_id);
	}

	protected override bool onNameChange(string pInput)
	{
		if (!base.onNameChange(pInput))
		{
			return false;
		}
		long tCultureId = meta_object.getID();
		string tCultureName = meta_object.data.name;
		foreach (Book tBook in World.world.books)
		{
			if (!tBook.isRekt() && tBook.data.culture_id == tCultureId)
			{
				tBook.data.culture_name = tCultureName;
			}
		}
		return true;
	}

	T IAugmentationsWindow<ITraitsEditor<CultureTrait>>.GetComponentInChildren<T>(bool includeInactive)
	{
		return GetComponentInChildren<T>(includeInactive);
	}
}
