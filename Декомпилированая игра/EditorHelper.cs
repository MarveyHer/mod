using System;

public static class EditorHelper
{
	public static bool HasArgument(string pName)
	{
		string[] tArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < tArgs.Length; i++)
		{
			if (tArgs[i].Contains(pName))
			{
				return true;
			}
		}
		return false;
	}

	public static string GetArgument(string pName)
	{
		string[] tArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < tArgs.Length; i++)
		{
			if (tArgs[i].Contains(pName))
			{
				return tArgs[i + 1];
			}
		}
		return null;
	}
}
