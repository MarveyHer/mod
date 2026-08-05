using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MetaNeedsElementUnit : UnitElement
{
	[SerializeField]
	private GameObject _container;

	[SerializeField]
	private Text _text;

	protected override IEnumerator showContent()
	{
		Actor tActor = SelectedUnit.unit;
		if (tActor != null && tActor.isAlive())
		{
			string tFinalText = MetaTextReportHelper.addSingleUnitText(tActor, pAddGap: false, pAddNameQuote: false);
			_text.text = tFinalText;
			if (!string.IsNullOrEmpty(tFinalText))
			{
				_container.gameObject.SetActive(value: true);
			}
		}
		yield break;
	}

	protected override void clear()
	{
		base.clear();
		_container.gameObject.SetActive(value: false);
	}
}
