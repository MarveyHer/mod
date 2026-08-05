using System;
using System.Linq;
using UnityEngine;

namespace NCMS.Utils;

[Obsolete("Compatible Layer will not be maintained and be removed in the future")]
public class GameObjects
{
	[Obsolete("Use ResourcesFinder.FindResources<T>(string name) instead")]
	public static GameObject FindEvenInactive(string Name)
	{
		GameObject[] source = Resources.FindObjectsOfTypeAll<GameObject>();
		return source.FirstOrDefault((GameObject obj) => string.Equals(((Object)obj).name, Name, StringComparison.CurrentCultureIgnoreCase));
	}
}
