namespace NeoModLoader.General.UI.Tab;

public class TabBombs : ReconstructedVanillaTab
{
	protected override string[] Groups => new string[0];

	protected override void InitTab()
	{
		tab = new WrappedPowersTab(PowerButtonCreator.GetTab("Tab_Bombs"));
	}
}
