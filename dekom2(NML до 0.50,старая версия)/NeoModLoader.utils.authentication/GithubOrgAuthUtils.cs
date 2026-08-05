using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using NeoModLoader.General;
using NeoModLoader.ui;
using Newtonsoft.Json;
using UnityEngine;

namespace NeoModLoader.utils.authentication;

public static class GithubOrgAuthUtils
{
	private struct TokenInfo
	{
		public string access_token;

		public string token_type;

		public string scope;
	}

	private struct UserInfo
	{
		public string login;
	}

	private struct DeviceFlow
	{
		public string device_code;

		public string user_code;

		public string verification_uri;

		public int interval;

		public int expires_in;
	}

	private const string client_id = "Iv1.c85ea6bddeb2ed41";

	private static string domain = "github.com";

	private static readonly string[] _alter_domains = new string[1] { "github.com" };

	public static bool Authenticate()
	{
		string tokenByDeviceFlow = GetTokenByDeviceFlow();
		if (string.IsNullOrEmpty(tokenByDeviceFlow))
		{
			return false;
		}
		HttpResponseMessage httpResponseMessage = HttpUtils.Get("https://api." + domain + "/user", new Dictionary<string, string>
		{
			{
				"Authorization",
				"Bearer " + tokenByDeviceFlow
			},
			{ "User-Agent", "NeoModLoader" }
		});
		UserInfo userInfo = JsonConvert.DeserializeObject<UserInfo>(httpResponseMessage.Content.ReadAsStringAsync().Result);
		httpResponseMessage = HttpUtils.Get("https://api." + domain + "/orgs/WorldBoxOpenMods/members/" + userInfo.login, new Dictionary<string, string>
		{
			{
				"Authorization",
				"Bearer " + tokenByDeviceFlow
			},
			{ "User-Agent", "NeoModLoader" },
			{ "Accept", "application/vnd.github.v3+json" }
		});
		if (httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
		{
			return true;
		}
		return false;
	}

	private static string GetTokenByDeviceFlow()
	{
		string text = "";
		string[] alter_domains = _alter_domains;
		foreach (string text2 in alter_domains)
		{
			try
			{
				text = HttpUtils.Post("https://" + text2 + "/login/device/code", new Dictionary<string, string> { { "client_id", "Iv1.c85ea6bddeb2ed41" } }, new Dictionary<string, string> { { "Accept", "application/json" } }, 5.0);
				if (!string.IsNullOrEmpty(text))
				{
					domain = text2;
					break;
				}
			}
			catch (Exception)
			{
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			throw new AuthenticaticationException("Failed to get device code.");
		}
		DeviceFlow deviceFlow = JsonConvert.DeserializeObject<DeviceFlow>(text);
		InformationWindow.ShowWindow(string.Format(LM.Get("GithubAuth Tip"), deviceFlow.user_code));
		Application.OpenURL(deviceFlow.verification_uri);
		int num = 0;
		while (num < deviceFlow.expires_in * 1000)
		{
			Thread.Sleep(deviceFlow.interval * 1000);
			num += deviceFlow.interval * 1000;
			text = HttpUtils.Post("https://" + domain + "/login/oauth/access_token", new Dictionary<string, string>
			{
				{ "client_id", "Iv1.c85ea6bddeb2ed41" },
				{ "device_code", deviceFlow.device_code },
				{ "grant_type", "urn:ietf:params:oauth:grant-type:device_code" }
			}, new Dictionary<string, string> { { "Accept", "application/json" } });
			if (text.Contains("access_token"))
			{
				break;
			}
		}
		InformationWindow.Back();
		return JsonConvert.DeserializeObject<TokenInfo>(text).access_token;
	}
}
