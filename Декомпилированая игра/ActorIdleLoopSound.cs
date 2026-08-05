using FMOD.Studio;

public class ActorIdleLoopSound
{
	internal EventInstance fmod_instance;

	private Actor _actor;

	public ActorIdleLoopSound(ActorAsset pAsset, Actor pActor)
	{
	}

	public void stop()
	{
		stopLoopCallback(_actor);
	}

	internal void stopLoopCallback(Actor pActor)
	{
		if (fmod_instance.isValid())
		{
			fmod_instance.stop(STOP_MODE.ALLOWFADEOUT);
			fmod_instance.release();
		}
	}
}
