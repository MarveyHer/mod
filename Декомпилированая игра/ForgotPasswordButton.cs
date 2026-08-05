using UnityEngine;
using UnityEngine.UI;

public class ForgotPasswordButton : MonoBehaviour
{
	public GameObject emailBG;

	public InputField emailInput;

	public Text statusMessage;

	public Button continueButton;

	private Button forgotPasswordButton;

	private bool checking;

	private void OnEnable()
	{
		if (Config.game_loaded)
		{
			newStatus("");
			emailInput.gameObject.SetActive(value: true);
			emailBG.gameObject.SetActive(value: true);
			continueButton.gameObject.SetActive(value: false);
			base.gameObject.SetActive(value: true);
			forgotPasswordButton = base.gameObject.GetComponent<Button>();
			checking = false;
		}
	}

	public void resetPassword()
	{
		checking = true;
		clearStatus();
	}

	private void Update()
	{
		forgotPasswordButton.interactable = !checking;
	}

	private void newStatus(string pMessage)
	{
		Debug.Log("new status " + pMessage);
		if (LocalizedTextManager.stringExists(pMessage))
		{
			statusMessage.GetComponent<LocalizedText>().key = pMessage;
			statusMessage.GetComponent<LocalizedText>().updateText();
		}
		else
		{
			statusMessage.text = pMessage;
		}
	}

	private void clearStatus()
	{
		newStatus("");
	}
}
