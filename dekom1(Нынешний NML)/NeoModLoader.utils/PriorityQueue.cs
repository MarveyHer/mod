using System;
using System.Collections;
using System.Collections.Generic;

namespace NeoModLoader.utils;

public class PriorityQueue<T> : IEnumerable<T>, IEnumerable
{
	private readonly IComparer<T> comparer;

	private T[] heap;

	public int Count { get; private set; }

	public T this[int index]
	{
		get
		{
			if (index > Count || index < 0)
			{
				throw new IndexOutOfRangeException($"{index} / {Count}");
			}
			return heap[index];
		}
	}

	public PriorityQueue(int capacity, IComparer<T> comparer)
	{
		this.comparer = comparer;
		heap = new T[(capacity > 0) ? capacity : 8];
	}

	public IEnumerator<T> GetEnumerator()
	{
		IEnumerator enumerator = heap.GetEnumerator() as IEnumerator<T>;
		return (IEnumerator<T>)(enumerator ?? Array.Empty<T>().GetEnumerator());
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private static int Parent(int i)
	{
		return i - 1 >> 1;
	}

	private static int Left(int i)
	{
		return (i << 1) + 1;
	}

	public T Peek()
	{
		if (Count == 0)
		{
			throw new InvalidOperationException("PriorityQueue is empty");
		}
		return heap[0];
	}

	public int Enqueue(T x)
	{
		if (Count == heap.Length)
		{
			Array.Resize(ref heap, Count << 1);
		}
		Count++;
		heap[Count - 1] = x;
		return SiftUp(Count - 1);
	}

	private int SiftUp(int i)
	{
		T val = heap[i];
		while (i > 0)
		{
			int num = Parent(i);
			if (comparer.Compare(val, heap[num]) >= 0)
			{
				break;
			}
			heap[i] = heap[num];
			i = num;
		}
		heap[i] = val;
		return i;
	}

	public T Dequeue()
	{
		if (Count == 0)
		{
			throw new InvalidOperationException("PriorityQueue is empty");
		}
		T result = heap[0];
		T x = heap[Count - 1];
		Count--;
		if (Count != 0)
		{
			SiftDown(0, x);
		}
		return result;
	}

	private void SiftDown(int i, T x)
	{
		while (true)
		{
			int num = Left(i);
			if (num > Count - 1)
			{
				break;
			}
			int num2 = num + 1;
			int num3 = ((num2 > Count - 1 || comparer.Compare(heap[num], heap[num2]) <= 0) ? num : num2);
			if (comparer.Compare(x, heap[num3]) <= 0)
			{
				break;
			}
			heap[i] = heap[num3];
			i = num3;
		}
		heap[i] = x;
	}
}
