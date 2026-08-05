using UnityEngine;
using UnityEngine.UI;

public class LocusDot : MonoBehaviour
{
	[SerializeField]
	private Image _status;

	internal Image status => _status;

	public void colorDot(Color pColor)
	{
		_status.color = pColor;
	}

	public void colorDot(char pGeneticCode)
	{
		colorDot(NucleobaseHelper.getColor(pGeneticCode));
	}
}
