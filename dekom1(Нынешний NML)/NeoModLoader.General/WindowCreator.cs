using UnityEngine;

namespace NeoModLoader.General;

public static class WindowCreator
{
	internal static void init()
	{
	}

	public static ScrollWindow CreateEmptyWindow(string pWindowID, string pWindowTitleKey, string pWindowIcon = "neomodloader")
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		if (ScrollWindow._all_windows.TryGetValue(pWindowID, out var value))
		{
			return value;
		}
		ScrollWindow scrollWindow = Object.Instantiate<ScrollWindow>(Resources.Load<ScrollWindow>("windows/empty"), CanvasMain.instance.transformWindows);
		scrollWindow.screen_id = pWindowID;
		((Object)scrollWindow).name = pWindowID;
		LocalizedText component = ((Component)scrollWindow.titleText).GetComponent<LocalizedText>();
		component.key = pWindowTitleKey;
		LocalizedTextManager.instance.texts.Add(component);
		ScrollWindow._all_windows[pWindowID] = scrollWindow;
		scrollWindow.create(true);
		Transform val = ((Component)scrollWindow).transform.Find("Background");
		((Component)val.Find("Scroll View")).gameObject.SetActive(true);
		((Component)val.Find("Scroll View")).GetComponent<RectTransform>().sizeDelta = new Vector2(232f, 270f);
		val.Find("Scroll View").localPosition = new Vector3(0f, -6f);
		((Component)val.Find("Scroll View/Viewport")).GetComponent<RectTransform>().sizeDelta = new Vector2(30f, 0f);
		val.Find("Scroll View/Viewport").localPosition = new Vector3(-131f, 135f);
		AssetManager.window_library.add(new WindowAsset
		{
			id = pWindowID,
			icon_path = pWindowIcon
		});
		return scrollWindow;
	}
}
