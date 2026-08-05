using System;
using System.Collections;
using System.Reflection;

public static class FieldInfoExtensions
{
	public static bool isString(this FieldInfo pField)
	{
		return pField.FieldType == typeof(string);
	}

	public static bool isCollection(this FieldInfo pField)
	{
		Type tType = pField.FieldType;
		if (pField.isString())
		{
			return false;
		}
		return typeof(ICollection).IsAssignableFrom(tType);
	}

	public static bool isEnumerable(this FieldInfo pField)
	{
		Type tType = pField.FieldType;
		if (pField.isString())
		{
			return false;
		}
		return typeof(IEnumerable).IsAssignableFrom(tType);
	}

	public static bool isCloneable(this FieldInfo pField)
	{
		Type tType = pField.FieldType;
		if (pField.isString())
		{
			return false;
		}
		return typeof(ICloneable).IsAssignableFrom(tType);
	}
}
