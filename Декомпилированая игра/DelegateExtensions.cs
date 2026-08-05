using System;

public static class DelegateExtensions
{
	public static string AsString<T>(this T pDelegate) where T : Delegate
	{
		if (pDelegate == null)
		{
			return "";
		}
		using ListPool<string> tStringToPrint = new ListPool<string>(pDelegate.GetInvocationList().Length);
		Delegate[] invocationList = pDelegate.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			T tObject = (T)invocationList[i];
			tStringToPrint.Add(tObject.Method.Name);
		}
		return string.Join(", ", tStringToPrint.ToArray());
	}
}
