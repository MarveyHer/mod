using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugKingdomFoes : MonoBehaviour
{
	[SerializeField]
	private DebugKingdomButton _prefab_button;

	[SerializeField]
	private Image _selector;

	[SerializeField]
	private GridLayoutGroup _grid_main;

	[SerializeField]
	private GridLayoutGroup _grid_civs;

	[SerializeField]
	private GridLayoutGroup _grid_minicivs;

	[SerializeField]
	private GridLayoutGroup _grid_minicivs_special;

	[SerializeField]
	private GridLayoutGroup _grid_concepts;

	[SerializeField]
	private GridLayoutGroup _grid_mobs;

	[SerializeField]
	private GridLayoutGroup _grid_creeps;

	[SerializeField]
	private GridLayoutGroup _grid_others;

	private List<DebugKingdomButton> _buttons = new List<DebugKingdomButton>();

	private KingdomAsset _current_selected;

	private bool _initialized;

	private void Awake()
	{
		create();
	}

	private void create()
	{
		if (_initialized)
		{
			return;
		}
		_initialized = true;
		AssetManager.kingdoms.checkForMissingTags();
		foreach (KingdomAsset tKingdomAsset in AssetManager.kingdoms.list)
		{
			if (!tKingdomAsset.isTemplateAsset())
			{
				DebugKingdomButton tNewButton = Object.Instantiate(parent: tKingdomAsset.group_main ? _grid_main.transform : (tKingdomAsset.group_creeps ? _grid_creeps.transform : (tKingdomAsset.concept ? _grid_concepts.transform : (tKingdomAsset.is_forced_by_trait ? _grid_others.transform : (tKingdomAsset.group_minicivs_cool ? _grid_minicivs_special.transform : (tKingdomAsset.group_miniciv ? _grid_minicivs.transform : (tKingdomAsset.civ ? _grid_civs.transform : ((!tKingdomAsset.mobs) ? _grid_others.transform : _grid_mobs.transform))))))), original: _prefab_button);
				tNewButton.setAsset(tKingdomAsset);
				_buttons.Add(tNewButton);
				tNewButton.GetComponent<Button>().onClick.AddListener(delegate
				{
					select(tNewButton);
				});
			}
		}
		select(_buttons.GetRandom());
	}

	private void select(DebugKingdomButton pButton)
	{
		_current_selected = pButton.kingdom_asset;
		_selector.transform.position = pButton.transform.position;
		updateButtons();
	}

	private void updateButtons()
	{
		foreach (DebugKingdomButton button in _buttons)
		{
			button.checkSelected(_current_selected);
		}
	}
}
