using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using NeoModLoader.utils.Builders;

namespace NeoModLoader.utils;

internal class AssetPatches
{
	[HarmonyPatch(typeof(Actor), "updateStats")]
	[HarmonyTranspiler]
	private static IEnumerable<CodeInstruction> MergeWithCustomStats(IEnumerable<CodeInstruction> instructions)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		CodeMatcher val = new CodeMatcher(instructions, (ILGenerator)null);
		val.MatchForward(false, (CodeMatch[])(object)new CodeMatch[1]
		{
			new CodeMatch((OpCode?)OpCodes.Callvirt, (object)AccessTools.Method(typeof(BaseStats), "clear", (Type[])null, (Type[])null), (string)null)
		});
		val.Advance(1);
		val.Insert((CodeInstruction[])(object)new CodeInstruction[2]
		{
			new CodeInstruction(OpCodes.Ldarg_0, (object)null),
			new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(AssetPatches), "MergeCustomStats", (Type[])null, (Type[])null))
		});
		return val.Instructions();
	}

	private static void MergeCustomStats(Actor __instance)
	{
		foreach (ActorTrait trait in __instance.traits)
		{
			if (ActorTraitBuilder.AdditionalBaseStatMethods.TryGetValue(trait.id, out var value))
			{
				((BaseSimObject)__instance).stats.mergeStats(value(__instance), 1f);
			}
		}
	}

	private static BaseStats[] GetCustomStats(ActorTrait trait)
	{
		if (SelectedUnit.unit == null || !SelectedUnit.unit.hasTrait(trait))
		{
			return Array.Empty<BaseStats>();
		}
		if (!ActorTraitBuilder.AdditionalBaseStatMethods.TryGetValue(trait.id, out var value))
		{
			return Array.Empty<BaseStats>();
		}
		return new BaseStats[1] { value(SelectedUnit.unit) };
	}

	[HarmonyPatch(typeof(TooltipLibrary), "showTrait")]
	[HarmonyTranspiler]
	private static IEnumerable<CodeInstruction> ShowCustomStats(IEnumerable<CodeInstruction> instructions)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		CodeMatcher val = new CodeMatcher(instructions, (ILGenerator)null);
		val.MatchForward(false, (CodeMatch[])(object)new CodeMatch[1]
		{
			new CodeMatch((OpCode?)OpCodes.Call, (object)AccessTools.Field(typeof(Array), "Empty"), (string)null)
		});
		val.RemoveInstruction();
		val.Insert((CodeInstruction[])(object)new CodeInstruction[2]
		{
			new CodeInstruction(OpCodes.Ldloc_0, (object)null),
			new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(AssetPatches), "GetCustomStats", (Type[])null, (Type[])null))
		});
		return val.Instructions();
	}
}
