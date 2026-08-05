using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeoModLoader.General.UI.Tab;

public abstract class ReconstructedVanillaTab
{
	protected class TabElement
	{
		public Vector2 pos_in_group;

		public RectTransform element;
	}

	internal WrappedPowersTab tab;

	protected abstract string[] Groups { get; }

	internal void Init()
	{
		InitTab();
	}

	protected abstract void InitTab();

	public void AddPowerButton(string pGroupId, PowerButton pPowerButton)
	{
		tab.AddPowerButton(pGroupId, pPowerButton);
	}

	public void AddCustomRect(string pGroupId, RectTransform pCustomRect, Vector2 pPosInGroup, bool pPlaceholder)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		tab.AddCustomRect(pGroupId, pCustomRect, pPosInGroup, pPlaceholder);
	}

	protected List<List<TabElement>> TrackElements()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)tab.Tab).transform;
		int childCount = transform.childCount;
		List<Transform> list = new List<Transform>();
		List<Vector2> list2 = new List<Vector2>();
		for (int i = 0; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (_is_line(child))
			{
				tab.RecordLine(((Component)child).gameObject);
				list2.Add(Vector2.op_Implicit(child.position));
			}
			else
			{
				list.Add(child);
			}
		}
		list.Sort(delegate(Transform a, Transform b)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return a.position.x.CompareTo(b.position.x);
		});
		list2.Sort(delegate(Vector2 a, Vector2 b)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return a.x.CompareTo(b.x);
		});
		List<List<TabElement>> list3 = new List<List<TabElement>>();
		foreach (Vector2 item in list2)
		{
			List<TabElement> list4 = new List<TabElement>();
			foreach (Transform item2 in list)
			{
				if (item2.position.x < item.x)
				{
					list4.Add(new TabElement
					{
						pos_in_group = Vector2.op_Implicit(item2.localPosition - new Vector3(item.x, 0f)),
						element = ((Component)item2).GetComponent<RectTransform>()
					});
				}
			}
			_sort_group(list4);
			list3.Add(list4);
		}
		return list3;
	}

	private bool _is_line(Transform pTransform)
	{
		return ((Object)pTransform).name.ToLower().Contains("line");
	}

	private void _sort_group(List<TabElement> group)
	{
		throw new NotImplementedException();
	}
}
