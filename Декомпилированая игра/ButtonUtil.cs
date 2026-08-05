using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonUtil : MonoBehaviour
{
	private Button _button;

	public void ResetState()
	{
		if (_button == null)
		{
			_button = GetComponent<Button>();
			_button.onClick.AddListener(playSound);
		}
		_button.enabled = false;
		_button.enabled = true;
	}

	private void playSound()
	{
		SoundBox.click();
	}
}
