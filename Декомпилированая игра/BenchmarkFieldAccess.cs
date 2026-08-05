public class BenchmarkFieldAccess
{
	public static void start()
	{
		if (Config.game_loaded)
		{
			int tCountTotal = 100000;
			Bench.bench("field_acess_test", "field_acess_total");
			Bench.bench("field_access", "field_acess_test");
			int tResult = 0;
			for (int i = 0; i < tCountTotal; i++)
			{
				tResult += World.world.tiles_list.Length;
			}
			Bench.benchEnd("field_access", "field_acess_test", pSaveCounter: true, tCountTotal);
			Bench.bench("temp_var", "field_acess_test");
			tResult = 0;
			MapBox tMapBox = World.world;
			for (int j = 0; j < tCountTotal; j++)
			{
				tResult += tMapBox.tiles_list.Length;
			}
			Bench.benchEnd("temp_var", "field_acess_test", pSaveCounter: true, tCountTotal);
			Bench.bench("temp_var_2", "field_acess_test");
			tResult = 0;
			WorldTile[] tList = World.world.tiles_list;
			for (int k = 0; k < tCountTotal; k++)
			{
				int tLen = tList.Length;
				tResult += tLen;
			}
			Bench.benchEnd("temp_var_2", "field_acess_test", pSaveCounter: true, tCountTotal);
			Bench.bench("result_len", "field_acess_test");
			tResult = 0;
			int tResultLen = World.world.tiles_list.Length;
			for (int l = 0; l < tCountTotal; l++)
			{
				tResult += tResultLen;
			}
			Bench.benchEnd("result_len", "field_acess_test", pSaveCounter: true, tCountTotal);
			Bench.benchEnd("field_acess_test", "field_acess_total", pSaveCounter: false, 0L);
		}
	}
}
