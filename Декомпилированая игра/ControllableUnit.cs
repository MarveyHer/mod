using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ControllableUnit
{
	private const float TOUCH_ATTACK_START_DELAY = 0.05f;

	private static Actor _unit_main = null;

	private static HashSet<Actor> _units = new HashSet<Actor>();

	private static Vector2 _movement_vector;

	private static Vector2 _click_vector;

	private static bool _action_pressed_jump = false;

	private static bool _action_pressed_dash = false;

	private static bool _action_pressed_backstep = false;

	private static bool _action_pressed_steal = false;

	private static bool _action_pressed_swear = false;

	private static bool _action_pressed_talk = false;

	private static bool _attack_pressed_button_left = false;

	private static bool _attack_pressed_button_right = false;

	private static bool _attack_just_pressed_button_left = false;

	private static bool _attack_just_pressed_button_right = false;

	private static float _touch_attack_started_at;

	private static bool _touch_attack_just_started;

	private static string[] _possessed_icons = new string[6] { "ui/Icons/iconBre", "ui/Icons/iconCrying", "ui/Icons/iconAngry", "ui/Icons/actor_traits/iconStupid", "ui/Icons/actor_traits/iconStrongMinded", "ui/Icons/iconDead" };

	public static bool isControllingUnit(Actor pUnit)
	{
		if (!isControllingUnit())
		{
			return false;
		}
		return _units.Contains(pUnit);
	}

	public static HashSet<Actor> getCotrolledUnits()
	{
		return _units;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool isControllingUnit()
	{
		return _units.Any();
	}

	public static int count()
	{
		return _units.Count;
	}

	public static bool isControllingCrabzilla()
	{
		if (isControllingUnit())
		{
			return _unit_main.asset.id == "crabzilla";
		}
		return false;
	}

	public static bool isControllingNormalUnits()
	{
		if (!isControllingUnit())
		{
			return false;
		}
		return _unit_main.asset.show_controllable_tip;
	}

	public static Actor getControllableUnit()
	{
		return _unit_main;
	}

	public static bool isAttackPressedLeft()
	{
		return _attack_pressed_button_left;
	}

	public static bool isAttackPressedRight()
	{
		return _attack_pressed_button_right;
	}

	public static bool isAttackJustPressedLeft()
	{
		return _attack_just_pressed_button_left;
	}

	public static bool isAttackJustPressedRight()
	{
		return _attack_just_pressed_button_right;
	}

	public static void setControllableCreatures(ListPool<Actor> pListActors)
	{
		foreach (ref Actor pListActor in pListActors)
		{
			setControllableCreature(pListActor);
		}
	}

	public static void setControllableCreatureAndSelected(Actor pActor)
	{
		using ListPool<Actor> tList = new ListPool<Actor>();
		foreach (Actor tActor in SelectedUnit.getAllSelected())
		{
			if (tActor.canBePossessed())
			{
				tList.Add(tActor);
			}
		}
		setControllableCreatures(tList);
		setControllableCreature(pActor);
		SelectedUnit.clear();
	}

	public static void setControllableCreatureCrabzilla(Actor pActor)
	{
		setControllableCreature(pActor);
	}

	public static void setControllableCreature(Actor pActor)
	{
		if (!pActor.canBePossessed())
		{
			return;
		}
		SelectedUnit.clear();
		_unit_main = pActor;
		_units.Add(pActor);
		addStatus(pActor);
		if (isControllingUnit())
		{
			Config.setWorldSpeed("x1");
			Config.paused = false;
		}
		if (Config.joyControls)
		{
			if (isControllingUnit())
			{
				World.world.joys.SetActive(value: true);
				if (isControllingCrabzilla())
				{
					UltimateJoystick.EnableJoystick("JoyRight");
					TouchPossessionController.instance.gameObject.SetActive(value: false);
				}
				else
				{
					UltimateJoystick.DisableJoystick("JoyRight");
					if (!InputHelpers.mouseSupported)
					{
						TouchPossessionController.instance.gameObject.SetActive(value: true);
					}
				}
			}
			else
			{
				World.world.joys.SetActive(value: false);
			}
			UltimateJoystick.ResetJoysticks();
		}
		else if (World.world.joys != null)
		{
			Object.Destroy(World.world.joys, 0.5f);
			World.world.joys = null;
		}
		_movement_vector = Vector2.zero;
		resetClickVector();
		_attack_pressed_button_left = false;
		_attack_pressed_button_right = false;
		_attack_just_pressed_button_left = false;
		_attack_just_pressed_button_right = false;
		if (isControllingNormalUnits() && InputHelpers.mouseSupported)
		{
			PossessionUI.toggle(pState: true);
		}
	}

	private static void resetClickVector()
	{
		_click_vector = Vector2.zero;
	}

	public static Vector2 getMovementVector()
	{
		return _movement_vector;
	}

	public static Vector2 getClickVector()
	{
		return _click_vector;
	}

	public static bool isActionPressedJump()
	{
		return _action_pressed_jump;
	}

	public static bool isActionPressedTalk()
	{
		return _action_pressed_talk;
	}

	public static bool isActionPressedDash()
	{
		return _action_pressed_dash;
	}

	public static bool isActionPressedBackstep()
	{
		return _action_pressed_backstep;
	}

	public static bool isActionPressedSteal()
	{
		return _action_pressed_steal;
	}

	public static bool isActionPressedSwear()
	{
		return _action_pressed_swear;
	}

	public static void remove(Actor pActor)
	{
		_units.Remove(pActor);
		if (_unit_main == pActor)
		{
			_unit_main = null;
			trySelectNewMain();
		}
	}

	private static void trySelectNewMain()
	{
		if (_units.Count == 0)
		{
			clear();
		}
		else
		{
			_unit_main = _units.GetRandom();
		}
	}

	public static void clear(bool pCallKill = true)
	{
		PossessionUI.toggle(pState: false);
		if (Config.joyControls)
		{
			World.world.joys.SetActive(value: false);
			UltimateJoystick.ResetJoysticks();
		}
		if (!isControllingUnit())
		{
			return;
		}
		foreach (ref Actor item in new ListPool<Actor>(getCotrolledUnits()))
		{
			Actor tActor = item;
			tActor.finishStatusEffect("possessed");
			tActor.cancelAllBeh();
			tActor.applyRandomForce();
			tActor.makeStunned(1f);
			tActor.makeConfused(6f);
			tActor.setPossessedMovement(pValue: false);
			if (pCallKill && tActor.asset.id == "crabzilla")
			{
				tActor.getHitFullHealth(AttackType.Divine);
			}
		}
		_unit_main = null;
		_units.Clear();
		World.world.selected_buttons.unselectAll();
	}

	public static void updateControllableUnit()
	{
		if (!isControllingUnit())
		{
			return;
		}
		if (InputHelpers.GetAnyMouseButtonUp())
		{
			foreach (Actor cotrolledUnit in getCotrolledUnits())
			{
				cotrolledUnit.resetAttackTimeout();
			}
		}
		updateCamera();
		updateMovementVector();
		updateClick();
		updateMouseAttackPosition();
		checkActions();
		checkPossessionStatus();
	}

	private static bool isAnyActionsPressed()
	{
		if (!_action_pressed_jump && !_action_pressed_dash && !_action_pressed_steal && !_action_pressed_swear && !_action_pressed_talk)
		{
			return _action_pressed_backstep;
		}
		return true;
	}

	private static void checkActions()
	{
		_action_pressed_jump = HotkeyLibrary.action_jump.isJustPressed() || TouchPossessionController.isActionPressedJump();
		_action_pressed_dash = HotkeyLibrary.action_dash.isJustPressed() || TouchPossessionController.isActionPressedDash();
		_action_pressed_backstep = HotkeyLibrary.action_backstep.isJustPressed() || TouchPossessionController.isActionPressedBackStep();
		bool tTouchAction = _attack_pressed_button_left;
		_action_pressed_steal = HotkeyLibrary.action_steal.isJustPressed() || (tTouchAction && TouchPossessionController.isSelectedActionSteal());
		_action_pressed_swear = HotkeyLibrary.action_swear.isJustPressed() || (tTouchAction && TouchPossessionController.isSelectedActionSwear());
		_action_pressed_talk = HotkeyLibrary.action_talk.isJustPressed() || (tTouchAction && TouchPossessionController.isSelectedActionTalk());
		_attack_just_pressed_button_right = _attack_just_pressed_button_right || (tTouchAction && TouchPossessionController.isSelectedActionKick());
	}

	private static void updateMouseAttackPosition()
	{
		resetClickVector();
		if (!InputHelpers.mouseSupported && Input.touchSupported)
		{
			_click_vector = getTouchAttackPosition();
		}
		else
		{
			_click_vector = World.world.getMousePos();
		}
	}

	private static bool getAttackTouch(out Touch pTouch)
	{
		pTouch = default(Touch);
		if (World.world.player_control.already_used_zoom)
		{
			return false;
		}
		UltimateJoystick ultimateJoystick = UltimateJoystick.GetUltimateJoystick("JoyLeft");
		bool tJoyActive = ultimateJoystick.GetJoystickState();
		int tTouchId = ultimateJoystick.getTouchId();
		bool tResult = false;
		Touch[] touches = Input.touches;
		for (int i = 0; i < touches.Length; i++)
		{
			Touch tTouch = touches[i];
			if (!World.world.isTouchOverUI(tTouch) && (!tJoyActive || tTouch.fingerId != tTouchId))
			{
				pTouch = tTouch;
				tResult = true;
				break;
			}
		}
		return tResult;
	}

	private static Vector2 getTouchAttackPosition()
	{
		Vector2 tResult = Vector2.zero;
		if (getAttackTouch(out var tTouch))
		{
			tResult = World.world.camera.ScreenToWorldPoint(tTouch.position);
		}
		return tResult;
	}

	private static void checkPossessionStatus()
	{
		if (!(_movement_vector != Vector2.zero) && !_attack_pressed_button_right && !_attack_pressed_button_left && !isAnyActionsPressed())
		{
			return;
		}
		foreach (Actor cotrolledUnit in getCotrolledUnits())
		{
			addStatus(cotrolledUnit);
			cotrolledUnit.stopSleeping();
			fixNextStep(cotrolledUnit);
		}
	}

	private static void fixNextStep(Actor pActor)
	{
		pActor.next_step_position = pActor.next_step_position_possession;
	}

	private static void addStatus(Actor pActor)
	{
		bool tHadStatus = false;
		if (pActor.hasStatus("possessed"))
		{
			tHadStatus = true;
		}
		pActor.addStatusEffect("possessed", 10f, pColorEffect: false);
		pActor.cancelAllBeh();
		if (!tHadStatus && pActor.hasTag("strong_mind"))
		{
			pActor.spawnSlashYell(World.world.getMousePos());
			pActor.addStatusEffect("swearing", 2f, pColorEffect: false);
			pActor.punchTargetAnimation(World.world.getMousePos(), pFlip: false, pReverse: false, -40f);
			string tRandomIcon = _possessed_icons.GetRandom();
			pActor.forceSocializeTopic(tRandomIcon);
		}
	}

	private static void updateClick()
	{
		if (Config.joyControls)
		{
			Touch tAttack;
			if (UltimateJoystick.GetUltimateJoystick("JoyRight").gameObject.activeSelf)
			{
				if (_attack_pressed_button_left && !UltimateJoystick.GetJoystickState("JoyRight"))
				{
					_attack_pressed_button_left = false;
				}
				else if (UltimateJoystick.GetTapCount("JoyRight"))
				{
					_attack_pressed_button_left = !_attack_pressed_button_left;
				}
			}
			else if (UltimateJoystick.GetJoystickState("JoyLeft") && Input.touchCount <= 1)
			{
				_attack_pressed_button_left = false;
				_attack_pressed_button_right = false;
				_attack_just_pressed_button_left = false;
				_attack_just_pressed_button_right = false;
			}
			else if (getAttackTouch(out tAttack))
			{
				if (tAttack.phase == TouchPhase.Began)
				{
					_touch_attack_started_at = Time.time;
					_touch_attack_just_started = true;
					return;
				}
				_attack_pressed_button_left = tAttack.phase == TouchPhase.Stationary || tAttack.phase == TouchPhase.Moved;
				_attack_pressed_button_right = false;
				_attack_just_pressed_button_left = _touch_attack_just_started;
				_attack_just_pressed_button_right = false;
				_touch_attack_just_started = false;
			}
			else
			{
				_attack_pressed_button_left = false;
				_attack_pressed_button_right = false;
				_attack_just_pressed_button_left = false;
				_attack_just_pressed_button_right = false;
			}
		}
		else
		{
			_attack_pressed_button_left = Input.GetMouseButton(0);
			_attack_pressed_button_right = Input.GetMouseButton(1);
			_attack_just_pressed_button_left = Input.GetMouseButtonDown(0);
			_attack_just_pressed_button_right = Input.GetMouseButtonDown(1);
		}
	}

	public static bool isMovementActionActive()
	{
		if (Config.joyControls)
		{
			UltimateJoystick tLeft = UltimateJoystick.GetUltimateJoystick("JoyLeft");
			if (tLeft == null)
			{
				return false;
			}
			if (tLeft.GetJoystickState())
			{
				return true;
			}
		}
		else
		{
			if (HotkeyLibrary.up.isHolding())
			{
				return true;
			}
			if (HotkeyLibrary.down.isHolding())
			{
				return true;
			}
			if (HotkeyLibrary.left.isHolding())
			{
				return true;
			}
			if (HotkeyLibrary.right.isHolding())
			{
				return true;
			}
		}
		return false;
	}

	private static void updateMovementVector()
	{
		if (Config.joyControls)
		{
			updateMovementVectorJoystick();
		}
		else
		{
			updateMovementVectorKeyboard();
		}
	}

	private static void updateMovementVectorJoystick()
	{
		if (isMovementActionActive())
		{
			float tVerticalJoyAxis = getJoyAxisVerticalLeft();
			float tHorizontalJoyAxis = getJoyAxisHorizontalLeft();
			_movement_vector.x = tHorizontalJoyAxis;
			_movement_vector.y = tVerticalJoyAxis;
		}
	}

	private static void updateMovementVectorKeyboard()
	{
		_movement_vector = Vector2.zero;
		if (HotkeyLibrary.up.isHolding())
		{
			_movement_vector.y = 1f;
		}
		else if (HotkeyLibrary.down.isHolding())
		{
			_movement_vector.y = -1f;
		}
		if (HotkeyLibrary.right.isHolding())
		{
			_movement_vector.x = 1f;
		}
		else if (HotkeyLibrary.left.isHolding())
		{
			_movement_vector.x = -1f;
		}
	}

	public static void updateCamera()
	{
		Vector2 tPos = _unit_main.current_position;
		Vector3 tCam = World.world.camera.transform.position;
		tCam.x = tPos.x;
		tCam.y = tPos.y;
		float tSpeed = 1f / World.world.camera.orthographicSize;
		World.world.camera.transform.position = Vector3.Lerp(World.world.camera.transform.position, tCam, tSpeed);
	}

	public static float getJoyAxisVerticalRight()
	{
		return UltimateJoystick.GetVerticalAxis("JoyRight");
	}

	public static float getJoyAxisHorizontalRight()
	{
		return UltimateJoystick.GetHorizontalAxis("JoyRight");
	}

	private static float getJoyAxisVerticalLeft()
	{
		return UltimateJoystick.GetVerticalAxis("JoyLeft");
	}

	private static float getJoyAxisHorizontalLeft()
	{
		return UltimateJoystick.GetHorizontalAxis("JoyLeft");
	}
}
