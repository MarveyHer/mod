using System;
using System.Collections.Generic;
using System.Reflection;
using NeoModLoader.api.attributes;
using NeoModLoader.services;
using NeoModLoader.utils;

namespace NeoModLoader.General;

[Experimental("This helper class is experimental. Maybe some errors will occur.")]
public static class RF
{
	private static Dictionary<Type, Dictionary<string, Delegate>> _method_cache = new Dictionary<Type, Dictionary<string, Delegate>>();

	private static Dictionary<Type, Dictionary<string, Delegate>> _getter_cache = new Dictionary<Type, Dictionary<string, Delegate>>();

	private static Dictionary<Type, Dictionary<string, Delegate>> _setter_cache = new Dictionary<Type, Dictionary<string, Delegate>>();

	public static Delegate GetMethodDelegate(this Type type, string name, bool is_static = false)
	{
		if (_method_cache.TryGetValue(type, out var value))
		{
			if (value.TryGetValue(name, out var value2))
			{
				return value2;
			}
			Delegate method = ReflectionHelper.GetMethod(type, name, is_static);
			value.Add(name, method);
			return method;
		}
		Delegate method2 = ReflectionHelper.GetMethod(type, name, is_static);
		_method_cache.Add(type, new Dictionary<string, Delegate> { { name, method2 } });
		return method2;
	}

	public static TF GetField<TF, TI>(this TI obj, string name)
	{
		if (_getter_cache.TryGetValue(typeof(TI), out var value))
		{
			if (value.TryGetValue(name, out var value2))
			{
				return ((Func<TI, TF>)value2)(obj);
			}
			Func<TI, TF> func = ReflectionHelper.CreateFieldGetter<TI, TF>(name);
			value.Add(name, func);
			return func(obj);
		}
		Func<TI, TF> func2 = ReflectionHelper.CreateFieldGetter<TI, TF>(name);
		_getter_cache.Add(typeof(TI), new Dictionary<string, Delegate> { { name, func2 } });
		return func2(obj);
	}

	public static TF GetField<TF>(this object obj, string name)
	{
		Type type = obj.GetType();
		if (_getter_cache.TryGetValue(type, out var value))
		{
			if (value.TryGetValue(name, out var value2))
			{
				return (TF)value2.DynamicInvoke(obj);
			}
			Delegate obj2 = ReflectionHelper.CreateFieldGetter<TF>(name, type);
			value.Add(name, obj2);
			return (TF)obj2.DynamicInvoke(obj);
		}
		Delegate obj3 = ReflectionHelper.CreateFieldGetter<TF>(name, type);
		_getter_cache.Add(type, new Dictionary<string, Delegate> { { name, obj3 } });
		return (TF)obj3.DynamicInvoke(obj);
	}

	public static object GetField(this object obj, string name, Type field_type)
	{
		Type type = obj.GetType();
		if (_getter_cache.TryGetValue(type, out var value))
		{
			if (value.TryGetValue(name, out var value2))
			{
				return value2.DynamicInvoke(obj);
			}
			Delegate obj2 = ReflectionHelper.CreateFieldGetter(name, type, field_type);
			value.Add(name, obj2);
			return obj2.DynamicInvoke(obj);
		}
		Delegate obj3 = ReflectionHelper.CreateFieldGetter(name, type, field_type);
		_getter_cache.Add(type, new Dictionary<string, Delegate> { { name, obj3 } });
		return obj3.DynamicInvoke(obj);
	}

	public static TF GetStaticField<TF, TI>(string name)
	{
		FieldInfo field = typeof(TI).GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (field != null)
		{
			return (TF)field.GetValue(null);
		}
		LogService.LogWarning("Cannot find '" + name + "' in type " + typeof(TI).FullName + ". Return default value.");
		try
		{
			throw new Exception();
		}
		catch (Exception ex)
		{
			LogService.LogWarning(ex.StackTrace);
		}
		return default(TF);
	}

	public static TF GetStaticField<TF>(string name, Type type)
	{
		FieldInfo field = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (field != null)
		{
			return (TF)field.GetValue(null);
		}
		LogService.LogWarning("Cannot find '" + name + "' in type " + type.FullName + ". Return default value.");
		try
		{
			throw new Exception();
		}
		catch (Exception ex)
		{
			LogService.LogWarning(ex.StackTrace);
		}
		return default(TF);
	}

	public static void SetField<TF, TI>(this TI obj, string name, TF value)
	{
		if (_setter_cache.TryGetValue(typeof(TI), out var value2))
		{
			if (value2.TryGetValue(name, out var value3))
			{
				((Action<TI, TF>)value3)(obj, value);
				return;
			}
			Action<TI, TF> action = ReflectionHelper.CreateFieldSetter<TI, TF>(name);
			value2.Add(name, action);
			action(obj, value);
		}
		else
		{
			Action<TI, TF> action2 = ReflectionHelper.CreateFieldSetter<TI, TF>(name);
			_setter_cache.Add(typeof(TI), new Dictionary<string, Delegate> { { name, action2 } });
			action2(obj, value);
		}
	}

	public static void SetStaticField<TF, TI>(string name, TF value)
	{
		FieldInfo field = typeof(TI).GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (field != null)
		{
			field.SetValue(null, value);
			return;
		}
		LogService.LogWarning("Cannot find '" + name + "' in type " + typeof(TI).FullName + ". No action taken.");
		try
		{
			throw new Exception();
		}
		catch (Exception ex)
		{
			LogService.LogWarning(ex.StackTrace);
		}
	}

	public static void SetStaticField<TF>(string name, TF value, Type TI)
	{
		FieldInfo field = TI.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (field != null)
		{
			field.SetValue(null, value);
			return;
		}
		LogService.LogWarning("Cannot find '" + name + "' in type " + TI.FullName + ". No action taken.");
		try
		{
			throw new Exception();
		}
		catch (Exception ex)
		{
			LogService.LogWarning(ex.StackTrace);
		}
	}
}
