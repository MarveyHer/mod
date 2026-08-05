using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorruptedTreesManager : MonoBehaviour
{
	private string currentString = "";

	private List<CorruptedTreeObject> _objects;

	public GameObject win_icon;

	public void Start()
	{
		_objects = new List<CorruptedTreeObject>();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			CorruptedTreeObject tObj = base.transform.GetChild(i).GetComponent<CorruptedTreeObject>();
			_objects.Add(tObj);
			tObj.transform.GetChild(0).gameObject.SetActive(value: false);
			tObj.GetComponent<Button>().onClick.AddListener(delegate
			{
				click(tObj);
			});
		}
		win_icon.gameObject.SetActive(value: false);
	}

	public void click(CorruptedTreeObject pObject)
	{
		if (!pObject.used)
		{
			pObject.used = true;
			pObject.transform.GetChild(0).gameObject.SetActive(value: true);
			pObject.GetComponent<Image>().enabled = false;
			string tWinCheck = "162534" ?? "";
			currentString += pObject.transform.name;
			tWinCheck = tWinCheck.Substring(0, currentString.Length);
			if ("162534".CompareTo(tWinCheck) == 0)
			{
				win();
			}
			else if (!tWinCheck.Contains(currentString))
			{
				lost();
			}
		}
	}

	private void win()
	{
		win_icon.gameObject.SetActive(value: true);
		AchievementLibrary.the_corrupted_trees.check();
	}

	private void lost()
	{
		foreach (CorruptedTreeObject @object in _objects)
		{
			@object.GetComponent<UiCreature>().click();
		}
	}
}
