using System;

public static class ActionExtensions
{
	public static bool[] Run(this WorldAction pAction, BaseSimObject pTarget = null, WorldTile pTile = null)
	{
		Delegate[] invocationList = pAction.GetInvocationList();
		bool[] tResults = new bool[invocationList.Length];
		int tIndex = 0;
		Delegate[] array = invocationList;
		for (int i = 0; i < array.Length; i++)
		{
			WorldAction tAction = (WorldAction)array[i];
			tResults[tIndex++] = tAction(pTarget, pTile);
		}
		return tResults;
	}

	public static bool RunAnyTrue(this WorldAction pAction, BaseSimObject pTarget = null, WorldTile pTile = null)
	{
		Delegate[] invocationList = pAction.GetInvocationList();
		bool tSuccess = false;
		Delegate[] array = invocationList;
		for (int i = 0; i < array.Length; i++)
		{
			if (((WorldAction)array[i])(pTarget, pTile))
			{
				tSuccess = true;
			}
		}
		return tSuccess;
	}

	public static bool[] Run(this AttackAction pAction, BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		Delegate[] invocationList = pAction.GetInvocationList();
		bool[] tResults = new bool[invocationList.Length];
		int tIndex = 0;
		Delegate[] array = invocationList;
		for (int i = 0; i < array.Length; i++)
		{
			AttackAction tAction = (AttackAction)array[i];
			tResults[tIndex++] = tAction(pSelf, pTarget, pTile);
		}
		return tResults;
	}

	public static bool RunAnyTrue(this AttackAction pAction, BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		Delegate[] invocationList = pAction.GetInvocationList();
		bool tSuccess = false;
		Delegate[] array = invocationList;
		for (int i = 0; i < array.Length; i++)
		{
			if (((AttackAction)array[i])(pSelf, pTarget, pTile))
			{
				tSuccess = true;
			}
		}
		return tSuccess;
	}
}
