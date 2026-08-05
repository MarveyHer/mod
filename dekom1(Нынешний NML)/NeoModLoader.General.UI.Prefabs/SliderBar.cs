using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NeoModLoader.General.UI.Prefabs;

public class SliderBar : APrefab<SliderBar>
{
	[SerializeField]
	private Slider _slider;

	[SerializeField]
	private TipButton _tip_button;

	public Slider slider => _slider;

	public TipButton tip_button => _tip_button;

	private void Awake()
	{
		if (!Initialized)
		{
			Init();
		}
	}

	public void Setup(float value, float min, float max, UnityAction<float> value_update, Vector2 size = default(Vector2), bool whole_numbers = false)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (!Initialized)
		{
			Init();
		}
		((UnityEventBase)slider.onValueChanged).RemoveAllListeners();
		slider.minValue = min;
		slider.maxValue = max;
		slider.value = value;
		slider.wholeNumbers = whole_numbers;
		((UnityEvent<float>)(object)slider.onValueChanged).AddListener(value_update);
		if (size != default(Vector2))
		{
			SetSize(size);
		}
	}

	public override void SetSize(Vector2 size)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		if (!Initialized)
		{
			Init();
		}
		((Component)this).GetComponent<RectTransform>().sizeDelta = size;
		((Component)((Component)this).transform.Find("Background")).GetComponent<RectTransform>().sizeDelta = size - new Vector2(0f, 10f);
		((Component)((Component)this).transform.Find("Fill Area")).GetComponent<RectTransform>().sizeDelta = size - new Vector2(0f, 10f);
		((Component)((Component)this).transform.Find("Fill Area/Fill")).GetComponent<RectTransform>().sizeDelta = Vector2.zero;
		((Component)((Component)this).transform.Find("Handle Slide Area")).GetComponent<RectTransform>().sizeDelta = size - new Vector2(10f, 0f);
		((Component)((Component)this).transform.Find("Handle Slide Area/Handle")).GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 0f);
	}

	internal static void _init()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Expected O, but got Unknown
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Expected O, but got Unknown
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Expected O, but got Unknown
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("SliderBar", new Type[2]
		{
			typeof(Slider),
			typeof(TipButton)
		});
		val.transform.SetParent(WorldBoxMod.Transform);
		val.GetComponent<RectTransform>().sizeDelta = new Vector2(172f, 20f);
		GameObject val2 = new GameObject("Background", new Type[1] { typeof(Image) });
		val2.transform.SetParent(val.transform);
		val2.transform.localScale = Vector3.one;
		val2.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
		val2.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/special_buttonGray");
		val2.GetComponent<Image>().type = (Type)1;
		GameObject val3 = new GameObject("Fill Area", new Type[1] { typeof(RectTransform) });
		val3.transform.SetParent(val.transform);
		val3.transform.localScale = Vector3.one;
		val3.GetComponent<RectTransform>().sizeDelta = new Vector2(-20f, 0f);
		GameObject val4 = new GameObject("Fill", new Type[1] { typeof(Image) });
		val4.transform.SetParent(val3.transform);
		val4.transform.localScale = Vector3.one;
		val4.GetComponent<RectTransform>().sizeDelta = new Vector2(10f, 0f);
		val4.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/special_buttonRed");
		val4.GetComponent<Image>().type = (Type)1;
		GameObject val5 = new GameObject("Handle Slide Area", new Type[1] { typeof(RectTransform) });
		val5.transform.SetParent(val.transform);
		val5.transform.localScale = Vector3.one;
		val5.GetComponent<RectTransform>().sizeDelta = new Vector2(-20f, 0f);
		GameObject val6 = new GameObject("Handle", new Type[1] { typeof(Image) });
		val6.transform.SetParent(val5.transform);
		val6.transform.localScale = Vector3.one;
		val6.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/special_buttonRed");
		val6.GetComponent<Image>().type = (Type)1;
		val6.GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 0f);
		APrefab<SliderBar>.Prefab = val.AddComponent<SliderBar>();
		Slider component = val.GetComponent<Slider>();
		component.fillRect = val4.GetComponent<RectTransform>();
		component.handleRect = val6.GetComponent<RectTransform>();
		((Selectable)component).targetGraphic = (Graphic)(object)val6.GetComponent<Image>();
		component.direction = (Direction)0;
		((Selectable)component).interactable = true;
		APrefab<SliderBar>.Prefab._slider = component;
		APrefab<SliderBar>.Prefab._tip_button = val.GetComponent<TipButton>();
	}
}
