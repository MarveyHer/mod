public class BenchmarkSprites
{
	public static void start()
	{
		if (!Config.game_loaded || !SelectedUnit.isSet())
		{
			return;
		}
		Actor tActor = SelectedUnit.unit;
		if (tActor.is_visible)
		{
			int tCountTotal = 100;
			Bench.bench("sprites_old", "sprites_test");
			for (int i = 0; i < tCountTotal; i++)
			{
				DynamicSpriteCreator.createNewSpriteUnit(tActor.frame_data, tActor.calculateMainSprite(), tActor.cached_sprite_head, tActor.kingdom.getColor(), tActor.asset, tActor.data.phenotype_index, tActor.data.phenotype_shade, UnitTextureAtlasID.Units);
			}
			Bench.benchEnd("sprites_old", "sprites_test", pSaveCounter: true, tCountTotal);
			Bench.bench("sprites_new", "sprites_test");
			for (int j = 0; j < tCountTotal; j++)
			{
				DynamicSpriteCreator.createNewSpriteUnit(tActor.frame_data, tActor.calculateMainSprite(), tActor.cached_sprite_head, tActor.kingdom.getColor(), tActor.asset, tActor.data.phenotype_index, tActor.data.phenotype_shade, UnitTextureAtlasID.Units);
			}
			Bench.benchEnd("sprites_new", "sprites_test", pSaveCounter: true, tCountTotal);
		}
	}
}
