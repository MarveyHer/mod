using UnityEngine;
using UnityEngine.UI;

public class DeleteWorldButton : MonoBehaviour
{
	private void Start()
	{
		if (TryGetComponent<Button>(out var tButton))
		{
			tButton.onClick.AddListener(deleteWorld);
		}
	}

	private void deleteWorld()
	{
		ScrollWindow.showWindow("save_delete_confirm");
	}
}
