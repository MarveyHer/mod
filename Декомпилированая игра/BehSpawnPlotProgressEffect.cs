using ai.behaviours;
using UnityEngine;

public class BehSpawnPlotProgressEffect : BehaviourActionActor
{
	private int _amount;

	public BehSpawnPlotProgressEffect(int pAmount = 1)
	{
		_amount = pAmount;
	}

	public override BehResult execute(Actor pActor)
	{
		_ = pActor.current_tile.zone;
		for (int i = 0; i < _amount; i++)
		{
			Vector3 tPos = pActor.current_position;
			tPos.y += 5f * pActor.actor_scale;
			tPos.y += Randy.randomFloat((0f - pActor.actor_scale) * 3f, pActor.actor_scale * 3f);
			tPos.x += Randy.randomFloat((0f - pActor.actor_scale) * 2f, pActor.actor_scale * 2f);
			_ = EffectsLibrary.spawnAt("fx_plot_progress", tPos, pActor.actor_scale * 0.8f) == null;
		}
		return BehResult.Continue;
	}
}
