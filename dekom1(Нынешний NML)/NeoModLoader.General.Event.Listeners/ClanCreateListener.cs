using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using NeoModLoader.General.Event.Handlers;
using NeoModLoader.services;

namespace NeoModLoader.General.Event.Listeners;

public class ClanCreateListener : AbstractListener<ClanCreateListener, ClanCreateHandler>
{
	protected static void HandleAll(Clan pClan, Actor pActor)
	{
		StringBuilder stringBuilder = null;
		int i = 0;
		int count = AbstractListener<ClanCreateListener, ClanCreateHandler>.instance.handlers.Count;
		bool flag = false;
		while (!flag)
		{
			try
			{
				for (; i < count; i++)
				{
					AbstractListener<ClanCreateListener, ClanCreateHandler>.instance.handlers[i].Handle(pClan, pActor);
				}
				flag = true;
			}
			catch (Exception ex)
			{
				AbstractListener<ClanCreateListener, ClanCreateHandler>.instance.handlers[i].HitException();
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.AppendLine("Failed to handle event in " + AbstractListener<ClanCreateListener, ClanCreateHandler>.instance.handlers[i].GetType().FullName);
				stringBuilder.AppendLine(ex.Message);
				stringBuilder.AppendLine(ex.StackTrace);
				i++;
			}
		}
		if (stringBuilder != null)
		{
			LogService.LogError(stringBuilder.ToString());
		}
	}

	[HarmonyTranspiler]
	[HarmonyPatch(typeof(ClanManager), "newClan")]
	private static IEnumerable<CodeInstruction> _newClan_Patch(IEnumerable<CodeInstruction> instr)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		List<CodeInstruction> list = new List<CodeInstruction>(instr);
		int pos = 6;
		list.Insert(pos++, new CodeInstruction(OpCodes.Dup, (object)null));
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldarg_1, (object)null));
		AbstractListener<ClanCreateListener, ClanCreateHandler>.InsertCallHandleCode(list, pos);
		return list;
	}

	[Obsolete("Operation is not supported", true)]
	private static MethodInfo _createHandleAllMethodByIL()
	{
		MethodInfo methodInfo = AccessTools.Method(typeof(ClanCreateHandler), "Handle", (Type[])null, (Type[])null);
		ParameterInfo[] parameters = methodInfo.GetParameters();
		List<Type> list = new List<Type>();
		ParameterInfo[] array = parameters;
		foreach (ParameterInfo parameterInfo in array)
		{
			list.Add(parameterInfo.ParameterType);
		}
		DynamicMethod dynamicMethod = new DynamicMethod("ClanCreateListener_HandleAll", typeof(void), list.ToArray());
		ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
		iLGenerator.Emit(OpCodes.Nop);
		iLGenerator.Emit(OpCodes.Ldnull);
		iLGenerator.Emit(OpCodes.Stloc_0);
		iLGenerator.Emit(OpCodes.Ldc_I4_0);
		iLGenerator.Emit(OpCodes.Stloc_1);
		iLGenerator.Emit(OpCodes.Call, AccessTools.PropertyGetter(typeof(ClanCreateListener), "instance"));
		iLGenerator.Emit(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ClanCreateListener), "handlers"));
		iLGenerator.Emit(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(List<ClanCreateHandler>), "Count"));
		iLGenerator.Emit(OpCodes.Stloc_2);
		iLGenerator.Emit(OpCodes.Ldc_I4_0);
		iLGenerator.Emit(OpCodes.Stloc_3);
		Label label = iLGenerator.DefineLabel();
		Label label2 = iLGenerator.DefineLabel();
		Label label3 = iLGenerator.DefineLabel();
		Label label4 = iLGenerator.DefineLabel();
		Label label5 = iLGenerator.DefineLabel();
		iLGenerator.Emit(OpCodes.Br, label);
		iLGenerator.MarkLabel(label2);
		iLGenerator.Emit(OpCodes.Nop);
		iLGenerator.Emit(OpCodes.Nop);
		iLGenerator.Emit(OpCodes.Br_S, label3);
		iLGenerator.MarkLabel(label4);
		iLGenerator.Emit(OpCodes.Nop);
		iLGenerator.Emit(OpCodes.Call, AccessTools.PropertyGetter(typeof(ClanCreateListener), "instance"));
		iLGenerator.Emit(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ClanCreateListener), "handlers"));
		iLGenerator.Emit(OpCodes.Ldloc_1);
		iLGenerator.Emit(OpCodes.Callvirt, AccessTools.Method(typeof(List<ClanCreateHandler>), "get_Item", (Type[])null, (Type[])null));
		for (int j = 0; j < parameters.Length; j++)
		{
			iLGenerator.Emit(OpCodes.Ldarg, j);
		}
		iLGenerator.Emit(OpCodes.Callvirt, methodInfo);
		iLGenerator.Emit(OpCodes.Nop);
		iLGenerator.Emit(OpCodes.Nop);
		iLGenerator.Emit(OpCodes.Ldloc_1);
		iLGenerator.Emit(OpCodes.Ldc_I4_1);
		iLGenerator.Emit(OpCodes.Add);
		iLGenerator.Emit(OpCodes.Stloc_1);
		iLGenerator.MarkLabel(label3);
		iLGenerator.Emit(OpCodes.Ldloc_1);
		iLGenerator.Emit(OpCodes.Ldloc_2);
		iLGenerator.Emit(OpCodes.Clt);
		iLGenerator.Emit(OpCodes.Stloc_S, (byte)4);
		iLGenerator.Emit(OpCodes.Ldloc_S, (byte)4);
		iLGenerator.Emit(OpCodes.Brtrue_S, label4);
		iLGenerator.Emit(OpCodes.Ldc_I4_1);
		iLGenerator.Emit(OpCodes.Stloc_3);
		iLGenerator.Emit(OpCodes.Nop);
		iLGenerator.Emit(OpCodes.Leave_S, label5);
		iLGenerator.Emit(OpCodes.Stloc_S, (byte)5);
		iLGenerator.Emit(OpCodes.Nop);
		iLGenerator.Emit(OpCodes.Call, AccessTools.PropertyGetter(typeof(ClanCreateListener), "instance"));
		iLGenerator.Emit(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ClanCreateListener), "handlers"));
		iLGenerator.Emit(OpCodes.Ldloc_1);
		iLGenerator.Emit(OpCodes.Callvirt, AccessTools.Method(typeof(List<ClanCreateHandler>), "get_Item", (Type[])null, (Type[])null));
		iLGenerator.Emit(OpCodes.Callvirt, AccessTools.Method(typeof(ClanCreateHandler), "HitException", (Type[])null, (Type[])null));
		iLGenerator.Emit(OpCodes.Nop);
		Label label6 = iLGenerator.DefineLabel();
		iLGenerator.Emit(OpCodes.Ldloc_0);
		iLGenerator.Emit(OpCodes.Brtrue_S, label6);
		iLGenerator.Emit(OpCodes.Newobj, typeof(StringBuilder).GetConstructor(Type.EmptyTypes));
		iLGenerator.Emit(OpCodes.Stloc_0);
		iLGenerator.MarkLabel(label6);
		iLGenerator.Emit(OpCodes.Ldloc_0);
		iLGenerator.Emit(OpCodes.Ldstr, "Failed to handle event in");
		iLGenerator.Emit(OpCodes.Call, AccessTools.PropertyGetter(typeof(ClanCreateListener), "instance"));
		iLGenerator.Emit(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ClanCreateListener), "handlers"));
		iLGenerator.Emit(OpCodes.Ldloc_1);
		iLGenerator.Emit(OpCodes.Callvirt, AccessTools.Method(typeof(List<ClanCreateHandler>), "get_Item", (Type[])null, (Type[])null));
		iLGenerator.Emit(OpCodes.Callvirt, AccessTools.Method(typeof(object), "GetType", (Type[])null, (Type[])null));
		iLGenerator.Emit(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Type), "FullName"));
		iLGenerator.Emit(OpCodes.Call, AccessTools.Method(typeof(string), "Concat", new Type[2]
		{
			typeof(string),
			typeof(string)
		}, (Type[])null));
		iLGenerator.Emit(OpCodes.Callvirt, AccessTools.Method(typeof(StringBuilder), "AppendLine", new Type[1] { typeof(string) }, (Type[])null));
		iLGenerator.Emit(OpCodes.Pop);
		iLGenerator.Emit(OpCodes.Ldloc_0);
		iLGenerator.Emit(OpCodes.Ldloc_S, (byte)5);
		iLGenerator.Emit(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Exception), "Message"));
		iLGenerator.Emit(OpCodes.Callvirt, AccessTools.Method(typeof(StringBuilder), "AppendLine", new Type[1] { typeof(string) }, (Type[])null));
		iLGenerator.Emit(OpCodes.Pop);
		iLGenerator.Emit(OpCodes.Ldloc_0);
		iLGenerator.Emit(OpCodes.Ldloc_S, (byte)5);
		iLGenerator.Emit(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Exception), "StackTrace"));
		iLGenerator.Emit(OpCodes.Callvirt, AccessTools.Method(typeof(StringBuilder), "AppendLine", new Type[1] { typeof(string) }, (Type[])null));
		iLGenerator.Emit(OpCodes.Pop);
		iLGenerator.Emit(OpCodes.Ldloc_1);
		iLGenerator.Emit(OpCodes.Ldc_I4_1);
		iLGenerator.Emit(OpCodes.Add);
		iLGenerator.Emit(OpCodes.Stloc_1);
		iLGenerator.Emit(OpCodes.Nop);
		iLGenerator.Emit(OpCodes.Leave_S, label5);
		iLGenerator.MarkLabel(label5);
		iLGenerator.Emit(OpCodes.Nop);
		iLGenerator.MarkLabel(label);
		iLGenerator.Emit(OpCodes.Ldloc_3);
		iLGenerator.Emit(OpCodes.Ldc_I4_0);
		iLGenerator.Emit(OpCodes.Ceq);
		iLGenerator.Emit(OpCodes.Stloc_S, (byte)6);
		iLGenerator.Emit(OpCodes.Ldloc_S, (byte)6);
		iLGenerator.Emit(OpCodes.Brtrue_S, label2);
		iLGenerator.Emit(OpCodes.Ldloc_0);
		iLGenerator.Emit(OpCodes.Ldnull);
		iLGenerator.Emit(OpCodes.Cgt_Un);
		iLGenerator.Emit(OpCodes.Stloc_S, (byte)7);
		iLGenerator.Emit(OpCodes.Ldloc_S, (byte)7);
		Label label7 = iLGenerator.DefineLabel();
		iLGenerator.Emit(OpCodes.Brfalse_S, label7);
		iLGenerator.Emit(OpCodes.Nop);
		iLGenerator.Emit(OpCodes.Ldloc_0);
		iLGenerator.Emit(OpCodes.Callvirt, AccessTools.Method(typeof(StringBuilder), "ToString", (Type[])null, (Type[])null));
		iLGenerator.Emit(OpCodes.Call, AccessTools.Method(typeof(LogService), "LogError", new Type[1] { typeof(string) }, (Type[])null));
		iLGenerator.Emit(OpCodes.Nop);
		iLGenerator.MarkLabel(label7);
		iLGenerator.Emit(OpCodes.Nop);
		iLGenerator.Emit(OpCodes.Ret);
		Delegate del = dynamicMethod.CreateDelegate(typeof(Delegate));
		return del.GetMethodInfo();
	}
}
