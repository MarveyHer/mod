using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace NeoModLoader.utils.authentication;

internal static class DiscordCommonAuthLogic
{
	internal static IEnumerable<string> GetRolesOfUser(string user_id)
	{
		HttpResponseMessage httpResponseMessage = HttpUtils.Get("http://95.216.161.50:3000/user/roles/" + user_id, new Dictionary<string, string>());
		if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
		{
			httpResponseMessage = HttpUtils.Get("https://keymasterer.uk:5000/user/roles/" + user_id, new Dictionary<string, string>());
		}
		string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
		return from role in result.Trim('[', ']', ' ').Split(',')
			select role.Trim('"', ' ');
	}

	internal static bool ModderIsInRolesList(IEnumerable<string> roles)
	{
		return roles.Any((string role) => role == "647734005625651220");
	}
}
