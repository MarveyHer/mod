using System;
using UnityEngine;

public class LogHandlerInit : MonoBehaviour
{
	private void Awake()
	{
		try
		{
			LogHandler.init();
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			throw;
		}
	}
}
