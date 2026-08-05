using NeoModLoader.General.UI.Window.Layout;
using UnityEngine;
using UnityEngine.UI;

namespace NeoModLoader.General.UI.Window;

public abstract class AutoLayoutWindow<T> : AutoVertLayoutGroup where T : AutoLayoutWindow<T>
{
	protected new bool Initialized;

	protected bool IsFirstOpen = true;

	protected bool IsOpened;

	protected ScrollWindow ScrollWindowComponent { get; set; }

	protected Transform ContentTransform { get; set; }

	protected Transform BackgroundTransform { get; set; }

	protected internal string WindowID { get; set; }

	private void OnEnable()
	{
		if (Initialized)
		{
			if (IsFirstOpen)
			{
				IsFirstOpen = false;
				OnFirstEnable();
			}
			OnNormalEnable();
			IsOpened = true;
		}
	}

	private void OnDisable()
	{
		if (Initialized)
		{
			IsOpened = false;
			OnNormalDisable();
		}
	}

	public static T CreateWindow(string pWindowID, string pWindowTitleKey)
	{
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		ScrollWindow scrollWindow = WindowCreator.CreateEmptyWindow(pWindowID, pWindowTitleKey);
		((Component)scrollWindow).gameObject.SetActive(false);
		((Component)scrollWindow.transform_content).gameObject.AddComponent<VerticalLayoutGroup>();
		T val = ((Component)scrollWindow.transform_content).gameObject.AddComponent<T>();
		val.BackgroundTransform = ((Component)scrollWindow).transform.Find("Background");
		((Component)scrollWindow.transform_scrollRect).gameObject.SetActive(true);
		val.ContentTransform = (Transform)(object)scrollWindow.transform_content;
		val.ScrollWindowComponent = scrollWindow;
		VerticalLayoutGroup layoutGroup = val.GetLayoutGroup();
		((LayoutGroup)layoutGroup).childAlignment = (TextAnchor)1;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childControlHeight = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childControlWidth = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childForceExpandHeight = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childForceExpandWidth = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childScaleHeight = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childScaleWidth = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).spacing = 10f;
		((LayoutGroup)layoutGroup).padding = new RectOffset(3, 3, 10, 10);
		ContentSizeFitter val2 = ((Component)scrollWindow.transform_content).gameObject.AddComponent<ContentSizeFitter>();
		val2.verticalFit = (FitMode)2;
		val2.horizontalFit = (FitMode)0;
		val.WindowID = pWindowID;
		val.Init();
		val.Initialized = true;
		return val;
	}

	protected new abstract void Init();

	public static void Reconstruct(ref T pWindow)
	{
		pWindow.ScrollWindowComponent.clickHide();
		ScrollWindow._all_windows.Remove(pWindow.WindowID);
		string windowID = pWindow.WindowID;
		string key = ((Component)pWindow.ScrollWindowComponent.titleText).GetComponent<LocalizedText>().key;
		Object.Destroy((Object)(object)((Component)pWindow.ScrollWindowComponent).gameObject);
		pWindow = CreateWindow(windowID, key);
	}

	public virtual void OnNormalDisable()
	{
	}

	public virtual void OnFirstEnable()
	{
	}

	public virtual void OnNormalEnable()
	{
	}
}
