using System.Collections;
using UnityEngine;

public class ClanChiefElement : ClanElement
{
	[SerializeField]
	private GameObject _title_element;

	[SerializeField]
	private PrefabUnitElement _chief_element;

	protected override IEnumerator showContent()
	{
		if (base.clan.hasChief())
		{
			track_objects.Add(base.clan.getChief());
			_title_element.SetActive(value: true);
			_chief_element.gameObject.SetActive(value: true);
			_chief_element.show(base.clan.getChief());
		}
		yield break;
	}

	protected override void clear()
	{
		_title_element.SetActive(value: false);
		_chief_element.gameObject.SetActive(value: false);
		base.clear();
	}

	public override bool checkRefreshWindow()
	{
		if (_chief_element.gameObject.activeSelf && !base.clan.hasChief())
		{
			return true;
		}
		return base.checkRefreshWindow();
	}
}
