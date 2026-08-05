using System;
using System.Collections.Generic;
using db;
using UnityEngine;

public class GraphTimeScaleContainer : MonoBehaviour
{
	public GraphTimeScale current_scale;

	private List<GraphTimeScale> _available_time_scales = new List<GraphTimeScale>();

	private GraphController _controller;

	public void calcBounds()
	{
		if (_controller == null)
		{
			_controller = GetComponentInParent<GraphController>();
		}
		_available_time_scales.Clear();
		_available_time_scales.Add(GraphTimeScale.year_10);
		foreach (NanoObject @object in _controller.getObjects())
		{
			using ListPool<GraphTimeScale> tAvailableTimeScales = DBGetter.getTimeScales(@object);
			foreach (ref GraphTimeScale item in tAvailableTimeScales)
			{
				GraphTimeScale tScale = item;
				if (!_available_time_scales.Contains(tScale))
				{
					_available_time_scales.Add(tScale);
				}
			}
		}
		bool tAnyTimeScalesAvailable = _available_time_scales.Count > 1;
		ButtonGraphScalePlusMinus[] componentsInChildren = GetComponentsInChildren<ButtonGraphScalePlusMinus>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.SetActive(tAnyTimeScalesAvailable);
		}
	}

	public bool resetTimeScale()
	{
		calcBounds();
		if (!_available_time_scales.Contains(current_scale))
		{
			current_scale = _available_time_scales.Last();
			return true;
		}
		return false;
	}

	public void setTimeScale(GraphTimeScale pScale)
	{
		current_scale = pScale;
	}

	public ListPool<GraphTimeScale> sharedTimeScales()
	{
		ListPool<GraphTimeScale> tScales = new ListPool<GraphTimeScale>((GraphTimeScale[])Enum.GetValues(typeof(GraphTimeScale)));
		foreach (NanoObject tCurrentObject in _controller.getObjects())
		{
			ListPool<GraphTimeScale> tAvailableTimeScales = DBGetter.getTimeScales(tCurrentObject);
			try
			{
				tScales.RemoveAll((GraphTimeScale tScale) => !tAvailableTimeScales.Contains(tScale));
			}
			finally
			{
				if (tAvailableTimeScales != null)
				{
					((IDisposable)tAvailableTimeScales).Dispose();
				}
			}
		}
		return tScales;
	}

	public bool randomizeTimeScale()
	{
		if (_available_time_scales.Count < 2)
		{
			return false;
		}
		using ListPool<GraphTimeScale> tScales = sharedTimeScales();
		if (tScales.Count == 0)
		{
			return false;
		}
		if (tScales.Count > 2)
		{
			tScales.Shift();
		}
		GraphTimeScale tScale = tScales.GetRandom();
		if (tScale != current_scale)
		{
			current_scale = tScale;
			return true;
		}
		return false;
	}

	public void timeScaleMinus()
	{
		int currentScaleIndex = (int)current_scale;
		if (currentScaleIndex > 0)
		{
			current_scale = (GraphTimeScale)(currentScaleIndex - 1);
		}
		else
		{
			current_scale = (GraphTimeScale)(_available_time_scales.Count - 1);
		}
	}

	public void timeScalePlus()
	{
		int currentScaleIndex = (int)current_scale;
		if (currentScaleIndex < _available_time_scales.Count - 1)
		{
			current_scale = (GraphTimeScale)(currentScaleIndex + 1);
		}
		else
		{
			current_scale = GraphTimeScale.year_10;
		}
	}

	public string getIndexString()
	{
		if (_available_time_scales.Count == 0)
		{
			return "";
		}
		return " (" + (int)(current_scale + 1) + "/" + _available_time_scales.Count + ")";
	}

	public GraphTimeScale getCurrentScale()
	{
		return current_scale;
	}
}
