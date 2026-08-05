using System.Collections.Generic;

public static class EnumerableExtensions
{
	public static T GetRandom<T>(this IEnumerable<T> pEnumerable)
	{
		if (!(pEnumerable is List<T> tList))
		{
			if (!(pEnumerable is ListPool<T> tListPool))
			{
				if (!(pEnumerable is T[] tArray))
				{
					if (pEnumerable is HashSet<T> tHashSet)
					{
						return tHashSet.GetRandom();
					}
					using ListPool<T> tTempList = new ListPool<T>(pEnumerable);
					return tTempList.GetRandom();
				}
				return tArray.GetRandom();
			}
			return tListPool.GetRandom();
		}
		return tList.GetRandom();
	}
}
