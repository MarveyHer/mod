using System;
using UnityEngine;
using UnityEngine.UI;

namespace NeoModLoader.General.UI.Prefabs;

public class SimpleStatBar : APrefab<SimpleStatBar>
{
	[SerializeField]
	private Image _background;

	[SerializeField]
	private Image _bar;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private StatBar _stat_bar;

	public Image background => _background;

	public Image bar => _bar;

	public Image icon => _icon;

	public StatBar stat_bar => _stat_bar;

	public virtual void Setup(float value, float max_value, string pEndText, Sprite pIcon, Sprite pBackground, Color pBarColor, Vector2 pSize, bool pReset = true, bool pFloat = false, bool pUpdateText = true, float pSpeed = 0.3f)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		if (!Initialized)
		{
			Init();
		}
		icon.sprite = pIcon;
		background.sprite = pBackground;
		if ((Object)(object)pBackground == (Object)null)
		{
			((Behaviour)background).enabled = false;
		}
		else
		{
			((Behaviour)background).enabled = true;
		}
		((Component)this).GetComponent<RectTransform>().sizeDelta = pSize;
		Vector2 val = pSize - new Vector2(pSize.y + 4f, pSize.y * 0.3f);
		((Component)((Component)this).transform.Find("Background")).GetComponent<RectTransform>().sizeDelta = val;
		((Component)this).transform.Find("Background").localPosition = new Vector3((pSize.x - val.x) / 2f - pSize.x * 0.02f, 0f);
		((Component)((Component)this).transform.Find("Mask")).GetComponent<RectTransform>().sizeDelta = val;
		((Component)this).transform.Find("Mask").localPosition = new Vector3((pSize.x - val.x) / 2f - pSize.x * 0.02f - val.x / 2f, 0f);
		((Component)bar).GetComponent<RectTransform>().sizeDelta = val;
		((Component)bar).transform.localPosition = new Vector3(val.x / 2f, 0f);
		((Component)icon).transform.localPosition = new Vector3((0f - pSize.x) / 2f + pSize.y / 2f, 0f, 0f);
		((Component)icon).GetComponent<RectTransform>().sizeDelta = new Vector2(pSize.y, pSize.y);
		((Component)((Component)this).transform.Find("Text")).GetComponent<RectTransform>().sizeDelta = new Vector2(val.x, val.y);
		((Component)this).transform.Find("Text").localPosition = new Vector3((pSize.x - val.x) / 2f - pSize.x * 0.02f, 0f);
		UpdateBar(value, max_value, pEndText, pBarColor, pReset, pFloat, pUpdateText, pSpeed);
	}

	public void UpdateBar(float value, float max_value, string pEndText, Color pBarColor = default(Color), bool pReset = true, bool pFloat = false, bool pUpdateText = true, float pSpeed = 0.3f)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!Initialized)
		{
			Init();
		}
		if (pBarColor != default(Color))
		{
			((Graphic)bar).color = pBarColor;
		}
		stat_bar.setBar(value, max_value, pEndText, pReset, pFloat, pUpdateText, pSpeed);
	}

	internal static void _init()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Expected O, but got Unknown
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Expected O, but got Unknown
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Expected O, but got Unknown
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("SimpleStatBar", new Type[3]
		{
			typeof(Button),
			typeof(TipButton),
			typeof(Image)
		});
		val.transform.SetParent(WorldBoxMod.Transform);
		val.transform.localScale = Vector3.one;
		val.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 14f);
		val.GetComponent<Image>().type = (Type)1;
		GameObject val2 = new GameObject("Background", new Type[1] { typeof(Image) });
		val2.transform.SetParent(val.transform);
		Image component = val2.GetComponent<Image>();
		component.sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
		component.type = (Type)1;
		((Graphic)component).color = new Color(0.49f, 0.49f, 0.49f);
		GameObject val3 = new GameObject("Mask", new Type[2]
		{
			typeof(Image),
			typeof(Mask)
		});
		val3.transform.SetParent(val.transform);
		Mask component2 = val3.GetComponent<Mask>();
		component2.showMaskGraphic = false;
		val3.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
		val3.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 0.5f);
		val3.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0.5f);
		GameObject val4 = new GameObject("Bar", new Type[1] { typeof(Image) });
		val4.transform.SetParent(val3.transform);
		Image component3 = val4.GetComponent<Image>();
		component3.sprite = SpriteTextureLoader.getSprite("ui/special/windowBar");
		component3.type = (Type)1;
		val4.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 0.5f);
		val4.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0.5f);
		GameObject val5 = new GameObject("Icon", new Type[2]
		{
			typeof(Image),
			typeof(Shadow)
		});
		val5.transform.SetParent(val.transform);
		Image component4 = val5.GetComponent<Image>();
		component4.sprite = SpriteTextureLoader.getSprite("ui/icons/iconHealth");
		GameObject val6 = new GameObject("Text", new Type[2]
		{
			typeof(Text),
			typeof(Shadow)
		});
		val6.transform.SetParent(val.transform);
		Text component5 = val6.GetComponent<Text>();
		component5.text = "0/0";
		component5.resizeTextForBestFit = true;
		component5.resizeTextMaxSize = 10;
		component5.resizeTextMinSize = 1;
		component5.alignment = (TextAnchor)1;
		((Graphic)component5).color = Color.white;
		component5.font = LocalizedTextManager.current_font;
		val.SetActive(false);
		StatBar statBar = val.AddComponent<StatBar>();
		statBar.textField = component5;
		statBar.mask = val3.GetComponent<RectTransform>();
		statBar.bar = val2.GetComponent<RectTransform>();
		val.SetActive(true);
		APrefab<SimpleStatBar>.Prefab = val.AddComponent<SimpleStatBar>();
		APrefab<SimpleStatBar>.Prefab._background = component;
		APrefab<SimpleStatBar>.Prefab._bar = component3;
		APrefab<SimpleStatBar>.Prefab._icon = component4;
		APrefab<SimpleStatBar>.Prefab._stat_bar = statBar;
	}
}
