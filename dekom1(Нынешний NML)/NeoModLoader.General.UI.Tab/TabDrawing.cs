namespace NeoModLoader.General.UI.Tab;

public class TabDrawing : ReconstructedVanillaTab
{
	public const string TILE_BRUSH = "tile_brush";

	public const string MAP_HELPER = "map_helper";

	public const string CLEANER = "cleaner";

	public const string DELETOR = "deletor";

	protected override string[] Groups => new string[4] { "tile_brush", "map_helper", "cleaner", "deletor" };

	protected override void InitTab()
	{
		tab = new WrappedPowersTab(PowerButtonCreator.GetTab("creation"));
	}
}
