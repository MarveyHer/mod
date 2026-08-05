public class CultureTraitButton : TraitButton<CultureTrait>
{
	protected override string tooltip_type => "culture_trait";

	internal override void load(string pTraitID)
	{
		CultureTrait tTrait = AssetManager.culture_traits.get(pTraitID);
		load(tTrait);
	}

	protected override void startSignal()
	{
		AchievementLibrary.trait_explorer_culture.checkBySignal();
	}

	protected override TooltipData tooltipDataBuilder()
	{
		return new TooltipData
		{
			culture_trait = augmentation_asset
		};
	}
}
