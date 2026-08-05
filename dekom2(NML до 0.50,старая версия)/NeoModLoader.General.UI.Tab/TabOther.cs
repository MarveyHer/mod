namespace NeoModLoader.General.UI.Tab;

public class TabOther : ReconstructedVanillaTab
{
	public const string INFO = "info";

	public const string STATUS = "status";

	public const string EDITOR_RAIN = "editor_rain";

	public const string LIFE_GAME = "life_game";

	public const string SHAPE_PRINTER = "shape_printer";

	protected override string[] Groups => new string[5] { "info", "status", "editor_rain", "life_game", "shape_printer" };

	protected override void InitTab()
	{
		tab = new WrappedPowersTab(PowerButtonCreator.GetTab("Tab_Other"));
	}
}
