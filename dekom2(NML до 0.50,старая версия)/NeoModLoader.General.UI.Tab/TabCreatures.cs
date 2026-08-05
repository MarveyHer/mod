namespace NeoModLoader.General.UI.Tab;

public class TabCreatures : ReconstructedVanillaTab
{
	public const string RACES = "races";

	public const string LAND_CREATURES = "land_creatures";

	public const string SEA_CREATURES = "sea_creatures";

	public const string MAGICAL_CREATURES = "magical_creatures";

	public const string UNDEAD_CREATURES = "undead_creatures";

	public const string IMPROPER_CREATURES = "improper_creatures";

	protected override string[] Groups => new string[6] { "races", "land_creatures", "sea_creatures", "magical_creatures", "undead_creatures", "improper_creatures" };

	protected override void InitTab()
	{
		tab = new WrappedPowersTab(PowerButtonCreator.GetTab("Tab_Creatures"));
	}
}
