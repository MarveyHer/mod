using System;
using System.Text.RegularExpressions;

[Serializable]
public class LanguageStructure
{
	public string[] sets_vowels;

	public string[] sets_consonants;

	public string[] sets_onset_1;

	public string[] sets_onset_2;

	public string[] sets_codas_1;

	public string[] sets_codas_2;

	public string[] sets_diphthongs;

	public string[] syllables_start;

	public string[] syllables_mid;

	public string[] syllables_ends;

	public string[] word_patterns;

	public float[] word_weights;

	public ArticleSettings settings_articles;

	public PrefixesSettings settings_prefixes;

	public SuffixesSettings settings_suffixes;

	public LanguageStructure()
	{
		generateSyllableSets();
	}

	public void generateSyllableSets()
	{
		if (syllables_start == null)
		{
			generateMainParts();
			generatePatterns();
			int tArticleMinSize = Randy.randomInt(1, 2);
			int tArticleMaxSize = Randy.randomInt(1, 3);
			settings_articles = new ArticleSettings();
			settings_articles.create(this, tArticleMinSize, tArticleMaxSize);
			settings_prefixes = new PrefixesSettings();
			settings_prefixes.create(this, 0, 4);
			settings_suffixes = new SuffixesSettings();
			settings_suffixes.create(this, 0, 4);
			syllables_start = generateSyllables("syllable_starts", Randy.randomInt(2, 10));
			syllables_mid = generateSyllables("syllable_mids", Randy.randomInt(2, 10));
			syllables_ends = generateSyllables("syllable_ends", Randy.randomInt(2, 10));
		}
	}

	private void generatePatterns()
	{
		int tTotal = Randy.randomInt(3, 10);
		word_patterns = new string[tTotal];
		word_weights = new float[tTotal];
		for (int i = 0; i < tTotal; i++)
		{
			word_patterns[i] = LanguageStructureHelpers.possible_word_patterns.GetRandom();
			word_weights[i] = Randy.randomFloat(0.05f, 1f);
		}
	}

	private void generateMainParts()
	{
		sets_consonants = generateParts("consonant", 5);
		sets_vowels = generateParts("vowel", 5);
		sets_onset_1 = generateParts("onset1", 5);
		sets_onset_2 = generateParts("onset2", 5);
		sets_codas_1 = generateParts("coda1", 5);
		sets_codas_2 = generateParts("coda2", 5);
		sets_diphthongs = generateParts("diphthongs", 5);
	}

	private string[] generateParts(string pID, int pAmount)
	{
		LinguisticsAsset tLinAsset = AssetManager.linguistics_library.get(pID);
		string[] tResultParts = new string[pAmount];
		for (int i = 0; i < pAmount; i++)
		{
			tResultParts[i] = tLinAsset.getRandom();
		}
		return tResultParts;
	}

	private string[] generateSyllables(string pID, int pAmount)
	{
		string[] tResultSyllables = new string[pAmount];
		LinguisticsAsset tLinAsset = AssetManager.linguistics_library.get(pID);
		for (int iAmount = 0; iAmount < pAmount; iAmount++)
		{
			string[] tPattern = tLinAsset.getRandomPattern();
			string tPatternMerged = string.Join("", tPattern);
			using (new StringBuilderPool())
			{
				string tPartOnset = string.Empty;
				string tPartNucleus = string.Empty;
				string tPartCoda = string.Empty;
				if (tPatternMerged.StartsWith("CC"))
				{
					tPartOnset = sets_onset_2.GetRandom();
				}
				else if (tPatternMerged.StartsWith("C"))
				{
					tPartOnset = sets_onset_1.GetRandom();
				}
				tPartNucleus = ((tPatternMerged.Contains("VV") || Randy.randomChance(0.2f)) ? sets_diphthongs.GetRandom() : sets_vowels.GetRandom());
				if (tPatternMerged.EndsWith("CC"))
				{
					tPartCoda = sets_codas_2.GetRandom();
				}
				else if (tPatternMerged.EndsWith("C"))
				{
					tPartCoda = sets_codas_1.GetRandom();
				}
				string tResult = tPartOnset + tPartNucleus + tPartCoda;
				tResultSyllables[iAmount] = tResult;
			}
		}
		return tResultSyllables;
	}

	private string fixOrthography(string pSyllable)
	{
		if (string.IsNullOrEmpty(pSyllable))
		{
			return pSyllable;
		}
		string tResult = pSyllable;
		tResult = Regex.Replace(tResult, "([bcdfghjklmnpqrstvwxyz])\\1{2,}", "$1$1");
		tResult = tResult.Replace("ck", "ck");
		tResult = tResult.Replace("kk", "ck");
		tResult = tResult.Replace("cc", "ck");
		tResult = Regex.Replace(tResult, "qw|qv", "qu");
		tResult = tResult.Replace("q", "qu");
		tResult = Regex.Replace(tResult, "aa+", "a");
		tResult = Regex.Replace(tResult, "ii+", "i");
		tResult = Regex.Replace(tResult, "uu+", "u");
		if (tResult.StartsWith("x"))
		{
			tResult = "z" + tResult.Substring(1);
		}
		tResult = Regex.Replace(tResult, "([bcdfghjklmnpqrstvwxyz])\\1\\1+", "$1$1");
		tResult = tResult.Replace("tch", "ch");
		tResult = tResult.Replace("dge", "ge");
		if (tResult.Length > 2)
		{
			string start = tResult.Substring(0, 2).ToLower();
			string[] array = new string[7] { "kg", "pn", "gn", "kn", "wr", "mn", "ps" };
			foreach (string cluster in array)
			{
				if (start == cluster)
				{
					tResult = tResult.Substring(1);
					break;
				}
			}
		}
		return tResult;
	}
}
