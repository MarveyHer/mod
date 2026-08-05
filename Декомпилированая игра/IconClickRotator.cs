using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IconClickRotator : MonoBehaviour
{
	private Quaternion _startRotation;

	private Coroutine _rotationRoutine;

	private void Awake()
	{
		base.gameObject.AddOrGetComponent<Button>().onClick.AddListener(click);
		base.gameObject.AddOrGetComponent<ScrollableButton>();
		_startRotation = base.transform.rotation;
	}

	private void click()
	{
		startRandomRotation();
	}

	private void startRandomRotation()
	{
		if (_rotationRoutine != null)
		{
			StopCoroutine(_rotationRoutine);
		}
		float tRandomAngle = Random.Range(-180f, 180f);
		Quaternion targetRotation = Quaternion.Euler(0f, 0f, tRandomAngle);
		_rotationRoutine = StartCoroutine(RotateTo(targetRotation, 0.2f));
	}

	private IEnumerator RotateTo(Quaternion targetRotation, float duration)
	{
		float time = 0f;
		Quaternion initialRotation = base.transform.rotation;
		while (time < duration)
		{
			base.transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, time / duration);
			time += Time.deltaTime;
			yield return null;
		}
		base.transform.rotation = targetRotation;
	}
}
