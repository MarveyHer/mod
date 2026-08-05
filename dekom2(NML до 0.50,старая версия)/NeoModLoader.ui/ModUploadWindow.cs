using System;
using System.IO;
using System.Linq;
using NeoModLoader.api;
using NeoModLoader.General;
using NeoModLoader.services;
using NeoModLoader.utils;
using RSG;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NeoModLoader.ui;

internal class ModUploadWindow : AbstractWindow<ModUploadWindow>
{
	private Text changelog_text;

	private Text mod_author_text;

	private Text mod_description_text;

	private Text mod_fileid_text;

	private Image mod_icon_image;

	private Text mod_name_text;

	private Text mod_version_text;

	private IMod selected_mod;

	public static void ShowWindow(IMod mod)
	{
		AbstractWindow<ModUploadWindow>.Instance.selected_mod = mod;
		ModDeclare declaration = mod.GetDeclaration();
		if (string.IsNullOrEmpty(declaration.IconPath))
		{
			AbstractWindow<ModUploadWindow>.Instance.mod_icon_image.sprite = InternalResourcesGetter.GetIcon();
		}
		else
		{
			AbstractWindow<ModUploadWindow>.Instance.mod_icon_image.sprite = SpriteLoadUtils.LoadSingleSprite(Path.Combine(declaration.FolderPath, declaration.IconPath));
		}
		AbstractWindow<ModUploadWindow>.Instance.mod_name_text.text = declaration.Name;
		AbstractWindow<ModUploadWindow>.Instance.mod_author_text.text = declaration.Author;
		AbstractWindow<ModUploadWindow>.Instance.mod_version_text.text = declaration.Version;
		AbstractWindow<ModUploadWindow>.Instance.mod_description_text.text = declaration.Description;
		ScrollWindow.showWindow(AbstractWindow<ModUploadWindow>.WindowId);
	}

	protected override void Init()
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Expected O, but got Unknown
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Expected O, but got Unknown
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Expected O, but got Unknown
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Expected O, but got Unknown
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Expected O, but got Unknown
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Expected O, but got Unknown
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Expected O, but got Unknown
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Expected O, but got Unknown
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_0739: Expected O, but got Unknown
		//IL_0754: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_0849: Unknown result type (might be due to invalid IL or missing references)
		//IL_0858: Unknown result type (might be due to invalid IL or missing references)
		//IL_085d: Unknown result type (might be due to invalid IL or missing references)
		//IL_088d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0894: Expected O, but got Unknown
		//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_092d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0934: Expected O, but got Unknown
		//IL_0959: Unknown result type (might be due to invalid IL or missing references)
		//IL_096b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0987: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c8: Expected O, but got Unknown
		//IL_09ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a69: Expected O, but got Unknown
		//IL_0a84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b09: Expected O, but got Unknown
		((Component)base.ContentTransform).gameObject.AddComponent<ContentSizeFitter>().verticalFit = (FitMode)2;
		VerticalLayoutGroup val = ((Component)base.ContentTransform).gameObject.AddComponent<VerticalLayoutGroup>();
		((LayoutGroup)val).childAlignment = (TextAnchor)1;
		((HorizontalOrVerticalLayoutGroup)val).childControlHeight = false;
		((HorizontalOrVerticalLayoutGroup)val).childControlWidth = false;
		((HorizontalOrVerticalLayoutGroup)val).childForceExpandHeight = false;
		((HorizontalOrVerticalLayoutGroup)val).childForceExpandWidth = false;
		((HorizontalOrVerticalLayoutGroup)val).childScaleHeight = false;
		((HorizontalOrVerticalLayoutGroup)val).childScaleWidth = false;
		((HorizontalOrVerticalLayoutGroup)val).spacing = 10f;
		((LayoutGroup)val).padding = new RectOffset(0, 0, 5, 0);
		GameObject val2 = new GameObject("TopBar", new Type[1] { typeof(RectTransform) });
		val2.transform.SetParent(base.ContentTransform);
		val2.transform.localScale = Vector3.one;
		val2.GetComponent<RectTransform>().sizeDelta = new Vector2(190f, 17f);
		GameObject val3 = new GameObject("DescIcon", new Type[1] { typeof(Image) });
		val3.transform.SetParent(val2.transform);
		val3.transform.localPosition = new Vector3(-90f, 0f);
		val3.transform.localScale = Vector3.one;
		val3.GetComponent<RectTransform>().sizeDelta = new Vector2(15f, 15f);
		val3.GetComponent<Image>().sprite = InternalResourcesGetter.GetIcon();
		GameObject val4 = new GameObject("Input FileId", new Type[1] { typeof(Image) });
		val4.transform.SetParent(val2.transform);
		val4.transform.localScale = Vector3.one;
		val4.transform.localPosition = new Vector3(5f, 0f);
		Image component = val4.GetComponent<Image>();
		component.sprite = SpriteTextureLoader.getSprite("ui/special/darkInputFieldEmpty");
		component.type = (Type)1;
		GameObject val5 = new GameObject("InputField", new Type[2]
		{
			typeof(Text),
			typeof(InputField)
		});
		val5.transform.SetParent(val4.transform);
		val5.transform.localPosition = Vector3.zero;
		val5.transform.localScale = Vector3.one;
		Text component2 = val5.GetComponent<Text>();
		val5.GetComponent<InputField>().textComponent = component2;
		component2.text = "";
		mod_fileid_text = component2;
		OT.InitializeCommonText(component2);
		component2.alignment = (TextAnchor)3;
		component2.resizeTextForBestFit = true;
		component2.resizeTextMinSize = 6;
		GameObject val6 = new GameObject("Image", new Type[1] { typeof(Image) });
		val6.transform.SetParent(val4.transform);
		val6.transform.localPosition = new Vector3(77f, 0f);
		val6.transform.localScale = Vector3.one;
		val6.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/inputFieldIcon");
		val6.GetComponent<RectTransform>().sizeDelta = new Vector2(15f, 15f);
		NameInput nameInput = val4.AddComponent<NameInput>();
		nameInput.inputField = val5.GetComponent<InputField>();
		nameInput.textField = component2;
		nameInput.addListener(delegate
		{
		});
		RectTransform component3 = val5.GetComponent<RectTransform>();
		component3.sizeDelta = new Vector2(170f, 15f);
		val4.GetComponent<RectTransform>().sizeDelta = component3.sizeDelta + new Vector2(2f, 2f);
		GameObject val7 = new GameObject("ModInfo", new Type[1] { typeof(Image) });
		val7.transform.SetParent(base.ContentTransform);
		val7.transform.localPosition = new Vector3(130f, -78f, 0f);
		val7.transform.localScale = Vector3.one;
		val7.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
		val7.GetComponent<Image>().type = (Type)1;
		val7.GetComponent<RectTransform>().sizeDelta = new Vector2(190f, 95f);
		GameObject val8 = new GameObject("ModIcon", new Type[1] { typeof(Image) });
		val8.transform.SetParent(val7.transform);
		val8.transform.localScale = Vector3.one;
		val8.transform.localPosition = new Vector3(-48f, 0f);
		val8.GetComponent<RectTransform>().sizeDelta = new Vector2(90f, 90f);
		mod_icon_image = val8.GetComponent<Image>();
		GameObject val9 = new GameObject("ModIconFrame", new Type[1] { typeof(Image) });
		val9.transform.SetParent(val8.transform);
		val9.GetComponent<Image>().sprite = InternalResourcesGetter.GetIconFrame();
		val9.GetComponent<Image>().type = (Type)1;
		val9.GetComponent<RectTransform>().sizeDelta = val8.GetComponent<RectTransform>().sizeDelta;
		GameObject info_grids = new GameObject("InfoGrids", new Type[1] { typeof(GridLayoutGroup) });
		info_grids.transform.SetParent(val7.transform);
		info_grids.transform.localScale = Vector3.one;
		info_grids.transform.localPosition = new Vector3(48f, 0f);
		info_grids.GetComponent<RectTransform>().sizeDelta = new Vector2(92f, 92f);
		GridLayoutGroup component4 = info_grids.GetComponent<GridLayoutGroup>();
		((LayoutGroup)component4).childAlignment = (TextAnchor)1;
		component4.constraint = (Constraint)1;
		component4.constraintCount = 1;
		component4.spacing = new Vector2(0f, 1f);
		component4.cellSize = new Vector2(92f, 15f);
		mod_name_text = create_grid_text("Mod Name");
		mod_author_text = create_grid_text("Mod Author");
		mod_version_text = create_grid_text("Mod Version");
		mod_description_text = create_grid_text("Mod Description");
		GameObject val10 = new GameObject("Input ChangeLog", new Type[1] { typeof(Image) });
		val10.transform.SetParent(base.ContentTransform);
		val10.transform.localScale = Vector3.one;
		val10.transform.localPosition = new Vector3(130f, -170f);
		Image component5 = val10.GetComponent<Image>();
		component5.sprite = SpriteTextureLoader.getSprite("ui/special/darkInputFieldEmpty");
		component5.type = (Type)1;
		GameObject val11 = new GameObject("InputField", new Type[2]
		{
			typeof(Text),
			typeof(InputField)
		});
		val11.transform.SetParent(val10.transform);
		val11.transform.localScale = Vector3.one;
		val11.transform.localPosition = Vector3.zero;
		Text component6 = val11.GetComponent<Text>();
		val11.GetComponent<InputField>().textComponent = component6;
		component6.text = "#CHANGELOG";
		changelog_text = component6;
		OT.InitializeCommonText(component6);
		component6.alignment = (TextAnchor)0;
		component6.resizeTextForBestFit = true;
		component6.resizeTextMinSize = 6;
		component6.resizeTextMaxSize = 10;
		val11.GetComponent<InputField>().lineType = (LineType)2;
		NameInput nameInput2 = val10.AddComponent<NameInput>();
		nameInput2.inputField = val11.GetComponent<InputField>();
		nameInput2.textField = component6;
		nameInput2.addListener(delegate
		{
		});
		RectTransform component7 = val11.GetComponent<RectTransform>();
		component7.sizeDelta = new Vector2(190f, 80f);
		val10.GetComponent<RectTransform>().sizeDelta = component7.sizeDelta + new Vector2(2f, 2f);
		GameObject val12 = new GameObject("UploadButton", new Type[2]
		{
			typeof(Image),
			typeof(Button)
		});
		val12.transform.SetParent(base.ContentTransform);
		val12.transform.localPosition = new Vector3(130f, -260f);
		val12.transform.localScale = Vector3.one;
		val12.GetComponent<RectTransform>().sizeDelta = new Vector2(190f, 30f);
		Image component8 = val12.GetComponent<Image>();
		component8.sprite = SpriteTextureLoader.getSprite("ui/special/special_buttonred");
		component8.type = (Type)1;
		GameObject val13 = new GameObject("Desc1", new Type[1] { typeof(Image) });
		val13.transform.SetParent(val12.transform);
		val13.transform.localPosition = new Vector3(-80f, 0f);
		val13.transform.localScale = Vector3.one;
		val13.GetComponent<RectTransform>().sizeDelta = new Vector2(30f, 30f);
		val13.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/icons/iconSaveCloud");
		GameObject val14 = new GameObject("Desc2", new Type[1] { typeof(Image) });
		val14.transform.SetParent(val12.transform);
		val14.transform.localPosition = new Vector3(80f, 0f);
		val14.transform.localScale = Vector3.one;
		val14.GetComponent<RectTransform>().sizeDelta = new Vector2(30f, 30f);
		val14.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/icons/iconSteam");
		GameObject val15 = new GameObject("Text", new Type[2]
		{
			typeof(Text),
			typeof(LocalizedText)
		});
		val15.transform.SetParent(val12.transform);
		val15.transform.localPosition = Vector3.zero;
		val15.transform.localScale = Vector3.one;
		val15.GetComponent<RectTransform>().sizeDelta = new Vector2(190f, 30f);
		Text component9 = val15.GetComponent<Text>();
		OT.InitializeCommonText(component9);
		component9.alignment = (TextAnchor)4;
		LocalizedText component10 = val15.GetComponent<LocalizedText>();
		component10.key = "ModUpload Title";
		((UnityEvent)val12.GetComponent<Button>().onClick).AddListener(new UnityAction(uploadSelectedMod));
		LocalizedTextManager.addTextField(component10);
		Text create_grid_text(string name)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			Text component11 = new GameObject(name, new Type[1] { typeof(Text) }).GetComponent<Text>();
			Transform transform;
			(transform = ((Component)component11).transform).SetParent(info_grids.transform);
			transform.localScale = Vector3.one;
			OT.InitializeCommonText(component11);
			component11.resizeTextForBestFit = true;
			component11.resizeTextMaxSize = 10;
			component11.resizeTextMinSize = 6;
			component11.text = name;
			component11.alignment = (TextAnchor)3;
			return component11;
		}
	}

	private void uploadSelectedMod()
	{
		string text = mod_fileid_text.text;
		if (text.Any((char c) => !char.IsDigit(c)))
		{
			text = null;
		}
		if (string.IsNullOrEmpty(text))
		{
			ModUploadAuthenticationService.Authenticate().Then((Func<IPromise>)(() => (IPromise)(object)ModWorkshopService.UploadMod(selected_mod, changelog_text.text, ModUploadAuthenticationService.Authed))).Then((Action)ModUploadingProgressWindow.FinishUpload, (Action<Exception>)ModUploadingProgressWindow.ErrorUpload);
		}
		else
		{
			ulong fileID = ulong.Parse(text);
			ModWorkshopService.TryEditMod(fileID, selected_mod, changelog_text.text).Then((Action)ModUploadingProgressWindow.FinishUpload, (Action<Exception>)ModUploadingProgressWindow.ErrorUpload).Done();
		}
	}
}
