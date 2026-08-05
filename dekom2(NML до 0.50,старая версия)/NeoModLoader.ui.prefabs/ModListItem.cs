using System;
using System.IO;
using NeoModLoader.api;
using NeoModLoader.General;
using NeoModLoader.General.UI.Prefabs;
using NeoModLoader.utils;
using UnityEngine;
using UnityEngine.UI;

namespace NeoModLoader.ui.prefabs;

internal class ModListItem : APrefab<ModListItem>
{
	private Image icon;

	private Text text;

	protected override void Init()
	{
		if (!Initialized)
		{
			base.Init();
			icon = ((Component)((Component)this).transform.Find("ModIcon")).GetComponent<Image>();
			text = ((Component)((Component)this).transform.Find("SimpleInfo")).GetComponent<Text>();
		}
	}

	public void Setup(ModDeclare pDeclare, Action pAction)
	{
		Init();
		if (!string.IsNullOrEmpty(pDeclare.IconPath))
		{
			icon.sprite = SpriteLoadUtils.LoadSingleSprite(Path.Combine(pDeclare.FolderPath, pDeclare.IconPath));
		}
		if ((Object)(object)icon.sprite == (Object)null)
		{
			icon.sprite = InternalResourcesGetter.GetIcon();
		}
		((Object)this).name = pDeclare.Name;
		string text = pDeclare.Name;
		string text2 = pDeclare.Author;
		string text3 = text + "_" + LocalizedTextManager.instance.language;
		string text4 = text2 + "_" + LocalizedTextManager.instance.language;
		if (LocalizedTextManager.stringExists(text3))
		{
			text = LM.Get(text3);
		}
		if (LocalizedTextManager.stringExists(text4))
		{
			text2 = LM.Get(text4);
		}
		this.text.text = text + "\n" + text2;
	}

	private static void _init()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Expected O, but got Unknown
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("ModListItem", new Type[1] { typeof(Image) });
		val.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
		val.GetComponent<Image>().type = (Type)1;
		val.GetComponent<RectTransform>().sizeDelta = new Vector2(88f, 40f);
		GameObject val2 = new GameObject("ModIcon", new Type[1] { typeof(Image) });
		val2.transform.SetParent(val.transform);
		val2.transform.localPosition = new Vector3(-24.5f, 0f, 0f);
		val2.transform.localScale = Vector3.one;
		val2.GetComponent<Image>().sprite = InternalResourcesGetter.GetIcon();
		val2.GetComponent<RectTransform>().sizeDelta = new Vector2(28f, 28f);
		GameObject val3 = new GameObject("IconFrame", new Type[1] { typeof(Image) });
		val3.transform.SetParent(val2.transform);
		val3.transform.localPosition = Vector3.zero;
		val3.transform.localScale = Vector3.one;
		val3.GetComponent<Image>().sprite = InternalResourcesGetter.GetIconFrame();
		val3.GetComponent<RectTransform>().sizeDelta = new Vector2(36f, 36f);
		GameObject val4 = new GameObject("ModName", new Type[1] { typeof(Text) });
		val4.transform.SetParent(val.transform);
		val4.transform.localPosition = new Vector3(20f, 0f, 0f);
		val4.transform.localScale = Vector3.one;
		val4.GetComponent<RectTransform>().sizeDelta = new Vector2(48f, 34f);
		Text component = val4.GetComponent<Text>();
		component.text = "Mod Name\nMod Author";
		component.alignment = (TextAnchor)0;
		component.font = LocalizedTextManager.current_font;
		component.fontSize = 6;
		component.supportRichText = true;
		APrefab<ModListItem>.Prefab = val.AddComponent<ModListItem>();
	}
}
