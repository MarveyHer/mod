using System;
using JetBrains.Annotations;
using NeoModLoader.services;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NeoModLoader.General;

public static class PowerButtonCreator
{
	public static PowerButton CreateWindowButton([NotNull] string pId, [NotNull] string pWindowId, Sprite pIcon, [CanBeNull] Transform pParent = null, Vector2 pLocalPosition = default(Vector2))
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		PowerButton powerButton = ResourcesFinder.FindResource<PowerButton>("world_laws");
		bool activeSelf = ((Component)powerButton).gameObject.activeSelf;
		if (activeSelf)
		{
			((Component)powerButton).gameObject.SetActive(false);
		}
		PowerButton powerButton2 = ((!((Object)(object)pParent == (Object)null)) ? Object.Instantiate<PowerButton>(powerButton, pParent) : Object.Instantiate<PowerButton>(powerButton));
		if (activeSelf)
		{
			((Component)powerButton).gameObject.SetActive(true);
		}
		((Object)powerButton2).name = pId;
		powerButton2.icon.sprite = pIcon;
		powerButton2.icon.overrideSprite = pIcon;
		powerButton2.open_window_id = pWindowId;
		powerButton2.type = PowerButtonType.Window;
		Transform transform = ((Component)powerButton2).transform;
		transform.localPosition = Vector2.op_Implicit(pLocalPosition);
		transform.localScale = Vector3.one;
		((Component)powerButton2).gameObject.SetActive(true);
		return powerButton2;
	}

	public static PowerButton CreateSimpleButton([NotNull] string pId, UnityAction pAction, Sprite pIcon, [CanBeNull] Transform pParent = null, Vector2 pLocalPosition = default(Vector2))
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		PowerButton powerButton = ResourcesFinder.FindResource<PowerButton>("world_laws");
		bool activeSelf = ((Component)powerButton).gameObject.activeSelf;
		if (activeSelf)
		{
			((Component)powerButton).gameObject.SetActive(false);
		}
		PowerButton powerButton2 = (((Object)(object)pParent == (Object)null) ? Object.Instantiate<PowerButton>(powerButton) : Object.Instantiate<PowerButton>(powerButton, pParent));
		if (activeSelf)
		{
			((Component)powerButton).gameObject.SetActive(true);
		}
		((Object)powerButton2).name = pId;
		powerButton2.icon.sprite = pIcon;
		powerButton2.icon.overrideSprite = pIcon;
		powerButton2.type = PowerButtonType.Library;
		if (pAction != null)
		{
			((UnityEvent)((Component)powerButton2).GetComponent<Button>().onClick).AddListener(pAction);
		}
		Transform transform = ((Component)powerButton2).transform;
		transform.localPosition = Vector2.op_Implicit(pLocalPosition);
		transform.localScale = Vector3.one;
		((Component)powerButton2).gameObject.SetActive(true);
		return powerButton2;
	}

	public static PowerButton CreateGodPowerButton(string pGodPowerId, Sprite pIcon, [CanBeNull] Transform pParent = null, Vector2 pLocalPosition = default(Vector2))
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		PowerButton powerButton = ResourcesFinder.FindResource<PowerButton>("inspect");
		bool activeSelf = ((Component)powerButton).gameObject.activeSelf;
		if (activeSelf)
		{
			((Component)powerButton).gameObject.SetActive(false);
		}
		PowerButton powerButton2 = (((Object)(object)pParent == (Object)null) ? Object.Instantiate<PowerButton>(powerButton) : Object.Instantiate<PowerButton>(powerButton, pParent));
		if (activeSelf)
		{
			((Component)powerButton).gameObject.SetActive(true);
		}
		((Object)powerButton2).name = pGodPowerId;
		powerButton2.icon.sprite = pIcon;
		powerButton2.icon.overrideSprite = pIcon;
		powerButton2.open_window_id = null;
		powerButton2.type = PowerButtonType.Active;
		Transform transform = ((Component)powerButton2).transform;
		transform.localPosition = Vector2.op_Implicit(pLocalPosition);
		transform.localScale = Vector3.one;
		((Component)powerButton2).gameObject.SetActive(true);
		return powerButton2;
	}

	public static PowerButton CreateToggleButton(string pGodPowerId, Sprite pIcon, [CanBeNull] Transform pParent = null, Vector2 pLocalPosition = default(Vector2), bool pNoAutoSetToggleAction = false)
	{
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		GodPower godPower = AssetManager.powers.get(pGodPowerId);
		if (godPower == null)
		{
			LogService.LogError("Cannot find GodPower with id " + pGodPowerId);
			return null;
		}
		if (godPower.toggle_action == null)
		{
			godPower.toggle_action = toggleOption;
		}
		else if (!pNoAutoSetToggleAction)
		{
			godPower.toggle_action = (PowerToggleAction)Delegate.Combine(godPower.toggle_action, new PowerToggleAction(toggleOption));
		}
		if (!PlayerConfig.dict.TryGetValue(godPower.toggle_name, out var value))
		{
			AssetManager.options_library.add(new OptionAsset
			{
				id = godPower.toggle_name,
				default_bool = false,
				type = OptionType.Bool
			});
			value = PlayerConfig.instance.data.add(new PlayerOptionData(godPower.toggle_name)
			{
				boolVal = false
			});
		}
		PowerButton powerButton = ResourcesFinder.FindResource<PowerButton>("map_kings_leaders");
		bool activeSelf = ((Component)powerButton).gameObject.activeSelf;
		if (activeSelf)
		{
			((Component)powerButton).gameObject.SetActive(false);
		}
		PowerButton powerButton2 = (((Object)(object)pParent == (Object)null) ? Object.Instantiate<PowerButton>(powerButton) : Object.Instantiate<PowerButton>(powerButton, pParent));
		if (activeSelf)
		{
			((Component)powerButton).gameObject.SetActive(true);
		}
		((Object)powerButton2).name = pGodPowerId;
		powerButton2.icon.sprite = pIcon;
		powerButton2.icon.overrideSprite = pIcon;
		powerButton2.open_window_id = null;
		powerButton2.type = PowerButtonType.Special;
		((Component)((Component)powerButton2).transform.Find("ToggleIcon")).GetComponent<ToggleIcon>().updateIcon(value.boolVal);
		LogService.LogInfo($"Set {((Object)powerButton2).name} toggle to {value.boolVal}");
		Transform transform = ((Component)powerButton2).transform;
		transform.localPosition = Vector2.op_Implicit(pLocalPosition);
		transform.localScale = Vector3.one;
		((Component)powerButton2).gameObject.SetActive(true);
		return powerButton2;
		static void toggleOption(string pPower)
		{
			GodPower godPower2 = AssetManager.powers.get(pPower);
			WorldTip.instance.showToolbarText(godPower2);
			if (!PlayerConfig.dict.TryGetValue(godPower2.toggle_name, out var value2))
			{
				value2 = new PlayerOptionData(godPower2.toggle_name)
				{
					boolVal = false
				};
				PlayerConfig.instance.data.add(value2);
			}
			value2.boolVal = !value2.boolVal;
			if (value2.boolVal && godPower2.map_modes_switch)
			{
				PowerLibrary.disableAllOtherMapModes(pPower);
			}
			PlayerConfig.saveData();
		}
	}

	public static PowersTab GetTab(string pId)
	{
		if (string.IsNullOrEmpty(pId))
		{
			return null;
		}
		Transform val = ((Component)CanvasMain.instance.canvas_ui).transform.Find("CanvasBottom/BottomElements/BottomElementsMover/CanvasScrollView/Scroll View/Viewport/Content/Power Tabs/" + pId);
		return ((Object)(object)val == (Object)null) ? null : ((Component)val).GetComponent<PowersTab>();
	}

	[Obsolete("Specifying a position vector has become useless in 0.50.5, tab order is now determined by sibling index.")]
	public static void AddButtonToTab(PowerButton button, PowersTab tab, Vector2 position, int? siblingIndex = null)
	{
		AddButtonToTab(button, tab, siblingIndex);
	}

	public static void AddButtonToTab(PowerButton button, PowersTab tab, int? siblingIndex = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Transform transform;
		(transform = ((Component)button).transform).SetParent(((Component)tab).transform);
		transform.localScale = Vector3.one;
		if (siblingIndex.HasValue)
		{
			transform.SetSiblingIndex(siblingIndex.Value);
		}
		tab._power_buttons.Add(button);
	}
}
