using System;
using UnityEngine;
using UnityEngine.Events;

public class GeneButton : ChainElement
{
	[SerializeField]
	private GameObject _petri_bg;

	private GeneAssetClickEvent _gene_asset_click_event;

	protected override void create()
	{
		base.create();
		button.onClick.AddListener(click);
	}

	private void click()
	{
		_gene_asset_click_event?.Invoke(base.gene);
		if (!InputHelpers.mouseSupported)
		{
			GetComponent<TipButton>().hoverAction();
		}
	}

	protected override void onStartDrag(DraggableLayoutElement pOriginalElement)
	{
		base.onStartDrag(pOriginalElement);
		_petri_bg.SetActive(value: false);
		colorChains();
		bool tShowLocked = !augmentation_asset.isUnlocked();
		locked_bg.gameObject.SetActive(tShowLocked);
	}

	internal void locusChild(UnityAction pAction, int pLocusIndex)
	{
		hideChains();
		button.onClick.RemoveListener(click);
		button.onClick.RemoveListener(pAction);
		button.onClick.AddListener(pAction);
		locus_index = pLocusIndex;
		disableTooltip();
	}

	protected override void fillTooltipData(GeneAsset pElement)
	{
		Tooltip.show(this, "gene", tooltipDataBuilder());
	}

	protected override TooltipData tooltipDataBuilder()
	{
		return new TooltipData
		{
			gene = base.gene
		};
	}

	public void addGeneClickCallback(GeneAssetClickEvent pAction)
	{
		_gene_asset_click_event = (GeneAssetClickEvent)Delegate.Combine(_gene_asset_click_event, pAction);
	}

	public void removeGeneClickCallback(GeneAssetClickEvent pAction)
	{
		_gene_asset_click_event = (GeneAssetClickEvent)Delegate.Remove(_gene_asset_click_event, pAction);
	}
}
