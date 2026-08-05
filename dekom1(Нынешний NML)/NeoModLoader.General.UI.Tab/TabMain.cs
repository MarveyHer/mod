namespace NeoModLoader.General.UI.Tab;

public class TabMain : ReconstructedVanillaTab
{
	public const string WORLD_INFO = "world_info";

	public const string REBUILD = "rebuild";

	public const string GAME_SETTING = "game_setting";

	public const string OTHERS = "others";

	public const string CUSTOM = "custom";

	private static readonly string[] _groups = new string[5] { "world_info", "rebuild", "game_setting", "others", "custom" };

	protected override string[] Groups => _groups;

	protected override void InitTab()
	{
		tab = new WrappedPowersTab(PowerButtonCreator.GetTab("main"));
	}
}
