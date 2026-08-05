using System;
using System.Collections.Generic;

public static class BenchmarkStructLoops
{
	private static List<WorldTileDataStruct> _test_world_tiles = new List<WorldTileDataStruct>();

	private static ListPool<WorldTileDataStruct> _test_world_tiles_pool;

	private static HashSet<WorldTileDataStruct> _test_hashset = new HashSet<WorldTileDataStruct>();

	private static WorldTileDataStruct[] _test_world_tiles_arr;

	private static int _runs = 0;

	public static void start()
	{
		int tCountTotal = _test_world_tiles.Count;
		if (_runs++ > 30 || _test_world_tiles_arr == null)
		{
			_runs = 0;
			_test_world_tiles_pool?.Dispose();
			_test_world_tiles.Clear();
			_test_hashset.Clear();
			int tRepeats = Randy.randomInt(1, 5);
			int tCount = World.world.tiles_list.Length;
			for (int i = 0; i < tRepeats; i++)
			{
				for (int j = 0; j < tCount; j++)
				{
					WorldTile tTile = World.world.tiles_list[j];
					int tTileID = tTile.data.tile_id + i * tCount;
					_test_world_tiles.Add(new WorldTileDataStruct(tTile, tTileID));
				}
				_test_world_tiles.Shuffle();
			}
			_test_hashset.UnionWith(_test_world_tiles);
			_test_world_tiles_pool = new ListPool<WorldTileDataStruct>(_test_world_tiles);
			_test_world_tiles_arr = _test_world_tiles.ToArray();
		}
		Bench.bench("loops_struct_test", "loops_struct_test_total");
		Bench.bench("list_for", "loops_struct_test");
		int tResult = 0;
		tCountTotal = 0;
		for (int k = 0; k < _test_world_tiles.Count; k++)
		{
			tResult += _test_world_tiles[k].tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("list_for", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("listpool_for", "loops_struct_test");
		tResult = 0;
		tCountTotal = 0;
		for (int l = 0; l < _test_world_tiles_pool.Count; l++)
		{
			tResult += _test_world_tiles_pool[l].tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("listpool_for", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("listpool_span_for", "loops_struct_test");
		tResult = 0;
		tCountTotal = 0;
		Span<WorldTileDataStruct> tNewSpan = _test_world_tiles_pool.AsSpan();
		for (int m = 0; m < tNewSpan.Length; m++)
		{
			WorldTileDataStruct tTile2 = tNewSpan[m];
			tResult += tTile2.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("listpool_span_for", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("hashset_foreach", "loops_struct_test");
		tResult = 0;
		tCountTotal = 0;
		foreach (WorldTileDataStruct item in _test_hashset)
		{
			tResult += item.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("hashset_foreach", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("list_for_local", "loops_struct_test");
		tResult = 0;
		tCountTotal = 0;
		List<WorldTileDataStruct> tList = _test_world_tiles;
		for (int n = 0; n < tList.Count; n++)
		{
			tResult += tList[n].tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("list_for_local", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("listpool_for_local", "loops_struct_test");
		tResult = 0;
		tCountTotal = 0;
		ListPool<WorldTileDataStruct> tListPool = _test_world_tiles_pool;
		for (int num = 0; num < tListPool.Count; num++)
		{
			tResult += tListPool[num].tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("listpool_for_local", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("listpool_span_for_local", "loops_struct_test");
		tResult = 0;
		tCountTotal = 0;
		Span<WorldTileDataStruct> tSpan = _test_world_tiles_pool.AsSpan();
		for (int num2 = 0; num2 < tSpan.Length; num2++)
		{
			WorldTileDataStruct tTile3 = tSpan[num2];
			tResult += tTile3.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("listpool_span_for_local", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("list_for_local_len", "loops_struct_test");
		tResult = 0;
		tCountTotal = 0;
		tList = _test_world_tiles;
		int tLen = tList.Count;
		for (int num3 = 0; num3 < tLen; num3++)
		{
			tResult += tList[num3].tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("list_for_local_len", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("listpool_for_local_len", "loops_struct_test");
		tResult = 0;
		tCountTotal = 0;
		tListPool = _test_world_tiles_pool;
		tLen = tListPool.Count;
		for (int num4 = 0; num4 < tLen; num4++)
		{
			tResult += tListPool[num4].tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("listpool_for_local_len", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("listpool_span_for_local_len", "loops_struct_test");
		tResult = 0;
		tCountTotal = 0;
		tSpan = _test_world_tiles_pool.AsSpan();
		tLen = tSpan.Length;
		for (int num5 = 0; num5 < tLen; num5++)
		{
			WorldTileDataStruct tTile4 = tSpan[num5];
			tResult += tTile4.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("listpool_span_for_local_len", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("list_foreach", "loops_struct_test");
		tResult = 0;
		tCountTotal = 0;
		foreach (WorldTileDataStruct test_world_tile in _test_world_tiles)
		{
			tResult += test_world_tile.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("list_foreach", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("listpool_foreach", "loops_struct_test");
		tResult = 0;
		tCountTotal = 0;
		foreach (ref WorldTileDataStruct item2 in _test_world_tiles_pool)
		{
			WorldTileDataStruct tTile5 = item2;
			tResult += tTile5.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("listpool_foreach", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("listpool_span_foreach", "loops_struct_test");
		tResult = 0;
		tCountTotal = 0;
		tSpan = _test_world_tiles_pool.AsSpan();
		Span<WorldTileDataStruct> span = tSpan;
		for (int num6 = 0; num6 < span.Length; num6++)
		{
			WorldTileDataStruct tTile6 = span[num6];
			tResult += tTile6.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("listpool_span_foreach", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("array_for", "loops_struct_test");
		tResult = 0;
		tCountTotal = 0;
		for (int num7 = 0; num7 < _test_world_tiles_arr.Length; num7++)
		{
			WorldTileDataStruct tTile7 = _test_world_tiles_arr[num7];
			tResult += tTile7.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("array_for", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("array_for_local", "loops_struct_test");
		tResult = 0;
		tCountTotal = 0;
		WorldTileDataStruct[] tArr = _test_world_tiles_arr;
		for (int num8 = 0; num8 < tArr.Length; num8++)
		{
			WorldTileDataStruct tTile8 = tArr[num8];
			tResult += tTile8.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("array_for_local", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("array_for_local_len", "loops_struct_test");
		tResult = 0;
		tCountTotal = 0;
		tArr = _test_world_tiles_arr;
		tLen = _test_world_tiles_arr.Length;
		for (int num9 = 0; num9 < tLen; num9++)
		{
			WorldTileDataStruct tTile9 = tArr[num9];
			tResult += tTile9.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("array_for_local_len", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("array_foreach", "loops_struct_test");
		tResult = 0;
		tCountTotal = 0;
		WorldTileDataStruct[] test_world_tiles_arr = _test_world_tiles_arr;
		for (int num6 = 0; num6 < test_world_tiles_arr.Length; num6++)
		{
			WorldTileDataStruct tTile10 = test_world_tiles_arr[num6];
			tResult += tTile10.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("array_foreach", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("ro_span_foreach", "loops_struct_test");
		ReadOnlySpan<WorldTileDataStruct> tReadOnlySpan = new ReadOnlySpan<WorldTileDataStruct>(_test_world_tiles_arr);
		tResult = 0;
		tCountTotal = 0;
		ReadOnlySpan<WorldTileDataStruct> readOnlySpan = tReadOnlySpan;
		for (int num6 = 0; num6 < readOnlySpan.Length; num6++)
		{
			WorldTileDataStruct tTile11 = readOnlySpan[num6];
			tResult += tTile11.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("ro_span_foreach", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("ro_span_for", "loops_struct_test");
		tReadOnlySpan = new ReadOnlySpan<WorldTileDataStruct>(_test_world_tiles_arr);
		tResult = 0;
		tCountTotal = 0;
		for (int num10 = 0; num10 < tReadOnlySpan.Length; num10++)
		{
			WorldTileDataStruct tTile12 = tReadOnlySpan[num10];
			tResult += tTile12.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("ro_span_for", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("span_foreach", "loops_struct_test");
		tSpan = new Span<WorldTileDataStruct>(_test_world_tiles_arr);
		tResult = 0;
		tCountTotal = 0;
		span = tSpan;
		for (int num6 = 0; num6 < span.Length; num6++)
		{
			WorldTileDataStruct tTile13 = span[num6];
			tResult += tTile13.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("span_foreach", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.bench("span_for", "loops_struct_test");
		tSpan = new Span<WorldTileDataStruct>(_test_world_tiles_arr);
		tResult = 0;
		tCountTotal = 0;
		for (int num11 = 0; num11 < tSpan.Length; num11++)
		{
			WorldTileDataStruct tTile14 = tSpan[num11];
			tResult += tTile14.tile_id;
			tCountTotal++;
		}
		Bench.benchEnd("span_for", "loops_struct_test", pSaveCounter: true, tCountTotal);
		Bench.benchEnd("loops_struct_test", "loops_struct_test_total", pSaveCounter: false, 0L);
	}
}
