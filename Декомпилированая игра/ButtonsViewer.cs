using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsViewer : MonoBehaviour
{
	private List<PowerButton> buttons;

	private Transform content;

	private float lastX;

	private float lastY;

	private Canvas canvas;

	private void Start()
	{
		content = base.transform.parent;
		canvas = CanvasMain.instance.canvas_ui;
		buttons = new List<PowerButton>();
		_ = base.transform.childCount;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			GameObject tObject = base.transform.GetChild(i).gameObject;
			if (tObject.HasComponent<PowerButton>() && tObject.activeSelf)
			{
				buttons.Add(tObject.GetComponent<PowerButton>());
			}
			else if (!tObject.HasComponent<Image>() || !tObject.activeSelf)
			{
				Object.Destroy(tObject);
			}
		}
	}

	private void Update()
	{
		if (lastX == content.position.x && lastY == content.position.y)
		{
			return;
		}
		lastX = content.position.x;
		lastY = content.position.y;
		int yo1 = 0;
		int yo2 = 0;
		bool foundHidden = false;
		for (int i = 0; i < buttons.Count; i++)
		{
			PowerButton tButton = buttons[i];
			if (foundHidden)
			{
				yo2++;
				tButton.gameObject.SetActive(value: false);
				continue;
			}
			yo1++;
			Vector3[] v = new Vector3[4];
			tButton.rect_transform.GetWorldCorners(v);
			float maxX = Mathf.Max(v[0].x, v[1].x, v[2].x, v[3].x);
			float minX = Mathf.Min(v[0].x, v[1].x, v[2].x, v[3].x);
			if (maxX < 0f || minX > (float)Screen.width)
			{
				tButton.gameObject.SetActive(value: false);
				if (minX > (float)Screen.width)
				{
					foundHidden = true;
				}
			}
			else
			{
				tButton.gameObject.SetActive(value: true);
			}
		}
	}
}
