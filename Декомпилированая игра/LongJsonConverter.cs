using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class LongJsonConverter : JsonConverter
{
	internal static long next_long = 100000000L;

	internal static Dictionary<string, long> longs = new Dictionary<string, long>();

	public override bool CanWrite => false;

	public override bool CanRead => true;

	public static void reset()
	{
		next_long = 100000000L;
		longs.Clear();
	}

	public static long getLong(string pString, JsonReader pReader)
	{
		if (string.IsNullOrEmpty(pString))
		{
			return -1L;
		}
		string tString = pString;
		if (pString.IndexOf('_') > 0)
		{
			string[] tSplit = pString.Split('_');
			if (tSplit.Length == 2)
			{
				string tPrefix = tSplit[0] + "_";
				if (MapStats.possible_formats.IndexOf(tPrefix) > -1)
				{
					tString = tSplit[1];
				}
			}
		}
		if (long.TryParse(tString, out var result))
		{
			return result;
		}
		bool tIsGuid = pString.Length == 8 || (pString.Length == 36 && pString[8] == '-' && pString[13] == '-' && pString[18] == '-' && pString[23] == '-');
		if (!longs.TryGetValue(pString, out var tLong))
		{
			tLong = next_long++;
			longs[pString] = tLong;
			if (!tIsGuid)
			{
				Debug.LogWarning(pReader.Path + " Failed to parse long <b>" + pString + "</b> " + pString.Length + " -> " + tLong);
			}
		}
		else if (!tIsGuid)
		{
			Debug.LogWarning(pReader.Path + " Failed to parse long <b>" + pString + "</b> " + pString.Length + " -> " + tLong + " already had it");
		}
		return tLong;
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		switch (reader.TokenType)
		{
		case JsonToken.Null:
			return -1L;
		case JsonToken.Integer:
			return Convert.ToInt64(reader.Value);
		case JsonToken.String:
			return getLong((string)reader.Value, reader);
		default:
			Debug.LogWarning("Unhandled type " + reader.Path + " " + reader.Value?.ToString() + " " + reader.TokenType.ToString() + " -> " + -1L);
			return -1L;
		}
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		writer.WriteValue(value);
	}

	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(long);
	}
}
