using UnityEngine;

public class ImageRotator : MonoBehaviour
{
	public float rotation_speed = 70f;

	private void Update()
	{
		base.transform.Rotate(Vector3.forward * rotation_speed * Time.deltaTime, Space.Self);
	}
}
