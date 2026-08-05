public class DeadKingdom : Kingdom
{
	public override void loadData(KingdomData pData)
	{
		setData(pData);
		data.load();
		ActorAsset tAsset = getActorAsset();
		asset = AssetManager.kingdoms.get(tAsset.kingdom_id_civilization);
	}

	public override int getAge()
	{
		int tStartYear = Date.getYear(data.created_time);
		return Date.getYear(data.died_time) - tStartYear;
	}

	public override string getMotto()
	{
		return data.motto;
	}
}
