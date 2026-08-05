using System;
using System.Collections.Generic;
using System.Threading;
using NeoModLoader.api;
using NeoModLoader.General;
using NeoModLoader.services;
using NeoModLoader.utils;
using NeoModLoader.utils.authentication;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NeoModLoader.ui;

internal class ModUploadAuthenticationWindow : AbstractWindow<ModUploadAuthenticationWindow>
{
	private static Button prefab_auth_button;

	internal static List<Func<bool>> all_auto_auth_funcs = new List<Func<bool>>
	{
		delegate
		{
			while (true)
			{
				if (!string.IsNullOrEmpty(Config.discordId))
				{
					return DiscordAutomaticRoleAuthUtils.Authenticate();
				}
				if (DiscordTracker._user_tries <= 0)
				{
					break;
				}
				Thread.Sleep(10000);
			}
			return false;
		}
	};

	private Transform auth_grid_transform;

	private Text auth_text;

	internal Func<bool> AuthFunc;

	internal bool AuthFuncSelected = false;

	internal bool AuthSkipped;

	private LocalizedText localized_auth_text;

	protected override void Init()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Expected O, but got Unknown
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Expected O, but got Unknown
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		VerticalLayoutGroup val = ((Component)base.ContentTransform).gameObject.AddComponent<VerticalLayoutGroup>();
		((LayoutGroup)val).childAlignment = (TextAnchor)1;
		((HorizontalOrVerticalLayoutGroup)val).childControlHeight = false;
		((HorizontalOrVerticalLayoutGroup)val).childControlWidth = false;
		((HorizontalOrVerticalLayoutGroup)val).childForceExpandHeight = false;
		((HorizontalOrVerticalLayoutGroup)val).childForceExpandWidth = false;
		((HorizontalOrVerticalLayoutGroup)val).spacing = 5f;
		((LayoutGroup)val).padding = new RectOffset(5, 5, 5, 5);
		GameObject val2 = new GameObject("AuthText", new Type[2]
		{
			typeof(Text),
			typeof(LocalizedText)
		});
		val2.transform.SetParent(base.ContentTransform);
		val2.transform.localScale = Vector3.one;
		val2.GetComponent<RectTransform>().sizeDelta = new Vector2(190f, 50f);
		auth_text = val2.GetComponent<Text>();
		OT.InitializeCommonText(auth_text);
		auth_text.alignment = (TextAnchor)4;
		auth_text.resizeTextForBestFit = true;
		auth_text.resizeTextMinSize = 6;
		auth_text.resizeTextMaxSize = 14;
		((Graphic)auth_text).color = Color.white;
		localized_auth_text = val2.GetComponent<LocalizedText>();
		localized_auth_text.setKeyAndUpdate("NML_AUTHENTICATION");
		LocalizedTextManager.addTextField(localized_auth_text);
		GameObject val3 = new GameObject("AuthGrid", new Type[1] { typeof(GridLayoutGroup) });
		val3.transform.SetParent(base.ContentTransform);
		val3.transform.localScale = Vector3.one;
		val3.GetComponent<RectTransform>().sizeDelta = new Vector2(200f, 100f);
		auth_grid_transform = val3.transform;
		GridLayoutGroup component = val3.GetComponent<GridLayoutGroup>();
		component.cellSize = new Vector2(48f, 48f);
		component.constraint = (Constraint)1;
		component.constraintCount = 3;
		component.spacing = new Vector2(5f, 5f);
		((LayoutGroup)component).padding = new RectOffset(5, 5, 5, 5);
		((LayoutGroup)component).childAlignment = (TextAnchor)4;
		GameObject val4 = new GameObject("AuthButton", new Type[3]
		{
			typeof(Image),
			typeof(Button),
			typeof(TipButton)
		});
		val4.transform.SetParent(WorldBoxMod.Transform);
		prefab_auth_button = val4.GetComponent<Button>();
		((Selectable)prefab_auth_button).image.sprite = SpriteTextureLoader.getSprite("ui/special/special_buttonred");
		((Selectable)prefab_auth_button).image.type = (Type)1;
		GameObject val5 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val5.transform.SetParent(val4.transform);
		val5.transform.localPosition = Vector3.zero;
		val5.transform.localScale = Vector3.one;
		val5.GetComponent<RectTransform>().sizeDelta = new Vector2(42f, 42f);
		CreateAuthButton("DiscordAuth", "ui/icons/iconDiscordWhite", DiscordRoleAuthViaUserLoginUtils.Authenticate, new Vector2(42f, 30.7f));
		CreateAuthButton("GithubAuth", InternalResourcesGetter.GetGitHubIcon(), GithubOrgAuthUtils.Authenticate);
		CreateAuthButton("SkipAuth", "ui/icons/iconArrowBack", null);
	}

	private Button CreateAuthButton(string pId, Sprite pIcon, Func<bool> pAuthFunc, Vector2 pIconSize = default(Vector2))
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Button val = Object.Instantiate<Button>(prefab_auth_button, auth_grid_transform);
		((Component)((Component)val).transform.Find("Icon")).GetComponent<Image>().sprite = pIcon;
		if (pIconSize != default(Vector2))
		{
			((Component)((Component)val).transform.Find("Icon")).GetComponent<RectTransform>().sizeDelta = pIconSize;
		}
		((UnityEvent)val.onClick).AddListener((UnityAction)delegate
		{
			if (pAuthFunc != null)
			{
				AuthFunc = pAuthFunc;
				AuthFuncSelected = true;
			}
			else
			{
				AuthSkipped = true;
			}
		});
		TipButton component = ((Component)val).GetComponent<TipButton>();
		component.textOnClick = pId + " Title";
		component.text_description_2 = pId + " Description";
		return val;
	}

	private Button CreateAuthButton(string pId, string pIconPath, Func<bool> pAuthFunc, Vector2 pIconSize = default(Vector2))
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return CreateAuthButton(pId, SpriteTextureLoader.getSprite(pIconPath), pAuthFunc, pIconSize);
	}

	public static void SetState(bool pAuthState, string pTipText = null)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)AbstractWindow<ModUploadAuthenticationWindow>.Instance.auth_text).color = (pAuthState ? Color.green : Color.red);
		AbstractWindow<ModUploadAuthenticationWindow>.Instance.localized_auth_text.setKeyAndUpdate(pAuthState ? "NML_AUTHENTICATED" : "NML_AUTHENTICATION_FAILED");
		if (!string.IsNullOrEmpty(pTipText))
		{
			Text obj = AbstractWindow<ModUploadAuthenticationWindow>.Instance.auth_text;
			obj.text = obj.text + "\n" + pTipText;
			LogService.LogInfoConcurrent(pTipText);
		}
	}

	public static void SetText(string pText, Color pColor = default(Color))
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)AbstractWindow<ModUploadAuthenticationWindow>.Instance.auth_text).color = ((pColor == default(Color)) ? Color.white : pColor);
		AbstractWindow<ModUploadAuthenticationWindow>.Instance.auth_text.text = pText;
	}

	public bool Opened()
	{
		return IsOpened;
	}

	public override void OnNormalEnable()
	{
		base.OnNormalEnable();
		AuthSkipped = false;
		AuthFuncSelected = false;
		AuthFunc = null;
	}

	public override void OnNormalDisable()
	{
		base.OnNormalDisable();
	}
}
