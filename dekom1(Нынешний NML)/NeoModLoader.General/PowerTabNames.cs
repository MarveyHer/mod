using System.Collections.Generic;

namespace NeoModLoader.General;

public static class PowerTabNames
{
	public const string Main = "main";

	public const string Drawing = "creation";

	public const string Kingdoms = "noosphere";

	public const string Creatures = "units";

	public const string Nature = "nature";

	public const string Bombs = "destruction";

	public const string Other = "other";

	public static List<string> GetNames()
	{
		return new List<string> { "main", "creation", "noosphere", "units", "nature", "destruction", "other" };
	}
}
