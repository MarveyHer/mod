using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneEditor : MonoBehaviour
{
	[SerializeField]
	private Text _text_unlocked_genes;

	[SerializeField]
	private Transform _transform_chromosomes;

	[SerializeField]
	private Transform _transform_loci;

	[SerializeField]
	private Transform _transform_gene_selector;

	[SerializeField]
	private ChromosomeElement _prefab_chromosome_element;

	[SerializeField]
	private LocusElement _prefab_locus_element;

	[SerializeField]
	private GeneButton _prefab_gene_button;

	private bool _initialized;

	private Dictionary<GeneAsset, GeneButton> _dictionary_gene_buttons = new Dictionary<GeneAsset, GeneButton>();

	private ObjectPoolGenericMono<ChromosomeElement> _pool_elements_chromosomes;

	private ObjectPoolGenericMono<LocusElement> _pool_elements_loci;

	private LocusElement _selected_locus;

	private Chromosome _selected_chromosome;

	public Image selection_locus;

	public Image selection_gene_asset;

	public Text genome_counter_text;

	private SubspeciesWindow _window_subspecies;

	private Subspecies _meta_object => SelectedMetas.selected_subspecies;

	internal void load()
	{
		init();
		clear();
		loadChromosomes();
		reloadButtons();
		recolorGenePoolButtons();
	}

	private void init()
	{
		if (!_initialized)
		{
			_initialized = true;
			_window_subspecies = GetComponentInParent<SubspeciesWindow>();
			_pool_elements_chromosomes = new ObjectPoolGenericMono<ChromosomeElement>(_prefab_chromosome_element, _transform_chromosomes);
			_pool_elements_loci = new ObjectPoolGenericMono<LocusElement>(_prefab_locus_element, _transform_loci);
			loadGeneButtons();
		}
	}

	private void clear()
	{
		_pool_elements_chromosomes.clear();
		_pool_elements_loci.clear();
		_selected_chromosome = null;
		_selected_locus = null;
	}

	private void OnEnable()
	{
		load();
	}

	private void OnDisable()
	{
		clear();
	}

	public void debugRandomizeGenes()
	{
		_meta_object.addDNAMutationToSeed();
		_meta_object.generateNucleus();
		_meta_object.genesChangedEvent();
		_meta_object.eventGMO();
		load();
	}

	public void debugShuffleGenes()
	{
		_meta_object.unstableGenomeEvent();
		load();
	}

	private void loadChromosomes(bool pSelectFirstChromosome = true)
	{
		foreach (Chromosome tChromosome in _meta_object.nucleus.chromosomes)
		{
			_pool_elements_chromosomes.getNext().show(tChromosome, clickChromosome);
		}
		if (pSelectFirstChromosome && _meta_object.nucleus.chromosomes.Count > 0)
		{
			clickChromosome(_meta_object.nucleus.chromosomes[0]);
		}
	}

	private void recolorGenePoolButtons()
	{
		foreach (GeneButton value in _dictionary_gene_buttons.Values)
		{
			value.colorChains();
		}
	}

	private void loadGeneButtons()
	{
		foreach (GeneAsset tAsset in AssetManager.gene_library.list)
		{
			if (!tAsset.is_empty)
			{
				GeneButton tGeneButton = Object.Instantiate(_prefab_gene_button, _transform_gene_selector);
				_dictionary_gene_buttons.Add(tAsset, tGeneButton);
				tGeneButton.load(tAsset);
				tGeneButton.is_editor_button = true;
				tGeneButton.addElementUnlockedAction(reloadButtons);
				tGeneButton.addGeneClickCallback(clickGeneAssetAction);
				tGeneButton.GetComponent<DraggableLayoutElement>().enabled = tAsset.isAvailable();
			}
		}
	}

	public void clickChromosome(Chromosome pChromosome)
	{
		foreach (ChromosomeElement tElement in _pool_elements_chromosomes.getListTotal())
		{
			if (tElement.gameObject.activeSelf)
			{
				if (tElement.chromosome == pChromosome)
				{
					tElement.image.color = Color.white;
				}
				else
				{
					tElement.image.color = Color.gray;
				}
			}
		}
		_selected_chromosome = pChromosome;
		showGenes(pChromosome);
		selectFirstNormalLocus();
	}

	private void selectFirstNormalLocus()
	{
		foreach (LocusElement tElement in _pool_elements_loci.getListTotal())
		{
			if (!tElement.isSpecialLocus())
			{
				selectLocus(tElement);
				break;
			}
		}
	}

	internal void selectLocus(LocusElement pElement)
	{
		_selected_locus = pElement;
	}

	private void clickGeneAssetAction(GeneAsset pGeneAsset)
	{
		if (!(_selected_locus == null) && pGeneAsset.isAvailable())
		{
			if (pGeneAsset != _selected_locus.getGeneAsset())
			{
				AchievementLibrary.engineered_evolution.check();
			}
			if (!Config.hasPremium)
			{
				ScrollWindow.showWindow("premium_menu");
				return;
			}
			_selected_chromosome.setGene(pGeneAsset, _selected_locus.locus_index);
			chromosomeUpdatedEvent();
		}
	}

	private void chromosomeUpdatedEvent()
	{
		_selected_chromosome.setDirty();
		_selected_chromosome.recalculate();
		_meta_object.genesChangedEvent();
		_meta_object.eventGMO();
		showGenes(_selected_chromosome);
		AchievementLibrary.simple_stupid_genetics.check();
		AchievementLibrary.fast_living.check();
		AchievementLibrary.long_living.check();
		AchievementLibrary.master_weaver.check();
		_pool_elements_chromosomes.clear();
		loadChromosomes(pSelectFirstChromosome: false);
	}

	public void showGenes(Chromosome pChromosome)
	{
		_pool_elements_loci.clear();
		for (int i = 0; i < pChromosome.genes.Count; i++)
		{
			GeneAsset tGene = pChromosome.genes[i];
			LocusElement next = _pool_elements_loci.getNext();
			next.show(i, pChromosome, tGene, pChromosome.getLocusType(i), selectLocus);
			next.addElementUnlockedAction(reloadButtons);
			next.addChromosomeUpdatedEvent(chromosomeUpdatedEvent);
		}
		_window_subspecies.updateStats();
	}

	private void updateTextGenome()
	{
		int tCurrent = _selected_chromosome.countNonEmpty();
		int tSize = _selected_chromosome.getAsset().amount_loci;
		genome_counter_text.text = tCurrent + " / " + tSize;
	}

	private void Update()
	{
		if (_meta_object == null || _selected_chromosome == null)
		{
			return;
		}
		selection_gene_asset.gameObject.SetActive(_selected_locus != null);
		selection_locus.gameObject.SetActive(_selected_locus != null);
		if (_selected_locus != null)
		{
			selection_locus.gameObject.transform.position = _selected_locus.transform.position;
			GeneButton tCurrentGeneButton = getCurrentGeneAssetButton();
			selection_gene_asset.gameObject.transform.position = tCurrentGeneButton.transform.position;
			if (!Config.isDraggingItem())
			{
				_ = tCurrentGeneButton != null;
			}
		}
	}

	private GeneButton getCurrentGeneAssetButton()
	{
		GeneAsset tAsset = _selected_locus.getGeneAsset();
		if (tAsset == null)
		{
			return null;
		}
		if (_dictionary_gene_buttons.ContainsKey(tAsset))
		{
			return _dictionary_gene_buttons[tAsset];
		}
		return null;
	}

	private void reloadButtons()
	{
		int tCounterUnlocked = 0;
		int tTotal = 0;
		foreach (GeneButton tB in _dictionary_gene_buttons.Values)
		{
			bool tUnlocked = tB.getElementAsset().isAvailable();
			tTotal++;
			if (tUnlocked)
			{
				tCounterUnlocked++;
				tB.image.color = Toolbox.color_white;
			}
			else
			{
				tB.image.color = Toolbox.color_black;
			}
			tB.GetComponent<DraggableLayoutElement>().enabled = tUnlocked;
		}
		_text_unlocked_genes.text = tCounterUnlocked + "/" + tTotal;
		AchievementLibrary.genes_explorer.checkBySignal();
	}

	protected virtual bool hasGene(GeneAsset pTrait)
	{
		return _selected_chromosome.hasGene(pTrait);
	}
}
