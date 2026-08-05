using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LogoButton : MonoBehaviour
{
	private List<UiCreature> listLetters;

	private float initScale = 1f;

	private Tweener tweener;

	private void Awake()
	{
		initScale = base.transform.localScale.x;
		loadLetters();
	}

	private void loadLetters()
	{
		listLetters = new List<UiCreature>();
		Transform letters = base.transform.FindRecursive("Letters").transform;
		int num = letters.childCount;
		for (int i = 0; i < num; i++)
		{
			UiCreature tLetter = letters.GetChild(i).GetComponent<UiCreature>();
			if (tLetter.dropped)
			{
				tLetter.resetPosition();
			}
			listLetters.Add(tLetter);
		}
	}

	private void letterFall()
	{
		if (listLetters.Count == 0)
		{
			loadLetters();
			AchievementLibrary.destroy_worldbox.check();
			return;
		}
		listLetters.ShuffleOne();
		UiCreature uiCreature = listLetters[0];
		listLetters.RemoveAt(0);
		uiCreature.click();
	}

	public void clickLogo()
	{
		MusicBox.playSound("event:/SFX/EXPLOSIONS/ExplosionHuge");
		if (tweener != null && tweener.active)
		{
			tweener.Kill();
		}
		float tScale = initScale * 1.2f;
		if (listLetters.Count == 0)
		{
			tScale = 1.6f;
			base.transform.localScale = new Vector3(tScale, tScale, tScale);
			tweener = base.transform.DOScale(new Vector3(initScale, initScale, initScale), 0.3f).SetEase(Ease.OutBack);
		}
		else
		{
			base.transform.localScale = new Vector3(tScale, tScale, tScale);
			tweener = base.transform.DOScale(new Vector3(initScale, initScale, initScale), 0.3f).SetEase(Ease.OutBack);
		}
		letterFall();
	}
}
