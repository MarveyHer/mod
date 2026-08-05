using System;
using NeoModLoader.General;
using NeoModLoader.General.UI.Window;
using NeoModLoader.services;
using UnityEngine;
using UnityEngine.UI;

namespace NeoModLoader.ui;

public class InformationWindow : SingleAutoLayoutWindow<InformationWindow>
{
	private Action on_close;

	private Text text;

	protected override void Init()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		text = new GameObject("Text", new Type[1] { typeof(Text) }).GetComponent<Text>();
		OT.InitializeCommonText(text);
		text.resizeTextForBestFit = true;
		text.resizeTextMinSize = 10;
		text.resizeTextMaxSize = 14;
		text.alignment = (TextAnchor)4;
		AddChild(((Component)text).gameObject);
	}

	public static void ShowWindow(string info, Action on_close = null)
	{
		SingleAutoLayoutWindow<InformationWindow>.Instance.text.text = info;
		SingleAutoLayoutWindow<InformationWindow>.Instance.on_close = on_close;
		ScrollWindow.showWindow(SingleAutoLayoutWindow<InformationWindow>.WindowId);
	}

	public override void OnNormalDisable()
	{
		try
		{
			on_close?.Invoke();
		}
		catch (Exception ex)
		{
			LogService.LogError(ex.Message);
			LogService.LogError(ex.StackTrace);
		}
		on_close = null;
	}

	public static void HideWindow()
	{
		SingleAutoLayoutWindow<InformationWindow>.Instance.ScrollWindowComponent.clickHide();
	}

	public static void Back()
	{
		SingleAutoLayoutWindow<InformationWindow>.Instance.ScrollWindowComponent.clickBack();
	}
}
