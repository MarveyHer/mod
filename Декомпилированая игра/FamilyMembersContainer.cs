using System.Collections;
using UnityEngine;

public class FamilyMembersContainer : FamilyElement
{
	private ObjectPoolGenericMono<PrefabUnitElement> _pool_parents;

	private ObjectPoolGenericMono<PrefabUnitElement> _pool_children;

	[SerializeField]
	private RectTransform _list_parents;

	[SerializeField]
	private RectTransform _list_children;

	[SerializeField]
	private LocalizedText _title_parents;

	[SerializeField]
	private LocalizedText _title_children;

	[SerializeField]
	private PrefabUnitElement _prefab;

	protected override void Awake()
	{
		_pool_children = new ObjectPoolGenericMono<PrefabUnitElement>(_prefab, _list_children);
		_pool_parents = new ObjectPoolGenericMono<PrefabUnitElement>(_prefab, _list_parents);
		base.Awake();
	}

	protected override IEnumerator showContent()
	{
		if (base.family.units.Count == 0)
		{
			yield break;
		}
		using ListPool<Actor> tFamilyMembers = new ListPool<Actor>(base.family.units);
		track_objects.AddRange(tFamilyMembers);
		tFamilyMembers.Sort(ListSorters.sortUnitByAgeOldFirst);
		tFamilyMembers.Sort(sortByMainParent);
		FamilyParentsMode tFamilyMode = base.family.getActorAsset().family_show_parents;
		bool num = tFamilyMode == FamilyParentsMode.Alpha;
		bool tShowNormalFamily = tFamilyMode == FamilyParentsMode.Normal;
		bool tHaveParents = false;
		bool tHaveChildren = false;
		if (num)
		{
			string tTerm = base.family.getActorAsset().getCollectiveTermID();
			_title_children.setKeyAndUpdate(tTerm);
		}
		else
		{
			_title_children.setKeyAndUpdate("children");
		}
		foreach (ref Actor item in tFamilyMembers)
		{
			Actor tActor = item;
			if (base.family.isMainFounder(tActor) && tShowNormalFamily)
			{
				if (!tHaveParents)
				{
					tHaveParents = true;
					showParents();
				}
				yield return new WaitForSecondsRealtime(0.025f);
				showMember(tActor, _pool_parents);
			}
			else
			{
				if (!tHaveChildren)
				{
					tHaveChildren = true;
					showChildren();
				}
				yield return new WaitForSecondsRealtime(0.025f);
				showMember(tActor, _pool_children);
			}
		}
	}

	private void showParents()
	{
		_title_parents.gameObject.SetActive(value: true);
		_list_parents.gameObject.SetActive(value: true);
	}

	private void showChildren()
	{
		_title_children.gameObject.SetActive(value: true);
		_list_children.gameObject.SetActive(value: true);
	}

	private int sortByMainParent(Actor pActor1, Actor pActor2)
	{
		if (base.family.isMainFounder(pActor1) && !base.family.isMainFounder(pActor2))
		{
			return -1;
		}
		if (!base.family.isMainFounder(pActor1) && base.family.isMainFounder(pActor2))
		{
			return 1;
		}
		return 0;
	}

	private void showMember(Actor pActor, ObjectPoolGenericMono<PrefabUnitElement> pPool)
	{
		PrefabUnitElement next = pPool.getNext();
		next.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
		next.show(pActor);
	}

	protected override void clear()
	{
		_title_parents.gameObject.SetActive(value: false);
		_list_parents.gameObject.SetActive(value: false);
		_title_children.gameObject.SetActive(value: false);
		_list_children.gameObject.SetActive(value: false);
		_pool_children.clear();
		_pool_parents.clear();
		base.clear();
	}
}
