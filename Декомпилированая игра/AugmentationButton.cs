using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AugmentationButton<TAugmentation> : MonoBehaviour where TAugmentation : BaseAugmentationAsset
{
	[NonSerialized]
	public TAugmentation augmentation_asset;

	internal Image image;

	internal Image locked_bg;

	private IconOutline _outline;

	private Shadow _shadow;

	private bool _tooltip_enabled = true;

	internal Button button;

	internal bool is_editor_button;

	private AugmentationUnlockedAction _on_augmentation_unlocked;

	private AugmentationButtonClickAction _on_button_clicked;

	protected bool created;

	private bool _selected;

	protected virtual string tooltip_type
	{
		get
		{
			throw new NotImplementedException(GetType().Name);
		}
	}

	public bool isSelected()
	{
		return _selected;
	}

	protected virtual void Awake()
	{
		create();
		if (TryGetComponent<DraggableLayoutElement>(out var tDraggableLayoutElement))
		{
			DraggableLayoutElement draggableLayoutElement = tDraggableLayoutElement;
			draggableLayoutElement.start_being_dragged = (Action<DraggableLayoutElement>)Delegate.Combine(draggableLayoutElement.start_being_dragged, new Action<DraggableLayoutElement>(onStartDrag));
		}
	}

	protected virtual void onStartDrag(DraggableLayoutElement pOriginalElement)
	{
		AugmentationButton<TAugmentation> tOriginalButton = pOriginalElement.GetComponent<AugmentationButton<TAugmentation>>();
		load(tOriginalButton.augmentation_asset);
		is_editor_button = tOriginalButton.is_editor_button;
	}

	protected virtual void create()
	{
		if (!created)
		{
			created = true;
			button = GetComponent<Button>();
			image = base.transform.Find("TiltEffect/icon").GetComponent<Image>();
			locked_bg = base.transform.Find("TiltEffect/locked_bg").GetComponent<Image>();
			locked_bg.gameObject.SetActive(value: false);
			initTooltip();
			_outline = base.transform.FindRecursive("outline")?.GetComponent<IconOutline>();
			_shadow = image.GetComponent<Shadow>();
			button.onClick.AddListener(delegate
			{
				_on_button_clicked?.Invoke(base.gameObject);
			});
		}
	}

	public virtual void load(TAugmentation pElement)
	{
		throw new NotImplementedException();
	}

	protected virtual void initTooltip()
	{
		if (TryGetComponent<TipButton>(out var tTipButton))
		{
			tTipButton.setHoverAction(showTooltip);
		}
	}

	protected virtual void Update()
	{
		throw new NotImplementedException();
	}

	protected void loadLegendaryOutline()
	{
		_shadow.enabled = true;
		if (!(_outline == null))
		{
			if (getRarity() == Rarity.R3_Legendary)
			{
				showOutline(RarityLibrary.legendary.color_container);
			}
			else
			{
				_outline.gameObject.SetActive(value: false);
			}
		}
	}

	private void showOutline(ContainerItemColor pContainer)
	{
		if (!(_outline == null))
		{
			_outline.show(pContainer);
			_shadow.enabled = false;
		}
	}

	public void showTooltip()
	{
		if (_tooltip_enabled)
		{
			if (!is_editor_button && !augmentation_asset.unlocked_with_achievement && !isElementUnlocked() && !WorldLawLibrary.world_law_cursed_world.isEnabled() && unlockElement())
			{
				startSignal();
				_on_augmentation_unlocked?.Invoke();
			}
			if (!is_editor_button || InputHelpers.mouseSupported || !Tooltip.isShowingFor(this))
			{
				fillTooltipData(augmentation_asset);
			}
			base.transform.localScale = new Vector3(1f, 1f, 1f);
			base.transform.DOKill();
			base.transform.DOScale(0.8f, 0.1f).SetEase(Ease.InBack);
		}
	}

	public void addElementUnlockedAction(AugmentationUnlockedAction pAction)
	{
		_on_augmentation_unlocked = (AugmentationUnlockedAction)Delegate.Combine(_on_augmentation_unlocked, pAction);
	}

	public void removeElementUnlockedAction(AugmentationUnlockedAction pAction)
	{
		_on_augmentation_unlocked = (AugmentationUnlockedAction)Delegate.Remove(_on_augmentation_unlocked, pAction);
	}

	protected virtual void clearActions()
	{
		_on_augmentation_unlocked = null;
		clearClickActions();
	}

	public virtual void updateIconColor(bool pSelected)
	{
		_selected = pSelected;
		if (is_editor_button)
		{
			if (!getElementAsset().isAvailable())
			{
				image.color = Toolbox.color_black;
			}
			else if (pSelected)
			{
				image.color = Toolbox.color_augmentation_selected;
			}
			else
			{
				image.color = Toolbox.color_augmentation_unselected;
			}
		}
	}

	public TAugmentation getElementAsset()
	{
		return augmentation_asset;
	}

	protected bool isElementUnlocked()
	{
		return augmentation_asset.isAvailable();
	}

	protected virtual bool unlockElement()
	{
		throw new NotImplementedException(GetType().Name);
	}

	protected virtual void startSignal()
	{
	}

	protected virtual void fillTooltipData(TAugmentation pElement)
	{
		throw new NotImplementedException(GetType().Name);
	}

	protected virtual TooltipData tooltipDataBuilder()
	{
		throw new NotImplementedException(GetType().Name);
	}

	protected virtual string getElementType()
	{
		throw new NotImplementedException(GetType().Name);
	}

	public virtual string getElementId()
	{
		throw new NotImplementedException(GetType().Name);
	}

	protected virtual Rarity getRarity()
	{
		throw new NotImplementedException(GetType().Name);
	}

	protected virtual void disableTooltip()
	{
		_tooltip_enabled = false;
	}

	public void addClickAction(AugmentationButtonClickAction pAction)
	{
		_on_button_clicked = (AugmentationButtonClickAction)Delegate.Combine(_on_button_clicked, pAction);
	}

	public void removeClickAction(AugmentationButtonClickAction pAction)
	{
		_on_button_clicked = (AugmentationButtonClickAction)Delegate.Remove(_on_button_clicked, pAction);
	}

	private void clearClickActions()
	{
		_on_button_clicked = null;
	}

	private void OnDestroy()
	{
		base.transform.DOKill();
	}
}
