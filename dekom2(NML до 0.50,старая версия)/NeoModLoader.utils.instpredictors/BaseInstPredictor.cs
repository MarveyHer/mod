using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace NeoModLoader.utils.instpredictors;

public class BaseInstPredictor
{
	private static readonly Dictionary<OpCode, HashSet<OpCode>> equal_opcodes = new Dictionary<OpCode, HashSet<OpCode>>();

	private readonly Func<CodeInstruction, bool> predicate;

	protected BaseInstPredictor()
	{
	}

	public BaseInstPredictor(OpCode pOpCode)
	{
		predicate = (CodeInstruction inst) => OpcodeEquals(pOpCode, inst);
	}

	public BaseInstPredictor(object pOperand)
	{
		predicate = (CodeInstruction inst) => inst.operand == pOperand;
	}

	public BaseInstPredictor(OpCode pOpCode, object pOperand)
	{
		predicate = (CodeInstruction inst) => OpcodeEquals(pOpCode, inst) && inst.operand == pOperand;
	}

	public BaseInstPredictor(Func<CodeInstruction, bool> pPredicate)
	{
		predicate = pPredicate;
	}

	public virtual bool Predict(CodeInstruction pInst)
	{
		return predicate?.Invoke(pInst) ?? true;
	}

	protected static bool OpcodeEquals(OpCode pOpCode, OpCode pOpCodeAnother)
	{
		return pOpCodeAnother == pOpCode;
	}

	protected static bool OpcodeEquals(CodeInstruction pInst, CodeInstruction pInstAnother)
	{
		HashSet<OpCode> value;
		return pInst.opcode == pInstAnother.opcode || (equal_opcodes.TryGetValue(pInst.opcode, out value) && value.Contains(pInstAnother.opcode));
	}

	protected static bool OpcodeEquals(OpCode pOpCode, CodeInstruction pInst)
	{
		HashSet<OpCode> value;
		return pInst.opcode == pOpCode || (equal_opcodes.TryGetValue(pOpCode, out value) && value.Contains(pInst.opcode));
	}

	protected static bool OpcodeEquals(CodeInstruction pInst, OpCode pOpCode)
	{
		HashSet<OpCode> value;
		return pInst.opcode == pOpCode || (equal_opcodes.TryGetValue(pOpCode, out value) && value.Contains(pInst.opcode));
	}

	internal static void _init()
	{
		AddEqualOpCodes(OpCodes.Br, OpCodes.Br_S);
		AddEqualOpCodes(OpCodes.Brtrue, OpCodes.Brtrue_S);
		AddEqualOpCodes(OpCodes.Brfalse, OpCodes.Brfalse_S);
	}

	private static void AddEqualOpCodes(params OpCode[] pOpCodes)
	{
		foreach (OpCode key in pOpCodes)
		{
			if (!equal_opcodes.TryGetValue(key, out var value))
			{
				value = new HashSet<OpCode>();
				equal_opcodes[key] = value;
			}
			value.UnionWith(pOpCodes);
			foreach (OpCode key2 in pOpCodes)
			{
				if (equal_opcodes.TryGetValue(key2, out var value2))
				{
					value.UnionWith(value2);
				}
			}
		}
	}
}
