namespace NeoModLoader.General.UI.Tab;

public class TabNature : ReconstructedVanillaTab
{
	public const string PHENOMENON = "phenomenon";

	public const string BIOMES = "biomes";

	public const string FERTILITY = "fertility";

	public const string RESOURCES = "resources";

	public const string DROP = "drop";

	protected override string[] Groups => new string[5] { "phenomenon", "biomes", "fertility", "resources", "drop" };

	protected override void InitTab()
	{
		tab = new WrappedPowersTab(PowerButtonCreator.GetTab("Tab_Nature"));
	}
}
