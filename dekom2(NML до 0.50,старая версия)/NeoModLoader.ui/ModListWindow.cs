using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NeoModLoader.api;
using NeoModLoader.constants;
using NeoModLoader.General;
using NeoModLoader.services;
using NeoModLoader.utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NeoModLoader.ui;

public class ModListWindow : AbstractListWindow<ModListWindow, IMod>
{
	public class ModListItem : AbstractListWindowItem<IMod>
	{
		public override void Setup(IMod mod)
		{
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Expected O, but got Unknown
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Expected O, but got Unknown
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Expected O, but got Unknown
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Expected O, but got Unknown
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Expected O, but got Unknown
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Expected O, but got Unknown
			ModDeclare mod_declare = mod.GetDeclaration();
			ModState modState = WorldBoxMod.AllRecognizedMods[mod_declare];
			Text component = ((Component)((Component)this).transform.Find("Text")).GetComponent<Text>();
			string text = mod_declare.Name;
			string text2 = mod_declare.Author;
			string text3 = mod_declare.Description;
			string text4 = text + "_" + LocalizedTextManager.instance.language;
			string text5 = text2 + "_" + LocalizedTextManager.instance.language;
			string text6 = text3 + "_" + LocalizedTextManager.instance.language;
			if (LocalizedTextManager.stringExists(text4))
			{
				text = LM.Get(text4);
			}
			if (LocalizedTextManager.stringExists(text5))
			{
				text2 = LM.Get(text5);
			}
			if (LocalizedTextManager.stringExists(text6))
			{
				text3 = LM.Get(text6);
			}
			component.text = text + "\t" + mod_declare.Version + "\n" + text2 + "\n" + text3;
			Sprite val = null;
			if (!string.IsNullOrEmpty(mod_declare.IconPath) && File.Exists(Path.Combine(mod_declare.FolderPath, mod_declare.IconPath)))
			{
				val = SpriteLoadUtils.LoadSingleSprite(Path.Combine(mod_declare.FolderPath, mod_declare.IconPath));
			}
			if ((Object)(object)val == (Object)null)
			{
				val = InternalResourcesGetter.GetIcon();
			}
			Image icon = ((Component)((Component)this).transform.Find("Icon")).GetComponent<Image>();
			Button component2 = ((Component)((Component)this).transform.Find("Configure")).GetComponent<Button>();
			Button component3 = ((Component)((Component)this).transform.Find("Website")).GetComponent<Button>();
			Button component4 = ((Component)((Component)this).transform.Find("OpenFolder")).GetComponent<Button>();
			TipButton icon_tip_button = ((Component)icon).GetComponent<TipButton>();
			icon.sprite = val;
			GameObject gameObject = mod.GetGameObject();
			IConfigurable configurable = ((gameObject != null) ? gameObject.GetComponent<IConfigurable>() : null);
			((Component)component2).gameObject.SetActive(configurable != null);
			((UnityEventBase)((Component)icon).GetComponent<Button>().onClick).RemoveAllListeners();
			((UnityEventBase)component2.onClick).RemoveAllListeners();
			((UnityEventBase)component3.onClick).RemoveAllListeners();
			((UnityEventBase)component4.onClick).RemoveAllListeners();
			((UnityEvent)component4.onClick).AddListener((UnityAction)delegate
			{
				Application.OpenURL(mod_declare.FolderPath);
			});
			if (modState == ModState.LOADED)
			{
				((UnityEvent)((Component)icon).GetComponent<Button>().onClick).AddListener((UnityAction)delegate
				{
					float time = Time.time;
					if (time - AbstractWindow<ModListWindow>.Instance.lastClickTime > 1f)
					{
						AbstractWindow<ModListWindow>.Instance.clickTimes = 0;
					}
					if (mod_declare != AbstractWindow<ModListWindow>.Instance.clickedMod)
					{
						AbstractWindow<ModListWindow>.Instance.clickedMod = mod_declare;
						AbstractWindow<ModListWindow>.Instance.clickTimes = 0;
					}
					AbstractWindow<ModListWindow>.Instance.lastClickTime = time;
					AbstractWindow<ModListWindow>.Instance.clickTimes++;
					if (AbstractWindow<ModListWindow>.Instance.clickTimes == 8)
					{
						new Task(delegate
						{
							Thread.Sleep(3000);
							if (AbstractWindow<ModListWindow>.Instance.clickTimes == 8)
							{
								ModUploadWindow.ShowWindow(mod);
							}
						}).Start();
					}
				});
			}
			if (modState == ModState.FAILED)
			{
				icon_tip_button.textOnClick = "ModLoadFailed Title";
				icon_tip_button.textOnClickDescription = "ModLoadFailed Description";
				icon_tip_button.text_description_2 = mod_declare.FailReason.ToString();
				((Graphic)icon).color = Color.red;
			}
			else
			{
				icon_tip_button.textOnClick = "ToggleMod Title";
				icon_tip_button.textOnClickDescription = (ModInfoUtils.isModDisabled(mod_declare.UID) ? "ModDisabled Description" : "ModEnabled Description");
				((Graphic)icon).color = (ModInfoUtils.isModDisabled(mod_declare.UID) ? Color.gray : Color.white);
				((UnityEvent)((Component)icon).GetComponent<Button>().onClick).AddListener((UnityAction)delegate
				{
					//IL_003d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					bool flag = ModInfoUtils.toggleMod(mod_declare.UID);
					icon_tip_button.textOnClickDescription = (flag ? "ModEnabled Description" : "ModDisabled Description");
					((Graphic)icon).color = (flag ? Color.white : Color.gray);
					if (flag)
					{
						ModCompileLoadService.TryCompileAndLoadModAtRuntime(mod_declare);
					}
				});
				icon_tip_button.text_description_2 = "";
			}
			((UnityEvent)component2.onClick).AddListener((UnityAction)delegate
			{
				ModConfigureWindow.ShowWindow(configurable?.GetConfig());
			});
			((UnityEvent)component3.onClick).AddListener((UnityAction)delegate
			{
				Application.OpenURL(mod.GetUrl());
			});
			if (!Config.isEditor)
			{
				((Component)((Component)this).transform.Find("Reload")).gameObject.SetActive(false);
				return;
			}
			GameObject gameObject2 = mod.GetGameObject();
			IReloadable reloadable = ((gameObject2 != null) ? gameObject2.GetComponent<IReloadable>() : null);
			if (reloadable == null)
			{
				((Component)((Component)this).transform.Find("Reload")).gameObject.SetActive(false);
				return;
			}
			Button component5 = ((Component)((Component)this).transform.Find("Reload")).GetComponent<Button>();
			((Component)component5).gameObject.SetActive(true);
			((UnityEventBase)component5.onClick).RemoveAllListeners();
			((UnityEvent)component5.onClick).AddListener((UnityAction)delegate
			{
				if (!ModReloadUtils.Prepare(reloadable, mod_declare))
				{
					LogService.LogWarning("Failed to prepare mod " + mod_declare.Name + " for reloading.");
				}
				else if (!ModReloadUtils.CompileNew())
				{
					LogService.LogWarning("Failed to compile new mod " + mod_declare.Name + " for reloading.");
				}
				else if (!ModReloadUtils.PatchHotfixMethodsNT())
				{
					LogService.LogWarning("Failed to patch hotfix methods of mod " + mod_declare.Name + " for reloading.");
				}
				else if (!ModReloadUtils.Reload())
				{
					LogService.LogWarning("Failed to reload mod " + mod_declare.Name + ".");
				}
			});
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static UnityAction _003C_003E9__6_0;

		public static UnityAction _003C_003E9__6_1;

		internal void _003CInit_003Eb__6_0()
		{
			if (Others.is_editor)
			{
				InformationWindow.ShowWindow("WorkshopMods Window is not supported in editor environment");
			}
			else
			{
				ScrollWindow.showWindow("WorkshopMods");
			}
		}

		internal void _003CInit_003Eb__6_1()
		{
			Application.OpenURL("https://github.com/WorldBoxOpenMods/ModLoader");
		}
	}

	private ModDeclare clickedMod;

	private int clickTimes;

	private float lastClickTime;

	private bool needRefresh;

	private readonly List<IMod> to_add = new List<IMod>();

	private void Update()
	{
		if (IsOpened && needRefresh)
		{
			if (to_add.Any())
			{
				AddItemToList(to_add[to_add.Count - 1]);
				to_add.RemoveAt(to_add.Count - 1);
			}
			else
			{
				needRefresh = false;
			}
		}
	}

	protected override void Init()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Expected O, but got Unknown
		GameObject val = new GameObject("WorkshopButton", new Type[3]
		{
			typeof(Image),
			typeof(Button),
			typeof(TipButton)
		});
		val.transform.SetParent(base.BackgroundTransform);
		val.transform.localPosition = new Vector3(125f, 0f);
		val.transform.localScale = Vector3.one;
		val.GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 20f);
		Image component = val.GetComponent<Image>();
		component.sprite = Resources.Load<Sprite>("ui/icons/iconSteam");
		Button component2 = val.GetComponent<Button>();
		ButtonClickedEvent onClick = component2.onClick;
		object obj = _003C_003Ec._003C_003E9__6_0;
		if (obj == null)
		{
			UnityAction val2 = delegate
			{
				if (Others.is_editor)
				{
					InformationWindow.ShowWindow("WorkshopMods Window is not supported in editor environment");
				}
				else
				{
					ScrollWindow.showWindow("WorkshopMods");
				}
			};
			_003C_003Ec._003C_003E9__6_0 = val2;
			obj = (object)val2;
		}
		((UnityEvent)onClick).AddListener((UnityAction)obj);
		TipButton component3 = val.GetComponent<TipButton>();
		component3.textOnClick = "WorkshopMods Title";
		GameObject val3 = new GameObject("ModLoaderButton", new Type[3]
		{
			typeof(Image),
			typeof(Button),
			typeof(TipButton)
		});
		val3.transform.SetParent(base.BackgroundTransform);
		val3.transform.localPosition = new Vector3(-125f, 0f);
		val3.transform.localScale = Vector3.one;
		val3.GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 20f);
		Image component4 = val3.GetComponent<Image>();
		component4.sprite = InternalResourcesGetter.GetIcon();
		TipButton component5 = val3.GetComponent<TipButton>();
		component5.textOnClick = "NeoModLoader-v" + WorldBoxMod.NeoModLoaderAssembly.GetName().Version;
		foreach (string allLanguage in LocalizedTextManager.getAllLanguages())
		{
			LM.Add(allLanguage, "NMLCommit", "commit\n" + InternalResourcesGetter.GetCommit());
		}
		component5.text_description_2 = "NMLCommit";
		component5.textOnClickDescription = "NeoModLoader Report";
		Button component6 = val3.GetComponent<Button>();
		ButtonClickedEvent onClick2 = component6.onClick;
		object obj2 = _003C_003Ec._003C_003E9__6_1;
		if (obj2 == null)
		{
			UnityAction val4 = delegate
			{
				Application.OpenURL("https://github.com/WorldBoxOpenMods/ModLoader");
			};
			_003C_003Ec._003C_003E9__6_1 = val4;
			obj2 = (object)val4;
		}
		((UnityEvent)onClick2).AddListener((UnityAction)obj2);
	}

	public override void OnNormalEnable()
	{
		needRefresh = true;
		ClearList();
		foreach (IMod loadedMod in WorldBoxMod.LoadedMods)
		{
			to_add.Add(loadedMod);
		}
		foreach (ModDeclare key in WorldBoxMod.AllRecognizedMods.Keys)
		{
			if (WorldBoxMod.AllRecognizedMods[key] != ModState.LOADED)
			{
				VirtualMod virtualMod = new VirtualMod();
				virtualMod.OnLoad(key, null);
				to_add.Add(virtualMod);
			}
		}
	}

	protected override AbstractListWindowItem<IMod> CreateItemPrefab()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Expected O, but got Unknown
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Expected O, but got Unknown
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Expected O, but got Unknown
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Expected O, but got Unknown
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Expected O, but got Unknown
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Expected O, but got Unknown
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Expected O, but got Unknown
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Expected O, but got Unknown
		//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0754: Unknown result type (might be due to invalid IL or missing references)
		//IL_075b: Expected O, but got Unknown
		//IL_0776: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("ModListItemPrefab", new Type[2]
		{
			typeof(Image),
			typeof(ModListItem)
		});
		val.SetActive(false);
		val.transform.SetParent(WorldBoxMod.Transform);
		val.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 50f);
		Image component = val.GetComponent<Image>();
		component.sprite = Resources.Load<Sprite>("ui/special/windowInnerSliced");
		component.type = (Type)1;
		GameObject val2 = new GameObject("Icon", new Type[3]
		{
			typeof(Image),
			typeof(Button),
			typeof(TipButton)
		});
		val2.transform.SetParent(val.transform);
		val2.transform.localPosition = new Vector3(-75f, 0f);
		val2.transform.localScale = Vector3.one;
		val2.GetComponent<RectTransform>().sizeDelta = new Vector2(40f, 40f);
		val2.GetComponent<TipButton>().type = "normal";
		Image component2 = val2.GetComponent<Image>();
		component2.sprite = InternalResourcesGetter.GetIcon();
		GameObject val3 = new GameObject("IconFrame", new Type[1] { typeof(Image) });
		val3.transform.SetParent(val2.transform);
		val3.transform.localPosition = Vector3.zero;
		val3.transform.localScale = Vector3.one;
		val3.GetComponent<RectTransform>().sizeDelta = val2.GetComponent<RectTransform>().sizeDelta + new Vector2(5f, 5f);
		Image component3 = val3.GetComponent<Image>();
		component3.sprite = InternalResourcesGetter.GetIconFrame();
		component3.type = (Type)1;
		GameObject val4 = new GameObject("Text", new Type[1] { typeof(Text) });
		val4.transform.SetParent(val.transform);
		val4.transform.localPosition = new Vector3(2.5f, 0f);
		val4.transform.localScale = Vector3.one;
		val4.GetComponent<RectTransform>().sizeDelta = new Vector2(105f, 50f);
		Text component4 = val4.GetComponent<Text>();
		component4.font = LocalizedTextManager.current_font;
		component4.fontSize = 6;
		component4.supportRichText = true;
		Vector2 val5 = default(Vector2);
		((Vector2)(ref val5))._002Ector(22f, 22f);
		GameObject val6 = new GameObject("Configure", new Type[3]
		{
			typeof(Image),
			typeof(Button),
			typeof(TipButton)
		});
		val6.transform.SetParent(val.transform);
		val6.transform.localPosition = new Vector3(87f, 12f);
		val6.transform.localScale = Vector3.one;
		val6.GetComponent<RectTransform>().sizeDelta = val5;
		val6.GetComponent<TipButton>().textOnClick = "ModConfigure Title";
		Image component5 = val6.GetComponent<Image>();
		component5.sprite = Resources.Load<Sprite>("ui/special/button2");
		component5.type = (Type)1;
		GameObject val7 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val7.transform.SetParent(val6.transform);
		val7.transform.localPosition = Vector3.zero;
		val7.transform.localScale = Vector3.one;
		val7.GetComponent<RectTransform>().sizeDelta = val5 * 0.875f;
		Image component6 = val7.GetComponent<Image>();
		component6.sprite = Resources.Load<Sprite>("ui/icons/iconoptions");
		GameObject val8 = new GameObject("Website", new Type[3]
		{
			typeof(Image),
			typeof(Button),
			typeof(TipButton)
		});
		val8.transform.SetParent(val.transform);
		val8.transform.localPosition = new Vector3(87f, -12f);
		val8.transform.localScale = Vector3.one;
		val8.GetComponent<RectTransform>().sizeDelta = val5;
		val8.GetComponent<TipButton>().textOnClick = "ModCommunity Title";
		Image component7 = val8.GetComponent<Image>();
		component7.sprite = Resources.Load<Sprite>("ui/special/button2");
		component7.type = (Type)1;
		GameObject val9 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val9.transform.SetParent(val8.transform);
		val9.transform.localPosition = Vector3.zero;
		val9.transform.localScale = Vector3.one;
		val9.GetComponent<RectTransform>().sizeDelta = val5 * 0.875f;
		Image component8 = val9.GetComponent<Image>();
		component8.sprite = Resources.Load<Sprite>("ui/icons/actor_traits/iconcommunity");
		GameObject val10 = new GameObject("Reload", new Type[3]
		{
			typeof(Image),
			typeof(Button),
			typeof(TipButton)
		});
		val10.transform.SetParent(val.transform);
		val10.transform.localPosition = new Vector3(64f, -12f);
		val10.transform.localScale = Vector3.one;
		val10.GetComponent<RectTransform>().sizeDelta = val5 * 0.9f;
		val10.GetComponent<TipButton>().textOnClick = "ModReload Title";
		Image component9 = val10.GetComponent<Image>();
		component9.sprite = Resources.Load<Sprite>("ui/special/special_buttonred");
		component9.type = (Type)1;
		GameObject val11 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val11.transform.SetParent(val10.transform);
		val11.transform.localPosition = Vector3.zero;
		val11.transform.localScale = Vector3.one;
		val11.GetComponent<RectTransform>().sizeDelta = val5 * 0.875f * 0.9f;
		Image component10 = val11.GetComponent<Image>();
		component10.sprite = InternalResourcesGetter.GetReloadIcon();
		GameObject val12 = new GameObject("OpenFolder", new Type[3]
		{
			typeof(Image),
			typeof(Button),
			typeof(TipButton)
		});
		val12.transform.SetParent(val.transform);
		val12.transform.localPosition = new Vector3(64f, 11f);
		val12.transform.localScale = Vector3.one;
		val12.GetComponent<RectTransform>().sizeDelta = val5 * 0.9f;
		val12.GetComponent<TipButton>().textOnClick = "OpenFolder Title";
		Image component11 = val12.GetComponent<Image>();
		component11.sprite = Resources.Load<Sprite>("ui/special/special_buttonred");
		component11.type = (Type)1;
		GameObject val13 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val13.transform.SetParent(val12.transform);
		val13.transform.localPosition = Vector3.zero;
		val13.transform.localScale = Vector3.one;
		val13.GetComponent<RectTransform>().sizeDelta = val5 * 0.875f * 0.9f;
		Image component12 = val13.GetComponent<Image>();
		component12.sprite = SpriteTextureLoader.getSprite("ui/icons/iconCustomWorld");
		return val.GetComponent<ModListItem>();
	}
}
