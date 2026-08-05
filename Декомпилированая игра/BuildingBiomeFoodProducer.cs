public class BuildingBiomeFoodProducer : BaseBuildingComponent
{
	private const float FOOD_INTERVAL = 90f;

	private float timer = 90f;

	public override void update(float pElapsed)
	{
		if (building.city == null || !building.isUsable())
		{
			return;
		}
		if (timer > 0f)
		{
			timer -= pElapsed;
			return;
		}
		timer = 90f;
		WorldTile tTile = building.tiles.GetRandom();
		string tFoodRes = tTile.Type.food_resource;
		if (string.IsNullOrEmpty(tFoodRes))
		{
			tFoodRes = tTile.main_type.food_resource;
		}
		if (!string.IsNullOrEmpty(tFoodRes) && building.city.getResourcesAmount(tFoodRes) < 10)
		{
			building.city.addResourcesToRandomStockpile(tFoodRes);
		}
	}
}
