using System.Collections.Generic;
using UnityEngine;

public class Chromosome
{
	private const string IMAGE_PATH_NORMAL = "chromosomes/normal/";

	private const string IMAGE_PATH_GOLD = "chromosomes/golden/";

	private const string STRING_UNKOWN = "???????";

	private const string COLOR_BOUND = "#444444";

	private const string COLORED_UNKOWN_TEXT = "<color=#444444>???????</color>";

	public readonly List<GeneAsset> genes = new List<GeneAsset>();

	private readonly BaseStats _merged_base_stats_male = new BaseStats();

	private readonly BaseStats _merged_base_stats_female = new BaseStats();

	private readonly BaseStats _merged_base_stats = new BaseStats();

	private readonly BaseStats _merged_base_stats_meta = new BaseStats();

	private static readonly (int, int)[] DIRECTIONS = new(int, int)[4]
	{
		(0, -1),
		(0, 1),
		(-1, 0),
		(1, 0)
	};

	private bool _dirty = true;

	private Sprite _cached_sprite;

	private int _cached_sprite_index = -1;

	public string chromosome_type;

	private readonly List<int> _loci_amplifiers = new List<int>();

	private readonly List<int> _loci_empty = new List<int>();

	private readonly BaseStats[] _base_stats_all = new BaseStats[4];

	private readonly int _columns;

	public Chromosome(string pType, bool pNew)
	{
		chromosome_type = pType;
		if (pNew)
		{
			int tSize = getAsset().amount_loci;
			GeneAsset tEmptyGene = AssetManager.gene_library.get("empty");
			for (int i = 0; i < tSize; i++)
			{
				genes.Add(tEmptyGene);
			}
			generateAmplifiers(pType);
		}
		_base_stats_all[0] = _merged_base_stats;
		_base_stats_all[1] = _merged_base_stats_meta;
		_base_stats_all[2] = _merged_base_stats_male;
		_base_stats_all[3] = _merged_base_stats_female;
		_columns = getAsset().amount_loci / 6;
	}

	public bool isLocusAmplifier(int pX, int pY)
	{
		int tIndex = getIndexFrom(pX, pY);
		return isLocusAmplifier(tIndex);
	}

	public bool isLocusAmplifier(int pLocusIndex)
	{
		return _loci_amplifiers.Contains(pLocusIndex);
	}

	public bool isVoidLocus(int pLocusIndex)
	{
		return _loci_empty.Contains(pLocusIndex);
	}

	public bool isSpecialLocusAt(int pX, int pY)
	{
		int tIndex = getIndexFrom(pX, pY);
		return isSpecialLocus(tIndex);
	}

	public bool isVoidLocusAt(int pX, int pY)
	{
		int tIndex = getIndexFrom(pX, pY);
		return isVoidLocus(tIndex);
	}

	public bool isAllSidesVoidLocus(int pX, int pY)
	{
		int tTotalSides = countBounds(pX, pY);
		int tCount = 0;
		if (isVoidLocusAt(pX - 1, pY))
		{
			tCount++;
		}
		if (isVoidLocusAt(pX + 1, pY))
		{
			tCount++;
		}
		if (isVoidLocusAt(pX, pY + 1))
		{
			tCount++;
		}
		if (isVoidLocusAt(pX, pY - 1))
		{
			tCount++;
		}
		return tCount == tTotalSides;
	}

	private bool isAmplifierLocusAt(int pX, int pY)
	{
		int tIndex = getIndexFrom(pX, pY);
		return isLocusAmplifier(tIndex);
	}

	private bool isForcedSynergyAt(int pX, int pY)
	{
		if (isAmplifierLocusAt(pX, pY))
		{
			return true;
		}
		if (getGeneAt(pX, pY).synergy_sides_always)
		{
			return true;
		}
		return false;
	}

	public bool isForcedSynergyLeft(int pX, int pY)
	{
		if (hasBoundLeft(pX, pY))
		{
			return false;
		}
		var (tX, tY) = getDirectionOffset(GeneDirection.Left);
		return isForcedSynergyAt(pX + tX, pY + tY);
	}

	public bool isForcedSynergyRight(int pX, int pY)
	{
		if (hasBoundRight(pX, pY))
		{
			return false;
		}
		var (tX, tY) = getDirectionOffset(GeneDirection.Right);
		return isForcedSynergyAt(pX + tX, pY + tY);
	}

	public bool isForcedSynergyUp(int pX, int pY)
	{
		if (hasBoundUp(pX, pY))
		{
			return false;
		}
		var (tX, tY) = getDirectionOffset(GeneDirection.Up);
		return isForcedSynergyAt(pX + tX, pY + tY);
	}

	public bool isForcedSynergyDown(int pX, int pY)
	{
		if (hasBoundDown(pX, pY))
		{
			return false;
		}
		var (tX, tY) = getDirectionOffset(GeneDirection.Down);
		return isForcedSynergyAt(pX + tX, pY + tY);
	}

	public LocusType getLocusType(int pLocusIndex)
	{
		if (isLocusAmplifier(pLocusIndex))
		{
			return LocusType.Amplifier;
		}
		if (isVoidLocus(pLocusIndex))
		{
			return LocusType.Empty;
		}
		return LocusType.Standard;
	}

	public void fillStatsForTooltip(LocusElement pLocus, BaseStats pStatsContainer)
	{
		int tLocusIndex = pLocus.locus_index;
		if (!isVoidLocus(tLocusIndex))
		{
			GeneAsset tGeneAsset = pLocus.getGeneAsset();
			if (tGeneAsset.is_bonus_male)
			{
				combineBonusesForSides(tLocusIndex, pStatsContainer);
			}
			else if (tGeneAsset.is_bonus_female)
			{
				combineBonusesForSides(tLocusIndex, pStatsContainer);
			}
			else
			{
				getBonusesFromGene(pLocus.locus_index, pStatsContainer, null, pCombineMeta: true);
			}
			pStatsContainer.normalize();
		}
	}

	private void generateAmplifiers(string pType)
	{
		ChromosomeTypeAsset tAsset = AssetManager.chromosome_type_library.get(pType);
		using ListPool<int> tList = new ListPool<int>();
		for (int i = 0; i < tAsset.amount_loci; i++)
		{
			tList.Add(i);
		}
		tList.Shuffle();
		int tAmountOfLociAmplifiers = Randy.randomInt(tAsset.amount_loci_min_amplifier, tAsset.amount_loci_max_amplifier);
		int tAmountOfLociEmpty = Randy.randomInt(tAsset.amount_loci_min_empty, tAsset.amount_loci_max_empty);
		for (int j = 0; j < tAmountOfLociAmplifiers; j++)
		{
			_loci_amplifiers.Add(tList.Pop());
		}
		for (int k = 0; k < tAmountOfLociEmpty; k++)
		{
			_loci_empty.Add(tList.Pop());
		}
	}

	public bool canAddGene(GeneAsset pAsset)
	{
		if (countEmpty() == 0)
		{
			return false;
		}
		return true;
	}

	public void setGene(GeneAsset pAsset, int pIndex)
	{
		genes[pIndex] = pAsset;
	}

	public GeneAsset getGene(int pIndex)
	{
		return genes[pIndex];
	}

	public ChromosomeTypeAsset getAsset()
	{
		return AssetManager.chromosome_type_library.get(chromosome_type);
	}

	public void load(ChromosomeData pData)
	{
		chromosome_type = pData.chromosome_type;
		foreach (string tGeneID in pData.loci)
		{
			GeneAsset tGeneAsset = AssetManager.gene_library.get(tGeneID);
			if (tGeneAsset != null)
			{
				genes.Add(tGeneAsset);
			}
		}
		_loci_amplifiers.AddRange(pData.super_loci);
		_loci_empty.AddRange(pData.void_loci);
	}

	public ChromosomeData getDataForSave()
	{
		ChromosomeData tData = new ChromosomeData();
		foreach (GeneAsset tAsset in genes)
		{
			tData.loci.Add(tAsset.id);
		}
		tData.super_loci.AddRange(_loci_amplifiers);
		tData.void_loci.AddRange(_loci_empty);
		tData.chromosome_type = chromosome_type;
		return tData;
	}

	public void addGene(GeneAsset pGeneAsset)
	{
		for (int i = 0; i < genes.Count; i++)
		{
			if (genes[i].is_empty && canAddToLocus(i))
			{
				genes[i] = pGeneAsset;
				break;
			}
		}
		setDirty();
	}

	public bool isSpecialLocus(int pIndex)
	{
		if (!_loci_amplifiers.Contains(pIndex))
		{
			return _loci_empty.Contains(pIndex);
		}
		return true;
	}

	public bool canAddToLocus(int pIndex)
	{
		if (_loci_amplifiers.Contains(pIndex))
		{
			return false;
		}
		if (_loci_empty.Contains(pIndex))
		{
			return false;
		}
		return true;
	}

	public int countNonEmpty()
	{
		int tResult = 0;
		for (int i = 0; i < genes.Count; i++)
		{
			if (!genes[i].is_empty && canAddToLocus(i))
			{
				tResult++;
			}
		}
		return tResult;
	}

	public int countEmpty()
	{
		int tResult = 0;
		for (int i = 0; i < genes.Count; i++)
		{
			if (genes[i].is_empty && canAddToLocus(i))
			{
				tResult++;
			}
		}
		return tResult;
	}

	public BaseStats getStats()
	{
		if (_dirty)
		{
			recalculate();
		}
		return _merged_base_stats;
	}

	public BaseStats getStatsMeta()
	{
		if (_dirty)
		{
			recalculate();
		}
		return _merged_base_stats_meta;
	}

	public BaseStats getStatsMale()
	{
		if (_dirty)
		{
			recalculate();
		}
		return _merged_base_stats_male;
	}

	public BaseStats getStatsFemale()
	{
		if (_dirty)
		{
			recalculate();
		}
		return _merged_base_stats_female;
	}

	public void setDirty()
	{
		_dirty = true;
	}

	public void recalculate()
	{
		if (!_dirty)
		{
			return;
		}
		_dirty = false;
		clearAllBaseStats();
		BaseStats tBaseStats = _merged_base_stats;
		BaseStats tBaseStatsMeta = _merged_base_stats_meta;
		BaseStats tBaseStatsMale = _merged_base_stats_male;
		BaseStats tBaseStatsFemale = _merged_base_stats_female;
		for (int i = 0; i < genes.Count; i++)
		{
			if (!isVoidLocus(i))
			{
				getBonusesFromGene(i, tBaseStats, tBaseStatsMeta);
			}
		}
		for (int j = 0; j < genes.Count; j++)
		{
			GeneAsset tGene = genes[j];
			if (!isVoidLocus(j))
			{
				if (tGene.is_bonus_male)
				{
					combineBonusesForSides(j, tBaseStatsMale);
				}
				if (tGene.is_bonus_female)
				{
					combineBonusesForSides(j, tBaseStatsFemale);
				}
			}
		}
	}

	private void combineBonusesForSides(int pLocusIndex, BaseStats pBaseStatsMain)
	{
		(int, int) xYFromIndex = getXYFromIndex(pLocusIndex);
		int tX = xYFromIndex.Item1;
		int tY = xYFromIndex.Item2;
		bool num = isNextToBad(pLocusIndex);
		getBonusesFromGene(tX, tY + 1, pBaseStatsMain);
		getBonusesFromGene(tX, tY - 1, pBaseStatsMain);
		getBonusesFromGene(tX - 1, tY, pBaseStatsMain);
		getBonusesFromGene(tX + 1, tY, pBaseStatsMain);
		if (num)
		{
			BaseStatsContainer[] array = pBaseStatsMain.getList().ToArray();
			foreach (BaseStatsContainer tStatsContainer in array)
			{
				float tVar = ((!Mathf.Approximately(Mathf.Floor(tStatsContainer.value), tStatsContainer.value)) ? (tStatsContainer.value * 0.5f) : Mathf.Floor(tStatsContainer.value * 0.5f));
				pBaseStatsMain[tStatsContainer.id] = tVar;
			}
			pBaseStatsMain.normalize();
		}
	}

	private void getBonusesFromGene(int pX, int pY, BaseStats pBaseStatsMain, BaseStats pBaseStatsMeta = null, bool pCombineMeta = false)
	{
		if (getGeneAt(pX, pY) != null)
		{
			int tLocusIndex = getIndexFrom(pX, pY);
			getBonusesFromGene(tLocusIndex, pBaseStatsMain, pBaseStatsMeta, pCombineMeta);
		}
	}

	private void getBonusesFromGene(int pLocusIndex, BaseStats pBaseStatsMain, BaseStats pBaseStatsMeta = null, bool pCombineMeta = false)
	{
		GeneAsset tGene = genes[pLocusIndex];
		bool tSynergyBonus = hasFullSynergy(pLocusIndex);
		bool num = isNextToBad(pLocusIndex);
		if (num)
		{
			tSynergyBonus = false;
		}
		if (num)
		{
			pBaseStatsMain.mergeStats(tGene.getHalfStats());
			if (pCombineMeta)
			{
				pBaseStatsMain.mergeStats(tGene.getHalfStatsMeta());
			}
			else
			{
				pBaseStatsMeta?.mergeStats(tGene.getHalfStatsMeta());
			}
			return;
		}
		pBaseStatsMain.mergeStats(tGene.base_stats);
		if (pCombineMeta)
		{
			pBaseStatsMain.mergeStats(tGene.base_stats_meta);
		}
		pBaseStatsMeta?.mergeStats(tGene.base_stats_meta);
		if (tSynergyBonus && !tGene.synergy_sides_always)
		{
			pBaseStatsMain.mergeStats(tGene.base_stats);
			if (pCombineMeta)
			{
				pBaseStatsMain.mergeStats(tGene.base_stats_meta);
			}
			else
			{
				pBaseStatsMeta?.mergeStats(tGene.base_stats_meta);
			}
		}
	}

	private void clearAllBaseStats()
	{
		BaseStats[] base_stats_all = _base_stats_all;
		for (int i = 0; i < base_stats_all.Length; i++)
		{
			base_stats_all[i].clear();
		}
	}

	public bool hasFullSynergyAt(int pX, int pY)
	{
		int tCount = 0;
		if (isAllSidesVoidLocus(pX, pY))
		{
			return false;
		}
		if (isNextToBad(pX, pY))
		{
			return false;
		}
		if (isNextToBadAmplifier(pX, pY))
		{
			return false;
		}
		if (hasSynergyConnectionLeft(pX, pY))
		{
			tCount++;
		}
		if (hasSynergyConnectionRight(pX, pY))
		{
			tCount++;
		}
		if (hasSynergyConnectionUp(pX, pY))
		{
			tCount++;
		}
		if (hasSynergyConnectionDown(pX, pY))
		{
			tCount++;
		}
		int tTotalSides = 0;
		if (!hasBoundLeft(pX, pY))
		{
			tTotalSides++;
		}
		if (!hasBoundRight(pX, pY))
		{
			tTotalSides++;
		}
		if (!hasBoundUp(pX, pY))
		{
			tTotalSides++;
		}
		if (!hasBoundDown(pX, pY))
		{
			tTotalSides++;
		}
		return tCount == tTotalSides;
	}

	public bool hasFullSynergy(int pLocusIndex)
	{
		var (tX, tY) = getXYFromIndex(pLocusIndex);
		return hasFullSynergyAt(tX, tY);
	}

	public bool hasAnySynergy(int pLocusIndex)
	{
		var (tX, tY) = getXYFromIndex(pLocusIndex);
		if (hasSynergyConnectionLeft(tX, tY))
		{
			return true;
		}
		if (hasSynergyConnectionRight(tX, tY))
		{
			return true;
		}
		if (hasSynergyConnectionUp(tX, tY))
		{
			return true;
		}
		if (hasSynergyConnectionDown(tX, tY))
		{
			return true;
		}
		return false;
	}

	public string getSynergyTooltipText(int pLocusIndex)
	{
		(int, int) xYFromIndex = getXYFromIndex(pLocusIndex);
		int tFromX = xYFromIndex.Item1;
		int tFromY = xYFromIndex.Item2;
		GeneAsset tGene = getGeneAt(tFromX, tFromY);
		using StringBuilderPool tBuilder = new StringBuilderPool();
		bool tBadHere = isBadAt(tFromX, tFromY);
		if (hasAnySynergy(pLocusIndex) && !tBadHere)
		{
			tBuilder.Append(Toolbox.coloredString(LocalizedTextManager.getText("sequence_synergy"), "#FFFFAA"));
		}
		else
		{
			tBuilder.Append(LocalizedTextManager.getText("sequence_synergy"));
		}
		tBuilder.Append("\n");
		bool num = hasSynergyConnectionLeft(tFromX, tFromY);
		bool tHasRight = hasSynergyConnectionRight(tFromX, tFromY);
		bool num2 = hasSynergyConnectionUp(tFromX, tFromY);
		bool tHasDown = hasSynergyConnectionDown(tFromX, tFromY);
		GeneAsset tSideAssetLeft = getGeneLeft(tFromX, tFromY);
		GeneAsset tSideAssetRight = getGeneRight(tFromX, tFromY);
		GeneAsset tSideAssetUp = getGeneUp(tFromX, tFromY);
		GeneAsset tSideAssetDown = getGeneDown(tFromX, tFromY);
		bool isForcedSynergyHere = isForcedSynergyAt(tFromX, tFromY);
		if (num2)
		{
			if (tBadHere || isBadAt(tFromX, tFromY - 1) || hasAmplifierBad(tFromX, tFromY - 1))
			{
				tBuilder.Append(getBadConnectionString());
			}
			else if (isForcedSynergyHere)
			{
				tBuilder.Append(NucleobaseHelper.getColoredNucleobaseFull(tSideAssetUp.genetic_code_down));
			}
			else
			{
				tBuilder.Append(NucleobaseHelper.getColoredNucleobaseFull(tGene.genetic_code_up));
			}
		}
		else if (hasBoundUp(tFromX, tFromY) || isConnectionDeniedUp(tFromX, tFromY))
		{
			tBuilder.Append("<color=#444444>???????</color>");
		}
		else
		{
			tBuilder.Append(getNotConnectedText(tGene.genetic_code_up, World.world.getCurSessionTime()));
		}
		tBuilder.Append("\n");
		if (num)
		{
			if (tBadHere || isBadAt(tFromX - 1, tFromY) || hasAmplifierBad(tFromX - 1, tFromY))
			{
				tBuilder.Append(getBadConnectionString());
			}
			else if (isForcedSynergyHere)
			{
				tBuilder.Append(NucleobaseHelper.getColoredNucleobaseFull(tSideAssetLeft.genetic_code_right));
			}
			else
			{
				tBuilder.Append(NucleobaseHelper.getColoredNucleobaseFull(tGene.genetic_code_left));
			}
		}
		else if (hasBoundLeft(tFromX, tFromY) || isConnectionDeniedLeft(tFromX, tFromY))
		{
			tBuilder.Append("<color=#444444>???????</color>");
		}
		else
		{
			tBuilder.Append(getNotConnectedText(tGene.genetic_code_left, World.world.getCurSessionTime()));
		}
		tBuilder.Append(" ... ");
		if (tHasRight)
		{
			if (tBadHere || isBadAt(tFromX + 1, tFromY) || hasAmplifierBad(tFromX + 1, tFromY))
			{
				tBuilder.Append(getBadConnectionString());
			}
			else if (isForcedSynergyHere)
			{
				tBuilder.Append(NucleobaseHelper.getColoredNucleobaseFull(tSideAssetRight.genetic_code_left));
			}
			else
			{
				tBuilder.Append(NucleobaseHelper.getColoredNucleobaseFull(tGene.genetic_code_right));
			}
		}
		else if (hasBoundRight(tFromX, tFromY) || isConnectionDeniedRight(tFromX, tFromY))
		{
			tBuilder.Append("<color=#444444>???????</color>");
		}
		else
		{
			tBuilder.Append(getNotConnectedText(tGene.genetic_code_right, World.world.getCurSessionTime()));
		}
		tBuilder.Append("\n");
		if (tHasDown)
		{
			if (tBadHere || isBadAt(tFromX, tFromY + 1) || hasAmplifierBad(tFromX, tFromY + 1))
			{
				tBuilder.Append(getBadConnectionString());
			}
			else if (isForcedSynergyHere)
			{
				tBuilder.Append(NucleobaseHelper.getColoredNucleobaseFull(tSideAssetDown.genetic_code_up));
			}
			else
			{
				tBuilder.Append(NucleobaseHelper.getColoredNucleobaseFull(tGene.genetic_code_down));
			}
		}
		else if (hasBoundDown(tFromX, tFromY) || isConnectionDeniedDown(tFromX, tFromY))
		{
			tBuilder.Append("<color=#444444>???????</color>");
		}
		else
		{
			tBuilder.Append(getNotConnectedText(tGene.genetic_code_down, World.world.getCurSessionTime()));
		}
		tBuilder.Append("\n");
		tBuilder.Append("\n");
		return tBuilder.ToString();
	}

	private string getBadConnectionString()
	{
		return InsultStringGenerator.getBadConnectionString();
	}

	private string getNotConnectedText(char pChar, double pTime)
	{
		string tFullNucleobase = NucleobaseHelper.getFullNucleobaseName(pChar);
		string tColor = NucleobaseHelper.getColorHex(pChar, pDark: true);
		using StringBuilderPool tBuilder = new StringBuilderPool();
		for (int i = 0; i < tFullNucleobase.Length; i++)
		{
			tBuilder.Append(tFullNucleobase[i]);
		}
		int tCharInt = pChar * 100;
		int xPosition = (int)((pTime + (double)tCharInt) * 8.0 % (double)tFullNucleobase.Length);
		tBuilder[xPosition] = '?';
		return Toolbox.coloredString(tBuilder.ToString(), tColor);
	}

	private int getIndexFrom(int pX, int pY)
	{
		return pX + pY * 6;
	}

	public (int, int) getXYFromIndex(int pIndex)
	{
		int item = pIndex % 6;
		int tY = pIndex / 6;
		return (item, tY);
	}

	public Sprite getSpriteNormal()
	{
		Sprite[] tSpriteList = SpriteTextureLoader.getSpriteList("chromosomes/normal/");
		if (_cached_sprite_index == -1)
		{
			_cached_sprite_index = Randy.randomInt(0, tSpriteList.Length - 1);
		}
		_cached_sprite = tSpriteList[_cached_sprite_index];
		return _cached_sprite;
	}

	public Sprite getSpriteGolden()
	{
		Sprite[] tSpriteList = SpriteTextureLoader.getSpriteList("chromosomes/golden/");
		if (_cached_sprite_index == -1)
		{
			_cached_sprite_index = Randy.randomInt(0, tSpriteList.Length - 1);
		}
		_cached_sprite = tSpriteList[_cached_sprite_index];
		return _cached_sprite;
	}

	public void cloneFrom(Chromosome pParentChromosome)
	{
		genes.AddRange(pParentChromosome.genes);
		_loci_empty.AddRange(pParentChromosome._loci_empty);
		_loci_amplifiers.AddRange(pParentChromosome._loci_amplifiers);
		setDirty();
	}

	public void mutateRandomGene()
	{
		using ListPool<int> tList = new ListPool<int>();
		for (int i = 0; i < genes.Count; i++)
		{
			if (!isSpecialLocus(i))
			{
				tList.Add(i);
			}
		}
		int tIndex = tList.GetRandom();
		GeneAsset tNewGene = AssetManager.gene_library.getRandomGeneForMutation();
		setGene(tNewGene, tIndex);
		setDirty();
	}

	public bool hasGene(GeneAsset pAsset)
	{
		return genes?.Contains(pAsset) ?? false;
	}

	public GeneAsset getGeneAtDirectionFrom(int pFromX, int pFromY, GeneDirection pDirection)
	{
		(int, int) directionOffset = getDirectionOffset(pDirection);
		int tX = directionOffset.Item1;
		int tY = directionOffset.Item2;
		int tPositionX = pFromX + tX;
		int tPositionY = pFromY + tY;
		if (!isCoordinatesValid(tPositionX, tPositionY))
		{
			return null;
		}
		int tIndex = getIndexFrom(pFromX + tX, pFromY + tY);
		return genes[tIndex];
	}

	public GeneAsset getGeneAt(int pFromX, int pFromY)
	{
		if (!isCoordinatesValid(pFromX, pFromY))
		{
			return null;
		}
		int tIndex = getIndexFrom(pFromX, pFromY);
		if (!isIndexValid(tIndex))
		{
			return null;
		}
		return genes[tIndex];
	}

	public GeneAsset getGeneLeft(int pFromX, int pFromY)
	{
		return getGeneAtDirectionFrom(pFromX, pFromY, GeneDirection.Left);
	}

	public GeneAsset getGeneRight(int pFromX, int pFromY)
	{
		return getGeneAtDirectionFrom(pFromX, pFromY, GeneDirection.Right);
	}

	public GeneAsset getGeneUp(int pFromX, int pFromY)
	{
		return getGeneAtDirectionFrom(pFromX, pFromY, GeneDirection.Up);
	}

	public GeneAsset getGeneDown(int pFromX, int pFromY)
	{
		return getGeneAtDirectionFrom(pFromX, pFromY, GeneDirection.Down);
	}

	private bool isIndexValid(int pIndex)
	{
		if (pIndex < 0)
		{
			return false;
		}
		if (pIndex >= genes.Count)
		{
			return false;
		}
		return true;
	}

	private bool isCoordinatesValid(int pX, int pY)
	{
		if (pX < 0)
		{
			return false;
		}
		if (pY < 0)
		{
			return false;
		}
		if (pX >= 6)
		{
			return false;
		}
		if (pY >= _columns)
		{
			return false;
		}
		return true;
	}

	public (int, int) getDirectionOffset(GeneDirection pDirection)
	{
		return pDirection switch
		{
			GeneDirection.Up => DIRECTIONS[0], 
			GeneDirection.Down => DIRECTIONS[1], 
			GeneDirection.Left => DIRECTIONS[2], 
			GeneDirection.Right => DIRECTIONS[3], 
			_ => (0, 0), 
		};
	}

	public bool canBeConnectedTo(int pFromX, int pFromY, int pToX, int pToY)
	{
		GeneAsset tFromAsset = getGeneAt(pFromX, pFromY);
		GeneAsset tToAsset = getGeneAt(pToX, pToY);
		if (tFromAsset == null || tToAsset == null)
		{
			return false;
		}
		if (!tFromAsset.is_empty)
		{
			_ = tToAsset.is_empty;
		}
		return false;
	}

	public int countBounds(int pX, int pY)
	{
		int tCount = 0;
		if (isCoordinatesValid(pX - 1, pY))
		{
			tCount++;
		}
		if (isCoordinatesValid(pX + 1, pY))
		{
			tCount++;
		}
		if (isCoordinatesValid(pX, pY - 1))
		{
			tCount++;
		}
		if (isCoordinatesValid(pX, pY + 1))
		{
			tCount++;
		}
		return tCount;
	}

	public bool hasSynergyConnectionLeft(int pFromX, int pFromY)
	{
		if (hasBoundLeft(pFromX, pFromY))
		{
			return false;
		}
		return hasSynergyConnection(pFromX, pFromY, GeneDirection.Left);
	}

	public bool hasSynergyConnectionRight(int pFromX, int pFromY)
	{
		if (hasBoundRight(pFromX, pFromY))
		{
			return false;
		}
		return hasSynergyConnection(pFromX, pFromY, GeneDirection.Right);
	}

	public bool hasSynergyConnectionUp(int pFromX, int pFromY)
	{
		if (hasBoundUp(pFromX, pFromY))
		{
			return false;
		}
		return hasSynergyConnection(pFromX, pFromY, GeneDirection.Up);
	}

	public bool hasSynergyConnectionDown(int pFromX, int pFromY)
	{
		if (hasBoundDown(pFromX, pFromY))
		{
			return false;
		}
		return hasSynergyConnection(pFromX, pFromY, GeneDirection.Down);
	}

	public bool isAllLociSynergy()
	{
		int tIndex = -1;
		foreach (GeneAsset tGene in genes)
		{
			tIndex++;
			if (!tGene.is_empty && !tGene.synergy_sides_always)
			{
				if (tGene.is_bad)
				{
					return false;
				}
				var (tX, tY) = getXYFromIndex(tIndex);
				if (!hasAllSynergiesAt(tX, tY, pCheckBounds: false))
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool hasAllSynergiesAt(int pFromX, int pFromY, bool pCheckBounds = true)
	{
		if (isAllSidesVoidLocus(pFromX, pFromY))
		{
			return false;
		}
		bool num = (pCheckBounds ? hasSynergyConnectionLeft(pFromX, pFromY) : (hasBoundLeft(pFromX, pFromY) || hasSynergyConnection(pFromX, pFromY, GeneDirection.Left)));
		bool tRight = (pCheckBounds ? hasSynergyConnectionRight(pFromX, pFromY) : (hasBoundRight(pFromX, pFromY) || hasSynergyConnection(pFromX, pFromY, GeneDirection.Right)));
		bool tUp = (pCheckBounds ? hasSynergyConnectionUp(pFromX, pFromY) : (hasBoundUp(pFromX, pFromY) || hasSynergyConnection(pFromX, pFromY, GeneDirection.Up)));
		bool tDown = (pCheckBounds ? hasSynergyConnectionDown(pFromX, pFromY) : (hasBoundDown(pFromX, pFromY) || hasSynergyConnection(pFromX, pFromY, GeneDirection.Down)));
		return num && tRight && tUp && tDown;
	}

	public bool hasSynergyConnection(int pFromX, int pFromY, GeneDirection pDirection)
	{
		GeneAsset tAssetParent = getGeneAt(pFromX, pFromY);
		bool tLocusAmplifier = isAmplifierLocusAt(pFromX, pFromY);
		bool tAnyBad = false;
		if (tAssetParent.synergy_sides_always)
		{
			tLocusAmplifier = true;
		}
		if (tAssetParent.is_bad)
		{
			tAnyBad = true;
		}
		if (!tLocusAmplifier && tAssetParent.is_empty)
		{
			return false;
		}
		(int, int) directionOffset = getDirectionOffset(pDirection);
		int tX = directionOffset.Item1;
		int tY = directionOffset.Item2;
		GeneAsset tAssetSide = getGeneAt(pFromX + tX, pFromY + tY);
		bool tForcedSynergySide = isAmplifierLocusAt(pFromX + tX, pFromY + tY);
		if (tAssetSide != null && tAssetSide.synergy_sides_always)
		{
			tForcedSynergySide = true;
		}
		if (tAssetSide != null && tAssetSide.is_bad)
		{
			tAnyBad = true;
		}
		if (!tForcedSynergySide)
		{
			if (tAssetSide == null)
			{
				return false;
			}
			if (tAssetSide.is_empty)
			{
				return false;
			}
		}
		if (!tAnyBad && tLocusAmplifier && tForcedSynergySide)
		{
			return false;
		}
		switch (pDirection)
		{
		case GeneDirection.Up:
			if (tLocusAmplifier || tForcedSynergySide)
			{
				return true;
			}
			if (tAssetParent.genetic_code_up == tAssetSide.genetic_code_down)
			{
				return true;
			}
			break;
		case GeneDirection.Down:
			if (tLocusAmplifier || tForcedSynergySide)
			{
				return true;
			}
			if (tAssetParent.genetic_code_down == tAssetSide.genetic_code_up)
			{
				return true;
			}
			break;
		case GeneDirection.Left:
			if (tLocusAmplifier || tForcedSynergySide)
			{
				return true;
			}
			if (tAssetParent.genetic_code_left == tAssetSide.genetic_code_right)
			{
				return true;
			}
			break;
		case GeneDirection.Right:
			if (tLocusAmplifier || tForcedSynergySide)
			{
				return true;
			}
			if (tAssetParent.genetic_code_right == tAssetSide.genetic_code_left)
			{
				return true;
			}
			break;
		}
		return false;
	}

	public bool isConnectionDeniedUp(int pFromX, int pFromY)
	{
		if (hasBoundAt(pFromX, pFromY - 1))
		{
			return true;
		}
		if (isForcedSynergyUp(pFromX, pFromY) && isForcedSynergyAt(pFromX, pFromY))
		{
			return true;
		}
		return false;
	}

	public bool isConnectionDeniedDown(int pFromX, int pFromY)
	{
		if (hasBoundAt(pFromX, pFromY + 1))
		{
			return true;
		}
		if (isForcedSynergyDown(pFromX, pFromY) && isForcedSynergyAt(pFromX, pFromY))
		{
			return true;
		}
		return false;
	}

	public bool isConnectionDeniedLeft(int pFromX, int pFromY)
	{
		if (hasBoundAt(pFromX - 1, pFromY))
		{
			return true;
		}
		if (isForcedSynergyLeft(pFromX, pFromY) && isForcedSynergyAt(pFromX, pFromY))
		{
			return true;
		}
		return false;
	}

	public bool isConnectionDeniedRight(int pFromX, int pFromY)
	{
		if (hasBoundAt(pFromX + 1, pFromY))
		{
			return true;
		}
		if (isForcedSynergyRight(pFromX, pFromY) && isForcedSynergyAt(pFromX, pFromY))
		{
			return true;
		}
		return false;
	}

	public bool hasBoundAt(int pX, int pY)
	{
		if (!isCoordinatesValid(pX, pY))
		{
			return true;
		}
		if (isVoidLocusAt(pX, pY))
		{
			return true;
		}
		return false;
	}

	public bool hasBoundLeft(int pX, int pY)
	{
		return hasBoundAt(pX - 1, pY);
	}

	public bool hasBoundRight(int pX, int pY)
	{
		return hasBoundAt(pX + 1, pY);
	}

	public bool hasBoundUp(int pX, int pY)
	{
		return hasBoundAt(pX, pY - 1);
	}

	public bool hasBoundDown(int pX, int pY)
	{
		return hasBoundAt(pX, pY + 1);
	}

	public void fillEmptyLoci()
	{
		for (int i = 0; i < genes.Count; i++)
		{
			GeneAsset tGeneAsset = genes[i];
			if (!isSpecialLocus(i) && tGeneAsset.is_empty)
			{
				GeneAsset tNewGene = AssetManager.gene_library.getRandomSimpleGene();
				setGene(tNewGene, i);
			}
		}
	}

	public bool isNextToBad(int pLocusIndex)
	{
		var (tX, tY) = getXYFromIndex(pLocusIndex);
		return isNextToBad(tX, tY);
	}

	public bool isNextToBad(int pX, int pY)
	{
		(int, int)[] dIRECTIONS = DIRECTIONS;
		for (int i = 0; i < dIRECTIONS.Length; i++)
		{
			var (tX, tY) = dIRECTIONS[i];
			if (isBadAt(pX + tX, pY + tY))
			{
				return true;
			}
		}
		return false;
	}

	public bool hasGenesAround(int pIndex)
	{
		var (tX, tY) = getXYFromIndex(pIndex);
		return hasGenesAround(tX, tY);
	}

	public bool hasGenesAround(int pX, int pY)
	{
		(int, int)[] dIRECTIONS = DIRECTIONS;
		for (int i = 0; i < dIRECTIONS.Length; i++)
		{
			(int, int) tuple = dIRECTIONS[i];
			int tX = tuple.Item1;
			int tY = tuple.Item2;
			int tCoordX = pX + tX;
			int tCoordY = pY + tY;
			if (isAmplifierLocusAt(tCoordX, tCoordY))
			{
				return true;
			}
			GeneAsset tGeneAsset = getGeneAt(tCoordX, tCoordY);
			if (tGeneAsset != null && !tGeneAsset.is_empty)
			{
				return true;
			}
		}
		return false;
	}

	public bool isNextToBadAmplifier(int pX, int pY)
	{
		(int, int)[] dIRECTIONS = DIRECTIONS;
		for (int i = 0; i < dIRECTIONS.Length; i++)
		{
			var (tX, tY) = dIRECTIONS[i];
			if (hasAmplifierBad(pX + tX, pY + tY))
			{
				return true;
			}
		}
		return false;
	}

	public bool isBadAt(int pX, int pY)
	{
		if (isVoidLocusAt(pX, pY))
		{
			return false;
		}
		GeneAsset tGeneAsset = getGeneAt(pX, pY);
		if (tGeneAsset == null)
		{
			return false;
		}
		if (tGeneAsset.is_bad)
		{
			return true;
		}
		return false;
	}

	public bool hasAmplifierBad(int pX, int pY)
	{
		if (!isLocusAmplifier(pX, pY))
		{
			return false;
		}
		if (isNextToBad(pX, pY))
		{
			return true;
		}
		return false;
	}

	public void shuffleGenes()
	{
		GeneAsset tPlaceholderGene = GeneLibrary.gene_for_generation;
		using ListPool<GeneAsset> tShuffledGenes = new ListPool<GeneAsset>();
		for (int i = 0; i < genes.Count; i++)
		{
			GeneAsset tGeneAsset = genes[i];
			if (!tGeneAsset.is_empty)
			{
				tShuffledGenes.Add(tGeneAsset);
				genes[i] = tPlaceholderGene;
			}
		}
		tShuffledGenes.Shuffle();
		for (int j = 0; j < genes.Count; j++)
		{
			if (tShuffledGenes.Count == 0)
			{
				break;
			}
			if (genes[j].for_generation)
			{
				genes[j] = tShuffledGenes.Pop();
			}
		}
		setDirty();
	}
}
