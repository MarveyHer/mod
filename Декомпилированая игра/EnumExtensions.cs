using System;

public static class EnumExtensions
{
	public static int Count<TEnum>(this TEnum pEnum) where TEnum : Enum
	{
		int iCount = 0;
		int tEnum = Convert.ToInt32(pEnum);
		while (tEnum != 0)
		{
			tEnum &= tEnum - 1;
			iCount++;
		}
		return iCount;
	}
}
