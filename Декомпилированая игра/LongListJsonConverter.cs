using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class LongListJsonConverter : JsonConverter
{
	public override bool CanWrite => false;

	public override bool CanRead => true;

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		if (reader.TokenType == JsonToken.Null)
		{
			return null;
		}
		if (reader.TokenType == JsonToken.StartArray)
		{
			using ListPool<long> tList = new ListPool<long>();
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
				case JsonToken.Integer:
					tList.Add(Convert.ToInt64(reader.Value));
					break;
				case JsonToken.Null:
					tList.Add(-1L);
					break;
				case JsonToken.String:
				{
					string tString = (string)reader.Value;
					tList.Add(LongJsonConverter.getLong(tString, reader));
					break;
				}
				case JsonToken.EndArray:
					return new List<long>(tList);
				}
			}
		}
		Debug.LogWarning("Unhandled type " + reader.Path + " " + reader.Value?.ToString() + " " + reader.TokenType.ToString() + " -> null");
		return null;
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		writer.WriteValue(value);
	}

	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(List<long>);
	}
}
