using UnityEngine;
using UnityEngine.UI;

public class InterestingPeopleElement : MonoBehaviour
{
	private ObjectPoolGenericMono<PrefabUnitElement> _pool_elements;

	[SerializeField]
	private PrefabUnitElement _element;

	[SerializeField]
	private Text _counter;

	[SerializeField]
	private Transform _grid;

	private void Awake()
	{
		_pool_elements = new ObjectPoolGenericMono<PrefabUnitElement>(_element, _grid);
		for (int i = 0; i < _grid.childCount; i++)
		{
			Object.DestroyImmediate(_grid.GetChild(i).gameObject);
		}
	}

	public void show(Actor pActor, int pValue)
	{
		showMember(pActor);
		_counter.text = pValue.ToString();
	}

	private void showMember(Actor pActor)
	{
		PrefabUnitElement next = _pool_elements.getNext();
		next.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
		next.show(pActor);
	}

	private void OnDisable()
	{
		_pool_elements.clear();
	}
}
