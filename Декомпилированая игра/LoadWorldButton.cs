using UnityEngine;
using UnityEngine.UI;

public class LoadWorldButton : MonoBehaviour
{
	private void Start()
	{
		if (TryGetComponent<Button>(out var tButton))
		{
			tButton.onClick.AddListener(loadWorld);
		}
	}

	private void loadWorld()
	{
		if (SaveManager.getCurrentMeta().saveVersion == 15)
		{
			ErrorWindow.errorMessage = "No, abandon it.";
			ScrollWindow.get("error_with_reason").clickShow();
		}
		else
		{
			ScrollWindow.showWindow("save_load_confirm");
		}
	}
}
