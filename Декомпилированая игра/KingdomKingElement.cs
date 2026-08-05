using System.Collections;
using UnityEngine;

public class KingdomKingElement : KingdomElement
{
	[SerializeField]
	private GameObject _title_element;

	[SerializeField]
	private PrefabUnitElement _king_element;

	protected override IEnumerator showContent()
	{
		if (base.kingdom.hasKing())
		{
			track_objects.Add(base.kingdom.king);
			_title_element.SetActive(value: true);
			_king_element.gameObject.SetActive(value: true);
			_king_element.show(base.kingdom.king);
		}
		yield break;
	}

	protected override void clear()
	{
		_title_element.SetActive(value: false);
		_king_element.gameObject.SetActive(value: false);
		base.clear();
	}

	public override bool checkRefreshWindow()
	{
		if (_king_element.gameObject.activeSelf && !base.kingdom.hasKing())
		{
			return true;
		}
		return base.checkRefreshWindow();
	}
}
