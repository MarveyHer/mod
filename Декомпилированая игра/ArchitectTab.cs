using System.Collections.Generic;
using UnityEngine;

public class ArchitectTab : MonoBehaviour
{
	private Dictionary<ArchitectMood, ArchitectMoodButton> _buttons = new Dictionary<ArchitectMood, ArchitectMoodButton>();

	[SerializeField]
	private ArchitectMoodButton _mood_prefab;

	[SerializeField]
	private Transform _grid_placement;

	private void Awake()
	{
		initButtons();
	}

	private void initButtons()
	{
		for (int i = 0; i < AssetManager.architect_mood_library.list.Count; i++)
		{
			ArchitectMood tAsset = AssetManager.architect_mood_library.list[i];
			ArchitectMoodButton tButton = initButton(tAsset);
			_buttons.Add(tAsset, tButton);
		}
	}

	private ArchitectMoodButton initButton(ArchitectMood pAsset)
	{
		ArchitectMoodButton architectMoodButton = Object.Instantiate(_mood_prefab, _grid_placement);
		architectMoodButton.setAsset(pAsset);
		architectMoodButton.addClickCallback(buttonAction);
		return architectMoodButton;
	}

	private void buttonAction(ArchitectMoodButton pElement)
	{
		ArchitectMood tAsset = pElement.getAsset();
		World.world.map_stats.player_mood = tAsset.id;
		World.world.clearArchitectMood();
		updateElements();
	}

	private void updateElements()
	{
		ArchitectMood tCurrentMood = World.world.getArchitectMood();
		foreach (ArchitectMoodButton value in _buttons.Values)
		{
			bool tEnabled = value.getAsset() == tCurrentMood;
			value.toggleSelectedButton(tEnabled);
			value.setIconActiveColor(tEnabled);
		}
	}

	private void OnEnable()
	{
		updateElements();
	}
}
