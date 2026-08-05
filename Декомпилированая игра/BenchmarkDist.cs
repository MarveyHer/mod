using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class BenchmarkDist
{
	public long result;

	internal string benchmark_group_id;

	internal string benchmark_id;

	internal List<WorldTile> test_tiles;

	internal bool print_to_console;

	private static BenchmarkDist _instance;

	public BenchmarkDist()
	{
		if (_instance == null)
		{
			benchmark_group_id = "dist_test_total";
			benchmark_id = "dist_test";
			test_tiles = new List<WorldTile>();
			_instance = this;
			setup();
		}
	}

	public static void update()
	{
		_instance.run();
	}

	public void setup()
	{
		if (!Config.game_loaded)
		{
			MapBox.on_world_loaded = (Action)Delegate.Combine(MapBox.on_world_loaded, (Action)delegate
			{
				setup();
			});
		}
		else
		{
			test_tiles.AddRange(World.world.tiles_list);
			test_tiles.ShuffleHalf();
			test_tiles.RemoveRange(test_tiles.Count / 2, test_tiles.Count / 2);
		}
	}

	public void run()
	{
		string tBenchmarkGroupId = benchmark_group_id;
		string tBenchmarkId = benchmark_id;
		int tCountTotal = 0;
		double tResult = 0.0;
		int tBest = -1;
		int tBestDist = int.MaxValue;
		float tBestDistFloat = float.MaxValue;
		List<WorldTile> tTiles = test_tiles;
		tTiles.Shuffle();
		int2[] tTestVec2s = new int2[tTiles.Count];
		for (int i = 0; i < tTiles.Count; i++)
		{
			tTestVec2s[i] = new int2(tTiles[i].x, tTiles[i].y);
		}
		float2[] tTestVec2sFloat = new float2[tTiles.Count];
		for (int j = 0; j < tTiles.Count; j++)
		{
			tTestVec2sFloat[j] = new float2(tTiles[j].x, tTiles[j].y);
		}
		NativeArray<int2> tTestVec2sNative = new NativeArray<int2>(tTestVec2s, Allocator.TempJob);
		NativeArray<float2> tTestVec2sFloatNative = new NativeArray<float2>(tTestVec2sFloat, Allocator.TempJob);
		WorldTile tTestTile = tTiles[0];
		Vector2Int tTestVec2 = tTestTile.pos;
		Vector3 tTestVec3 = tTestTile.posV3;
		int2 tTestVec2Int2 = new int2(tTestTile.x, tTestTile.y);
		float2 tTestVec2Float2 = new float2(tTestTile.x, tTestTile.y);
		Bench.bench(tBenchmarkId, tBenchmarkGroupId);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("DistTile", tBenchmarkId);
		for (int k = 1; k < tTiles.Count; k++)
		{
			WorldTile tTile = tTiles[k];
			float tDist = Toolbox.DistTile(tTestTile, tTile);
			if (tDist < tBestDistFloat)
			{
				tBestDistFloat = tDist;
				tBest = k;
			}
			tResult += (double)tDist;
			tCountTotal++;
		}
		Bench.benchEnd("DistTile", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("DistVec2", tBenchmarkId);
		for (int l = 1; l < tTiles.Count; l++)
		{
			WorldTile tTile2 = tTiles[l];
			float tDist2 = Toolbox.DistVec2(tTestVec2, tTile2.pos);
			if (tDist2 < tBestDistFloat)
			{
				tBestDistFloat = tDist2;
				tBest = l;
			}
			tResult += (double)tDist2;
			tCountTotal++;
		}
		Bench.benchEnd("DistVec2", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("DistVec3", tBenchmarkId);
		for (int m = 1; m < tTiles.Count; m++)
		{
			WorldTile tTile3 = tTiles[m];
			float tDist3 = Toolbox.DistVec3(tTestVec3, tTile3.posV3);
			if (tDist3 < tBestDistFloat)
			{
				tBestDistFloat = tDist3;
				tBest = m;
			}
			tResult += (double)tDist3;
			tCountTotal++;
		}
		Bench.benchEnd("DistVec3", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("Dist", tBenchmarkId);
		for (int n = 1; n < tTiles.Count; n++)
		{
			WorldTile tTile4 = tTiles[n];
			float tDist4 = Toolbox.Dist(tTestTile.x, tTestTile.y, tTile4.x, tTile4.y);
			if (tDist4 < tBestDistFloat)
			{
				tBestDistFloat = tDist4;
				tBest = n;
			}
			tResult += (double)tDist4;
			tCountTotal++;
		}
		Bench.benchEnd("Dist", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("DistFloat", tBenchmarkId);
		for (int num = 1; num < tTiles.Count; num++)
		{
			WorldTile tTile5 = tTiles[num];
			float tDist5 = DistFloat(tTestTile.x, tTestTile.y, tTile5.x, tTile5.y);
			if (tDist5 < tBestDistFloat)
			{
				tBestDistFloat = tDist5;
				tBest = num;
			}
			tResult += (double)tDist5;
			tCountTotal++;
		}
		Bench.benchEnd("DistFloat", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("Dist.pos", tBenchmarkId);
		for (int num2 = 1; num2 < tTiles.Count; num2++)
		{
			Vector2Int tTile6 = tTiles[num2].pos;
			float tDist6 = Toolbox.Dist(tTestVec2.x, tTestVec2.y, tTile6.x, tTile6.y);
			if (tDist6 < tBestDistFloat)
			{
				tBestDistFloat = tDist6;
				tBest = num2;
			}
			tResult += (double)tDist6;
			tCountTotal++;
		}
		Bench.benchEnd("Dist.pos", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("FastDistTile", tBenchmarkId);
		for (int num3 = 1; num3 < tTiles.Count; num3++)
		{
			WorldTile tTile7 = tTiles[num3];
			int tDist7 = Toolbox.SquaredDistTile(tTestTile, tTile7);
			if (tDist7 < tBestDist)
			{
				tBestDist = tDist7;
				tBest = num3;
			}
			tResult += (double)tDist7;
			tCountTotal++;
		}
		Bench.benchEnd("FastDistTile", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("FastDist", tBenchmarkId);
		for (int num4 = 1; num4 < tTiles.Count; num4++)
		{
			WorldTile tTile8 = tTiles[num4];
			int tDist8 = Toolbox.SquaredDist(tTestTile.x, tTestTile.y, tTile8.x, tTile8.y);
			if (tDist8 < tBestDist)
			{
				tBestDist = tDist8;
				tBest = num4;
			}
			tResult += (double)tDist8;
			tCountTotal++;
		}
		Bench.benchEnd("FastDist", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("FastDistFloat", tBenchmarkId);
		for (int num5 = 1; num5 < tTiles.Count; num5++)
		{
			WorldTile tTile9 = tTiles[num5];
			float tDist9 = FastDistFloat(tTestTile.x, tTestTile.y, tTile9.x, tTile9.y);
			if (tDist9 < tBestDistFloat)
			{
				tBestDistFloat = tDist9;
				tBest = num5;
			}
			tResult += (double)tDist9;
			tCountTotal++;
		}
		Bench.benchEnd("FastDistFloat", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("FastDistVec2", tBenchmarkId);
		for (int num6 = 1; num6 < tTiles.Count; num6++)
		{
			WorldTile tTile10 = tTiles[num6];
			int tDist10 = Toolbox.SquaredDistVec2(tTestVec2, tTile10.pos);
			if (tDist10 < tBestDist)
			{
				tBestDist = tDist10;
				tBest = num6;
			}
			tResult += (double)tDist10;
			tCountTotal++;
		}
		Bench.benchEnd("FastDistVec2", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("FastDistVec3", tBenchmarkId);
		for (int num7 = 1; num7 < tTiles.Count; num7++)
		{
			WorldTile tTile11 = tTiles[num7];
			float tDist11 = Toolbox.SquaredDistVec3(tTestVec3, tTile11.posV3);
			if (tDist11 < tBestDistFloat)
			{
				tBestDistFloat = tDist11;
				tBest = num7;
			}
			tResult += (double)tDist11;
			tCountTotal++;
		}
		Bench.benchEnd("FastDistVec3", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("FastDist.pos", tBenchmarkId);
		for (int num8 = 1; num8 < tTiles.Count; num8++)
		{
			Vector2Int tTile12 = tTiles[num8].pos;
			float tDist12 = Toolbox.SquaredDist(tTestVec2.x, tTestVec2.y, tTile12.x, tTile12.y);
			if (tDist12 < tBestDistFloat)
			{
				tBestDistFloat = tDist12;
				tBest = num8;
			}
			tResult += (double)tDist12;
			tCountTotal++;
		}
		Bench.benchEnd("FastDist.pos", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("distancesq", tBenchmarkId);
		for (int num9 = 1; num9 < tTiles.Count; num9++)
		{
			WorldTile tTile13 = tTiles[num9];
			float tDist13 = math.distancesq(tTestTile.x, tTile13.x) + math.distancesq(tTestTile.y, tTile13.y);
			if (tDist13 < tBestDistFloat)
			{
				tBestDistFloat = tDist13;
				tBest = num9;
			}
			tResult += (double)tDist13;
			tCountTotal++;
		}
		Bench.benchEnd("distancesq", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("float2", tBenchmarkId);
		for (int num10 = 1; num10 < tTiles.Count; num10++)
		{
			WorldTile tTile14 = tTiles[num10];
			float2 tTile15 = new float2(tTile14.x, tTile14.y);
			float tDist14 = math.distancesq(tTestVec2Float2, tTile15);
			if (tDist14 < tBestDistFloat)
			{
				tBestDistFloat = tDist14;
				tBest = num10;
			}
			tResult += (double)tDist14;
			tCountTotal++;
		}
		Bench.benchEnd("float2", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("int2", tBenchmarkId);
		for (int num11 = 1; num11 < tTiles.Count; num11++)
		{
			WorldTile tTile16 = tTiles[num11];
			float tDist15 = math.distancesq(y: new int2(tTile16.x, tTile16.y), x: tTestVec2Int2);
			if (tDist15 < tBestDistFloat)
			{
				tBestDistFloat = tDist15;
				tBest = num11;
			}
			tResult += (double)tDist15;
			tCountTotal++;
		}
		Bench.benchEnd("int2", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("int2array", tBenchmarkId);
		for (int num12 = 1; num12 < tTestVec2s.Length; num12++)
		{
			float tDist16 = math.distancesq(tTestVec2Int2, tTestVec2s[num12]);
			if (tDist16 < tBestDistFloat)
			{
				tBestDistFloat = tDist16;
				tBest = num12;
			}
			tResult += (double)tDist16;
			tCountTotal++;
		}
		Bench.benchEnd("int2array", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("nint2array", tBenchmarkId);
		for (int num13 = 1; num13 < tTestVec2sFloatNative.Length; num13++)
		{
			float tDist17 = math.distancesq(tTestVec2Int2, tTestVec2sFloatNative[num13]);
			if (tDist17 < tBestDistFloat)
			{
				tBestDistFloat = tDist17;
				tBest = num13;
			}
			tResult += (double)tDist17;
			tCountTotal++;
		}
		Bench.benchEnd("nint2array", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("float2array", tBenchmarkId);
		for (int num14 = 1; num14 < tTestVec2sFloat.Length; num14++)
		{
			float tDist18 = math.distancesq(tTestVec2Float2, tTestVec2sFloat[num14]);
			if (tDist18 < tBestDistFloat)
			{
				tBestDistFloat = tDist18;
				tBest = num14;
			}
			tResult += (double)tDist18;
			tCountTotal++;
		}
		Bench.benchEnd("float2array", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tBest = -1;
		tBestDist = int.MaxValue;
		tBestDistFloat = float.MaxValue;
		tResult = 0.0;
		tCountTotal = 0;
		Bench.bench("nfloat2array", tBenchmarkId);
		for (int num15 = 1; num15 < tTestVec2sNative.Length; num15++)
		{
			float tDist19 = math.distancesq(tTestVec2Float2, tTestVec2sNative[num15]);
			if (tDist19 < tBestDistFloat)
			{
				tBestDistFloat = tDist19;
				tBest = num15;
			}
			tResult += (double)tDist19;
			tCountTotal++;
		}
		Bench.benchEnd("nfloat2array", tBenchmarkId, pSaveCounter: true, tTiles[tBest].tile_id);
		tTestVec2sNative.Dispose();
		tTestVec2sFloatNative.Dispose();
		Bench.benchEnd(tBenchmarkId, tBenchmarkGroupId, pSaveCounter: false, 0L);
		if (print_to_console)
		{
			Debug.Log("LAST:\n" + Bench.printableBenchResults(tBenchmarkId, false, "DistTile", "DistVec2", "DistVec3", "Dist", "DistFloat", "Dist.pos", "FastDistTile", "FastDistVec2", "FastDistVec3", "FastDist", "FastDistFloat", "FastDist.pos", "int2", "int2array", "nint2array", "float2", "float2array", "nfloat2array", "distancesq", "job_new", "job_prefill", "pjob_prefill", "BurstDist", "BurstDistFloat", "BurstFastDistFloat", "BurstDist.pos", "BurstFastDist", "BurstFastDist.pos"));
			Debug.Log("AVG:\n" + Bench.printableBenchResults(tBenchmarkId, true, "DistTile", "DistVec2", "DistVec3", "Dist", "DistFloat", "Dist.pos", "FastDistTile", "FastDistVec2", "FastDistVec3", "FastDist", "FastDistFloat", "FastDist.pos", "int2", "int2array", "nint2array", "float2", "float2array", "nfloat2array", "distancesq", "job_new", "job_prefill", "pjob_prefill", "BurstDist", "BurstDistFloat", "BurstFastDistFloat", "BurstDist.pos", "BurstFastDist", "BurstFastDist.pos"));
		}
		result = (long)tResult;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float DistFloat(float x1, float y1, float x2, float y2)
	{
		return Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float FastDistFloat(float x1, float y1, float x2, float y2)
	{
		return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
	}
}
