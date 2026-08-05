using System;
using System.Reflection;
using Newtonsoft.Json;

public class CustomDataContainerConverter : JsonConverter
{
	public override void WriteJson(JsonWriter pWriter, object pValue, JsonSerializer pSerializer)
	{
		FieldInfo field = pValue.GetType().GetField("dict", BindingFlags.Instance | BindingFlags.NonPublic);
		Type tDictType = field.FieldType;
		object tDictValue = field.GetValue(pValue);
		pSerializer.Serialize(pWriter, tDictValue, tDictType);
	}

	public override object ReadJson(JsonReader pReader, Type pObjectType, object pExistingValue, JsonSerializer pSerializer)
	{
		object tContainer = Activator.CreateInstance(pObjectType);
		FieldInfo field = pObjectType.GetField("dict", BindingFlags.Instance | BindingFlags.NonPublic);
		Type tDictType = field.FieldType;
		field.SetValue(tContainer, pSerializer.Deserialize(pReader, tDictType));
		return tContainer;
	}

	public override bool CanConvert(Type pObjectType)
	{
		return false;
	}
}
