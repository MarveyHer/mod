using System.Collections;
using UnityEngine;

public class KingdomCapitalElement : KingdomElement
{
	[SerializeField]
	private CityListElement _capital_element;

	protected override IEnumerator showContent()
	{
		if (base.kingdom.hasCapital())
		{
			track_objects.Add(base.kingdom.capital);
			_capital_element.gameObject.SetActive(value: true);
			_capital_element.show(base.kingdom.capital);
		}
		yield break;
	}

	protected override void clear()
	{
		_capital_element.gameObject.SetActive(value: false);
		base.clear();
	}

	public override bool checkRefreshWindow()
	{
		if (_capital_element.gameObject.activeSelf && !base.kingdom.hasCapital())
		{
			return true;
		}
		return base.checkRefreshWindow();
	}
}
