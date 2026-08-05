using System;
using Newtonsoft.Json;

public class DelegateConverter : JsonConverter
{
	public override void WriteJson(JsonWriter pWriter, object pValue, JsonSerializer pSerializer)
	{
		if (pValue != null)
		{
			Delegate[] tDelegates = ((Delegate)pValue).GetInvocationList();
			string[] tDelegateNames = new string[tDelegates.Length];
			for (int i = 0; i < tDelegates.Length; i++)
			{
				tDelegateNames[i] = tDelegates[i].Method.DeclaringType?.ToString() + "." + tDelegates[i].Method.Name;
			}
			pSerializer.Serialize(pWriter, tDelegateNames, typeof(string[]));
		}
	}

	public override object ReadJson(JsonReader pReader, Type pObjectType, object pExistingValue, JsonSerializer pSerializer)
	{
		return null;
	}

	public override bool CanConvert(Type pObjectType)
	{
		if (pObjectType != null)
		{
			if (!(pObjectType == typeof(Delegate)))
			{
				return pObjectType.IsSubclassOf(typeof(Delegate));
			}
			return true;
		}
		return false;
	}
}
