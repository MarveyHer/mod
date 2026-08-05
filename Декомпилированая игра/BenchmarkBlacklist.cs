using System.Collections.Generic;
using UnityEngine;

public static class BenchmarkBlacklist
{
	private static List<WorldTile> _test_world_tiles = new List<WorldTile>();

	private static HashSet<WorldTile> _test_hashset = new HashSet<WorldTile>();

	private static WorldTile[] _test_world_tiles_arr;

	private static List<string> _names = new List<string>();

	private static HashSet<string> _names_set = new HashSet<string>();

	private static int _runs = 0;

	private static int _max = 250;

	private static HashSet<string> _good_words = new HashSet<string>();

	private static HashSet<string> _bad_words = new HashSet<string>();

	private static HashSet<string> _result_good_words = new HashSet<string>();

	private static HashSet<string> _result_bad_words = new HashSet<string>();

	public static void start()
	{
		if (_runs-- <= 0)
		{
			_names.Clear();
			_names_set.Clear();
			_max = Randy.randomInt(50, 255);
			_runs = 5;
		}
		if (_names.Count == 0)
		{
			_good_words.Clear();
			_bad_words.Clear();
			_names_set.Clear();
			Blacklist.init();
			BlacklistTest.init();
			BlacklistTest2.init();
			BlacklistTest3.init();
			BlacklistTest4.init();
			BlacklistTest5.init();
			BlacklistTest6.init();
			BlacklistTest7.init();
			BlacklistTest8.init();
			BlacklistTest9.init();
			BlacklistTest10.init();
			BlacklistTest11.init();
			BlacklistTest12.init();
			BlacklistTest13.init();
			AssetManager.name_generator.list.Shuffle();
			foreach (NameGeneratorAsset nameGenAsset in AssetManager.name_generator.list)
			{
				if (_names_set.Count > _max)
				{
					break;
				}
				for (int i = 0; i < 150; i++)
				{
					string tInput = NameGenerator.generateNameFromTemplate(nameGenAsset);
					if (string.IsNullOrEmpty(tInput))
					{
						Debug.LogError("name generator returned null or empty string " + nameGenAsset.id);
						continue;
					}
					_names_set.Add(tInput);
					if (_names_set.Count > _max)
					{
						break;
					}
				}
			}
			_names.AddRange(_names_set);
			bool tOK1 = false;
			bool tOK2 = false;
			for (int j = 0; j < _names.Count; j++)
			{
				if (!Blacklist.checkBlackList(_names[j]))
				{
					tOK1 = true;
					_good_words.Add(_names[j]);
				}
				else
				{
					tOK2 = true;
					_bad_words.Add(_names[j]);
				}
			}
			if (!tOK1 || !tOK2)
			{
				_runs = 0;
				start();
			}
			Debug.Log("Unique names for test " + _names.Count + " / " + _max + " => " + _good_words.Count + " / " + _bad_words.Count);
		}
		_result_good_words.Clear();
		_result_bad_words.Clear();
		Bench.bench("blacklist_test", "blacklist_test_total");
		Bench.bench("current_blacklist_good", "blacklist_test");
		int tGood = 0;
		for (int k = 0; k < _names.Count; k++)
		{
			if (!Blacklist.checkBlackList(_names[k]))
			{
				tGood++;
				_result_good_words.Add(_names[k]);
			}
		}
		Bench.benchEnd("current_blacklist_good", "blacklist_test", pSaveCounter: true, tGood);
		Bench.bench("current_blacklist_bad", "blacklist_test");
		int tBad = 0;
		for (int l = 0; l < _names.Count; l++)
		{
			if (Blacklist.checkBlackList(_names[l]))
			{
				tBad++;
				_result_bad_words.Add(_names[l]);
			}
		}
		Bench.benchEnd("current_blacklist_bad", "blacklist_test", pSaveCounter: true, tBad);
		checkResult("current_blacklist_bad");
		Bench.bench("three_blacklist_good_9", "blacklist_test");
		int tGood2 = 0;
		for (int m = 0; m < _names.Count; m++)
		{
			if (!BlacklistTest9.checkBlackList(_names[m]))
			{
				tGood2++;
				_result_good_words.Add(_names[m]);
			}
		}
		Bench.benchEnd("three_blacklist_good_9", "blacklist_test", pSaveCounter: true, tGood2);
		Bench.bench("three_blacklist_bad_9", "blacklist_test");
		int tBad2 = 0;
		for (int n = 0; n < _names.Count; n++)
		{
			if (BlacklistTest9.checkBlackList(_names[n]))
			{
				tBad2++;
				_result_bad_words.Add(_names[n]);
			}
		}
		Bench.benchEnd("three_blacklist_bad_9", "blacklist_test", pSaveCounter: true, tBad2);
		checkResult("three_blacklist_bad_9");
		Bench.bench("old_blacklist_good_10", "blacklist_test");
		int tGood3 = 0;
		for (int num = 0; num < _names.Count; num++)
		{
			if (!BlacklistTest10.checkBlackList(_names[num]))
			{
				tGood3++;
				_result_good_words.Add(_names[num]);
			}
		}
		Bench.benchEnd("old_blacklist_good_10", "blacklist_test", pSaveCounter: true, tGood3);
		Bench.bench("old_blacklist_bad_10", "blacklist_test");
		int tBad3 = 0;
		for (int num2 = 0; num2 < _names.Count; num2++)
		{
			if (BlacklistTest10.checkBlackList(_names[num2]))
			{
				tBad3++;
				_result_bad_words.Add(_names[num2]);
			}
		}
		Bench.benchEnd("old_blacklist_bad_10", "blacklist_test", pSaveCounter: true, tBad3);
		checkResult("old_blacklist_bad_10");
		Bench.bench("slice_blacklist_good_11", "blacklist_test");
		int tGood4 = 0;
		for (int num3 = 0; num3 < _names.Count; num3++)
		{
			if (!BlacklistTest11.checkBlackList(_names[num3]))
			{
				tGood4++;
				_result_good_words.Add(_names[num3]);
			}
		}
		Bench.benchEnd("slice_blacklist_good_11", "blacklist_test", pSaveCounter: true, tGood4);
		Bench.bench("slice_blacklist_bad_11", "blacklist_test");
		int tBad4 = 0;
		for (int num4 = 0; num4 < _names.Count; num4++)
		{
			if (BlacklistTest11.checkBlackList(_names[num4]))
			{
				tBad4++;
				_result_bad_words.Add(_names[num4]);
			}
		}
		Bench.benchEnd("slice_blacklist_bad_11", "blacklist_test", pSaveCounter: true, tBad4);
		checkResult("slice_blacklist_bad_11");
		Bench.bench("ref_blacklist_good_12", "blacklist_test");
		int tGood5 = 0;
		for (int num5 = 0; num5 < _names.Count; num5++)
		{
			if (!BlacklistTest12.checkBlackList(_names[num5]))
			{
				tGood5++;
				_result_good_words.Add(_names[num5]);
			}
		}
		Bench.benchEnd("ref_blacklist_good_12", "blacklist_test", pSaveCounter: true, tGood5);
		Bench.bench("ref_blacklist_bad_12", "blacklist_test");
		int tBad5 = 0;
		for (int num6 = 0; num6 < _names.Count; num6++)
		{
			if (BlacklistTest12.checkBlackList(_names[num6]))
			{
				tBad5++;
				_result_bad_words.Add(_names[num6]);
			}
		}
		Bench.benchEnd("ref_blacklist_bad_12", "blacklist_test", pSaveCounter: true, tBad5);
		checkResult("ref_blacklist_bad_12");
		Bench.bench("idx_blacklist_good_13", "blacklist_test");
		int tGood6 = 0;
		for (int num7 = 0; num7 < _names.Count; num7++)
		{
			if (!BlacklistTest13.checkBlackList(_names[num7]))
			{
				tGood6++;
				_result_good_words.Add(_names[num7]);
			}
		}
		Bench.benchEnd("idx_blacklist_good_13", "blacklist_test", pSaveCounter: true, tGood6);
		Bench.bench("idx_blacklist_bad_13", "blacklist_test");
		int tBad6 = 0;
		for (int num8 = 0; num8 < _names.Count; num8++)
		{
			if (BlacklistTest13.checkBlackList(_names[num8]))
			{
				tBad6++;
				_result_bad_words.Add(_names[num8]);
			}
		}
		Bench.benchEnd("idx_blacklist_bad_13", "blacklist_test", pSaveCounter: true, tBad6);
		checkResult("idx_blacklist_bad_13");
		Bench.benchEnd("blacklist_test", "blacklist_test_total", pSaveCounter: false, 0L);
	}

	public static void checkResult(string pBenchmarkName)
	{
		if (_result_good_words.Count != _good_words.Count || _result_bad_words.Count != _bad_words.Count)
		{
			Debug.LogError(pBenchmarkName + ": Blacklist check failed " + _result_good_words.Count + " " + _good_words.Count + " " + _result_bad_words.Count + " " + _bad_words.Count);
			foreach (string tWord in _result_good_words)
			{
				if (!_good_words.Contains(tWord))
				{
					Debug.LogError(pBenchmarkName + ": Missing good word: " + tWord);
				}
			}
			foreach (string tWord2 in _result_bad_words)
			{
				if (!_bad_words.Contains(tWord2))
				{
					Debug.LogError(pBenchmarkName + ": Missing bad word: " + tWord2);
				}
			}
			foreach (string tWord3 in _good_words)
			{
				if (!_result_good_words.Contains(tWord3))
				{
					Debug.LogError(pBenchmarkName + ": Extra good word: " + tWord3);
				}
			}
			foreach (string tWord4 in _bad_words)
			{
				if (!_result_bad_words.Contains(tWord4))
				{
					Debug.LogError(pBenchmarkName + ": Extra bad word: " + tWord4);
				}
			}
		}
		_result_good_words.Clear();
		_result_bad_words.Clear();
	}
}
