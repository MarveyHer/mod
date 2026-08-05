using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementGroup : MonoBehaviour
{
	public AchievementButton achievementButtonPrefab;

	private List<AchievementButton> _elements = new List<AchievementButton>();

	public Text title;

	public Text counter;

	public Transform transformContent;

	public void showGroup(AchievementGroupAsset pAchievementGroup)
	{
		title.GetComponent<LocalizedText>().setKeyAndUpdate(pAchievementGroup.getLocaleID());
		title.color = pAchievementGroup.getColor();
		if (pAchievementGroup.achievements_list.Count <= 0)
		{
			return;
		}
		int tTotalUnlocked = 0;
		foreach (Achievement tAchievement in pAchievementGroup.achievements_list)
		{
			AchievementButton tButton = Object.Instantiate(achievementButtonPrefab, transformContent);
			tButton.Load(tAchievement);
			if (AchievementLibrary.isUnlocked(tAchievement))
			{
				tTotalUnlocked++;
			}
			_elements.Add(tButton);
		}
		counter.text = tTotalUnlocked + " / " + pAchievementGroup.achievements_list.Count;
	}
}
