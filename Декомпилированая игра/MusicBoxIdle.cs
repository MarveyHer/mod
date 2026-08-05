using System.Collections.Generic;
using FMOD.Studio;

public class MusicBoxIdle
{
	private List<BaseSimObject> _toRemove = new List<BaseSimObject>();

	public Dictionary<BaseSimObject, EventInstance> currentAttachedSounds = new Dictionary<BaseSimObject, EventInstance>();

	private float _timer;

	public void update(float pElapsed)
	{
		if (_timer > 2f)
		{
			_timer -= pElapsed;
			return;
		}
		_timer = 2f;
		_toRemove.Clear();
		if (World.world.quality_changer.isLowRes())
		{
			clearAllSounds();
		}
		checkDeadSounds();
		if (!World.world.quality_changer.isLowRes())
		{
			updateBuildings();
		}
	}

	public virtual void checkDeadSounds()
	{
		foreach (BaseSimObject tObj in currentAttachedSounds.Keys)
		{
			bool toRemove = false;
			if (!tObj.isAlive())
			{
				toRemove = true;
			}
			if (toRemove)
			{
				_toRemove.Add(tObj);
			}
		}
		foreach (BaseSimObject tObj2 in _toRemove)
		{
			removeSound(tObj2);
		}
	}

	private void updateBuildings()
	{
	}

	private void removeSound(BaseSimObject pObj)
	{
		currentAttachedSounds.TryGetValue(pObj, out var tInstance);
		if (tInstance.isValid())
		{
			tInstance.stop(STOP_MODE.ALLOWFADEOUT);
			tInstance.release();
			currentAttachedSounds.Remove(pObj);
		}
	}

	private void playAttachedSound(BaseSimObject pObject, string pSound)
	{
		if (MusicBox.sounds_on)
		{
			currentAttachedSounds.TryGetValue(pObject, out var tInstance);
			if (!tInstance.isValid())
			{
				currentAttachedSounds.Add(pObject, tInstance);
			}
		}
	}

	private bool isPlaying(BaseSimObject pObject)
	{
		currentAttachedSounds.TryGetValue(pObject, out var tInstance);
		if (tInstance.isValid())
		{
			return true;
		}
		return false;
	}

	public void clearAllSounds()
	{
		foreach (EventInstance tInstance in currentAttachedSounds.Values)
		{
			tInstance.stop(STOP_MODE.ALLOWFADEOUT);
			tInstance.release();
		}
		currentAttachedSounds.Clear();
	}

	public int CountCurrentSounds()
	{
		return currentAttachedSounds.Count;
	}
}
