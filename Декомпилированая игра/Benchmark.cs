using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

public static class Benchmark
{
	public static void benchHashsetStart()
	{
		for (int i = 0; i < 1000; i++)
		{
			benchObjHashsetCreateVsAdd(5000);
		}
		UnityEngine.Debug.Log("- BenchTest - list:" + Bench.getBenchResult("BenchTest - list"));
		UnityEngine.Debug.Log("- BenchTest - hashset:" + Bench.getBenchResult("BenchTest - hashset"));
	}

	public static void benchObjHashsetCreateVsAdd(int pAmount = 3000)
	{
		Bench.bench_enabled = true;
		List<BenchObject> tOriginalList = new List<BenchObject>();
		for (int i = 0; i < pAmount; i++)
		{
			tOriginalList.Add(new BenchObject());
		}
		int tTries = 10;
		List<BenchObject> tTest1List = new List<BenchObject>();
		tTest1List.AddRange(tOriginalList);
		Bench.bench("BenchTest - list");
		for (int j = 0; j < tTries; j++)
		{
			BenchObject tObject = tTest1List.GetRandom();
			tTest1List.Remove(tObject);
		}
		Bench.benchEnd("BenchTest - list", "main", pSaveCounter: false, 0L);
		HashSet<BenchObject> tSet = new HashSet<BenchObject>();
		tSet.UnionWith(tOriginalList);
		Bench.bench("BenchTest - hashset");
		for (int k = 0; k < tTries; k++)
		{
			BenchObject tObject2 = tOriginalList.GetRandom();
			tSet.Remove(tObject2);
		}
		tTest1List.Clear();
		tTest1List.AddRange(tSet);
		Bench.benchEnd("BenchTest - hashset", "main", pSaveCounter: false, 0L);
	}

	public static void benchObjectsVsData(int pObjects)
	{
		double tTries = 1000.0;
		Bench.bench_enabled = true;
		int tAmount = pObjects;
		UnityEngine.Debug.Log("----");
		UnityEngine.Debug.Log("NEW BENCH - " + pObjects);
		BenchObject[] tObjects = new BenchObject[tAmount];
		for (int i = 0; i < tObjects.Length; i++)
		{
			tObjects[i] = new BenchObject();
		}
		Stopwatch stopwatch_objects = new Stopwatch();
		stopwatch_objects.Start();
		for (int iR = 0; (double)iR < tTries; iR++)
		{
			for (int j = 0; j < tObjects.Length; j++)
			{
				tObjects[j].update(0f);
			}
		}
		stopwatch_objects.Stop();
		tObjects = new BenchObject[tAmount];
		for (int k = 0; k < tObjects.Length; k++)
		{
			tObjects[k] = new BenchObject();
		}
		Stopwatch stopwatch_data = new Stopwatch();
		stopwatch_data.Start();
		for (int l = 0; (double)l < tTries; l++)
		{
			for (int m = 0; m < tObjects.Length; m++)
			{
				tObjects[m].updateMove(0f);
				tObjects[m].updateMove(0f);
				tObjects[m].updateMove(0f);
				tObjects[m].updateMove(0f);
				tObjects[m].updateMove(0f);
			}
		}
		stopwatch_data.Stop();
		tObjects = new BenchObject[tAmount];
		for (int n = 0; n < tObjects.Length; n++)
		{
			tObjects[n] = new BenchObject();
		}
		Stopwatch stopwatch_data_optimized = new Stopwatch();
		stopwatch_data_optimized.Start();
		for (int num = 0; (double)num < tTries; num++)
		{
			foreach (BenchObject obj in tObjects)
			{
				obj.updateMove(0f);
				obj.updateMove(0f);
				obj.updateMove(0f);
				obj.updateMove(0f);
				obj.updateMove(0f);
			}
		}
		stopwatch_data_optimized.Stop();
		tObjects = new BenchObject[tAmount];
		for (int num3 = 0; num3 < tObjects.Length; num3++)
		{
			tObjects[num3] = new BenchObject();
		}
		Stopwatch stopwatch_parallel = new Stopwatch();
		stopwatch_parallel.Start();
		for (int num4 = 0; (double)num4 < tTries; num4++)
		{
			Parallel.ForEach(tObjects, World.world.parallel_options, delegate(BenchObject pObject)
			{
				pObject.updateMove(0f);
				pObject.updateMove(0f);
				pObject.updateMove(0f);
				pObject.updateMove(0f);
				pObject.updateMove(0f);
			});
		}
		stopwatch_parallel.Stop();
		tObjects = new BenchObject[tAmount];
		for (int i2 = 0; i2 < tObjects.Length; i2++)
		{
			tObjects[i2] = new BenchObject();
		}
		Stopwatch stopwatch_data_index = new Stopwatch();
		stopwatch_data_index.Start();
		for (int iR2 = 0; (double)iR2 < tTries; iR2++)
		{
			for (int i3 = 0; i3 < tObjects.Length; i3++)
			{
				tObjects[i3].derp += 22;
				if (tObjects[i3].derp == 1000)
				{
					tObjects[i3].derp += 10;
					if (tObjects[i3].derp < 10)
					{
						tObjects[i3].derp += 5;
					}
					else
					{
						tObjects[i3].derp -= 5;
					}
				}
			}
			for (int i4 = 0; i4 < tObjects.Length; i4++)
			{
				tObjects[i4].derp += 22;
				if (tObjects[i4].derp == 1000)
				{
					tObjects[i4].derp += 10;
					if (tObjects[i4].derp < 10)
					{
						tObjects[i4].derp += 5;
					}
					else
					{
						tObjects[i4].derp -= 5;
					}
				}
			}
			for (int i5 = 0; i5 < tObjects.Length; i5++)
			{
				tObjects[i5].derp += 22;
				if (tObjects[i5].derp == 1000)
				{
					tObjects[i5].derp += 10;
					if (tObjects[i5].derp < 10)
					{
						tObjects[i5].derp += 5;
					}
					else
					{
						tObjects[i5].derp -= 5;
					}
				}
			}
			for (int i6 = 0; i6 < tObjects.Length; i6++)
			{
				tObjects[i6].derp += 22;
				if (tObjects[i6].derp == 1000)
				{
					tObjects[i6].derp += 10;
					if (tObjects[i6].derp < 10)
					{
						tObjects[i6].derp += 5;
					}
					else
					{
						tObjects[i6].derp -= 5;
					}
				}
			}
			for (int i7 = 0; i7 < tObjects.Length; i7++)
			{
				tObjects[i7].derp += 22;
				if (tObjects[i7].derp == 1000)
				{
					tObjects[i7].derp += 10;
					if (tObjects[i7].derp < 10)
					{
						tObjects[i7].derp += 5;
					}
					else
					{
						tObjects[i7].derp -= 5;
					}
				}
			}
		}
		stopwatch_data_index.Stop();
		tObjects = new BenchObject[tAmount];
		for (int i8 = 0; i8 < tObjects.Length; i8++)
		{
			tObjects[i8] = new BenchObject();
		}
		Stopwatch stopwatch_data_temp = new Stopwatch();
		stopwatch_data_temp.Start();
		for (int iR3 = 0; (double)iR3 < tTries; iR3++)
		{
			foreach (BenchObject tObject in tObjects)
			{
				tObject.derp += 22;
				if (tObject.derp == 1000)
				{
					tObject.derp += 10;
					if (tObject.derp < 10)
					{
						tObject.derp += 5;
					}
					else
					{
						tObject.derp -= 5;
					}
				}
			}
			foreach (BenchObject tObject2 in tObjects)
			{
				tObject2.derp += 22;
				if (tObject2.derp == 1000)
				{
					tObject2.derp += 10;
					if (tObject2.derp < 10)
					{
						tObject2.derp += 5;
					}
					else
					{
						tObject2.derp -= 5;
					}
				}
			}
			foreach (BenchObject tObject3 in tObjects)
			{
				tObject3.derp += 22;
				if (tObject3.derp == 1000)
				{
					tObject3.derp += 10;
					if (tObject3.derp < 10)
					{
						tObject3.derp += 5;
					}
					else
					{
						tObject3.derp -= 5;
					}
				}
			}
			foreach (BenchObject tObject4 in tObjects)
			{
				tObject4.derp += 22;
				if (tObject4.derp == 1000)
				{
					tObject4.derp += 10;
					if (tObject4.derp < 10)
					{
						tObject4.derp += 5;
					}
					else
					{
						tObject4.derp -= 5;
					}
				}
			}
			foreach (BenchObject tObject5 in tObjects)
			{
				tObject5.derp += 22;
				if (tObject5.derp == 1000)
				{
					tObject5.derp += 10;
					if (tObject5.derp < 10)
					{
						tObject5.derp += 5;
					}
					else
					{
						tObject5.derp -= 5;
					}
				}
			}
		}
		stopwatch_data_temp.Stop();
		UnityEngine.Debug.Log("bench_objects " + (double)stopwatch_objects.ElapsedTicks / tTries + " 100%");
		UnityEngine.Debug.Log("bench_data " + getResult(stopwatch_objects, stopwatch_data, tTries));
		UnityEngine.Debug.Log("bench_data_index " + getResult(stopwatch_objects, stopwatch_data_index, tTries));
		UnityEngine.Debug.Log("bench_data_temp " + getResult(stopwatch_objects, stopwatch_data_temp, tTries));
		UnityEngine.Debug.Log("stopwatch_parallel " + getResult(stopwatch_objects, stopwatch_parallel, tTries));
		UnityEngine.Debug.Log("stopwatch_data_optimized " + getResult(stopwatch_objects, stopwatch_data_optimized, tTries));
	}

	private static string getResult(Stopwatch p1, Stopwatch p2, double pTries)
	{
		double num = (double)p1.ElapsedTicks / pTries;
		double tAv2 = (double)p2.ElapsedTicks / pTries;
		double tResult = num / tAv2 * 100.0 - 100.0;
		return tAv2 + ", " + tResult + "%";
	}

	public static void benchNativeECSAndOOP()
	{
		Bench.bench_enabled = true;
		int tAmount = 200000;
		NativeArray<Vector3> tNativVec = new NativeArray<Vector3>(tAmount, Allocator.TempJob);
		NativeArray<int> tNativX = new NativeArray<int>(tAmount, Allocator.TempJob);
		NativeArray<int> tNativY = new NativeArray<int>(tAmount, Allocator.TempJob);
		NativeArray<int> tNativHealth = new NativeArray<int>(tAmount, Allocator.TempJob);
		ActorData[] tNormaArray = new ActorData[tAmount];
		for (int i = 0; i < tAmount; i++)
		{
			tNormaArray[i] = new ActorData();
		}
		Bench.bench("test_native_vectors");
		for (int j = 0; j < tAmount; j++)
		{
			Vector3 tVec = tNativVec[j];
			tVec.x = j;
			tVec.y = j;
			tNativVec[j] = tVec;
		}
		for (int k = 0; k < tAmount; k++)
		{
			tNativHealth[k] = k;
		}
		Bench.benchEnd("test_native_vectors", "main", pSaveCounter: false, 0L);
		Bench.bench("test_native_x_y");
		for (int l = 0; l < tAmount; l++)
		{
			tNativX[l] = l;
			tNativY[l] = l;
		}
		for (int m = 0; m < tAmount; m++)
		{
			tNativHealth[m] = m;
		}
		Bench.benchEnd("test_native_x_y", "main", pSaveCounter: false, 0L);
		Bench.bench("test_normal_temp_var");
		for (int n = 0; n < tAmount; n++)
		{
			ActorData obj = tNormaArray[n];
			obj.x = n;
			obj.y = n;
		}
		for (int num = 0; num < tAmount; num++)
		{
			tNormaArray[num].health = num;
		}
		Bench.benchEnd("test_normal_temp_var", "main", pSaveCounter: false, 0L);
		Bench.bench("test_normal_direct");
		for (int num2 = 0; num2 < tAmount; num2++)
		{
			tNormaArray[num2].x = num2;
			tNormaArray[num2].y = num2;
		}
		for (int num3 = 0; num3 < tAmount; num3++)
		{
			tNormaArray[num3].health = num3;
		}
		Bench.benchEnd("test_normal_direct", "main", pSaveCounter: false, 0L);
		UnityEngine.Debug.Log("-  - - - - - - ");
		UnityEngine.Debug.Log("- BenchTest - test_native_vectors: " + Bench.getBenchResult("test_native_vectors", "main", pAverage: false));
		UnityEngine.Debug.Log("- BenchTest - test_native_x_y: " + Bench.getBenchResult("test_native_x_y", "main", pAverage: false));
		UnityEngine.Debug.Log("- BenchTest - test_normal_temp_var: " + Bench.getBenchResult("test_normal_temp_var", "main", pAverage: false));
		UnityEngine.Debug.Log("- BenchTest - test_normal_direct: " + Bench.getBenchResult("test_normal_direct", "main", pAverage: false));
		UnityEngine.Debug.Log("- BenchTest - test_job_native_vectors: " + Bench.getBenchResult("test_job_native_vectors", "main", pAverage: false));
		UnityEngine.Debug.Log("- BenchTest - test_job_native_xy: " + Bench.getBenchResult("test_job_native_xy", "main", pAverage: false));
		tNativVec.Dispose();
		tNativX.Dispose();
		tNativY.Dispose();
		tNativHealth.Dispose();
	}

	public static void benchReferenceVsDict()
	{
	}

	public static void testVirtual()
	{
		int tTries = 1000;
		BenchTest1 tTest1 = new BenchTest1();
		BenchTest2 tTest2 = new BenchTest2();
		Bench.bench("BenchTest - normal");
		for (int i = 0; i < tTries; i++)
		{
			tTest1.test();
		}
		Bench.benchEnd("BenchTest - normal", "main", pSaveCounter: false, 0L);
		Bench.bench("BenchTest - virtual");
		for (int j = 0; j < tTries; j++)
		{
			tTest2.testVirtual();
		}
		Bench.benchEnd("BenchTest - virtual", "main", pSaveCounter: false, 0L);
		UnityEngine.Debug.Log("Benchmark:");
		UnityEngine.Debug.Log("- BenchTest - normal:" + Bench.getBenchResult("BenchTest - normal"));
		UnityEngine.Debug.Log("- BenchTest - virtual:" + Bench.getBenchResult("BenchTest - virtual"));
	}

	public static void testQueue()
	{
		int tElements = 10000;
		List<TileType> tList = new List<TileType>();
		Queue<TileType> tQueue = new Queue<TileType>();
		LinkedList<TileType> tLinked = new LinkedList<TileType>();
		for (int i = 0; i < tElements; i++)
		{
			tList.Add(new TileType());
			tQueue.Enqueue(new TileType());
			tLinked.AddLast(new TileType());
		}
		Bench.bench("list");
		for (int j = 0; j < tList.Count; j++)
		{
			_ = tList[0];
			tList.RemoveAt(0);
		}
		Bench.benchEnd("list", "main", pSaveCounter: false, 0L);
		Bench.bench("queue");
		for (int k = 0; k < tQueue.Count; k++)
		{
			tQueue.Dequeue();
		}
		Bench.benchEnd("queue", "main", pSaveCounter: false, 0L);
		Bench.bench("linked");
		for (int l = 0; l < tLinked.Count; l++)
		{
			_ = tLinked.First;
			tLinked.RemoveFirst();
		}
		Bench.benchEnd("linked", "main", pSaveCounter: false, 0L);
		UnityEngine.Debug.Log("!!!BENCH REMOVE AT 0 " + tElements);
		Bench.printBenchResult("list");
		Bench.printBenchResult("queue");
		Bench.printBenchResult("linked");
	}

	public static void testRemoveStructs()
	{
		int tTries = 100;
		int tObjects = 500;
		List<Vector3> tListObjects = new List<Vector3>();
		List<Vector3> tListToRemove = new List<Vector3>();
		List<Vector3> tList1 = new List<Vector3>();
		List<Vector3> tList2 = new List<Vector3>();
		HashSet<Vector3> tHash = new HashSet<Vector3>();
		for (int i = 0; i < tObjects; i++)
		{
			tListObjects.Add(new Vector3
			{
				x = Randy.randomInt(0, 1000),
				y = Randy.randomInt(0, 1000),
				z = Randy.randomInt(0, 1000)
			});
		}
		tListObjects.Shuffle();
		for (int j = 0; j < tTries; j++)
		{
			tListToRemove.Add(tListObjects.GetRandom());
		}
		Bench.bench("remove");
		foreach (Vector3 tVec in tListObjects)
		{
			tList1.Add(tVec);
		}
		for (int k = 0; k < tTries; k++)
		{
			tList1.Remove(tListToRemove[k]);
		}
		Bench.benchEnd("remove", "main", pSaveCounter: false, 0L);
		Bench.bench("RemoveAtSwapBack");
		foreach (Vector3 tVec2 in tListObjects)
		{
			tList2.Add(tVec2);
		}
		for (int l = 0; l < tTries; l++)
		{
			tList2.RemoveAtSwapBack(tListToRemove[l]);
		}
		Bench.benchEnd("RemoveAtSwapBack", "main", pSaveCounter: false, 0L);
		Bench.benchEnd("remove_native", "main", pSaveCounter: false, 0L);
		Bench.bench("remove_hashset");
		foreach (Vector3 tVec3 in tListObjects)
		{
			tHash.Add(tVec3);
		}
		for (int m = 0; m < tTries; m++)
		{
			tHash.Remove(tListToRemove[m]);
		}
		Bench.benchEnd("remove_hashset", "main", pSaveCounter: false, 0L);
		UnityEngine.Debug.Log("Benchmark:");
		UnityEngine.Debug.Log("- built-in remove:" + Bench.getBenchResult("remove"));
		UnityEngine.Debug.Log("- own RemoveAtSwapBack: " + Bench.getBenchResult("RemoveAtSwapBack"));
		UnityEngine.Debug.Log("- native RemoveAtSwapBack: " + Bench.getBenchResult("remove_native"));
		UnityEngine.Debug.Log("- remove hashset: " + Bench.getBenchResult("remove_hashset"));
	}

	public static void testCapacity()
	{
		int tTicks = 100;
		int tValues = 100000;
		Bench.bench("new_list");
		List<List<int>> tAllLists = new List<List<int>>(tTicks);
		for (int i = 0; i < tTicks; i++)
		{
			List<int> tList = new List<int>();
			tAllLists.Add(tList);
			for (int jj = 0; jj < tValues; jj++)
			{
				tList.Add(jj);
			}
		}
		Bench.benchEnd("new_list", "main", pSaveCounter: false, 0L);
		Bench.bench("new_list_reused");
		for (int j = 0; j < tAllLists.Count; j++)
		{
			List<int> tList2 = tAllLists[j];
			tList2.Clear();
			for (int k = 0; k < tValues; k++)
			{
				tList2.Add(k);
			}
		}
		Bench.benchEnd("new_list_reused", "main", pSaveCounter: false, 0L);
		Bench.bench("new_list_set_capacity");
		tAllLists = new List<List<int>>(tTicks);
		for (int l = 0; l < tTicks; l++)
		{
			List<int> tList3 = new List<int>(tValues);
			tAllLists.Add(tList3);
			for (int m = 0; m < tValues; m++)
			{
				tList3.Add(m);
			}
		}
		Bench.benchEnd("new_list_set_capacity", "main", pSaveCounter: false, 0L);
		Bench.bench("new_list_set_capacity_reused");
		for (int n = 0; n < tAllLists.Count; n++)
		{
			List<int> tList4 = tAllLists[n];
			tList4.Clear();
			for (int num = 0; num < tValues; num++)
			{
				tList4.Add(num);
			}
		}
		Bench.benchEnd("new_list_set_capacity_reused", "main", pSaveCounter: false, 0L);
		Bench.printBenchResult("new_list");
		Bench.printBenchResult("new_list_set_capacity");
		Bench.printBenchResult("new_list_reused");
		Bench.printBenchResult("new_list_set_capacity_reused");
	}
}
