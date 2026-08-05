using UnityEngine;

public static class GameObjectExtensions
{
	public static T AddOrGetComponent<T>(this GameObject pGameObject) where T : Component
	{
		if (!pGameObject.TryGetComponent<T>(out var tOutComponent))
		{
			return pGameObject.AddComponent<T>();
		}
		return tOutComponent;
	}

	public static bool HasComponent<T>(this GameObject pGameObject)
	{
		T component;
		return pGameObject.TryGetComponent<T>(out component);
	}

	public static bool HasComponent<T>(this Component pComponent)
	{
		return pComponent.gameObject.HasComponent<T>();
	}

	public static T AddComponent<T>(this Component pComponent) where T : Component
	{
		return pComponent.gameObject.AddComponent<T>();
	}
}
