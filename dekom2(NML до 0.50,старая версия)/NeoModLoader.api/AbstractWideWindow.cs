using System;
using NeoModLoader.General;
using NeoModLoader.utils;
using UnityEngine;
using UnityEngine.UI;

namespace NeoModLoader.api;

public abstract class AbstractWideWindow<T> : AbstractWindow<T> where T : AbstractWideWindow<T>
{
	public void SetSize(Vector2 pSize)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		((Component)AbstractWindow<T>.Instance.BackgroundTransform).GetComponent<RectTransform>().sizeDelta = pSize;
		AbstractWindow<T>.Instance.BackgroundTransform.parent.Find("CloseBackground").localPosition = new Vector3(pSize.x / 2f - 20f, pSize.y / 2f + 7f);
		((Component)AbstractWindow<T>.Instance.BackgroundTransform.Find("TitleBackground")).GetComponent<RectTransform>().sizeDelta = new Vector2(pSize.x / 2f, 30f);
		AbstractWindow<T>.Instance.BackgroundTransform.Find("TitleBackground").localPosition = new Vector3(0f, pSize.y / 2f + 5f);
		((Component)((Component)AbstractWindow<T>.Instance).GetComponent<ScrollWindow>().titleText).transform.localPosition = new Vector3(0f, pSize.y / 2f + 5f);
		((Component)((Component)AbstractWindow<T>.Instance).GetComponent<ScrollWindow>().titleText).GetComponent<RectTransform>().sizeDelta = new Vector2(pSize.x / 2f * 0.92f, 28f);
	}

	public static T CreateAndInit(string pWindowId, Vector2 pSize = default(Vector2))
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		AbstractWindow<T>.WindowId = pWindowId;
		if (pSize == default(Vector2))
		{
			((Vector2)(ref pSize))._002Ector(600f, 280f);
		}
		ScrollWindow scrollWindow = WindowCreator.CreateEmptyWindow(pWindowId, pWindowId + " Title");
		GameObject gameObject = ((Component)scrollWindow).gameObject;
		AbstractWindow<T>.Instance = gameObject.AddComponent<T>();
		((Component)AbstractWindow<T>.Instance).gameObject.SetActive(false);
		AbstractWindow<T>.Instance.BackgroundTransform = ((Component)scrollWindow).transform.Find("Background");
		((Component)AbstractWindow<T>.Instance.BackgroundTransform.Find("Scroll View")).gameObject.SetActive(true);
		AbstractWindow<T>.Instance.ContentTransform = AbstractWindow<T>.Instance.BackgroundTransform.Find("Scroll View/Viewport/Content");
		((Component)AbstractWindow<T>.Instance.BackgroundTransform).GetComponent<Image>().sprite = InternalResourcesGetter.GetWindowEmptyFrame();
		((Component)AbstractWindow<T>.Instance.BackgroundTransform).GetComponent<Image>().type = (Type)1;
		GameObject val = new GameObject("TitleBackground", new Type[1] { typeof(Image) });
		val.transform.SetParent(AbstractWindow<T>.Instance.BackgroundTransform);
		val.transform.localPosition = new Vector3(0f, 145f);
		val.transform.localScale = Vector3.one;
		val.transform.SetSiblingIndex(1);
		val.GetComponent<Image>().sprite = InternalResourcesGetter.GetWindowBigCloseSliced();
		val.GetComponent<Image>().type = (Type)1;
		AbstractWindow<T>.Instance.SetSize(pSize);
		AbstractWindow<T>.Instance.Init();
		AbstractWindow<T>.Instance.Initialized = true;
		return AbstractWindow<T>.Instance;
	}
}
