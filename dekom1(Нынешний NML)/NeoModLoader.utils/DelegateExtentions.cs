using System;
using System.Reflection;

namespace NeoModLoader.utils;

public static class DelegateExtentions
{
	public static Type[] GetDelegateParameters(this Type delegateType)
	{
		MethodInfo method = delegateType.GetMethod("Invoke");
		ParameterInfo[] parameters = method.GetParameters();
		Type[] array = new Type[parameters.Length];
		for (int i = 0; i < parameters.Length; i++)
		{
			array[i] = parameters[i].ParameterType;
		}
		return array;
	}

	public static D AsDelegate<D>(this string String) where D : Delegate
	{
		return (D)String.AsDelegate(typeof(D));
	}

	public static Delegate AsDelegate(this string String, Type DelegateType = null)
	{
		if (String == null)
		{
			throw new ArgumentNullException("The String is null!");
		}
		if (String.Contains("&"))
		{
			string[] array = String.Split('&');
			if ((object)DelegateType == null)
			{
				DelegateType = Type.GetType(array[0]);
			}
			String = array[1];
		}
		string[] array2 = String.Split('+');
		Delegate[] array3 = new Delegate[array2.Length];
		Type[] types = DelegateType?.GetDelegateParameters() ?? throw new ArgumentException("The String Does Not Contain the delegate type!");
		for (int i = 0; i < array2.Length; i++)
		{
			string[] array4 = array2[i].Split(':');
			MethodInfo method = Type.GetType(array4[0]).GetMethod(array4[1], BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, types, null);
			array3[i] = method.CreateDelegate(DelegateType);
		}
		return Delegate.Combine(array3);
	}

	public static string AsString(this Delegate pDelegate, bool IncludeType = false)
	{
		Delegate[] array = pDelegate?.GetInvocationList() ?? throw new ArgumentNullException("The Delegate is null!");
		string[] array2 = new string[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			MethodInfo method = array[i].Method;
			array2[i] = method.DeclaringType.AssemblyQualifiedName + ":" + method.Name;
		}
		string text = string.Join("+", array2);
		if (IncludeType)
		{
			text = string.Join("&", pDelegate.GetType().AssemblyQualifiedName, text);
		}
		return text;
	}
}
