using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphCategoriesContainer : MonoBehaviour
{
	public GraphController graph_controller;

	public GraphCategoryGroup category_group = GraphCategoryGroup.General;

	private GraphCategoryGroup _last_category_group;

	private GraphCategoryGroup _last_category_groups;

	private List<HistoryDataAsset> _current_list = new List<HistoryDataAsset>();

	private Dictionary<string, ButtonGraphCategory> _category_buttons = new Dictionary<string, ButtonGraphCategory>();

	private ButtonGraphCategory _prefab_button;

	private bool _is_initialized;

	[SerializeField]
	private TabTogglesGroup _category_groups;

	private void init()
	{
		if (!_is_initialized)
		{
			_is_initialized = true;
			ButtonGraphCategory[] componentsInChildren = GetComponentsInChildren<ButtonGraphCategory>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Object.Destroy(componentsInChildren[i].gameObject);
			}
			_prefab_button = Resources.Load<ButtonGraphCategory>("ui/graphs/GraphCategoryButton");
		}
	}

	public void apply()
	{
		init();
		List<HistoryDataAsset> tCategoriesList = graph_controller.getCategories();
		if (_last_category_group == category_group && _current_list.Count > 0 && _current_list.Count == tCategoriesList.Count && _current_list.All(tCategoriesList.Contains) && tCategoriesList.All(_current_list.Contains))
		{
			foreach (ButtonGraphCategory tButton in _category_buttons.Values)
			{
				graph_controller.setCategoryEnabled(tButton.gameObject.name, tButton.is_on, pUpdateGraph: false);
			}
			return;
		}
		foreach (ButtonGraphCategory value in _category_buttons.Values)
		{
			value.gameObject.SetActive(value: false);
		}
		GraphCategoryGroup tCategories = GraphCategoryGroup.None;
		_current_list = new List<HistoryDataAsset>(tCategoriesList);
		foreach (HistoryDataAsset tCategory in _current_list)
		{
			tCategories |= tCategory.category_group;
			if (tCategory.category_group.HasFlag(category_group))
			{
				if (!_category_buttons.TryGetValue(tCategory.id, out var tCategoryButton))
				{
					tCategoryButton = Object.Instantiate(_prefab_button, base.transform);
					tCategoryButton.gameObject.name = tCategory.id;
					tCategoryButton.transform.SetParent(base.transform);
					tCategoryButton.init();
					tCategoryButton.setAsset(tCategory);
					tCategoryButton.is_on = graph_controller.isCategoryEnabled(tCategory.id);
					_category_buttons.Add(tCategory.id, tCategoryButton);
				}
				tCategoryButton.gameObject.SetActive(value: true);
			}
		}
		_last_category_group = category_group;
		showCategoryGroups(tCategories);
	}

	private void showCategoryGroups(GraphCategoryGroup pGroups)
	{
		_category_groups.gameObject.SetActive(pGroups.Count() > 1);
		if (_last_category_groups == pGroups)
		{
			return;
		}
		_category_groups.clearButtons();
		if (pGroups.HasFlag(GraphCategoryGroup.General))
		{
			_category_groups.tryAddButton("ui/Icons/iconRenown", "tab_general_stats", apply, delegate
			{
				category_group = GraphCategoryGroup.General;
			});
		}
		if (pGroups.HasFlag(GraphCategoryGroup.Noosphere))
		{
			_category_groups.tryAddButton("ui/Icons/iconKnowledge", "tab_noosphere", apply, delegate
			{
				category_group = GraphCategoryGroup.Noosphere;
			});
		}
		if (pGroups.HasFlag(GraphCategoryGroup.Deaths))
		{
			_category_groups.tryAddButton("civ/map_mark_death", "tab_deaths", apply, delegate
			{
				category_group = GraphCategoryGroup.Deaths;
			});
		}
		if (pGroups.HasFlag(GraphCategoryGroup.Biomes))
		{
			_category_groups.tryAddButton("ui/Icons/iconSeedClover", "tab_biomes", apply, delegate
			{
				category_group = GraphCategoryGroup.Biomes;
			});
		}
		if (pGroups.HasFlag(GraphCategoryGroup.Tiles))
		{
			_category_groups.tryAddButton("ui/Icons/iconZones", "tab_tiles", apply, delegate
			{
				category_group = GraphCategoryGroup.Tiles;
			});
		}
		_last_category_groups = pGroups;
		_category_groups.enableFirst();
	}

	public void setCategoryEnabled(string pId, bool pEnabled)
	{
		if (graph_controller.multi_chart)
		{
			foreach (ButtonGraphCategory value in _category_buttons.Values)
			{
				value.is_on = value.gameObject.name == pId;
			}
			graph_controller.disableAllCategories(pId);
		}
		graph_controller.setCategoryEnabled(pId, pEnabled, pUpdateGraph: false);
		graph_controller.adjustCharts();
	}
}
