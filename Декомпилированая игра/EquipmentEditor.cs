using System.Collections.Generic;
using UnityEngine;

public class EquipmentEditor : AugmentationsEditor<EquipmentAsset, EquipmentButton, EquipmentEditorButton, ItemGroupAsset, EquipmentGroupElement, IEquipmentWindow, IEquipmentEditor>, IEquipmentEditor, IAugmentationsEditor
{
	[SerializeField]
	protected Sprite sprite_art;

	[SerializeField]
	protected Sprite sprite_art_void;

	protected override List<ItemGroupAsset> augmentation_groups_list => AssetManager.item_groups.list;

	protected override EquipmentAsset edited_marker_augmentation => null;

	protected override List<EquipmentAsset> all_augmentations_list => AssetManager.items.list;

	protected override void onEnableRain()
	{
		rain_editor_state = PlayerConfig.instance.data.equipment_editor_state;
		augmentations_list_link = PlayerConfig.instance.data.equipment_editor;
		validateRainData();
		augmentations_hashset.Clear();
		augmentations_hashset.UnionWith(augmentations_list_link);
		loadEditorSelectedAugmentations();
		rain_state_toggle_action = delegate
		{
			toggleRainState(ref PlayerConfig.instance.data.equipment_editor_state);
		};
		rain_state_switcher.toggleState(rain_editor_state == RainState.Remove);
	}

	protected override void OnEnable()
	{
		if (!rain_editor)
		{
			Actor tActor = getCurrentActor();
			if (!tActor.canEditEquipment())
			{
				return;
			}
			foreach (EquipmentEditorButton all_augmentation_button in all_augmentation_buttons)
			{
				EquipmentAsset tAsset = all_augmentation_button.augmentation_button.getElementAsset();
				bool tShowButton = true;
				if (!tActor.asset.canEditItem(tAsset))
				{
					tShowButton = false;
				}
				all_augmentation_button.gameObject.SetActive(tShowButton);
			}
		}
		base.OnEnable();
	}

	protected override void metaAugmentationClick(EquipmentEditorButton pButton)
	{
		if (!isAugmentationAvailable(pButton.augmentation_button))
		{
			return;
		}
		EquipmentButton tButton = pButton.augmentation_button;
		EquipmentAsset tItemAsset = pButton.augmentation_button.getElementAsset();
		if (canChangeSlot(tItemAsset))
		{
			bool num = hasAugmentation(tButton);
			if (!isSlotEmpty(tButton))
			{
				removeAugmentation(tButton);
			}
			if (!num)
			{
				addAugmentation(tButton);
			}
		}
		augmentation_window.checkEquipmentTabIcon();
		base.metaAugmentationClick(pButton);
	}

	protected override void rainAugmentationClick(EquipmentEditorButton pButton)
	{
		if (isAugmentationAvailable(pButton.augmentation_button))
		{
			string tItemId = pButton.augmentation_button.getElementAsset().id;
			if (!augmentations_hashset.Contains(tItemId))
			{
				augmentations_hashset.Add(tItemId);
			}
			else
			{
				augmentations_hashset.Remove(tItemId);
			}
			base.rainAugmentationClick(pButton);
		}
	}

	protected override void showActiveButtons()
	{
		augmentation_window.reloadEquipment();
	}

	protected override ListPool<EquipmentAsset> getOrderedAugmentationsList()
	{
		return new ListPool<EquipmentAsset>(all_augmentations_list);
	}

	protected override void createButton(EquipmentAsset pElement, EquipmentGroupElement pGroup)
	{
		if (pElement.show_in_meta_editor)
		{
			bool tShowButton = true;
			if (!rain_editor && !getCurrentActor().asset.canEditItem(pElement))
			{
				tShowButton = false;
			}
			EquipmentEditorButton tEditorButton = Object.Instantiate(prefab_editor_augmentation, pGroup.augmentation_buttons_transform);
			tEditorButton.augmentation_button.is_editor_button = true;
			tEditorButton.augmentation_button.load(pElement);
			all_augmentation_buttons.Add(tEditorButton);
			pGroup.augmentation_buttons.Add(tEditorButton);
			tEditorButton.augmentation_button.button.onClick.RemoveAllListeners();
			tEditorButton.augmentation_button.button.onClick.AddListener(delegate
			{
				editorButtonClick(tEditorButton);
			});
			tEditorButton.gameObject.SetActive(tShowButton);
		}
	}

	protected override void startSignal()
	{
		AchievementLibrary.equipment_explorer.checkBySignal();
	}

	private bool canChangeSlot(EquipmentAsset pAsset)
	{
		if (!pAsset.can_be_given)
		{
			return false;
		}
		return getSlotFromCurrentActor(pAsset.equipment_type).canChangeSlot();
	}

	private bool isSlotEmpty(EquipmentButton pButton)
	{
		EquipmentAsset tItemAsset = pButton.getElementAsset();
		return getSlotFromCurrentActor(tItemAsset.equipment_type).isEmpty();
	}

	protected override bool hasAugmentation(EquipmentButton pButton)
	{
		EquipmentAsset tItemAsset = pButton.getElementAsset();
		ActorEquipmentSlot tSlot = getSlotFromCurrentActor(tItemAsset.equipment_type);
		if (tSlot.isEmpty())
		{
			return false;
		}
		Item item = tSlot.getItem();
		string tAssetId = pButton.getElementAsset().id;
		if (item.getAsset().id == tAssetId)
		{
			return true;
		}
		return false;
	}

	protected override bool addAugmentation(EquipmentButton pButton)
	{
		Actor tActor = getCurrentActor();
		EquipmentAsset tItemAsset = pButton.getElementAsset();
		Item tItem = World.world.items.generateItem(tItemAsset, tActor.kingdom, World.world.map_stats.player_name, 1, tActor, 0, pByPlayer: true);
		tItem.addMod("divine_rune");
		tActor.equipment.setItem(tItem, tActor);
		return true;
	}

	protected override bool removeAugmentation(EquipmentButton pButton)
	{
		Actor currentActor = getCurrentActor();
		EquipmentType tSlotType = pButton.getElementAsset().equipment_type;
		currentActor.equipment.getSlot(tSlotType).takeAwayItem();
		currentActor.setStatsDirty();
		return true;
	}

	private ActorEquipmentSlot getSlotFromCurrentActor(EquipmentType pType)
	{
		return getCurrentActor().equipment.getSlot(pType);
	}

	private Actor getCurrentActor()
	{
		return SelectedUnit.unit;
	}

	protected override void loadEditorSelectedButton(EquipmentButton pButton, string pAugmentationId)
	{
		base.loadEditorSelectedButton(pButton, pAugmentationId);
		EquipmentAsset tAsset = AssetManager.items.get(pAugmentationId);
		pButton.load(tAsset);
	}

	protected override bool isAugmentationExists(string pId)
	{
		return AssetManager.items.has(pId);
	}

	protected override void toggleRainState(ref RainState pState)
	{
		base.toggleRainState(ref pState);
		art.sprite = ((pState == RainState.Add) ? sprite_art : sprite_art_void);
		if (pState == RainState.Add)
		{
			augmentations_hashset.Clear();
			augmentations_hashset.UnionWith(augmentations_list_link);
			reloadButtons();
		}
	}
}
