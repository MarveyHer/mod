using System.Collections.Generic;
using UnityEngine;

public class EffectInfinityCoin : BaseEffect
{
	private static List<Actor> _temp_list = new List<Actor>();

	private bool used;

	internal override void create()
	{
		base.create();
	}

	internal override void spawnOnTile(WorldTile pTile)
	{
		prepare(new Vector3(pTile.posV3.x, pTile.posV3.y - 1f), 0.25f);
	}

	internal override void prepare(Vector2 pVector, float pScale = 1f)
	{
		base.prepare(pVector, pScale);
		Vector3 tV = base.transform.localPosition;
		tV.z = -2f;
		current_position = tV;
		base.transform.localPosition = tV;
		used = false;
		World.world.startShake(0.1f, 0.02f, 3f);
	}

	private void Update()
	{
		if (sprite_animation.currentFrameIndex >= 32 && !used)
		{
			World.world.startShake(0.2f, 0.01f, 3f);
			used = true;
			Vector3 tVec = base.transform.localPosition;
			tVec.y += 2f;
			BaseEffect tEffect = EffectsLibrary.spawnAt("fx_boulder_impact", tVec, base.transform.localScale.x);
			if (tEffect != null)
			{
				tVec = tEffect.transform.localPosition;
				tVec.z = -1f;
				tEffect.transform.localPosition = tVec;
			}
			EffectsLibrary.spawnExplosionWave(tVec, 5f);
			doAction();
		}
	}

	private void doAction()
	{
		int tTotal = 0;
		int tToRemove = 0;
		List<Actor> tActorList = World.world.units.getSimpleList();
		for (int i = 0; i < tActorList.Count; i++)
		{
			Actor tActor = tActorList[i];
			if (tActor.isAlive() && !tActor.isFavorite() && !tActor.asset.ignored_by_infinity_coin)
			{
				tTotal++;
			}
		}
		tToRemove = ((tTotal % 2 != 0) ? (tTotal / 2 + 1) : (tTotal / 2));
		int tKilled = 0;
		_temp_list.AddRange(World.world.units);
		for (int j = 0; j < _temp_list.Count; j++)
		{
			_temp_list.ShuffleOne(j);
			Actor tAc = _temp_list[j];
			if (tToRemove == 0)
			{
				break;
			}
			if (tAc.isAlive() && !tAc.isFavorite() && !tAc.asset.ignored_by_infinity_coin && !tAc.is_invincible)
			{
				tKilled++;
				tToRemove--;
				tAc.getHitFullHealth(AttackType.Divine);
			}
		}
		WorldTip.addWordReplacement("$removed$", tKilled.ToString());
		WorldTip.showNow("infinity_coin_used", pTranslate: true, "top");
		_temp_list.Clear();
	}
}
