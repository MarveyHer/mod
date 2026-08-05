using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

public static class Date
{
	public static string getAgoString(double pTimestamp)
	{
		return formatSeconds(World.world.getWorldTimeElapsedSince(pTimestamp)) + " ago";
	}

	public static string formatSeconds(float pSeconds)
	{
		if (pSeconds < 60f)
		{
			return ((int)pSeconds).ToText() + "s";
		}
		string tResult = ((int)pSeconds / 60).ToText();
		return tResult + "m";
	}

	public static float getMonthTime()
	{
		int tTotalMonths = getMonthsSince(0.0);
		return (float)World.world.getCurWorldTime() - (float)tTotalMonths * 5f;
	}

	public static string getYearDate(double pTime)
	{
		return getYear(pTime).ToText();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int getYear(double pTime)
	{
		return getYear0(pTime) + 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int getYear0(double pTime)
	{
		return (int)(pTime / 60.0);
	}

	public static int[] getRawDate(double pTime)
	{
		int tDateYear = (int)(pTime / 5.0 / 12.0);
		while (pTime < 0.0)
		{
			pTime += 600000.0;
		}
		double num = pTime / 5.0;
		double tTotalYearTime = num / 12.0;
		int tTotalMonths = (int)num;
		int tTotalYears = (int)tTotalYearTime;
		int tDateMonth = (int)((pTime - (double)((float)tTotalYears * 5f * 12f)) / 5.0);
		int tDateDay = (int)((pTime - (double)((float)tTotalMonths * 5f)) / 5.0 * 30.0);
		tDateYear++;
		tDateMonth++;
		tDateDay++;
		return new int[3] { tDateDay, tDateMonth, tDateYear };
	}

	public static string getDate(double pTime)
	{
		int[] rawDate = getRawDate(pTime);
		int tDateDay = rawDate[0];
		int tDateMonth = rawDate[1];
		int tDateYear = rawDate[2];
		if (LocalizedTextManager.instance.language == "en")
		{
			using (StringBuilderPool tString = new StringBuilderPool())
			{
				tString.Append(tDateDay);
				tString.Append(GetDaySuffix(tDateDay));
				tString.Append(" of ");
				tString.Append(formatMonth(tDateMonth));
				tString.Append(", ");
				tString.Append(tDateYear.ToText());
				return tString.ToString();
			}
		}
		return formatDate(tDateDay, tDateMonth, tDateYear);
	}

	internal static string formatMonth(int pMonth)
	{
		return LocalizedTextManager.getText("month_" + pMonth);
	}

	internal static string formatDate(int pDay, int pMonth, int pYear)
	{
		CultureInfo culture = LocalizedTextManager.getCulture();
		string tLongDatePattern = culture.DateTimeFormat.LongDatePattern;
		if (culture.TwoLetterISOLanguageName == "ar")
		{
			tLongDatePattern = "/ddMMMMyyyy/";
		}
		string tResult = Regex.Replace(tLongDatePattern, "\\bdddd[,\\s]*", "").Trim();
		string tMonthTranslation = LocalizedTextManager.getText("inflected_month_" + pMonth);
		MatchCollection tMatches = null;
		if (tResult.Contains("'"))
		{
			tMatches = Regex.Matches(tResult, "'[^']*'");
			for (int i = 0; i < tMatches.Count; i++)
			{
				tResult = tResult.Replace(tMatches[i].Value, "{{{" + i + "}}}");
			}
		}
		tResult = tResult.Replace("MMMM", "[[[1]]]");
		tResult = tResult.Replace("yyyy", "[[[2]]]");
		tResult = tResult.Replace("dd", "[[[3]]]");
		tResult = Regex.Replace(tResult, "\\b[d]\\b", "[[[4]]]");
		tResult = tResult.Replace("MM", "[[[5]]]");
		tResult = tResult.Replace("M", "[[[6]]]");
		tResult = tResult.Replace("[[[1]]]", tMonthTranslation);
		tResult = tResult.Replace("[[[2]]]", pYear.ToText());
		tResult = tResult.Replace("[[[3]]]", (pDay < 10) ? ("0" + pDay) : pDay.ToString());
		tResult = tResult.Replace("[[[4]]]", pDay.ToString());
		tResult = tResult.Replace("[[[5]]]", (pMonth < 10) ? ("0" + pMonth) : pMonth.ToString());
		tResult = tResult.Replace("[[[6]]]", pMonth.ToString());
		if (tMatches != null && tMatches.Count > 0)
		{
			for (int i2 = tMatches.Count - 1; i2 >= 0; i2--)
			{
				tResult = tResult.Replace("{{{" + i2 + "}}}", tMatches[i2].Value.Trim('\''));
			}
		}
		return tResult;
	}

	public static int getCurrentMonth()
	{
		return getMonth(World.world.getCurWorldTime());
	}

	public static int getMonth(double pTimestamp)
	{
		float tYear = getYear0(pTimestamp);
		return (int)((pTimestamp - (double)(tYear * 12f * 5f)) / 5.0 + 1.0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int getCurrentYear()
	{
		return getYear(World.world.getCurWorldTime());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int getYearsSince(double pFrom)
	{
		return getYear0(World.world.getCurWorldTime() - pFrom);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int getMonthsSince(double pFrom)
	{
		return (int)((World.world.getCurWorldTime() - pFrom) / 5.0);
	}

	private static string GetDaySuffix(int day)
	{
		switch (day)
		{
		case 1:
		case 21:
		case 31:
			return "st";
		case 2:
		case 22:
			return "nd";
		case 3:
		case 23:
			return "rd";
		default:
			return "th";
		}
	}

	public static bool isMonolithMonth()
	{
		if (getCurrentMonth() == 4)
		{
			return true;
		}
		return false;
	}

	public static string getUIStringYearMonthShort()
	{
		return "y:" + getCurrentYear().ToText() + ", m:" + getCurrentMonth().ToText();
	}

	public static string getUIStringYearMonth()
	{
		return "y: " + getCurrentYear().ToText() + ", m: " + getCurrentMonth().ToText();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string TimeNow()
	{
		DateTime dt = DateTime.Now;
		char[] array = new char[8];
		Write2Chars(array, 0, dt.Hour);
		array[2] = ':';
		Write2Chars(array, 3, dt.Minute);
		array[5] = ':';
		Write2Chars(array, 6, dt.Second);
		return new string(array);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Write2Chars(char[] chars, int offset, int value)
	{
		chars[offset] = Digit(value / 10);
		chars[offset + 1] = Digit(value % 10);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static char Digit(int value)
	{
		return (char)(value + 48);
	}
}
