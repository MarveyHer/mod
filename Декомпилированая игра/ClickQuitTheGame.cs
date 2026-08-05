using db;
using UnityEngine;

public class ClickQuitTheGame : MonoBehaviour
{
	public void clickQuit()
	{
		DBManager.clearAndClose();
		Application.Quit();
	}
}
