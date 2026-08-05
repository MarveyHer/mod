using System;
using UnityEngine;
using UnityEngine.UI;

public class ChromosomeElement : MonoBehaviour
{
	private static readonly Color color_synergy_gold = Toolbox.makeColor("#FFF841");

	private static readonly Color color_normal_blue = Toolbox.makeColor("#00B0FF");

	internal Chromosome chromosome;

	private ChromosomeClickEvent _click_event;

	public Image image;

	private void Start()
	{
		setupTooltip();
		GetComponent<Button>().onClick.AddListener(clickChromosome);
		if (TryGetComponent<DraggableLayoutElement>(out var tDraggableLayoutElement))
		{
			DraggableLayoutElement draggableLayoutElement = tDraggableLayoutElement;
			draggableLayoutElement.start_being_dragged = (Action<DraggableLayoutElement>)Delegate.Combine(draggableLayoutElement.start_being_dragged, new Action<DraggableLayoutElement>(onStartDrag));
		}
	}

	protected virtual void onStartDrag(DraggableLayoutElement pOriginalElement)
	{
		ChromosomeElement tOriginalButton = pOriginalElement.GetComponent<ChromosomeElement>();
		show(tOriginalButton.chromosome, null);
	}

	private void clickChromosome()
	{
		_click_event?.Invoke(chromosome);
	}

	public void show(Chromosome pChromosome, ChromosomeClickEvent pClickEvent)
	{
		chromosome = pChromosome;
		_click_event = pClickEvent;
		if (pChromosome.isAllLociSynergy())
		{
			image.sprite = chromosome.getSpriteGolden();
		}
		else
		{
			image.sprite = chromosome.getSpriteNormal();
		}
	}

	protected virtual void setupTooltip()
	{
		if (TryGetComponent<TipButton>(out var tTipButton))
		{
			tTipButton.setHoverAction(tooltipAction);
		}
	}

	protected void tooltipAction()
	{
		Tooltip.show(this, "chromosome", new TooltipData
		{
			chromosome = chromosome
		});
	}
}
