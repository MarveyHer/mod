using System.Collections;
using UnityEngine;

public class KingdomAlliesContainer : KingdomDiplomacyContainer<KingdomBanner, Kingdom, KingdomData>
{
	protected override IEnumerator showContent()
	{
		using ListPool<Kingdom> tList = World.world.wars.getNeutralKingdoms(base.kingdom);
		if (base.kingdom.hasAlliance())
		{
			foreach (Kingdom tKingdom in base.kingdom.getAlliance().kingdoms_list)
			{
				if (tKingdom != base.kingdom && !tKingdom.isRekt())
				{
					tList.Add(tKingdom);
				}
			}
		}
		track_objects.AddRange(tList);
		if (tList.Count == 0)
		{
			yield break;
		}
		yield return new WaitForSecondsRealtime(0.025f);
		Vector3 tScale = new Vector3(0.5f, 0.5f, 1f);
		foreach (ref Kingdom item in tList)
		{
			Kingdom tKingdom2 = item;
			if (!tKingdom2.isRekt())
			{
				KingdomBanner tElement = pool_elements.getNext();
				tElement.diplo_banner = true;
				tElement.GetComponent<TipButton>().showOnClick = true;
				tElement.GetComponentInChildren<RotateOnHover>().enabled = true;
				if (!tElement.HasComponent<DraggableLayoutElement>())
				{
					tElement.AddComponent<DraggableLayoutElement>();
				}
				tElement.load(tKingdom2);
				tElement.GetComponent<UiButtonHoverAnimation>().enabled = false;
				tElement.GetComponent<UiButtonHoverAnimation>().scale_size = 1f;
				tElement.GetComponent<UiButtonHoverAnimation>().default_scale = tScale;
				tElement.GetComponent<TipButton>().setDefaultScale(tScale);
				RectTransform component = tElement.GetComponent<RectTransform>();
				component.SetAnchor(AnchorPresets.MiddleCenter);
				component.localScale = tScale;
				component.anchoredPosition = new Vector2(0f, 0f);
			}
		}
	}
}
