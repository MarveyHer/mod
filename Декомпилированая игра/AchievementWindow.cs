using System.Collections.Generic;
using UnityEngine;

public class AchievementWindow : MonoBehaviour
{
	public AchievementGroup achievementGroupPrefab;

	private List<AchievementGroup> _elements = new List<AchievementGroup>();

	public Transform transformContent;

	public StatBar achievementBar;

	private void OnEnable()
	{
		showList();
	}

	internal void showList()
	{
		if (!Config.game_loaded)
		{
			return;
		}
		for (int i = 0; i < _elements.Count; i++)
		{
			Object.Destroy(_elements[i].gameObject);
		}
		_elements.Clear();
		foreach (AchievementGroupAsset tAchievementGroup in AssetManager.achievement_groups.list)
		{
			showElement(tAchievementGroup);
		}
		updateTotalBar();
	}

	private void updateTotalBar()
	{
		int tMax = AssetManager.achievements.list.Count;
		int tUnlocked = AchievementLibrary.countUnlocked();
		achievementBar.setBar(tUnlocked, tMax, "/" + tMax.ToText());
	}

	private void showElement(AchievementGroupAsset pAchievementGroup)
	{
		AchievementGroup tElement = Object.Instantiate(achievementGroupPrefab, transformContent);
		tElement.showGroup(pAchievementGroup);
		_elements.Add(tElement);
	}
}
