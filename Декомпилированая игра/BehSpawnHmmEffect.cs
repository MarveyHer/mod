using ai.behaviours;

public class BehSpawnHmmEffect : BehaviourActionActor
{
	private int _amount;

	public BehSpawnHmmEffect(int pAmount = 1)
	{
		_amount = pAmount;
	}

	public override BehResult execute(Actor pActor)
	{
		_ = pActor.current_tile.zone;
		for (int i = 0; i < _amount; i++)
		{
			WorldTile tRandomTile = pActor.current_tile.neighbours.GetRandom();
			EffectsLibrary.spawnAt("fx_hmm", tRandomTile.posV, pActor.actor_scale);
		}
		return BehResult.Continue;
	}
}
