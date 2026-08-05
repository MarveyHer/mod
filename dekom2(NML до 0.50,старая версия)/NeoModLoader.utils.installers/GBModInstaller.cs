using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NeoModLoader.constants;
using NeoModLoader.services;

namespace NeoModLoader.utils.installers;

internal class GBModInstaller : ACmdModInstaller
{
	private const string base_match_regex = "^(?<scheme>ncms|nml):(?<url_to_archive>.*)$";

	private const string addition_match_regex = "^(?<scheme>ncms|nml):(?<url_to_archive>.*),(?<mod_type>.*),(?<mod_id>.*)$";

	public override async Task<bool> CheckInstall(string pParam)
	{
		if (!pParam.StartsWith("ncms:") && !pParam.StartsWith("nml:"))
		{
			return false;
		}
		Match match;
		if (!Regex.IsMatch(pParam, "^(?<scheme>ncms|nml):(?<url_to_archive>.*),(?<mod_type>.*),(?<mod_id>.*)$"))
		{
			if (!Regex.IsMatch(pParam, "^(?<scheme>ncms|nml):(?<url_to_archive>.*)$"))
			{
				return false;
			}
			match = Regex.Match(pParam, "^(?<scheme>ncms|nml):(?<url_to_archive>.*)$");
		}
		else
		{
			match = Regex.Match(pParam, "^(?<scheme>ncms|nml):(?<url_to_archive>.*),(?<mod_type>.*),(?<mod_id>.*)$");
		}
		string url_to_archive = match.Groups["url_to_archive"].Value;
		using WebClient client = new WebClient();
		string zip_file_path = Path.Combine(Paths.ModsPath, Guid.NewGuid().ToString() + ".zip");
		await client.DownloadFileTaskAsync(new Uri(url_to_archive), zip_file_path);
		string mod_folder_path = ModInfoUtils.TryToUnzipModZip(zip_file_path);
		return ModCompileLoadService.TryCompileAndLoadModAtRuntime(ModInfoUtils.recogMod(mod_folder_path));
	}
}
