using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NeoModLoader.services;

namespace NeoModLoader.utils;

public static class SystemUtils
{
	public static void CmdRunAs(string[] parameters)
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.FileName = "cmd.exe";
		processStartInfo.Arguments = string.Join(" ", parameters);
		Console.WriteLine(processStartInfo.Arguments);
		processStartInfo.Verb = "runas";
		Process.Start(processStartInfo);
	}

	public static void BashRun(string[] parameters)
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.FileName = "bash";
		processStartInfo.Arguments = string.Join(" ", parameters);
		Console.WriteLine(processStartInfo.Arguments);
		Process.Start(processStartInfo);
	}

	public static List<string> SearchFileRecursive(string path, Func<string, bool> fileNameJudge, Func<string, bool> dirNameJudge)
	{
		List<string> list = new List<string>();
		Queue<DirectoryInfo> queue = new Queue<DirectoryInfo>();
		queue.Enqueue(new DirectoryInfo(path));
		while (queue.Count > 0)
		{
			DirectoryInfo directoryInfo = queue.Dequeue();
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				if (fileNameJudge(fileInfo.Name))
				{
					list.Add(fileInfo.FullName);
				}
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				if (dirNameJudge(directoryInfo2.Name))
				{
					queue.Enqueue(directoryInfo2);
				}
			}
		}
		return list;
	}

	public static void CopyDirectory(string pSource, string pTarget)
	{
		if (string.IsNullOrEmpty(pSource) || string.IsNullOrEmpty(pTarget))
		{
			LogService.LogWarning("Source or target is null or empty");
			LogService.LogStackTraceAsWarning();
			return;
		}
		if (!Directory.Exists(pSource))
		{
			LogService.LogWarning("Source directory " + pSource + " does not exist");
			LogService.LogStackTraceAsWarning();
			return;
		}
		if (!Directory.Exists(pTarget))
		{
			Directory.CreateDirectory(pTarget);
		}
		Queue<string> queue = new Queue<string>();
		queue.Enqueue("");
		while (queue.Count > 0)
		{
			string text = queue.Dequeue();
			DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(pSource, text));
			DirectoryInfo directoryInfo2 = new DirectoryInfo(Path.Combine(pTarget, text));
			if (!directoryInfo2.Exists)
			{
				directoryInfo2.Create();
			}
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				fileInfo.CopyTo(Path.Combine(pTarget, text, fileInfo.Name), overwrite: true);
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo3 in directories)
			{
				queue.Enqueue(Path.Combine(text, directoryInfo3.Name));
			}
		}
	}
}
