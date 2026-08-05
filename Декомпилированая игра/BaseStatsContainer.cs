using Newtonsoft.Json;
using UnityEngine;

public class BaseStatsContainer
{
	public string id;

	public float value;

	[JsonIgnore]
	public BaseStatAsset asset => AssetManager.base_stats_library.get(id);

	public void normalize()
	{
		BaseStatAsset tAsset = asset;
		if (tAsset.normalize)
		{
			value = Mathf.Clamp(value, tAsset.normalize_min, tAsset.normalize_max);
		}
	}
}
