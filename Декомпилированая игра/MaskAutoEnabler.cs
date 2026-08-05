using UnityEngine;
using UnityEngine.UI;

public class MaskAutoEnabler : MonoBehaviour
{
	private void Awake()
	{
		GetComponent<Mask>().enabled = true;
		GetComponent<Image>().enabled = true;
	}
}
