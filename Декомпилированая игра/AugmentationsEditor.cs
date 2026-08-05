using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AugmentationsEditor<TAugmentation, TAugmentationButton, TAugmentationEditorButton, TAugmentationGroupAsset, TAugmentationGroup, TAugmentationWindow, TEditorInterface> : BaseAugmentationsEditor where TAugmentation : BaseAugmentationAsset where TAugmentationButton : AugmentationButton<TAugmentation> where TAugmentationEditorButton : AugmentationEditorButton<TAugmentationButton, TAugmentation> where TAugmentationGroupAsset : BaseCategoryAsset where TAugmentationGroup : AugmentationCategory<TAugmentation, TAugmentationButton, TAugmentationEditorButton> where TAugmentationWindow : IAugmentationsWindow<TEditorInterface> where TEditorInterface : IAugmentationsEditor
{
	private const float FOCUS_SCROLL_OFFSET_TOP = -5f;

	private const float FOCUS_SCROLL_OFFSET_BOTTOM = 1f;

	public const float FOCUS_SCROLL_DURATION = 0.3f;

	[SerializeField]
	protected Image art;

	public TAugmentationButton prefab_augmentation;

	public TAugmentationEditorButton prefab_editor_augmentation;

	public TAugmentationGroup prefab_augmentation_group;

	protected readonly Dictionary<string, TAugmentationGroup> dict_groups = new Dictionary<string, TAugmentationGroup>();

	protected readonly List<TAugmentationEditorButton> all_augmentation_buttons = new List<TAugmentationEditorButton>();

	protected TAugmentationWindow augmentation_window;

	protected ObjectPoolGenericMono<TAugmentationButton> selected_editor_buttons;

	[SerializeField]
	private WindowMetaTab _editor_tab;

	protected virtual List<TAugmentationGroupAsset> augmentation_groups_list
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	protected virtual List<TAugmentation> all_augmentations_list
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	protected virtual TAugmentation edited_marker_augmentation => null;

	protected override void create()
	{
		base.create();
		augmentation_window = GetComponentInParent<TAugmentationWindow>();
		if (rain_editor)
		{
			selected_editor_buttons = new ObjectPoolGenericMono<TAugmentationButton>(prefab_augmentation, selected_editor_augmentations_grid.transform);
		}
	}

	protected override void OnEnable()
	{
		if (rain_editor)
		{
			onEnableRain();
		}
		base.OnEnable();
	}

	protected virtual ListPool<TAugmentation> getOrderedAugmentationsList()
	{
		ListPool<TAugmentation> listPool = new ListPool<TAugmentation>(all_augmentations_list);
		listPool.Sort(delegate(TAugmentation pT1, TAugmentation pT2)
		{
			int num = pT2.priority.CompareTo(pT1.priority);
			if (num == 0)
			{
				num = StringComparer.Ordinal.Compare(pT1.id, pT2.id);
			}
			return num;
		});
		return listPool;
	}

	public override void reloadButtons()
	{
		base.reloadButtons();
		int tCounterSelected = 0;
		int tCounterUnlocked = 0;
		int tTotal = 0;
		foreach (TAugmentationEditorButton tB in all_augmentation_buttons)
		{
			bool num = isAugmentationAvailable(tB.augmentation_button);
			TAugmentation tAsset = tB.augmentation_button.getElementAsset();
			tTotal++;
			if (num)
			{
				tCounterUnlocked++;
			}
			tB.selected_icon.gameObject.SetActive(value: false);
			if (num)
			{
				tB.augmentation_button.image.color = Toolbox.color_augmentation_unselected;
			}
			bool can_be_given = tAsset.can_be_given;
			bool tSelected = false;
			if (!can_be_given)
			{
				bool tHas = !rain_editor && hasAugmentation(tB.augmentation_button);
				tB.selected_icon.gameObject.SetActive(tHas);
				tB.selected_icon.color = Toolbox.color_log_warning;
				if (tHas)
				{
					tCounterSelected++;
					tSelected = true;
				}
			}
			else if (rain_editor && augmentations_hashset.Contains(tB.augmentation_button.getElementId()))
			{
				Color tColor = ((rain_editor_state != RainState.Add) ? ColorStyleLibrary.m.getSelectorRemoveColor() : ColorStyleLibrary.m.getSelectorColor());
				tB.selected_icon.gameObject.SetActive(value: true);
				tB.selected_icon.color = tColor;
				tSelected = true;
			}
			else if (!rain_editor && hasAugmentation(tB.augmentation_button))
			{
				tB.selected_icon.gameObject.SetActive(value: true);
				tB.selected_icon.color = ColorStyleLibrary.m.getSelectorColor();
				tSelected = true;
				tCounterSelected++;
			}
			tB.augmentation_button.updateIconColor(tSelected);
		}
		foreach (TAugmentationGroup tElement in dict_groups.Values)
		{
			if (tElement.asset.show_counter)
			{
				tElement.updateCounter();
			}
			else
			{
				tElement.hideCounter();
			}
		}
		if (rain_editor)
		{
			text_counter_augmentations.text = tCounterUnlocked + "/" + tTotal;
		}
		else
		{
			text_counter_augmentations.text = tCounterSelected + "/" + tTotal;
		}
		startSignal();
	}

	protected override void groupsBuilder()
	{
		using ListPool<TAugmentation> tOrderedElementList = getOrderedAugmentationsList();
		foreach (TAugmentationGroupAsset tGroupAsset in augmentation_groups_list)
		{
			TAugmentationGroup tNewTransform = UnityEngine.Object.Instantiate(prefab_augmentation_group, augmentation_groups_parent);
			tNewTransform.asset = tGroupAsset;
			tNewTransform.clearDebug();
			dict_groups.Add(tGroupAsset.id, tNewTransform);
			tNewTransform.title.GetComponent<LocalizedText>().setKeyAndUpdate(tGroupAsset.getLocaleID());
			tNewTransform.title.color = tGroupAsset.getColor();
		}
		foreach (ref TAugmentation item in tOrderedElementList)
		{
			TAugmentation tElement = item;
			TAugmentationGroup tTransformParent = dict_groups[tElement.group_id];
			createButton(tElement, tTransformParent);
		}
	}

	protected override void checkEnabledGroups()
	{
		foreach (TAugmentationGroup value in dict_groups.Values)
		{
			bool tGroupState = value.countActiveButtons() > 0;
			value.gameObject.SetActive(tGroupState);
		}
	}

	protected void editorButtonClick(TAugmentationEditorButton pButton)
	{
		if (!InputHelpers.mouseSupported && !Tooltip.isShowingFor(pButton.augmentation_button))
		{
			return;
		}
		if (!Config.hasPremium)
		{
			ScrollWindow.showWindow("premium_menu");
		}
		else if (pButton.augmentation_button.getElementAsset().can_be_given)
		{
			if (rain_editor)
			{
				rainAugmentationClick(pButton);
			}
			else
			{
				metaAugmentationClick(pButton);
			}
			reloadButtons();
		}
	}

	protected virtual void metaAugmentationClick(TAugmentationEditorButton pButton)
	{
		showActiveButtons();
		refreshAugmentationWindow();
	}

	protected virtual void rainAugmentationClick(TAugmentationEditorButton pButton)
	{
		saveRainValues();
		loadEditorSelectedAugmentations();
	}

	protected virtual void validateRainData()
	{
		augmentations_list_link.RemoveAll(delegate(string tId)
		{
			TAugmentation val = all_augmentations_list.Find((TAugmentation tAugmentation) => tAugmentation.id == tId);
			if (val == null)
			{
				return true;
			}
			return !val.isAvailable();
		});
	}

	protected virtual void refreshAugmentationWindow()
	{
		augmentation_window.updateStats();
		augmentation_window.reloadBanner();
	}

	protected void saveRainValues()
	{
		augmentations_list_link.Clear();
		foreach (string tElementID in augmentations_hashset)
		{
			augmentations_list_link.Add(tElementID);
		}
		PlayerConfig.saveData();
	}

	protected virtual void loadEditorSelectedAugmentations()
	{
		selected_editor_buttons.clear();
		foreach (string tAugmentationId in augmentations_hashset)
		{
			if (isAugmentationExists(tAugmentationId))
			{
				TAugmentationButton tButton = selected_editor_buttons.getNext();
				loadEditorSelectedButton(tButton, tAugmentationId);
			}
		}
	}

	public void scrollToGroupStarter(GameObject pButton)
	{
		scrollToGroupStarter(pButton, pIgnoreTooltipCheck: false);
	}

	public virtual void scrollToGroupStarter(GameObject pButton, bool pIgnoreTooltipCheck)
	{
		if (!pIgnoreTooltipCheck && !InputHelpers.mouseSupported && !Tooltip.isShowingFor(pButton.GetComponent<TAugmentationButton>()))
		{
			return;
		}
		bool tDelay = false;
		if (!base.gameObject.activeInHierarchy)
		{
			if (!(_editor_tab != null))
			{
				return;
			}
			_editor_tab.container.showTab(_editor_tab);
			tDelay = true;
		}
		StartCoroutine(scrollToGroupStarterRoutine(pButton, tDelay));
	}

	private IEnumerator scrollToGroupStarterRoutine(GameObject pButton, bool pWithDelay)
	{
		if (pWithDelay)
		{
			yield return new WaitForSeconds(Config.getScrollToGroupDelay());
		}
		scrollToGroup(pButton);
	}

	private void scrollToGroup(GameObject pButton, float pDuration = 0.3f)
	{
		TAugmentationGroup tTraitGroup = null;
		foreach (TAugmentationGroup tGroup in dict_groups.Values)
		{
			TAugmentationButton tTraitButton = pButton.GetComponent<TAugmentationButton>();
			if (tGroup.hasAugmentation(tTraitButton.getElementAsset()))
			{
				tTraitGroup = tGroup;
				break;
			}
		}
		if (tTraitGroup == null)
		{
			return;
		}
		RectTransform obj = pButton.GetComponentInParent<HeaderContainer>().transform as RectTransform;
		RectTransform tContentRect = base.transform.parent.GetComponent<RectTransform>();
		RectTransform component = tContentRect.parent.GetComponent<RectTransform>();
		RectTransform tEditorRect = base.transform as RectTransform;
		RectTransform tCategoryRect = tTraitGroup.GetComponent<RectTransform>();
		float tViewportHeight = component.rect.height;
		float tHeaderHeight = obj.rect.height;
		float tContentHeight = tContentRect.rect.height;
		float tEditorHeight = tEditorRect.rect.height;
		float tCategoryHeight = tCategoryRect.rect.height;
		float tEditorOffset = Mathf.Abs(tEditorRect.anchoredPosition.y) - tEditorHeight * (1f - tEditorRect.pivot.y) - tHeaderHeight;
		float tCategoryUpperY = Mathf.Abs(tCategoryRect.anchoredPosition.y) - tCategoryHeight * (1f - tCategoryRect.pivot.y) + tEditorOffset;
		float tCategoryLowerY = tCategoryUpperY + tCategoryHeight;
		bool tIsAbove = tCategoryUpperY < tContentRect.localPosition.y;
		bool tIsBelow = tCategoryLowerY > tContentRect.localPosition.y + tViewportHeight - tHeaderHeight;
		if (tIsAbove || tIsBelow)
		{
			float tScrollTo;
			if (tIsAbove)
			{
				tScrollTo = tCategoryUpperY;
				tScrollTo -= -5f;
			}
			else
			{
				tScrollTo = tCategoryLowerY - tViewportHeight + tHeaderHeight;
				tScrollTo += 1f;
			}
			tScrollTo = Mathf.Clamp(tScrollTo, 0f, tContentHeight - tViewportHeight);
			tContentRect.DOLocalMoveY(tScrollTo, pDuration);
		}
	}

	protected virtual bool isAugmentationExists(string pId)
	{
		throw new NotImplementedException();
	}

	protected virtual void loadEditorSelectedButton(TAugmentationButton pButton, string pAugmentationId)
	{
		pButton.removeClickAction(scrollToGroupStarter);
		pButton.addClickAction(scrollToGroupStarter);
	}

	protected virtual void createButton(TAugmentation pElement, TAugmentationGroup pGroup)
	{
		throw new NotImplementedException();
	}

	protected virtual bool hasAugmentation(TAugmentationButton pButton)
	{
		throw new NotImplementedException();
	}

	protected virtual bool addAugmentation(TAugmentationButton pButton)
	{
		throw new NotImplementedException();
	}

	protected virtual bool removeAugmentation(TAugmentationButton pButton)
	{
		throw new NotImplementedException();
	}

	public WindowMetaTab getEditorTab()
	{
		return _editor_tab;
	}

	protected bool isAugmentationAvailable(TAugmentationButton pButton)
	{
		return pButton.getElementAsset().isAvailable();
	}
}
