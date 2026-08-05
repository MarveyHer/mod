using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComponentListSapient<TListElement, TMetaObject, TData, TComponent> : ComponentListBase<TListElement, TMetaObject, TData, TComponent>, ISapientListComponent where TListElement : WindowListElementBase<TMetaObject, TData> where TMetaObject : CoreSystemObject<TData> where TData : BaseSystemData where TComponent : ComponentListBase<TListElement, TMetaObject, TData, TComponent>
{
	[SerializeField]
	private Text _sapient_counter;

	[SerializeField]
	private Text _non_sapient_counter;

	private SapientListFilter _filter;

	protected override void show()
	{
		if (Config.game_loaded)
		{
			base.show();
			if (_sapient_counter != null)
			{
				_sapient_counter.text = latest_counted.ToString();
			}
			if (_non_sapient_counter != null)
			{
				_non_sapient_counter.text = latest_counted.ToString();
			}
		}
	}

	protected override IEnumerable<TMetaObject> getFiltered(IEnumerable<TMetaObject> pList)
	{
		switch (_filter)
		{
		case SapientListFilter.Default:
			foreach (TMetaObject item in base.getFiltered(pList))
			{
				yield return item;
			}
			break;
		case SapientListFilter.Sapient:
			foreach (ISapient tMeta2 in pList)
			{
				if (tMeta2.isSapient())
				{
					yield return (TMetaObject)tMeta2;
				}
			}
			break;
		case SapientListFilter.NonSapient:
			foreach (ISapient tMeta in pList)
			{
				if (!tMeta.isSapient())
				{
					yield return (TMetaObject)tMeta;
				}
			}
			break;
		}
	}

	public void setShowSapientOnly()
	{
		_filter = SapientListFilter.Sapient;
	}

	public void setShowNonSapientOnly()
	{
		_filter = SapientListFilter.NonSapient;
	}

	public override void setDefault()
	{
		_filter = SapientListFilter.Default;
	}

	public void setSapientCounter(Text pCounter)
	{
		_sapient_counter = pCounter;
	}

	public void setNonSapientCounter(Text pCounter)
	{
		_non_sapient_counter = pCounter;
	}
}
