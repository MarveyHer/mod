using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace NeoModLoader.utils;

internal static class ReflectionHelper
{
	internal static Delegate GetMethod<T>(string method_name, bool is_static = false)
	{
		return createMethodDelegate(is_static ? typeof(T).GetMethod(method_name, BindingFlags.Static | BindingFlags.NonPublic) : AccessTools.Method(typeof(T), method_name, (Type[])null, (Type[])null));
	}

	internal static Delegate GetMethod(Type type, string method_name, bool is_static = false)
	{
		return createMethodDelegate(is_static ? type.GetMethod(method_name, BindingFlags.Static | BindingFlags.NonPublic) : AccessTools.Method(type, method_name, (Type[])null, (Type[])null));
	}

	internal static Delegate CreateFieldGetter(string field_name, Type instance_type, Type output_type)
	{
		FieldInfo fieldInfo = instance_type.GetField(field_name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ?? AccessTools.Field(instance_type, field_name);
		if (fieldInfo == null)
		{
			MonoBehaviour.print((object)("Cannot find '" + field_name + "' in type " + instance_type.FullName));
		}
		try
		{
			ParameterExpression parameterExpression = Expression.Parameter(instance_type, "instance");
			UnaryExpression expression = ((!fieldInfo.DeclaringType.IsValueType) ? Expression.TypeAs(parameterExpression, fieldInfo.DeclaringType) : Expression.Convert(parameterExpression, fieldInfo.DeclaringType));
			return (!output_type.IsPrimitive) ? Expression.Lambda<Delegate>(Expression.TypeAs(Expression.Field(expression, fieldInfo), output_type), new ParameterExpression[1] { parameterExpression }).Compile() : Expression.Lambda<Delegate>(Expression.Field(expression, fieldInfo), new ParameterExpression[1] { parameterExpression }).Compile();
		}
		catch (Exception)
		{
			Debug.LogError((object)("Expression Tree-Getter:" + fieldInfo.DeclaringType?.ToString() + "::" + field_name));
			return null;
		}
	}

	internal static Delegate CreateFieldGetter<OutType>(string field_name, Type instance_type)
	{
		return CreateFieldGetter(field_name, instance_type, typeof(OutType));
	}

	internal static Func<InstanceType, OutType> CreateFieldGetter<InstanceType, OutType>(string field_name)
	{
		FieldInfo fieldInfo = typeof(InstanceType).GetField(field_name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ?? AccessTools.Field(typeof(InstanceType), field_name);
		if (fieldInfo == null)
		{
			MonoBehaviour.print((object)("Cannot find '" + field_name + "' in type " + typeof(InstanceType).FullName));
		}
		try
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(InstanceType), "instance");
			UnaryExpression expression = ((!fieldInfo.DeclaringType.IsValueType) ? Expression.TypeAs(parameterExpression, fieldInfo.DeclaringType) : Expression.Convert(parameterExpression, fieldInfo.DeclaringType));
			return (!typeof(OutType).IsPrimitive) ? Expression.Lambda<Func<InstanceType, OutType>>(Expression.TypeAs(Expression.Field(expression, fieldInfo), typeof(OutType)), new ParameterExpression[1] { parameterExpression }).Compile() : Expression.Lambda<Func<InstanceType, OutType>>(Expression.Field(expression, fieldInfo), new ParameterExpression[1] { parameterExpression }).Compile();
		}
		catch (Exception)
		{
			Debug.LogError((object)("Expression Tree-Getter:" + fieldInfo.DeclaringType?.ToString() + "::" + field_name));
			return null;
		}
	}

	internal static Action<TI, TF> CreateFieldSetter<TI, TF>(string field_name)
	{
		FieldInfo field = typeof(TI).GetField(field_name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		ParameterExpression parameterExpression = Expression.Parameter(typeof(TI), "instance");
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(TF), field_name);
		if (field.FieldType == typeof(TF))
		{
			return Expression.Lambda<Action<TI, TF>>(Expression.Assign(Expression.Field(parameterExpression, field), parameterExpression2), new ParameterExpression[2] { parameterExpression, parameterExpression2 }).Compile();
		}
		return Expression.Lambda<Action<TI, TF>>(Expression.Assign(Expression.Field(parameterExpression, field), field.FieldType.IsValueType ? Expression.Convert(parameterExpression2, field.FieldType) : Expression.TypeAs(parameterExpression2, field.FieldType)), new ParameterExpression[2] { parameterExpression, parameterExpression2 }).Compile();
	}

	private static Delegate createMethodDelegate(MethodInfo method_info)
	{
		List<ParameterExpression> list = method_info.GetParameters().Select((ParameterInfo p, int i) => Expression.Parameter(p.ParameterType, p.Name)).ToList();
		MethodCallExpression body;
		if (method_info.IsStatic)
		{
			body = Expression.Call(method_info, list);
		}
		else
		{
			ParameterExpression parameterExpression = Expression.Parameter(method_info.ReflectedType, "instance");
			body = Expression.Call(parameterExpression, method_info, list);
			list.Insert(0, parameterExpression);
		}
		LambdaExpression lambdaExpression = Expression.Lambda(body, list);
		return lambdaExpression.Compile();
	}
}
