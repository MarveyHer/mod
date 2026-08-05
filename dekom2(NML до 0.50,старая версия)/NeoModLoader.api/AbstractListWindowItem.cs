using UnityEngine;

namespace NeoModLoader.api;

public abstract class AbstractListWindowItem<TItem> : MonoBehaviour
{
	public abstract void Setup(TItem pObject);
}
