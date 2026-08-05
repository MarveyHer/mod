public class ActorTraitButton : TraitButton<ActorTrait>
{
	protected override string tooltip_type => "trait";

	internal override void load(string pTraitID)
	{
		ActorTrait tTrait = AssetManager.traits.get(pTraitID);
		load(tTrait);
	}

	protected override TooltipData tooltipDataBuilder()
	{
		return new TooltipData
		{
			trait = augmentation_asset
		};
	}
}
