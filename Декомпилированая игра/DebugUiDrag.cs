using UnityEngine;
using UnityEngine.EventSystems;

public class DebugUiDrag : EventTrigger
{
	private bool dragging;

	private Transform mainTransform;

	private Transform canvasContainer;

	private DebugTool _tool;

	private Canvas _canvas;

	private void Start()
	{
		_tool = base.transform.GetComponentInParent<DebugTool>();
		_canvas = base.transform.GetComponentInParent<Canvas>();
		mainTransform = _tool.transform;
		canvasContainer = mainTransform.parent;
	}

	public void Update()
	{
		if (dragging)
		{
			Vector3 tNewPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			mainTransform.SetParent(null, worldPositionStays: true);
			mainTransform.SetParent(canvasContainer, worldPositionStays: true);
			Vector2 tSizeDelta = _tool.GetComponent<RectTransform>().sizeDelta;
			tNewPos.x += tSizeDelta.x / 2f - 75f;
			tNewPos.y += 20f;
			mainTransform.position = tNewPos;
		}
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		dragging = true;
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		dragging = false;
	}
}
