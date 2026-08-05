using System.Collections.Generic;
using ai.behaviours;
using UnityEngine;

public class GodFinger : BaseActorComponent
{
	public const float FLYING_SPEED = 8f;

	public const int MAX_TARGET_TILES = 1200;

	internal GodPower god_power;

	internal string brush;

	internal float flying_target = 8f;

	private float _rotate_wiggle = 30f;

	internal static string[] power_over_water = Toolbox.splitStringIntoArray("tile_high_soil#10", "tile_soil#10", "tile_hills", "tile_mountains", "tile_summit", "shovel_plus");

	internal static string[] power_over_ground = Toolbox.splitStringIntoArray("seeds_candy", "seeds_corrupted", "seeds_crystal", "seeds_desert", "seeds_enchanted", "seeds_grass", "seeds_infernal", "seeds_jungle", "seeds_lemon", "seeds_mushroom", "seeds_permafrost", "seeds_savanna", "seeds_swamp", "seeds_birch", "seeds_maple", "seeds_flower", "seeds_garlic", "seeds_rocklands", "seeds_celestial", "seeds_singularity", "seeds_clover", "seeds_paradox", "fertilizer_plants#4", "fertilizer_trees#4");

	internal HashSet<WorldTile> target_tiles = new HashSet<WorldTile>(1800);

	internal FingerTarget finger_target;

	private SpriteAnimation fingerTip;

	internal Color debug_color;

	private static Color[] _random_colors = new Color[20]
	{
		Toolbox.color_green,
		Toolbox.color_red,
		Toolbox.color_blue,
		Toolbox.color_yellow,
		Toolbox.color_purple,
		Toolbox.color_fire,
		Toolbox.color_phenotype_green_0,
		Toolbox.color_phenotype_green_1,
		Toolbox.color_phenotype_green_2,
		Toolbox.color_phenotype_green_3,
		Toolbox.color_magenta_0,
		Toolbox.color_magenta_1,
		Toolbox.color_magenta_2,
		Toolbox.color_magenta_3,
		Toolbox.color_magenta_4,
		Toolbox.color_teal_0,
		Toolbox.color_teal_1,
		Toolbox.color_teal_2,
		Toolbox.color_teal_3,
		Toolbox.color_teal_4
	};

	internal bool is_drawing
	{
		get
		{
			if (actor.ai.hasTask())
			{
				return (actor.ai.action as BehFinger)?.drawing_action ?? false;
			}
			return false;
		}
	}

	internal bool drawing_over_water => finger_target == FingerTarget.Water;

	internal bool drawing_over_ground => finger_target == FingerTarget.Ground;

	internal override void create(Actor pActor)
	{
		base.create(pActor);
		debug_color = _random_colors.GetRandom();
		base.gameObject.name = "GF " + actor.getID();
		fingerTip = base.transform.Find("Tip").gameObject.GetComponent<SpriteAnimation>();
		fingerTip.gameObject.SetActive(value: false);
		actor.target_angle = Vector3.zero;
		actor.setFlying(pVal: true);
		actor.position_height = 8f;
	}

	internal void lightAction()
	{
		AchievementLibrary.god_finger_lightning.check();
	}

	public override void update(float pElapsed)
	{
		if (!actor.isAlive() || World.world.isPaused())
		{
			return;
		}
		bool tIsDrawing = is_drawing;
		bool tGonnaDraw = !tIsDrawing && flying_target < 2f;
		if (tIsDrawing)
		{
			actor.target_angle.z = Mathf.Clamp(actor.target_angle.z, 25f, 35f);
			if (actor.target_angle.z < _rotate_wiggle)
			{
				actor.target_angle.z += 100f * pElapsed;
			}
			else if (actor.target_angle.z > _rotate_wiggle)
			{
				actor.target_angle.z -= 100f * pElapsed;
			}
			else
			{
				_rotate_wiggle = Randy.randomInt(25, 35);
			}
			actor.rotation_cooldown = 300f;
		}
		else if (tGonnaDraw)
		{
			if (actor.target_angle.z < 30f)
			{
				actor.target_angle.z += 100f * pElapsed;
			}
			actor.rotation_cooldown = 300f;
		}
		else
		{
			actor.rotation_cooldown = 0f;
		}
		if (flying_target != actor.position_height)
		{
			actor.position_height = Mathf.MoveTowards(actor.position_height, flying_target, pElapsed * 8f);
		}
		fingerTip.gameObject.SetActive(tIsDrawing);
		if (tIsDrawing)
		{
			fingerTip.update(pElapsed);
			if (isInMapBounds(actor.current_position))
			{
				drawOnTile(actor.current_tile);
			}
		}
	}

	private bool isInMapBounds(Vector3 pPos)
	{
		if (pPos.x > 0f && pPos.y > 0f && pPos.x < (float)MapBox.width)
		{
			return pPos.y < (float)MapBox.height;
		}
		return false;
	}

	public void drawOnTile(WorldTile pTile)
	{
		World.world.conway_layer.checkKillRange(pTile.pos, 2);
		string current_brush = Config.current_brush;
		Config.current_brush = brush;
		if (god_power.click_power_action != null || god_power.click_power_brush_action != null)
		{
			if (god_power.click_power_brush_action != null)
			{
				god_power.click_power_brush_action(pTile, god_power);
			}
			else if (god_power.click_power_action != null)
			{
				god_power.click_power_action(pTile, god_power);
			}
		}
		if (god_power.click_action != null || god_power.click_brush_action != null)
		{
			if (god_power.click_brush_action != null)
			{
				god_power.click_brush_action(pTile, god_power.id);
			}
			else if (god_power.click_action != null)
			{
				god_power.click_action(pTile, god_power.id);
			}
		}
		World.world.loopWithBrush(pTile, Config.current_brush_data, clearTargets, "god_finger");
		World.world.loopWithBrush(pTile, Brush.get(2), fingerTile, "god_finger");
		Config.current_brush = current_brush;
	}

	public bool clearTargets(WorldTile pTile, string pPowerID)
	{
		target_tiles.Remove(pTile);
		return true;
	}

	public bool fingerTile(WorldTile pTile, string pPowerID)
	{
		pTile.doUnits(delegate(Actor pActor)
		{
			if (!pActor.asset.flag_finger && pActor.asset.can_be_killed_by_stuff)
			{
				pActor.getHitFullHealth(AttackType.Gravity);
			}
		});
		return true;
	}

	public override void Dispose()
	{
		target_tiles.Clear();
		finger_target = FingerTarget.None;
		base.Dispose();
	}

	internal static bool deathFlip(BaseSimObject pTarget, WorldTile pTile, float pElapsed)
	{
		Actor tActor = pTarget.a;
		if (tActor.isFalling())
		{
			tActor.updateFall();
			return true;
		}
		if (tActor.target_angle.z < 90f)
		{
			tActor.target_angle.z = Mathf.Lerp(tActor.target_angle.z, 90f, pElapsed * 4f);
			if (tActor.target_angle.z > 90f)
			{
				tActor.target_angle.z = 90f;
			}
			if (!tActor.is_visible)
			{
				tActor.updateRotation();
			}
			if (Mathf.Abs(tActor.current_rotation.z) >= 89f)
			{
				tActor.dieAndDestroy(AttackType.None);
				return true;
			}
		}
		tActor.updateDeadBlackAnimation(pElapsed);
		return true;
	}

	public static void debug_trail(GodFinger pFinger)
	{
		if (!DebugConfig.isOn(DebugOption.ShowGodFingerTargetting) || !MapBox.isRenderGameplay() || !pFinger.actor.is_visible)
		{
			return;
		}
		AiSystemActor tAI = pFinger.actor.ai;
		if (tAI.hasTask())
		{
			Color tColor = Toolbox.color_white;
			switch (tAI.task.id)
			{
			default:
				return;
			case "godfinger_move":
				tColor = Toolbox.color_blue;
				break;
			case "godfinger_find_target":
				tColor = Toolbox.color_red;
				break;
			case "godfinger_random_fun_move":
				tColor = Toolbox.color_green;
				break;
			case "godfinger_circle_move":
				tColor = Toolbox.color_purple;
				break;
			case "godfinger_circle_move_big":
				tColor = Toolbox.color_yellow;
				break;
			case "godfinger_circle_move_small":
				tColor = Toolbox.color_fire;
				break;
			}
			BaseEffect tEffects = EffectsLibrary.spawn("fx_weapon_particle", null, null, null, 0f, pFinger.fingerTip.transform.position.x, pFinger.fingerTip.transform.position.y);
			if (tEffects != null)
			{
				((StatusParticle)tEffects).spawnParticle(tEffects.transform.position, tColor);
			}
		}
	}
}
