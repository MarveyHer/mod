using System.Collections.Generic;

public static class LoyaltyCalculator
{
	public static Dictionary<LoyaltyAsset, int> results = new Dictionary<LoyaltyAsset, int>();

	public static int total = 0;

	public static int calculate(City pCity)
	{
		clear();
		foreach (LoyaltyAsset tAsset in AssetManager.loyalty_library.list)
		{
			int tResult = tAsset.calc(pCity);
			total += tResult;
			if (tResult != 0)
			{
				results.Add(tAsset, tResult);
			}
		}
		return total;
	}

	private static void clear()
	{
		total = 0;
		results.Clear();
	}
}
