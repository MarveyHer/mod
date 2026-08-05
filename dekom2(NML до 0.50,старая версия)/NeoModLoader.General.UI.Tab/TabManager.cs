using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using NeoModLoader.constants;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NeoModLoader.General.UI.Tab;

public static class TabManager
{
	private const int tab_count_each_line = 14;

	private const float check_new_tabs_interval = 1f;

	private const float shrink_coef = 0.79f;

	private const float default_tab_width = 43f;

	private const float default_tab_height = 18f;

	private const float default_icon_width = 33f;

	private const float default_icon_height = 11f;

	private const float default_tab_y = 2.0082f;

	private static readonly Transform tab_entry_container;

	private static readonly Transform tab_container;

	private static readonly List<Button> tab_entries;

	private static readonly List<string> tab_names;

	private static readonly HashSet<string> tab_names_set;

	private static float _check_timer;

	private static Vector3 _last_mouse_pos;

	public static readonly TabMain TabMain;

	public static readonly TabDrawing TabDrawing;

	public static readonly TabKingdoms TabKingdoms;

	public static readonly TabCreatures TabCreatures;

	public static readonly TabNature TabNature;

	public static readonly TabOther TabOther;

	private static readonly List<string> common_fix_for_tab_button;

	internal static void _init()
	{
		Harmony.CreateAndPatchAll(typeof(TabManager), "wbom.nml");
		List<string> names = PowerTabNames.GetNames();
		for (int i = 1; i < names.Count; i++)
		{
			tab_names.Add(names[i]);
			tab_names_set.Add(names[i]);
		}
		TabMain.Init();
		TabDrawing.Init();
		TabKingdoms.Init();
		TabCreatures.Init();
		TabNature.Init();
		TabOther.Init();
	}

	private static void _loadPredefinedOrder()
	{
		if (!File.Exists(Paths.TabOrderRecordPath))
		{
			return;
		}
		List<string> list = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Paths.TabOrderRecordPath));
		if (list == null)
		{
			return;
		}
		List<int> list2 = new List<int>();
		foreach (string item in list)
		{
			int num = tab_names.IndexOf(item);
			if (num >= 0)
			{
				list2.Add(num);
			}
		}
		List<int> list3 = new List<int>(list2);
		list3.Sort();
		for (int i = 0; i < list3.Count; i++)
		{
			int num2 = list2[i];
			int num3 = list3[i];
			if (num2 == num3)
			{
				continue;
			}
			tab_names.Swap(num2, num3);
			tab_entries.Swap(num2, num3);
			for (int j = i + 1; j < list3.Count; j++)
			{
				if (list2[j] == num3)
				{
					list2[j] = num2;
					break;
				}
			}
		}
	}

	private static void _savePredefinedOrder()
	{
		File.WriteAllText(Paths.TabOrderRecordPath, JsonConvert.SerializeObject((object)tab_names));
	}

	[HarmonyTranspiler]
	[HarmonyPatch(typeof(PowerTabController), "getNext")]
	private static IEnumerable<CodeInstruction> _getNext_Patch(IEnumerable<CodeInstruction> instr)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		List<CodeInstruction> list = new List<CodeInstruction>();
		list.Add(new CodeInstruction(OpCodes.Ldarg_0, (object)null));
		list.Add(new CodeInstruction(OpCodes.Ldarg_1, (object)null));
		list.Add(new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(TabManager), "_getNext_Overwrite", (Type[])null, (Type[])null)));
		list.Add(new CodeInstruction(OpCodes.Ret, (object)null));
		return list;
	}

	[HarmonyTranspiler]
	[HarmonyPatch(typeof(PowerTabController), "getPrev")]
	private static IEnumerable<CodeInstruction> _getPrev_Patch(IEnumerable<CodeInstruction> instr)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		List<CodeInstruction> list = new List<CodeInstruction>();
		list.Add(new CodeInstruction(OpCodes.Ldarg_0, (object)null));
		list.Add(new CodeInstruction(OpCodes.Ldarg_1, (object)null));
		list.Add(new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(TabManager), "_getPrev_Overwrite", (Type[])null, (Type[])null)));
		list.Add(new CodeInstruction(OpCodes.Ret, (object)null));
		return list;
	}

	private static Button _getNext_Overwrite(this PowerTabController instance, string pActiveTab)
	{
		return tab_entries[(tab_names.IndexOf(pActiveTab) + 1) % tab_entries.Count];
	}

	private static Button _getPrev_Overwrite(this PowerTabController instance, string pActiveTab)
	{
		int num = tab_names.IndexOf(pActiveTab);
		if (num < 0)
		{
			num = 1;
		}
		if (num == 0)
		{
			num = tab_entries.Count;
		}
		return tab_entries[num - 1];
	}

	internal static void _checkNewTabs()
	{
		if (_check_timer > 0f)
		{
			_check_timer -= Time.deltaTime;
			return;
		}
		_check_timer = 1f;
		PowersTab[] componentsInChildren = ((Component)tab_container).GetComponentsInChildren<PowersTab>(true);
		Button[] componentsInChildren2 = ((Component)tab_entry_container).GetComponentsInChildren<Button>(false);
		if (componentsInChildren2.Length == tab_entries.Count)
		{
			return;
		}
		bool flag = false;
		PowersTab[] array = componentsInChildren;
		foreach (PowersTab powersTab in array)
		{
			string name = ((Object)powersTab).name;
			if (tab_names_set.Contains(name))
			{
				continue;
			}
			string text = GetTabMainPart(name);
			Button[] array2 = componentsInChildren2;
			foreach (Button val in array2)
			{
				if (!(GetTabMainPart(((Object)val).name) != text))
				{
					flag = true;
					_addDragEventTo(val, name);
					_addTabEntry(((Component)val).gameObject, name);
					break;
				}
			}
		}
		if (flag)
		{
			_updateTabLayout();
		}
		static string GetTabMainPart(string text2)
		{
			return common_fix_for_tab_button.Aggregate(text2.ToLower(), (string current, string fix) => current.Replace(fix, ""));
		}
	}

	private static void _addDragEventTo(Button tab_entry, string pTabName)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		EventTrigger val = ((Component)tab_entry).GetComponent<EventTrigger>();
		if ((Object)(object)val == (Object)null)
		{
			val = ((Component)tab_entry).gameObject.AddComponent<EventTrigger>();
		}
		Entry val2 = new Entry();
		val2.eventID = (EventTriggerType)14;
		((UnityEvent<BaseEventData>)(object)val2.callback).AddListener((UnityAction<BaseEventData>)delegate
		{
			_setToValidPosition(tab_entry, pTabName);
		});
		val.triggers.Add(val2);
		val2 = new Entry();
		val2.eventID = (EventTriggerType)5;
		((UnityEvent<BaseEventData>)(object)val2.callback).AddListener((UnityAction<BaseEventData>)delegate
		{
			_onDragTabEntry(tab_entry, pTabName);
		});
		val.triggers.Add(val2);
	}

	private static void _setToValidPosition(Button pTabEntry, string pTabName)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_last_mouse_pos = Vector3.zero;
		_savePredefinedOrder();
		_updateTabLayout();
	}

	private static void _onDragTabEntry(Button pTabEntry, string pTabName)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		RectTransform tab_entry_rect = ((Component)pTabEntry).GetComponent<RectTransform>();
		Vector3 mousePosition = Input.mousePosition;
		Vector3 val = ((Transform)tab_entry_rect).parent.InverseTransformPoint(mousePosition);
		if (_last_mouse_pos == Vector3.zero)
		{
			_last_mouse_pos = val;
			return;
		}
		Vector3 delta = val - _last_mouse_pos;
		Vector3 current_pos = ((Transform)tab_entry_rect).localPosition;
		int index = tab_names.IndexOf(pTabName);
		if (index == 0)
		{
			if (delta.x > 0f)
			{
				Vector3 localPosition = ((Component)tab_entries[1]).transform.localPosition;
				if (current_pos.x > localPosition.x)
				{
					swap(left: false);
				}
			}
		}
		else if (index == tab_names.Count - 1)
		{
			if (delta.x < 0f)
			{
				Vector3 localPosition2 = ((Component)tab_entries[index - 1]).transform.localPosition;
				if (current_pos.x < localPosition2.x)
				{
					swap(left: true);
				}
			}
		}
		else if (delta.x < 0f)
		{
			Vector3 localPosition3 = ((Component)tab_entries[index - 1]).transform.localPosition;
			if (current_pos.x < localPosition3.x)
			{
				swap(left: true);
			}
		}
		else
		{
			Vector3 localPosition4 = ((Component)tab_entries[index + 1]).transform.localPosition;
			if (current_pos.x > localPosition4.x)
			{
				swap(left: false);
			}
		}
		_last_mouse_pos = val;
		((Transform)tab_entry_rect).localPosition = new Vector3(current_pos.x + delta.x, current_pos.y, current_pos.z);
		void swap(bool left)
		{
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			tab_names.Swap(index, index + ((!left) ? 1 : (-1)));
			tab_entries.Swap(index, index + ((!left) ? 1 : (-1)));
			_updateTabEntryRectAs(tab_entries[index], index);
			_updateTabEntryRectAs(tab_entries[index + ((!left) ? 1 : (-1))], index + ((!left) ? 1 : (-1)));
			Vector3 localPosition5 = ((Transform)tab_entry_rect).localPosition;
			if (Math.Abs(localPosition5.y - current_pos.y) > 0.01f)
			{
				delta.x = 0f;
				current_pos = localPosition5;
			}
		}
	}

	private static void _updateTabLayout()
	{
		_loadPredefinedOrder();
		int num = 0;
		foreach (Button tab_entry in tab_entries)
		{
			_updateTabEntryRectAs(tab_entry, num++);
		}
	}

	private static void _updateTabEntryRectAs(Button tab, int index)
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		int num = Math.Min(14, tab_entries.Count);
		int num2 = index % num - num / 2;
		int num3 = index / num;
		RectTransform component = ((Component)tab).gameObject.GetComponent<RectTransform>();
		float num4 = 2.0082f + (Mathf.Pow(0.79f, (float)num3) * 18f - 18f) / 2f + (1f - Mathf.Pow(0.79f, (float)num3)) / 0.20999998f * 18f;
		component.sizeDelta = new Vector2(Mathf.Pow(0.79f, (float)num3) * 43f, Mathf.Pow(0.79f, (float)num3) * 18f);
		((Transform)component).localPosition = new Vector3((float)num2 * 43f, num4, ((Transform)component).localPosition.z);
		try
		{
			RectTransform component2 = ((Component)((Component)tab).transform.Find("Icon")).gameObject.GetComponent<RectTransform>();
			component2.sizeDelta = new Vector2(Mathf.Pow(0.79f, (float)num3) * 33f, Mathf.Pow(0.79f, (float)num3) * 11f);
		}
		catch (Exception)
		{
		}
	}

	private static void _addTabEntry(GameObject pTabEntry, string pTabId)
	{
		if (tab_entries.Count % 2 == 0)
		{
			tab_entries.Insert(0, pTabEntry.GetComponent<Button>());
			tab_names.Insert(0, pTabId);
		}
		else
		{
			tab_entries.Add(pTabEntry.GetComponent<Button>());
			tab_names.Add(pTabId);
		}
		tab_names_set.Add(pTabId);
	}

	public static PowersTab CreateTab(string name, string pTitleKey, string pDescKey, Sprite pIcon, string pOptionDescKey = "hotkey_tip_tab_other")
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		GameObject tab_entry = Object.Instantiate<GameObject>(ResourcesFinder.FindResources<GameObject>("Button_Other")[0], tab_entry_container);
		Object.DestroyImmediate((Object)(object)tab_entry.GetComponent<GraphicRaycaster>());
		Object.DestroyImmediate((Object)(object)tab_entry.GetComponent<Canvas>());
		((Object)tab_entry).name = "Button_" + name;
		((Component)tab_entry.transform.Find("Icon")).GetComponent<Image>().sprite = pIcon;
		PowersTab tab = Object.Instantiate<PowersTab>(ResourcesFinder.FindResources<GameObject>("Tab_Other")[0].GetComponent<PowersTab>(), tab_container);
		((Object)tab).name = "Tab_" + name;
		Button tab_entry_button = tab_entry.GetComponent<Button>();
		tab_entry_button.onClick = new ButtonClickedEvent();
		((UnityEvent)tab_entry_button.onClick).AddListener((UnityAction)delegate
		{
			tab.showTab(tab_entry_button);
		});
		((UnityEvent)tab_entry_button.onClick).AddListener((UnityAction)delegate
		{
			tab_entry.GetComponent<ButtonSfx>().playSound();
		});
		TipButton component = tab_entry.GetComponent<TipButton>();
		component.textOnClick = pTitleKey;
		component.textOnClickDescription = pDescKey;
		component.text_description_2 = pOptionDescKey;
		for (int num = 7; num < ((Component)tab).transform.childCount; num++)
		{
			Object.Destroy((Object)(object)((Component)((Component)tab).transform.GetChild(num)).gameObject);
		}
		tab.powerButtons.Clear();
		PowerButton[] componentsInChildren = ((Component)tab).GetComponentsInChildren<PowerButton>();
		foreach (PowerButton powerButton in componentsInChildren)
		{
			if (!((Object)(object)powerButton == (Object)null) && !((Object)(object)powerButton.rect_transform == (Object)null))
			{
				tab.powerButtons.Add(powerButton);
			}
		}
		foreach (PowerButton powerButton2 in tab.powerButtons)
		{
			powerButton2.findNeighbours(tab.powerButtons);
		}
		_addDragEventTo(tab_entry_button, ((Object)tab).name);
		_addTabEntry(tab_entry, ((Object)tab).name);
		_updateTabLayout();
		((Component)tab).gameObject.SetActive(false);
		((Component)tab).gameObject.SetActive(true);
		return tab;
	}

	static TabManager()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		tab_entry_container = ((Component)CanvasMain.instance.canvas_ui).transform.Find("CanvasBottom/BottomElements/BottomElementsMover/TabsButtons");
		tab_container = ((Component)CanvasMain.instance.canvas_ui).transform.Find("CanvasBottom/BottomElements/BottomElementsMover/CanvasScrollView/Scroll View/Viewport/Content/buttons");
		tab_entries = new List<Button>(PowerTabController.instance._buttons);
		tab_names = new List<string>();
		tab_names_set = new HashSet<string>();
		_last_mouse_pos = Vector3.zero;
		TabMain = new TabMain();
		TabDrawing = new TabDrawing();
		TabKingdoms = new TabKingdoms();
		TabCreatures = new TabCreatures();
		TabNature = new TabNature();
		TabOther = new TabOther();
		common_fix_for_tab_button = new List<string> { "newtab", "new_tab", "tab", "newbutton", "new_button", "button", "additional", "_", " " };
	}
}
