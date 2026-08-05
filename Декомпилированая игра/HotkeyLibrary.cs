using System;
using System.Collections.Generic;
using System.Globalization;
using Beebyte.Obfuscator;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
[ObfuscateLiterals]
public class HotkeyLibrary : AssetLibrary<HotkeyAsset>
{
	public static HotkeyAsset cancel;

	public static HotkeyAsset console;

	public static HotkeyAsset remove;

	public static HotkeyAsset pause;

	public static HotkeyAsset hide_ui;

	public static HotkeyAsset action_jump;

	public static HotkeyAsset action_dash;

	public static HotkeyAsset action_backstep;

	public static HotkeyAsset action_talk;

	public static HotkeyAsset action_steal;

	public static HotkeyAsset action_swear;

	public static HotkeyAsset left;

	public static HotkeyAsset right;

	public static HotkeyAsset up;

	public static HotkeyAsset down;

	public static HotkeyAsset next_unit_in_multi_selection;

	public static HotkeyAsset next_tab;

	public static HotkeyAsset prev_tab;

	public static HotkeyAsset zoom_in;

	public static HotkeyAsset zoom_out;

	public static HotkeyAsset zoom;

	public static HotkeyAsset world_speed;

	public static HotkeyAsset brush;

	public static HotkeyAsset follow_unit;

	public static HotkeyAsset control_unit;

	public static HotkeyAsset fullscreen_switch;

	public static HotkeyAsset many_mod;

	public static HotkeyAsset fast_civ_mod;

	public static KeyCode[] mod_keys = new KeyCode[0];

	private HotkeyAsset[] action_hotkeys = new HotkeyAsset[0];

	private Dictionary<string, float> holding_times = new Dictionary<string, float>();

	private bool holdingAnyModKey;

	private bool runModKeyCheck = true;

	private bool _last_input_active;

	private MetaType[] _meta_zones = new MetaType[10]
	{
		MetaType.Army,
		MetaType.Alliance,
		MetaType.Kingdom,
		MetaType.City,
		MetaType.Clan,
		MetaType.Religion,
		MetaType.Culture,
		MetaType.Language,
		MetaType.Family,
		MetaType.Subspecies
	};

	public override void init()
	{
		base.init();
		addHotkeysForUnitControlLayer();
		fullscreen_switch = add(new HotkeyAsset
		{
			id = "fullscreen_switch",
			default_key_1 = KeyCode.Return,
			default_key_mod_1 = KeyCode.LeftAlt,
			just_pressed_action = delegate
			{
				PlayerConfig.toggleFullScreen();
			}
		});
		console = add(new HotkeyAsset
		{
			id = "console",
			default_key_1 = KeyCode.Tilde,
			default_key_2 = KeyCode.BackQuote,
			check_controls_locked = true,
			just_pressed_action = delegate
			{
				if (EventSystem.current.currentSelectedGameObject == null)
				{
					World.world.console.Toggle();
				}
			}
		});
		cancel = add(new HotkeyAsset
		{
			id = "cancel",
			default_key_1 = KeyCode.Escape,
			just_pressed_action = escapeAction
		});
		add(new HotkeyAsset
		{
			id = "back",
			default_key_1 = KeyCode.Mouse3,
			just_pressed_action = backAction
		});
		pause = add(new HotkeyAsset
		{
			id = "pause",
			default_key_1 = KeyCode.Space,
			check_window_not_active = true,
			check_controls_locked = true,
			just_pressed_action = delegate
			{
				Config.paused = !Config.paused;
			}
		});
		hide_ui = add(new HotkeyAsset
		{
			id = "hide_ui",
			default_key_1 = KeyCode.H,
			check_window_not_active = true,
			check_controls_locked = true,
			just_pressed_action = delegate
			{
				Config.ui_main_hidden = !Config.ui_main_hidden;
			}
		});
		remove = add(new HotkeyAsset
		{
			id = "remove",
			default_key_1 = KeyCode.Delete,
			default_key_2 = KeyCode.Backspace,
			check_window_not_active = true,
			check_controls_locked = true,
			just_pressed_action = delegate
			{
				if (SelectedUnit.isSet())
				{
					SelectedUnit.killSelected();
				}
				else
				{
					string pID = "life_eraser";
					if (World.world.isSelectedPower("life_eraser"))
					{
						pID = "demolish";
					}
					World.world.selected_buttons.clickPowerButton(PowerButton.get(pID));
				}
			}
		});
		zoom = add(new HotkeyAsset
		{
			id = "zoom",
			use_mouse_wheel = true,
			holding_cooldown = 0f,
			check_window_not_active = true,
			check_controls_locked = true,
			allow_unit_control = true,
			holding_action = delegate(HotkeyAsset pAsset)
			{
				if (World.world.isPointerInGame() && (!World.world.isOverUI() || MoveCamera.inSpectatorMode()))
				{
					float y = Input.mouseScrollDelta.y;
					if (y < 0f)
					{
						MoveCamera.zoomOutWheel(pAsset);
					}
					else if (y > 0f)
					{
						MoveCamera.zoomInWheel(pAsset);
					}
				}
			}
		});
		world_speed = add(new HotkeyAsset
		{
			id = "world_speed",
			default_key_mod_1 = KeyCode.LeftControl,
			default_key_mod_2 = KeyCode.RightControl,
			default_key_mod_3 = KeyCode.LeftMeta,
			check_window_not_active = true,
			check_controls_locked = true,
			use_mouse_wheel = true,
			holding_cooldown = 0f,
			holding_action = delegate
			{
				float y = Input.mouseScrollDelta.y;
				WorldTimeScaleAsset time_scale_asset = Config.time_scale_asset;
				if (y < 0f)
				{
					Config.prevWorldSpeed();
				}
				else if (y > 0f)
				{
					Config.nextWorldSpeed();
				}
				if (time_scale_asset != Config.time_scale_asset)
				{
					string text = LocalizedTextManager.getText("changed_worldspeed");
					string text2 = null;
					text2 = ((Config.time_scale_asset.getLocaleID() == null) ? Toolbox.coloredText(Config.time_scale_asset.id, "#95DD5D") : Toolbox.coloredText(Config.time_scale_asset.getLocaleID(), "#95DD5D", pLocalize: true));
					text = text.Replace("$speed$", text2);
					WorldTip.instance.showToolbarText(text);
				}
			}
		});
		brush = add(new HotkeyAsset
		{
			id = "brush",
			default_key_mod_1 = KeyCode.LeftAlt,
			default_key_mod_2 = KeyCode.RightAlt,
			check_window_not_active = true,
			check_controls_locked = true,
			use_mouse_wheel = true,
			holding_cooldown = 0f,
			holding_action = delegate
			{
				float y = Input.mouseScrollDelta.y;
				string current_brush = Config.current_brush;
				if (y < 0f)
				{
					BrushLibrary.nextBrush();
				}
				else if (y > 0f)
				{
					BrushLibrary.previousBrush();
				}
				if (current_brush != Config.current_brush)
				{
					BrushData brushData = Brush.get(Config.current_brush);
					string localeID = brushData.getLocaleID();
					string text = LocalizedTextManager.getText("changed_brush");
					string text2 = Toolbox.coloredText(localeID, "#95DD5D", pLocalize: true);
					text2 = text2 + " (" + Toolbox.coloredText(brushData.size.ToString(), "#95DD5D") + ")";
					text = text.Replace("$brush$", text2);
					WorldTip.instance.showToolbarText(text);
				}
			}
		});
		many_mod = add(new HotkeyAsset
		{
			id = "many_mod",
			default_key_mod_1 = KeyCode.RightShift,
			default_key_mod_2 = KeyCode.LeftShift,
			disable_for_controlled_unit = true,
			check_only_not_controllable_unit = true
		});
		fast_civ_mod = add(new HotkeyAsset
		{
			id = "fast_civ_mod",
			default_key_mod_1 = KeyCode.RightControl,
			default_key_mod_2 = KeyCode.LeftControl
		});
		left = add(new HotkeyAsset
		{
			id = "left",
			default_key_1 = KeyCode.A,
			default_key_2 = KeyCode.LeftArrow,
			holding_action = MoveCamera.move,
			holding_cooldown = 0f,
			check_window_not_active = true,
			check_controls_locked = true,
			allow_unit_control = true
		});
		right = clone("right", "left");
		t.default_key_1 = KeyCode.D;
		t.default_key_2 = KeyCode.RightArrow;
		up = clone("up", "left");
		t.default_key_1 = KeyCode.W;
		t.default_key_2 = KeyCode.UpArrow;
		down = clone("down", "left");
		t.default_key_1 = KeyCode.S;
		t.default_key_2 = KeyCode.DownArrow;
		clone("fast_left", "left");
		t.default_key_mod_1 = KeyCode.RightShift;
		t.default_key_mod_2 = KeyCode.LeftShift;
		clone("fast_right", "right");
		t.default_key_mod_1 = KeyCode.RightShift;
		t.default_key_mod_2 = KeyCode.LeftShift;
		clone("fast_up", "up");
		t.default_key_mod_1 = KeyCode.RightShift;
		t.default_key_mod_2 = KeyCode.LeftShift;
		clone("fast_down", "down");
		t.default_key_mod_1 = KeyCode.RightShift;
		t.default_key_mod_2 = KeyCode.LeftShift;
		zoom_in = add(new HotkeyAsset
		{
			id = "zoom_in",
			default_key_1 = KeyCode.Q,
			default_key_2 = KeyCode.Plus,
			default_key_3 = KeyCode.KeypadPlus,
			check_window_not_active = true,
			check_controls_locked = true,
			holding_action = MoveCamera.zoomIn,
			holding_cooldown = 0f
		});
		zoom_out = add(new HotkeyAsset
		{
			id = "zoom_out",
			default_key_1 = KeyCode.E,
			default_key_2 = KeyCode.Minus,
			default_key_3 = KeyCode.KeypadMinus,
			check_window_not_active = true,
			check_controls_locked = true,
			holding_action = MoveCamera.zoomOut,
			holding_cooldown = 0f
		});
		add(new HotkeyAsset
		{
			id = "power_left",
			default_key_1 = KeyCode.LeftArrow,
			default_key_2 = KeyCode.A,
			default_key_mod_1 = KeyCode.LeftControl,
			default_key_mod_2 = KeyCode.LeftMeta,
			default_key_mod_3 = KeyCode.RightControl,
			check_window_not_active = true,
			check_controls_locked = true,
			just_pressed_action = powerMove,
			holding_action = powerMove
		});
		clone("power_right", "power_left");
		t.default_key_1 = KeyCode.RightArrow;
		t.default_key_2 = KeyCode.D;
		clone("power_up", "power_left");
		t.default_key_1 = KeyCode.UpArrow;
		t.default_key_2 = KeyCode.W;
		clone("power_down", "power_left");
		t.default_key_1 = KeyCode.DownArrow;
		t.default_key_2 = KeyCode.S;
		add(new HotkeyAsset
		{
			id = "toggle_power",
			default_key_1 = KeyCode.Return,
			default_key_2 = KeyCode.KeypadEnter,
			check_controls_locked = true,
			just_pressed_action = delegate
			{
				PowerButton activeButton = PowersTab.getActiveTab().getActiveButton();
				if (!(activeButton == null))
				{
					if (activeButton.godPower != null)
					{
						string text = activeButton.godPower.id;
						if (!(text == "clock"))
						{
							if (text == "pause")
							{
								activeButton.clickSpecial();
							}
							else
							{
								activeButton.godPower.select_button_action?.Invoke(activeButton.godPower.id);
								if (activeButton.godPower.toggle_action != null)
								{
									activeButton.godPower.toggle_action?.Invoke(activeButton.godPower.id);
									PowerButtonSelector.instance.checkToggleIcons();
								}
							}
						}
						else
						{
							Config.nextWorldSpeed(pCycle: true);
						}
					}
					else if (activeButton.type == PowerButtonType.Options)
					{
						activeButton.gameObject.GetComponent<Button>().onClick.Invoke();
					}
					else
					{
						activeButton.clickButton();
					}
				}
			}
		});
		clone("toggle_power2", "toggle_power");
		t.default_key_mod_1 = KeyCode.LeftControl;
		t.default_key_mod_2 = KeyCode.LeftMeta;
		next_tab = add(new HotkeyAsset
		{
			id = "next_tab",
			default_key_1 = KeyCode.Tab,
			check_window_not_active = true,
			check_controls_locked = true,
			check_no_multi_unit_selection = true,
			just_pressed_action = delegate
			{
				Button next = PowerTabController.instance.getNext(PowersTab.getActiveTab().name);
				PowersTab.showTabFromButton(next);
				TipButton component = next.gameObject.GetComponent<TipButton>();
				string pText = LocalizedTextManager.getText(component.textOnClick) + "\n" + LocalizedTextManager.getText(component.textOnClickDescription);
				WorldTip.instance.showToolbarText(pText);
			}
		});
		prev_tab = add(new HotkeyAsset
		{
			id = "prev_tab",
			default_key_1 = KeyCode.Tab,
			default_key_mod_1 = KeyCode.LeftShift,
			default_key_mod_2 = KeyCode.RightShift,
			check_window_not_active = true,
			check_controls_locked = true,
			check_no_multi_unit_selection = true,
			just_pressed_action = delegate
			{
				PowersTab.showTabFromButton(PowerTabController.instance.getPrev(PowersTab.getActiveTab().name));
			}
		});
		add(new HotkeyAsset
		{
			id = "hotkey_1",
			default_key_1 = KeyCode.Alpha1,
			default_key_2 = KeyCode.Keypad1,
			check_window_not_active = true,
			check_controls_locked = true,
			just_pressed_action = delegate(HotkeyAsset pAsset)
			{
				string text = pAsset.id;
				string hotkeyFromData = getHotkeyFromData(text);
				if (!string.IsNullOrEmpty(hotkeyFromData))
				{
					hotkeySelectNano(pAsset, hotkeyFromData);
				}
				else
				{
					string stringVal = PlayerConfig.dict[text].stringVal;
					hotkeySelectPower(pAsset, stringVal);
				}
			}
		});
		clone("hotkey_2", "hotkey_1");
		t.default_key_1 = KeyCode.Alpha2;
		t.default_key_2 = KeyCode.Keypad2;
		clone("hotkey_3", "hotkey_1");
		t.default_key_1 = KeyCode.Alpha3;
		t.default_key_2 = KeyCode.Keypad3;
		clone("hotkey_4", "hotkey_1");
		t.default_key_1 = KeyCode.Alpha4;
		t.default_key_2 = KeyCode.Keypad4;
		clone("hotkey_5", "hotkey_1");
		t.default_key_1 = KeyCode.Alpha5;
		t.default_key_2 = KeyCode.Keypad5;
		clone("hotkey_6", "hotkey_1");
		t.default_key_1 = KeyCode.Alpha6;
		t.default_key_2 = KeyCode.Keypad6;
		clone("hotkey_7", "hotkey_1");
		t.default_key_1 = KeyCode.Alpha7;
		t.default_key_2 = KeyCode.Keypad7;
		clone("hotkey_8", "hotkey_1");
		t.default_key_1 = KeyCode.Alpha8;
		t.default_key_2 = KeyCode.Keypad8;
		clone("hotkey_9", "hotkey_1");
		t.default_key_1 = KeyCode.Alpha9;
		t.default_key_2 = KeyCode.Keypad9;
		clone("hotkey_0", "hotkey_1");
		t.default_key_1 = KeyCode.Alpha0;
		t.default_key_2 = KeyCode.Keypad0;
		add(new HotkeyAsset
		{
			id = "save_hotkey_1",
			default_key_1 = KeyCode.Alpha1,
			default_key_2 = KeyCode.Keypad1,
			default_key_mod_1 = KeyCode.LeftControl,
			default_key_mod_2 = KeyCode.LeftMeta,
			check_window_not_active = true,
			check_controls_locked = true,
			just_pressed_action = delegate(HotkeyAsset pAsset)
			{
				if (SelectedObjects.isNanoObjectSet())
				{
					hotkeySaveTab(pAsset);
				}
				else
				{
					hotkeySavePower(pAsset);
				}
			}
		});
		clone("save_hotkey_2", "save_hotkey_1");
		t.default_key_1 = KeyCode.Alpha2;
		t.default_key_2 = KeyCode.Keypad2;
		clone("save_hotkey_3", "save_hotkey_1");
		t.default_key_1 = KeyCode.Alpha3;
		t.default_key_2 = KeyCode.Keypad3;
		clone("save_hotkey_4", "save_hotkey_1");
		t.default_key_1 = KeyCode.Alpha4;
		t.default_key_2 = KeyCode.Keypad4;
		clone("save_hotkey_5", "save_hotkey_1");
		t.default_key_1 = KeyCode.Alpha5;
		t.default_key_2 = KeyCode.Keypad5;
		clone("save_hotkey_6", "save_hotkey_1");
		t.default_key_1 = KeyCode.Alpha6;
		t.default_key_2 = KeyCode.Keypad6;
		clone("save_hotkey_7", "save_hotkey_1");
		t.default_key_1 = KeyCode.Alpha7;
		t.default_key_2 = KeyCode.Keypad7;
		clone("save_hotkey_8", "save_hotkey_1");
		t.default_key_1 = KeyCode.Alpha8;
		t.default_key_2 = KeyCode.Keypad8;
		clone("save_hotkey_9", "save_hotkey_1");
		t.default_key_1 = KeyCode.Alpha9;
		t.default_key_2 = KeyCode.Keypad9;
		clone("save_hotkey_0", "save_hotkey_1");
		t.default_key_1 = KeyCode.Alpha0;
		t.default_key_2 = KeyCode.Keypad0;
		add(new HotkeyAsset
		{
			id = "zone_type_previous",
			default_key_1 = KeyCode.Z,
			check_window_not_active = true,
			check_controls_locked = true,
			just_pressed_action = delegate
			{
				switchZones(-1);
			}
		});
		clone("zone_type_next", "zone_type_previous");
		t.just_pressed_action = delegate
		{
			switchZones(1);
		};
		t.default_key_1 = KeyCode.X;
		add(new HotkeyAsset
		{
			id = "zone_type_state_next",
			default_key_1 = KeyCode.C,
			check_window_not_active = true,
			check_controls_locked = true,
			just_pressed_action = delegate
			{
				toggleZones(1);
			}
		});
		clone("zone_type_state_previous", "zone_type_state_next");
		t.just_pressed_action = delegate
		{
			toggleZones(-1);
		};
		t.default_key_mod_1 = KeyCode.LeftControl;
		t.default_key_mod_2 = KeyCode.LeftMeta;
		follow_unit = add(new HotkeyAsset
		{
			id = "follow_unit",
			default_key_1 = KeyCode.F,
			check_window_not_active = false,
			check_controls_locked = true,
			just_pressed_action = delegate
			{
				Actor unit = SelectedUnit.unit;
				if (ScrollWindow.isWindowActive())
				{
					ScrollWindow currentWindow = ScrollWindow.getCurrentWindow();
					if (!(currentWindow.screen_id != "unit") && !currentWindow.GetComponent<UnitWindow>().name_input.inputField.isFocused && SelectedUnit.isSet())
					{
						World.world.followUnit(unit);
						ScrollWindow.hideAllEvent();
					}
				}
				else if (MapBox.isRenderGameplay())
				{
					Actor actorNearCursor = World.world.getActorNearCursor();
					if (actorNearCursor == null)
					{
						if (MoveCamera.hasFocusUnit())
						{
							MoveCamera.clearFocusUnitOnly();
						}
						else if (SelectedUnit.isSet())
						{
							World.world.followUnit(unit);
						}
					}
					else if (actorNearCursor.isCameraFollowingUnit())
					{
						MoveCamera.clearFocusUnitOnly();
					}
					else
					{
						World.world.followUnit(actorNearCursor);
					}
				}
			}
		});
		control_unit = add(new HotkeyAsset
		{
			id = "control_unit",
			default_key_1 = KeyCode.G,
			check_window_not_active = false,
			just_pressed_action = delegate
			{
				if (MoveCamera.hasFocusUnit())
				{
					World.world.move_camera.clearFocusUnitAndUnselect();
				}
				Actor unit = SelectedUnit.unit;
				if (ScrollWindow.isWindowActive())
				{
					ScrollWindow currentWindow = ScrollWindow.getCurrentWindow();
					if (!(currentWindow.screen_id != "unit") && !currentWindow.GetComponent<UnitWindow>().name_input.inputField.isFocused && SelectedUnit.isSet())
					{
						ControllableUnit.setControllableCreature(unit);
						ScrollWindow.hideAllEvent();
					}
				}
				else if (MapBox.isRenderGameplay())
				{
					Actor actorNearCursor = World.world.getActorNearCursor();
					if (ControllableUnit.isControllingUnit())
					{
						if (ControllableUnit.isControllingUnit(actorNearCursor))
						{
							ControllableUnit.clear();
							return;
						}
						if (actorNearCursor != null)
						{
							ControllableUnit.clear();
							ControllableUnit.setControllableCreature(actorNearCursor);
							return;
						}
						if (actorNearCursor == null)
						{
							ControllableUnit.clear();
							return;
						}
					}
					if (actorNearCursor == null)
					{
						if (SelectedUnit.isSet())
						{
							ControllableUnit.setControllableCreatureAndSelected(unit);
						}
					}
					else
					{
						ControllableUnit.setControllableCreatureAndSelected(actorNearCursor);
					}
				}
			}
		});
		add(new HotkeyAsset
		{
			id = "meta_window_previous",
			default_key_1 = KeyCode.LeftArrow,
			default_key_2 = KeyCode.Q,
			default_key_3 = KeyCode.A,
			just_pressed_action = delegate
			{
				MetaSwitchManager.switchWindows(MetaSwitchManager.Direction.Left);
			},
			check_controls_locked = true,
			check_window_active = true
		});
		clone("meta_window_next", "meta_window_previous");
		t.default_key_1 = KeyCode.RightArrow;
		t.default_key_2 = KeyCode.E;
		t.default_key_3 = KeyCode.D;
		t.just_pressed_action = delegate
		{
			MetaSwitchManager.switchWindows(MetaSwitchManager.Direction.Right);
		};
		add(new HotkeyAsset
		{
			id = "window_tab_next",
			default_key_1 = KeyCode.Tab,
			default_key_2 = KeyCode.S,
			default_key_3 = KeyCode.DownArrow,
			just_pressed_action = windowTabsSwitch,
			check_controls_locked = true,
			check_window_active = true
		});
		clone("window_tab_previous", "window_tab_next");
		t.default_key_mod_1 = KeyCode.LeftShift;
		t.default_key_mod_2 = KeyCode.RightShift;
		clone("window_tab_previous_2", "window_tab_next");
		t.default_key_1 = KeyCode.W;
		t.default_key_2 = KeyCode.UpArrow;
		t.default_key_3 = KeyCode.None;
	}

	private void addHotkeysForUnitControlLayer()
	{
		next_unit_in_multi_selection = add(new HotkeyAsset
		{
			id = "next_unit_in_multi_selection",
			default_key_1 = KeyCode.Tab,
			check_window_not_active = true,
			check_controls_locked = true,
			check_multi_unit_selection = true,
			ignore_same_key_diagnostic = true,
			just_pressed_action = delegate
			{
				SelectedUnit.nextMainUnit();
			}
		});
		action_jump = add(new HotkeyAsset
		{
			id = "action_jump",
			default_key_1 = KeyCode.Space,
			ignore_same_key_diagnostic = true,
			check_window_not_active = true,
			check_controls_locked = true,
			check_only_controllable_unit = true
		});
		action_dash = add(new HotkeyAsset
		{
			id = "action_dash",
			default_key_1 = KeyCode.LeftShift,
			default_key_2 = KeyCode.RightShift,
			ignore_same_key_diagnostic = true,
			check_window_not_active = true,
			check_controls_locked = true,
			ignore_mod_keys = true,
			check_only_controllable_unit = true
		});
		action_backstep = add(new HotkeyAsset
		{
			id = "action_backstep",
			default_key_1 = KeyCode.LeftControl,
			default_key_2 = KeyCode.RightControl,
			ignore_same_key_diagnostic = true,
			check_window_not_active = true,
			check_controls_locked = true,
			ignore_mod_keys = true,
			check_only_controllable_unit = true
		});
		action_swear = add(new HotkeyAsset
		{
			id = "action_swear",
			default_key_1 = KeyCode.F,
			ignore_same_key_diagnostic = true,
			check_window_not_active = true,
			check_controls_locked = true,
			check_only_controllable_unit = true
		});
		action_steal = add(new HotkeyAsset
		{
			id = "action_steal",
			default_key_1 = KeyCode.Q,
			ignore_same_key_diagnostic = true,
			check_window_not_active = true,
			check_controls_locked = true,
			check_only_controllable_unit = true
		});
		action_talk = add(new HotkeyAsset
		{
			id = "action_talk",
			default_key_1 = KeyCode.T,
			ignore_same_key_diagnostic = true,
			check_window_not_active = true,
			check_controls_locked = true,
			check_only_controllable_unit = true
		});
	}

	private void switchZones(int pIndexChange)
	{
		MetaType tMetaType = Zones.getCurrentMapBorderMode(pCheckOnlyOption: true);
		int tCurrentEnabledIndex = Array.IndexOf(_meta_zones, tMetaType);
		tCurrentEnabledIndex += pIndexChange;
		tCurrentEnabledIndex = Toolbox.loopIndex(tCurrentEnabledIndex, _meta_zones.Length);
		tMetaType = _meta_zones[tCurrentEnabledIndex];
		MetaTypeAsset tMetaTypeAsset = AssetManager.meta_type_library.getAsset(tMetaType);
		AssetManager.powers.get(tMetaTypeAsset.power_option_zone_id).toggle_action(tMetaTypeAsset.power_option_zone_id);
		PowerButtonSelector.instance.checkToggleIcons();
		GodPower tPower = AssetManager.powers.get(tMetaTypeAsset.power_option_zone_id);
		WorldTip.instance.showToolbarText(tPower);
	}

	private void toggleZones(int pIndexChange)
	{
		MetaType tMetaType = Zones.getCurrentMapBorderMode(pCheckOnlyOption: true);
		if (tMetaType != MetaType.None)
		{
			MetaTypeAsset tMetaTypeAsset = AssetManager.meta_type_library.getAsset(tMetaType);
			GodPower tPower = AssetManager.powers.get(tMetaTypeAsset.power_option_zone_id);
			if (tPower.multi_toggle)
			{
				tMetaTypeAsset.toggleOptionZone(tPower, pIndexChange, pDisable: false);
				PowerButtonSelector.instance.checkToggleIcons();
			}
		}
	}

	private void windowTabsSwitch(HotkeyAsset pAsset)
	{
		ScrollWindow tWindow = ScrollWindow.getCurrentWindow();
		List<WindowMetaTab> tContentTabs = tWindow.tabs.getContentTabs();
		if (tContentTabs.Count >= 2)
		{
			WindowMetaTab tActiveTab = tWindow.tabs.getActiveTab();
			int tIndex = tContentTabs.IndexOf(tActiveTab);
			switch (pAsset.id)
			{
			case "window_tab_next":
				tIndex++;
				break;
			case "window_tab_previous":
			case "window_tab_previous_2":
				tIndex--;
				break;
			}
			tIndex = Toolbox.loopIndex(tIndex, tContentTabs.Count);
			WindowMetaTab windowMetaTab = tContentTabs[tIndex];
			windowMetaTab.doAction();
			WorldTip.showNowTop(windowMetaTab.getWorldTipText(), pTranslate: false);
		}
	}

	private bool navigateWindowBack(HotkeyAsset pAsset)
	{
		if (!ScrollWindow.isWindowActive())
		{
			return false;
		}
		if (ScrollWindow.isAnimationActive())
		{
			ScrollWindow.finishAnimations();
		}
		WindowHistory.clickBack();
		return true;
	}

	private bool navigateTabBack(HotkeyAsset pAsset)
	{
		if (ScrollWindow.isWindowActive())
		{
			return false;
		}
		if (!SelectedTabsHistory.showPreviousTab())
		{
			return false;
		}
		return true;
	}

	private void backAction(HotkeyAsset pAsset)
	{
		if (!navigateWindowBack(pAsset) && !navigateTabBack(pAsset) && !PowersTab.getActiveTab().getAsset().tab_type_main)
		{
			PowerTabController.showMainTab();
		}
	}

	private void escapeAction(HotkeyAsset pAsset)
	{
		if (World.world.console.isActive())
		{
			World.world.console.Hide();
		}
		else if (ControllableUnit.isControllingUnit())
		{
			ControllableUnit.clear();
		}
		else if (World.world.tutorial.isActive())
		{
			World.world.tutorial.endTutorial();
		}
		else
		{
			if (MapBox.controlsLocked() || MapBox.isControllingUnit())
			{
				return;
			}
			if (MoveCamera.hasFocusUnit())
			{
				MoveCamera.clearFocusUnitOnly();
			}
			else
			{
				if (navigateWindowBack(pAsset))
				{
					return;
				}
				if (Config.ui_main_hidden)
				{
					Config.ui_main_hidden = false;
				}
				else if (!navigateTabBack(pAsset))
				{
					if (World.world.selected_buttons.selectedButton != null)
					{
						World.world.selected_buttons.unselectAll();
					}
					else if (SelectedUnit.isSet())
					{
						SelectedUnit.clear();
					}
					else if (PowersTab.isTabSelected())
					{
						World.world.selected_buttons.unselectTabs();
						SelectedObjects.unselectNanoObject();
					}
					else
					{
						ScrollWindow.showWindow("quit_game");
					}
				}
			}
		}
	}

	private void powerMove(HotkeyAsset pAsset)
	{
		PowersTab tActiveTab = PowersTab.getActiveTab();
		switch (pAsset.id)
		{
		case "power_left":
			tActiveTab.leftButton();
			break;
		case "power_right":
			tActiveTab.rightButton();
			break;
		case "power_up":
			tActiveTab.upButton();
			break;
		case "power_down":
			tActiveTab.downButton();
			break;
		}
	}

	public override void linkAssets()
	{
		base.linkAssets();
		HashSet<KeyCode> tModKeys = new HashSet<KeyCode>();
		HashSet<HotkeyAsset> tActionHotkeys = new HashSet<HotkeyAsset>();
		foreach (HotkeyAsset tAsset in list)
		{
			tAsset.overridden_key_1 = tAsset.default_key_1;
			tAsset.overridden_key_2 = tAsset.default_key_2;
			tAsset.overridden_key_3 = tAsset.default_key_3;
			tAsset.overridden_key_mod_1 = tAsset.default_key_mod_1;
			tAsset.overridden_key_mod_2 = tAsset.default_key_mod_2;
			tAsset.overridden_key_mod_3 = tAsset.default_key_mod_3;
			if (tAsset.default_key_mod_1 != KeyCode.None)
			{
				tModKeys.Add(tAsset.default_key_mod_1);
			}
			if (tAsset.default_key_mod_2 != KeyCode.None)
			{
				tModKeys.Add(tAsset.default_key_mod_2);
			}
			if (tAsset.default_key_mod_3 != KeyCode.None)
			{
				tModKeys.Add(tAsset.default_key_mod_3);
			}
			if (tAsset.just_pressed_action != null)
			{
				tActionHotkeys.Add(tAsset);
			}
			else if (tAsset.holding_action != null)
			{
				tActionHotkeys.Add(tAsset);
			}
		}
		mod_keys = tModKeys.ToArray();
		action_hotkeys = tActionHotkeys.ToArray();
	}

	public override void editorDiagnostic()
	{
		base.editorDiagnostic();
		Dictionary<string, HotkeyAsset> tKeys = new Dictionary<string, HotkeyAsset>();
		foreach (HotkeyAsset tAsset in list)
		{
			if (tAsset.ignore_same_key_diagnostic)
			{
				continue;
			}
			string tPre = "";
			if (tAsset.check_window_active)
			{
				tPre += "ui+";
			}
			using ListPool<string> tAllKeys = new ListPool<string>();
			bool tHasMod = tAsset.default_key_mod_1 != KeyCode.None;
			if (tAsset.default_key_1 != KeyCode.None)
			{
				if (tHasMod)
				{
					if (tAsset.default_key_mod_1 != KeyCode.None)
					{
						tAllKeys.Add(tPre + tAsset.default_key_1.ToString() + "+" + tAsset.default_key_mod_1);
					}
					if (tAsset.default_key_mod_2 != KeyCode.None)
					{
						tAllKeys.Add(tPre + tAsset.default_key_1.ToString() + "+" + tAsset.default_key_mod_2);
					}
					if (tAsset.default_key_mod_3 != KeyCode.None)
					{
						tAllKeys.Add(tPre + tAsset.default_key_1.ToString() + "+" + tAsset.default_key_mod_3);
					}
				}
				else
				{
					tAllKeys.Add(tPre + tAsset.default_key_1);
				}
			}
			if (tAsset.default_key_2 != KeyCode.None)
			{
				if (tHasMod)
				{
					if (tAsset.default_key_mod_1 != KeyCode.None)
					{
						tAllKeys.Add(tPre + tAsset.default_key_2.ToString() + "+" + tAsset.default_key_mod_1);
					}
					if (tAsset.default_key_mod_2 != KeyCode.None)
					{
						tAllKeys.Add(tPre + tAsset.default_key_2.ToString() + "+" + tAsset.default_key_mod_2);
					}
					if (tAsset.default_key_mod_3 != KeyCode.None)
					{
						tAllKeys.Add(tPre + tAsset.default_key_2.ToString() + "+" + tAsset.default_key_mod_3);
					}
				}
				else
				{
					tAllKeys.Add(tPre + tAsset.default_key_2);
				}
			}
			if (tAsset.default_key_3 != KeyCode.None)
			{
				if (tHasMod)
				{
					if (tAsset.default_key_mod_1 != KeyCode.None)
					{
						tAllKeys.Add(tPre + tAsset.default_key_3.ToString() + "+" + tAsset.default_key_mod_1);
					}
					if (tAsset.default_key_mod_2 != KeyCode.None)
					{
						tAllKeys.Add(tPre + tAsset.default_key_3.ToString() + "+" + tAsset.default_key_mod_2);
					}
					if (tAsset.default_key_mod_3 != KeyCode.None)
					{
						tAllKeys.Add(tPre + tAsset.default_key_3.ToString() + "+" + tAsset.default_key_mod_3);
					}
				}
				else
				{
					tAllKeys.Add(tPre + tAsset.default_key_3);
				}
			}
			foreach (ref string item in tAllKeys)
			{
				string tKey = item;
				if (tKeys.ContainsKey(tKey))
				{
					BaseAssetLibrary.logAssetError("<e>" + tAsset.id + "</e> has the same key as asset: <e>" + tKeys[tKey].id + "</e>", tKey);
				}
				else
				{
					tKeys.Add(tKey, tAsset);
				}
			}
		}
	}

	public static bool isHoldingControlForSelection()
	{
		if (!Input.GetKey(KeyCode.LeftControl))
		{
			return Input.GetKey(KeyCode.RightControl);
		}
		return true;
	}

	public static bool isHoldingAlt()
	{
		if (!Input.GetKey(KeyCode.LeftAlt))
		{
			return Input.GetKey(KeyCode.RightAlt);
		}
		return true;
	}

	public static bool isHoldingAnyMod()
	{
		if (AssetManager.hotkey_library == null)
		{
			return false;
		}
		return AssetManager.hotkey_library.isHoldingAnyModKey();
	}

	public void reset()
	{
		foreach (HotkeyAsset item in list)
		{
			item.overridden_key_1 = item.default_key_1;
			item.overridden_key_2 = item.default_key_2;
			item.overridden_key_3 = item.default_key_3;
			item.overridden_key_mod_1 = item.default_key_mod_1;
			item.overridden_key_mod_2 = item.default_key_mod_2;
			item.overridden_key_mod_3 = item.default_key_mod_3;
		}
	}

	public string replaceSpecialTextKeys(string pText)
	{
		if (!pText.Contains("$"))
		{
			return pText;
		}
		foreach (HotkeyAsset tAsset in list)
		{
			if (pText.Contains(tAsset.id))
			{
				string tKeyToReplace = "$" + tAsset.id + "$";
				string tHotKeyCode = tAsset.getLocalizedKeys();
				pText = pText.Replace(tKeyToReplace, tHotKeyCode);
				if (pText.Contains("$mouse_wheel$"))
				{
					string tLocalizedText = Toolbox.coloredText("mouse_wheel", "#95DD5D", pLocalize: true);
					pText = pText.Replace("$mouse_wheel$", tLocalizedText);
				}
				if (!pText.Contains("$"))
				{
					return pText;
				}
			}
		}
		return pText;
	}

	public bool isHoldingAnyModKey()
	{
		if (!Input.anyKey)
		{
			return false;
		}
		if (runModKeyCheck)
		{
			runModKeyCheck = false;
			holdingAnyModKey = false;
			KeyCode[] array = mod_keys;
			for (int i = 0; i < array.Length; i++)
			{
				if (Input.GetKey(array[i]))
				{
					holdingAnyModKey = true;
					break;
				}
			}
		}
		return holdingAnyModKey;
	}

	public void checkHotKeyActions()
	{
		runModKeyCheck = true;
		bool tScrollWheel = Input.mouseScrollDelta.y != 0f;
		if (!World.world.has_focus || (!Input.anyKey && !tScrollWheel))
		{
			return;
		}
		bool tIsInputActive = isInputActive();
		bool tEscThisFrame = _last_input_active && !tIsInputActive;
		_last_input_active = tIsInputActive;
		if (tIsInputActive || tEscThisFrame)
		{
			return;
		}
		bool tControlsLocked = MapBox.controlsLocked();
		bool tIsControllingUnit = MapBox.isControllingUnit();
		HotkeyAsset[] array = action_hotkeys;
		foreach (HotkeyAsset tAsset in array)
		{
			if ((tAsset.use_mouse_wheel && !tScrollWheel) || (tAsset.check_controls_locked && (tControlsLocked || (tIsControllingUnit && !tAsset.allow_unit_control))) || !tAsset.checkIsPossible())
			{
				continue;
			}
			if (tAsset.just_pressed_action != null && tAsset.isJustPressed())
			{
				tAsset.just_pressed_action(tAsset);
				if (tAsset.holding_action != null)
				{
					holding_times[tAsset.id] = tAsset.holding_cooldown_first_action;
				}
			}
			else if (tAsset.holding_action != null && tAsset.isHolding())
			{
				holding_times.TryGetValue(tAsset.id, out var tHoldingTime);
				tHoldingTime -= Time.deltaTime;
				if (tHoldingTime > 0f)
				{
					holding_times[tAsset.id] = tHoldingTime;
					continue;
				}
				tAsset.holding_action(tAsset);
				holding_times[tAsset.id] = tAsset.holding_cooldown;
			}
		}
	}

	private bool isInputActive()
	{
		if (!EventSystem.current.isFocused)
		{
			return false;
		}
		GameObject tSelected = EventSystem.current.currentSelectedGameObject;
		if (tSelected == null)
		{
			return false;
		}
		InputField tInput = tSelected.GetComponent<InputField>();
		if (tInput == null)
		{
			return false;
		}
		return tInput.isFocused;
	}

	public static bool allowedToUsePowers()
	{
		if (ScrollWindow.isWindowActive())
		{
			return false;
		}
		return true;
	}

	public void changeKey(HotkeyAsset pAsset, KeyCode pCode)
	{
	}

	public void load()
	{
	}

	public void hotkeySelectPower(HotkeyAsset pAsset, string pSelectPower)
	{
		if (!string.IsNullOrEmpty(pSelectPower) && AssetManager.powers.get(pSelectPower) == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(pSelectPower))
		{
			showTipNothing(pAsset);
			return;
		}
		PowerButton tPowerButton = PowerButton.get(pSelectPower);
		if (tPowerButton == null)
		{
			return;
		}
		if (tPowerButton.isSelected())
		{
			tPowerButton.cancelSelection();
			return;
		}
		tPowerButton.selectPowerTab(delegate
		{
			World.world.selected_buttons.clickPowerButton(tPowerButton);
			if (tPowerButton.isSelected())
			{
				WorldTip.instance.showToolbarText(tPowerButton.godPower);
			}
		});
	}

	public void hotkeySelectNano(HotkeyAsset pAsset, string pSelectNano)
	{
		if (string.IsNullOrEmpty(pSelectNano))
		{
			showTipNothing(pAsset);
			return;
		}
		string[] tSelectData = pSelectNano.Split("|");
		string tMetaTypeId = tSelectData[0];
		long tFirstNanoId = long.Parse(tSelectData[1]);
		MetaTypeAsset tAsset = AssetManager.meta_type_library.get(tMetaTypeId);
		NanoObject tObject = tAsset.get(tFirstNanoId);
		if (tObject.isRekt() && tSelectData.Length < 3)
		{
			showTipNothing(pAsset);
			return;
		}
		NanoObject tLastSelectedNano = SelectedObjects.getSelectedNanoObject();
		if (SelectedObjects.isNanoObjectSet() && SelectedObjects.getSelectedNanoObject() == tObject)
		{
			if (tLastSelectedNano == SelectedUnit.unit)
			{
				World.world.locatePosition(SelectedUnit.unit.current_position);
			}
			else if (tObject is IMetaObject)
			{
				Actor tActorTarget = (tObject as IMetaObject).getRandomUnit();
				if (tActorTarget != null)
				{
					World.world.locatePosition(tActorTarget.current_position);
				}
			}
			return;
		}
		if (World.world.isAnyPowerSelected())
		{
			PowerButtonSelector.instance.unselectAll();
		}
		SelectedObjects.unselectNanoObject();
		SelectedUnit.clear();
		if (tMetaTypeId == "unit")
		{
			if (tSelectData.Length >= 3)
			{
				using (ListPool<Actor> tPool = new ListPool<Actor>(tSelectData.Length))
				{
					for (int i = 1; i < tSelectData.Length; i++)
					{
						long tActorId = long.Parse(tSelectData[i]);
						Actor tActor = World.world.units.get(tActorId);
						if (!tActor.isRekt())
						{
							tPool.Add(tActor);
						}
					}
					if (tPool.Count > 0)
					{
						SelectedUnit.selectMultiple(tPool);
						SelectedObjects.setNanoObject(SelectedUnit.unit);
						if (tLastSelectedNano == SelectedUnit.unit)
						{
							World.world.locatePosition(SelectedUnit.unit.current_position);
						}
					}
					if (tPool.Count == 0)
					{
						showTipNothing(pAsset);
					}
					else if (tPool.Count == 1)
					{
						PowerTabController.showTabSelectedUnit();
					}
					else
					{
						PowerTabController.showTabMultipleUnits();
					}
					return;
				}
			}
			SelectedUnit.select(tObject as Actor);
			SelectedObjects.setNanoObject(SelectedUnit.unit);
			PowerTabController.showTabSelectedUnit();
		}
		else
		{
			tAsset.selectAndInspect(tObject, pFromNameplate: false, pCheckNameplate: false);
		}
	}

	public void showTipNothing(HotkeyAsset pAsset)
	{
		string tLocalizedHotkeyText = LocalizedTextManager.getText("hotkey_tip_empty_tip");
		tLocalizedHotkeyText = tLocalizedHotkeyText.Replace("$save_hotkey$", "$save_" + pAsset.id + "$");
		tLocalizedHotkeyText = AssetManager.hotkey_library.replaceSpecialTextKeys(tLocalizedHotkeyText);
		WorldTip.instance.showToolbarText(tLocalizedHotkeyText);
	}

	public void hotkeySavePower(HotkeyAsset pAsset)
	{
		string tSelectedPower = World.world.getSelectedPowerID();
		string tHotkey = pAsset.id.Replace("save_", "");
		string tLocalizedHotkeyText = "";
		if (string.IsNullOrEmpty(tSelectedPower))
		{
			tSelectedPower = string.Empty;
			tLocalizedHotkeyText = LocalizedTextManager.getText("hotkey_tip_cleared");
		}
		else
		{
			tLocalizedHotkeyText = LocalizedTextManager.getText("hotkey_tip_saved_power");
		}
		tLocalizedHotkeyText = tLocalizedHotkeyText.Replace("$save_hotkey$", "$" + tHotkey + "$");
		tLocalizedHotkeyText = AssetManager.hotkey_library.replaceSpecialTextKeys(tLocalizedHotkeyText);
		WorldTip.instance.showToolbarText(tLocalizedHotkeyText);
		PlayerConfig.dict[tHotkey].stringVal = tSelectedPower;
		PlayerConfig.saveData();
		getHotkeyFromData(tHotkey) = string.Empty;
	}

	public void hotkeySaveTab(HotkeyAsset pAsset)
	{
		string tHotkey = pAsset.id.Replace("save_", "");
		string tLocalizedHotkeyText = "";
		string tSelectedNano;
		if (!SelectedObjects.isNanoObjectSet())
		{
			tLocalizedHotkeyText = LocalizedTextManager.getText("hotkey_tip_cleared");
			tSelectedNano = string.Empty;
		}
		else
		{
			tLocalizedHotkeyText = LocalizedTextManager.getText("hotkey_tip_saved_nano");
			NanoObject tNano = SelectedObjects.getSelectedNanoObject();
			tSelectedNano = tNano.getMetaTypeAsset().id ?? "";
			if (SelectedUnit.isSet())
			{
				foreach (Actor tActor in SelectedUnit.getAllSelectedList())
				{
					tSelectedNano += $"|{tActor.id}";
				}
			}
			else
			{
				tSelectedNano += $"|{tNano.id}";
			}
		}
		tLocalizedHotkeyText = tLocalizedHotkeyText.Replace("$save_hotkey$", "$" + tHotkey + "$");
		tLocalizedHotkeyText = AssetManager.hotkey_library.replaceSpecialTextKeys(tLocalizedHotkeyText);
		getHotkeyFromData(tHotkey) = tSelectedNano;
		WorldTip.instance.showToolbarText(tLocalizedHotkeyText);
	}

	public ref string getHotkeyFromData(string pHotkeyId)
	{
		return pHotkeyId switch
		{
			"hotkey_1" => ref World.world.hotkey_tabs_data.hotkey_data_1, 
			"hotkey_2" => ref World.world.hotkey_tabs_data.hotkey_data_2, 
			"hotkey_3" => ref World.world.hotkey_tabs_data.hotkey_data_3, 
			"hotkey_4" => ref World.world.hotkey_tabs_data.hotkey_data_4, 
			"hotkey_5" => ref World.world.hotkey_tabs_data.hotkey_data_5, 
			"hotkey_6" => ref World.world.hotkey_tabs_data.hotkey_data_6, 
			"hotkey_7" => ref World.world.hotkey_tabs_data.hotkey_data_7, 
			"hotkey_8" => ref World.world.hotkey_tabs_data.hotkey_data_8, 
			"hotkey_9" => ref World.world.hotkey_tabs_data.hotkey_data_9, 
			"hotkey_0" => ref World.world.hotkey_tabs_data.hotkey_data_0, 
			_ => ref World.world.hotkey_tabs_data.hotkey_data_1, 
		};
	}

	public void initDebugHotkeys()
	{
		initDebugHotkeysBase();
		initUnitDebugHotkeys();
		initDebugWindowHotkeys();
		add(new HotkeyAsset
		{
			id = "debug_autosave",
			default_key_1 = KeyCode.S,
			default_key_mod_1 = KeyCode.LeftAlt,
			just_pressed_action = debugAutosave
		});
		add(new HotkeyAsset
		{
			id = "debug_next_test_map",
			default_key_1 = KeyCode.PageUp,
			just_pressed_action = delegate
			{
				if (!SmoothLoader.isLoading())
				{
					World.world.transition_screen.startTransition(TestMaps.loadNextMap);
				}
			}
		});
		add(new HotkeyAsset
		{
			id = "debug_prev_test_map",
			default_key_1 = KeyCode.PageDown,
			just_pressed_action = delegate
			{
				if (!SmoothLoader.isLoading())
				{
					World.world.transition_screen.startTransition(TestMaps.loadPrevMap);
				}
			}
		});
	}

	private void initDebugHotkeysBase()
	{
		add(new HotkeyAsset
		{
			id = "export_unit_sprites",
			default_key_1 = KeyCode.Y,
			check_window_not_active = true,
			check_controls_locked = true,
			just_pressed_action = delegate
			{
				WorldTip.instance.showToolbarText("Exporting unit sprites");
				AssetManager.dynamic_sprites_library.export();
			}
		});
		add(new HotkeyAsset
		{
			id = "autotester",
			default_key_1 = KeyCode.U,
			check_window_not_active = true,
			check_controls_locked = true,
			just_pressed_action = delegate
			{
				World.world.auto_tester.toggleAutoTester();
			}
		});
		add(new HotkeyAsset
		{
			id = "test_zones_border_growth",
			default_key_1 = KeyCode.O,
			check_window_not_active = true,
			check_controls_locked = true,
			just_pressed_action = delegate
			{
				DebugZonesTool.actionGrowBorder();
			}
		});
		add(new HotkeyAsset
		{
			id = "test_zones_abandon_zones",
			default_key_1 = KeyCode.P,
			check_window_not_active = true,
			check_controls_locked = true,
			just_pressed_action = delegate
			{
				WorldTile[] tiles_list = World.world.tiles_list;
				foreach (WorldTile pTile in tiles_list)
				{
					World.world.buildings.addBuilding("poop", pTile);
				}
			}
		});
		add(new HotkeyAsset
		{
			id = "test_colors",
			default_key_1 = KeyCode.R,
			check_window_not_active = true,
			check_controls_locked = true,
			just_pressed_action = delegate
			{
				foreach (Kingdom kingdom in World.world.kingdoms)
				{
					kingdom.generateBanner();
					ColorAsset random = AssetManager.kingdom_colors_library.list.GetRandom();
					kingdom.data.setColorID(AssetManager.kingdom_colors_library.list.IndexOf(random));
					if (kingdom.updateColor(random))
					{
						World.world.zone_calculator.dirtyAndClear();
					}
				}
			}
		});
	}

	private void initDebugWindowHotkeys()
	{
		add(new HotkeyAsset
		{
			id = "debug_building_shadow_x_increase",
			default_key_1 = KeyCode.X,
			default_key_mod_1 = KeyCode.LeftControl,
			just_pressed_action = debugShadow,
			check_controls_locked = true,
			check_window_active = true,
			check_debug_active = true
		});
		clone("debug_building_shadow_x_reduce", "debug_building_shadow_x_increase");
		t.default_key_mod_1 = KeyCode.LeftShift;
		clone("debug_building_shadow_y_increase", "debug_building_shadow_x_increase");
		t.default_key_1 = KeyCode.Y;
		clone("debug_building_shadow_y_reduce", "debug_building_shadow_y_increase");
		t.default_key_mod_1 = KeyCode.LeftShift;
		clone("debug_building_shadow_distortion_increase", "debug_building_shadow_x_increase");
		t.default_key_1 = KeyCode.D;
		clone("debug_building_shadow_distortion_reduce", "debug_building_shadow_distortion_increase");
		t.default_key_mod_1 = KeyCode.LeftShift;
	}

	private void initUnitDebugHotkeys()
	{
		add(new HotkeyAsset
		{
			id = "debug_unit_set_task",
			default_key_1 = KeyCode.V,
			default_key_mod_1 = KeyCode.LeftControl,
			check_window_not_active = true,
			check_controls_locked = true,
			check_render_gameplay = true,
			check_debug_active = true,
			just_pressed_action = delegate
			{
				if (DebugConfig.isOn(DebugOption.DebugUnitHotkeys))
				{
					World.world.getActorNearCursor()?.addStatusEffect("budding");
				}
			}
		});
		add(new HotkeyAsset
		{
			id = "debug_general_key",
			default_key_1 = KeyCode.N,
			check_debug_active = true,
			just_pressed_action = delegate
			{
				if (!DebugConfig.isOn(DebugOption.DebugUnitHotkeys) || !SelectedUnit.isSet())
				{
					return;
				}
				using ListPool<Actor> listPool = new ListPool<Actor>(SelectedUnit.getAllSelected());
				foreach (ref Actor item in listPool)
				{
					item.getHitFullHealth(AttackType.Divine);
				}
			}
		});
		add(new HotkeyAsset
		{
			id = "debug_monolith",
			default_key_1 = KeyCode.M,
			default_key_mod_1 = KeyCode.LeftControl,
			check_window_not_active = true,
			check_controls_locked = true,
			check_render_gameplay = true,
			check_debug_active = true,
			just_pressed_action = delegate
			{
				if (!DebugConfig.isOn(DebugOption.DebugMonolith))
				{
					return;
				}
				foreach (Building current in World.world.buildings)
				{
					if (current.asset.id == "monolith")
					{
						BuildingMonolith component_monolith = current.component_monolith;
						component_monolith.doMonolithAction(component_monolith.building.current_tile, pForce: true);
					}
				}
			}
		});
	}

	private void debugAutosave(HotkeyAsset pAsset)
	{
		if (Config.isEditor)
		{
			AutoSaveManager.autoSave(pSkipDelete: true, pForce: true);
		}
	}

	private void debugShadow(HotkeyAsset pAsset)
	{
		if (!DebugConfig.isOn(DebugOption.DebugWindowHotkeys) || ScrollWindow.getCurrentWindow().name != "building_asset")
		{
			return;
		}
		BuildingAsset tAsset = BaseDebugAssetWindow<BuildingAsset, BuildingDebugAssetElement>.current_element.asset;
		if (tAsset.shadow)
		{
			switch (pAsset.id)
			{
			case "debug_building_shadow_x_increase":
				tAsset.shadow_bound.x += 0.05f;
				break;
			case "debug_building_shadow_x_reduce":
				tAsset.shadow_bound.x -= 0.05f;
				break;
			case "debug_building_shadow_y_increase":
				tAsset.shadow_bound.y += 0.05f;
				break;
			case "debug_building_shadow_y_reduce":
				tAsset.shadow_bound.y -= 0.05f;
				break;
			case "debug_building_shadow_distortion_increase":
				tAsset.shadow_distortion += 0.05f;
				break;
			case "debug_building_shadow_distortion_reduce":
				tAsset.shadow_distortion -= 0.05f;
				break;
			}
			Debug.Log("t.setShadow(" + tAsset.shadow_bound.x.ToString(CultureInfo.InvariantCulture) + "f, " + tAsset.shadow_bound.y.ToString(CultureInfo.InvariantCulture) + "f, " + tAsset.shadow_distortion.ToString(CultureInfo.InvariantCulture) + "f);");
			BuildingAssetWindow.reloadSprites();
		}
	}

	public void debug(DebugTool pTool)
	{
		foreach (HotkeyAsset tAsset in list)
		{
			if (tAsset.just_pressed_action == null && tAsset.holding_action == null)
			{
				if (tAsset.isJustPressed())
				{
					pTool.setText(tAsset.id, "just_pressed", 0f, pShowBar: false, 0L);
				}
				if (tAsset.isHolding())
				{
					pTool.setText(tAsset.id, "holding", 0f, pShowBar: false, 0L);
				}
			}
		}
	}
}
