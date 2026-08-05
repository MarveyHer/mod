using System;
using System.Collections.Concurrent;
using System.IO;
using UnityEngine;
using WorldBoxConsole;

public class LogHandler : MonoBehaviour
{
	private static string folder_base = "/logs";

	private static string dataName = "/error";

	public static string log = "";

	internal static int errorNum = 0;

	private static string lastError = "";

	private static int errorRepeated = 0;

	private static bool _init_handler = false;

	private static bool _init_instance = false;

	private static bool toggledConsole = false;

	private static string _filename = null;

	private static ConcurrentQueue<LogItem> log_queue = new ConcurrentQueue<LogItem>();

	private static LogHandler _instance;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void init()
	{
		if (!_init_handler)
		{
			_init_handler = true;
			if (!Application.isEditor)
			{
				Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
				Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
				Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
			}
			Application.logMessageReceivedThreaded += HandleLog;
			Application.logMessageReceivedThreaded += WorldBoxConsole.Console.HandleLog;
			if (!Directory.Exists(getDirPath()))
			{
				Directory.CreateDirectory(getDirPath());
			}
		}
	}

	[RuntimeInitializeOnLoadMethod]
	public static void initInstance()
	{
		if (!_init_instance)
		{
			_init_instance = true;
			if (_instance == null)
			{
				GameObject obj = new GameObject("[LogHandler]");
				_instance = obj.AddComponent<LogHandler>();
				UnityEngine.Object.DontDestroyOnLoad(obj);
				obj.hideFlags = HideFlags.DontSave;
			}
		}
	}

	private void Update()
	{
		LogItem tLogItem;
		while (log_queue.TryDequeue(out tLogItem))
		{
			ProcessLog(tLogItem.log, tLogItem.stack_trace, tLogItem.type);
		}
	}

	private static void HandleLog(string pLogString, string pStackTrace, LogType pLogType)
	{
		if (ThreadHelper.isMainThread())
		{
			ProcessLog(pLogString, pStackTrace, pLogType);
		}
		else
		{
			log_queue.Enqueue(new LogItem(pLogString, pStackTrace, pLogType));
		}
	}

	private static void ProcessLog(string pLogString, string pStackTrace, LogType pLogType)
	{
		pLogString = pLogString.Trim(' ', '\n');
		if (pLogType == LogType.Error || pLogType == LogType.Exception || pLogType == LogType.Assert)
		{
			if (errorNum > 100)
			{
				return;
			}
			log = "";
			if (errorNum == 0)
			{
				log = log + "Game Version: " + Application.version;
				if (!string.IsNullOrEmpty(Config.versionCodeText))
				{
					log = log + " (" + Config.versionCodeText;
					if (!string.IsNullOrEmpty(Config.gitCodeText))
					{
						log = log + "@" + Config.gitCodeText;
					}
					log += ")";
				}
				log = log + "\nModded: " + Config.MODDED;
				log = log + "\noperatingSystemFamily: " + SystemInfo.operatingSystemFamily;
				log = log + "\ndeviceModel: " + SystemInfo.deviceModel;
				log = log + "\ndeviceName: " + SystemInfo.deviceName;
				log = log + "\ndeviceType: " + SystemInfo.deviceType;
				log = log + "\nsystemMemorySize: " + SystemInfo.systemMemorySize;
				log = log + "\ngraphicsDeviceID: " + SystemInfo.graphicsDeviceID;
				log = log + "\ngraphicsActiveTier: " + Graphics.activeTier;
				log = log + "\nGC.GetTotalMemory: " + GC.GetTotalMemory(forceFullCollection: false) / 1000000 + " mb";
				log = log + "\ngraphicsMemorySize: " + SystemInfo.graphicsMemorySize;
				log = log + "\nmaxTextureSize: " + SystemInfo.maxTextureSize;
				log = log + "\noperatingSystem: " + SystemInfo.operatingSystem;
				log = log + "\nprocessorType: " + SystemInfo.processorType;
				log = log + "\ninstallMode: " + Application.installMode;
				log = log + "\nsandboxType: " + Application.sandboxType;
				try
				{
					if (Input.anyKey)
					{
						string tMods = "";
						if (HotkeyLibrary.isHoldingAlt())
						{
							tMods += "Alt ";
						}
						if (HotkeyLibrary.isHoldingControlForSelection())
						{
							tMods += "Ctrl ";
						}
						if (HotkeyLibrary.isHoldingAnyMod())
						{
							tMods += "Mod ";
						}
						log = log + "\nkeyboard: " + Input.anyKey + " " + Input.anyKeyDown + " " + Input.inputString + " " + tMods;
						if (Input.mousePresent)
						{
							string tMouse0State = (Input.GetMouseButton(0) ? "press0" : (Input.GetMouseButtonDown(0) ? "down0" : (Input.GetMouseButtonUp(0) ? "up0" : "none1")));
							string tMouse1State = (Input.GetMouseButton(1) ? "press1" : (Input.GetMouseButtonDown(1) ? "down1" : (Input.GetMouseButtonUp(1) ? "up1" : "none1")));
							string tMouse2State = (Input.GetMouseButton(2) ? "press2" : (Input.GetMouseButtonDown(2) ? "down2" : (Input.GetMouseButtonUp(2) ? "up2" : "none2")));
							string tMouseLocation = Input.mousePosition.ToString();
							log = log + "\nmouse: " + tMouseLocation + " " + tMouse0State + " " + tMouse1State + " " + tMouse2State;
						}
					}
				}
				catch (Exception)
				{
				}
				log = log + "\nFPS: " + FPS.fps;
				log += "\n-----------\n\n";
			}
			if (!MemoryExtensions.AsSpan(pStackTrace).Trim().IsEmpty && pStackTrace == lastError)
			{
				errorRepeated++;
				return;
			}
			if (MemoryExtensions.AsSpan(pStackTrace).Trim().IsEmpty && pLogString == lastError)
			{
				errorRepeated++;
				return;
			}
			clearRepeat();
			log = log + "- error[" + errorNum + "]: " + pLogString + "\n";
			log = log + "- stack:\n" + pStackTrace + "\n";
			lastError = pStackTrace;
			File.AppendAllText(getPath(), log);
			errorNum++;
			openConsole();
		}
		else
		{
			clearRepeat();
			log = log + "- trace: " + pLogString + "\n";
		}
	}

	private static void openConsole()
	{
		if (Config.show_console_on_error && World.world != null && World.world.console != null && !toggledConsole)
		{
			toggledConsole = true;
			World.world.console.Show();
		}
	}

	private static void clearRepeat()
	{
		if (errorRepeated > 0)
		{
			log = log + "- last error repeated " + errorRepeated + " times\n";
			lastError = "";
			errorRepeated = 0;
		}
	}

	public static string getDirPath()
	{
		return Application.persistentDataPath + folder_base;
	}

	private static string getPath()
	{
		if (_filename == null)
		{
			_filename = getFileName();
		}
		return _filename;
	}

	private static string getFileName()
	{
		string tDateTimeNow = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
		return getDirPath() + dataName + "_" + tDateTimeNow + ".log";
	}
}
