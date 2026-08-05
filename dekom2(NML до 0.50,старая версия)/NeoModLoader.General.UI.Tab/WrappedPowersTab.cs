using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace NeoModLoader.General.UI.Tab;

internal class WrappedPowersTab
{
	private class WrappedRectTransform
	{
		public readonly bool Placehold;

		public readonly Vector2 PositionInGroup;

		public readonly RectTransform Rect;

		public WrappedRectTransform(RectTransform pRect, Vector2 pPositionInGroup, bool pPlacehold)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			base._002Ector();
			Rect = pRect;
			PositionInGroup = pPositionInGroup;
			Placehold = pPlacehold;
		}
	}

	private class PlaceholdRegions
	{
		private class SimpleRegion
		{
			public readonly Vector2 LeftUpCorner;

			public readonly Vector2 RightDownCorner;

			public SimpleRegion(RectTransform pRect)
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				base._002Ector();
				Rect rect = pRect.rect;
				LeftUpCorner = new Vector2(((Rect)(ref rect)).xMin, ((Rect)(ref rect)).yMax);
				RightDownCorner = new Vector2(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).yMin);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool Contains(float pX, float pY)
			{
				return ContainsX(pX) && ContainsY(pY);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool ContainsX(float pX)
			{
				return pX >= LeftUpCorner.x && pX <= RightDownCorner.x;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool ContainsY(float pY)
			{
				return pY >= RightDownCorner.y && pY <= RightDownCorner.y;
			}
		}

		private HashSet<SimpleRegion> _regions = new HashSet<SimpleRegion>();

		public void AddRegion(RectTransform pRect)
		{
			_regions.Add(new SimpleRegion(pRect));
		}

		public bool Overlap(RectTransform pRect)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Rect rect = pRect.rect;
			foreach (SimpleRegion region in _regions)
			{
				if (region.Contains(((Rect)(ref rect)).xMin, ((Rect)(ref rect)).yMin) || region.Contains(((Rect)(ref rect)).xMin, ((Rect)(ref rect)).yMax) || region.Contains(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).yMin) || region.Contains(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).yMax))
				{
					return true;
				}
			}
			return false;
		}
	}

	private const float space = 4f;

	private const float tab_start_x = 87.4f;

	private const float assumed_button_size = 32f;

	private static readonly RectTransform _empty_button_placehold;

	private static readonly float[] available_y;

	private Queue<GameObject> _active_lines;

	private Queue<GameObject> _inactive_lines;

	private Dictionary<string, List<PowerButton>> ButtonGroups;

	private Dictionary<string, List<WrappedRectTransform>> CustomRectGroups;

	public bool Modifiable;

	public PowersTab Tab;

	public WrappedPowersTab(PowersTab pPowersTab)
	{
		Tab = pPowersTab;
		Modifiable = !PowerTabNames.GetNames().Contains(((Object)Tab).name);
		ButtonGroups = new Dictionary<string, List<PowerButton>>();
		CustomRectGroups = new Dictionary<string, List<WrappedRectTransform>>();
		AddGroup("Default");
		_inactive_lines = new Queue<GameObject>();
		_active_lines = new Queue<GameObject>();
	}

	internal void RecordLine(GameObject line)
	{
		_active_lines.Enqueue(line);
	}

	public static void _init()
	{
		((Transform)_empty_button_placehold).SetParent(WorldBoxMod.Transform);
	}

	public bool HasGroup(string pGroupId)
	{
		return ButtonGroups.ContainsKey(pGroupId);
	}

	public void AddPowerButton(string pGroupId, PowerButton pPowerButton)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		List<PowerButton> list = ButtonGroups[pGroupId];
		if ((Object)(object)pPowerButton != (Object)null)
		{
			Transform transform;
			(transform = ((Component)pPowerButton).transform).SetParent(((Component)Tab).transform);
			transform.localScale = Vector3.one;
		}
		list.Add(pPowerButton);
	}

	public void AddCustomRect(string pGroupId, RectTransform pRect, Vector2 pPositionInGroup, bool pPlacehold)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		List<WrappedRectTransform> list = CustomRectGroups[pGroupId];
		((Transform)pRect).SetParent(((Component)Tab).transform);
		((Transform)pRect).localScale = Vector3.one;
		list.Add(new WrappedRectTransform(pRect, pPositionInGroup, pPlacehold));
	}

	public void AddGroup(string pGroupId)
	{
		ButtonGroups.Add(pGroupId, new List<PowerButton>());
		CustomRectGroups.Add(pGroupId, new List<WrappedRectTransform>());
	}

	public void ResetGroups()
	{
		ButtonGroups.Clear();
	}

	public void UpdateLayout()
	{
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		foreach (GameObject active_line in _active_lines)
		{
			active_line.SetActive(false);
			_inactive_lines.Enqueue(active_line);
		}
		float num = 87.4f;
		bool flag = true;
		foreach (string key in ButtonGroups.Keys)
		{
			List<PowerButton> list = ButtonGroups[key];
			List<WrappedRectTransform> list2 = CustomRectGroups[key];
			if (list.Count <= 0 && list2.Count <= 0)
			{
				continue;
			}
			num += 8f;
			if (!flag)
			{
				_add_line(num);
			}
			else
			{
				flag = false;
			}
			num += 8f;
			PlaceholdRegions placeholdRegions = new PlaceholdRegions();
			foreach (WrappedRectTransform item in list2)
			{
				((Transform)item.Rect).localPosition = Vector2.op_Implicit(item.PositionInGroup + new Vector2(num, 0f));
				if (item.Placehold)
				{
					placeholdRegions.AddRegion(item.Rect);
				}
			}
			bool flag2 = true;
			foreach (PowerButton item2 in list)
			{
				RectTransform val = (((Object)(object)item2 == (Object)null) ? _empty_button_placehold : ((Component)item2).GetComponent<RectTransform>());
				if ((Object)(object)item2 == (Object)null)
				{
					val.sizeDelta = new Vector2(32f, 32f);
					val.pivot = new Vector2(0.5f, 0.5f);
				}
				bool flag3 = false;
				while (!flag3)
				{
					if (flag2)
					{
						num += 16f;
						((Transform)val).localPosition = new Vector3(num, available_y[0]);
						flag2 = false;
						if (!placeholdRegions.Overlap(val))
						{
							flag3 = true;
						}
					}
					else
					{
						((Transform)val).localPosition = new Vector3(num, available_y[1]);
						flag2 = true;
						num += 20f;
						if (!placeholdRegions.Overlap(val))
						{
							flag3 = true;
						}
					}
				}
			}
			num = ((!flag2) ? (num + 16f) : (num + 4f));
		}
		Tab.powerButtons.Clear();
		PowerButton[] componentsInChildren = ((Component)Tab).GetComponentsInChildren<PowerButton>();
		foreach (PowerButton powerButton in componentsInChildren)
		{
			if (!((Object)(object)powerButton == (Object)null) && !((Object)(object)powerButton.rect_transform == (Object)null))
			{
				Tab.powerButtons.Add(powerButton);
			}
		}
		foreach (PowerButton powerButton2 in Tab.powerButtons)
		{
			powerButton2.findNeighbours(Tab.powerButtons);
		}
	}

	private void _add_line(float pX)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		GameObject val;
		if (_inactive_lines.Count > 0)
		{
			val = _inactive_lines.Dequeue();
		}
		else
		{
			val = Object.Instantiate<GameObject>(ResourcesFinder.FindResource<GameObject>("_line"), ((Component)Tab).transform);
			((Behaviour)val.GetComponent<Image>()).enabled = true;
			val.transform.localScale = new Vector3(1f, 48.3f, 1f);
		}
		val.SetActive(true);
		val.transform.localPosition = new Vector3(pX, 37.2f);
		_active_lines.Enqueue(val);
	}

	static WrappedPowersTab()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		_empty_button_placehold = new GameObject("Empty Button Placehold", new Type[1] { typeof(RectTransform) }).GetComponent<RectTransform>();
		available_y = new float[2] { 18f, -18f };
	}
}
