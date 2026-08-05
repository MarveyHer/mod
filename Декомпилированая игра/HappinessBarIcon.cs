using UnityEngine;
using UnityEngine.UI;

public class HappinessBarIcon : MonoBehaviour
{
	[SerializeField]
	private Image _icon;

	private Actor _actor;

	private void Awake()
	{
		GetComponentInParent<StatBar>().addCallback(barUpdated);
	}

	public void load(Actor pActor)
	{
		_actor = pActor;
	}

	private void barUpdated(float pValue, float pMax)
	{
		if (!_actor.isRekt())
		{
			Sprite tSprite = HappinessHelper.getSpriteBasedOnHappinessValue(_actor.getHappiness());
			_icon.sprite = tSprite;
		}
	}

	private void OnDisable()
	{
		_actor = null;
	}
}
