using UnityEngine.UI;

public class FamilyWindow : WindowMetaGeneric<Family, FamilyData>
{
	public Text title_family;

	public override MetaType meta_type => MetaType.Family;

	protected override Family meta_object => SelectedMetas.selected_family;

	protected override void showTopPartInformation()
	{
		base.showTopPartInformation();
		Family tFamily = meta_object;
		if (tFamily != null)
		{
			ActorAsset tAsset = tFamily.getActorAsset();
			title_family.text = LocalizedTextManager.getText(tAsset.getCollectiveTermID());
		}
	}

	internal override void showStatsRows()
	{
		Family tFamily = meta_object;
		tryShowPastNames();
		showStatRow("founded", tFamily.getFoundedDate(), MetaType.None, -1L, "iconAge");
		tryToShowActor("founder", tFamily.data.main_founder_id_1, tFamily.data.founder_actor_name_1, null, "actor_traits/iconStupid");
		if (tFamily.data.main_founder_id_2 != -1)
		{
			tryToShowActor("founder", tFamily.data.main_founder_id_2, tFamily.data.founder_actor_name_2, null, "actor_traits/iconStupid");
		}
		tryToShowMetaKingdom("origin", tFamily.data.founder_kingdom_id, tFamily.data.founder_kingdom_name);
		tryToShowMetaCity("birthplace", tFamily.data.founder_city_id, tFamily.data.founder_city_name);
		tryToShowMetaSubspecies("founder_subspecies", tFamily.data.subspecies_id, tFamily.data.subspecies_name);
		foreach (Family tOriginFamily in tFamily.getOriginFamilies())
		{
			tryToShowMetaFamily("origin_family", -1L, null, tOriginFamily);
		}
		tryToShowMetaSpecies("founder_species", tFamily.data.species_id);
	}
}
