using System.Collections.Generic;
using ChartAndGraph;
using db;
using db.tables;
using UnityEngine;

public class GraphController : MonoBehaviour
{
	public static MinMax min_max;

	public GraphChart chart;

	[SerializeField]
	private MetaType _meta_type = MetaType.City;

	private GraphCategoriesContainer _container_graph_categories;

	private GraphTimeScaleContainer _container_time_scale;

	public bool clear_on_enable;

	public bool multi_chart;

	private VerticalAxis _vertical_axis;

	private HorizontalAxis _horizontal_axis;

	private List<HistoryDataAsset> _list_categories = new List<HistoryDataAsset>();

	private Dictionary<string, bool> _category_enabled = new Dictionary<string, bool>();

	private Dictionary<string, MinMax> _min_max_categories = new Dictionary<string, MinMax>();

	private long _min_timestamp = long.MinValue;

	private long _max_timestamp = long.MaxValue;

	private HashSet<MetaType> _current_types = new HashSet<MetaType>();

	private List<NanoObject> _current_objects = new List<NanoObject>();

	private Dictionary<NanoObject, HistoryTable> _last_data = new Dictionary<NanoObject, HistoryTable>();

	private GraphTimeAsset _current_sample;

	private HistoryInterval _current_interval;

	private Dictionary<string, CategoryData> _current_datas = new Dictionary<string, CategoryData>();

	private bool _events_hooked;

	private bool _loaded;

	private bool _categories_loaded;

	private long _last_timestamp = -1L;

	private void Awake()
	{
		_container_time_scale = base.transform.GetComponentInChildren<GraphTimeScaleContainer>();
		_vertical_axis = base.transform.GetComponentInChildren<VerticalAxis>();
		_horizontal_axis = base.transform.GetComponentInChildren<HorizontalAxis>();
		_container_graph_categories = base.transform.GetComponentInChildren<GraphCategoriesContainer>();
	}

	internal List<NanoObject> getObjects()
	{
		return _current_objects;
	}

	internal List<HistoryDataAsset> getCategories()
	{
		return _list_categories;
	}

	internal bool hasCategory(HistoryDataAsset pCategory)
	{
		return _list_categories.Contains(pCategory);
	}

	internal bool hasCategory(string pCategory)
	{
		HistoryDataAsset tCategory = AssetManager.history_data_library.get(pCategory);
		return hasCategory(tCategory);
	}

	private static string getCategoryName(string pCategory)
	{
		return GraphHelpers.getCategoryName(pCategory);
	}

	private NanoObject extractObject(string pCategory)
	{
		if (!pCategory.Contains('|'))
		{
			return null;
		}
		_ = pCategory.Split('|')[0];
		string tType = pCategory.Split('|')[1];
		string tTypeID = pCategory.Split('|')[2];
		foreach (NanoObject tObject in _current_objects)
		{
			if (tObject.getType() == tType && tObject.getTypeID() == tTypeID)
			{
				return tObject;
			}
		}
		return null;
	}

	internal bool isCategoryEnabled(string pCategory)
	{
		string tCategoryName = getCategoryName(pCategory);
		return _category_enabled[tCategoryName];
	}

	internal string getActiveCategory()
	{
		foreach (string tCategory in _category_enabled.Keys)
		{
			if (_category_enabled[tCategory])
			{
				return tCategory;
			}
		}
		return null;
	}

	private void loadCategories()
	{
		if (_categories_loaded)
		{
			return;
		}
		_categories_loaded = true;
		_list_categories.Clear();
		HashSet<HistoryDataAsset> tCommonCategories = new HashSet<HistoryDataAsset>();
		foreach (MetaType tType in _current_types)
		{
			HistoryMetaDataAsset[] assets = AssetManager.history_meta_data_library.getAssets(tType);
			HashSet<HistoryDataAsset> tMetaCategories = new HashSet<HistoryDataAsset>();
			HistoryMetaDataAsset[] array = assets;
			foreach (HistoryMetaDataAsset tHistoryAsset in array)
			{
				tMetaCategories.UnionWith(tHistoryAsset.categories);
			}
			if (tCommonCategories.Count == 0)
			{
				tCommonCategories.UnionWith(tMetaCategories);
			}
			else
			{
				tCommonCategories.IntersectWith(tMetaCategories);
			}
		}
		foreach (NanoObject tCurrentObject in _current_objects)
		{
			foreach (HistoryDataAsset tDataAsset in tCommonCategories)
			{
				if (!hasCategory(tDataAsset))
				{
					addCategory(tDataAsset, tDataAsset.enabled_default);
				}
				colorCategory(tDataAsset, tCurrentObject, multi_chart);
			}
		}
	}

	internal void addCategory(HistoryDataAsset pAsset, bool pEnabled = false)
	{
		_list_categories.Add(pAsset);
		_category_enabled[pAsset.id] = pEnabled;
	}

	internal void disableAllCategories(string pExcept = null)
	{
		foreach (HistoryDataAsset tCategory in getCategories())
		{
			if (!(tCategory.id == pExcept))
			{
				setCategoryEnabled(tCategory.id, pIsOn: false, pUpdateGraph: false);
			}
		}
	}

	internal void pickRandomCategory()
	{
		using ListPool<string> tBestCategories = GraphHelpers.bestCategories(_min_max_categories);
		if (tBestCategories.Count != 0)
		{
			string tCategory = tBestCategories.GetRandom();
			tryEnableCategory(tCategory);
		}
	}

	internal void tryEnableCategory(string pCategory)
	{
		if (!string.IsNullOrEmpty(pCategory) && hasCategory(pCategory))
		{
			_container_graph_categories.setCategoryEnabled(pCategory, pEnabled: true);
		}
	}

	internal void setCategoryEnabled(string pCategory, bool pIsOn, bool pUpdateGraph = true)
	{
		_category_enabled[pCategory] = pIsOn;
		foreach (string tCategory in chart.DataSource.CategoryNames)
		{
			if (tCategory.StartsWith(pCategory + "|"))
			{
				chart.DataSource.SetCategoryEnabled(tCategory, pIsOn);
			}
		}
		if (pUpdateGraph)
		{
			updateGraph();
		}
	}

	private void hookEvents()
	{
		if (!_events_hooked)
		{
			_events_hooked = true;
			chart.PointHovered.AddListener(delegate
			{
				Tooltip.cancelHiding();
			});
			if (multi_chart)
			{
				chart.PointHovered.AddListener(multiChartHover);
			}
			else
			{
				chart.PointHovered.AddListener(singleChartHover);
			}
			chart.NonHovered.AddListener(delegate
			{
				Tooltip.scheduledHide(0.15f, pSkipTouch: true);
			});
		}
	}

	private void multiChartHover(GraphChartBase.GraphEventArgs pArgs)
	{
		long tYear = (long)pArgs.Value.x;
		string tCategoryName = getCategoryName(pArgs.Category);
		if (Tooltip.anyActive())
		{
			Tooltip tTooltip = Tooltip.findActive(delegate(Tooltip pTooltip)
			{
				if (pTooltip.asset.id != "graph_multi_resource")
				{
					return false;
				}
				return !(pTooltip.data.tip_name != tCategoryName) && pTooltip.data.custom_data_long["year"] == tYear;
			});
			if (tTooltip != null)
			{
				tTooltip.reposition();
				return;
			}
		}
		CustomDataContainer<string> tColorData = new CustomDataContainer<string>();
		CustomDataContainer<long> tValueData = new CustomDataContainer<long>();
		tValueData["year"] = tYear;
		foreach (string tCategory in chart.DataSource.CategoryNames)
		{
			if (isCategoryEnabled(tCategory))
			{
				NanoObject tObject = extractObject(tCategory);
				string tObjectName = tObject.name;
				(long tValue, long tPrevious) categoryValueAtTime = getCategoryValueAtTime(tCategory, (long)pArgs.Value.x);
				long tValue = categoryValueAtTime.tValue;
				long tPrevious = categoryValueAtTime.tPrevious;
				tValueData[tObjectName] = tValue;
				tValueData[tObjectName + "_previous"] = tPrevious;
				tColorData[tObjectName] = Toolbox.colorToHex(tObject.getColor().getColorText());
			}
		}
		Tooltip.show(pArgs.Position, "graph_multi_resource", new TooltipData
		{
			tip_name = tCategoryName,
			custom_data_long = tValueData,
			custom_data_string = tColorData
		});
	}

	private void singleChartHover(GraphChartBase.GraphEventArgs pArgs)
	{
		Tooltip.hideTooltip();
		CustomDataContainer<long> tCustomData = new CustomDataContainer<long>();
		tCustomData["year"] = (long)pArgs.Value.x;
		foreach (string tCategory in chart.DataSource.CategoryNames)
		{
			if (isCategoryEnabled(tCategory))
			{
				string tCategoryName = getCategoryName(tCategory);
				(long tValue, long tPrevious) categoryValueAtTime = getCategoryValueAtTime(tCategory, (long)pArgs.Value.x);
				long tValue = categoryValueAtTime.tValue;
				long tPrevious = categoryValueAtTime.tPrevious;
				tCustomData[tCategoryName] = tValue;
				tCustomData[tCategoryName + "_previous"] = tPrevious;
			}
		}
		NanoObject tObject = extractObject(pArgs.Category);
		Tooltip.show(pArgs.Position, "graph_resource", new TooltipData
		{
			custom_data_long = tCustomData,
			nano_object = tObject
		});
	}

	public void resetAndUpdateGraph()
	{
		_loaded = false;
		_categories_loaded = false;
		_current_interval = HistoryInterval.None;
		_container_time_scale.resetTimeScale();
		updateGraph();
		_container_time_scale.calcBounds();
	}

	public bool randomTimeScale()
	{
		if (_container_time_scale.randomizeTimeScale())
		{
			updateGraph();
			return true;
		}
		return false;
	}

	public void forceUpdateGraph()
	{
		updateGraph();
	}

	private void updateGraph()
	{
		if (!Config.disable_db && Config.graphs)
		{
			chart.DataSource.StartBatch();
			loadGraph();
			if (_container_time_scale.resetTimeScale())
			{
				clearChartData();
			}
			loadSample();
			loadCategoryAndCharts();
			adjustCharts();
			chart.DataSource.EndBatch();
		}
	}

	private void loadGraph()
	{
		if (!_loaded)
		{
			_loaded = true;
			chart.GetComponent<HorizontalAxis>().CustomNumberFormatWorldbox = GraphHelpers.horizontalFormatYears;
			chart.CustomNumberFormat = GraphHelpers.verticalFormat;
			_vertical_axis.enabled = true;
			_horizontal_axis.enabled = true;
			if (multi_chart)
			{
				loadMultiChart();
			}
			else
			{
				loadSingleChart();
			}
			hookEvents();
		}
	}

	private void loadSingleChart()
	{
		NanoObject tSelectedObject = AssetManager.meta_type_library.getAsset(_meta_type).get_selected();
		selectContainer(tSelectedObject);
	}

	private void loadMultiChart()
	{
		_current_types.Clear();
		_current_objects.Clear();
		_last_data.Clear();
		foreach (NanoObject tObject in Config.selected_objects_graph)
		{
			if (tObject != null && tObject.isAlive())
			{
				addContainer(tObject);
			}
		}
		clearChartData();
		_category_enabled.Clear();
	}

	private void showCategory(string pCategory, NanoObject pObject)
	{
		string tType = pObject.getType();
		string tTypeID = pObject.getTypeID();
		CategoryData tData = _current_datas[tTypeID];
		string tCategoryName = pCategory + "|" + tType + "|" + tTypeID;
		for (LinkedListNode<Dictionary<string, long>> tNode = tData.Last; tNode != null; tNode = tNode.Previous)
		{
			if (tNode.Value.ContainsKey(pCategory))
			{
				long tValue = tNode.Value[pCategory];
				long tTimestamp = tNode.Value["timestamp"];
				bool tHide = false;
				long tPrevValue = tNode.Previous?.Value[pCategory] ?? 0;
				long tNextValue = tNode.Next?.Value[pCategory] ?? 0;
				if (tValue == tPrevValue && tValue == tNextValue)
				{
					tHide = true;
				}
				chart.DataSource.AddPointToCategory(tCategoryName, tTimestamp, tValue, tHide ? 0f : (-1f));
			}
		}
	}

	private (long tValue, long tPrevious) getCategoryValueAtTime(string pCategory, long pTime)
	{
		string tCategory = getCategoryName(pCategory);
		string tTypeID = pCategory.Split('|').Last();
		CategoryData categoryData = _current_datas[tTypeID];
		long tValue = 0L;
		long tPrevious = 0L;
		bool tFound = false;
		for (LinkedListNode<Dictionary<string, long>> tNode = categoryData.Last; tNode != null; tNode = tNode.Previous)
		{
			if (tNode.Value.ContainsKey(tCategory))
			{
				if (tFound)
				{
					tPrevious = tNode.Value[tCategory];
					break;
				}
				long tTimestamp = tNode.Value["timestamp"];
				if (tTimestamp <= pTime)
				{
					if (tTimestamp <= pTime)
					{
						tValue = tNode.Value[tCategory];
					}
					tFound = true;
				}
			}
		}
		return (tValue: tValue, tPrevious: tPrevious);
	}

	private void colorCategory(HistoryDataAsset pHistoryDataAsset, NanoObject pObject, bool pColorFromObject = false)
	{
		string tType = pObject.getType();
		string tTypeID = pObject.getTypeID();
		string tCategory = pHistoryDataAsset.id;
		string tCategoryName = tCategory + "|" + tType + "|" + tTypeID;
		float tLineThickness = 2f;
		MaterialTiling tTiling = new MaterialTiling
		{
			EnableTiling = false
		};
		bool tStretchFill = true;
		Material tChartLineMaterial;
		Material tChartInnerFillMaterial;
		if (pColorFromObject)
		{
			Color colorText = pObject.getColor().getColorText();
			tChartLineMaterial = HistoryDataAsset.getChartLineMaterial(colorText);
			tChartInnerFillMaterial = HistoryDataAsset.getChartInnerFillMaterial(colorText);
		}
		else
		{
			tChartLineMaterial = pHistoryDataAsset.getChartLineMaterial();
			tChartInnerFillMaterial = pHistoryDataAsset.getChartInnerFillMaterial();
		}
		chart.DataSource.AddCategory(tCategoryName, tChartLineMaterial, tLineThickness, tTiling, tChartInnerFillMaterial, tStretchFill, null, 0.0);
		chart.DataSource.SetCategoryEnabled(tCategoryName, isCategoryEnabled(tCategory));
		chart.DataSource.Set2DCategoryPrefabs(tCategoryName, null, pHistoryDataAsset.getHoverPointMaterial());
		int tPointSize = 10;
		chart.DataSource.SetCategoryPoint(tCategoryName, pHistoryDataAsset.getChartPointMaterial(), tPointSize);
	}

	private MinMax getMinMax(string pCategoryName)
	{
		long tMin = long.MaxValue;
		long tMax = long.MinValue;
		bool tFound = false;
		string tCategory = pCategoryName.Split('|')[0];
		_ = pCategoryName.Split('|')[1];
		string tTypeID = pCategoryName.Split('|')[2];
		if (_current_datas.Count == 0 || !_current_datas.ContainsKey(tTypeID))
		{
			return new MinMax(0L, 0L);
		}
		for (LinkedListNode<Dictionary<string, long>> tNode = _current_datas[tTypeID].Last; tNode != null; tNode = tNode.Previous)
		{
			Dictionary<string, long> tData = tNode.Value;
			if (tData.ContainsKey(tCategory))
			{
				long tValue = tData[tCategory];
				long tTimestamp = tData["timestamp"];
				if (tFound && tTimestamp < _min_timestamp)
				{
					break;
				}
				if (tValue < tMin)
				{
					tMin = tValue;
				}
				if (tValue > tMax)
				{
					tMax = tValue;
				}
				tFound = true;
			}
		}
		if (!tFound)
		{
			return new MinMax(0L, 0L);
		}
		return new MinMax(tMin, tMax);
	}

	internal void adjustCharts()
	{
		long tMinValue = long.MaxValue;
		long tMaxValue = 0L;
		_min_max_categories.Clear();
		foreach (string tCategory in chart.DataSource.CategoryNames)
		{
			MinMax tMinMax = getMinMax(tCategory);
			_min_max_categories.Add(tCategory, tMinMax);
			if (isCategoryEnabled(tCategory))
			{
				if (tMinMax.max > tMaxValue)
				{
					tMaxValue = tMinMax.max;
				}
				if (tMinMax.min < tMinValue)
				{
					tMinValue = tMinMax.min;
				}
			}
		}
		tMaxValue = GraphHelpers.calculateNiceMaxAxisSize((double)tMaxValue * 1.05);
		int tVerticalDivisions = GraphHelpers.findVerticalDivision(tMaxValue);
		if (tMinValue >= 0)
		{
			tMinValue = 0L;
		}
		else
		{
			tMinValue = GraphHelpers.calculateNiceMaxAxisSize((double)(-tMinValue) * 1.05);
			if (tMinValue < tMaxValue)
			{
				long tMultiplier = tMaxValue / tVerticalDivisions;
				int tMinValueMultiplier = Mathf.CeilToInt((float)tMinValue / (float)tMultiplier);
				tMinValue = tMinValueMultiplier * tMultiplier;
				tVerticalDivisions += tMinValueMultiplier;
			}
			else
			{
				tVerticalDivisions = GraphHelpers.findVerticalDivision(tMinValue);
				long tMultiplier2 = tMinValue / tVerticalDivisions;
				int tMaxValueMultiplier = Mathf.CeilToInt((float)tMaxValue / (float)tMultiplier2);
				tMaxValue = tMaxValueMultiplier * tMultiplier2;
				tVerticalDivisions += tMaxValueMultiplier;
			}
		}
		chart.DataSource.VerticalViewOrigin = -tMinValue;
		chart.DataSource.VerticalViewSize = tMaxValue + tMinValue;
		chart.DataSource.HorizontalViewOrigin = GraphTimeLibrary.getMinTime(_current_sample);
		chart.DataSource.HorizontalViewSize = GraphTimeLibrary.getMaxTime(_current_sample) - GraphTimeLibrary.getMinTime(_current_sample);
		_horizontal_axis.MainDivisions.Total = 5;
		_horizontal_axis.MainDivisions.FractionDigits = 2;
		_vertical_axis.MainDivisions.Total = tVerticalDivisions;
		min_max = new MinMax(-tMinValue, tMaxValue);
	}

	private void loadSample()
	{
		GraphTimeScale tScale = _container_time_scale.getCurrentScale();
		_current_sample = AssetManager.graph_time_library.get(tScale.ToString());
		bool tClear = false;
		_current_interval = _current_sample.interval;
		foreach (NanoObject tCurrentObject in _current_objects)
		{
			string tTypeID = tCurrentObject.getTypeID();
			if (!_current_datas.TryGetValue(tTypeID, out var tData))
			{
				tData = new CategoryData();
				_current_datas[tTypeID] = tData;
			}
			if (DBGetter.getData(tData, tCurrentObject, _current_interval, _last_data[tCurrentObject]))
			{
				tClear = true;
			}
		}
		if (tClear)
		{
			clearChartData();
		}
		_min_timestamp = GraphTimeLibrary.getMinTime(_current_sample);
		_max_timestamp = GraphTimeLibrary.getMaxTime(_current_sample);
	}

	private void clearChartData()
	{
		_categories_loaded = false;
		chart.DataSource.Clear();
	}

	private void loadCategoryAndCharts()
	{
		loadCategories();
		foreach (NanoObject tCurrentObject in _current_objects)
		{
			foreach (HistoryDataAsset list_category in _list_categories)
			{
				string tCategory = list_category.id;
				showCategory(tCategory, tCurrentObject);
			}
		}
		_container_graph_categories.apply();
	}

	private void selectContainer(NanoObject pMetaObject)
	{
		MetaType tMetaType = pMetaObject.getMetaType();
		if (!_current_types.Contains(tMetaType))
		{
			_category_enabled.Clear();
			clearChartData();
		}
		else if (!_current_objects.Contains(pMetaObject))
		{
			clearChartData();
		}
		_current_types.Clear();
		_current_objects.Clear();
		_last_data.Clear();
		_current_types.Add(tMetaType);
		_current_objects.Add(pMetaObject);
		HistoryMetaDataAsset[] assets = AssetManager.history_meta_data_library.getAssets(tMetaType);
		foreach (HistoryMetaDataAsset tHistoryAsset in assets)
		{
			_last_data[pMetaObject] = tHistoryAsset.collector(pMetaObject);
			_last_data[pMetaObject].timestamp = Date.getCurrentYear();
		}
	}

	private void addContainer(NanoObject pMetaObject)
	{
		MetaType tMetaType = pMetaObject.getMetaType();
		_current_types.Add(tMetaType);
		_current_objects.Add(pMetaObject);
		HistoryMetaDataAsset[] assets = AssetManager.history_meta_data_library.getAssets(tMetaType);
		foreach (HistoryMetaDataAsset tHistoryAsset in assets)
		{
			_last_data[pMetaObject] = tHistoryAsset.collector(pMetaObject);
			_last_data[pMetaObject].timestamp = Date.getCurrentYear();
		}
	}

	private void clearGraph()
	{
		clearChartData();
		_category_enabled.Clear();
		_list_categories.Clear();
		_loaded = false;
		_container_graph_categories.apply();
	}

	internal void load()
	{
		_loaded = false;
		if (multi_chart)
		{
			_current_interval = HistoryInterval.None;
			_container_time_scale.resetTimeScale();
		}
		if (clear_on_enable)
		{
			clearGraph();
		}
		if (_last_timestamp != Date.getMonthsSince(0.0))
		{
			_last_timestamp = Date.getMonthsSince(0.0);
			foreach (CategoryData value in _current_datas.Values)
			{
				value.Dispose();
			}
			_current_datas.Clear();
			clearChartData();
		}
		updateGraph();
		_container_time_scale.calcBounds();
	}

	private void clear()
	{
		_current_types.Clear();
		_current_objects.Clear();
		_last_data.Clear();
	}

	private void OnEnable()
	{
		load();
	}

	private void OnDisable()
	{
		clear();
	}
}
