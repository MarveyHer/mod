using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSfx : MonoBehaviour
{
	private Button _button;

	private void Start()
	{
		_button = GetComponent<Button>();
		_button.onClick.AddListener(playSound);
	}

	private void playSound()
	{
		SoundBox.click();
		_button.enabled = false;
		_button.enabled = true;
	}
}
