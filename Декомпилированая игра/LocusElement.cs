using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LocusElement : ChainElement, IDropHandler, IEventSystemHandler
{
	private Chromosome _chromosome;

	private LocusClickEvent _locus_click_event;

	private Action _chromosome_updated_event;

	private LocusType locus_type;

	public Image sprite_background;

	public Image effect_amplifier;

	public Image effect_locus_amplifier_bad;

	public Sprite sprite_locus_bg_normal;

	public Sprite sprite_locus_bg_synergy;

	public Sprite sprite_locus_bg_bad;

	[SerializeField]
	private LocusDot _dot_left;

	[SerializeField]
	private LocusDot _dot_right;

	[SerializeField]
	private LocusDot _dot_up;

	[SerializeField]
	private LocusDot _dot_down;

	private float _normal_size = 0.8f;

	private float _super_size = 0.8f;

	private int _locus_x;

	private int _locus_y;

	private SpriteAnimation _animation_amplifier;

	private SpriteAnimation _animation_amplifier_bad;

	[SerializeField]
	private GeneButton _gene_button;

	protected override void create()
	{
		base.create();
		is_editor_button = false;
		_animation_amplifier = effect_amplifier.GetComponent<SpriteAnimation>();
		_animation_amplifier_bad = effect_locus_amplifier_bad.GetComponent<SpriteAnimation>();
	}

	protected override void Update()
	{
		base.Update();
		if (isAmplifier())
		{
			if (_animation_amplifier.isActiveAndEnabled)
			{
				_animation_amplifier.update(Time.deltaTime);
			}
			if (_animation_amplifier_bad.isActiveAndEnabled)
			{
				_animation_amplifier_bad.update(Time.deltaTime);
			}
		}
	}

	private void click()
	{
		if (base.gene.can_drop_and_grab)
		{
			_locus_click_event(this);
			checkSprite();
			if (!InputHelpers.mouseSupported)
			{
				GetComponent<TipButton>().hoverAction();
			}
		}
	}

	private void clearLocus()
	{
		_locus_click_event(null);
		checkSprite();
	}

	private void checkSprite()
	{
		bool num = isEmptyLocus();
		bool tIsAmplifier = isAmplifier();
		bool tLocusBgVisible = !num && !tIsAmplifier;
		bool tNextToBad = _chromosome.isNextToBad(_locus_x, _locus_y);
		if (tIsAmplifier)
		{
			if (tNextToBad)
			{
				effect_amplifier.gameObject.SetActive(value: false);
				effect_locus_amplifier_bad.gameObject.SetActive(value: true);
			}
			else
			{
				effect_amplifier.gameObject.SetActive(value: true);
				effect_locus_amplifier_bad.gameObject.SetActive(value: false);
			}
		}
		else
		{
			effect_amplifier.gameObject.SetActive(value: false);
			effect_locus_amplifier_bad.gameObject.SetActive(value: false);
		}
		if (shouldBeBadLocus())
		{
			sprite_background.sprite = sprite_locus_bg_bad;
		}
		else if (shouldBeGoldenLocus())
		{
			sprite_background.sprite = sprite_locus_bg_synergy;
		}
		else
		{
			sprite_background.sprite = sprite_locus_bg_normal;
		}
		sprite_background.gameObject.SetActive(tLocusBgVisible);
		checkChainsColors();
		if (num || tIsAmplifier)
		{
			_gene_button.gameObject.SetActive(value: false);
		}
		else
		{
			_gene_button.gameObject.SetActive(value: true);
			_gene_button.load(base.gene);
			_gene_button.is_editor_button = true;
			_gene_button.locusChild(click, locus_index);
		}
		if (isAmplifier())
		{
			base.transform.localScale = new Vector3(_super_size, _super_size, _super_size);
		}
		else
		{
			base.transform.localScale = new Vector3(_normal_size, _normal_size, _normal_size);
		}
		GetComponent<TipButton>().setDefaultScale(base.transform.localScale);
	}

	private bool shouldBeBadChainSide(int pX, int pY, int pOffsetX, int pOffsetY)
	{
		return shouldBeBadChain(pX, pY, pX + pOffsetX, pY + pOffsetY);
	}

	private bool shouldBeBadChain(int pX, int pY, int pToX, int pToY)
	{
		if (base.gene.is_bad)
		{
			return true;
		}
		GeneAsset tSideAsset = _chromosome.getGeneAt(pToX, pToY);
		if (tSideAsset != null && tSideAsset.is_bad)
		{
			return true;
		}
		if (_chromosome.hasAmplifierBad(pX, pY))
		{
			return true;
		}
		if (_chromosome.hasAmplifierBad(pToX, pToY))
		{
			return true;
		}
		return false;
	}

	private void checkChainsColors()
	{
		int tX = _locus_x;
		int tY = _locus_y;
		Chromosome tChromosome = _chromosome;
		GeneAsset tGeneLeft = tChromosome.getGeneLeft(tX, tY);
		GeneAsset tGeneRight = tChromosome.getGeneRight(tX, tY);
		GeneAsset tGeneUp = tChromosome.getGeneUp(tX, tY);
		GeneAsset tGeneDown = tChromosome.getGeneDown(tX, tY);
		bool tCanConnectLeft = !tChromosome.hasBoundLeft(tX, tY);
		bool tCanConnectRight = !tChromosome.hasBoundRight(tX, tY);
		bool tCanConnectUp = !tChromosome.hasBoundUp(tX, tY);
		bool tCanConnectDown = !tChromosome.hasBoundDown(tX, tY);
		bool tSynergyLeft = tChromosome.hasSynergyConnectionLeft(tX, tY);
		bool tSynergyRight = tChromosome.hasSynergyConnectionRight(tX, tY);
		bool tSynergyUp = tChromosome.hasSynergyConnectionUp(tX, tY);
		bool tSynergyDown = tChromosome.hasSynergyConnectionDown(tX, tY);
		if (!tSynergyLeft)
		{
			hideChain(chain_left);
		}
		else if (shouldBeBadChain(tX, tY, tX - 1, tY))
		{
			showChain(chain_left, pShow: true, base.gene.genetic_code_left, NucleobaseHelper.color_bad);
		}
		else if (tChromosome.isForcedSynergyLeft(tX, tY))
		{
			showChain(chain_left, pShow: true, base.gene.genetic_code_left);
		}
		else
		{
			showChain(chain_left, pShow: true, tGeneLeft.genetic_code_right);
		}
		if (!tSynergyRight)
		{
			hideChain(chain_right);
		}
		else if (shouldBeBadChain(tX, tY, tX + 1, tY))
		{
			showChain(chain_right, pShow: true, base.gene.genetic_code_right, NucleobaseHelper.color_bad);
		}
		else if (tChromosome.isForcedSynergyRight(tX, tY))
		{
			showChain(chain_right, pShow: true, base.gene.genetic_code_right);
		}
		else
		{
			showChain(chain_right, pShow: true, tGeneRight.genetic_code_left);
		}
		if (!tSynergyUp)
		{
			hideChain(chain_up);
		}
		else if (shouldBeBadChain(tX, tY, tX, tY - 1))
		{
			showChain(chain_up, pShow: true, base.gene.genetic_code_up, NucleobaseHelper.color_bad);
		}
		else if (tChromosome.isForcedSynergyUp(tX, tY))
		{
			showChain(chain_up, pShow: true, base.gene.genetic_code_up);
		}
		else
		{
			showChain(chain_up, pShow: true, tGeneUp.genetic_code_down);
		}
		if (!tSynergyDown)
		{
			hideChain(chain_down);
		}
		else if (shouldBeBadChain(tX, tY, tX, tY + 1))
		{
			showChain(chain_down, pShow: true, base.gene.genetic_code_down, NucleobaseHelper.color_bad);
		}
		else if (tChromosome.isForcedSynergyDown(tX, tY))
		{
			showChain(chain_down, pShow: true, base.gene.genetic_code_down);
		}
		else
		{
			showChain(chain_down, pShow: true, tGeneDown.genetic_code_up);
		}
		showDot(_dot_left, tCanConnectLeft && !tSynergyLeft, base.gene.genetic_code_left);
		showDot(_dot_right, tCanConnectRight && !tSynergyRight, base.gene.genetic_code_right);
		showDot(_dot_up, tCanConnectUp && !tSynergyUp, base.gene.genetic_code_up);
		showDot(_dot_down, tCanConnectDown && !tSynergyDown, base.gene.genetic_code_down);
	}

	public override void load(GeneAsset pAsset)
	{
		throw new NotImplementedException("Use show instead");
	}

	internal override void load(string pElementID)
	{
		throw new NotImplementedException("Use show instead");
	}

	public void show(int pLocusIndex, Chromosome pChromosome, GeneAsset pGene, LocusType pLocusType, LocusClickEvent pLocusClickEvent)
	{
		base.load(pGene);
		clearActions();
		_chromosome = pChromosome;
		locus_index = pLocusIndex;
		(int, int) xYFromIndex = _chromosome.getXYFromIndex(pLocusIndex);
		int tX = xYFromIndex.Item1;
		int tY = xYFromIndex.Item2;
		_locus_x = tX;
		_locus_y = tY;
		_locus_click_event = pLocusClickEvent;
		locus_type = pLocusType;
		base.gameObject.name = "Locus " + base.gene.id;
		colorChains();
		checkSprite();
	}

	protected override void clearActions()
	{
		base.clearActions();
		_chromosome_updated_event = null;
	}

	public bool shouldBeBadLocus()
	{
		bool is_bad = base.gene.is_bad;
		bool tNextToBad = _chromosome.isNextToBad(_locus_x, _locus_y);
		return is_bad || tNextToBad;
	}

	public bool shouldBeGoldenLocus()
	{
		if (isAmplifier())
		{
			return true;
		}
		if (base.gene.synergy_sides_always)
		{
			return true;
		}
		if (_chromosome.hasFullSynergy(locus_index))
		{
			return true;
		}
		return false;
	}

	public bool isAmplifier()
	{
		return locus_type == LocusType.Amplifier;
	}

	public bool isAmplifierBad()
	{
		return _chromosome.hasAmplifierBad(_locus_x, _locus_y);
	}

	public bool isEmptyLocus()
	{
		return locus_type == LocusType.Empty;
	}

	protected override void fillTooltipData(GeneAsset pElement)
	{
		Tooltip.show(this, "gene", tooltipDataBuilder());
	}

	protected override TooltipData tooltipDataBuilder()
	{
		return new TooltipData
		{
			gene = base.gene,
			locus = this,
			chromosome = _chromosome
		};
	}

	public bool canAddGene()
	{
		return _chromosome.canAddToLocus(locus_index);
	}

	public bool isSpecialLocus()
	{
		return _chromosome.isSpecialLocus(locus_index);
	}

	public void OnDrop(PointerEventData pEventData)
	{
		if (pEventData.pointerDrag == null || isAmplifier())
		{
			return;
		}
		if (!Config.hasPremium)
		{
			ScrollWindow.showWindow("premium_menu");
			return;
		}
		GeneButton tGeneButton = pEventData.pointerDrag.GetComponent<GeneButton>();
		if (tGeneButton == null)
		{
			return;
		}
		GeneAsset tGeneAsset = tGeneButton.getElementAsset();
		if (tGeneAsset.can_drop_and_grab)
		{
			if (tGeneButton.locus_index > -1)
			{
				GeneAsset tOldAsset = _chromosome.getGene(locus_index);
				_chromosome.setGene(tOldAsset, tGeneButton.locus_index);
			}
			GeneAsset tOldGene = getGeneAsset();
			_chromosome.setGene(tGeneAsset, locus_index);
			_chromosome_updated_event();
			SelectedMetas.selected_subspecies.eventGMO();
			if (tGeneAsset != tOldGene)
			{
				AchievementLibrary.engineered_evolution.check();
			}
			fillTooltipData(base.gene);
		}
	}

	public void addChromosomeUpdatedEvent(Action pChromosomeUpdatedEvent)
	{
		_chromosome_updated_event = pChromosomeUpdatedEvent;
	}

	protected void showDot(LocusDot pChainDot, bool pShow, char pGeneticCode)
	{
		pChainDot.gameObject.SetActive(pShow);
		if (pShow)
		{
			pChainDot.colorDot(pGeneticCode);
		}
	}

	protected override void startSignal()
	{
		AchievementLibrary.genes_explorer.checkBySignal();
	}

	protected override bool unlockElement()
	{
		bool result = base.unlockElement();
		isElementUnlocked();
		return result;
	}
}
