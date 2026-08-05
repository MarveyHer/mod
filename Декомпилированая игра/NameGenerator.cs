using System;
using System.Collections.Generic;
using UnityPools;

public class NameGenerator
{
	private static int _current_consonants = 0;

	private static int _current_vowels = 0;

	private static readonly char[] vowels_all = new char[6] { 'a', 'e', 'i', 'o', 'u', 'y' };

	private static readonly char[] consonants_all = new char[20]
	{
		'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm',
		'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z'
	};

	private static bool _initiated = false;

	[ThreadStatic]
	private static Dictionary<string, ListPool<string>> _dict_splitted_items;

	public static void init()
	{
		if (!_initiated)
		{
			_initiated = true;
			Blacklist.init();
		}
	}

	public static string generateName(Actor pActor, MetaType pType, long pSeed, ActorSex pSex = ActorSex.None)
	{
		string tNameTemplate = null;
		int tSeedMeta = pType.GetHashCode();
		pSeed += tSeedMeta;
		if (pActor.hasCulture())
		{
			OnomasticsData tOnomasticData = pActor.culture.getOnomasticData(pType);
			if (tOnomasticData != null)
			{
				return tOnomasticData.generateName(pSex, 0, pSeed);
			}
			tNameTemplate = pActor.culture.getNameTemplate(pType);
		}
		else
		{
			foreach (Actor tParent in pActor.getParents())
			{
				if (tParent.hasCulture())
				{
					OnomasticsData tOnomasticData2 = tParent.culture.getOnomasticData(pType);
					if (tOnomasticData2 != null)
					{
						return tOnomasticData2.generateName(pSex, 0, pSeed);
					}
					tNameTemplate = tParent.culture.getNameTemplate(pType);
					break;
				}
			}
		}
		if (string.IsNullOrEmpty(tNameTemplate))
		{
			tNameTemplate = pActor.asset.getNameTemplate(pType);
		}
		return getName(tNameTemplate, pSex, pForceLegacy: false, null, pSeed);
	}

	public static string getName(string pAssetID, ActorSex pSex = ActorSex.Male, bool pForceLegacy = false, string pTemplate = null, long? pSeed = null, bool pIgnoreBlackList = false)
	{
		init();
		NameGeneratorAsset tAsset = AssetManager.name_generator.get(pAssetID);
		_current_consonants = 0;
		_current_vowels = 0;
		string tName = generateNameFromTemplate(tAsset, null, null, pForceLegacy, 0, pTemplate, null, pTestReplacer: false, pSeed, pSex, pIgnoreBlackList);
		if (!tAsset.hasOnomastics() && pSex == ActorSex.Female)
		{
			string lastLetter = tName.Substring(tName.Length - 1, 1);
			bool tFound = false;
			string[] vowels = tAsset.vowels;
			for (int i = 0; i < vowels.Length; i++)
			{
				if (vowels[i].CompareTo(lastLetter) == 0)
				{
					tFound = true;
					break;
				}
			}
			if (!tFound)
			{
				tName += Randy.getRandom(tAsset.vowels);
			}
		}
		return tName;
	}

	private static string firstToUpper(string pString)
	{
		return pString.FirstToUpper();
	}

	private static string addVowel(string[] pList, bool pUppercase = false)
	{
		_current_consonants = 0;
		_current_vowels++;
		if (pUppercase)
		{
			return firstToUpper(Randy.getRandom(pList));
		}
		return Randy.getRandom(pList);
	}

	private static string addEnding(NameGeneratorAsset pTemplate, string pName)
	{
		string tEnding = Randy.getRandom(pTemplate.parts);
		if (isConsonant(tEnding[0]) && isConsonant(pName[pName.Length - 1]))
		{
			tEnding = addVowel(pTemplate.vowels) + tEnding;
		}
		else if (!isConsonant(tEnding[0]) && !isConsonant(pName[pName.Length - 1]))
		{
			tEnding = addConsonant(pTemplate.consonants) + tEnding;
		}
		return tEnding;
	}

	private static string addConsonant(string[] pList, bool pUppercase = false)
	{
		_current_consonants++;
		_current_vowels = 0;
		if (pUppercase)
		{
			return firstToUpper(Randy.getRandom(pList));
		}
		return Randy.getRandom(pList);
	}

	private static string addPart(string[] pArray, bool pUppercase = false)
	{
		string tPart = Randy.getRandom(pArray);
		if (isConsonant(tPart[tPart.Length - 1]))
		{
			_current_consonants++;
			_current_vowels = 0;
		}
		else
		{
			_current_consonants = 0;
			_current_vowels++;
		}
		if (pUppercase)
		{
			tPart = firstToUpper(tPart);
		}
		return tPart;
	}

	private static bool isConsonant(char pChar)
	{
		return consonants_all.IndexOf(pChar) > -1;
	}

	private static bool isVowel(char pChar)
	{
		return vowels_all.IndexOf(pChar) > -1;
	}

	public static string generateNameFromTemplate(string pAssetID, Actor pActor = null, Kingdom pKingdom = null, bool pForceLegacy = false)
	{
		return generateNameFromTemplate(AssetManager.name_generator.get(pAssetID), pActor, pKingdom, pForceLegacy);
	}

	public static string generateNameFromOnomastics(NameGeneratorAsset pAsset, string pTemplate = null, Actor pActor = null, long? pSeed = null, ActorSex pSex = ActorSex.None)
	{
		OnomasticsData tOriginalData = (string.IsNullOrEmpty(pTemplate) ? OnomasticsCache.getOriginalData(pAsset.onomastics_templates.GetRandom()) : OnomasticsCache.getOriginalData(pTemplate));
		ActorSex tSex = pSex;
		if (pActor != null)
		{
			tSex = pActor.data.sex;
		}
		return tOriginalData.generateName(tSex, 0, pSeed);
	}

	public static string generateNamesFromTemplate(int pAmount, NameGeneratorAsset pAsset, Actor pActor = null, Kingdom pKingdom = null, bool pForceLegacy = false, bool pTestReplacer = false)
	{
		string tRes = "";
		HashSet<string> tUniques = new HashSet<string>();
		List<string> tNames = new List<string>();
		if (pAsset.hasOnomastics() && !pForceLegacy)
		{
			foreach (string tTemplate in pAsset.onomastics_templates)
			{
				tUniques.Clear();
				tNames.Clear();
				for (int i = 0; i < 100; i++)
				{
					tUniques.Add(generateNameFromTemplate(pAsset, pActor, pKingdom, pForceLegacy: false, 0, tTemplate, null, pTestReplacer));
				}
				tRes = tRes + "\n--- " + tTemplate;
				tRes = tRes + "\n -- (" + tUniques.Count + " / " + 100 + ") \n";
				tNames.AddRange(tUniques);
				tNames.Shuffle();
				if (tNames.Count - pAmount > 0)
				{
					tNames.RemoveRange(pAmount, tNames.Count - pAmount);
				}
				tNames.Sort();
				foreach (string tName in tNames)
				{
					tRes = tRes + tName + "\n";
				}
			}
		}
		else
		{
			for (int j = 0; j < 100; j++)
			{
				tUniques.Add(generateNameFromTemplate(pAsset, pActor, pKingdom, pForceLegacy, 0, null, null, pTestReplacer));
			}
			tRes += "\n--- Legacy";
			tRes = tRes + "\n -- (" + tUniques.Count + " / " + 100 + ") \n";
			tNames.AddRange(tUniques);
			tNames.Shuffle();
			if (tNames.Count - pAmount > 0)
			{
				tNames.RemoveRange(pAmount, tNames.Count - pAmount);
			}
			tNames.Sort();
			foreach (string tName2 in tNames)
			{
				tRes = tRes + tName2 + "\n";
			}
		}
		return tRes;
	}

	public static string generateNameFromTemplate(NameGeneratorAsset pAsset, Actor pActor = null, Kingdom pKingdom = null, bool pForceLegacy = false, int pCalls = 0, string pOnomasticsTemplate = null, string[] pClassicTemplate = null, bool pTestReplacer = false, long? pSeed = null, ActorSex pSex = ActorSex.None, bool pIgnoreBlacklist = false)
	{
		if (pCalls > 50)
		{
			return string.Empty;
		}
		if (pAsset.hasOnomastics() && !pForceLegacy)
		{
			return generateNameFromOnomastics(pAsset, pOnomasticsTemplate, pActor, pSeed, pSex);
		}
		_current_consonants = 0;
		_current_vowels = 0;
		string tName = "";
		string[] obj = pClassicTemplate ?? pAsset.templates.GetRandom();
		bool tMakeUpper = false;
		bool tAdditionAdded = false;
		string[] array = obj;
		foreach (string tStep in array)
		{
			string tMain;
			string tLastPart;
			if (tStep.Contains('#'))
			{
				string[] array2 = tStep.Split('#');
				tMain = array2[0];
				tLastPart = array2[1];
			}
			else
			{
				tMain = tStep;
				tLastPart = "";
			}
			if (pAsset.use_dictionary)
			{
				if (tMain == "$comma$")
				{
					tName += ", ";
					continue;
				}
				if (tMain.Contains(';'))
				{
					tMain = tMain.Split(';').GetRandom();
				}
				Dictionary<string, ListPool<string>> dict_splitted_items = _dict_splitted_items;
				if (dict_splitted_items == null || !dict_splitted_items.ContainsKey(tMain))
				{
					if (_dict_splitted_items == null)
					{
						_dict_splitted_items = UnsafeCollectionPool<Dictionary<string, ListPool<string>>, KeyValuePair<string, ListPool<string>>>.Get();
					}
					ListPool<string> tNewList = new ListPool<string>(pAsset.dict_parts[tMain].Split(','));
					_dict_splitted_items.Add(tMain, tNewList);
				}
				_dict_splitted_items[tMain].ShuffleLast();
				string tMottoPartId = _dict_splitted_items[tMain].Last();
				if (_dict_splitted_items[tMain].Count > 1)
				{
					_dict_splitted_items[tMain].Pop();
				}
				tName += tMottoPartId;
				continue;
			}
			switch (tMain)
			{
			case "RANDOM_LETTER":
				tName = ((!Randy.randomBool()) ? (tName + addConsonant(pAsset.consonants, pUppercase: true)) : (tName + addVowel(pAsset.vowels, pUppercase: true)));
				break;
			case "space":
			case " ":
				tName += " ";
				break;
			case "letters":
			{
				string[] tNumbers3 = tLastPart.Split('-');
				tName += addWord(pAsset, int.Parse(tNumbers3[0]), int.Parse(tNumbers3[1]));
				break;
			}
			case "Letters":
			{
				string[] tNumbers2 = tLastPart.Split('-');
				tName += addWord(pAsset, int.Parse(tNumbers2[0]), int.Parse(tNumbers2[1]), pToUpperFirst: true);
				break;
			}
			case "part":
				tName += pAsset.parts.GetRandom();
				break;
			case "consonant":
				tName += addConsonant(pAsset.consonants);
				break;
			case "CONSONANT":
				tName += addConsonant(pAsset.consonants, pUppercase: true);
				break;
			case "vowel":
				tName += addVowel(pAsset.vowels);
				break;
			case "vowelchance":
				if (Randy.randomBool())
				{
					tName += addVowel(pAsset.vowels);
				}
				break;
			case "removalchance":
				if (Randy.randomBool())
				{
					tName.Remove(tName.Length - 1);
				}
				break;
			case "VOWEL":
				tName += addVowel(pAsset.vowels, pUppercase: true);
				break;
			case "special1":
				tName += pAsset.special1.GetRandom();
				break;
			case "special2":
				tName += pAsset.special2.GetRandom();
				break;
			case "Part":
			{
				string tPart = pAsset.parts.GetRandom();
				tPart = firstToUpper(tPart);
				tName += tPart;
				break;
			}
			case "number":
				tName += Randy.randomInt(0, 10);
				break;
			case "addition_start":
				if (!tAdditionAdded && Randy.randomChance(pAsset.add_addition_chance))
				{
					tName = tName + pAsset.addition_start.GetRandom() + " ";
					tAdditionAdded = true;
				}
				break;
			case "addition_ending":
				if (!tAdditionAdded && Randy.randomChance(pAsset.add_addition_chance))
				{
					tName = tName + " " + pAsset.addition_ending.GetRandom();
					tAdditionAdded = true;
				}
				break;
			case "part_group":
				foreach (string part_group in pAsset.part_groups)
				{
					string[] tGroupParts6 = part_group.Split(',');
					tName += tGroupParts6.GetRandom();
				}
				break;
			case "part_group2":
				foreach (string item in pAsset.part_groups2)
				{
					string[] tGroupParts5 = item.Split(',');
					tName += tGroupParts5.GetRandom();
				}
				break;
			case "part_group3":
				foreach (string item2 in pAsset.part_groups3)
				{
					string[] tGroupParts4 = item2.Split(',');
					tName += tGroupParts4.GetRandom();
				}
				break;
			case "Part_group":
				tMakeUpper = true;
				foreach (string part_group2 in pAsset.part_groups)
				{
					string[] tGroupParts3 = part_group2.Split(',');
					if (tMakeUpper)
					{
						tName += firstToUpper(tGroupParts3.GetRandom());
						tMakeUpper = false;
					}
					else
					{
						tName += tGroupParts3.GetRandom();
					}
				}
				break;
			case "Part_group2":
				tMakeUpper = true;
				foreach (string item3 in pAsset.part_groups2)
				{
					string[] tGroupParts2 = item3.Split(',');
					if (tMakeUpper)
					{
						tName += firstToUpper(tGroupParts2.GetRandom());
						tMakeUpper = false;
					}
					else
					{
						tName += tGroupParts2.GetRandom();
					}
				}
				break;
			case "Part_group3":
				tMakeUpper = true;
				foreach (string item4 in pAsset.part_groups3)
				{
					string[] tGroupParts = item4.Split(',');
					if (tMakeUpper)
					{
						tName += firstToUpper(tGroupParts.GetRandom());
						tMakeUpper = false;
					}
					else
					{
						tName += tGroupParts.GetRandom();
					}
				}
				break;
			}
		}
		if (tName.Contains('$'))
		{
			if (pTestReplacer)
			{
				NameGeneratorReplacers.replacer_debug(ref tName);
			}
			else
			{
				if (pAsset.replacer != null)
				{
					pAsset.replacer(ref tName, pActor);
				}
				if (pAsset.replacer_kingdom != null)
				{
					pAsset.replacer_kingdom(ref tName, pKingdom);
				}
			}
		}
		bool tRedo = false;
		if (string.IsNullOrEmpty(tName))
		{
			tRedo = true;
		}
		else if (!pAsset.use_dictionary && !pIgnoreBlacklist && Blacklist.checkBlackList(tName))
		{
			tRedo = true;
		}
		if (tRedo)
		{
			return generateNameFromTemplate(pAsset, pActor, pKingdom, pForceLegacy, ++pCalls, pOnomasticsTemplate, pClassicTemplate, pTestReplacer);
		}
		tName = firstToUpper(tName);
		if (pAsset.finalizer != null)
		{
			tName = pAsset.finalizer(tName);
		}
		if (_dict_splitted_items != null)
		{
			foreach (ListPool<string> value in _dict_splitted_items.Values)
			{
				value.Dispose();
			}
			_dict_splitted_items.Clear();
			UnsafeCollectionPool<Dictionary<string, ListPool<string>>, KeyValuePair<string, ListPool<string>>>.Release(_dict_splitted_items);
			_dict_splitted_items = null;
		}
		return tName;
	}

	private static string addWord(NameGeneratorAsset pAsset, int pMin, int pMax, bool pToUpperFirst = false)
	{
		string tName = "";
		int tWidth = Randy.randomInt(pMin, pMax);
		for (int i = 0; i < tWidth; i++)
		{
			if (_current_consonants >= pAsset.max_consonants_in_row)
			{
				tName += addVowel(pAsset.vowels, pToUpperFirst);
				pToUpperFirst = false;
			}
			else if (_current_vowels >= pAsset.max_vowels_in_row)
			{
				tName += addConsonant(pAsset.consonants, pToUpperFirst);
				pToUpperFirst = false;
			}
			else if (Randy.randomBool())
			{
				tName += addVowel(pAsset.vowels, pToUpperFirst);
				pToUpperFirst = false;
			}
			else
			{
				tName += addConsonant(pAsset.consonants, pToUpperFirst);
				pToUpperFirst = false;
			}
		}
		return tName;
	}
}
