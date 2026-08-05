using System;
using System.Collections.Generic;
using System.Linq;
using NeoModLoader.api;
using NeoModLoader.General;
using NeoModLoader.General.UI.Prefabs;
using NeoModLoader.ui.prefabs;
using NeoModLoader.utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NeoModLoader.ui;

internal class NewModListWindow : AbstractWideWindow<NewModListWindow>
{
	private enum DisplayType
	{
		Mod,
		Resource
	}

	private readonly Dictionary<ModDeclare, ModInfoPanel> ModInfoPanels = new Dictionary<ModDeclare, ModInfoPanel>();

	private DisplayType CurrentDisplayType;

	private ModDeclare CurrentSelected;

	private ObjectPoolGenericMono<ModListItem> ListItemPool;

	private RectTransform ListPart;

	private List<ModDeclare> ListToShow;

	private SimpleButton ModCommunityButton;

	private SimpleButton ModConfigureButton;

	private RectTransform ModInfoPart;

	private SimpleButton OpenModFolderButton;

	private SimpleButton ReloadModButton;

	private SimpleButton ToggleModButton;

	private SimpleButton UploadModButton;

	protected override void Init()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Expected O, but got Unknown
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Expected O, but got Unknown
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Expected O, but got Unknown
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Expected O, but got Unknown
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Expected O, but got Unknown
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Expected O, but got Unknown
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_0632: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Expected O, but got Unknown
		//IL_0694: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Expected O, but got Unknown
		//IL_0710: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Expected O, but got Unknown
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Expected O, but got Unknown
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		//IL_081d: Unknown result type (might be due to invalid IL or missing references)
		//IL_083c: Expected O, but got Unknown
		//IL_0864: Unknown result type (might be due to invalid IL or missing references)
		//IL_0884: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("TypeSelectPart", new Type[2]
		{
			typeof(Image),
			typeof(VerticalLayoutGroup)
		});
		val.transform.SetParent(base.BackgroundTransform);
		val.transform.localPosition = new Vector3(-260f, 0f);
		val.transform.localScale = Vector3.one;
		val.GetComponent<Image>().sprite = InternalResourcesGetter.GetWindowEmptyFrame();
		val.GetComponent<Image>().type = (Type)1;
		val.GetComponent<RectTransform>().sizeDelta = new Vector2(48f, 255f);
		OT.InitializeNoActionVerticalLayoutGroup(val.GetComponent<VerticalLayoutGroup>());
		((LayoutGroup)val.GetComponent<VerticalLayoutGroup>()).padding = new RectOffset(0, 0, 12, 0);
		SimpleButton simpleButton = Object.Instantiate<SimpleButton>(APrefab<SimpleButton>.Prefab, val.transform);
		((Object)simpleButton).name = "TypeMod";
		simpleButton.Setup(new UnityAction(ShowMods), InternalResourcesGetter.GetIcon(), null, new Vector2(32f, 32f), "normal", new TooltipData
		{
			tip_name = "TypeMod Title"
		});
		((Behaviour)simpleButton.Background).enabled = false;
		SimpleButton simpleButton2 = Object.Instantiate<SimpleButton>(APrefab<SimpleButton>.Prefab, val.transform);
		((Object)simpleButton2).name = "TypeResource";
		simpleButton2.Setup(new UnityAction(ShowResources), SpriteTextureLoader.getSprite("ui/icons/tech/icon_tech_city_storage_3"), null, new Vector2(32f, 32f), "normal", new TooltipData
		{
			tip_name = "TypeResource Title"
		});
		((Behaviour)simpleButton2.Background).enabled = false;
		GameObject gameObject = ((Component)base.BackgroundTransform.Find("Scroll View")).gameObject;
		((Object)gameObject).name = "List Scroll View";
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(108f, 255f);
		((Transform)component).localPosition = new Vector3(-174f, 0f, 0f);
		((Transform)component).localScale = Vector3.one;
		ScrollRect component2 = gameObject.GetComponent<ScrollRect>();
		component2.verticalScrollbarVisibility = (ScrollbarVisibility)0;
		((Component)component2.verticalScrollbar).GetComponent<RectTransform>().sizeDelta = new Vector2(10f, 0f);
		Image component3 = gameObject.GetComponent<Image>();
		component3.sprite = SpriteTextureLoader.getSprite("ui/special/windowEmptyFrame");
		component3.type = (Type)1;
		((Graphic)component3).color = Color.white;
		RectTransform component4 = ((Component)gameObject.transform.Find("Viewport")).GetComponent<RectTransform>();
		component4.sizeDelta = new Vector2(0f, -20f);
		((Transform)component4).localPosition = ((Transform)component4).localPosition - new Vector3(0f, 10f);
		VerticalLayoutGroup pVerticalLayoutGroup = ((Component)base.ContentTransform).gameObject.AddComponent<VerticalLayoutGroup>();
		OT.InitializeNoActionVerticalLayoutGroup(pVerticalLayoutGroup);
		ContentSizeFitter val2 = ((Component)base.ContentTransform).gameObject.AddComponent<ContentSizeFitter>();
		val2.verticalFit = (FitMode)2;
		((Behaviour)((Component)base.BackgroundTransform.Find("Scrollgradient")).GetComponent<Image>()).enabled = false;
		Transform contentTransform = base.ContentTransform;
		ListPart = (RectTransform)(object)((contentTransform is RectTransform) ? contentTransform : null);
		ListItemPool = new ObjectPoolGenericMono<ModListItem>(APrefab<ModListItem>.Prefab, (Transform)(object)ListPart);
		GameObject val3 = new GameObject("ModInfoPart", new Type[2]
		{
			typeof(Image),
			typeof(VerticalLayoutGroup)
		});
		val3.transform.SetParent(base.BackgroundTransform);
		val3.transform.localPosition = new Vector3(60f, 25f);
		val3.transform.localScale = Vector3.one;
		val3.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
		val3.GetComponent<Image>().type = (Type)1;
		ModInfoPart = val3.GetComponent<RectTransform>();
		GameObject val4 = new GameObject("ModControlPart", new Type[2]
		{
			typeof(Image),
			typeof(HorizontalLayoutGroup)
		});
		val4.transform.SetParent(base.BackgroundTransform);
		val4.transform.localPosition = new Vector3(60f, -102f);
		val4.transform.localScale = Vector3.one;
		val4.GetComponent<Image>().sprite = InternalResourcesGetter.GetWindowEmptyFrame();
		val4.GetComponent<Image>().type = (Type)1;
		GameObject val5 = new GameObject("NMLGeneralPart", new Type[2]
		{
			typeof(Image),
			typeof(VerticalLayoutGroup)
		});
		val5.transform.SetParent(base.BackgroundTransform);
		val5.transform.localPosition = new Vector3(264f, 0f);
		val5.transform.localScale = Vector3.one;
		val5.GetComponent<Image>().sprite = InternalResourcesGetter.GetWindowEmptyFrame();
		val5.GetComponent<Image>().type = (Type)1;
		component = val4.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(350f, 48f);
		HorizontalLayoutGroup component5 = val4.GetComponent<HorizontalLayoutGroup>();
		((LayoutGroup)component5).childAlignment = (TextAnchor)3;
		((HorizontalOrVerticalLayoutGroup)component5).childControlHeight = false;
		((HorizontalOrVerticalLayoutGroup)component5).childControlWidth = false;
		((HorizontalOrVerticalLayoutGroup)component5).childForceExpandHeight = false;
		((HorizontalOrVerticalLayoutGroup)component5).childForceExpandWidth = false;
		((HorizontalOrVerticalLayoutGroup)component5).childScaleHeight = false;
		((HorizontalOrVerticalLayoutGroup)component5).childScaleWidth = false;
		((HorizontalOrVerticalLayoutGroup)component5).spacing = 4f;
		((LayoutGroup)component5).padding = new RectOffset(12, 0, 0, 0);
		ModConfigureButton = Object.Instantiate<SimpleButton>(APrefab<SimpleButton>.Prefab, (Transform)(object)component);
		((Object)ModConfigureButton).name = "ModConfigureButton";
		ModConfigureButton.Setup(new UnityAction(ConfigureSelectedMod), SpriteTextureLoader.getSprite("ui/icons/iconOptions"), null, new Vector2(32f, 32f), "normal", new TooltipData
		{
			tip_name = "ModConfigure Title"
		});
		((Behaviour)ModConfigureButton.Background).enabled = false;
		ModCommunityButton = Object.Instantiate<SimpleButton>(APrefab<SimpleButton>.Prefab, (Transform)(object)component);
		((Object)ModCommunityButton).name = "ModCommunityButton";
		ModCommunityButton.Setup(new UnityAction(CommunityOfSelectedMod), SpriteTextureLoader.getSprite("ui/icons/iconCommunity"), null, new Vector2(32f, 32f), "normal", new TooltipData
		{
			tip_name = "ModCommunity Title"
		});
		((Behaviour)ModCommunityButton.Background).enabled = false;
		OpenModFolderButton = Object.Instantiate<SimpleButton>(APrefab<SimpleButton>.Prefab, (Transform)(object)component);
		((Object)OpenModFolderButton).name = "OpenModFolderButton";
		OpenModFolderButton.Setup(new UnityAction(FolderOfSelectedMod), SpriteTextureLoader.getSprite("ui/icons/iconCustomWorld"), null, new Vector2(32f, 32f), "normal", new TooltipData
		{
			tip_name = "OpenFolder Title"
		});
		((Behaviour)OpenModFolderButton.Background).enabled = false;
		ToggleModButton = Object.Instantiate<SimpleButton>(APrefab<SimpleButton>.Prefab, (Transform)(object)component);
		((Object)ToggleModButton).name = "ToggleModButton";
		ToggleModButton.Setup(new UnityAction(ToggleSelectedMod), SpriteTextureLoader.getSprite("ui/icons/iconOn"), null, new Vector2(32f, 32f), "normal", new TooltipData
		{
			tip_name = "ToggleMod Title"
		});
		((Behaviour)ToggleModButton.Background).enabled = false;
		ReloadModButton = Object.Instantiate<SimpleButton>(APrefab<SimpleButton>.Prefab, (Transform)(object)component);
		((Object)ReloadModButton).name = "ReloadModButton";
		ReloadModButton.Setup(new UnityAction(ReloadSelectedMod), InternalResourcesGetter.GetReloadIcon(), null, new Vector2(32f, 32f), "normal", new TooltipData
		{
			tip_name = "ReloadMod Title"
		});
		((Behaviour)ReloadModButton.Background).enabled = false;
		UploadModButton = Object.Instantiate<SimpleButton>(APrefab<SimpleButton>.Prefab, (Transform)(object)component);
		((Object)UploadModButton).name = "UploadModButton";
		UploadModButton.Setup(new UnityAction(UploadSelectedMod), SpriteTextureLoader.getSprite("ui/icons/iconSaveCloud"), null, new Vector2(32f, 32f), "normal", new TooltipData
		{
			tip_name = "UploadMod Title"
		});
		((Behaviour)UploadModButton.Background).enabled = false;
		component = val3.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(350f, 200f);
		component = val5.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(48f, 255f);
	}

	private void ShowResources()
	{
		Clean();
	}

	private void ShowMods()
	{
		Clean();
		ListToShow = WorldBoxMod.AllRecognizedMods.Keys.ToList();
		foreach (ModDeclare item in ListToShow)
		{
			ModListItem next = ListItemPool.getNext();
			ModDeclare local_mod = item;
			next.Setup(item, delegate
			{
				Select(local_mod);
			});
		}
	}

	public override void OnFirstEnable()
	{
		CurrentDisplayType = DisplayType.Mod;
	}

	public override void OnNormalEnable()
	{
		switch (CurrentDisplayType)
		{
		case DisplayType.Mod:
			ShowMods();
			break;
		case DisplayType.Resource:
			ShowResources();
			break;
		}
	}

	private void Clean()
	{
		ListItemPool.clear();
	}

	private void Select(ModDeclare pDeclare)
	{
		if (CurrentSelected != pDeclare)
		{
			CurrentSelected = pDeclare;
			RefreshInfoPart();
			RefreshControlPart();
		}
	}

	private void RefreshControlPart()
	{
		throw new NotImplementedException();
	}

	private void RefreshInfoPart()
	{
		foreach (ModInfoPanel value in ModInfoPanels.Values)
		{
			((Component)value).gameObject.SetActive(false);
		}
		if (ModInfoPanels.ContainsKey(CurrentSelected))
		{
			((Component)ModInfoPanels[CurrentSelected]).gameObject.SetActive(true);
			return;
		}
		ModInfoPanel modInfoPanel = Object.Instantiate<ModInfoPanel>(APrefab<ModInfoPanel>.Prefab, (Transform)(object)ModInfoPart);
		modInfoPanel.Setup(CurrentSelected);
		ModInfoPanels.Add(CurrentSelected, modInfoPanel);
	}

	private void CommunityOfSelectedMod()
	{
		throw new NotImplementedException();
	}

	private void ConfigureSelectedMod()
	{
		throw new NotImplementedException();
	}

	private void UploadSelectedMod()
	{
		throw new NotImplementedException();
	}

	private void ReloadSelectedMod()
	{
		throw new NotImplementedException();
	}

	private void ToggleSelectedMod()
	{
		throw new NotImplementedException();
	}

	private void FolderOfSelectedMod()
	{
		throw new NotImplementedException();
	}
}
