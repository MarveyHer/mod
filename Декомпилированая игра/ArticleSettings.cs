public class ArticleSettings : StructureSettings
{
	public override void create(LanguageStructure pStructure, int pSizeMin, int pSizeMax)
	{
		WordType[] word_types = LanguageStructureHelpers.word_types;
		foreach (WordType tType in word_types)
		{
			generate(pStructure, tType, pSizeMin, pSizeMax);
		}
	}

	public void generate(LanguageStructure pStructure, WordType pWord, int pSizeMin, int pSizeMax)
	{
		bool tEnabled = Randy.randomBool();
		enabled[(int)pWord] = tEnabled;
		if (tEnabled)
		{
			sets[(int)pWord] = generateSets(pStructure, Randy.randomInt(pSizeMin, pSizeMax));
			separator[(int)pWord] = LanguageStructureHelpers.possible_article_separators.GetRandom();
		}
	}

	private string[] generateSets(LanguageStructure pStructure, int pAmount)
	{
		string[] tResultArticles = new string[pAmount];
		for (int i = 0; i < pAmount; i++)
		{
			tResultArticles[i] = Randy.randomInt(0, 5) switch
			{
				0 => pStructure.sets_consonants.GetRandom() + pStructure.sets_vowels.GetRandom() + pStructure.sets_consonants.GetRandom(), 
				1 => pStructure.sets_onset_2.GetRandom() + pStructure.sets_vowels.GetRandom(), 
				2 => pStructure.sets_consonants.GetRandom() + pStructure.sets_vowels.GetRandom(), 
				3 => pStructure.sets_vowels.GetRandom() + pStructure.sets_consonants.GetRandom() + pStructure.sets_vowels.GetRandom(), 
				_ => pStructure.sets_vowels.GetRandom() ?? "", 
			};
		}
		return tResultArticles;
	}
}
