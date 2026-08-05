using System;
using NeoModLoader.api;
using NeoModLoader.General;
using UnityEngine;
using UnityEngine.UI;

namespace NeoModLoader.ui;

internal class ModUploadingProgressWindow : AbstractWindow<ModUploadingProgressWindow>
{
	public class UploadProgress : IProgress<float>
	{
		public void Report(float value)
		{
			AbstractWindow<ModUploadingProgressWindow>.Instance.real_progress = value;
			if (!(AbstractWindow<ModUploadingProgressWindow>.Instance.progress >= value))
			{
				AbstractWindow<ModUploadingProgressWindow>.Instance.progress = value;
			}
		}

		public void Reset()
		{
			AbstractWindow<ModUploadingProgressWindow>.Instance.progress = 0f;
			AbstractWindow<ModUploadingProgressWindow>.Instance.real_progress = 0f;
		}
	}

	private Image bar;

	internal ulong fileId;

	private Text percent;

	private float progress = 0f;

	private float real_progress = 0f;

	private float start_time;

	private bool uploading = false;

	private UploadProgress uploadProgress = new UploadProgress();

	private void Update()
	{
		if (Initialized && IsOpened && uploading)
		{
			if (progress < 0.9f)
			{
				progress += Math.Max(0f, real_progress / (Time.time - start_time) * Time.deltaTime);
			}
			else
			{
				progress = Math.Max(progress, Mathf.Lerp(progress, real_progress, Time.deltaTime * 0.1f));
			}
			UpdateDisplay();
		}
	}

	protected override void Init()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Expected O, but got Unknown
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		percent = new GameObject("Percent", new Type[1] { typeof(Text) }).GetComponent<Text>();
		RectTransform component = ((Component)percent).GetComponent<RectTransform>();
		((Transform)component).SetParent(base.ContentTransform);
		((Transform)component).localScale = Vector3.one;
		((Transform)component).localPosition = new Vector3(130f, -100f);
		component.sizeDelta = new Vector2(180f, 30f);
		OT.InitializeCommonText(percent);
		percent.alignment = (TextAnchor)4;
		percent.resizeTextMaxSize = 14;
		percent.resizeTextMinSize = 6;
		percent.resizeTextForBestFit = true;
		Image component2 = new GameObject("Bar", new Type[2]
		{
			typeof(Image),
			typeof(Mask)
		}).GetComponent<Image>();
		component2.sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
		component2.type = (Type)1;
		((Graphic)component2).color = Color.gray;
		RectTransform val = (RectTransform)((Component)component2).transform;
		RectTransform val2 = val;
		((Transform)val).SetParent(base.ContentTransform);
		((Transform)val2).localScale = Vector3.one;
		((Transform)val2).localPosition = new Vector3(130f, -123f);
		val2.sizeDelta = new Vector2(190f, 20f);
		bar = new GameObject("Image", new Type[1] { typeof(Image) }).GetComponent<Image>();
		RectTransform val3 = (RectTransform)((Component)bar).transform;
		RectTransform val4 = val3;
		((Transform)val3).SetParent((Transform)(object)val2);
		((Transform)val4).localScale = Vector3.one;
		val4.sizeDelta = new Vector2(190f, 20f);
		((Transform)val4).localPosition = new Vector3((0f - val4.sizeDelta.x) / 2f, 0f);
		val4.pivot = new Vector2(0f, 0.5f);
		((Graphic)bar).color = Color.green;
	}

	public static UploadProgress ShowWindow()
	{
		AbstractWindow<ModUploadingProgressWindow>.Instance.uploading = true;
		AbstractWindow<ModUploadingProgressWindow>.Instance.uploadProgress.Reset();
		ScrollWindow.showWindow(AbstractWindow<ModUploadingProgressWindow>.WindowId);
		AbstractWindow<ModUploadingProgressWindow>.Instance.start_time = Time.time;
		return AbstractWindow<ModUploadingProgressWindow>.Instance.uploadProgress;
	}

	public override void OnNormalEnable()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		base.OnNormalEnable();
		progress = 0f;
		fileId = 0uL;
		((Graphic)percent).color = Color.white;
		uploadProgress.Reset();
	}

	public override void OnNormalDisable()
	{
		base.OnNormalDisable();
		uploading = false;
	}

	private void UpdateDisplay()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		((Component)bar).transform.localScale = new Vector3(progress, 1f, 1f);
		percent.text = $"{(int)(progress * 100f)}%";
	}

	public static void FinishUpload()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		AbstractWindow<ModUploadingProgressWindow>.Instance.uploading = false;
		AbstractWindow<ModUploadingProgressWindow>.Instance.progress = 1f;
		AbstractWindow<ModUploadingProgressWindow>.Instance.UpdateDisplay();
		AbstractWindow<ModUploadingProgressWindow>.Instance.percent.text = LM.Get("ModUploadFinish");
		((Graphic)AbstractWindow<ModUploadingProgressWindow>.Instance.percent).color = Color.green;
		if (AbstractWindow<ModUploadingProgressWindow>.Instance.fileId != 0)
		{
			Application.OpenURL("steam://url/CommunityFilePage/" + AbstractWindow<ModUploadingProgressWindow>.Instance.fileId);
		}
	}

	public static void ErrorUpload(Exception obj)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		AbstractWindow<ModUploadingProgressWindow>.Instance.uploading = false;
		AbstractWindow<ModUploadingProgressWindow>.Instance.percent.text = LM.Get("NML_" + obj.Message);
		((Graphic)AbstractWindow<ModUploadingProgressWindow>.Instance.percent).color = Color.red;
	}
}
