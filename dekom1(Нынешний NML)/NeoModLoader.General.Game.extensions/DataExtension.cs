using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeoModLoader.General.Game.extensions;

public static class DataExtension
{
	public static bool TryGet<TCustomData>(this BaseSystemData data, string key, out TCustomData result) where TCustomData : ICustomData, new()
	{
		result = new TCustomData();
		data.get(key, out var pResult, null);
		if (pResult == null)
		{
			return false;
		}
		JObject val;
		try
		{
			val = JObject.Parse(pResult);
		}
		catch (JsonReaderException)
		{
			return false;
		}
		SerializedCustomData serializedCustomData = ((JToken)val).ToObject<SerializedCustomData>();
		if (serializedCustomData == null)
		{
			return false;
		}
		result.Deserialize(serializedCustomData);
		return true;
	}

	public static void Set<TCustomData>(this BaseSystemData data, string key, TCustomData value) where TCustomData : ICustomData
	{
		data.set(key, JsonConvert.SerializeObject((object)value.Serialize()));
	}
}
