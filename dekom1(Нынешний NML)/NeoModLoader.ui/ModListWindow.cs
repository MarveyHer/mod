using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
		private IMod _mod;

		private IEnumerator WaitOpenWindow()
		{
			yield return (object)new WaitForSeconds(3f);
			if (AbstractWindow<ModListWindow>.Instance.clickTimes == 8)
			{
				ModUploadWindow.ShowWindow(_mod);
			}
		}

		public override void Setup(IMod mod)
		{
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Expected O, but got Unknown
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Expected O, but got Unknown
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Expected O, but got Unknown
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Expected O, but got Unknown
			//IL_0582: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Expected O, but got Unknown
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_053f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Expected O, but got Unknown
			//IL_0648: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Expected O, but got Unknown
			_mod = mod;
			ModDeclare mod_declare = mod.GetDeclaration();
			ModState modState = WorldBoxMod.AllRecognizedMods[mod_declare];
			Text component = ((Component)((Component)this).transform.Find("Text")).GetComponent<Text>();
			Text state_text = ((Component)((Component)this).transform.Find("StateText")).GetComponent<Text>();
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
			switch (mod_declare.ModType)
			{
			case ModTypeEnum.NEOMOD:
			case ModTypeEnum.COMPILED_NEOMOD:
			case ModTypeEnum.RESOURCE_PACK:
				component.text = text + "\t" + mod_declare.Version + "\n" + text2 + "\n" + text3;
				break;
			case ModTypeEnum.BEPINEX:
				component.text = "[BepInEx] " + text + "\t" + mod_declare.Version + "\n" + text2 + "\n" + text3;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
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
						((MonoBehaviour)this).StartCoroutine("WaitOpenWindow");
					}
				});
			}
			if (1 == 0)
			{
			}
			string text7 = default(string);
			switch (modState)
			{
			case ModState.DISABLED:
				text7 = LM.Get("mod_state_disabled");
				break;
			case ModState.LOADED:
				text7 = LM.Get("mod_state_enabled");
				break;
			case ModState.FAILED:
				text7 = LM.Get("mod_state_failed");
				break;
			default:
				if (1 == 0)
				{
				}
				global::_003CPrivateImplementationDetails_003E.ThrowInvalidOperationException();
				break;
			}
			if (1 == 0)
			{
			}
			string current_state_text = text7;
			string next_state_text = LM.Get(ModInfoUtils.isModDisabled(mod_declare.UID) ? "mod_next_state_disabled" : "mod_next_state_enabled");
			state_text.text = current_state_text + ", " + next_state_text;
			if (modState == ModState.FAILED)
			{
				icon_tip_button.textOnClick = "ModLoadFailed Title";
				icon_tip_button.textOnClickDescription = "ModLoadFailed Description";
				icon_tip_button.text_description_2 = mod_declare.FailReason.ToString();
				((Graphic)icon).color = Color.red;
				((UnityEvent)((Component)icon).GetComponent<Button>().onClick).AddListener((UnityAction)delegate
				{
					//IL_0023: Unknown result type (might be due to invalid IL or missing references)
					//IL_001c: Unknown result type (might be due to invalid IL or missing references)
					bool flag = ModInfoUtils.toggleMod(mod_declare.UID);
					((Graphic)icon).color = (flag ? Color.red : Color.yellow);
					next_state_text = LM.Get((!flag) ? "mod_next_state_disabled" : "mod_next_state_enabled");
					state_text.text = current_state_text + ", " + next_state_text;
				});
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
					next_state_text = LM.Get((!flag) ? "mod_next_state_disabled" : "mod_next_state_enabled");
					state_text.text = current_state_text + ", " + next_state_text;
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

	private readonly Queue<IMod> to_add = new Queue<IMod>();

	private ModDeclare clickedMod;

	private int clickTimes;

	private float lastClickTime;

	private bool needRefresh;

	private void Update()
	{
		if (IsOpened && needRefresh)
		{
			if (to_add.Any())
			{
				AddItemToList(to_add.Dequeue());
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
		val.transform.localPosition = new Vector3(140f, 0f);
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
			to_add.Enqueue(loadedMod);
		}
		foreach (ModDeclare key in WorldBoxMod.AllRecognizedMods.Keys)
		{
			if (WorldBoxMod.AllRecognizedMods[key] != ModState.LOADED)
			{
				VirtualMod virtualMod = new VirtualMod();
				virtualMod.OnLoad(key, null);
				to_add.Enqueue(virtualMod);
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
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Expected O, but got Unknown
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Expected O, but got Unknown
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Expected O, but got Unknown
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Expected O, but got Unknown
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Expected O, but got Unknown
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Expected O, but got Unknown
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Expected O, but got Unknown
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Expected O, but got Unknown
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0801: Unknown result type (might be due to invalid IL or missing references)
		//IL_0808: Expected O, but got Unknown
		//IL_0823: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_0847: Unknown result type (might be due to invalid IL or missing references)
		//IL_084e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0858: Unknown result type (might be due to invalid IL or missing references)
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
		GameObject val5 = new GameObject("StateText", new Type[1] { typeof(Text) });
		val5.transform.SetParent(val.transform);
		val5.transform.localPosition = new Vector3(2.5f, -15.5f);
		val5.transform.localScale = Vector3.one;
		val5.GetComponent<RectTransform>().sizeDelta = new Vector2(105f, 10f);
		Text component5 = val5.GetComponent<Text>();
		component5.font = LocalizedTextManager.current_font;
		component5.fontSize = 6;
		component5.supportRichText = true;
		component5.alignment = (TextAnchor)6;
		Vector2 val6 = default(Vector2);
		((Vector2)(ref val6))._002Ector(22f, 22f);
		GameObject val7 = new GameObject("Configure", new Type[3]
		{
			typeof(Image),
			typeof(Button),
			typeof(TipButton)
		});
		val7.transform.SetParent(val.transform);
		val7.transform.localPosition = new Vector3(87f, 12f);
		val7.transform.localScale = Vector3.one;
		val7.GetComponent<RectTransform>().sizeDelta = val6;
		val7.GetComponent<TipButton>().textOnClick = "ModConfigure Title";
		Image component6 = val7.GetComponent<Image>();
		component6.sprite = Resources.Load<Sprite>("ui/special/button2");
		component6.type = (Type)1;
		GameObject val8 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val8.transform.SetParent(val7.transform);
		val8.transform.localPosition = Vector3.zero;
		val8.transform.localScale = Vector3.one;
		val8.GetComponent<RectTransform>().sizeDelta = val6 * 0.875f;
		Image component7 = val8.GetComponent<Image>();
		component7.sprite = Resources.Load<Sprite>("ui/icons/iconoptions");
		GameObject val9 = new GameObject("Website", new Type[3]
		{
			typeof(Image),
			typeof(Button),
			typeof(TipButton)
		});
		val9.transform.SetParent(val.transform);
		val9.transform.localPosition = new Vector3(87f, -12f);
		val9.transform.localScale = Vector3.one;
		val9.GetComponent<RectTransform>().sizeDelta = val6;
		val9.GetComponent<TipButton>().textOnClick = "ModCommunity Title";
		Image component8 = val9.GetComponent<Image>();
		component8.sprite = Resources.Load<Sprite>("ui/special/button2");
		component8.type = (Type)1;
		GameObject val10 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val10.transform.SetParent(val9.transform);
		val10.transform.localPosition = Vector3.zero;
		val10.transform.localScale = Vector3.one;
		val10.GetComponent<RectTransform>().sizeDelta = val6 * 0.875f;
		Image component9 = val10.GetComponent<Image>();
		component9.sprite = Resources.Load<Sprite>("ui/icons/actor_traits/iconcommunity");
		GameObject val11 = new GameObject("Reload", new Type[3]
		{
			typeof(Image),
			typeof(Button),
			typeof(TipButton)
		});
		val11.transform.SetParent(val.transform);
		val11.transform.localPosition = new Vector3(64f, -12f);
		val11.transform.localScale = Vector3.one;
		val11.GetComponent<RectTransform>().sizeDelta = val6 * 0.9f;
		val11.GetComponent<TipButton>().textOnClick = "ModReload Title";
		Image component10 = val11.GetComponent<Image>();
		component10.sprite = Resources.Load<Sprite>("ui/special/special_buttonred");
		component10.type = (Type)1;
		GameObject val12 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val12.transform.SetParent(val11.transform);
		val12.transform.localPosition = Vector3.zero;
		val12.transform.localScale = Vector3.one;
		val12.GetComponent<RectTransform>().sizeDelta = val6 * 0.875f * 0.9f;
		Image component11 = val12.GetComponent<Image>();
		component11.sprite = InternalResourcesGetter.GetReloadIcon();
		GameObject val13 = new GameObject("OpenFolder", new Type[3]
		{
			typeof(Image),
			typeof(Button),
			typeof(TipButton)
		});
		val13.transform.SetParent(val.transform);
		val13.transform.localPosition = new Vector3(64f, 11f);
		val13.transform.localScale = Vector3.one;
		val13.GetComponent<RectTransform>().sizeDelta = val6 * 0.9f;
		val13.GetComponent<TipButton>().textOnClick = "OpenFolder Title";
		Image component12 = val13.GetComponent<Image>();
		component12.sprite = Resources.Load<Sprite>("ui/special/special_buttonred");
		component12.type = (Type)1;
		GameObject val14 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val14.transform.SetParent(val13.transform);
		val14.transform.localPosition = Vector3.zero;
		val14.transform.localScale = Vector3.one;
		val14.GetComponent<RectTransform>().sizeDelta = val6 * 0.875f * 0.9f;
		Image component13 = val14.GetComponent<Image>();
		component13.sprite = SpriteTextureLoader.getSprite("ui/icons/iconCustomWorld");
		return val.GetComponent<ModListItem>();
	}
}
