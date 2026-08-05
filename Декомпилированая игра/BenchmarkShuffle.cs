using System;
using System.Collections.Generic;
using UnityEngine;

public class BenchmarkShuffle
{
	public int result;

	internal int max_tiles;

	internal int amount;

	internal string benchmark_total_group_id;

	internal string benchmark_group_id;

	internal List<WorldTile> test_tiles;

	internal bool print_to_console;

	internal static Dictionary<string, BenchmarkShuffle> _benchmarks = new Dictionary<string, BenchmarkShuffle>();

	public BenchmarkShuffle(DebugToolAsset pAsset, int pAmount, int pMaxTiles)
	{
		if (!_benchmarks.ContainsKey(pAsset.benchmark_group_id))
		{
			amount = pAmount;
			max_tiles = pMaxTiles;
			benchmark_total_group_id = pAsset.benchmark_total_group;
			benchmark_group_id = pAsset.benchmark_group_id;
			test_tiles = new List<WorldTile>();
			_benchmarks.Add(pAsset.benchmark_group_id, this);
			setup();
		}
	}

	public static void update(DebugToolAsset pAsset)
	{
		_benchmarks[pAsset.benchmark_group_id].run();
	}

	public void setup()
	{
		if (!Config.game_loaded)
		{
			MapBox.on_world_loaded = (Action)Delegate.Combine(MapBox.on_world_loaded, (Action)delegate
			{
				setup();
			});
			return;
		}
		int num = max_tiles;
		test_tiles.Clear();
		int tGenerateTiles = Mathf.CeilToInt(Mathf.Sqrt(num));
		tGenerateTiles *= tGenerateTiles;
		using ListPool<WorldTile> tTiles = new ListPool<WorldTile>(World.world.tiles_list);
		tTiles.Shuffle();
		for (int i = 0; i < tGenerateTiles; i++)
		{
			test_tiles.Add(tTiles.Pop());
		}
		test_tiles.Shuffle();
	}

	public void run()
	{
		int tAmount = amount;
		string tBenchmarkGroupId = benchmark_total_group_id;
		string tBenchmarkId = benchmark_group_id;
		int tCountTotal = 0;
		int tResult = 0;
		List<WorldTile> tTiles = test_tiles;
		for (int i = tAmount - 1; i >= 0; i--)
		{
			WorldTile tTile = tTiles[i];
			tResult += tTile.data.tile_id;
			tCountTotal++;
		}
		Bench.bench(tBenchmarkId, tBenchmarkGroupId);
		test_tiles.Shuffle();
		tResult = 0;
		tCountTotal = 0;
		Bench.bench($"no_shuffle_for_{tAmount}", tBenchmarkId);
		for (int j = 0; j < tAmount; j++)
		{
			WorldTile tTile2 = tTiles[j];
			tResult += tTile2.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd($"no_shuffle_for_{tAmount}", tBenchmarkId, pSaveCounter: true, tCountTotal);
		test_tiles.Shuffle();
		tResult = 0;
		tCountTotal = 0;
		Bench.bench($"shuffle_all_{tAmount}", tBenchmarkId);
		tTiles.Shuffle();
		for (int k = 0; k < tAmount; k++)
		{
			WorldTile tTile3 = tTiles[k];
			tResult += tTile3.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd($"shuffle_all_{tAmount}", tBenchmarkId, pSaveCounter: true, tCountTotal);
		test_tiles.Shuffle();
		tResult = 0;
		tCountTotal = 0;
		Bench.bench($"shuffle_one_new_list_{tAmount}", tBenchmarkId);
		ListPool<WorldTile> tNewList = new ListPool<WorldTile>(tTiles);
		for (int l = 0; l < tAmount; l++)
		{
			tNewList.ShuffleOne(l);
			WorldTile tTile4 = tNewList[l];
			tResult += tTile4.data.tile_id;
			tCountTotal++;
		}
		tNewList.Dispose();
		Bench.benchEnd($"shuffle_one_new_list_{tAmount}", tBenchmarkId, pSaveCounter: true, tCountTotal);
		test_tiles.Shuffle();
		tResult = 0;
		tCountTotal = 0;
		Bench.bench($"shuffle_one_{tAmount}", tBenchmarkId);
		for (int m = 0; m < tAmount; m++)
		{
			tTiles.ShuffleOne(m);
			WorldTile tTile5 = tTiles[m];
			tResult += tTile5.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd($"shuffle_one_{tAmount}", tBenchmarkId, pSaveCounter: true, tCountTotal);
		test_tiles.Shuffle();
		tResult = 0;
		tCountTotal = 0;
		Bench.bench($"shuffle_for_{tAmount}", tBenchmarkId);
		int tRandomStart = Randy.randomInt(0, tAmount);
		int tLength = tAmount + tRandomStart;
		for (int n = tRandomStart; n < tLength; n++)
		{
			int j2 = n % tAmount;
			WorldTile tTile6 = tTiles[j2];
			tResult += tTile6.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd($"shuffle_for_{tAmount}", tBenchmarkId, pSaveCounter: true, tCountTotal);
		test_tiles.Shuffle();
		tResult = 0;
		tCountTotal = 0;
		Bench.bench($"shuffle_2for_{tAmount}", tBenchmarkId);
		tRandomStart = Randy.randomInt(0, tAmount);
		for (int num = tRandomStart; num < tAmount; num++)
		{
			WorldTile tTile7 = tTiles[num];
			tResult += tTile7.data.tile_id;
			tCountTotal++;
		}
		for (int num2 = 0; num2 < tRandomStart; num2++)
		{
			WorldTile tTile8 = tTiles[num2];
			tResult += tTile8.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd($"shuffle_2for_{tAmount}", tBenchmarkId, pSaveCounter: true, tCountTotal);
		test_tiles.Shuffle();
		tResult = 0;
		tCountTotal = 0;
		Bench.bench($"shuffle_iterator_{tAmount}", tBenchmarkId);
		foreach (WorldTile tTile9 in tTiles.LoopRandom())
		{
			tResult += tTile9.data.tile_id;
			tCountTotal++;
			if (tCountTotal == tAmount)
			{
				break;
			}
		}
		Bench.benchEnd($"shuffle_iterator_{tAmount}", tBenchmarkId, pSaveCounter: true, tCountTotal);
		test_tiles.Shuffle();
		tResult = 0;
		tCountTotal = 0;
		Bench.bench($"shuffle_iterator_limit_{tAmount}", tBenchmarkId);
		foreach (WorldTile tTile10 in tTiles.LoopRandom(tAmount))
		{
			tResult += tTile10.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd($"shuffle_iterator_limit_{tAmount}", tBenchmarkId, pSaveCounter: true, tCountTotal);
		test_tiles.Shuffle();
		tResult = 0;
		tCountTotal = 0;
		Bench.bench($"no_shuffle_iterator_{tAmount}", tBenchmarkId);
		foreach (WorldTile tTile11 in tTiles)
		{
			tResult += tTile11.data.tile_id;
			tCountTotal++;
			if (tCountTotal == tAmount)
			{
				break;
			}
		}
		Bench.benchEnd($"no_shuffle_iterator_{tAmount}", tBenchmarkId, pSaveCounter: true, tCountTotal);
		Bench.benchEnd(tBenchmarkId, tBenchmarkGroupId, pSaveCounter: false, 0L);
		if (print_to_console)
		{
			Debug.Log("LAST:\n" + Bench.printableBenchResults(tBenchmarkId, false, $"no_shuffle_for_{tAmount}", $"no_shuffle_iterator_{tAmount}", $"shuffle_iterator_{tAmount}", $"shuffle_iterator_limit_{tAmount}", $"shuffle_for_{tAmount}", $"shuffle_2for_{tAmount}", $"shuffle_one_{tAmount}", $"shuffle_one_new_list_{tAmount}", $"shuffle_all_{tAmount}"));
			Debug.Log("AVG:\n" + Bench.printableBenchResults(tBenchmarkId, true, $"no_shuffle_for_{tAmount}", $"no_shuffle_iterator_{tAmount}", $"shuffle_iterator_{tAmount}", $"shuffle_iterator_limit_{tAmount}", $"shuffle_for_{tAmount}", $"shuffle_2for_{tAmount}", $"shuffle_one_{tAmount}", $"shuffle_one_new_list_{tAmount}", $"shuffle_all_{tAmount}"));
		}
		result = tResult;
	}
}
