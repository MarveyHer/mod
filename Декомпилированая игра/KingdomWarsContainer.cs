using System.Collections;
using UnityEngine;

public class KingdomWarsContainer : KingdomDiplomacyContainer<WarBanner, War, WarData>
{
	protected override IEnumerator showContent()
	{
		if (!base.kingdom.hasEnemies())
		{
			yield break;
		}
		using ListPool<War> tList = new ListPool<War>(base.kingdom.getWars());
		track_objects.AddRange(tList);
		yield return new WaitForSecondsRealtime(0.025f);
		Vector3 tScale = new Vector3(0.8f, 0.8f, 1f);
		foreach (ref War item in tList)
		{
			War tWar = item;
			if (!tWar.isRekt())
			{
				WarBanner tElement = pool_elements.getNext();
				TipButton tTipButton = tElement.GetComponent<TipButton>();
				if (!tElement.HasComponent<DraggableLayoutElement>())
				{
					tElement.AddComponent<DraggableLayoutElement>();
				}
				tTipButton.showOnClick = true;
				tTipButton.setDefaultScale(tScale);
				tElement.buttons_enabled = true;
				tElement.load(tWar);
				UiButtonHoverAnimation component = tElement.GetComponent<UiButtonHoverAnimation>();
				component.enabled = false;
				component.scale_size = 1f;
				component.default_scale = tScale;
				RectTransform component2 = tElement.GetComponent<RectTransform>();
				component2.SetAnchor(AnchorPresets.MiddleCenter);
				component2.localScale = tScale;
				component2.anchoredPosition = new Vector2(0f, 0f);
			}
		}
	}
}
