#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace NeoModLoader.utils.authentication;

public class DiscordRoleAuthViaUserLoginUtils
{
	private struct TokenInfo
	{
		public string access_token;

		public string token_type;

		public string expires_in;

		public string refresh_token;

		public string scope;
	}

	private const string client_id = "1171719697557880892";

	public static bool Authenticate()
	{
		return DiscordCommonAuthLogic.ModderIsInRolesList(DiscordCommonAuthLogic.GetRolesOfUser(GetUserID(GetAuthToken())));
	}

	public static void Test()
	{
		TokenInfo authToken = GetAuthToken();
		Debug.WriteLine(authToken.access_token);
		string userID = GetUserID(authToken);
		Debug.WriteLine(userID);
		IEnumerable<string> rolesOfUser = DiscordCommonAuthLogic.GetRolesOfUser(userID);
		bool flag = DiscordCommonAuthLogic.ModderIsInRolesList(rolesOfUser);
		Debug.WriteLine(flag);
		if (flag)
		{
			Console.WriteLine("You are a modder!");
		}
		else
		{
			Console.WriteLine("You are not a modder!");
		}
		Console.WriteLine("Tests:");
		rolesOfUser = DiscordCommonAuthLogic.GetRolesOfUser("1171719697557880892");
		rolesOfUser.ToList().ForEach(Console.WriteLine);
		rolesOfUser = DiscordCommonAuthLogic.GetRolesOfUser("0000000000000000000");
		rolesOfUser.ToList().ForEach(Console.WriteLine);
	}

	private static string GetUserID(TokenInfo token_info)
	{
		HttpResponseMessage httpResponseMessage = HttpUtils.Get("https://discordapp.com/api/users/@me", new Dictionary<string, string> { 
		{
			"Authorization",
			token_info.token_type + " " + token_info.access_token
		} });
		string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
		string[] source = result.Trim(' ', 'd', 'a', 't', 'a', ':', '{', '}').Split(',');
		using (IEnumerator<string[]> enumerator = (from segment in source
			select segment.Split(':') into pair
			where pair[0].Trim('"', ' ') == "id"
			select pair).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				string[] current = enumerator.Current;
				return current[1].Trim('"', ' ');
			}
		}
		return "";
	}

	private static TokenInfo GetAuthToken()
	{
		HttpListener listener = new HttpListener();
		listener.Prefixes.Add("http://localhost:36549/");
		listener.Start();
		Application.OpenURL("https://discord.com/api/oauth2/authorize?client_id=1171719697557880892&redirect_uri=http%3A%2F%2Flocalhost%3A36549&response_type=code&scope=identify");
		new Task(delegate
		{
			HttpListener httpListener = listener;
			int num = 0;
			while (num < 60000)
			{
				if (!httpListener.IsListening)
				{
					return;
				}
				num += 100;
				Thread.Sleep(100);
			}
			httpListener.Close();
		}).Start();
		HttpListenerContext context;
		try
		{
			context = listener.GetContext();
		}
		catch (InvalidOperationException innerException)
		{
			throw new Exception("Failed to get context", innerException);
		}
		HttpListenerRequest request = context.Request;
		HttpListenerResponse response = context.Response;
		string text;
		try
		{
			text = request.QueryString["code"];
			string text2 = "<html><head><title>NeoModLoader</title><style>body {background-color: black; color: white;}</style></head><body>Success!<br>You can close this page!</body></html>";
			response.OutputStream.Write((from c in text2.ToCharArray()
				select (byte)c).ToArray(), 0, text2.Length);
		}
		catch (Exception)
		{
			string text2 = "<html><head><title>NeoModLoader</title><style>body {background-color: black; color: white;}</style></head><body>Error!<br>Authentication declined!</body></html>";
			Debug.LogWarning((object)"Manual Discord Authentication declined!");
			response.OutputStream.Write((from c in text2.ToCharArray()
				select (byte)c).ToArray(), 0, text2.Length);
			throw new AuthenticaticationException("Discord user authentication declined.");
		}
		response.Close();
		Debug.WriteLine(text);
		listener.Close();
		HttpResponseMessage result;
		using (HttpClient httpClient = new HttpClient())
		{
			result = httpClient.GetAsync("https://keymasterer.uk/nml/api/get-discord-access-token/" + text).Result;
		}
		string result2 = result.Content.ReadAsStringAsync().Result;
		Debug.WriteLine(result2);
		Console.WriteLine(result2);
		string[] array = result2.Split(',');
		return new TokenInfo
		{
			token_type = array[0].Split(':')[1].Trim('"', ' '),
			access_token = array[1].Split(':')[1].Trim('"', ' '),
			expires_in = array[2].Split(':')[1].Trim('"', ' '),
			refresh_token = array[3].Split(':')[1].Trim('"', ' '),
			scope = array[4].Split(':')[1].Trim('"', ' ', '}')
		};
	}
}
