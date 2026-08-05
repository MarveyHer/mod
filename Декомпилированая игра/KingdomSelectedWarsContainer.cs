public class KingdomSelectedWarsContainer : KingdomDiplomacyContainer<WarBanner, War, WarData>
{
	protected override void OnEnable()
	{
	}

	public void update(Kingdom pKingdom)
	{
		meta_object = pKingdom;
		clear();
		if (!base.kingdom.hasEnemies())
		{
			return;
		}
		using ListPool<War> tList = new ListPool<War>(base.kingdom.getWars());
		track_objects.AddRange(tList);
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
				tElement.buttons_enabled = true;
				tElement.load(tWar);
			}
		}
	}
}
