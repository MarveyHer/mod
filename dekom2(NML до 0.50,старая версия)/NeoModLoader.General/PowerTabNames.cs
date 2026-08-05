using System.Collections.Generic;

namespace NeoModLoader.General;

public static class PowerTabNames
{
	public const string Main = "Tab_Main";

	public const string Drawing = "Tab_Drawing";

	public const string Kingdoms = "Tab_Kingdoms";

	public const string Creatures = "Tab_Creatures";

	public const string Nature = "Tab_Nature";

	public const string Bombs = "Tab_Bombs";

	public const string Other = "Tab_Other";

	public static List<string> GetNames()
	{
		return new List<string> { "Tab_Main", "Tab_Drawing", "Tab_Kingdoms", "Tab_Creatures", "Tab_Nature", "Tab_Bombs", "Tab_Other" };
	}
}
