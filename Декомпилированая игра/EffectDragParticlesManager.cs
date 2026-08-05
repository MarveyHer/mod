using System.Collections.Generic;
using UnityEngine;

public class EffectDragParticlesManager : MonoBehaviour
{
	public static EffectDragParticlesManager instance;

	private ObjectPoolGenericMono<EffectParticlesCursor> _pool;

	[SerializeField]
	private EffectParticlesCursor _prefab;

	[SerializeField]
	private List<SpriteSet> _sprite_sets;

	[SerializeField]
	public float _spawn_interval = 10f;

	private void Awake()
	{
		_pool = new ObjectPoolGenericMono<EffectParticlesCursor>(_prefab, base.transform);
		instance = this;
	}

	private void Update()
	{
		updateSpawn();
		updateAnimation();
	}

	private void updateAnimation()
	{
		if (_pool.countActive() == 0)
		{
			return;
		}
		foreach (EffectParticlesCursor tEffect in _pool.getListTotal())
		{
			if (tEffect.isActiveAndEnabled)
			{
				tEffect.update();
			}
		}
	}

	private void updateSpawn()
	{
		if (Config.isDraggingItem() && Config.dragging_item_object != null && Config.dragging_item_object.spawn_particles_on_drag && (float)Time.frameCount % _spawn_interval == 0f)
		{
			spawnNew(Config.dragging_item_object.transform.position);
		}
	}

	public void spawnNew(Vector3 pPos)
	{
		EffectParticlesCursor next = _pool.getNext();
		next.setFrames(_sprite_sets.GetRandom().sprites);
		next.launch();
		next.getAnimation().setActionFinish(finishingEffectAction);
		Vector2 tPos = pPos;
		tPos.x += Randy.randomFloat(-1f, 1f);
		tPos.y += Randy.randomFloat(-1f, 1f);
		next.transform.position = tPos;
	}

	private void finishingEffectAction(MonoBehaviour pEffectObject)
	{
		_pool.release(pEffectObject.GetComponent<EffectParticlesCursor>());
	}
}
