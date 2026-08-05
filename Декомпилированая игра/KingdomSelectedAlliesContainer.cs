public class KingdomSelectedAlliesContainer : KingdomDiplomacyContainer<KingdomBanner, Kingdom, KingdomData>
{
	protected override void OnEnable()
	{
	}

	public void update(Kingdom pKingdom)
	{
		meta_object = pKingdom;
		clear();
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
			return;
		}
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
			}
		}
	}
}
