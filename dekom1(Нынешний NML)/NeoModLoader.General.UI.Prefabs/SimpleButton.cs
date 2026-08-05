using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NeoModLoader.General.UI.Prefabs;

public class SimpleButton : APrefab<SimpleButton>
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private TipButton tipButton;

	[SerializeField]
	private Image background;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private Text text;

	public Button Button => button;

	public TipButton TipButton => tipButton;

	public Image Background => background;

	public Image Icon => icon;

	public Text Text => text;

	private void Awake()
	{
		if (!Initialized)
		{
			Init();
		}
	}

	public void Setup(UnityAction pClickAction, Sprite pIcon, string pText = null, Vector2 pSize = default(Vector2), string pTipType = null, TooltipData pTipData = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (pSize == default(Vector2))
		{
			((Vector2)(ref pSize))._002Ector(32f, 32f);
		}
		SetSize(pSize);
		if (string.IsNullOrEmpty(pText))
		{
			((Component)Text).gameObject.SetActive(false);
			((Component)Icon).gameObject.SetActive(true);
		}
		else
		{
			((Component)Icon).gameObject.SetActive(false);
			((Component)Text).gameObject.SetActive(true);
		}
		Icon.sprite = pIcon;
		Text.text = pText;
		((UnityEventBase)Button.onClick).RemoveAllListeners();
		((UnityEvent)Button.onClick).AddListener(pClickAction);
		if (string.IsNullOrEmpty(pTipType))
		{
			((Behaviour)TipButton).enabled = false;
			return;
		}
		((Behaviour)TipButton).enabled = true;
		TipButton.type = pTipType;
		if (string.IsNullOrEmpty(pTipData.tip_name))
		{
			TipButton.hoverAction = TipButton.showTooltipDefault;
			return;
		}
		TipButton.hoverAction = delegate
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			Tooltip.show(((Component)this).gameObject, TipButton.type, pTipData);
			((Component)this).transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
			ShortcutExtensions.DOKill((Component)(object)((Component)this).transform, false);
			TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(ShortcutExtensions.DOScale(((Component)this).transform, 1f, 0.1f), (Ease)26);
		};
	}

	public override void SetSize(Vector2 pSize)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).GetComponent<RectTransform>().sizeDelta = pSize;
		float num = Mathf.Min(pSize.x, pSize.y);
		((Component)Icon).GetComponent<RectTransform>().sizeDelta = new Vector2(num, num) * 0.875f;
		((Component)Text).GetComponent<RectTransform>().sizeDelta = pSize * 0.875f;
	}

	internal static void _init()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("SimpleButton", new Type[3]
		{
			typeof(Button),
			typeof(Image),
			typeof(TipButton)
		});
		val.transform.SetParent(WorldBoxMod.Transform);
		((Behaviour)val.GetComponent<TipButton>()).enabled = false;
		val.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/special_buttonRed");
		val.GetComponent<Image>().type = (Type)1;
		GameObject val2 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val2.transform.SetParent(val.transform);
		val2.transform.localPosition = Vector3.zero;
		val2.transform.localScale = Vector3.one;
		GameObject val3 = new GameObject("Text", new Type[1] { typeof(Text) });
		val3.transform.SetParent(val.transform);
		val3.transform.localPosition = Vector3.zero;
		val3.transform.localScale = Vector3.one;
		Text component = val3.GetComponent<Text>();
		component.font = LocalizedTextManager.current_font;
		((Graphic)component).color = Color.white;
		component.resizeTextForBestFit = true;
		component.resizeTextMinSize = 1;
		component.resizeTextMaxSize = 10;
		component.alignment = (TextAnchor)4;
		val3.SetActive(false);
		APrefab<SimpleButton>.Prefab = val.AddComponent<SimpleButton>();
		APrefab<SimpleButton>.Prefab.button = val.GetComponent<Button>();
		APrefab<SimpleButton>.Prefab.tipButton = val.GetComponent<TipButton>();
		APrefab<SimpleButton>.Prefab.background = val.GetComponent<Image>();
		APrefab<SimpleButton>.Prefab.icon = val2.GetComponent<Image>();
		APrefab<SimpleButton>.Prefab.text = component;
	}
}
