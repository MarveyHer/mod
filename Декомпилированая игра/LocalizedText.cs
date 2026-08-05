using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UPersian.Utils;

public class LocalizedText : UIBehaviour
{
	public const string DEFAULT_KEY = "??????";

	protected const char LINE_ENDING = '\n';

	public bool convertToUppercase;

	public bool autoField = true;

	public bool specialTags;

	public string key = "??????";

	private FontStyle? _font_style_before;

	private bool? _shadow_before = false;

	private bool _has_shadow;

	internal Text text;

	private TextAnchor? _text_alignment_before;

	protected override void Awake()
	{
		base.Awake();
		text = GetComponent<Text>();
	}

	protected override void Start()
	{
		base.Start();
		if (autoField)
		{
			LocalizedTextManager.addTextField(this);
			updateText();
		}
	}

	public void setKeyAndUpdate(string pKey)
	{
		key = pKey;
		updateText();
	}

	protected override void OnRectTransformDimensionsChange()
	{
		GameLanguageAsset current_language = LocalizedTextManager.current_language;
		if ((current_language == null || current_language.isRTL()) && !string.IsNullOrEmpty(key) && !(key == "??????") && !(text == null))
		{
			updateText();
			base.OnRectTransformDimensionsChange();
		}
	}

	internal virtual void updateText(bool pCheckText = true)
	{
		if (text == null || LocalizedTextManager.instance == null || !LocalizedTextManager.instance.initiated)
		{
			return;
		}
		if (LocalizedTextManager.current_font != null)
		{
			text.font = LocalizedTextManager.current_font;
		}
		string tText = LocalizedTextManager.getText(key, text);
		if (convertToUppercase)
		{
			tText = tText.ToUpper();
		}
		if (specialTags && tText.Contains("$"))
		{
			if (tText.Contains("$total_prem_powers$"))
			{
				tText = tText.Replace("$total_prem_powers$", GodPower.premium_powers.Count.ToString() ?? "");
			}
			if (tText.Contains("$minutes$"))
			{
				tText = tText.Replace("$minutes$", 30.ToString() ?? "");
			}
			if (tText.Contains("$minutes_clock$"))
			{
				tText = tText.Replace("$minutes_clock$", 720.ToString() ?? "");
			}
			if (tText.Contains("$hours_clock$"))
			{
				tText = tText.Replace("$hours_clock$", 12.ToString() ?? "");
			}
			if (tText.Contains("$power$") && Config.power_to_unlock != null)
			{
				tText = tText.Replace("$power$", Config.power_to_unlock.getLocaleID().Localize() ?? "");
			}
			if (tText.Contains("$hours$"))
			{
				tText = tText.Replace("$hours$", 3.ToString() ?? "");
			}
			if (tText.Contains("$number$"))
			{
				tText = tText.Replace("$number$", 3.ToString() ?? "");
			}
			if (tText.Contains("$discord_count$"))
			{
				tText = tText.Replace("$discord_count$", 560000.ToText() ?? "");
			}
			if (tText.Contains("$wbcode$"))
			{
				tText = tText.Replace("$wbcode$", "<color=cyan>WB-5555-1166-5555</color>");
			}
			if (tText.Contains("$lifeissimhours$"))
			{
				tText = tText.Replace("$lifeissimhours$", 24f.ToText());
			}
			if (tText.Contains("$current_era_year"))
			{
				tText = tText.Replace("$current_era_year$", Date.getCurrentYear().ToText());
			}
			if (tText.Contains("$era_moons_left"))
			{
				int tMoonsLeft = World.world.era_manager.calculateMoonsLeft();
				tText = tText.Replace("$era_moons_left$", tMoonsLeft.ToText());
			}
		}
		text.text = tText;
		checkTextFont();
		if (pCheckText)
		{
			checkSpecialLanguages();
		}
	}

	internal void checkTextFont(GameLanguageAsset pLanguage = null)
	{
		if (!(text == null))
		{
			if (pLanguage == null)
			{
				pLanguage = LocalizedTextManager.current_language;
			}
			Font tFont = pLanguage.font();
			if (!(tFont == null))
			{
				text.font = tFont;
			}
		}
	}

	internal void checkSpecialLanguages(GameLanguageAsset pLanguage = null)
	{
		if (text == null)
		{
			return;
		}
		if (pLanguage == null)
		{
			pLanguage = LocalizedTextManager.current_language;
		}
		checkTextFont(pLanguage);
		if (!_text_alignment_before.HasValue)
		{
			_text_alignment_before = text.alignment;
		}
		if (!_font_style_before.HasValue)
		{
			_font_style_before = text.fontStyle;
		}
		if (!_shadow_before.HasValue)
		{
			_shadow_before = (_has_shadow = text.HasComponent<Shadow>());
		}
		if (pLanguage.hasForcedStyle())
		{
			text.fontStyle = pLanguage.force_style.style;
			if (text.fontSize < 9 && pLanguage.force_style.shadow && !_has_shadow)
			{
				text.gameObject.AddComponent<Shadow>().effectColor = new Color(0f, 0f, 0f, 160f);
				_has_shadow = true;
			}
		}
		else
		{
			text.fontStyle = _font_style_before.Value;
			if (_has_shadow && _shadow_before == false)
			{
				if (text.TryGetComponent<Shadow>(out var tTextShadow))
				{
					Object.Destroy(tTextShadow);
				}
				_has_shadow = false;
			}
		}
		if (pLanguage.isRTL())
		{
			text.text = getRTLText(text, text.text);
			text.alignment = getRTLAlignment(_text_alignment_before.Value);
		}
		else
		{
			text.alignment = _text_alignment_before.Value;
		}
		if (pLanguage.isHindi() && !Regex.IsMatch(text.text, "[a-zA-Z]"))
		{
			text.SetHindiText(text.text);
		}
	}

	internal static string getRTLText(Text pText, string pString)
	{
		pText.cachedTextGenerator.Populate(pString, pText.GetGenerationSettings(pText.rectTransform.rect.size));
		if (!(pText.cachedTextGenerator.lines is List<UILineInfo> tLines))
		{
			return null;
		}
		string tLinedText = "";
		if (tLines.Count == 0)
		{
			tLinedText = pString;
		}
		for (int i = 0; i < tLines.Count; i++)
		{
			if (i < tLines.Count - 1)
			{
				int startIndex = tLines[i].startCharIdx;
				int length = tLines[i + 1].startCharIdx - tLines[i].startCharIdx;
				tLinedText += pString.Substring(startIndex, length);
				if (tLinedText.Length > 0 && tLinedText[tLinedText.Length - 1] != '\n' && tLinedText[tLinedText.Length - 1] != '\r')
				{
					tLinedText += "\n";
				}
			}
			else
			{
				tLinedText += pString.Substring(tLines[i].startCharIdx);
			}
		}
		UPersianUtils.RtlFix(ref tLinedText);
		return tLinedText;
	}

	internal TextAnchor getRTLAlignment(TextAnchor pTextAlignment)
	{
		return pTextAlignment switch
		{
			TextAnchor.UpperLeft => TextAnchor.UpperRight, 
			TextAnchor.UpperRight => TextAnchor.UpperLeft, 
			TextAnchor.MiddleLeft => TextAnchor.MiddleRight, 
			TextAnchor.MiddleRight => TextAnchor.MiddleLeft, 
			TextAnchor.LowerLeft => TextAnchor.LowerRight, 
			TextAnchor.LowerRight => TextAnchor.LowerLeft, 
			_ => pTextAlignment, 
		};
	}
}
