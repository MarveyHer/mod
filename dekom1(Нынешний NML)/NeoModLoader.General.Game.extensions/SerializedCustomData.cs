using Newtonsoft.Json.Linq;

namespace NeoModLoader.General.Game.extensions;

public class SerializedCustomData
{
	public string ModId;

	public string DataVersion;

	public JObject Data;

	public SerializedCustomData(string modId, string dataVersion, JObject data)
	{
		ModId = modId;
		DataVersion = dataVersion;
		Data = data;
	}
}
