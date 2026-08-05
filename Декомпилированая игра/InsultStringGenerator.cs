using System.Collections.Generic;

public static class InsultStringGenerator
{
	private static string[] _insult_characters_2 = new string[4] { "#", "%", "&", "@" };

	private static string[] _insult_characters = new string[5] { "!", "#", "%", "&", "@" };

	private static List<string> _cached_bad_strings = new List<string>();

	private static List<string> _cached_bad_connections_string = new List<string>();

	private const int MAX_HARMFUL_DNA_SEQUENCES = 30;

	private const int MAX_BAD_CONNECTION_STRINGS = 30;

	public static string getRandomText(int pMin = 4, int pMax = 9, bool pUseSameSizeSet = false)
	{
		using StringBuilderPool tLocalNameBuilder = new StringBuilderPool();
		int tRandomSize = Randy.randomInt(pMin, pMax);
		for (int i = 0; i < tRandomSize; i++)
		{
			string tRandomCharacter = (pUseSameSizeSet ? _insult_characters_2.GetRandom() : _insult_characters.GetRandom());
			tLocalNameBuilder.Append(tRandomCharacter);
		}
		return tLocalNameBuilder.ToString();
	}

	public static string getDNASequenceBad()
	{
		string tResult;
		if (_cached_bad_strings.Count < 30)
		{
			using StringBuilderPool tBuilder = new StringBuilderPool();
			for (int i = 0; i < 6; i++)
			{
				if (i > 0)
				{
					tBuilder.Append(" ");
				}
				tBuilder.Append(getRandomText(3, 3, pUseSameSizeSet: true));
			}
			tResult = tBuilder.ToString();
			tResult = Toolbox.coloredString(tResult, "#B159FF");
			_cached_bad_strings.Add(tResult);
		}
		else
		{
			tResult = _cached_bad_strings.GetRandom();
		}
		return tResult;
	}

	public static string getBadConnectionString()
	{
		string tResult;
		if (_cached_bad_connections_string.Count < 30)
		{
			tResult = getRandomText(7, 7, pUseSameSizeSet: true);
			tResult = Toolbox.coloredString(tResult, "#B159FF");
			_cached_bad_connections_string.Add(tResult);
		}
		else
		{
			tResult = _cached_bad_connections_string.GetRandom();
		}
		return tResult;
	}
}
