using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using NeoModLoader.constants;
using NeoModLoader.utils;
using UnityEngine;

namespace NeoModLoader.services;

public static class LogService
{
	private enum LogType
	{
		Info,
		Warning,
		Error
	}

	private class WrappedMessage
	{
		public string message;

		public LogType type;

		public WrappedMessage(string message, LogType type)
		{
			this.message = message;
			this.type = type;
		}

		public void Reset(string message, LogType type)
		{
			this.message = message;
			this.type = type;
		}
	}

	private class ConcurrentLogHandle : MonoBehaviour
	{
		private void Update()
		{
			int num = 0;
			WrappedMessage result;
			while (num <= 32 && concurrent_log_queue.TryDequeue(out result))
			{
				num++;
				switch (result.type)
				{
				case LogType.Info:
					LogInfo(result.message);
					break;
				case LogType.Warning:
					LogWarning(result.message);
					break;
				case LogType.Error:
					LogError(result.message);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (_pool.Count < 100)
				{
					_pool.Add(result);
				}
			}
		}
	}

	private static readonly ConcurrentQueue<WrappedMessage> concurrent_log_queue = new ConcurrentQueue<WrappedMessage>();

	private static ConcurrentBag<WrappedMessage> _pool = new ConcurrentBag<WrappedMessage>();

	private const int pool_size = 100;

	public static void PullAllConcurrentLogToCurrentThread()
	{
		WrappedMessage result;
		while (concurrent_log_queue.TryDequeue(out result))
		{
			switch (result.type)
			{
			case LogType.Info:
				LogInfo(result.message);
				break;
			case LogType.Warning:
				LogWarning(result.message);
				break;
			case LogType.Error:
				LogError(result.message);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (_pool.Count < 100)
			{
				_pool.Add(result);
			}
		}
	}

	internal static void Init()
	{
		((Component)WorldBoxMod.Transform).gameObject.AddComponent<ConcurrentLogHandle>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogInfoConcurrent(string message)
	{
		if (_pool.TryTake(out var result))
		{
			result.Reset(message, LogType.Info);
		}
		else
		{
			result = new WrappedMessage(message, LogType.Info);
		}
		concurrent_log_queue.Enqueue(result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogWarningConcurrent(string message)
	{
		if (_pool.TryTake(out var result))
		{
			result.Reset(message, LogType.Warning);
		}
		else
		{
			result = new WrappedMessage(message, LogType.Warning);
		}
		concurrent_log_queue.Enqueue(result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogErrorConcurrent(string message)
	{
		if (_pool.TryTake(out var result))
		{
			result.Reset(message, LogType.Error);
		}
		else
		{
			result = new WrappedMessage(message, LogType.Error);
		}
		concurrent_log_queue.Enqueue(result);
	}

	public static void LogException(Exception exception)
	{
		if (Others.unity_player_enabled)
		{
			Debug.LogException(exception);
		}
		else
		{
			Console.WriteLine(exception);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogError(string message)
	{
		if (Others.unity_player_enabled)
		{
			Debug.LogError((object)("[NML]: " + message));
		}
		else
		{
			Console.Error.WriteLine("[NML]: " + message);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogWarning(string message)
	{
		if (Others.unity_player_enabled)
		{
			Debug.LogWarning((object)("[NML]: " + message));
		}
		else
		{
			Console.WriteLine("[NML]: " + message);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogInfo(string message)
	{
		if (Others.unity_player_enabled)
		{
			Debug.Log((object)("[NML]: " + message));
		}
		else
		{
			Console.WriteLine("[NML]: " + message);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogStackTraceAsInfo()
	{
		LogInfo(OtherUtils.GetStackTrace(2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogStackTraceAsWarning()
	{
		LogWarning(OtherUtils.GetStackTrace(2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogStackTraceAsError()
	{
		LogError(OtherUtils.GetStackTrace(2));
	}
}
