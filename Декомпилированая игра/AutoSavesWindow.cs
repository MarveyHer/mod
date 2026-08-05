using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoSavesWindow : MonoBehaviour
{
	[SerializeField]
	private AutoSaveElement _element_prefab;

	private List<AutoSaveElement> elements = new List<AutoSaveElement>();

	private Queue<AutoSaveData> _showQueue = new Queue<AutoSaveData>();

	[SerializeField]
	private VerticalLayoutGroup _elements_parent;

	private float _timer;

	private void OnEnable()
	{
		prepareList();
		prepareSaves();
	}

	private void prepareSaves()
	{
		_showQueue.Clear();
		using ListPool<AutoSaveData> tDatas = AutoSaveManager.getAutoSaves();
		for (int i = 0; i < tDatas.Count; i++)
		{
			AutoSaveData tData = tDatas[i];
			_showQueue.Enqueue(tData);
		}
	}

	private void Update()
	{
		if (_timer > 0f)
		{
			_timer -= Time.deltaTime;
			return;
		}
		_timer = 0.02f;
		showNextItemFromQueue();
	}

	private void showNextItemFromQueue()
	{
		if (_showQueue.Count != 0)
		{
			AutoSaveData tData = _showQueue.Dequeue();
			renderMapElement(tData);
		}
	}

	private void prepareList()
	{
		foreach (AutoSaveElement element in elements)
		{
			Object.Destroy(element.gameObject);
		}
		elements.Clear();
	}

	private void renderMapElement(AutoSaveData pData)
	{
		AutoSaveElement tElement = Object.Instantiate(_element_prefab, _elements_parent.transform);
		elements.Add(tElement);
		tElement.load(pData);
	}
}
