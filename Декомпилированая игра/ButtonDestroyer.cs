using UnityEngine;

public class ButtonDestroyer : MonoBehaviour
{
	private void Awake()
	{
		if (Globals.specialAbstudio)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
