using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NameInput : MonoBehaviour
{
	public InputField inputField;

	public Text textField;

	private string LastValue;

	public bool can_be_empty;

	public bool is_onomastics;

	private Outline _outline;

	private void Start()
	{
		textField.horizontalOverflow = HorizontalWrapMode.Wrap;
		if (is_onomastics)
		{
			inputField.onValidateInput = validateOnomastics;
		}
		else
		{
			inputField.onValidateInput = validate;
		}
	}

	private char validate(string pText, int pCharIndex, char pAddedChar)
	{
		if (pAddedChar == '<' || pAddedChar == '>')
		{
			return '\0';
		}
		return pAddedChar;
	}

	private char validateOnomastics(string pText, int pCharIndex, char pAddedChar)
	{
		char tResult = pAddedChar;
		bool tIsLetter = char.IsLetter(tResult);
		bool tIsSpace = char.IsWhiteSpace(tResult);
		bool tIsApostrophe = tResult == '\'';
		bool tIsFirstLetter = pText.Length == 0;
		if (!(tIsLetter || tIsSpace || tIsApostrophe))
		{
			return '\0';
		}
		if (tIsFirstLetter)
		{
			return char.ToUpper(tResult);
		}
		char num = pText[pText.Length - 1];
		bool tIsLetterPrevious = char.IsLetter(num);
		bool tIsSpacePrevious = char.IsWhiteSpace(num);
		bool tIsApostrophePrevious = num == '\'';
		if (tIsLetter)
		{
			if (tIsLetterPrevious)
			{
				tResult = char.ToLower(tResult);
			}
			else if (tIsSpacePrevious)
			{
				tResult = char.ToUpper(tResult);
			}
		}
		else if (tIsSpace)
		{
			if (tIsSpacePrevious)
			{
				return '\0';
			}
		}
		else if (tIsApostrophe && tIsApostrophePrevious)
		{
			return '\0';
		}
		return tResult;
	}

	public void addListener(UnityAction<string> pAction)
	{
		inputField.onValueChanged.AddListener(pAction);
	}

	private void OnEnable()
	{
		inputField.onEndEdit.AddListener(checkInput);
	}

	private void OnDisable()
	{
		inputField.onEndEdit.RemoveAllListeners();
		if (_outline != null)
		{
			_outline.enabled = false;
		}
	}

	public void SetOutline()
	{
		if (_outline == null)
		{
			_outline = inputField.gameObject.AddOrGetComponent<Outline>();
		}
		_outline.enabled = true;
		Color tTextColor = textField.color;
		Color tGlowColor = new Color(tTextColor.r, tTextColor.g, tTextColor.b, 0.2f);
		_outline.effectColor = tGlowColor;
	}

	private void checkInput(string pInput)
	{
		if (string.IsNullOrWhiteSpace(pInput) && !can_be_empty)
		{
			inputField.text = LastValue;
		}
		else
		{
			LastValue = pInput;
		}
	}

	public void setText(string pText)
	{
		textField.text = pText;
		inputField.text = pText;
		LastValue = pText;
	}
}
