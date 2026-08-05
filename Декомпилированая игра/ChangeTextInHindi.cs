using UnityEngine;
using UnityEngine.UI;

public class ChangeTextInHindi : MonoBehaviour
{
	private void Start()
	{
		string text = base.gameObject.GetComponent<Text>().text;
		base.gameObject.GetComponent<Text>().SetHindiText(text);
	}
}
