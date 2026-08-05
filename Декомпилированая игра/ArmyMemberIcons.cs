using System.Collections;
using UnityEngine;

public class ArmyMemberIcons : ArmyElement
{
	[SerializeField]
	private UnitAvatarLoader _top;

	[SerializeField]
	private UnitAvatarLoader _top_left;

	[SerializeField]
	private UnitAvatarLoader _top_right;

	[SerializeField]
	private UnitAvatarLoader _left;

	[SerializeField]
	private UnitAvatarLoader _right;

	[SerializeField]
	private UnitAvatarLoader _bottom;

	[SerializeField]
	private UnitAvatarLoader _bottom_left;

	[SerializeField]
	private UnitAvatarLoader _bottom_right;

	[SerializeField]
	private ArmyBanner _banner;

	private UnitAvatarLoader[] _list_warrior_avatars;

	protected override void Awake()
	{
		_list_warrior_avatars = new UnitAvatarLoader[8] { _top, _top_left, _right, _bottom_right, _bottom, _bottom_left, _left, _top_right };
		base.Awake();
	}

	protected override void clear()
	{
		UnitAvatarLoader[] list_warrior_avatars = _list_warrior_avatars;
		for (int i = 0; i < list_warrior_avatars.Length; i++)
		{
			list_warrior_avatars[i].gameObject.SetActive(value: false);
		}
		_banner.gameObject.SetActive(value: false);
	}

	protected override IEnumerator showContent()
	{
		_banner.gameObject.SetActive(value: true);
		_banner.load(base.army);
		using ListPool<Actor> tUnits = new ListPool<Actor>(base.army.getUnits());
		if (tUnits.Count == 0)
		{
			yield break;
		}
		tUnits.Shuffle();
		for (int i = 0; i < _list_warrior_avatars.Length; i++)
		{
			yield return new WaitForEndOfFrame();
			if (tUnits.Count != 0)
			{
				UnitAvatarLoader obj = _list_warrior_avatars[i];
				Actor tActor = tUnits.Pop();
				obj.gameObject.SetActive(value: true);
				obj.load(tActor);
				continue;
			}
			break;
		}
	}
}
