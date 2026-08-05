using System.Globalization;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CountUpOnClick : MonoBehaviour
{
	private const float TWEEN_DURATION = 0.45f;

	[SerializeField]
	private Text _text;

	private Tweener _cur_tween;

	private int _value;

	private string _end = "";

	private bool _value_updated;

	private void Start()
	{
		if (!TryGetComponent<Button>(out var tButton) || _text == null)
		{
			base.enabled = false;
		}
		else if (!_value_updated && !checkString())
		{
			base.enabled = false;
		}
		else
		{
			tButton.onClick.AddListener(countAnimation);
		}
	}

	public void setValue(int pValue, string pEnd = "")
	{
		base.enabled = true;
		_value = pValue;
		_end = pEnd;
		_value_updated = true;
		_text.text = _value.ToText(4) + pEnd;
	}

	private bool checkString()
	{
		string tTargetText = _text.text;
		if (!checkIfStringIsLegit(tTargetText))
		{
			return false;
		}
		if (!int.TryParse(tTargetText, NumberStyles.Any, CultureInfo.CurrentCulture, out _value))
		{
			base.enabled = false;
			return false;
		}
		return true;
	}

	private bool checkIfStringIsLegit(string pString)
	{
		if (string.IsNullOrEmpty(pString))
		{
			return false;
		}
		if (!pString.All(char.IsDigit))
		{
			return false;
		}
		return true;
	}

	private void countAnimation()
	{
		if (_value_updated)
		{
			_value_updated = false;
		}
		checkDestroyTween();
		_cur_tween = _text.DOUpCounter(0, _value, 0.45f, _end);
	}

	public Text getText()
	{
		return _text;
	}

	private void OnDisable()
	{
		checkDestroyTween();
	}

	private void checkDestroyTween()
	{
		_cur_tween.Kill(complete: true);
		_cur_tween = null;
	}
}
