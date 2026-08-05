public class AllianceSelectedKingdomsContainer : AllianceKingdomsContainer
{
	protected override void OnEnable()
	{
	}

	public void update(Alliance pAlliance)
	{
		meta_object = pAlliance;
		clear();
		using ListPool<Kingdom> tList = new ListPool<Kingdom>(base.alliance.kingdoms_hashset);
		track_objects.AddRange(tList);
		foreach (ref Kingdom item in tList)
		{
			Kingdom tKingdom = item;
			showBanner(tKingdom);
		}
	}
}
