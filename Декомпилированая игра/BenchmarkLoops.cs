using System;
using System.Collections.Generic;

public class BenchmarkLoops
{
	private List<WorldTile> _test_world_tiles = new List<WorldTile>();

	private ListPool<WorldTile> _test_world_tiles_pool;

	private HashSet<WorldTile> _test_hashset = new HashSet<WorldTile>();

	private WorldTile[] _test_world_tiles_arr;

	private List<WorldTile> _new_tiles = new List<WorldTile>();

	private int _runs;

	private bool _counter;

	private int _max_amount;

	private DebugToolAsset _asset;

	internal static Dictionary<string, BenchmarkLoops> _benchmarks = new Dictionary<string, BenchmarkLoops>();

	public BenchmarkLoops(DebugToolAsset pAsset, int pMaxAmount)
	{
		if (!_benchmarks.ContainsKey(pAsset.benchmark_group_id))
		{
			_benchmarks.Add(pAsset.benchmark_group_id, this);
			_max_amount = pMaxAmount;
			_asset = pAsset;
		}
	}

	public static void update(DebugToolAsset pAsset)
	{
		_benchmarks[pAsset.benchmark_group_id].run();
	}

	public void run()
	{
		string tGroupID = _asset.benchmark_group_id;
		string tTotalGroupID = _asset.benchmark_total_group;
		int tCountTotal = _test_world_tiles.Count;
		_counter = Randy.randomBool();
		if (_runs++ > 10 || _test_world_tiles_arr == null)
		{
			_runs = 0;
			_test_world_tiles_pool?.Dispose();
			_test_hashset.Clear();
			_test_world_tiles_arr?.Clear();
			_test_world_tiles.Clear();
			foreach (WorldTile new_tile in _new_tiles)
			{
				new_tile.Dispose();
			}
			_new_tiles.Clear();
			for (int i = 0; i < _max_amount; i++)
			{
				_test_world_tiles.Add(World.world.tiles_list.GetRandom());
			}
			_test_hashset.UnionWith(_test_world_tiles);
			_test_world_tiles_pool = new ListPool<WorldTile>(_test_world_tiles);
			_test_world_tiles_arr = _test_world_tiles.ToArray();
		}
		Bench.bench(tGroupID, tTotalGroupID);
		_test_world_tiles.Shuffle();
		_test_world_tiles.Shuffle();
		_test_world_tiles.Shuffle();
		_test_world_tiles.Shuffle();
		_test_world_tiles_arr.Shuffle();
		_test_world_tiles_arr.Shuffle();
		_test_world_tiles_arr.Shuffle();
		_test_world_tiles_arr.Shuffle();
		_test_world_tiles_pool.Shuffle();
		_test_world_tiles_pool.Shuffle();
		_test_world_tiles_pool.Shuffle();
		_test_world_tiles_pool.Shuffle();
		Bench.bench("list_for_field", tGroupID);
		int tResult = 0;
		tCountTotal = 0;
		for (int j = 0; j < _test_world_tiles.Count; j++)
		{
			WorldTile tTile = _test_world_tiles[j];
			tResult += tTile.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("list_for_field", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_pool.Shuffle();
		Bench.bench("lpool_for_field", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		for (int k = 0; k < _test_world_tiles_pool.Count; k++)
		{
			WorldTile tTile2 = _test_world_tiles_pool[k];
			tResult += tTile2.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("lpool_for_field", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_pool.Shuffle();
		Bench.bench("lpool_span_for", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		Span<WorldTile> tNewSpan = _test_world_tiles_pool.AsSpan();
		for (int l = 0; l < tNewSpan.Length; l++)
		{
			WorldTile tTile3 = tNewSpan[l];
			tResult += tTile3.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("lpool_span_for", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles.Shuffle();
		Bench.bench("list_for_local", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		List<WorldTile> tList = _test_world_tiles;
		for (int m = 0; m < tList.Count; m++)
		{
			WorldTile tTile4 = tList[m];
			tResult += tTile4.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("list_for_local", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_pool.Shuffle();
		Bench.bench("lpool_for_local", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		ListPool<WorldTile> tListPool = _test_world_tiles_pool;
		for (int n = 0; n < tListPool.Count; n++)
		{
			WorldTile tTile5 = tListPool[n];
			tResult += tTile5.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("lpool_for_local", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_pool.Shuffle();
		Bench.bench("lpool_span_for_local", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		Span<WorldTile> tSpan = _test_world_tiles_pool.AsSpan();
		for (int num = 0; num < tSpan.Length; num++)
		{
			WorldTile tTile6 = tSpan[num];
			tResult += tTile6.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("lpool_span_for_local", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles.Shuffle();
		Bench.bench("list_for_local_len", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		tList = _test_world_tiles;
		int tLen = tList.Count;
		for (int num2 = 0; num2 < tLen; num2++)
		{
			WorldTile tTile7 = tList[num2];
			tResult += tTile7.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("list_for_local_len", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_pool.Shuffle();
		Bench.bench("lpool_for_local_len", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		tListPool = _test_world_tiles_pool;
		tLen = tListPool.Count;
		for (int num3 = 0; num3 < tLen; num3++)
		{
			WorldTile tTile8 = tListPool[num3];
			tResult += tTile8.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("lpool_for_local_len", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_pool.Shuffle();
		Bench.bench("lpool_span_for_local_len", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		tSpan = _test_world_tiles_pool.AsSpan();
		tLen = tSpan.Length;
		for (int num4 = 0; num4 < tLen; num4++)
		{
			WorldTile tTile9 = tSpan[num4];
			tResult += tTile9.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("lpool_span_for_local_len", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles.Shuffle();
		Bench.bench("list_foreach_field", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		foreach (WorldTile tTile10 in _test_world_tiles)
		{
			tResult += tTile10.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("list_foreach_field", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_pool.Shuffle();
		Bench.bench("lpool_foreach_field", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		foreach (ref WorldTile item in _test_world_tiles_pool)
		{
			WorldTile tTile11 = item;
			tResult += tTile11.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("lpool_foreach_field", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles.Shuffle();
		Bench.bench("list_foreach_local", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		tList = _test_world_tiles;
		foreach (WorldTile tTile12 in tList)
		{
			tResult += tTile12.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("list_foreach_local", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_pool.Shuffle();
		Bench.bench("lpool_foreach_local", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		tListPool = _test_world_tiles_pool;
		foreach (ref WorldTile item2 in tListPool)
		{
			WorldTile tTile13 = item2;
			tResult += tTile13.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("lpool_foreach_local", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_pool.Shuffle();
		Bench.bench("lpool_span_foreach", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		tSpan = _test_world_tiles_pool.AsSpan();
		Span<WorldTile> span = tSpan;
		for (int num5 = 0; num5 < span.Length; num5++)
		{
			WorldTile tTile14 = span[num5];
			tResult += tTile14.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("lpool_span_foreach", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles.Shuffle();
		Bench.bench("list_span_for", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		tSpan = _test_world_tiles.AsSpan();
		for (int num6 = 0; num6 < tSpan.Length; num6++)
		{
			WorldTile tTile15 = tSpan[num6];
			tResult += tTile15.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("list_span_for", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles.Shuffle();
		Bench.bench("list_span_for_new", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		Span<WorldTile> tLocalSpan = _test_world_tiles.AsSpan();
		for (int num7 = 0; num7 < tLocalSpan.Length; num7++)
		{
			WorldTile tTile16 = tLocalSpan[num7];
			tResult += tTile16.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("list_span_for_new", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles.Shuffle();
		Bench.bench("list_span_foreach", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		tSpan = _test_world_tiles.AsSpan();
		span = tSpan;
		for (int num5 = 0; num5 < span.Length; num5++)
		{
			WorldTile tTile17 = span[num5];
			tResult += tTile17.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("list_span_foreach", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles.Shuffle();
		Bench.bench("list_span_foreach_new", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		span = _test_world_tiles.AsSpan();
		for (int num5 = 0; num5 < span.Length; num5++)
		{
			WorldTile tTile18 = span[num5];
			tResult += tTile18.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("list_span_foreach_new", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles.Shuffle();
		Bench.bench("list_rspan_for", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		ReadOnlySpan<WorldTile> tReadOnlySpan = _test_world_tiles.AsReadOnlySpan();
		for (int num8 = 0; num8 < tReadOnlySpan.Length; num8++)
		{
			WorldTile tTile19 = tReadOnlySpan[num8];
			tResult += tTile19.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("list_rspan_for", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles.Shuffle();
		Bench.bench("list_rspan_for_new", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		ReadOnlySpan<WorldTile> tLocalReadOnlySpan = _test_world_tiles.AsReadOnlySpan();
		for (int num9 = 0; num9 < tLocalReadOnlySpan.Length; num9++)
		{
			WorldTile tTile20 = tLocalReadOnlySpan[num9];
			tResult += tTile20.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("list_rspan_for_new", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles.Shuffle();
		Bench.bench("list_rspan_foreach", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		tReadOnlySpan = _test_world_tiles.AsReadOnlySpan();
		ReadOnlySpan<WorldTile> readOnlySpan = tReadOnlySpan;
		for (int num5 = 0; num5 < readOnlySpan.Length; num5++)
		{
			WorldTile tTile21 = readOnlySpan[num5];
			tResult += tTile21.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("list_rspan_foreach", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles.Shuffle();
		Bench.bench("list_rspan_foreach_new", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		readOnlySpan = _test_world_tiles.AsReadOnlySpan();
		for (int num5 = 0; num5 < readOnlySpan.Length; num5++)
		{
			WorldTile tTile22 = readOnlySpan[num5];
			tResult += tTile22.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("list_rspan_foreach_new", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_arr.Shuffle();
		Bench.bench("arr_for_field", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		for (int num10 = 0; num10 < _test_world_tiles_arr.Length; num10++)
		{
			WorldTile tTile23 = _test_world_tiles_arr[num10];
			tResult += tTile23.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("arr_for_field", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_arr.Shuffle();
		Bench.bench("arr_for_local", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		WorldTile[] tArr = _test_world_tiles_arr;
		foreach (WorldTile tTile24 in tArr)
		{
			tResult += tTile24.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("arr_for_local", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_arr.Shuffle();
		Bench.bench("arr_for_local_len", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		tArr = _test_world_tiles_arr;
		tLen = tArr.Length;
		for (int num12 = 0; num12 < tLen; num12++)
		{
			WorldTile tTile25 = tArr[num12];
			tResult += tTile25.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("arr_for_local_len", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_arr.Shuffle();
		Bench.bench("arr_foreach_field", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		WorldTile[] test_world_tiles_arr = _test_world_tiles_arr;
		foreach (WorldTile tTile26 in test_world_tiles_arr)
		{
			tResult += tTile26.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("arr_foreach_field", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_arr.Shuffle();
		Bench.bench("arr_foreach_local", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		tArr = _test_world_tiles_arr;
		test_world_tiles_arr = tArr;
		foreach (WorldTile tTile27 in test_world_tiles_arr)
		{
			tResult += tTile27.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("arr_foreach_local", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_arr.Shuffle();
		Bench.bench("arr_rspan_foreach", tGroupID);
		tReadOnlySpan = new ReadOnlySpan<WorldTile>(_test_world_tiles_arr);
		tResult = 0;
		tCountTotal = 0;
		readOnlySpan = tReadOnlySpan;
		for (int num5 = 0; num5 < readOnlySpan.Length; num5++)
		{
			WorldTile tTile28 = readOnlySpan[num5];
			tResult += tTile28.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("arr_rspan_foreach", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_arr.Shuffle();
		Bench.bench("arr_rspan_for", tGroupID);
		tReadOnlySpan = new ReadOnlySpan<WorldTile>(_test_world_tiles_arr);
		tResult = 0;
		tCountTotal = 0;
		for (int num13 = 0; num13 < tReadOnlySpan.Length; num13++)
		{
			WorldTile tTile29 = tReadOnlySpan[num13];
			tResult += tTile29.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("arr_rspan_for", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_arr.Shuffle();
		Bench.bench("arr_span_foreach", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		tSpan = new Span<WorldTile>(_test_world_tiles_arr);
		span = tSpan;
		for (int num5 = 0; num5 < span.Length; num5++)
		{
			WorldTile tTile30 = span[num5];
			tResult += tTile30.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("arr_span_foreach", tGroupID, pSaveCounter: true, tResult);
		_test_world_tiles_arr.Shuffle();
		Bench.bench("arr_span_for", tGroupID);
		tResult = 0;
		tCountTotal = 0;
		tSpan = new Span<WorldTile>(_test_world_tiles_arr);
		for (int num14 = 0; num14 < tSpan.Length; num14++)
		{
			WorldTile tTile31 = tSpan[num14];
			tResult += tTile31.data.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("arr_span_for", tGroupID, pSaveCounter: true, tResult);
		Bench.benchEnd(tGroupID, tTotalGroupID, pSaveCounter: false, 0L);
	}
}
