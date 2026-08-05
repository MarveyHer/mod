using System;
using System.Diagnostics;
using System.Text;

namespace NeoModLoader.utils;

public static class OtherUtils
{
	public static string GetStackTrace(int skip_frames = 0, string indent = "")
	{
		string stackTrace = Environment.StackTrace;
		StringBuilder stringBuilder = new StringBuilder();
		string[] array = stackTrace.Split('\n');
		if (!string.IsNullOrEmpty(indent))
		{
			for (int i = skip_frames; i < array.Length; i++)
			{
				stringBuilder.AppendLine(array[i]);
			}
		}
		else
		{
			for (int j = skip_frames; j < array.Length; j++)
			{
				for (int k = 0; k < j - skip_frames; k++)
				{
					stringBuilder.Append(indent);
				}
				stringBuilder.AppendLine(array[j]);
			}
		}
		return stringBuilder.ToString();
	}

	public static bool CalledBy(string pMethodName, Type pTypeConstraint, bool pSearchAll = false)
	{
		StackTrace stackTrace = new StackTrace();
		StackFrame[] frames = stackTrace.GetFrames();
		if (frames == null)
		{
			return false;
		}
		if (frames.Length < 3)
		{
			return false;
		}
		if (!pSearchAll)
		{
			return frames[2].GetMethod().Name == pMethodName && (frames[2].GetType() == pTypeConstraint || frames[2].GetType().IsSubclassOf(pTypeConstraint));
		}
		for (int i = 2; i < frames.Length; i++)
		{
			if (frames[i].GetMethod().Name == pMethodName && (frames[i].GetType() == pTypeConstraint || frames[i].GetType().IsSubclassOf(pTypeConstraint)))
			{
				return true;
			}
		}
		return false;
	}
}
