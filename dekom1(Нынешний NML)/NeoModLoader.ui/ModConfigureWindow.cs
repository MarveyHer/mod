using System;
using System.Collections.Generic;
using NeoModLoader.api;
using NeoModLoader.General;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace NeoModLoader.ui;

public class ModConfigureWindow : AbstractWindow<ModConfigureWindow>
{
	private class ModConfigGrid : MonoBehaviour
	{
		private Transform grid;

		private Text title;

		private void OnEnable()
		{
			title = ((Component)((Component)this).transform.Find("Title")).GetComponent<Text>();
			grid = ((Component)this).transform.Find("Grid");
		}

		public void Setup(string id, Dictionary<string, ModConfigItem> items)
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			((Object)this).name = id;
			title.text = LM.Get(id);
			foreach (KeyValuePair<string, ModConfigItem> item in items)
			{
				ModConfigListItem next = _itemPool.getNext();
				Transform transform;
				(transform = ((Component)next).transform).SetParent(grid);
				transform.localScale = Vector3.one;
				next.Setup(item.Value);
			}
		}
	}

	private class ModConfigListItem : MonoBehaviour
	{
		public GameObject switch_area;

		public GameObject slider_area;

		public GameObject text_area;

		public GameObject select_area;

		public void Setup(ModConfigItem pItem)
		{
			((Object)this).name = pItem.Id;
			switch_area.SetActive(false);
			slider_area.SetActive(false);
			text_area.SetActive(false);
			select_area.SetActive(false);
			switch (pItem.Type)
			{
			case ConfigItemType.SWITCH:
				setup_switch(pItem);
				break;
			case ConfigItemType.SLIDER:
				setup_slider(pItem);
				break;
			case ConfigItemType.INT_SLIDER:
				setup_int_slider(pItem);
				break;
			case ConfigItemType.TEXT:
				setup_text(pItem);
				break;
			case ConfigItemType.SELECT:
				break;
			}
		}

		private void setup_text(ModConfigItem pItem)
		{
			text_area.SetActive(true);
			TextInput component = ((Component)text_area.transform.Find("Input")).GetComponent<TextInput>();
			component.Setup(pItem.TextVal, delegate(string pStringVal)
			{
				if (!AbstractWindow<ModConfigureWindow>.Instance._modifiedItems.ContainsKey(pItem))
				{
					AbstractWindow<ModConfigureWindow>.Instance._modifiedItems.Add(pItem, pItem.GetValue());
				}
				pItem.SetValue(pStringVal, pSkipCallback: true);
			});
			component.tip_button.textOnClick = pItem.Id;
			component.tip_button.text_description_2 = pItem.Id + " Description";
			((Component)text_area.transform.Find("Info/Text")).GetComponent<Text>().text = LM.Get(pItem.Id);
			if (string.IsNullOrEmpty(pItem.IconPath))
			{
				((Component)text_area.transform.Find("Info/Icon")).gameObject.SetActive(false);
				return;
			}
			Image component2 = ((Component)text_area.transform.Find("Info/Icon")).GetComponent<Image>();
			((Component)component2).gameObject.SetActive(true);
			component2.sprite = SpriteTextureLoader.getSprite(pItem.IconPath);
		}

		private void setup_slider(ModConfigItem pItem)
		{
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			slider_area.SetActive(true);
			Text value = ((Component)slider_area.transform.Find("Info/Value")).GetComponent<Text>();
			value.text = $"{pItem.FloatVal:F2}";
			SliderBar component = ((Component)slider_area.transform.Find("Slider")).GetComponent<SliderBar>();
			component.Setup(pItem.FloatVal, pItem.MinFloatVal, pItem.MaxFloatVal, delegate(float pFloatVal)
			{
				if (!AbstractWindow<ModConfigureWindow>.Instance._modifiedItems.ContainsKey(pItem))
				{
					AbstractWindow<ModConfigureWindow>.Instance._modifiedItems.Add(pItem, pItem.GetValue());
				}
				pItem.SetValue(pFloatVal, pSkipCallback: true);
				value.text = $"{pItem.FloatVal:F2}";
			});
			component.tip_button.textOnClick = pItem.Id;
			component.tip_button.text_description_2 = pItem.Id + " Description";
			((Component)slider_area.transform.Find("Info/Text")).GetComponent<Text>().text = LM.Get(pItem.Id);
			if (string.IsNullOrEmpty(pItem.IconPath))
			{
				((Component)slider_area.transform.Find("Info/Icon")).gameObject.SetActive(false);
				return;
			}
			Image component2 = ((Component)slider_area.transform.Find("Info/Icon")).GetComponent<Image>();
			((Component)component2).gameObject.SetActive(true);
			component2.sprite = SpriteTextureLoader.getSprite(pItem.IconPath);
		}

		private void setup_int_slider(ModConfigItem pItem)
		{
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			slider_area.SetActive(true);
			Text value = ((Component)slider_area.transform.Find("Info/Value")).GetComponent<Text>();
			value.text = $"{pItem.IntVal}";
			SliderBar component = ((Component)slider_area.transform.Find("Slider")).GetComponent<SliderBar>();
			component.Setup(pItem.IntVal, pItem.MinIntVal, pItem.MaxIntVal, delegate(float pIntVal)
			{
				if (!AbstractWindow<ModConfigureWindow>.Instance._modifiedItems.ContainsKey(pItem))
				{
					AbstractWindow<ModConfigureWindow>.Instance._modifiedItems.Add(pItem, pItem.GetValue());
				}
				pItem.SetValue(pIntVal, pSkipCallback: true);
				value.text = $"{pItem.IntVal}";
			}, default(Vector2), whole_numbers: true);
			component.tip_button.textOnClick = pItem.Id;
			component.tip_button.text_description_2 = pItem.Id + " Description";
			((Component)slider_area.transform.Find("Info/Text")).GetComponent<Text>().text = LM.Get(pItem.Id);
			if (string.IsNullOrEmpty(pItem.IconPath))
			{
				((Component)slider_area.transform.Find("Info/Icon")).gameObject.SetActive(false);
				return;
			}
			Image component2 = ((Component)slider_area.transform.Find("Info/Icon")).GetComponent<Image>();
			((Component)component2).gameObject.SetActive(true);
			component2.sprite = SpriteTextureLoader.getSprite(pItem.IconPath);
		}

		private void setup_switch(ModConfigItem pItem)
		{
			switch_area.SetActive(true);
			NeoModLoader.General.UI.Prefabs.SwitchButton component = ((Component)switch_area.transform.Find("Button")).GetComponent<NeoModLoader.General.UI.Prefabs.SwitchButton>();
			component.Setup(pItem.BoolVal, delegate
			{
				if (!AbstractWindow<ModConfigureWindow>.Instance._modifiedItems.ContainsKey(pItem))
				{
					AbstractWindow<ModConfigureWindow>.Instance._modifiedItems.Add(pItem, pItem.GetValue());
				}
				pItem.SetValue(!pItem.BoolVal, pSkipCallback: true);
			});
			component.tip_button.textOnClick = pItem.Id;
			component.tip_button.text_description_2 = pItem.Id + " Description";
			((Component)switch_area.transform.Find("Text")).GetComponent<Text>().text = LM.Get(pItem.Id);
			if (string.IsNullOrEmpty(pItem.IconPath))
			{
				((Component)switch_area.transform.Find("Icon")).gameObject.SetActive(false);
				return;
			}
			Image component2 = ((Component)switch_area.transform.Find("Icon")).GetComponent<Image>();
			((Component)component2).gameObject.SetActive(true);
			component2.sprite = SpriteTextureLoader.getSprite(pItem.IconPath);
		}
	}

	private static ModConfigGrid _gridPrefab;

	private static ModConfigListItem _itemPrefab;

	private static ObjectPoolGenericMono<ModConfigGrid> _gridPool;

	private static ObjectPoolGenericMono<ModConfigListItem> _itemPool;

	private readonly Dictionary<ModConfigItem, object> _modifiedItems = new Dictionary<ModConfigItem, object>();

	private ModConfig _config;

	protected override void Init()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Expected O, but got Unknown
		((Component)base.BackgroundTransform.Find("Scroll View")).gameObject.SetActive(true);
		((Component)base.BackgroundTransform.Find("Scroll View")).GetComponent<RectTransform>().sizeDelta = new Vector2(232f, 270f);
		base.BackgroundTransform.Find("Scroll View").localPosition = new Vector3(0f, -6f);
		((Component)base.BackgroundTransform.Find("Scroll View/Viewport")).GetComponent<RectTransform>().sizeDelta = new Vector2(30f, 0f);
		base.BackgroundTransform.Find("Scroll View/Viewport").localPosition = new Vector3(-131f, 135f);
		VerticalLayoutGroup val = ((Component)base.ContentTransform).gameObject.AddComponent<VerticalLayoutGroup>();
		((HorizontalOrVerticalLayoutGroup)val).childControlHeight = true;
		((HorizontalOrVerticalLayoutGroup)val).childControlWidth = true;
		((HorizontalOrVerticalLayoutGroup)val).childForceExpandHeight = false;
		((HorizontalOrVerticalLayoutGroup)val).childForceExpandWidth = false;
		((LayoutGroup)val).childAlignment = (TextAnchor)1;
		((LayoutGroup)val).padding = new RectOffset(32, 32, 0, 0);
		ContentSizeFitter val2 = ((Component)base.ContentTransform).gameObject.AddComponent<ContentSizeFitter>();
		val2.verticalFit = (FitMode)2;
		_createGridPrefab();
		_createItemPrefab();
		_gridPool = new ObjectPoolGenericMono<ModConfigGrid>(_gridPrefab, base.ContentTransform);
		_itemPool = new ObjectPoolGenericMono<ModConfigListItem>(_itemPrefab, base.BackgroundTransform);
	}

	private static void _createItemPrefab()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Expected O, but got Unknown
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Expected O, but got Unknown
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Expected O, but got Unknown
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Expected O, but got Unknown
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Expected O, but got Unknown
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Expected O, but got Unknown
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Expected O, but got Unknown
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Expected O, but got Unknown
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Expected O, but got Unknown
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Expected O, but got Unknown
		//IL_073a: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("ConfigItem", new Type[2]
		{
			typeof(Image),
			typeof(VerticalLayoutGroup)
		});
		VerticalLayoutGroup component = val.GetComponent<VerticalLayoutGroup>();
		((LayoutGroup)component).childAlignment = (TextAnchor)3;
		((LayoutGroup)component).padding = new RectOffset(4, 4, 3, 3);
		GameObject val2 = new GameObject("SwitchArea", new Type[1] { typeof(HorizontalLayoutGroup) });
		HorizontalLayoutGroup component2 = val2.GetComponent<HorizontalLayoutGroup>();
		((HorizontalOrVerticalLayoutGroup)component2).childControlWidth = false;
		((HorizontalOrVerticalLayoutGroup)component2).childControlHeight = false;
		((LayoutGroup)component2).childAlignment = (TextAnchor)3;
		val2.transform.SetParent(val.transform);
		val2.transform.localScale = Vector3.one;
		NeoModLoader.General.UI.Prefabs.SwitchButton switchButton = Object.Instantiate<NeoModLoader.General.UI.Prefabs.SwitchButton>(APrefab<NeoModLoader.General.UI.Prefabs.SwitchButton>.Prefab, val2.transform);
		((Component)switchButton).transform.localScale = Vector3.one;
		((Object)switchButton).name = "Button";
		GameObject val3 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val3.transform.SetParent(val2.transform);
		val3.transform.localScale = Vector3.one;
		val3.GetComponent<RectTransform>().sizeDelta = new Vector2(16f, 16f);
		GameObject val4 = new GameObject("Text", new Type[1] { typeof(Text) });
		val4.transform.SetParent(val2.transform);
		val4.transform.localScale = Vector3.one;
		val4.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 16f);
		Text component3 = val4.GetComponent<Text>();
		OT.InitializeCommonText(component3);
		component3.alignment = (TextAnchor)3;
		component3.resizeTextForBestFit = true;
		component3.resizeTextMinSize = 1;
		GameObject val5 = new GameObject("SliderArea", new Type[2]
		{
			typeof(RectTransform),
			typeof(VerticalLayoutGroup)
		});
		val5.transform.SetParent(val.transform);
		val5.transform.localScale = Vector3.one;
		VerticalLayoutGroup component4 = val5.GetComponent<VerticalLayoutGroup>();
		((HorizontalOrVerticalLayoutGroup)component4).childControlWidth = true;
		((HorizontalOrVerticalLayoutGroup)component4).childControlHeight = false;
		((HorizontalOrVerticalLayoutGroup)component4).childForceExpandWidth = false;
		((LayoutGroup)component4).childAlignment = (TextAnchor)1;
		((HorizontalOrVerticalLayoutGroup)component4).spacing = 4f;
		GameObject val6 = new GameObject("Info", new Type[2]
		{
			typeof(RectTransform),
			typeof(HorizontalLayoutGroup)
		});
		val6.transform.SetParent(val5.transform);
		val6.transform.localScale = Vector3.one;
		val6.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 18f);
		HorizontalLayoutGroup component5 = val6.GetComponent<HorizontalLayoutGroup>();
		((HorizontalOrVerticalLayoutGroup)component5).childControlWidth = false;
		((HorizontalOrVerticalLayoutGroup)component5).childControlHeight = false;
		((LayoutGroup)component5).childAlignment = (TextAnchor)3;
		GameObject val7 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val7.transform.SetParent(val6.transform);
		val7.transform.localScale = Vector3.one;
		val7.GetComponent<RectTransform>().sizeDelta = new Vector2(16f, 16f);
		GameObject val8 = new GameObject("Text", new Type[1] { typeof(Text) });
		val8.transform.SetParent(val6.transform);
		val8.transform.localScale = Vector3.one;
		val8.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 16f);
		Text component6 = val8.GetComponent<Text>();
		OT.InitializeCommonText(component6);
		component6.alignment = (TextAnchor)3;
		component6.resizeTextForBestFit = true;
		GameObject val9 = new GameObject("Value", new Type[1] { typeof(Text) });
		val9.transform.SetParent(val6.transform);
		val9.transform.localScale = Vector3.one;
		val9.GetComponent<RectTransform>().sizeDelta = new Vector2(32f, 16f);
		Text component7 = val9.GetComponent<Text>();
		OT.InitializeCommonText(component7);
		component7.alignment = (TextAnchor)5;
		component7.resizeTextForBestFit = true;
		component7.resizeTextMinSize = 1;
		SliderBar sliderBar = Object.Instantiate<SliderBar>(APrefab<SliderBar>.Prefab, val5.transform);
		((Component)sliderBar).transform.localScale = Vector3.one;
		((Object)sliderBar).name = "Slider";
		sliderBar.SetSize(new Vector2(170f, 20f));
		GameObject val10 = new GameObject("TextArea", new Type[2]
		{
			typeof(RectTransform),
			typeof(VerticalLayoutGroup)
		});
		val10.transform.SetParent(val.transform);
		val10.transform.localScale = Vector3.one;
		VerticalLayoutGroup component8 = val10.GetComponent<VerticalLayoutGroup>();
		((HorizontalOrVerticalLayoutGroup)component8).childControlWidth = true;
		((HorizontalOrVerticalLayoutGroup)component8).childControlHeight = false;
		((LayoutGroup)component8).childAlignment = (TextAnchor)1;
		((HorizontalOrVerticalLayoutGroup)component8).spacing = 4f;
		GameObject val11 = new GameObject("Info", new Type[2]
		{
			typeof(RectTransform),
			typeof(HorizontalLayoutGroup)
		});
		val11.transform.SetParent(val10.transform);
		val11.transform.localScale = Vector3.one;
		val11.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 18f);
		HorizontalLayoutGroup component9 = val11.GetComponent<HorizontalLayoutGroup>();
		((HorizontalOrVerticalLayoutGroup)component9).childControlWidth = false;
		((HorizontalOrVerticalLayoutGroup)component9).childControlHeight = false;
		((HorizontalOrVerticalLayoutGroup)component9).childForceExpandWidth = false;
		((LayoutGroup)component9).childAlignment = (TextAnchor)3;
		((HorizontalOrVerticalLayoutGroup)component9).spacing = 8f;
		GameObject val12 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val12.transform.SetParent(val11.transform);
		val12.transform.localScale = Vector3.one;
		val12.GetComponent<RectTransform>().sizeDelta = new Vector2(16f, 16f);
		GameObject val13 = new GameObject("Text", new Type[1] { typeof(Text) });
		val13.transform.SetParent(val11.transform);
		val13.transform.localScale = Vector3.one;
		val13.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 16f);
		Text component10 = val13.GetComponent<Text>();
		OT.InitializeCommonText(component10);
		component10.alignment = (TextAnchor)3;
		component10.resizeTextForBestFit = true;
		component10.resizeTextMinSize = 1;
		TextInput textInput = Object.Instantiate<TextInput>(APrefab<TextInput>.Prefab, val10.transform);
		((Component)textInput).transform.localScale = Vector3.one;
		((Object)textInput).name = "Input";
		textInput.SetSize(new Vector2(170f, 20f));
		GameObject val14 = new GameObject("SelectArea", new Type[1] { typeof(RectTransform) });
		val14.transform.SetParent(val.transform);
		val14.transform.localScale = Vector3.one;
		val.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
		val.GetComponent<Image>().type = (Type)1;
		val.transform.SetParent(WorldBoxMod.Transform);
		_itemPrefab = val.AddComponent<ModConfigListItem>();
		_itemPrefab.switch_area = val2;
		_itemPrefab.slider_area = val5;
		_itemPrefab.text_area = val10;
		_itemPrefab.select_area = val14;
	}

	private static void _createGridPrefab()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Expected O, but got Unknown
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("ConfigGrid", new Type[1] { typeof(VerticalLayoutGroup) });
		VerticalLayoutGroup component = val.GetComponent<VerticalLayoutGroup>();
		((HorizontalOrVerticalLayoutGroup)component).childControlHeight = true;
		((HorizontalOrVerticalLayoutGroup)component).childControlWidth = true;
		((HorizontalOrVerticalLayoutGroup)component).childForceExpandHeight = false;
		((HorizontalOrVerticalLayoutGroup)component).childForceExpandWidth = false;
		((LayoutGroup)component).childAlignment = (TextAnchor)1;
		GameObject val2 = new GameObject("Title", new Type[1] { typeof(Text) });
		val2.transform.SetParent(val.transform);
		val2.transform.localScale = Vector3.one;
		Text component2 = val2.GetComponent<Text>();
		component2.text = "Mod Config";
		component2.font = LocalizedTextManager.current_font;
		component2.resizeTextForBestFit = true;
		component2.resizeTextMinSize = 1;
		component2.resizeTextMaxSize = 10;
		component2.alignment = (TextAnchor)4;
		GameObject val3 = new GameObject("Grid", new Type[2]
		{
			typeof(Image),
			typeof(VerticalLayoutGroup)
		});
		val3.transform.SetParent(val.transform);
		val3.transform.localScale = Vector3.one;
		component = val3.GetComponent<VerticalLayoutGroup>();
		((HorizontalOrVerticalLayoutGroup)component).childControlHeight = true;
		((HorizontalOrVerticalLayoutGroup)component).childControlWidth = true;
		((HorizontalOrVerticalLayoutGroup)component).childForceExpandHeight = false;
		((HorizontalOrVerticalLayoutGroup)component).childForceExpandWidth = false;
		((LayoutGroup)component).childAlignment = (TextAnchor)1;
		((LayoutGroup)component).padding = new RectOffset(4, 4, 5, 5);
		((HorizontalOrVerticalLayoutGroup)component).spacing = 4f;
		val3.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
		val3.GetComponent<Image>().type = (Type)1;
		((Graphic)val3.GetComponent<Image>()).color = new Color(1f, 1f, 1f, 0.5608f);
		val.transform.SetParent(WorldBoxMod.Transform);
		_gridPrefab = val.AddComponent<ModConfigGrid>();
	}

	public static void ShowWindow(ModConfig pConfig)
	{
		if (pConfig != null)
		{
			AbstractWindow<ModConfigureWindow>.Instance._config = pConfig;
			ScrollWindow.showWindow(AbstractWindow<ModConfigureWindow>.WindowId);
		}
	}

	public override void OnNormalEnable()
	{
		_modifiedItems.Clear();
		foreach (KeyValuePair<string, Dictionary<string, ModConfigItem>> item in _config._config)
		{
			ModConfigGrid next = _gridPool.getNext();
			next.Setup(item.Key, item.Value);
		}
	}

	public override void OnNormalDisable()
	{
		_gridPool.clear();
		_itemPool.clear();
		foreach (KeyValuePair<ModConfigItem, object> modifiedItem in _modifiedItems)
		{
			object value = modifiedItem.Key.GetValue();
			if (value != modifiedItem.Value)
			{
				modifiedItem.Key.SetValue(value);
			}
		}
		_config?.Save();
		_config = null;
	}
}
