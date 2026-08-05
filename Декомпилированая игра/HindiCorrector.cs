using System;

public class HindiCorrector
{
	private static string[] hindi_letters = new string[178]
	{
		"‘", "’", "“", "”", "(", ")", "{", "}", "=", "।",
		"?", "-", "µ", "॰", ",", ".", "\u094d ", "०", "१", "२",
		"३", "४", "५", "६", "७", "८", "९", "x", ":", "ल\u094dम",
		"ङ", "ऩ", "ऱ", "य\u093c", "ग़", "ड़", "ढ़", "ख़\u094dय", "ख़\u094d", "ख़",
		"क़\u094dय", "क़\u094d", "क़", "फ\u093c\u094d", "फ़", "ज़\u094dय", "ज़\u094d", "ज़", "त\u094dत\u094d", "त\u094dत",
		"क\u094dत", "द\u0943", "क\u0943", "ह\u094dन", "ह\u094dय", "ह\u0943", "ह\u094dम", "ह\u094dर", "ह\u094d", "द\u094dद",
		"क\u094dष\u094d", "क\u094dष", "त\u094dर\u094d", "त\u094dर", "ज\u094dञ", "छ\u094dय", "ट\u094dय", "ठ\u094dय", "ड\u094dय", "ढ\u094dय",
		"द\u094dय", "द\u094dव", "श\u094dर", "ट\u094dर", "ड\u094dर", "ढ\u094dर", "छ\u094dर", "क\u094dर", "फ\u094dर", "द\u094dर",
		"प\u094dर", "ग\u094dर", "र\u0941", "र\u0942", "\u094dर", "ओ", "औ", "आ", "अ", "ई",
		"इ", "उ", "ऊ", "ऐ", "ए", "ऋ", "क\u094d", "क", "क\u094dक", "ख\u094d",
		"ख", "ग\u094d", "ग", "घ\u094d", "घ", "ङ", "च\u0948", "च\u094d", "च", "छ",
		"ज\u094d", "ज", "झ\u094d", "झ", "ञ", "ट\u094dट", "ट\u094dठ", "ट", "ठ", "ड\u094dड",
		"ड\u094dढ", "ड", "ढ", "ण\u094d", "ण", "त\u094d", "त", "थ\u094d", "थ", "द\u094dध",
		"द", "ध\u094d", "ध", "न\u094d", "न", "प\u094d", "प", "फ\u094d", "फ", "ब\u094d",
		"ब", "भ\u094d", "भ", "म\u094d", "म", "य\u094d", "य", "र", "ल\u094d", "ल",
		"ळ", "व\u094d", "व", "श\u094d", "श", "ष\u094d", "ष", "स\u094d", "स", "ह",
		"ऑ", "\u0949", "\u094b", "\u094c", "\u093e", "\u0940", "\u0941", "\u0942", "\u0943", "\u0947",
		"\u0948", "\u0902", "\u0901", "\u0903", "\u0945", "ऽ", "\u094d ", "\u094d"
	};

	private static string[] replace_letters = new string[178]
	{
		"^", "*", "Þ", "ß", "¼", "½", "¿", "À", "¾", "A",
		"\\", "&", "&", "Œ", "]", "-", "~ ", "å", "ƒ", "„",
		"…", "†", "‡", "ˆ", "‰", "Š", "‹", "Û", "%", "Ye",
		"³", "u+", "j+", ";+", "x+", "M+", "<+", "[+;", "[+", "[k+",
		"D+;", "D+", "d+", "¶+", "Q+", "T+;", "T+", "t+", "Ù", "Ùk",
		"Dr", "–", "—", "à", "á", "â", "ã", "ºz", "º", "í",
		"{", "{k", "«", "=", "K", "Nî", "Vî", "Bî", "Mî", "<î",
		"|", "}", "J", "Vª", "Mª", "<ªª", "Nª", "Ø", "Ý", "æ",
		"ç", "xz", "#", ":", "z", "vks", "vkS", "vk", "v", "bZ",
		"b", "m", "Å", ",s", ",", "_", "D", "d", "ô", "[",
		"[k", "X", "x", "?", "?k", "³", "pkS", "P", "p", "N",
		"T", "t", "÷", ">", "¥", "ê", "ë", "V", "B", "ì",
		"ï", "M", "<", ".", ".k", "R", "r", "F", "Fk", ")",
		"n", "/", "/k", "U", "u", "I", "i", "¶", "Q", "C",
		"c", "H", "Hk", "E", "e", "\u00b8", ";", "j", "Y", "y",
		"G", "O", "o", "'", "'k", "\"", "\"k", "L", "l", "g",
		"v‚", "‚", "ks", "kS", "k", "h", "q", "w", "`", "s",
		"S", "a", "¡", "%", "W", "·", "~ ", "~"
	};

	public static string GetCorrectedHindiText(string unicode_substring)
	{
		int array_one_length = hindi_letters.Length;
		string modified_substring = unicode_substring;
		for (int position_of_quote = modified_substring.IndexOf("'", StringComparison.Ordinal); position_of_quote >= 0; position_of_quote = modified_substring.IndexOf("'", StringComparison.Ordinal))
		{
			modified_substring = ReplaceFirstOccurrence(modified_substring, "'", "^");
			modified_substring = ReplaceFirstOccurrence(modified_substring, "'", "*");
		}
		for (int position_of_Dquote = modified_substring.IndexOf("\"", StringComparison.Ordinal); position_of_Dquote >= 0; position_of_Dquote = modified_substring.IndexOf("\"", StringComparison.Ordinal))
		{
			modified_substring = ReplaceFirstOccurrence(modified_substring, "\"", "ß");
			modified_substring = ReplaceFirstOccurrence(modified_substring, "\"", "Þ");
		}
		for (int position_of_f = modified_substring.IndexOf("\u093f", StringComparison.Ordinal); position_of_f != -1; position_of_f = modified_substring.IndexOf("\u093f", position_of_f + 1, StringComparison.Ordinal))
		{
			char character_left_to_f = modified_substring[position_of_f - 1];
			modified_substring = modified_substring.Replace(character_left_to_f + "\u093f", "f" + character_left_to_f);
			while (modified_substring.Contains("\u094df" + character_left_to_f))
			{
				int index = modified_substring.IndexOf("\u094df" + character_left_to_f, StringComparison.Ordinal);
				modified_substring = modified_substring.Replace(modified_substring[index - 1] + "\u094df" + character_left_to_f, "f" + modified_substring[index - 1] + "\u094d" + character_left_to_f);
			}
		}
		string set_of_matras = "\u093e\u093f\u0940\u0941\u0942\u0943\u0947\u0948\u094b\u094c\u0902:\u0901\u0945";
		modified_substring += "  ";
		for (int position_of_half_R = modified_substring.IndexOf("र\u094d", StringComparison.Ordinal); position_of_half_R > 0; position_of_half_R = modified_substring.IndexOf("र\u094d", StringComparison.Ordinal))
		{
			int probable_position_of_Z = position_of_half_R + 2;
			if (modified_substring[probable_position_of_Z + 1] == '\u094d')
			{
				probable_position_of_Z += 2;
			}
			char character_right_to_probable_position_of_Z = modified_substring[probable_position_of_Z + 1];
			while (set_of_matras.IndexOf(character_right_to_probable_position_of_Z) != -1)
			{
				probable_position_of_Z++;
				character_right_to_probable_position_of_Z = modified_substring[probable_position_of_Z + 1];
			}
			string string_to_be_Replaced = modified_substring.Substring(position_of_half_R + 2, probable_position_of_Z - position_of_half_R - 1);
			modified_substring = modified_substring.Replace("र\u094d" + string_to_be_Replaced, string_to_be_Replaced + "Z");
		}
		modified_substring = modified_substring.Substring(0, modified_substring.Length - 2);
		for (int input_symbol_idx = 0; input_symbol_idx < array_one_length; input_symbol_idx++)
		{
			int idx = 0;
			if (modified_substring.Contains(hindi_letters[input_symbol_idx]))
			{
				while (idx != -1)
				{
					modified_substring = modified_substring.Replace(hindi_letters[input_symbol_idx], replace_letters[input_symbol_idx]);
					idx = modified_substring.IndexOf(hindi_letters[input_symbol_idx], StringComparison.Ordinal);
				}
			}
		}
		return modified_substring;
	}

	public static string ReplaceFirstOccurrence(string Source, string Find, string Replace)
	{
		int Place = Source.IndexOf(Find, StringComparison.Ordinal);
		if (Place < 0)
		{
			return Source;
		}
		return Source.Remove(Place, Find.Length).Insert(Place, Replace);
	}
}
