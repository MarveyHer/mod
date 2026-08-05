using System;

public static class BuildingTweenExtension
{
	internal static void checkTweens(this Building pBuilding)
	{
		switch (pBuilding.animation_state)
		{
		case BuildingAnimationState.OnRuin:
			pBuilding.setScaleTween(1f, 0.1f, 0f, pBuilding.completeMakingRuin, iTween.easeInCubic);
			break;
		case BuildingAnimationState.OnRemove:
		{
			EasingFunction tEase = iTween.easeInBack;
			if (pBuilding.chopped)
			{
				tEase = iTween.easeInCubic;
				pBuilding.scale_helper.scale_use_x = true;
			}
			pBuilding.setScaleTween(1f, 0.5f, 0f, pBuilding.removeBuildingFinal, tEase, 1);
			if (pBuilding.asset.city_building)
			{
				pBuilding.startShake(0.5f);
			}
			break;
		}
		}
	}

	internal static void setScaleTween(this Building pBuilding, float pFrom = 0f, float pDuration = 0.2f, float pTarget = 1f, Action pActionOnComplete = null, EasingFunction pEase = null, int pPriority = 0)
	{
		BuildingTweenScaleHelper tHelper = pBuilding.scale_helper;
		if (!tHelper.active || tHelper.scale_final_action == null || !(tHelper.scale_last_priority >= (float)pPriority))
		{
			if (pEase == null)
			{
				pEase = iTween.easeOutBack;
			}
			tHelper.active = true;
			tHelper.scale_start = pFrom;
			tHelper.scale_target = pTarget;
			tHelper.scale_time = World.world.getCurSessionTime() + (double)pDuration;
			tHelper.scale_duration = pDuration;
			tHelper.scale_final_action = pActionOnComplete;
			tHelper.scale_ease = pEase;
			if (tHelper.scale_use_x)
			{
				pBuilding.current_scale.x = pBuilding.asset.scale_base.x * pFrom;
			}
			else
			{
				pBuilding.current_scale.y = pBuilding.asset.scale_base.y * pFrom;
			}
			pBuilding.batch.c_scale.Add(pBuilding);
		}
	}

	public static void checkFinalAction(this Building pBuilding)
	{
		pBuilding.scale_helper.scale_final_action?.Invoke();
		pBuilding.scale_helper.scale_final_action = null;
		pBuilding.scale_helper.angle_final_action?.Invoke();
		pBuilding.scale_helper.angle_final_action = null;
	}

	internal static void finishScaleTween(this Building pBuilding)
	{
		pBuilding.setAnimationState(BuildingAnimationState.Normal);
		BuildingTweenScaleHelper tHelper = pBuilding.scale_helper;
		tHelper.scale_time = World.world.getCurSessionTime() + (double)tHelper.scale_duration;
	}

	internal static void updateAngle(this Building pBuilding, float pElapsed)
	{
		if (pBuilding.current_rotation.z != pBuilding.scale_helper.angle_target)
		{
			BuildingTweenScaleHelper tHelper = pBuilding.scale_helper;
			tHelper.angle_time += pElapsed;
			if (tHelper.angle_time >= 1f)
			{
				tHelper.angle_time = 1f;
				pBuilding.batch.c_angle.Remove(pBuilding);
				pBuilding.batch.actions_to_run.Add(pBuilding.checkFinalAction);
			}
			float tAngle = iTween.easeInExpo(0f, 1f, tHelper.angle_time);
			pBuilding.current_rotation.Set(0f, 0f, tAngle * pBuilding.scale_helper.angle_target);
		}
	}

	internal static void updateScale(this Building pBuilding)
	{
		if (pBuilding.scale_helper.active)
		{
			BuildingTweenScaleHelper tHelper = pBuilding.scale_helper;
			double tTimeLeft = tHelper.scale_time - World.world.getCurSessionTime();
			float tScale = 1f;
			if (tTimeLeft <= 0.0)
			{
				tHelper.scale_time = World.world.getCurSessionTime() + (double)tHelper.scale_duration;
				tHelper.active = false;
				pBuilding.batch.actions_to_run.Add(pBuilding.checkFinalAction);
				pBuilding.batch.c_scale.Remove(pBuilding);
				tScale = tHelper.scale_target;
			}
			else
			{
				float tTimeSpent = (float)(((double)tHelper.scale_duration - tTimeLeft) / (double)tHelper.scale_duration);
				tScale = tHelper.scale_ease(tHelper.scale_start, tHelper.scale_target, tTimeSpent);
			}
			if (tHelper.scale_use_x)
			{
				pBuilding.current_scale.x = pBuilding.asset.scale_base.x * tScale;
			}
			else
			{
				pBuilding.current_scale.y = pBuilding.asset.scale_base.y * tScale;
			}
		}
	}
}
