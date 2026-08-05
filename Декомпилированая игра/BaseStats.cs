using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class BaseStats : ICloneable
{
	[JsonProperty]
	private List<BaseStatsContainer> _stats_list = new List<BaseStatsContainer>();

	private Dictionary<string, BaseStatsContainer> _stats_dict = new Dictionary<string, BaseStatsContainer>();

	private List<BaseStatsContainer> _multipliers_list;

	[JsonProperty]
	private HashSet<string> _tags;

	public float this[string pKey]
	{
		get
		{
			return get(pKey);
		}
		set
		{
			set(pKey, value);
		}
	}

	private void set(string pID, float pAmount)
	{
		BaseStatAsset tAsset = AssetManager.base_stats_library.get(pID);
		if (tAsset.ignore)
		{
			return;
		}
		BaseStatsContainer tStatsContainer;
		if (pAmount == 0f && !tAsset.normalize)
		{
			if (_stats_dict.TryGetValue(pID, out var tRemoveStatsContainer))
			{
				if (tAsset.multiplier)
				{
					_multipliers_list?.Remove(tRemoveStatsContainer);
				}
				_stats_list.Remove(tRemoveStatsContainer);
				_stats_dict.Remove(pID);
			}
		}
		else if (!_stats_dict.TryGetValue(pID, out tStatsContainer))
		{
			tStatsContainer = new BaseStatsContainer();
			tStatsContainer.value = pAmount;
			tStatsContainer.id = pID;
			_stats_dict[pID] = tStatsContainer;
			_stats_list.Add(tStatsContainer);
			if (tAsset.multiplier)
			{
				if (_multipliers_list == null)
				{
					_multipliers_list = new List<BaseStatsContainer>();
				}
				_multipliers_list.Add(tStatsContainer);
			}
		}
		else
		{
			_stats_dict[pID].value = pAmount;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public List<BaseStatsContainer> getList()
	{
		return _stats_list;
	}

	public void checkStatName(string pID)
	{
		if (Config.game_loaded && AssetManager.base_stats_library.get(pID) == null)
		{
			Debug.LogError("base_stats_library.get() - no asset with id: " + pID);
			Debug.LogError("Only call stats with S.id, never with pure strings to avoid typos");
		}
	}

	public float get(string pID)
	{
		if (_stats_dict.TryGetValue(pID, out var tContainer))
		{
			return tContainer.value;
		}
		return 0f;
	}

	public bool hasStat(string pID)
	{
		return _stats_dict.ContainsKey(pID);
	}

	public BaseStatsContainer getContainer(string pID)
	{
		BaseStatsContainer tContainer = null;
		_stats_dict.TryGetValue(pID, out tContainer);
		return tContainer;
	}

	internal void mergeStats(BaseStats pStats, float pMultiplier = 1f)
	{
		for (int i = 0; i < pStats._stats_list.Count; i++)
		{
			BaseStatsContainer tStat = pStats._stats_list[i];
			this[tStat.id] += tStat.value * pMultiplier;
		}
		if (pStats._tags != null)
		{
			if (_tags == null)
			{
				_tags = new HashSet<string>(pStats._tags);
			}
			else
			{
				_tags.UnionWith(pStats._tags);
			}
		}
	}

	public void checkMultipliers()
	{
		if (_multipliers_list == null)
		{
			return;
		}
		for (int i = 0; i < _multipliers_list.Count; i++)
		{
			BaseStatsContainer tMultiplierContainer = _multipliers_list[i];
			BaseStatAsset tAssetMultiplier = tMultiplierContainer.asset;
			BaseStatsContainer tContainerToMultiply = getContainer(tAssetMultiplier.main_stat_to_multiply);
			if (tContainerToMultiply != null)
			{
				tContainerToMultiply.value += tContainerToMultiply.value * tMultiplierContainer.value;
			}
		}
	}

	public bool hasTag(string pTag)
	{
		return _tags?.Contains(pTag) ?? false;
	}

	public bool hasTags(string[] pTags)
	{
		return _tags?.Overlaps(pTags) ?? false;
	}

	public void normalize()
	{
		for (int i = 0; i < _stats_list.Count; i++)
		{
			_stats_list[i].normalize();
		}
	}

	internal void clear()
	{
		_multipliers_list?.Clear();
		_stats_list.Clear();
		_stats_dict.Clear();
		_tags?.Clear();
	}

	public void reset()
	{
		clear();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void addTag(string pTag)
	{
		if (_tags == null)
		{
			_tags = new HashSet<string>();
		}
		_tags.Add(pTag);
	}

	public bool hasTags()
	{
		HashSet<string> tags = _tags;
		if (tags == null)
		{
			return false;
		}
		return tags.Count > 0;
	}

	public bool hasStats()
	{
		List<BaseStatsContainer> stats_list = _stats_list;
		if (stats_list == null)
		{
			return false;
		}
		return stats_list.Count > 0;
	}

	public bool ShouldSerialize_tags()
	{
		return hasTags();
	}

	public bool ShouldSerialize_stats_list()
	{
		return hasStats();
	}

	public void addCombatAction(string pCombatAction)
	{
	}

	public object Clone()
	{
		BaseStats baseStats = new BaseStats();
		baseStats.mergeStats(this);
		return baseStats;
	}
}
