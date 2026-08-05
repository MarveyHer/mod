using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using NeoModLoader.utils.instpredictors;

namespace NeoModLoader.utils;

public static class HarmonyUtils
{
	public static int FindCodeSnippet(List<CodeInstruction> pCodes, out List<CodeInstruction> pResult, params BaseInstPredictor[] pSnippetPredictors)
	{
		int i;
		for (i = 0; i < pCodes.Count - pSnippetPredictors.Length; i++)
		{
			if (!pSnippetPredictors.Where((BaseInstPredictor t, int j) => !t.Predict(pCodes[i + j])).Any())
			{
				pResult = pCodes.GetRange(i, pSnippetPredictors.Length);
				return i;
			}
		}
		pResult = null;
		return -1;
	}

	public static int FindCodeSnippetIdx(List<CodeInstruction> pCodes, params BaseInstPredictor[] pSnippetPredictors)
	{
		int i;
		for (i = 0; i < pCodes.Count - pSnippetPredictors.Length; i++)
		{
			if (!pSnippetPredictors.Where((BaseInstPredictor t, int j) => !t.Predict(pCodes[i + j])).Any())
			{
				return i;
			}
		}
		return -1;
	}

	public static CodeInstruction FindInst(List<CodeInstruction> pCodes, BaseInstPredictor pPredictor)
	{
		return pCodes.FirstOrDefault(pPredictor.Predict);
	}

	public static TOperand FindInstOperand<TOperand>(List<CodeInstruction> pCodes, BaseInstPredictor pPredictor)
	{
		CodeInstruction val = FindInst(pCodes, pPredictor);
		if (val == null)
		{
			return default(TOperand);
		}
		return (val.operand is TOperand val2) ? val2 : default(TOperand);
	}

	public static int FindInstIdx<TOperand>(List<CodeInstruction> pCodes, BaseInstPredictor pPredictor)
	{
		for (int i = 0; i < pCodes.Count; i++)
		{
			if (pPredictor.Predict(pCodes[i]))
			{
				return i;
			}
		}
		return -1;
	}

	internal static void _init()
	{
		BaseInstPredictor._init();
	}
}
