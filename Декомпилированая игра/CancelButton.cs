using UnityEngine;
using UnityEngine.UI;

public class CancelButton : MonoBehaviour
{
	public Image powerIcon;

	public bool goUp;

	public bool goDown;

	private bool _going_down;

	private bool _going_up;

	private RectTransform _rect;

	private float _timer;

	private const float Y_TOP_TARGET = 90f;

	private void Awake()
	{
		_rect = GetComponent<RectTransform>();
	}

	public void setIconFrom(PowerButton pButton)
	{
		if (pButton.godPower != null && !(pButton.icon == null))
		{
			powerIcon.sprite = pButton.icon.sprite;
		}
	}

	private void Update()
	{
		if (goDown != _going_down)
		{
			_going_down = goDown;
			_timer = 0f;
			if (goDown)
			{
				_timer = 0.95f;
			}
		}
		if (goUp != _going_up)
		{
			_going_up = goUp;
			_timer = -1f;
		}
		if (_timer < 1f)
		{
			_timer += Time.deltaTime / 2f;
			_timer = Mathf.Clamp(_timer, 0f, 1f);
			float tVal = (_going_down ? iTween.easeInOutCirc(0f, -90f, _timer) : ((!_going_up) ? iTween.easeInOutCirc(_rect.anchoredPosition.y, 0f, _timer) : iTween.easeInQuart(0f, 90f, _timer)));
			_rect.anchoredPosition = new Vector3(_rect.anchoredPosition.x, tVal);
		}
	}
}
