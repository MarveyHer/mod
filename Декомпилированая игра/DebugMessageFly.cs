using System.Collections.Generic;
using UnityEngine;

public class DebugMessageFly : MonoBehaviour
{
	private List<string> listString = new List<string>();

	public Transform originTransform;

	private TextMesh textMesh;

	private void Awake()
	{
		textMesh = GetComponent<TextMesh>();
	}

	public void addString(string pText)
	{
		if (textMesh.color.a < 0.3f)
		{
			listString.Clear();
		}
		else if (listString.Count > 20)
		{
			listString.RemoveAt(0);
		}
		listString.Add(pText);
		Vector3 pos = new Vector3(originTransform.localPosition.x, originTransform.localPosition.y);
		base.transform.localPosition = pos;
		string tNewString = "";
		foreach (string tString in listString)
		{
			tNewString = tNewString + tString + "\n";
		}
		textMesh.text = tNewString;
		Color tC = textMesh.color;
		tC.a = 1f;
		textMesh.color = tC;
	}

	public void moveUp()
	{
		Vector3 tV = base.transform.localPosition;
		tV.y += 3f;
		base.transform.localPosition = tV;
	}

	private void Update()
	{
		Vector3 tS = base.transform.localScale;
		tS.x += 2f * Time.deltaTime;
		if (tS.x > 1f)
		{
			tS.x = 1f;
		}
		base.transform.localScale = tS;
		Vector3 tV = base.transform.localPosition;
		tV.y += 0.5f * Time.deltaTime;
		base.transform.localPosition = tV;
		Color tC = textMesh.color;
		tC.a -= 0.3f * Time.deltaTime;
		textMesh.color = tC;
		if (tC.a <= 0f)
		{
			Object.Destroy(base.gameObject);
			DebugMessage.instance.list.Remove(this);
		}
	}
}
