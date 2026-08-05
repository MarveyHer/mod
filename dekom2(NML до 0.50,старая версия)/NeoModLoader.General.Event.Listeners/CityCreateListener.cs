using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using NeoModLoader.General.Event.Handlers;
using NeoModLoader.services;

namespace NeoModLoader.General.Event.Listeners;

public class CityCreateListener : AbstractListener<CityCreateListener, CityCreateHandler>
{
	protected static void HandleAll(City pCity)
	{
		StringBuilder stringBuilder = null;
		int i = 0;
		int count = AbstractListener<CityCreateListener, CityCreateHandler>.instance.handlers.Count;
		bool flag = false;
		while (!flag)
		{
			try
			{
				for (; i < count; i++)
				{
					AbstractListener<CityCreateListener, CityCreateHandler>.instance.handlers[i].Handle(pCity);
				}
				flag = true;
			}
			catch (Exception ex)
			{
				AbstractListener<CityCreateListener, CityCreateHandler>.instance.handlers[i].HitException();
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.AppendLine("Failed to handle event in " + AbstractListener<CityCreateListener, CityCreateHandler>.instance.handlers[i].GetType().FullName);
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
	[HarmonyPatch(typeof(City), "newCityEvent")]
	private static IEnumerable<CodeInstruction> _newCityEvent_Patch(IEnumerable<CodeInstruction> instr)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		List<CodeInstruction> list = new List<CodeInstruction>(instr);
		int pos = 4;
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldarg_0, (object)null));
		AbstractListener<CityCreateListener, CityCreateHandler>.InsertCallHandleCode(list, pos);
		return list;
	}
}
