using System.Reflection;
using NeoModLoader.utils;
using UnityEngine;

namespace NeoModLoader.General.UI.Prefabs;

public abstract class APrefab<T> : MonoBehaviour where T : APrefab<T>
{
	private static T mPrefab;

	protected bool Initialized;

	public static T Prefab
	{
		get
		{
			if ((Object)(object)mPrefab == (Object)null)
			{
				if (OtherUtils.CalledBy("_init", typeof(T), pSearchAll: true))
				{
					return null;
				}
				typeof(T).GetMethod("_init", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(null, null);
			}
			return mPrefab;
		}
		protected set
		{
			mPrefab = value;
		}
	}

	public static T Instantiate(Transform pParent = null, bool pWorldPositionStays = false, string pName = null)
	{
		T val = Object.Instantiate<T>(Prefab, pParent, pWorldPositionStays);
		if (!string.IsNullOrEmpty(pName))
		{
			((Object)val).name = pName;
		}
		return val;
	}

	public virtual void SetSize(Vector2 pSize)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		RectTransform component = ((Component)this).GetComponent<RectTransform>();
		if (!((Object)(object)component == (Object)null))
		{
			component.sizeDelta = pSize;
		}
	}

	protected virtual void Init()
	{
		if (!Initialized)
		{
			Initialized = true;
		}
	}
}
