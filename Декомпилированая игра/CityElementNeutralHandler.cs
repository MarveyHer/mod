using System.Collections;
using UnityEngine;

public class CityElementNeutralHandler : CityElement
{
	[SerializeField]
	private GameObject _layout_element_content_meta;

	[SerializeField]
	private GameObject _layout_element_wants;

	[SerializeField]
	private GameObject _layout_element_ruler;

	private void checkNeutralElements()
	{
		if (meta_object.isNeutral())
		{
			_layout_element_content_meta.SetActive(value: false);
			_layout_element_wants.SetActive(value: false);
			_layout_element_ruler.SetActive(value: false);
		}
	}

	protected override IEnumerator showContent()
	{
		checkNeutralElements();
		return base.showContent();
	}
}
