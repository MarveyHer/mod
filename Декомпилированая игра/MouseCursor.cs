using UnityEngine;

public class MouseCursor : MonoBehaviour
{
	private const int CURSOR_DEFAULT_0 = 0;

	private const int CURSOR_DOWN_1 = 1;

	private const int CURSOR_UP_2 = 2;

	private const int CURSOR_HOLD_6 = 6;

	private const int CURSOR_DRAG_7 = 7;

	private const int CURSOR_PINKIE_8 = 8;

	private const int CURSOR_SPRINKLE_13 = 13;

	private const int CURSOR_DRAW_17 = 17;

	private const int CURSOR_ATTACK = 22;

	private const int CURSOR_INSPECT = 26;

	private const int CURSOR_OVER_UI = 27;

	private const int CLICK_FRAMES = 6;

	private const int UP_FRAMES = 4;

	private const int PINKIE_FRAMES = 5;

	private const int SPRINKLE_FRAMES = 4;

	private const int DRAW_FRAMES = 5;

	private const int ATTACK_FRAMES = 4;

	private const int ANIM_SPEED = 5;

	public Texture2D mouseCursorDefault;

	public Texture2D mouseCursorDown;

	public Texture2D mouseCursorUp1;

	public Texture2D mouseCursorUp2;

	public Texture2D mouseCursorUp3;

	public Texture2D mouseCursorUp4;

	public Texture2D mouseCursorHold;

	public Texture2D mouseCursorDrag;

	public Texture2D mouseCursorPinkie1;

	public Texture2D mouseCursorPinkie2;

	public Texture2D mouseCursorPinkie3;

	public Texture2D mouseCursorPinkie4;

	public Texture2D mouseCursorPinkie5;

	public Texture2D mouseCursorSprinkle1;

	public Texture2D mouseCursorSprinkle2;

	public Texture2D mouseCursorSprinkle3;

	public Texture2D mouseCursorSprinkle4;

	public Texture2D mouseCursorDraw1;

	public Texture2D mouseCursorDraw2;

	public Texture2D mouseCursorDraw3;

	public Texture2D mouseCursorDraw4;

	public Texture2D mouseCursorDraw5;

	public Texture2D mouseCursorAttack1;

	public Texture2D mouseCursorAttack2;

	public Texture2D mouseCursorAttack3;

	public Texture2D mouseCursorAttack4;

	public Texture2D mouseCursorInspect;

	public Texture2D mouseCursorOverUI;

	private static int _counter = 0;

	private static bool _pressed = false;

	private static int _pressing = 0;

	private static int _dragged = 0;

	private static bool _right = false;

	private static bool _middle = false;

	private static bool _anim_done = true;

	private static int _cur_cursor = -1;

	private static Texture2D[] _cursors;

	private static int _last_texture_id = -1;

	private static bool _can_drag = false;

	private static bool _selected_unit_attack = false;

	private static MouseHoldAnimation _mouse_hold_animation = MouseHoldAnimation.Default;

	private static Texture2D _selected_cursor_texture;

	private void Awake()
	{
		if (!Input.mousePresent)
		{
			Object.Destroy(this);
			return;
		}
		_cursors = new Texture2D[28]
		{
			mouseCursorDefault, mouseCursorDown, mouseCursorUp1, mouseCursorUp2, mouseCursorUp3, mouseCursorUp4, mouseCursorHold, mouseCursorDrag, mouseCursorPinkie1, mouseCursorPinkie2,
			mouseCursorPinkie3, mouseCursorPinkie4, mouseCursorPinkie5, mouseCursorSprinkle1, mouseCursorSprinkle2, mouseCursorSprinkle3, mouseCursorSprinkle4, mouseCursorDraw1, mouseCursorDraw2, mouseCursorDraw3,
			mouseCursorDraw4, mouseCursorDraw5, mouseCursorAttack1, mouseCursorAttack2, mouseCursorAttack3, mouseCursorAttack4, mouseCursorInspect, mouseCursorOverUI
		};
	}

	private void OnEnable()
	{
		setCursorDefault();
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
		{
			if (!_pressed)
			{
				_pressing = 0;
				_dragged = 0;
				_counter = 0;
			}
			_pressed = true;
			_anim_done = false;
			_right = Input.GetMouseButtonDown(1);
			_middle = Input.GetMouseButtonDown(2);
		}
		else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
		{
			if (!Input.anyKey)
			{
				_pressed = false;
				_pressing = 0;
				_dragged = 0;
				_right = Input.GetMouseButtonUp(1);
				_middle = Input.GetMouseButtonUp(2);
			}
			else
			{
				_right = Input.GetMouseButton(1);
				_middle = Input.GetMouseButton(2);
			}
		}
		else if (_pressed)
		{
			if (!Input.anyKey)
			{
				_pressed = false;
				_pressing = 0;
				_dragged = 0;
				_right = false;
				_middle = false;
			}
			else
			{
				_right = Input.GetMouseButton(1);
				_middle = Input.GetMouseButton(2);
				_pressing++;
				if (Config.isDraggingItem())
				{
					_dragged++;
				}
			}
		}
		_can_drag = World.world.isOverUI() || World.world.canDragMap();
		_selected_unit_attack = ControllableUnit.isControllingUnit();
		_mouse_hold_animation = World.world.getSelectedPowerHoldAnimation();
		if (_selected_unit_attack)
		{
			setCursor(22);
		}
		else if (_can_drag && Config.isDraggingItem() && _pressed && _dragged > 3)
		{
			if (_dragged < 10)
			{
				setCursor(6);
			}
			else
			{
				setCursor(7);
			}
		}
		else if (!_pressed && _anim_done)
		{
			if (shouldShowInspectCursor())
			{
				setCursorInspect();
			}
			else if (_right)
			{
				setCursor(8);
			}
			else
			{
				setCursorDefault();
			}
			_counter = 0;
		}
		else if (_can_drag && _pressed && _pressing > 3)
		{
			if (!World.world.isOverUI() || World.world.player_control.isPointerOverUIScroll())
			{
				if (_pressing < 10)
				{
					setCursor(6);
				}
				else
				{
					setCursor(7);
				}
			}
			else if (_right)
			{
				setCursor(8);
			}
			else
			{
				setCursor(1);
			}
		}
		else if (!_pressed && shouldShowInspectCursor())
		{
			setCursorInspect();
		}
		else if (_right)
		{
			int value = Mathf.CeilToInt(_counter / 5);
			if (value > 0 && value < 5)
			{
				setCursor(value + 8);
			}
			else
			{
				setCursor(8);
			}
			_counter++;
			if (_counter > 40)
			{
				_counter = 0;
				if (!_pressed)
				{
					_anim_done = true;
				}
			}
		}
		else
		{
			int tPauseFrames = 10;
			int tValue = 0;
			switch (_mouse_hold_animation)
			{
			case MouseHoldAnimation.Sprinkle:
				tPauseFrames = 4;
				tValue = Mathf.CeilToInt(_counter / 5) % 4;
				setCursor(13 + tValue);
				break;
			case MouseHoldAnimation.Draw:
				tPauseFrames = 5;
				tValue = Mathf.CeilToInt(_counter / 5) % 5;
				setCursor(17 + tValue);
				break;
			default:
				tPauseFrames = 6;
				tValue = Mathf.CeilToInt(_counter / 5);
				if (tValue < 6)
				{
					setCursor(tValue);
				}
				else
				{
					setCursorDefault();
				}
				break;
			}
			_counter++;
			if (_counter >= tPauseFrames * 5 && !_pressed)
			{
				_anim_done = true;
			}
		}
		renderCursor();
	}

	private void renderCursor()
	{
		int tTextureID = -1;
		if (_selected_cursor_texture != null)
		{
			tTextureID = _selected_cursor_texture.GetHashCode();
		}
		if (_last_texture_id != tTextureID)
		{
			Cursor.SetCursor(_selected_cursor_texture, Vector2.zero, CursorMode.Auto);
			_last_texture_id = tTextureID;
		}
	}

	private void setCursor(int pCursor = -1)
	{
		_cur_cursor = pCursor;
		_selected_cursor_texture = _cursors[pCursor];
		if (_selected_cursor_texture == null)
		{
			_selected_cursor_texture = mouseCursorDefault;
		}
	}

	private bool shouldShowInspectCursor()
	{
		if (UnitSelectionEffect.last_actor != null)
		{
			return true;
		}
		if (World.world.isOverUI())
		{
			return false;
		}
		if (ScrollWindow.isWindowActive())
		{
			return false;
		}
		if (World.world.quality_changer.isLowRes())
		{
			WorldTile tCursorTile = World.world.getMouseTilePosCachedFrame();
			if (tCursorTile == null)
			{
				return false;
			}
			if (Zones.showMapBorders())
			{
				MetaTypeAsset tMetaTypeAsset = World.world.getCachedMapMetaAsset();
				if (tMetaTypeAsset == null)
				{
					return false;
				}
				if (tMetaTypeAsset.check_tile_has_meta(tCursorTile.zone, tMetaTypeAsset, tMetaTypeAsset.getZoneOptionState()))
				{
					return true;
				}
			}
		}
		if (World.world.nameplate_manager.isOverNameplate())
		{
			return true;
		}
		return false;
	}

	private void setCursorDefault()
	{
		MapBox world = World.world;
		if ((object)world != null && world.isOverUiButton())
		{
			setCursor(27);
		}
		else
		{
			setCursor(0);
		}
	}

	private void setCursorInspect()
	{
		setCursor(26);
	}

	public static void debug(DebugTool pTool)
	{
		pTool.setText("_counter:", _counter, 0f, pShowBar: false, 0L);
		pTool.setText("_cur_cursor:", _cur_cursor, 0f, pShowBar: false, 0L);
		string tCursorName = "null";
		pTool.setText("_cur_cursor:", _cur_cursor switch
		{
			0 => "mouseCursorDefault", 
			1 => "mouseCursorDown", 
			2 => "mouseCursorUp1", 
			3 => "mouseCursorUp2", 
			4 => "mouseCursorUp3", 
			5 => "mouseCursorUp4", 
			6 => "mouseCursorHold", 
			7 => "mouseCursorDrag", 
			8 => "mouseCursorPinkie1", 
			9 => "mouseCursorPinkie2", 
			10 => "mouseCursorPinkie3", 
			11 => "mouseCursorPinkie4", 
			12 => "mouseCursorPinkie5", 
			13 => "mouseCursorSprinkle1", 
			14 => "mouseCursorSprinkle2", 
			15 => "mouseCursorSprinkle3", 
			16 => "mouseCursorSprinkle4", 
			17 => "mouseCursorDraw1", 
			18 => "mouseCursorDraw2", 
			19 => "mouseCursorDraw3", 
			20 => "mouseCursorDraw4", 
			21 => "mouseCursorDraw5", 
			_ => _cur_cursor.ToString(), 
		}, 0f, pShowBar: false, 0L);
		pTool.setText("_pressed:", _pressed, 0f, pShowBar: false, 0L);
		pTool.setText("_pressing:", _pressing, 0f, pShowBar: false, 0L);
		pTool.setText("_dragged:", _dragged, 0f, pShowBar: false, 0L);
		pTool.setText("_right:", _right, 0f, pShowBar: false, 0L);
		pTool.setText("_middle:", _middle, 0f, pShowBar: false, 0L);
		pTool.setText("_anim_done:", _anim_done, 0f, pShowBar: false, 0L);
		pTool.setSeparator();
		pTool.setText("_can_drag:", _can_drag, 0f, pShowBar: false, 0L);
		pTool.setText("_hold_animation:", _mouse_hold_animation, 0f, pShowBar: false, 0L);
		pTool.setSeparator();
		pTool.setText("_last_texture_id:", _last_texture_id, 0f, pShowBar: false, 0L);
		pTool.setText("_selected_cursor_texture:", _selected_cursor_texture.GetHashCode().ToString(), 0f, pShowBar: false, 0L);
	}
}
