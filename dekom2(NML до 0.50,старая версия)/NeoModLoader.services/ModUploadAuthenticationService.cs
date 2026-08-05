using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeoModLoader.api;
using NeoModLoader.General;
using NeoModLoader.ui;
using NeoModLoader.utils.authentication;
using RSG;

namespace NeoModLoader.services;

public static class ModUploadAuthenticationService
{
	public static bool Authed { get; private set; }

	public static void AutoAuth()
	{
		new Task(delegate
		{
			int num = 0;
			foreach (Func<bool> all_auto_auth_func in ModUploadAuthenticationWindow.all_auto_auth_funcs)
			{
				try
				{
					LogService.LogInfoConcurrent($"Trying auto auth at {num}...");
					Authed = all_auto_auth_func();
					if (Authed)
					{
						LogService.LogInfoConcurrent("Auto auth success!");
						break;
					}
					LogService.LogInfoConcurrent($"Failed auto auth at {num}.");
				}
				catch (Exception ex)
				{
					LogService.LogInfoConcurrent($"Failed auto auth at {num}: {ex.Message}");
				}
				finally
				{
					num++;
				}
			}
		}).Start();
	}

	public static Promise Authenticate()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		Promise promise = new Promise();
		if (Authed)
		{
			new Task(delegate
			{
				Thread.Sleep(500);
				promise.Resolve();
			}).Start();
			return promise;
		}
		ScrollWindow.showWindow(AbstractWindow<ModUploadAuthenticationWindow>.WindowId);
		new Task(delegate
		{
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			while (true)
			{
				if (!AbstractWindow<ModUploadAuthenticationWindow>.Instance.Opened())
				{
					promise.Reject(new Exception("Canceled"));
					return;
				}
				if (AbstractWindow<ModUploadAuthenticationWindow>.Instance.AuthSkipped)
				{
					promise.Resolve();
					return;
				}
				if (AbstractWindow<ModUploadAuthenticationWindow>.Instance.AuthFuncSelected)
				{
					AbstractWindow<ModUploadAuthenticationWindow>.Instance.AuthFuncSelected = false;
					bool result;
					try
					{
						CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
						Task<bool> task = new Task<bool>(AbstractWindow<ModUploadAuthenticationWindow>.Instance.AuthFunc, cancellationTokenSource.Token);
						ModUploadAuthenticationWindow.SetText(LM.Get("NML_AUTHENTICATION_WAITING"));
						task.Start();
						int num = 0;
						while (!task.IsCompleted)
						{
							Thread.Sleep(100);
							num += 100;
							if (num >= 60000)
							{
								cancellationTokenSource.Cancel();
								throw new AuthenticaticationException("Authentication timeout.");
							}
						}
						if (task.IsFaulted && task.Exception != null)
						{
							throw task.Exception;
						}
						result = task.Result;
					}
					catch (AuthenticaticationException ex)
					{
						Exception ex2 = ex;
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.AppendLine("Exception when auth: ");
						do
						{
							stringBuilder.AppendLine($"{ex.GetType()}: {ex.Message}");
							stringBuilder.AppendLine(ex.StackTrace);
							ex2 = ex2.InnerException;
						}
						while (ex2 != null);
						LogService.LogInfoConcurrent(stringBuilder.ToString());
						ModUploadAuthenticationWindow.SetState(pAuthState: false, ex.Message);
						continue;
					}
					catch (Exception innerException)
					{
						StringBuilder stringBuilder2 = new StringBuilder();
						stringBuilder2.AppendLine("Exception when auth: ");
						do
						{
							stringBuilder2.AppendLine($"{innerException.GetType()}: {innerException.Message}");
							stringBuilder2.AppendLine(innerException.StackTrace);
							innerException = innerException.InnerException;
						}
						while (innerException != null);
						LogService.LogInfoConcurrent(stringBuilder2.ToString());
						ModUploadAuthenticationWindow.SetState(pAuthState: false, innerException.Message);
						continue;
					}
					LogService.LogInfoConcurrent($"Auth result: {result}");
					if (result)
					{
						break;
					}
					ModUploadAuthenticationWindow.SetState(pAuthState: false);
				}
			}
			Authed = true;
			ModUploadAuthenticationWindow.SetState(pAuthState: true);
			promise.Resolve();
		}).Start();
		return promise;
	}
}
