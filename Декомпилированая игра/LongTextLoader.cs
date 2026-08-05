using System;
using UnityEngine;
using UnityEngine.UI;

public class LongTextLoader : MonoBehaviour
{
	public TextAsset textAsset;

	protected Text m_text;

	private void Start()
	{
		m_text = GetComponent<Text>();
		create();
		finish();
	}

	private void finish()
	{
		RectTransform tRect = m_text.GetComponent<RectTransform>();
		tRect.sizeDelta = new Vector2(tRect.sizeDelta.x, m_text.preferredHeight + 10f);
		RectTransform component = base.transform.parent.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(component.sizeDelta.x, tRect.sizeDelta.y);
		float tLocPos = 0f - component.transform.localPosition.y;
		component.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, tRect.sizeDelta.y + 20f + tLocPos);
	}

	public virtual void create()
	{
		try
		{
			m_text.text = textAsset.text;
		}
		catch (Exception)
		{
			Debug.LogError("LongTextLoader: Text File is too long");
		}
	}
}
