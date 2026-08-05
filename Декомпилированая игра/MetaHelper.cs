public static class MetaHelper
{
	public static void addRandomTrait<TTrait>(ITraitsOwner<TTrait> pMetaObject, BaseTraitLibrary<TTrait> pLibrary) where TTrait : BaseTrait<TTrait>
	{
		int tMin = 1;
		int tMax = 3;
		if (WorldLawLibrary.world_law_glitched_noosphere.isEnabled())
		{
			tMin = 3;
			tMax = 6;
		}
		int tAmount = Randy.randomInt(tMin, tMax);
		for (int i = 0; i < tAmount; i++)
		{
			TTrait tTrait = pLibrary.getRandomSpawnTrait();
			if (tTrait.isAvailable())
			{
				pMetaObject.addTrait(tTrait, pRemoveOpposites: true);
			}
		}
	}
}
