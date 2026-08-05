using UnityEngine;

public class LoadingScreenSheepAnimation : MonoBehaviour
{
	internal static float angle;

	private void Update()
	{
		angle += Time.deltaTime * 20f;
		base.transform.localEulerAngles = new Vector3(0f, 0f, angle);
	}
}
