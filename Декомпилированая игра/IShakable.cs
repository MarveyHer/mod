using DG.Tweening;
using UnityEngine;

public interface IShakable
{
	float shake_duration { get; }

	float shake_strength { get; }

	Tweener shake_tween { get; set; }

	Transform transform { get; }

	void shake()
	{
		killShakeTween();
		shake_tween = transform.DOShakePosition(shake_duration, shake_strength);
	}

	void killShakeTween()
	{
		shake_tween.Kill(complete: true);
	}
}
