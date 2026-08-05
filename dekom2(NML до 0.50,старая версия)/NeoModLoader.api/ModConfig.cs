using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoModLoader.services;
using Newtonsoft.Json;

namespace NeoModLoader.api;

public class ModConfig
{
	private readonly string _path;

	internal Dictionary<string, Dictionary<string, ModConfigItem>> _config = new Dictionary<string, Dictionary<string, ModConfigItem>>();

	public Dictionary<string, ModConfigItem> this[string pGroupId] => _config[pGroupId];

	public ModConfig(string path, bool pIsPersistent = false)
	{
		if (!File.Exists(path))
		{
			if (!pIsPersistent)
			{
				LogService.LogWarning("ModConfig file " + path + " does not exist, suggest to create one");
			}
			else
			{
				_path = path;
			}
			return;
		}
		string text = File.ReadAllText(path);
		Dictionary<string, List<ModConfigItem>> dictionary = JsonConvert.DeserializeObject<Dictionary<string, List<ModConfigItem>>>(text);
		if (dictionary == null)
		{
			if (!pIsPersistent)
			{
				LogService.LogWarning("ModConfig file " + path + " is empty or in invalid format!");
			}
			else
			{
				_path = path;
			}
			return;
		}
		_path = path;
		foreach (string key in dictionary.Keys)
		{
			CreateGroup(key);
			List<ModConfigItem> list = dictionary[key];
			foreach (ModConfigItem item in list)
			{
				_config[key][item.Id] = item;
				if (item.Type == ConfigItemType.SLIDER && item.MaxFloatVal < item.MinFloatVal)
				{
					item.SetFloatRange(item.MinFloatVal, item.MinFloatVal);
				}
				if (item.Type == ConfigItemType.INT_SLIDER && item.MaxIntVal < item.MinIntVal)
				{
					item.SetIntRange(item.MinIntVal, item.MinIntVal);
				}
				item.SetValue(item.GetValue(), !pIsPersistent);
			}
		}
	}

	public void MergeWith(ModConfig pDefaultConfig)
	{
		HashSet<string> hashSet = new HashSet<string>();
		foreach (string key in _config.Keys)
		{
			if (!pDefaultConfig._config.ContainsKey(key))
			{
				hashSet.Add(key);
				continue;
			}
			Dictionary<string, ModConfigItem> dictionary = _config[key];
			Dictionary<string, ModConfigItem> default_group = pDefaultConfig._config[key];
			HashSet<string> hashSet2 = new HashSet<string>();
			foreach (string item in dictionary.Keys.Where((string item) => !default_group.ContainsKey(item)))
			{
				hashSet2.Add(item);
			}
			foreach (string item2 in hashSet2)
			{
				dictionary.Remove(item2);
			}
		}
		foreach (string item3 in hashSet)
		{
			_config.Remove(item3);
		}
		foreach (string key2 in pDefaultConfig._config.Keys)
		{
			if (!_config.ContainsKey(key2))
			{
				_config[key2] = new Dictionary<string, ModConfigItem>();
			}
			Dictionary<string, ModConfigItem> group = _config[key2];
			Dictionary<string, ModConfigItem> dictionary2 = pDefaultConfig._config[key2];
			foreach (string item4 in dictionary2.Keys.Where((string item) => group.ContainsKey(item)))
			{
				group[item4].CallBack = dictionary2[item4].CallBack;
				if (group[item4].Type != dictionary2[item4].Type)
				{
					object obj = dictionary2[item4].GetValue();
					switch (dictionary2[item4].Type)
					{
					case ConfigItemType.SLIDER:
						switch (group[item4].Type)
						{
						case ConfigItemType.TEXT:
						{
							if (float.TryParse(obj.ToString(), out var result4))
							{
								obj = result4;
							}
							break;
						}
						case ConfigItemType.SWITCH:
							obj = (((bool)group[item4].GetValue()) ? 1 : 0);
							break;
						case ConfigItemType.INT_SLIDER:
							obj = (int)group[item4].GetValue();
							break;
						}
						group[item4].SetFloatRange(dictionary2[item4].MinFloatVal, dictionary2[item4].MaxFloatVal);
						break;
					case ConfigItemType.INT_SLIDER:
						switch (group[item4].Type)
						{
						case ConfigItemType.TEXT:
						{
							if (int.TryParse(obj.ToString(), out var result3))
							{
								obj = result3;
							}
							break;
						}
						case ConfigItemType.SWITCH:
							obj = (((bool)group[item4].GetValue()) ? 1 : 0);
							break;
						case ConfigItemType.SLIDER:
							obj = (float)group[item4].GetValue();
							break;
						}
						group[item4].SetIntRange(dictionary2[item4].MinIntVal, dictionary2[item4].MaxIntVal);
						break;
					case ConfigItemType.SWITCH:
						switch (group[item4].Type)
						{
						case ConfigItemType.TEXT:
						{
							if (bool.TryParse(obj.ToString(), out var result))
							{
								obj = result;
							}
							if (int.TryParse(obj.ToString(), out var result2))
							{
								obj = result2 != 0;
							}
							break;
						}
						case ConfigItemType.SLIDER:
							obj = (float)group[item4].GetValue() != 0f;
							break;
						case ConfigItemType.INT_SLIDER:
							obj = (int)group[item4].GetValue() != 0;
							break;
						}
						break;
					}
					AddConfigItem(key2, item4, dictionary2[item4].Type, obj, dictionary2[item4].IconPath, dictionary2[item4].CallBack);
				}
				else if (group[item4].Type == ConfigItemType.SLIDER)
				{
					group[item4].SetFloatRange(dictionary2[item4].MinFloatVal, dictionary2[item4].MaxFloatVal);
					float num = ((group[item4].GetValue() is float) ? ((float)group[item4].GetValue()) : 0f);
					if (num < dictionary2[item4].MinFloatVal || num > dictionary2[item4].MaxFloatVal)
					{
						group[item4].SetValue(dictionary2[item4].GetValue());
					}
				}
				else if (group[item4].Type == ConfigItemType.INT_SLIDER)
				{
					group[item4].SetIntRange(dictionary2[item4].MinIntVal, dictionary2[item4].MaxIntVal);
					float num2 = ((group[item4].GetValue() is int) ? ((int)group[item4].GetValue()) : 0);
					if (num2 < (float)dictionary2[item4].MinIntVal || num2 > (float)dictionary2[item4].MaxIntVal)
					{
						group[item4].SetValue(dictionary2[item4].GetValue());
					}
				}
			}
			foreach (string item5 in dictionary2.Keys.Where((string item) => !group.ContainsKey(item)))
			{
				if (dictionary2[item5].Type == ConfigItemType.SLIDER)
				{
					AddConfigSliderItemWithRange(key2, item5, (float)dictionary2[item5].GetValue(), dictionary2[item5].MinFloatVal, dictionary2[item5].MaxFloatVal, dictionary2[item5].IconPath, dictionary2[item5].CallBack);
				}
				else if (dictionary2[item5].Type == ConfigItemType.INT_SLIDER)
				{
					AddConfigSliderItemWithIntRange(key2, item5, (int)dictionary2[item5].GetValue(), dictionary2[item5].MinIntVal, dictionary2[item5].MaxIntVal, dictionary2[item5].IconPath, dictionary2[item5].CallBack);
				}
				else
				{
					AddConfigItem(key2, item5, dictionary2[item5].Type, dictionary2[item5].GetValue(), dictionary2[item5].IconPath, dictionary2[item5].CallBack);
				}
			}
		}
	}

	public void Save(string path = null)
	{
		if (path == null)
		{
			path = _path;
		}
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		Dictionary<string, List<ModConfigItem>> dictionary = new Dictionary<string, List<ModConfigItem>>();
		foreach (string key in _config.Keys)
		{
			Dictionary<string, ModConfigItem> dictionary2 = _config[key];
			dictionary[key] = new List<ModConfigItem>();
			foreach (KeyValuePair<string, ModConfigItem> item in dictionary2)
			{
				dictionary[key].Add(item.Value);
			}
		}
		string contents = JsonConvert.SerializeObject((object)dictionary);
		File.WriteAllText(path, contents);
	}

	public void CreateGroup(string pId)
	{
		if (_config.ContainsKey(pId))
		{
			LogService.LogWarning("ModConfigGroup " + pId + " already exists!");
			LogService.LogStackTraceAsWarning();
		}
		else
		{
			_config[pId] = new Dictionary<string, ModConfigItem>();
		}
	}

	public ModConfigItem AddConfigItem(string pGroupId, string pId, ConfigItemType pType, object pDefaultValue, string pIconPath = "", string pCallback = "")
	{
		if (!_config.TryGetValue(pGroupId, out var value))
		{
			value = new Dictionary<string, ModConfigItem>();
			_config[pGroupId] = value;
		}
		if (value.ContainsKey(pId))
		{
			LogService.LogWarning("ModConfigItem " + pId + " already exists in group " + pGroupId + "! Overwriting...");
			LogService.LogStackTraceAsWarning();
		}
		else
		{
			value[pId] = new ModConfigItem
			{
				Id = pId
			};
		}
		value[pId].Type = pType;
		value[pId].CallBack = pCallback;
		value[pId].SetValue(pDefaultValue);
		value[pId].IconPath = pIconPath;
		return value[pId];
	}

	public ModConfigItem AddConfigSliderItemWithRange(string pGroupId, string pId, float pDefaultValue, float pMinValue, float pMaxValue, string pIconPath = "", string pCallback = "")
	{
		if (!_config.TryGetValue(pGroupId, out var value))
		{
			value = new Dictionary<string, ModConfigItem>();
			_config[pGroupId] = value;
		}
		if (value.ContainsKey(pId))
		{
			LogService.LogWarning("ModConfigItem " + pId + " already exists in group " + pGroupId + "! Overwriting...");
			LogService.LogStackTraceAsWarning();
		}
		else
		{
			value[pId] = new ModConfigItem
			{
				Id = pId
			};
		}
		value[pId].Type = ConfigItemType.SLIDER;
		value[pId].CallBack = pCallback;
		value[pId].SetFloatRange(pMinValue, pMaxValue);
		value[pId].SetValue(pDefaultValue);
		value[pId].IconPath = pIconPath;
		return value[pId];
	}

	public ModConfigItem AddConfigSliderItemWithIntRange(string pGroupId, string pId, int pDefaultValue, int pMinValue, int pMaxValue, string pIconPath = "", string pCallback = "")
	{
		if (!_config.TryGetValue(pGroupId, out var value))
		{
			value = new Dictionary<string, ModConfigItem>();
			_config[pGroupId] = value;
		}
		if (value.ContainsKey(pId))
		{
			LogService.LogWarning("ModConfigItem " + pId + " already exists in group " + pGroupId + "! Overwriting...");
			LogService.LogStackTraceAsWarning();
		}
		else
		{
			value[pId] = new ModConfigItem
			{
				Id = pId
			};
		}
		value[pId].Type = ConfigItemType.INT_SLIDER;
		value[pId].CallBack = pCallback;
		value[pId].SetIntRange(pMinValue, pMaxValue);
		value[pId].SetValue(pDefaultValue);
		value[pId].IconPath = pIconPath;
		return value[pId];
	}
}
