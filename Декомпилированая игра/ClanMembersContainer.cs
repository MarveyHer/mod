using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ClanMembersContainer : ClanElement
{
	private ObjectPoolGenericMono<PrefabUnitElement> _pool_members;

	[SerializeField]
	private RectTransform _list_members;

	[SerializeField]
	private LocalizedText _title_members;

	[SerializeField]
	private PrefabUnitElement _prefab;

	[SerializeField]
	private Text _members_counter;

	protected override void Awake()
	{
		_pool_members = new ObjectPoolGenericMono<PrefabUnitElement>(_prefab, _list_members);
		base.Awake();
	}

	protected override IEnumerator showContent()
	{
		if (base.clan.units.Count == 0)
		{
			yield break;
		}
		Actor tChief = base.clan.getChief();
		using ListPool<Actor> _clan_members = new ListPool<Actor>(base.clan.units);
		track_objects.AddRange(_clan_members);
		_clan_members.Remove(tChief);
		if (_clan_members.Count == 0)
		{
			yield break;
		}
		_title_members.gameObject.SetActive(value: true);
		_list_members.gameObject.SetActive(value: true);
		_members_counter.text = base.clan.getTextMaxMembers();
		Actor tClanChief = base.clan.getChief();
		if (tClanChief == null || !tClanChief.hasCulture())
		{
			_clan_members.Sort(ListSorters.sortUnitByAgeOldFirst);
		}
		else
		{
			ListSorters.sortUnitsSortedByAgeAndTraits(_clan_members, base.clan.getClanCulture());
		}
		foreach (ref Actor item in _clan_members)
		{
			Actor tActor = item;
			yield return new WaitForSecondsRealtime(0.025f);
			showMember(tActor);
		}
	}

	private void showMember(Actor pActor)
	{
		PrefabUnitElement next = _pool_members.getNext();
		next.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
		next.show(pActor);
	}

	protected override void clear()
	{
		_title_members.gameObject.SetActive(value: false);
		_list_members.gameObject.SetActive(value: false);
		_pool_members.clear();
		base.clear();
	}
}
