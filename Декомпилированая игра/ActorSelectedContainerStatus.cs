using UnityEngine;

public class ActorSelectedContainerStatus : SelectedElementBase<StatusEffectButton>
{
	[SerializeField]
	private StatusEffectButton _prefab_status;

	private void Awake()
	{
		_pool = new ObjectPoolGenericMono<StatusEffectButton>(_prefab_status, _grid);
	}

	public void update(NanoObject pNano)
	{
		refresh(pNano);
	}

	protected override void refresh(NanoObject pNano)
	{
		clear();
		foreach (Status tData in ((Actor)pNano).getStatuses())
		{
			if (!tData.is_finished)
			{
				loadStatusButton(tData);
			}
		}
	}

	private void loadStatusButton(Status pStatus)
	{
		StatusEffectButton next = _pool.getNext();
		next.load(pStatus);
		next.setUpdatableTooltip(pState: true);
	}
}
