using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NeoModLoader.General.UI.Prefabs;

public class TextInput : APrefab<TextInput>
{
	[SerializeField]
	private Image _icon;

	[SerializeField]
	private InputField _input;

	[SerializeField]
	private Text _text;

	[SerializeField]
	private TipButton _tip_button;

	public Image icon => _icon;

	public InputField input => _input;

	public Text text => _text;

	public TipButton tip_button => _tip_button;

	private void Awake()
	{
		if (!Initialized)
		{
			Init();
		}
	}

	public virtual void Setup(string value, UnityAction<string> value_update, Sprite pIcon = null, Sprite pBackground = null)
	{
		if (!Initialized)
		{
			Init();
		}
		((UnityEventBase)input.onEndEdit).RemoveAllListeners();
		input.text = value;
		((UnityEvent<string>)(object)input.onEndEdit).AddListener(value_update);
		if ((Object)(object)pIcon == (Object)null)
		{
			icon.sprite = SpriteTextureLoader.getSprite("ui/special/inputFieldIcon");
		}
		else
		{
			icon.sprite = pIcon;
		}
		if ((Object)(object)pBackground == (Object)null)
		{
			((Component)this).GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/darkInputFieldEmpty");
		}
		else
		{
			((Component)this).GetComponent<Image>().sprite = pBackground;
		}
	}

	public override void SetSize(Vector2 size)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (!Initialized)
		{
			Init();
		}
		((Component)this).GetComponent<RectTransform>().sizeDelta = size;
		((Component)text).GetComponent<RectTransform>().sizeDelta = size - new Vector2(size.y / 2f + 4f, 2f);
		((Component)icon).GetComponent<RectTransform>().sizeDelta = new Vector2(size.y, size.y) - new Vector2(2f, 2f);
		((Component)text).transform.localPosition = new Vector3((0f - size.x) / 2f, 0f);
		((Component)icon).transform.localPosition = new Vector3((size.x - size.y / 2f) / 2f, 0f);
	}

	internal static void _init()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("TextInput", new Type[2]
		{
			typeof(TipButton),
			typeof(Image)
		});
		val.transform.SetParent(WorldBoxMod.Transform);
		Image component = val.GetComponent<Image>();
		component.sprite = SpriteTextureLoader.getSprite("ui/special/darkInputFieldEmpty");
		component.type = (Type)1;
		GameObject val2 = new GameObject("InputField", new Type[2]
		{
			typeof(Text),
			typeof(InputField)
		});
		val2.transform.SetParent(val.transform);
		val2.transform.localScale = Vector3.one;
		val2.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
		Text component2 = val2.GetComponent<Text>();
		OT.InitializeCommonText(component2);
		component2.alignment = (TextAnchor)3;
		component2.resizeTextForBestFit = true;
		InputField component3 = val2.GetComponent<InputField>();
		component3.textComponent = component2;
		component3.text = "";
		component3.lineType = (LineType)0;
		GameObject val3 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val3.transform.SetParent(val.transform);
		val3.transform.localScale = Vector3.one;
		val3.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/inputFieldIcon");
		APrefab<TextInput>.Prefab = val.AddComponent<TextInput>();
		APrefab<TextInput>.Prefab._icon = val3.GetComponent<Image>();
		APrefab<TextInput>.Prefab._input = component3;
		APrefab<TextInput>.Prefab._text = component2;
		APrefab<TextInput>.Prefab._tip_button = val.GetComponent<TipButton>();
	}
}
