using System;
using Newtonsoft.Json;

public class BrushPixelDataConverter : JsonConverter
{
	public override void WriteJson(JsonWriter pWriter, object pValue, JsonSerializer pSerializer)
	{
		BrushPixelData tBrushPixelData = (BrushPixelData)pValue;
		string tString = tBrushPixelData.x + "," + tBrushPixelData.y + "," + tBrushPixelData.dist;
		pSerializer.Serialize(pWriter, tString, typeof(string));
	}

	public override object ReadJson(JsonReader pReader, Type pObjectType, object pExistingValue, JsonSerializer pSerializer)
	{
		string tString = pSerializer.Deserialize<string>(pReader);
		if (string.IsNullOrEmpty(tString))
		{
			return null;
		}
		int[] tArray = Array.ConvertAll(tString.Split(','), int.Parse);
		return new BrushPixelData(tArray[0], tArray[1], tArray[2]);
	}

	public override bool CanConvert(Type pObjectType)
	{
		if (pObjectType != null)
		{
			return pObjectType == typeof(BrushPixelData);
		}
		return false;
	}
}
