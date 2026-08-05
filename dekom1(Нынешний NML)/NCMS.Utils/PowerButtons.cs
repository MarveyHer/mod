using System;
using System.Collections.Generic;
using NeoModLoader.General;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NCMS.Utils;

[Obsolete("Compatible Layer will not be maintained and be removed in the future")]
public class PowerButtons
{
	private static Dictionary<string, PowerButton> toggle_buttons = new Dictionary<string, PowerButton>();

	public static Dictionary<string, PowerButton> CustomButtons = new Dictionary<string, PowerButton>();

	public static Dictionary<string, bool> ToggleValues = new Dictionary<string, bool>();

	public static PowerButton CreateButton(string name, Sprite sprite, string title, string description, Vector2 position, ButtonType type = ButtonType.Click, Transform parent = null, UnityAction call = null)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Expected O, but got Unknown
		LM.AddToCurrentLocale(name, title);
		LM.AddToCurrentLocale(name + " Description", description);
		LM.ApplyLocale(pUpdateTexts: false);
		switch (type)
		{
		case ButtonType.Click:
		{
			PowerButton component = PowerButtonCreator.CreateSimpleButton(name, call, sprite, parent, position);
			CustomButtons[name] = component;
			return component;
		}
		case ButtonType.GodPower:
		{
			PowerButton component = PowerButtonCreator.CreateGodPowerButton(name, sprite, parent, position);
			if (call != null)
			{
				((UnityEvent)component._button.onClick).AddListener(call);
			}
			CustomButtons[name] = component;
			return component;
		}
		default:
			throw new ArgumentOutOfRangeException("type", type, null);
		case ButtonType.Toggle:
		{
			GameObject gameObject = ((Component)ResourcesFinder.FindResource<PowerButton>("map_kings_leaders")).gameObject;
			bool activeSelf = gameObject.activeSelf;
			gameObject.SetActive(false);
			GameObject val = (((Object)(object)parent == (Object)null) ? Object.Instantiate<GameObject>(gameObject) : Object.Instantiate<GameObject>(gameObject, parent));
			gameObject.SetActive(activeSelf);
			val.transform.localPosition = Vector2.op_Implicit(position);
			PowerButton component = val.GetComponent<PowerButton>();
			Button component2 = val.GetComponent<Button>();
			((UnityEventBase)component2.onClick).RemoveAllListeners();
			component.open_window_id = string.Empty;
			((Object)component).name = name;
			component.icon.sprite = sprite;
			component.type = PowerButtonType.Library;
			toggle_buttons[name] = component;
			ToggleValues.Add(name, value: false);
			((UnityEvent)component2.onClick).AddListener((UnityAction)delegate
			{
				ToggleButton(name);
			});
			if (call != null)
			{
				((UnityEvent)component2.onClick).AddListener(call);
			}
			((Component)val.transform.Find("ToggleIcon")).GetComponent<ToggleIcon>().updateIcon(false);
			val.SetActive(true);
			CustomButtons[name] = component;
			return component;
		}
		}
	}

	public static Button CreateTextButton(string name, string text, Vector2 position, Color color, Transform parent = null, UnityAction callback = null)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject(name, new Type[2]
		{
			typeof(Image),
			typeof(Button)
		});
		if ((Object)(object)parent != (Object)null)
		{
			val.transform.SetParent(parent);
		}
		val.transform.localScale = Vector3.one;
		val.transform.localPosition = Vector2.op_Implicit(position);
		val.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/special_buttonRed");
		((Graphic)val.GetComponent<Image>()).color = color;
		((Graphic)val.GetComponent<Image>()).SetNativeSize();
		((UnityEvent)val.GetComponent<Button>().onClick).AddListener(callback);
		GameObject val2 = new GameObject(name + "_text", new Type[2]
		{
			typeof(Text),
			typeof(Outline)
		});
		val2.transform.SetParent(val.transform);
		val2.transform.localScale = Vector3.one;
		val2.transform.localPosition = Vector3.zero;
		Text component = val2.GetComponent<Text>();
		component.font = Resources.Load<Font>("fonts/roboto-bold");
		((Graphic)component).color = Color.white;
		component.text = text;
		component.fontSize = 12;
		component.alignment = (TextAnchor)4;
		val2.GetComponent<RectTransform>().sizeDelta = val.GetComponent<RectTransform>().sizeDelta;
		Outline component2 = val2.GetComponent<Outline>();
		((Shadow)component2).effectDistance = new Vector2(1f, -1f);
		((Shadow)component2).effectColor = new Color(0f, 0f, 0f, 0.2f);
		return val.GetComponent<Button>();
	}

	public static void AddButtonToTab(PowerButton button, PowerTab tab, Vector2 position)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		PowerButtonCreator.AddButtonToTab(button, PowerButtonCreator.GetTab("Tab_" + tab), position);
	}

	public static bool GetToggleValue(string name)
	{
		if (!toggle_buttons.TryGetValue(name, out var value))
		{
			throw new Exception("Toggle button added by NCMS Method not found for " + name);
		}
		if ((Object)(object)((Component)value).transform.Find("ToggleIcon") == (Object)null)
		{
			throw new Exception("Toggle button added by NCMS Method is invalid for " + name);
		}
		GodPower godPower = AssetManager.powers.get(name);
		return (godPower == null) ? ToggleValues[name] : PlayerConfig.dict[godPower.toggle_name].boolVal;
	}

	public static void ToggleButton(string name)
	{
		if (toggle_buttons.TryGetValue(name, out var value))
		{
			Transform val = ((Component)value).transform.Find("ToggleIcon");
			if ((Object)(object)((Component)value).transform.Find("ToggleIcon") == (Object)null)
			{
				throw new Exception("Toggle button added by NCMS Method is invalid for " + name);
			}
			GodPower godPower = AssetManager.powers.get(name);
			if (godPower == null)
			{
				ToggleValues[name] = !ToggleValues[name];
				((Component)val).GetComponent<ToggleIcon>().updateIcon(ToggleValues[name]);
			}
			else
			{
				PlayerConfig.dict[godPower.toggle_name].boolVal = !PlayerConfig.dict[godPower.toggle_name].boolVal;
				value.checkToggleIcon();
			}
			return;
		}
		throw new Exception("Toggle button added by NCMS Method not found for " + name);
	}
}
