using UnityEngine;
using UnityEngine.UI;

public class HistoryHudItem : MonoBehaviour
{
	private bool _creating = true;

	private float _remove_timer = 8f;

	private CanvasGroup _canvas_group;

	private Button _button;

	private WorldLogMessage _message;

	public Text textField;

	public Image icon;

	private RectTransform _rect_transform;

	public Image background;

	private bool _removing;

	private HistoryHud _history_hud;

	private float _time_limit;

	internal float target_bottom;

	private void Start()
	{
		_history_hud = GetComponentInParent<HistoryHud>();
		_canvas_group = GetComponent<CanvasGroup>();
		_canvas_group.alpha = 0f;
		_button = GetComponent<Button>();
		_rect_transform = GetComponent<RectTransform>();
		_button.onClick.AddListener(delegate
		{
			if (!MapBox.controlsLocked() && !MapBox.isControllingUnit() && !World.world.isAnyPowerSelected())
			{
				_remove_timer = 0f;
				_message.jumpToLocation();
			}
		});
	}

	private void OnEnable()
	{
		_creating = true;
		_remove_timer = 8f;
		_removing = false;
		GetComponent<CanvasGroup>().alpha = 0f;
	}

	public bool isRemoving()
	{
		return _removing;
	}

	public void setMessage(WorldLogMessage pMessage)
	{
		textField.text = pMessage.getFormatedText(textField);
		textField.GetComponent<LocalizedText>().checkTextFont();
		textField.GetComponent<LocalizedText>().checkSpecialLanguages();
		if (pMessage.getAsset().path_icon != "")
		{
			Sprite tSprite = SpriteTextureLoader.getSprite(pMessage.getAsset().path_icon);
			icon.sprite = tSprite;
		}
		else
		{
			icon.gameObject.SetActive(value: false);
		}
		_message = pMessage;
	}

	public void moveTo(float newBottom)
	{
		_time_limit = 0f;
		target_bottom = newBottom;
	}

	public void moveToAndDestroy(float newBottom)
	{
		_time_limit = 0f;
		target_bottom = newBottom;
		_remove_timer = 0.5f;
		_removing = true;
	}

	private void Update()
	{
		background.raycastTarget = _history_hud.raycastOn;
		_rect_transform.sizeDelta = new Vector2(_rect_transform.sizeDelta.x, 10f);
		if (_creating)
		{
			if (_canvas_group.alpha < 1f)
			{
				_canvas_group.alpha += Time.deltaTime * Config.time_scale_asset.multiplier * 2f;
			}
			else
			{
				_creating = false;
			}
		}
		else
		{
			if (Config.paused || ScrollWindow.isWindowActive() || RewardedAds.isShowing())
			{
				return;
			}
			if (_time_limit <= 2f)
			{
				_time_limit += Time.deltaTime;
				_rect_transform.SetTop(0f - Mathf.Lerp(_rect_transform.offsetMax.y, 0f - target_bottom, _time_limit / 2f));
			}
			if (_removing && _rect_transform.offsetMax.y > 10f)
			{
				_history_hud.makeInactive(this);
				return;
			}
			_remove_timer -= Time.deltaTime;
			if (_remove_timer <= 0f)
			{
				_canvas_group.alpha -= Time.deltaTime * 2f;
				if (_canvas_group.alpha <= 0f)
				{
					_history_hud.makeInactive(this);
				}
			}
		}
	}

	private void OnDisable()
	{
		_message.clear();
	}
}
