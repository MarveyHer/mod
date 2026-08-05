using System.Collections.Generic;

public interface ILibraryWithUnlockables
{
	IEnumerable<BaseUnlockableAsset> elements_list { get; }

	int countTotalKnowledge()
	{
		int tTotalAmount = 0;
		foreach (BaseUnlockableAsset item in elements_list)
		{
			if (item.show_in_knowledge_window)
			{
				tTotalAmount++;
			}
		}
		return tTotalAmount;
	}

	int countUnlockedByPlayer()
	{
		int tUnlockedAmount = 0;
		foreach (BaseUnlockableAsset tAsset in elements_list)
		{
			if (tAsset.show_in_knowledge_window && tAsset.isUnlockedByPlayer())
			{
				tUnlockedAmount++;
			}
		}
		return tUnlockedAmount;
	}
}
