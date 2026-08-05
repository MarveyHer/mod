using System.Collections.Generic;
using UnityEngine;

public class DragonAsset : ScriptableObject
{
	private Dictionary<DragonState, DragonAssetContainer> _dict;

	public DragonAssetContainer[] list;

	public DragonAssetContainer getAsset(DragonState pState)
	{
		if (_dict == null)
		{
			_dict = new Dictionary<DragonState, DragonAssetContainer>();
			DragonAssetContainer[] array = list;
			foreach (DragonAssetContainer tContainer in array)
			{
				_dict.Add(tContainer.id, tContainer);
			}
		}
		return _dict[pState];
	}
}
