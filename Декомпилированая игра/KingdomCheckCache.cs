using System.Collections.Generic;

public class KingdomCheckCache
{
	public Dictionary<long, bool> dict = new Dictionary<long, bool>();

	public long getHash(Kingdom pK1, Kingdom pK2)
	{
		int tHash1 = pK1.GetHashCode();
		int tHash2 = pK2.GetHashCode();
		if (tHash1 > tHash2)
		{
			return tHash1 * 1000000 + tHash2;
		}
		return tHash2 * 1000000 + tHash1;
	}

	public void clear()
	{
		dict.Clear();
	}
}
