using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

[Serializable]
public sealed class ListPool<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>, IDisposable
{
	public struct Enumerator(T[] source, int itemsCount) : IEnumerator<T>, IEnumerator, IDisposable
	{
		private readonly T[] _source = source;

		private readonly int _itemsCount = itemsCount;

		private int _index = -1;

		public readonly ref T Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			[return: MaybeNull]
			get
			{
				return ref _source[_index];
			}
		}

		readonly T IEnumerator<T>.Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			[return: MaybeNull]
			get
			{
				return _source[_index];
			}
		}

		readonly object? IEnumerator.Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			[return: MaybeNull]
			get
			{
				return _source[_index];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			return ++_index < _itemsCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			_index = -1;
		}

		public readonly void Dispose()
		{
		}
	}

	private const int MinimumCapacity = 32;

	private T[] _items;

	[NonSerialized]
	private object? _syncRoot;

	private static readonly ArrayPool<T> _arrayPool = ArrayPool<T>.Shared;

	private static readonly bool _should_clean = !typeof(T).IsValueType && typeof(string) != typeof(T);

	public int Capacity => _items.Length;

	int ICollection.Count => Count;

	bool IList.IsFixedSize => false;

	bool ICollection.IsSynchronized => false;

	bool IList.IsReadOnly => false;

	object ICollection.SyncRoot
	{
		get
		{
			if (_syncRoot == null)
			{
				Interlocked.CompareExchange<object>(ref _syncRoot, new object(), (object)null);
			}
			return _syncRoot;
		}
	}

	object IList.this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: MaybeNull]
		get
		{
			if (index >= Count)
			{
				throw new IndexOutOfRangeException("index");
			}
			return _items[index];
		}
		set
		{
			if (index >= Count)
			{
				throw new IndexOutOfRangeException("index");
			}
			if (value is T valueAsTSource)
			{
				_items[index] = valueAsTSource;
				return;
			}
			throw new ArgumentException($"Wrong value type. Expected {typeof(T)}, got: '{value}'.", "value");
		}
	}

	public int Count { get; private set; }

	public bool IsReadOnly => false;

	public T this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: MaybeNull]
		get
		{
			if (index >= Count)
			{
				throw new IndexOutOfRangeException("index");
			}
			return _items[index];
		}
		set
		{
			if (index >= Count)
			{
				throw new IndexOutOfRangeException("index");
			}
			_items[index] = value;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ListPool()
	{
		_items = _arrayPool.Rent(32);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ListPool(int capacity)
	{
		_items = _arrayPool.Rent((capacity < 32) ? 32 : capacity);
	}

	public ListPool(ICollection<T> collection)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		T[] buffer = _arrayPool.Rent((collection.Count > 32) ? collection.Count : 32);
		collection.CopyTo(buffer, 0);
		_items = buffer;
		Count = collection.Count;
	}

	public ListPool(IEnumerable<T> source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int _count = Enumerable.Count(source);
		_items = _arrayPool.Rent((_count > 32) ? _count : 32);
		T[] buffer = _items;
		Count = 0;
		int count = 0;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (count < buffer.Length)
			{
				buffer[count] = enumerator.Current;
				count++;
				continue;
			}
			Count = count;
			AddWithResize(enumerator.Current);
			count++;
			buffer = _items;
		}
		Count = count;
	}

	public ListPool(T[] source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int capacity = ((source.Length > 32) ? source.Length : 32);
		T[] buffer = _arrayPool.Rent(capacity);
		source.CopyTo(buffer, 0);
		_items = buffer;
		Count = source.Length;
	}

	public ListPool(ReadOnlySpan<T> source)
	{
		int capacity = ((source.Length > 32) ? source.Length : 32);
		T[] buffer = _arrayPool.Rent(capacity);
		source.CopyTo(buffer);
		_items = buffer;
		Count = source.Length;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose()
	{
		if (_should_clean)
		{
			Clear();
		}
		Count = 0;
		_arrayPool.Return(_items);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	int IList.Add(object item)
	{
		if (item is T itemAsTSource)
		{
			Add(itemAsTSource);
			return Count - 1;
		}
		throw new ArgumentException($"Wrong value type. Expected {typeof(T)}, got: '{item}'.", "item");
	}

	bool IList.Contains(object item)
	{
		if (item is T itemAsTSource)
		{
			return Contains(itemAsTSource);
		}
		throw new ArgumentException($"Wrong value type. Expected {typeof(T)}, got: '{item}'.", "item");
	}

	int IList.IndexOf(object item)
	{
		if (item is T itemAsTSource)
		{
			return IndexOf(itemAsTSource);
		}
		throw new ArgumentException($"Wrong value type. Expected {typeof(T)}, got: '{item}'.", "item");
	}

	void IList.Remove(object item)
	{
		if (item is T itemAsTSource)
		{
			Remove(itemAsTSource);
		}
		else if (item != null)
		{
			throw new ArgumentException($"Wrong value type. Expected {typeof(T)}, got: '{item}'.", "item");
		}
	}

	void IList.Insert(int index, object item)
	{
		if (item is T itemAsTSource)
		{
			Insert(index, itemAsTSource);
			return;
		}
		throw new ArgumentException($"Wrong value type. Expected {typeof(T)}, got: '{item}'.", "item");
	}

	void ICollection.CopyTo(Array array, int arrayIndex)
	{
		Array.Copy(_items, 0, array, arrayIndex, Count);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(T item)
	{
		T[] buffer = _items;
		int count = Count;
		if (count < buffer.Length)
		{
			buffer[count] = item;
			Count = count + 1;
		}
		else
		{
			AddWithResize(item);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear()
	{
		if (Count > 0)
		{
			Array.Clear(_items, 0, Count);
			Count = 0;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear(int pAfterIndex)
	{
		if (Count > 0 && pAfterIndex < Count)
		{
			Array.Clear(_items, pAfterIndex, Count - pAfterIndex);
			Count = pAfterIndex;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(T item)
	{
		return IndexOf(item) > -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int IndexOf(T item)
	{
		return Array.IndexOf(_items, item, 0, Count);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void CopyTo(T[] array, int arrayIndex)
	{
		Array.Copy(_items, 0, array, arrayIndex, Count);
	}

	public bool Remove(T item)
	{
		if (item == null)
		{
			return false;
		}
		int index = IndexOf(item);
		if (index == -1)
		{
			return false;
		}
		RemoveAt(index);
		return true;
	}

	public void Insert(int index, T item)
	{
		int count = Count;
		T[] buffer = _items;
		if (buffer.Length == count)
		{
			int newCapacity = count * 2;
			EnsureCapacity(newCapacity);
			buffer = _items;
		}
		if (index < count)
		{
			Array.Copy(buffer, index, buffer, index + 1, count - index);
			buffer[index] = item;
			Count++;
			return;
		}
		if (index == count)
		{
			buffer[index] = item;
			Count++;
			return;
		}
		throw new IndexOutOfRangeException("index");
	}

	public void RemoveAt(int index)
	{
		int count = Count;
		T[] buffer = _items;
		if (index >= count)
		{
			throw new IndexOutOfRangeException("index");
		}
		count--;
		Array.Copy(buffer, index + 1, buffer, index, count - index);
		if (_should_clean)
		{
			buffer[count] = default(T);
		}
		Count = count;
	}

	public int RemoveAll(Predicate<T> match)
	{
		int count = Count;
		T[] buffer = _items;
		int freeIndex;
		for (freeIndex = 0; freeIndex < count && !match(buffer[freeIndex]); freeIndex++)
		{
		}
		if (freeIndex >= count)
		{
			return 0;
		}
		int current = freeIndex + 1;
		while (current < count)
		{
			for (; current < count && match(buffer[current]); current++)
			{
			}
			if (current < count)
			{
				buffer[freeIndex++] = buffer[current++];
			}
		}
		if (_should_clean)
		{
			Array.Clear(buffer, freeIndex, count - freeIndex);
		}
		int result = count - freeIndex;
		Count = freeIndex;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return new Enumerator(_items, Count);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator()
	{
		return new Enumerator(_items, Count);
	}

	public void AddRange(Span<T> items)
	{
		int count = Count;
		T[] buffer = _items;
		if (buffer.Length - items.Length - count < 0)
		{
			EnsureCapacity(buffer.Length + items.Length);
			buffer = _items;
		}
		items.CopyTo(MemoryExtensions.AsSpan(buffer).Slice(count));
		Count += items.Length;
	}

	public void AddRange(ReadOnlySpan<T> items)
	{
		int count = Count;
		T[] buffer = _items;
		if (buffer.Length - items.Length - count < 0)
		{
			EnsureCapacity(buffer.Length + items.Length);
			buffer = _items;
		}
		items.CopyTo(MemoryExtensions.AsSpan(buffer).Slice(count));
		Count += items.Length;
	}

	public void AddRange(T[] items)
	{
		int count = Count;
		T[] buffer = _items;
		if (buffer.Length - items.Length - count < 0)
		{
			EnsureCapacity(buffer.Length + items.Length);
			buffer = _items;
		}
		Array.Copy(items, 0, buffer, count, items.Length);
		Count += items.Length;
	}

	public void AddRange(IEnumerable<T> items)
	{
		int count = Count;
		T[] buffer = _items;
		if (items is ICollection<T> collection)
		{
			if (buffer.Length - collection.Count - count < 0)
			{
				EnsureCapacity(buffer.Length + collection.Count);
				buffer = _items;
			}
			collection.CopyTo(buffer, count);
			Count += collection.Count;
			return;
		}
		foreach (T item in items)
		{
			if (count < buffer.Length)
			{
				buffer[count] = item;
				count++;
				continue;
			}
			Count = count;
			AddWithResize(item);
			count++;
			buffer = _items;
		}
		Count = count;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<T> AsSpan()
	{
		return MemoryExtensions.AsSpan(_items, 0, Count);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Memory<T> AsMemory()
	{
		return MemoryExtensions.AsMemory(_items, 0, Count);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void AddWithResize(T item)
	{
		ArrayPool<T> arrayPool = _arrayPool;
		T[] oldBuffer = _items;
		T[] newBuffer = arrayPool.Rent(oldBuffer.Length * 2);
		int count = oldBuffer.Length;
		Array.Copy(oldBuffer, 0, newBuffer, 0, count);
		newBuffer[count] = item;
		_items = newBuffer;
		Count = count + 1;
		arrayPool.Return(oldBuffer, _should_clean);
	}

	public void EnsureCapacity(int capacity)
	{
		if (capacity > Capacity)
		{
			ArrayPool<T> arrayPool = _arrayPool;
			T[] newBuffer = arrayPool.Rent(capacity);
			T[] oldBuffer = _items;
			Array.Copy(oldBuffer, 0, newBuffer, 0, oldBuffer.Length);
			_items = newBuffer;
			arrayPool.Return(oldBuffer, _should_clean);
		}
	}

	public T[] GetRawBuffer()
	{
		return _items;
	}

	public void SetOffsetManually(int offset)
	{
		Count = offset;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator()
	{
		return new Enumerator(_items, Count);
	}

	public void Sort()
	{
		Sort(0, Count, null);
	}

	public void Sort(IComparer<T> comparer)
	{
		Sort(0, Count, comparer);
	}

	public void Sort(int index, int count, IComparer<T> comparer)
	{
		Array.Sort(_items, index, count, comparer);
	}

	public void Sort(Comparison<T> comparison)
	{
		Array.Sort(_items, 0, Count, Comparer<T>.Create(comparison));
	}

	public void Reverse()
	{
		Array.Reverse(_items, 0, Count);
	}

	public void Reverse(int index, int count)
	{
		Array.Reverse(_items, index, count);
	}
}
