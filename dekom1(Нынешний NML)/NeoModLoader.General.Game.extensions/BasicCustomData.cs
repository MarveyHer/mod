using System;
using Newtonsoft.Json.Linq;

namespace NeoModLoader.General.Game.extensions;

public sealed class BasicCustomData<TDataClass> : ICustomData where TDataClass : class, new()
{
	public TDataClass Data { get; private set; } = new TDataClass();

	public BasicCustomData(TDataClass data)
	{
		if (data != null)
		{
			Data = data;
		}
	}

	public BasicCustomData()
		: this((TDataClass)null)
	{
	}

	public SerializedCustomData Serialize()
	{
		return new SerializedCustomData("UNKNOWN", "NO-VERSIONING-SUPPORT", JObject.FromObject((object)Data));
	}

	public void Deserialize(SerializedCustomData data)
	{
		if (data.ModId != "UNKNOWN" || data.DataVersion != "NO-VERSIONING-SUPPORT")
		{
			throw new Exception("Supplied data object is not compatible with the basic custom data serializer, mod ID or version mismatch");
		}
		Data = ((JToken)data.Data).ToObject<TDataClass>();
	}
}
