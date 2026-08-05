using System;
using System.Reflection;
using HarmonyLib;
using NeoModLoader.services;
using Newtonsoft.Json;

namespace NeoModLoader.api;

public class ModConfigItem
{
	private MethodInfo callback;

	[JsonProperty("Type")]
	public ConfigItemType Type { get; internal set; }

	[JsonProperty("Id")]
	public string Id { get; internal set; }

	[JsonProperty("IconPath")]
	public string IconPath { get; internal set; }

	[JsonProperty("BoolVal")]
	public bool BoolVal { get; internal set; }

	[JsonProperty("TextVal")]
	public string TextVal { get; internal set; }

	[JsonProperty("FloatVal")]
	public float FloatVal { get; internal set; }

	[JsonProperty("MaxFloatVal")]
	public float MaxFloatVal { get; internal set; } = 1f;

	[JsonProperty("MinFloatVal")]
	public float MinFloatVal { get; internal set; }

	[JsonProperty("IntVal")]
	public int IntVal { get; internal set; }

	[JsonProperty("MaxIntVal")]
	public int MaxIntVal { get; internal set; } = 1;

	[JsonProperty("MinIntVal")]
	public int MinIntVal { get; internal set; }

	[JsonProperty("Callback")]
	public string CallBack { get; internal set; }

	public void SetFloatRange(float pMin, float pMax)
	{
		if (pMax < pMin)
		{
			throw new ArgumentException("Max value must be greater than min value!");
		}
		MinFloatVal = pMin;
		MaxFloatVal = pMax;
	}

	public void SetIntRange(int pMin, int pMax)
	{
		if (pMax < pMin)
		{
			throw new ArgumentException("Max value must be greater than min value!");
		}
		MinIntVal = pMin;
		MaxIntVal = pMax;
	}

	public void SetValue(object val, bool pSkipCallback = false)
	{
		try
		{
			switch (Type)
			{
			case ConfigItemType.SWITCH:
			{
				bool boolVal = BoolVal;
				BoolVal = Convert.ToBoolean(val);
				if (string.IsNullOrEmpty(CallBack) || pSkipCallback)
				{
					break;
				}
				if (callback == null)
				{
					callback = AccessTools.Method(CallBack, new Type[1] { typeof(bool) }, (Type[])null);
				}
				if (callback == null)
				{
					LogService.LogWarning($"No found callback({typeof(bool)}) {CallBack}");
					break;
				}
				try
				{
					callback.Invoke(null, new object[1] { BoolVal });
					break;
				}
				catch (Exception ex3)
				{
					LogService.LogError($"Failed to set value '{BoolVal}'({typeof(bool)}) for config item '{Id}'");
					LogService.LogError(ex3.Message);
					LogService.LogError(ex3.StackTrace);
					BoolVal = boolVal;
					break;
				}
			}
			case ConfigItemType.SLIDER:
			{
				float floatVal = FloatVal;
				FloatVal = Convert.ToSingle(val);
				FloatVal = Math.Max(MinFloatVal, Math.Min(MaxFloatVal, FloatVal));
				if (string.IsNullOrEmpty(CallBack) || pSkipCallback)
				{
					break;
				}
				MethodInfo methodInfo4 = AccessTools.Method(CallBack, new Type[1] { typeof(float) }, (Type[])null);
				if (methodInfo4 == null)
				{
					LogService.LogWarning($"No found callback({typeof(float)}) {CallBack}");
					break;
				}
				try
				{
					methodInfo4.Invoke(null, new object[1] { FloatVal });
					break;
				}
				catch (Exception ex5)
				{
					LogService.LogError($"Failed to set value '{FloatVal}'({typeof(float)}) for config item '{Id}'");
					LogService.LogError(ex5.Message);
					LogService.LogError(ex5.StackTrace);
					FloatVal = floatVal;
					break;
				}
			}
			case ConfigItemType.INT_SLIDER:
			{
				int intVal2 = IntVal;
				IntVal = Convert.ToInt32(val);
				IntVal = Math.Max(MinIntVal, Math.Min(MaxIntVal, IntVal));
				if (string.IsNullOrEmpty(CallBack) || pSkipCallback)
				{
					break;
				}
				MethodInfo methodInfo3 = AccessTools.Method(CallBack, new Type[1] { typeof(int) }, (Type[])null);
				if (methodInfo3 == null)
				{
					LogService.LogWarning($"No found callback({typeof(int)}) {CallBack}");
					break;
				}
				try
				{
					methodInfo3.Invoke(null, new object[1] { IntVal });
					break;
				}
				catch (Exception ex4)
				{
					LogService.LogError($"Failed to set value '{IntVal}'({typeof(int)}) for config item '{Id}'");
					LogService.LogError(ex4.Message);
					LogService.LogError(ex4.StackTrace);
					IntVal = intVal2;
					break;
				}
			}
			case ConfigItemType.TEXT:
			{
				string textVal = TextVal;
				TextVal = Convert.ToString(val);
				if (string.IsNullOrEmpty(CallBack) || pSkipCallback)
				{
					break;
				}
				MethodInfo methodInfo2 = AccessTools.Method(CallBack, new Type[1] { typeof(string) }, (Type[])null);
				if (methodInfo2 == null)
				{
					LogService.LogWarning($"No found callback({typeof(string)}) {CallBack}");
					break;
				}
				try
				{
					methodInfo2.Invoke(null, new object[1] { TextVal });
					break;
				}
				catch (Exception ex2)
				{
					LogService.LogError($"Failed to set value '{TextVal}'({typeof(string)}) for config item '{Id}'");
					LogService.LogError(ex2.Message);
					LogService.LogError(ex2.StackTrace);
					TextVal = textVal;
					break;
				}
			}
			case ConfigItemType.SELECT:
			{
				int intVal = IntVal;
				IntVal = Convert.ToInt32(val);
				if (string.IsNullOrEmpty(CallBack) || pSkipCallback)
				{
					break;
				}
				MethodInfo methodInfo = AccessTools.Method(CallBack, new Type[1] { typeof(int) }, (Type[])null);
				if (methodInfo == null)
				{
					LogService.LogWarning($"No found callback({typeof(int)}) {CallBack}");
					break;
				}
				try
				{
					methodInfo.Invoke(null, new object[1] { IntVal });
					break;
				}
				catch (Exception ex)
				{
					LogService.LogError($"Failed to set value '{IntVal}'({typeof(int)}) for config item '{Id}'");
					LogService.LogError(ex.Message);
					LogService.LogError(ex.StackTrace);
					IntVal = intVal;
					break;
				}
			}
			}
		}
		catch (Exception ex6)
		{
			LogService.LogError($"Error while setting value for config item {Type}! {ex6.Message}");
			LogService.LogError(ex6.StackTrace);
			LogService.LogError("Set default value instead.");
			switch (Type)
			{
			case ConfigItemType.SWITCH:
				BoolVal = false;
				break;
			case ConfigItemType.SLIDER:
				FloatVal = 0f;
				break;
			case ConfigItemType.INT_SLIDER:
				IntVal = 0;
				break;
			case ConfigItemType.TEXT:
				TextVal = "";
				break;
			case ConfigItemType.SELECT:
				IntVal = 0;
				break;
			}
		}
	}

	public object GetValue()
	{
		ConfigItemType type = Type;
		if (1 == 0)
		{
		}
		object result = type switch
		{
			ConfigItemType.SWITCH => BoolVal, 
			ConfigItemType.SLIDER => FloatVal, 
			ConfigItemType.INT_SLIDER => IntVal, 
			ConfigItemType.TEXT => TextVal, 
			ConfigItemType.SELECT => IntVal, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		if (1 == 0)
		{
		}
		return result;
	}
}
