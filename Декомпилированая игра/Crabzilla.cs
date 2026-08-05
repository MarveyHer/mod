using System;
using UnityEngine;

public class Crabzilla : BaseActorComponent
{
	internal const float HIGH_HP_THRESHOLD = 0.7f;

	internal const float MED_HP_THRESHOLD = 0.35f;

	private CrabLeg[] list_legs;

	private CrabLegJoint[] list_joints;

	private CrabLimbGroup[] list_limbs;

	private int active_limb = -1;

	public CrabBody mainBody;

	internal const float angle0_min = -20f;

	internal const float angle0_max = 30f;

	public GameObject armTarget;

	public GameObject mouthSprite;

	private SpriteAnimation mouthSpriteAnim;

	private bool _beam_enabled;

	private Vector3 bodyRotationTarget;

	private Vector3 bodyRotation;

	private float moveRotationLimit = 5f;

	private Vector3 bodyPosTarget;

	private Vector3 bodyPos;

	private float bodyPosTimeout;

	public CrabArm arm1;

	public CrabArm arm2;

	public float z_pos = 10f;

	internal override void create(Actor pActor)
	{
		base.create(pActor);
		base.transform.position = actor.current_position;
		bodyPos = new Vector3(0f, 27.8f, 0f);
		bodyPosTarget = new Vector3(0f, 27.8f, 0f);
		mouthSpriteAnim = mouthSprite.GetComponent<SpriteAnimation>();
		createLimbs();
		ControllableUnit.setControllableCreatureCrabzilla(actor);
		if (Config.isMobile)
		{
			WorldTip.showNow("crabzilla_controls_mobile", pTranslate: true, "top", 8f);
		}
		else
		{
			WorldTip.showNow("crabzilla_controls_pc", pTranslate: true, "top", 8f);
		}
		if (Config.joyControls)
		{
			UltimateJoystick.ResetJoysticks();
		}
		Vector3 tCur = base.transform.position;
		tCur.z = z_pos;
		base.transform.position = tCur;
		actor.current_position = base.transform.position;
	}

	public bool isBeamEnabled()
	{
		return _beam_enabled;
	}

	internal void legMoved()
	{
		if (!(bodyPosTimeout > 0f))
		{
			bodyPosTarget.y = 27.8f + Randy.randomFloat(-3f, 3f);
		}
	}

	public override void update(float pElapsed)
	{
		if (bodyPosTimeout > 0f)
		{
			bodyPosTimeout -= pElapsed;
		}
		arm1.update(pElapsed);
		arm2.update(pElapsed);
		CrabLeg[] array = list_legs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].update(pElapsed);
		}
		if (isAnyLimbFlickering())
		{
			list_limbs[active_limb].update(pElapsed);
		}
		bool tAttackPressed = ControllableUnit.isAttackPressedLeft();
		_beam_enabled = tAttackPressed;
		mouthSprite.SetActive(isBeamEnabled());
		if (mouthSprite.gameObject.activeSelf)
		{
			mouthSpriteAnim.update(pElapsed);
			MusicBox.inst.playDrawingSound("event:/SFX/UNIQUE/Crabzilla/CrabzillaVoice", actor.current_position.x, actor.current_position.y);
		}
		Vector2 tMovementVector = ControllableUnit.getMovementVector();
		if (!ControllableUnit.isMovementActionActive())
		{
			tMovementVector = Vector2.zero;
		}
		if (tMovementVector.x > 0f)
		{
			bodyRotationTarget.z = moveRotationLimit;
		}
		else if (tMovementVector.x < 0f)
		{
			bodyRotationTarget.z = 0f - moveRotationLimit;
		}
		else
		{
			bodyRotationTarget.z = 0f;
		}
		float tElapsed = World.world.elapsed * 60f;
		bodyRotation = Vector3.MoveTowards(bodyRotation, bodyRotationTarget, 0.7f * tElapsed);
		if (tMovementVector.y > 0f && bodyRotation.z > moveRotationLimit)
		{
			bodyRotation.z = moveRotationLimit;
		}
		else if (tMovementVector.y < 0f && bodyRotation.z < 0f - moveRotationLimit)
		{
			bodyRotation.z = 0f - moveRotationLimit;
		}
		bodyPos.z = 0f;
		bodyPosTarget.z = 0f;
		mainBody.transform.localRotation = Quaternion.Euler(bodyRotation);
		bodyPos = Vector2.MoveTowards(bodyPos, bodyPosTarget, 0.7f * tElapsed);
		mainBody.transform.localPosition = bodyPos;
		Vector3 tNewPos = base.transform.position;
		if (!object.Equals(tMovementVector, Vector2.zero))
		{
			Vector2 tCur = base.transform.position;
			tCur = Vector2.MoveTowards(tCur, tCur + tMovementVector * 0.2f * tElapsed, 1f * tElapsed);
			tNewPos = new Vector3(tCur.x, tCur.y);
			if (tNewPos.x < 0f)
			{
				tNewPos.x = 0f;
			}
			if (tNewPos.y < 0f)
			{
				tNewPos.y = 0f;
			}
			if (tNewPos.x > (float)MapBox.width)
			{
				tNewPos.x = MapBox.width;
			}
			if (tNewPos.y > (float)MapBox.height)
			{
				tNewPos.y = MapBox.height;
			}
			tNewPos.z = z_pos;
		}
		tNewPos.x += actor.shake_offset.x;
		tNewPos.y += actor.shake_offset.y;
		base.transform.position = tNewPos;
		actor.current_position = base.transform.position;
		actor.dirty_current_tile = true;
		updateArms();
	}

	private void updateArms()
	{
		if (Config.joyControls)
		{
			Vector2 tCur = armTarget.transform.position;
			float tVerticalAxis = ControllableUnit.getJoyAxisVerticalRight();
			float tHorizontalAxis = ControllableUnit.getJoyAxisHorizontalRight();
			Vector2 tPosVec = new Vector2(tHorizontalAxis, tVerticalAxis);
			if (!object.Equals(tPosVec, Vector2.zero))
			{
				tCur = Vector2.MoveTowards(tCur, tCur + tPosVec * 2f, 1f);
				if (Toolbox.DistVec3(tCur, base.transform.position) > 35f)
				{
					tCur = Vector2.MoveTowards(base.transform.position, tCur, 35f);
				}
			}
			armTarget.transform.position = tCur;
		}
		else
		{
			Vector3 tMousePos = World.world.getMousePos();
			armTarget.transform.position = tMousePos;
		}
	}

	private void createLimbs()
	{
		list_joints = GetComponentsInChildren<CrabLegJoint>(includeInactive: false);
		CrabLegJoint[] array = list_joints;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].crabzilla = this;
		}
		list_legs = GetComponentsInChildren<CrabLeg>(includeInactive: false);
		CrabLeg[] array2 = list_legs;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].crabzilla = this;
		}
		arm1.crabzilla = this;
		arm2.crabzilla = this;
		list_limbs = new CrabLimbGroup[Enum.GetNames(typeof(CrabLimb)).Length];
		for (int j = 0; j < list_limbs.Length; j++)
		{
			list_limbs[j] = new CrabLimbGroup((CrabLimb)j, actor);
		}
		list_limbs.Shuffle();
		array2 = list_legs;
		foreach (CrabLeg obj in array2)
		{
			obj.create();
			obj.update(World.world.delta_time);
		}
		array = list_joints;
		foreach (CrabLegJoint obj2 in array)
		{
			obj2.create();
			obj2.LateUpdate();
		}
		update(World.world.delta_time);
		array2 = list_legs;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].moveLeg();
		}
	}

	internal static bool getHit(BaseSimObject pSelf, BaseSimObject pAttackedBy = null, WorldTile pTile = null)
	{
		Actor a = pSelf.a;
		Crabzilla tZilla = a.getActorComponent<Crabzilla>();
		if (a.getHealthRatio() > 0.45f)
		{
			return true;
		}
		tZilla.ShowLimbDamage();
		return true;
	}

	public void ShowLimbDamage()
	{
		if (!isAnyLimbFlickering())
		{
			active_limb++;
			if (active_limb >= list_limbs.Length)
			{
				active_limb = 0;
				list_limbs.Shuffle();
			}
			actor.startShake(0.05f);
			list_limbs[active_limb].showDamage();
		}
	}

	private bool isAnyLimbFlickering()
	{
		if (active_limb == -1)
		{
			return false;
		}
		if (list_limbs[active_limb].IsFlickering())
		{
			return true;
		}
		return false;
	}
}
