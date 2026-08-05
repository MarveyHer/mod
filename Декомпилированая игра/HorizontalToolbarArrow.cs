using UnityEngine;

public class HorizontalToolbarArrow : ToolbarArrow
{
	public bool is_left = true;

	protected override void Update()
	{
		base.Update();
		float tVal = iTween.easeInOutCirc(0f, hide_position.x, timer);
		if (arrow_transform.localPosition.x != tVal)
		{
			arrow_transform.localPosition = new Vector3(tVal, 0f);
		}
	}

	protected override float getScrollPosition()
	{
		return scroll_rect.horizontalNormalizedPosition;
	}

	protected override void setScrollPosition(float pValue)
	{
		scroll_rect.horizontalNormalizedPosition = pValue;
	}

	protected override float getEndPosition()
	{
		float tEndPos = getScrollPosition();
		float tS = (float)Screen.width / CanvasMain.instance.canvas_ui.scaleFactor / scroll_rect.content.rect.width;
		if (is_left)
		{
			return tEndPos - Mathf.Min(tS, 0.5f);
		}
		return tEndPos + Mathf.Min(tS, 0.5f);
	}

	protected override void onScroll(Vector2 pVal)
	{
		float tWidth = (float)Screen.width / CanvasMain.instance.canvas_ui.scaleFactor;
		should_show = true;
		if (scroll_rect.content.rect.width < tWidth)
		{
			should_show = false;
		}
		else if (is_left)
		{
			if (getScrollPosition() > 0.1f)
			{
				should_show = true;
			}
			else
			{
				should_show = false;
			}
		}
		else if (getScrollPosition() == 1f)
		{
			should_show = false;
		}
		else if (getScrollPosition() < 0.98f)
		{
			should_show = true;
		}
		else
		{
			should_show = false;
		}
	}
}
