using System;
using System.Collections.Generic;
using System.IO;
using NeoModLoader.api;
using NeoModLoader.services;
using NeoModLoader.utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NeoModLoader.ui;

internal class WorkshopModListWindow : AbstractListWindow<WorkshopModListWindow, ModDeclare>
{
	public class WorkshopModListItem : AbstractListWindowItem<ModDeclare>
	{
		public override void Setup(ModDeclare modDeclare)
		{
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Expected O, but got Unknown
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Expected O, but got Unknown
			Text component = ((Component)((Component)this).transform.Find("Text")).GetComponent<Text>();
			component.text = modDeclare.Name + "\t" + modDeclare.Version + "\n" + modDeclare.Author + "\n" + modDeclare.Description;
			Sprite val = null;
			if (!string.IsNullOrEmpty(modDeclare.IconPath))
			{
				val = SpriteLoadUtils.LoadSingleSprite(Path.Combine(modDeclare.FolderPath, modDeclare.IconPath));
			}
			if ((Object)(object)val == (Object)null)
			{
				val = InternalResourcesGetter.GetIcon();
			}
			Image component2 = ((Component)((Component)this).transform.Find("Icon")).GetComponent<Image>();
			component2.sprite = val;
			Button component3 = ((Component)((Component)this).transform.Find("Load")).GetComponent<Button>();
			((UnityEvent)component3.onClick).AddListener((UnityAction)delegate
			{
				if (ModCompileLoadService.IsModLoaded(modDeclare.UID))
				{
					ErrorWindow.errorMessage = "Failed to load mod " + modDeclare.Name + ":\nMod already loaded.";
					ScrollWindow.get("error_with_reason").clickShow(false);
				}
				else
				{
					ModCompileLoadService.TryCompileAndLoadModAtRuntime(modDeclare);
				}
			});
			Button component4 = ((Component)((Component)this).transform.Find("Website")).GetComponent<Button>();
			((UnityEvent)component4.onClick).AddListener((UnityAction)delegate
			{
				string fileName = Path.GetFileName(modDeclare.FolderPath);
				Application.OpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + fileName);
			});
		}
	}

	private float checkTimer = 0.015f;

	private HashSet<string> showedMods = new HashSet<string>();

	private void Update()
	{
		if (checkTimer > 0f)
		{
			checkTimer -= Time.deltaTime;
			return;
		}
		checkTimer = 0.015f;
		showNextMod();
	}

	protected override void Init()
	{
	}

	public override void OnNormalEnable()
	{
		ModWorkshopService.steamWorkshopPromise.Then((Action)ModWorkshopService.FindSubscribedMods).Catch((Action<Exception>)delegate(Exception err)
		{
			Debug.LogError((object)err);
			ErrorWindow.errorMessage = "Error happened while connecting to Steam Workshop:\n" + err.Message.ToString();
			ScrollWindow.get("error_with_reason").clickShow(false);
		});
	}

	private void showNextMod()
	{
		ModDeclare nextModFromWorkshopItem = ModWorkshopService.GetNextModFromWorkshopItem();
		if (nextModFromWorkshopItem != null)
		{
			AddItemToList(nextModFromWorkshopItem);
		}
	}

	protected override void AddItemToList(ModDeclare item)
	{
		if (!showedMods.Contains(item.UID))
		{
			showedMods.Add(item.UID);
			base.AddItemToList(item);
		}
	}

	protected override AbstractListWindowItem<ModDeclare> CreateItemPrefab()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Expected O, but got Unknown
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Expected O, but got Unknown
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Expected O, but got Unknown
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Expected O, but got Unknown
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Expected O, but got Unknown
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("WorkshopModListItemPrefab", new Type[2]
		{
			typeof(Image),
			typeof(WorkshopModListItem)
		});
		val.SetActive(false);
		val.transform.SetParent(WorldBoxMod.Transform);
		val.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 50f);
		Image component = val.GetComponent<Image>();
		component.sprite = Resources.Load<Sprite>("ui/special/windowInnerSliced");
		component.type = (Type)1;
		GameObject val2 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val2.transform.SetParent(val.transform);
		val2.transform.localPosition = new Vector3(-75f, 0f);
		val2.transform.localScale = Vector3.one;
		val2.GetComponent<RectTransform>().sizeDelta = new Vector2(40f, 40f);
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
		val4.transform.localPosition = new Vector3(12.5f, 0f);
		val4.transform.localScale = Vector3.one;
		val4.GetComponent<RectTransform>().sizeDelta = new Vector2(125f, 50f);
		Text component4 = val4.GetComponent<Text>();
		component4.font = LocalizedTextManager.current_font;
		component4.fontSize = 6;
		component4.supportRichText = true;
		Vector2 val5 = default(Vector2);
		((Vector2)(ref val5))._002Ector(22f, 22f);
		GameObject val6 = new GameObject("Load", new Type[2]
		{
			typeof(Image),
			typeof(Button)
		});
		val6.transform.SetParent(val.transform);
		val6.transform.localPosition = new Vector3(87f, 12f);
		val6.transform.localScale = Vector3.one;
		val6.GetComponent<RectTransform>().sizeDelta = val5;
		Image component5 = val6.GetComponent<Image>();
		component5.sprite = Resources.Load<Sprite>("ui/special/button2");
		component5.type = (Type)1;
		GameObject val7 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val7.transform.SetParent(val6.transform);
		val7.transform.localPosition = Vector3.zero;
		val7.transform.localScale = Vector3.one;
		val7.GetComponent<RectTransform>().sizeDelta = val5 * 0.875f;
		Image component6 = val7.GetComponent<Image>();
		component6.sprite = Resources.Load<Sprite>("ui/icons/iconGameServices");
		GameObject val8 = new GameObject("Website", new Type[2]
		{
			typeof(Image),
			typeof(Button)
		});
		val8.transform.SetParent(val.transform);
		val8.transform.localPosition = new Vector3(87f, -12f);
		val8.transform.localScale = Vector3.one;
		val8.GetComponent<RectTransform>().sizeDelta = val5;
		Image component7 = val8.GetComponent<Image>();
		component7.sprite = Resources.Load<Sprite>("ui/special/button2");
		component7.type = (Type)1;
		GameObject val9 = new GameObject("Icon", new Type[1] { typeof(Image) });
		val9.transform.SetParent(val8.transform);
		val9.transform.localPosition = Vector3.zero;
		val9.transform.localScale = Vector3.one;
		val9.GetComponent<RectTransform>().sizeDelta = val5 * 0.875f;
		Image component8 = val9.GetComponent<Image>();
		component8.sprite = Resources.Load<Sprite>("ui/icons/iconCommunity");
		return val.GetComponent<WorkshopModListItem>();
	}
}
