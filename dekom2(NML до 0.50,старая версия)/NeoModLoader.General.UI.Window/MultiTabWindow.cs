using System;
using System.Collections.Generic;
using System.Linq;
using NeoModLoader.General.UI.Prefabs;
using NeoModLoader.General.UI.Window.Layout;
using NeoModLoader.utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NeoModLoader.General.UI.Window;

public abstract class MultiTabWindow<T> : AutoLayoutWindow<T> where T : MultiTabWindow<T>
{
	private readonly Dictionary<SimpleButton, AutoVertLayoutGroup> m_tabs = new Dictionary<SimpleButton, AutoVertLayoutGroup>();

	private RectTransform m_tab_entries_left;

	private RectTransform m_tab_entries_right;

	protected string CurrentTab { get; private set; } = "Default";

	public new static T CreateWindow(string pWindowID, string pWindowTitleKey)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Expected O, but got Unknown
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Expected O, but got Unknown
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Expected O, but got Unknown
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Expected O, but got Unknown
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Expected O, but got Unknown
		ScrollWindow scrollWindow = WindowCreator.CreateEmptyWindow(pWindowID, pWindowTitleKey);
		((Component)scrollWindow).gameObject.SetActive(false);
		((Component)scrollWindow.transform_content).gameObject.AddComponent<VerticalLayoutGroup>();
		T val = ((Component)scrollWindow.transform_content).gameObject.AddComponent<T>();
		val.BackgroundTransform = ((Component)scrollWindow).transform.Find("Background");
		((Component)scrollWindow.transform_scrollRect).gameObject.SetActive(true);
		scrollWindow.transform_scrollRect.sizeDelta = new Vector2(210f, scrollWindow.transform_scrollRect.sizeDelta.y);
		val.ContentTransform = (Transform)(object)scrollWindow.transform_content;
		val.ScrollWindowComponent = scrollWindow;
		VerticalLayoutGroup layoutGroup = val.GetLayoutGroup();
		((LayoutGroup)layoutGroup).childAlignment = (TextAnchor)1;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childControlHeight = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childControlWidth = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childForceExpandHeight = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childForceExpandWidth = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childScaleHeight = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childScaleWidth = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).spacing = 10f;
		((LayoutGroup)layoutGroup).padding = new RectOffset(3, 3, 10, 10);
		ContentSizeFitter val2 = ((Component)scrollWindow.transform_content).gameObject.AddComponent<ContentSizeFitter>();
		val2.verticalFit = (FitMode)2;
		val2.horizontalFit = (FitMode)0;
		GameObject val3 = new GameObject("TabEntriesContainer", new Type[2]
		{
			typeof(RectTransform),
			typeof(HorizontalLayoutGroup)
		});
		val3.transform.SetParent(val.BackgroundTransform);
		val3.transform.SetAsFirstSibling();
		val3.transform.localPosition = Vector3.zero;
		val3.transform.localScale = Vector3.one;
		val3.GetComponent<RectTransform>().sizeDelta = new Vector2(256f, 220f);
		HorizontalLayoutGroup component = val3.GetComponent<HorizontalLayoutGroup>();
		((LayoutGroup)component).childAlignment = (TextAnchor)4;
		((HorizontalOrVerticalLayoutGroup)component).childControlHeight = false;
		((HorizontalOrVerticalLayoutGroup)component).childControlWidth = false;
		((HorizontalOrVerticalLayoutGroup)component).childForceExpandHeight = false;
		((HorizontalOrVerticalLayoutGroup)component).childForceExpandWidth = false;
		((HorizontalOrVerticalLayoutGroup)component).childScaleHeight = false;
		((HorizontalOrVerticalLayoutGroup)component).childScaleWidth = false;
		((HorizontalOrVerticalLayoutGroup)component).spacing = 208f;
		GameObject val4 = new GameObject("LeftContainer", new Type[4]
		{
			typeof(RectTransform),
			typeof(VerticalLayoutGroup),
			typeof(Mask),
			typeof(Image)
		});
		val4.transform.SetParent(val3.transform);
		val4.transform.localScale = Vector3.one;
		val4.GetComponent<Mask>().showMaskGraphic = false;
		VerticalLayoutGroup component2 = val4.GetComponent<VerticalLayoutGroup>();
		((LayoutGroup)component2).childAlignment = (TextAnchor)1;
		((HorizontalOrVerticalLayoutGroup)component2).childControlHeight = false;
		((HorizontalOrVerticalLayoutGroup)component2).childControlWidth = false;
		((HorizontalOrVerticalLayoutGroup)component2).childForceExpandHeight = false;
		((HorizontalOrVerticalLayoutGroup)component2).childForceExpandWidth = false;
		((HorizontalOrVerticalLayoutGroup)component2).childScaleHeight = false;
		((HorizontalOrVerticalLayoutGroup)component2).childScaleWidth = false;
		((HorizontalOrVerticalLayoutGroup)component2).spacing = 4f;
		((LayoutGroup)component2).padding = new RectOffset(4, 0, 0, 0);
		val4.GetComponent<RectTransform>().sizeDelta = new Vector2(24f, 220f);
		val.m_tab_entries_left = val4.GetComponent<RectTransform>();
		GameObject val5 = Object.Instantiate<GameObject>(val4, val3.transform);
		((Object)val5).name = "RightContainer";
		val5.transform.localScale = Vector3.one;
		((LayoutGroup)val5.GetComponent<VerticalLayoutGroup>()).padding = new RectOffset(0, 4, 0, 0);
		val.m_tab_entries_right = val5.GetComponent<RectTransform>();
		val.WindowID = pWindowID;
		val.Init();
		val.Initialized = true;
		return val;
	}

	protected AutoVertLayoutGroup CreateTab(string pTabID, Sprite pTabIcon, UnityAction<string> pAdditionTabSwitchAction = null)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Expected O, but got Unknown
		AutoVertLayoutGroup tab = Object.Instantiate<AutoVertLayoutGroup>(APrefab<AutoVertLayoutGroup>.Prefab, base.ContentTransform.parent);
		tab.Setup(default(Vector2), (TextAnchor)1, 10f, new RectOffset(3, 3, 10, 10));
		((Component)tab).transform.localScale = Vector3.one;
		((Component)tab).transform.localPosition = Vector3.zero;
		((Component)tab).GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
		((Component)tab).gameObject.SetActive(false);
		((Object)tab).name = pTabID;
		SimpleButton tab_entry = Object.Instantiate<SimpleButton>(APrefab<SimpleButton>.Prefab, (Transform)(object)((((Transform)m_tab_entries_left).childCount > ((Transform)m_tab_entries_right).childCount) ? m_tab_entries_right : m_tab_entries_left));
		tab_entry.Setup((UnityAction)delegate
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected O, but got Unknown
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			foreach (Transform item in base.ContentTransform.parent)
			{
				Transform val = item;
				((Component)val).gameObject.SetActive(false);
			}
			if (((Graphic)tab_entry.Background).color == Color.gray)
			{
				((Graphic)tab_entry.Background).color = Color.white;
				CurrentTab = "Default";
				((Component)tab).gameObject.SetActive(false);
				((Component)base.ContentTransform).gameObject.SetActive(true);
			}
			else
			{
				((Graphic)tab_entry.Background).color = Color.gray;
				CurrentTab = pTabID;
				((Component)tab).gameObject.SetActive(true);
				pAdditionTabSwitchAction?.Invoke(pTabID);
			}
			foreach (KeyValuePair<SimpleButton, AutoVertLayoutGroup> item2 in m_tabs.Where((KeyValuePair<SimpleButton, AutoVertLayoutGroup> tab_entry_pair) => (Object)(object)tab_entry_pair.Key != (Object)(object)tab_entry))
			{
				((Graphic)item2.Key.Background).color = Color.white;
				((Component)item2.Value).gameObject.SetActive(false);
			}
		}, pTabIcon, null, new Vector2(24f, 48f), "normal", new TooltipData
		{
			tip_name = pTabID,
			tip_description = pTabID + " Description"
		});
		tab_entry.Background.sprite = InternalResourcesGetter.GetWindowVertNamePlate();
		m_tabs.Add(tab_entry, tab);
		ResizeTabEntries();
		return tab;
	}

	private void ResizeTabEntries()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		VerticalLayoutGroup val = null;
		RectTransform val2 = null;
		num = ((Transform)m_tab_entries_left).childCount;
		val = ((Component)m_tab_entries_left).GetComponent<VerticalLayoutGroup>();
		val2 = m_tab_entries_left;
		if (num <= 4)
		{
			((HorizontalOrVerticalLayoutGroup)val).spacing = 4f;
		}
		else
		{
			((HorizontalOrVerticalLayoutGroup)val).spacing = (val2.sizeDelta.y - (float)(num * 48)) / (float)(num - 1);
		}
		num = ((Transform)m_tab_entries_right).childCount;
		val = ((Component)m_tab_entries_right).GetComponent<VerticalLayoutGroup>();
		val2 = m_tab_entries_right;
		if (num <= 4)
		{
			((HorizontalOrVerticalLayoutGroup)val).spacing = 4f;
		}
		else
		{
			((HorizontalOrVerticalLayoutGroup)val).spacing = (val2.sizeDelta.y - (float)(num * 48)) / (float)(num - 1);
		}
	}
}
