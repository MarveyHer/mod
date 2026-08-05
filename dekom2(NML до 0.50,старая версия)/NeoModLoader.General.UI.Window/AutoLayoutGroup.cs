using System;
using UnityEngine;
using UnityEngine.UI;

namespace NeoModLoader.General.UI.Window;

public abstract class AutoLayoutGroup<T, TElement> : AutoLayoutElement<TElement> where T : LayoutGroup where TElement : AutoLayoutGroup<T, TElement>
{
	protected ContentSizeFitter m_fitter;

	protected T m_layout;

	public ContentSizeFitter fitter
	{
		get
		{
			if ((Object)(object)m_fitter == (Object)null)
			{
				m_fitter = ((Component)this).gameObject.GetComponent<ContentSizeFitter>();
			}
			return m_fitter;
		}
	}

	public T layout
	{
		get
		{
			if ((Object)(object)m_layout == (Object)null)
			{
				m_layout = GetLayoutGroup();
			}
			return m_layout;
		}
	}

	public virtual void AddChild(GameObject pChild, int pIndex = -1)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Transform transform;
		(transform = pChild.transform).SetParent(((Component)this).transform);
		transform.localScale = Vector3.one;
		int childCount = ((Component)this).transform.childCount;
		transform.SetSiblingIndex((pIndex + childCount) % childCount);
	}

	public virtual T GetLayoutGroup()
	{
		T component = ((Component)this).gameObject.GetComponent<T>();
		return ((Object)(object)component != (Object)null) ? component : ((Component)this).gameObject.AddComponent<T>();
	}

	public TSub BeginSubGroup<TSub, TSubGroup>(Vector2 pSize = default(Vector2)) where TSub : AutoLayoutGroup<TSubGroup, TSub> where TSubGroup : LayoutGroup
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("TSubGroup", new Type[2]
		{
			typeof(TSub),
			typeof(TSubGroup)
		});
		TSub component = val.GetComponent<TSub>();
		if (pSize != default(Vector2))
		{
			component.SetSize(pSize);
		}
		AddChild(val);
		return component;
	}

	public override void SetSize(Vector2 pSize)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).GetComponent<RectTransform>().sizeDelta = pSize;
	}
}
