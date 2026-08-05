public class ClanTraitButton : TraitButton<ClanTrait>
{
	protected override string tooltip_type => "clan_trait";

	internal override void load(string pTraitID)
	{
		ClanTrait tTrait = AssetManager.clan_traits.get(pTraitID);
		load(tTrait);
	}

	protected override void startSignal()
	{
		AchievementLibrary.trait_explorer_clan.checkBySignal();
	}

	protected override TooltipData tooltipDataBuilder()
	{
		return new TooltipData
		{
			clan_trait = augmentation_asset
		};
	}
}
