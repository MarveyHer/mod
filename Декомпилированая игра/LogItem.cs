using System;
using UnityEngine;

public readonly struct LogItem
{
	public readonly string log;

	public readonly string stack_trace;

	public readonly LogType type;

	public readonly DateTime time;

	public LogItem(string pLog, string pStackTrace, LogType pType)
	{
		log = pLog;
		stack_trace = pStackTrace;
		type = pType;
		time = DateTime.Now;
	}

	public LogItem(string pLog, string pStackTrace, LogType pType, DateTime pTime)
	{
		log = pLog;
		stack_trace = pStackTrace;
		type = pType;
		time = pTime;
	}
}
