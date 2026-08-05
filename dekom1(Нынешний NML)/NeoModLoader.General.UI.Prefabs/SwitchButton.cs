using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NeoModLoader.General.UI.Prefabs;

public class SwitchButton : APrefab<SwitchButton>
{
	[SerializeField]
	private Button _button;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private Text _text;

	[SerializeField]
	private TipButton _tip_button;

	public Button button => _button;

	public Image icon => _icon;

	public Text text => _text;

	public TipButton tip_button => _tip_button;

	private void Awake()
	{
		if (!Initialized)
		{
			Init();
		}
	}

	public void Setup(bool value, Action value_update)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		if (!Initialized)
		{
			Init();
		}
		icon.sprite = (value ? SpriteTextureLoader.getSprite("ui/icons/iconOn") : SpriteTextureLoader.getSprite("ui/icons/iconOff"));
		text.text = (value ? LM.Get("short_on") : LM.Get("short_off"));
		((UnityEventBase)button.onClick).RemoveAllListeners();
		((UnityEvent)button.onClick).AddListener((UnityAction)delegate
		{
			value_update();
			Setup(!value, value_update);
		});
	}

	internal static void _init()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("SwitchButton", new Type[4]
		{
			typeof(Image),
			typeof(Button),
			typeof(TipButton),
			typeof(HorizontalLayoutGroup)
		});
		val.transform.SetParent(WorldBoxMod.Transform);
		val.transform.localScale = Vector3.one;
		val.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 18f);
		HorizontalLayoutGroup component = val.GetComponent<HorizontalLayoutGroup>();
		((HorizontalOrVerticalLayoutGroup)component).childControlWidth = false;
		((HorizontalOrVerticalLayoutGroup)component).childControlHeight = false;
		((LayoutGroup)component).childAlignment = (TextAnchor)4;
		val.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/special_buttonRed");
		val.GetComponent<Image>().type = (Type)1;
		GameObject val2 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val2.transform.SetParent(val.transform);
		val2.transform.localScale = Vector3.one;
		val2.GetComponent<RectTransform>().sizeDelta = new Vector2(18f, 18f);
		GameObject val3 = new GameObject("Text", new Type[1] { typeof(Text) });
		val3.transform.SetParent(val.transform);
		val3.transform.localScale = Vector3.one;
		val3.GetComponent<RectTransform>().sizeDelta = new Vector2(24f, 18f);
		Text component2 = val3.GetComponent<Text>();
		component2.resizeTextForBestFit = true;
		OT.InitializeCommonText(component2);
		component2.alignment = (TextAnchor)4;
		APrefab<SwitchButton>.Prefab = val.AddComponent<SwitchButton>();
		APrefab<SwitchButton>.Prefab._button = val.GetComponent<Button>();
		APrefab<SwitchButton>.Prefab._icon = val2.GetComponent<Image>();
		APrefab<SwitchButton>.Prefab._text = val3.GetComponent<Text>();
		APrefab<SwitchButton>.Prefab._tip_button = val.GetComponent<TipButton>();
	}
}
