using UnityEngine;

public class BrushSizesButton : MonoBehaviour
{
	private PowerButton _power_button;

	private string _latest_used = string.Empty;

	private void Awake()
	{
		_power_button = GetComponent<PowerButton>();
	}

	private void Update()
	{
		if (Config.current_brush != _latest_used)
		{
			_latest_used = Config.current_brush;
			BrushData tNewBrush = Brush.get(Config.current_brush);
			if (tNewBrush == null)
			{
				Debug.LogError(Config.current_brush + " is not a valid brush");
			}
			else
			{
				tNewBrush.setupImage(_power_button.icon);
			}
		}
	}
}
