using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityPools;

public static class NameGeneratorTests
{
	private static string _test_string;

	public static void runTests()
	{
	}

	public static string testAllNamesForUniqueness()
	{
		string tRes = "";
		foreach (NameGeneratorAsset nameGenAsset in AssetManager.name_generator.list)
		{
			HashSet<string> tUniqueList = new HashSet<string>();
			for (int i = 0; i < 1000; i++)
			{
				string tInput = NameGenerator.generateNameFromTemplate(nameGenAsset);
				if (!tUniqueList.Contains(tInput))
				{
					tUniqueList.Add(tInput);
				}
			}
			tRes = tRes + "Unique names for asset " + nameGenAsset.id + ": " + tUniqueList.Count + "\n";
		}
		return writeResults("name_test3_uniq", tRes);
	}

	public static string testAllNamesOutput()
	{
		string tRes = "";
		foreach (NameGeneratorAsset nameGenAsset in AssetManager.name_generator.list)
		{
			tRes = tRes + "\n--- asset name: " + nameGenAsset.id + " ---\n";
			tRes = tRes + NameGenerator.generateNamesFromTemplate(20, nameGenAsset, null, null, pForceLegacy: false, pTestReplacer: true) + "\n";
		}
		return writeResults("name_test3", tRes);
	}

	public static string testNamesAlliances()
	{
		testNameStart();
		testName("alliance_name");
		return testNameEnd();
	}

	public static string testNamesWars()
	{
		testNameStart();
		testName("war_conquest");
		testName("war_rebellion");
		testName("war_spite");
		testName("war_inspire");
		testName("war_whisper");
		return testNameEnd();
	}

	public static string testNamesItems()
	{
		testNameStart();
		testName("boots_name");
		testName("armor_name");
		testName("helmet_name");
		testName("ring_name");
		testName("amulet_name");
		return testNameEnd();
	}

	public static string testNamesWeapons()
	{
		testNameStart();
		testName("sword_name");
		testName("axe_name");
		testName("hammer_name");
		testName("stick_name");
		testName("blaster_name");
		testName("spear_name");
		testName("bow_name");
		testName("flame_sword_name");
		testName("necromancer_staff_name");
		testName("evil_staff_name");
		testName("white_staff_name");
		testName("plague_doctor_staff_name");
		testName("druid_staff_name");
		return testNameEnd();
	}

	public static void testNameStart()
	{
		_test_string = "";
	}

	public static string testNameEnd()
	{
		return writeResults("name_test2", _test_string);
	}

	public static void testName(string pID, int pAmount = 20)
	{
		_test_string = _test_string + "\n--- " + pID + ":\n";
		NameGeneratorAsset tNameAsset = AssetManager.name_generator.get(pID);
		_test_string = _test_string + NameGenerator.generateNamesFromTemplate(100, tNameAsset, null, null, pForceLegacy: false, pTestReplacer: true) + "\n";
	}

	public static string testNamesBooks()
	{
		testNameStart();
		using ListPool<string> tBookNames = new ListPool<string> { "book_name_fable", "book_name_biology", "book_name_math", "book_name_diplomacy_manual", "book_name_love_story", "book_name_bad_story", "book_name_warfare_manual", "book_name_economy_manual", "book_name_stewardship_manual", "book_name_history" };
		tBookNames.Shuffle();
		foreach (ref string item in tBookNames)
		{
			testName(item);
		}
		return testNameEnd();
	}

	public static string testNamesDefault()
	{
		string tRes = "";
		tRes += "\n--- default - legacy:\n";
		for (int i = 0; i < 100; i++)
		{
			tRes = tRes + NameGenerator.getName("orc_unit", ActorSex.Male, pForceLegacy: true) + "\n";
		}
		tRes += "\n--- default_name - onomastics:\n";
		for (int j = 0; j < 100; j++)
		{
			tRes = tRes + NameGenerator.getName("orc_unit") + "\n";
		}
		return writeResults("name_test_default", tRes);
	}

	public static string testNamesClans()
	{
		string tRes = "";
		tRes += "\n--- human_clan name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("human_clan")) + "\n";
		tRes += "\n--- elf_clan name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("elf_clan")) + "\n";
		tRes += "\n--- dwarf_clan name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("dwarf_clan")) + "\n";
		tRes += "\n--- orc_clan name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("orc_clan")) + "\n";
		return writeResults("name_test2", tRes);
	}

	public static string testNamesKingdoms()
	{
		string tRes = "";
		tRes += "\n--- human_kingdom name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("human_kingdom")) + "\n";
		tRes += "\n--- elf_kingdom name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("elf_kingdom")) + "\n";
		tRes += "\n--- dwarf_kingdom name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("dwarf_kingdom")) + "\n";
		tRes += "\n--- orc_kingdom name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("orc_kingdom")) + "\n";
		return writeResults("name_test2", tRes);
	}

	public static string testNamesCities()
	{
		string tRes = "";
		tRes += "\n--- human_city name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("human_city")) + "\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("human_city"), null, null, pForceLegacy: true) + "\n";
		tRes += "\n--- elf_city name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("elf_city")) + "\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("elf_city"), null, null, pForceLegacy: true) + "\n";
		tRes += "\n--- dwarf_city name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("dwarf_city")) + "\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("dwarf_city"), null, null, pForceLegacy: true) + "\n";
		tRes += "\n--- orc_city name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("orc_city")) + "\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("orc_city"), null, null, pForceLegacy: true) + "\n";
		return writeResults("name_test2", tRes);
	}

	public static string testNamesCulture()
	{
		string tRes = "";
		tRes += "\n--- elf_culture name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("elf_culture")) + "\n";
		tRes += "\n--- dwarf_culture name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("dwarf_culture")) + "\n";
		tRes += "\n--- orc_culture name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("orc_culture")) + "\n";
		tRes += "\n--- human_culture name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("human_culture")) + "\n";
		return writeResults("name_test2", tRes);
	}

	public static string testMottos()
	{
		string tRes = "";
		tRes += "\n--- Mottos:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(100, AssetManager.name_generator.get("clan_mottos")) + "\n";
		return writeResults("name_test_mottos", tRes);
	}

	public static string testNames()
	{
		string tRes = "";
		tRes += "\n--- elf name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("elf_unit")) + "\n";
		tRes += "\n--- elf City:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("elf_city")) + "\n";
		tRes += "\n--- elf Kingdom:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("elf_kingdom")) + "\n";
		tRes += "\n--- dwarf name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("dwarf_unit")) + "\n";
		tRes += "\n--- dwarf City:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("dwarf_city")) + "\n";
		tRes += "\n--- dwarf Kingdom:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("dwarf_kingdom")) + "\n";
		tRes += "\n--- orc name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("orc_unit")) + "\n";
		tRes += "\n--- orc City:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("orc_city")) + "\n";
		tRes += "\n--- orc Kingdom:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("orc_kingdom")) + "\n";
		tRes += "\n--- Human name:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("human_unit")) + "\n";
		tRes += "\n--- Human City:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("human_city")) + "\n";
		tRes += "\n--- Human Kingdom:\n";
		tRes = tRes + NameGenerator.generateNamesFromTemplate(20, AssetManager.name_generator.get("human_kingdom")) + "\n";
		return writeResults("name_test2", tRes);
	}

	public static string testShowOnomasticsVsLegacy()
	{
		string tRes = "";
		string tOno = "[<color=green>ONO</color>]";
		string tLeg = "[<color=orange>LEG</color>]";
		string tEmp = "[<color=red>---</color>]";
		string tDic = "[<color=yellow>DIC</color>]";
		foreach (NameGeneratorAsset tAsset in AssetManager.name_generator.list)
		{
			if ((!string.IsNullOrEmpty("") && !tAsset.id.Contains("")) || "".Contains(tAsset.id))
			{
				continue;
			}
			string tItem1 = tEmp;
			string tItem2 = tEmp;
			string tItem3 = tEmp;
			string tCode1 = " ";
			string tCode2 = " ";
			if (tAsset.hasOnomastics())
			{
				tCode1 = "+";
				tItem2 = tOno;
			}
			if (tAsset.use_dictionary)
			{
				tItem1 = tDic;
			}
			List<string[]> templates = tAsset.templates;
			if (templates != null && templates.Count > 0)
			{
				tCode2 = "-";
				tItem3 = tLeg;
			}
			tRes = tRes + tCode1 + tCode2 + " " + tItem1 + " " + tItem2 + " " + tItem3 + " " + tAsset.id + "\n";
			if (tAsset.hasOnomastics())
			{
				List<string[]> templates2 = tAsset.templates;
				if (templates2 != null && templates2.Count > 0)
				{
					tRes += compareOnomasticsVsLegacy(tAsset.id, 15000);
				}
			}
		}
		return writeResults("name_test_ono", tRes);
	}

	public static string compareOnomasticsVsLegacy(string pNameAssetID, int pRuns)
	{
		string tRes = "";
		Randy.resetSeed(Randy.randomInt(1, 500));
		NameGeneratorAsset tAsset = AssetManager.name_generator.get(pNameAssetID);
		HashSet<string> tUniques = UnsafeCollectionPool<HashSet<string>, string>.Get();
		HashSet<string> tUniques2 = UnsafeCollectionPool<HashSet<string>, string>.Get();
		HashSet<string> tUniquesBoth = UnsafeCollectionPool<HashSet<string>, string>.Get();
		float tStart1 = Time.realtimeSinceStartup;
		for (int i = 0; i < pRuns; i++)
		{
			tUniques.Add(NameGenerator.generateNameFromTemplate(tAsset, null, null, pForceLegacy: true).ToLowerInvariant());
		}
		float tEnd1 = Time.realtimeSinceStartup;
		float tStart2 = Time.realtimeSinceStartup;
		for (int j = 0; j < pRuns; j++)
		{
			tUniques2.Add(NameGenerator.generateNameFromTemplate(tAsset).ToLowerInvariant());
		}
		float tEnd2 = Time.realtimeSinceStartup;
		int tUniqueCount = 0;
		int tUniqueCount2 = 0;
		int tUniqueCountBoth = 0;
		foreach (string tName in tUniques)
		{
			if (tUniques2.Contains(tName))
			{
				tUniqueCountBoth++;
				tUniquesBoth.Add(tName);
			}
			else
			{
				tUniqueCount++;
			}
		}
		foreach (string tName2 in tUniques2)
		{
			if (!tUniques.Contains(tName2))
			{
				tUniqueCount2++;
			}
		}
		Dictionary<int, int> tLengths = new Dictionary<int, int>();
		Dictionary<int, int> tLengths2 = new Dictionary<int, int>();
		int tMinLength = int.MaxValue;
		int tMaxLength = 0;
		foreach (string item in tUniques)
		{
			int tLen = item.Length;
			if (tLen < tMinLength)
			{
				tMinLength = tLen;
			}
			if (tLen > tMaxLength)
			{
				tMaxLength = tLen;
			}
		}
		int tMinLength2 = int.MaxValue;
		int tMaxLength2 = 0;
		foreach (string item2 in tUniques2)
		{
			int tLen2 = item2.Length;
			if (tLen2 < tMinLength2)
			{
				tMinLength2 = tLen2;
			}
			if (tLen2 > tMaxLength2)
			{
				tMaxLength2 = tLen2;
			}
		}
		int num = Mathf.Min(tMinLength, tMinLength2);
		int tMaxKeys = Mathf.Max(tMaxLength, tMaxLength2);
		for (int k = num; k <= tMaxKeys; k++)
		{
			tLengths[k] = 0;
			tLengths2[k] = 0;
		}
		foreach (string item3 in tUniques)
		{
			tLengths[item3.Length]++;
		}
		foreach (string item4 in tUniques2)
		{
			tLengths2[item4.Length]++;
		}
		float tLegacyOnlyPerc = 100f * (float)tUniqueCount / (float)tUniques.Count;
		float tOnoOnlyPerc = 100f * (float)tUniqueCount2 / (float)tUniques2.Count;
		float tBothPerc = 100f * (float)tUniqueCountBoth / (float)tUniques.Count;
		string tLegacyOnlyPercStr = ((tLegacyOnlyPerc < 25f) ? ("<color=green>" + tLegacyOnlyPerc.ToString("F2") + "%</color>") : ((tLegacyOnlyPerc < 70f) ? ("<color=orange>" + tLegacyOnlyPerc.ToString("F2") + "%</color>") : ("<color=red>" + tLegacyOnlyPerc.ToString("F2") + "%</color>")));
		string tOnoOnlyPercStr = ((tOnoOnlyPerc < 25f) ? ("<color=green>" + tOnoOnlyPerc.ToString("F2") + "%</color>") : ((tOnoOnlyPerc < 70f) ? ("<color=orange>" + tOnoOnlyPerc.ToString("F2") + "%</color>") : ("<color=red>" + tOnoOnlyPerc.ToString("F2") + "%</color>")));
		string tBothPercStr = ((tBothPerc < 25f) ? ("<color=red>" + tBothPerc.ToString("F2") + "%</color>") : ((tBothPerc < 70f) ? ("<color=orange>" + tBothPerc.ToString("F2") + "%</color>") : ("<color=green>" + tBothPerc.ToString("F2") + "%</color>")));
		using ListPool<string[]> tRows = new ListPool<string[]>();
		tRows.Add(new string[2]
		{
			"Unique " + pNameAssetID + " :",
			pRuns + " runs"
		});
		tRows.Add(new string[4]
		{
			"Legacy :",
			tUniques.Count.ToString() ?? "",
			100 * tUniques.Count / tUniques2.Count + "%",
			(tEnd1 - tStart1).ToString("F2") + "s"
		});
		tRows.Add(new string[4]
		{
			"Ono :",
			tUniques2.Count.ToString() ?? "",
			100 * tUniques2.Count / tUniques.Count + "%",
			(tEnd2 - tStart2).ToString("F2") + "s"
		});
		tRows.Add(new string[3]
		{
			"names only in legacy :",
			tUniqueCount.ToString() ?? "",
			tLegacyOnlyPercStr
		});
		tRows.Add(new string[3]
		{
			"names only in ono :",
			tUniqueCount2.ToString() ?? "",
			tOnoOnlyPercStr
		});
		tRows.Add(new string[3]
		{
			"names in both :",
			tUniqueCountBoth.ToString() ?? "",
			tBothPercStr
		});
		string tMinLengthStr = ((tMinLength < tMinLength2) ? ("<color=red>" + tMinLength + "</color>") : tMinLength.ToString());
		string tMinLength2Str = ((tMinLength2 < tMinLength) ? ("<color=red>" + tMinLength2 + "</color>") : tMinLength2.ToString());
		string tMaxLengthStr = ((tMaxLength > tMaxLength2) ? ("<color=red>" + tMaxLength + "</color>") : tMaxLength.ToString());
		string tMaxLength2Str = ((tMaxLength2 > tMaxLength) ? ("<color=red>" + tMaxLength2 + "</color>") : tMaxLength2.ToString());
		tRows.Add(new string[2]
		{
			"min/max len legacy :",
			tMinLengthStr + "-" + tMaxLengthStr
		});
		tRows.Add(new string[2]
		{
			"min/max len ono :",
			tMinLength2Str + "-" + tMaxLength2Str
		});
		tRes = tRes + "\n" + Toolbox.printRows(tRows);
		tRows.Clear();
		string[] tKeysLens = tLengths.Select((KeyValuePair<int, int> p) => p.Key.ToString()).ToArray();
		string[] tLegLens = tLengths.Select((KeyValuePair<int, int> p) => p.Value.ToString()).ToArray();
		string[] tOnoLens = tLengths2.Select((KeyValuePair<int, int> p) => p.Value.ToString()).ToArray();
		string[] tKeysComb = new string[1] { "len dist" }.Concat(tKeysLens).ToArray();
		string[] tLegComb = new string[1] { "legacy :" }.Concat(tLegLens).ToArray();
		string[] tOnoComb = new string[1] { "ono :" }.Concat(tOnoLens).ToArray();
		tRows.Add(tKeysComb);
		tRows.Add(tLegComb);
		tRows.Add(tOnoComb);
		tRes = tRes + "\n" + Toolbox.printRows(tRows);
		HashSet<string> tOnlyU1 = UnsafeCollectionPool<HashSet<string>, string>.Get();
		tOnlyU1.UnionWith(tUniques);
		tOnlyU1.ExceptWith(tUniques2);
		using ListPool<string> tU1 = new ListPool<string>(tOnlyU1);
		tU1.Sort();
		using ListPool<string> tPrint1 = new ListPool<string>(91);
		using ListPool<string> tPrint2 = new ListPool<string>(91);
		using ListPool<string> tPrintBoth = new ListPool<string>(91);
		if (tU1.Count > 0)
		{
			tPrint1.Add("Legacy");
			(string, string) tuple = findShortestLongest(tU1);
			string tShortest = tuple.Item1;
			string tLongest = tuple.Item2;
			for (int i2 = 0; i2 < Mathf.Min(tU1.Count, 30); i2++)
			{
				tPrint1.Add(tU1.Shift());
			}
			for (int i3 = 0; i3 < Mathf.Min(tU1.Count, 30); i3++)
			{
				tPrint1.Insert(Mathf.Min(31, tPrint1.Count), tU1.Pop());
			}
			int tMiddleIndex = Mathf.Max(tU1.Count / 2 - 15, 0);
			for (int i4 = 0; i4 < Mathf.Min(tU1.Count, 30); i4++)
			{
				tPrint1.Insert(Mathf.Min(30 + i4 + 1, tPrint1.Count), tU1[i4 + tMiddleIndex]);
			}
			tPrint1.Add(Toolbox.fillLeft("", tLongest.Length, '='));
			tPrint1.Add("Min/Max");
			tPrint1.Add(Toolbox.fillLeft("", tLongest.Length, '='));
			tPrint1.Add(tLongest);
			tPrint1.Add(tShortest);
		}
		HashSet<string> tOnlyU2 = UnsafeCollectionPool<HashSet<string>, string>.Get();
		tOnlyU2.UnionWith(tUniques2);
		tOnlyU2.ExceptWith(tUniques);
		using ListPool<string> tU2 = new ListPool<string>(tOnlyU2);
		tU2.Sort();
		if (tU2.Count > 0)
		{
			tPrint2.Add("Ono");
			(string, string) tuple2 = findShortestLongest(tU2);
			string tShortest2 = tuple2.Item1;
			string tLongest2 = tuple2.Item2;
			for (int i5 = 0; i5 < Mathf.Min(tU2.Count, 30); i5++)
			{
				tPrint2.Add(tU2.Shift());
			}
			for (int i6 = 0; i6 < Mathf.Min(tU2.Count, 30); i6++)
			{
				tPrint2.Insert(Mathf.Min(31, tPrint2.Count), tU2.Pop());
			}
			int tMiddleIndex2 = Mathf.Max(tU2.Count / 2 - 15, 0);
			for (int i7 = 0; i7 < Mathf.Min(tU2.Count, 30); i7++)
			{
				tPrint2.Insert(Mathf.Min(30 + i7 + 1, tPrint2.Count), tU2[i7 + tMiddleIndex2]);
			}
			tPrint2.Add(Toolbox.fillLeft("", tLongest2.Length, '='));
			tPrint2.Add("Min/Max");
			tPrint2.Add(Toolbox.fillLeft("", tLongest2.Length, '='));
			tPrint2.Add(tLongest2);
			tPrint2.Add(tShortest2);
		}
		using ListPool<string> tBoth = new ListPool<string>(tUniquesBoth);
		tBoth.Sort();
		if (tBoth.Count > 0)
		{
			tPrintBoth.Add("Both");
			(string, string) tuple3 = findShortestLongest(tBoth);
			string tShortest3 = tuple3.Item1;
			string tLongest3 = tuple3.Item2;
			for (int i8 = 0; i8 < Mathf.Min(tBoth.Count, 30); i8++)
			{
				tPrintBoth.Add(tBoth.Shift());
			}
			for (int i9 = 0; i9 < Mathf.Min(tBoth.Count, 30); i9++)
			{
				tPrintBoth.Insert(Mathf.Min(31, tBoth.Count), tBoth.Pop());
			}
			int tMiddleIndex3 = Mathf.Max(tBoth.Count / 2 - 15, 0);
			for (int i10 = 0; i10 < Mathf.Min(tBoth.Count, 30); i10++)
			{
				tPrintBoth.Insert(Mathf.Min(30 + i10 + 1, tBoth.Count), tBoth[i10 + tMiddleIndex3]);
			}
			tPrintBoth.Add(Toolbox.fillLeft("", tLongest3.Length, '='));
			tPrintBoth.Add("Min/Max");
			tPrintBoth.Add(Toolbox.fillLeft("", tLongest3.Length, '='));
			tPrintBoth.Add(tLongest3);
			tPrintBoth.Add(tShortest3);
		}
		tRes = tRes + "\n" + Toolbox.printColumns(tPrint1, tPrint2, tPrintBoth);
		UnsafeCollectionPool<HashSet<string>, string>.Release(tOnlyU1);
		UnsafeCollectionPool<HashSet<string>, string>.Release(tOnlyU2);
		UnsafeCollectionPool<HashSet<string>, string>.Release(tUniques);
		UnsafeCollectionPool<HashSet<string>, string>.Release(tUniques2);
		UnsafeCollectionPool<HashSet<string>, string>.Release(tUniquesBoth);
		return tRes;
	}

	private static (string, string) findShortestLongest(ListPool<string> pHashSet)
	{
		string tLongest = null;
		string tShortest = null;
		int tMaxLength = int.MinValue;
		int tMinLength = int.MaxValue;
		foreach (ref string item in pHashSet)
		{
			string tString = item;
			int tLength = tString.Length;
			if (tLength > tMaxLength)
			{
				tMaxLength = tLength;
				tLongest = tString;
			}
			if (tLength < tMinLength)
			{
				tMinLength = tLength;
				tShortest = tString;
			}
		}
		return (tShortest, tLongest);
	}

	public static string writeResults(string pFilename, string pResults)
	{
		File.WriteAllText(Application.persistentDataPath + "/" + pFilename, pResults);
		Debug.Log("Written result to " + pFilename + " in " + Application.persistentDataPath);
		return pResults;
	}
}
