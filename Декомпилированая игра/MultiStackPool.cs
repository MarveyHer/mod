using System;
using System.Collections.Generic;

public class MultiStackPool<T> where T : new()
{
	private Dictionary<Type, StackPool<T>> _pools = new Dictionary<Type, StackPool<T>>();

	public U get<U>() where U : T, new()
	{
		Type tType = typeof(U);
		if (!_pools.TryGetValue(tType, out var tPool))
		{
			tPool = new StackPool<T>();
			_pools.Add(tType, tPool);
		}
		return tPool.get<U>();
	}

	public void release(T pObject)
	{
		Type tType = pObject.GetType();
		if (_pools.TryGetValue(tType, out var tPool))
		{
			tPool.release(pObject);
		}
	}

	public void clear()
	{
		foreach (StackPool<T> value in _pools.Values)
		{
			value.clear();
		}
		_pools.Clear();
	}
}
