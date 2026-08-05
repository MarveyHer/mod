using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DebugTool : MonoBehaviour
{
	public const int DT_WIDTH = 126;

	public const int DT_HEIGHT = 60;

	protected ObjectPoolGenericMono<DebugToolTextElement> pool_texts;

	public DebugToolTextElement element_prefab;

	internal int textCount;

	public Dropdown dropdown;

	internal bool sort_order_reversed;

	internal bool sort_by_names;

	internal bool sort_by_values = true;

	internal bool show_averages = true;

	internal bool percentage_slowest;

	internal bool hide_zeroes = true;

	internal bool show_counter = true;

	internal bool show_max = true;

	internal DebugToolState state = DebugToolState.FrameBudget;

	public DebugToolType type;

	internal bool paused;

	internal DebugToolAsset asset;

	[HideInInspector]
	public DebugDropdown active_dropdown;

	private double last_update_timestamp;

	private List<DebugIconOptionAction> list_actions = new List<DebugIconOptionAction>();

	private List<Image> list_icons = new List<Image>();

	private Transform transform_texts;

	private Transform benchmark_icons;

	private string _latest_text;

	private void Awake()
	{
		populateOptions();
		benchmark_icons = base.transform.FindRecursive("Benchmark Icons");
		initButtons();
		initElements();
	}

	private void initElements()
	{
		transform_texts = base.transform.FindRecursive("Texts");
		pool_texts = new ObjectPoolGenericMono<DebugToolTextElement>(element_prefab, transform_texts);
		element_prefab.gameObject.SetActive(value: false);
	}

	private float calculateLineHeight(Text pText)
	{
		Vector2 extents = pText.cachedTextGenerator.rectExtents.size * 0.5f;
		return pText.cachedTextGeneratorForLayout.GetPreferredHeight("A", pText.GetGenerationSettings(extents));
	}

	internal void populateOptions()
	{
		dropdown.ClearOptions();
		List<string> tOptions = new List<string>();
		foreach (DebugToolAsset tAsset in AssetManager.debug_tool_library.list)
		{
			if (tAsset.type == type)
			{
				tOptions.Add(tAsset.name);
			}
		}
		dropdown.AddOptions(tOptions);
		dropdown.onValueChanged.RemoveListener(switchTool);
		dropdown.onValueChanged.AddListener(switchTool);
	}

	public void filterOptions(string pInput)
	{
		DebugDropdownOption[] componentsInChildren = active_dropdown.transform.GetComponentsInChildren<DebugDropdownOption>(includeInactive: true);
		foreach (DebugDropdownOption tOption in componentsInChildren)
		{
			string tName = tOption.title.text;
			if (tName == "Debug option")
			{
				tOption.gameObject.SetActive(value: false);
			}
			else if (!string.IsNullOrEmpty(pInput) && !tName.ToLower().Contains(pInput.ToLower()))
			{
				tOption.gameObject.SetActive(value: false);
			}
			else
			{
				tOption.gameObject.SetActive(value: true);
			}
		}
	}

	private void initButtons()
	{
		newButton("SortByName", clickSortByName, delegate(Image pIcon)
		{
			checkIcon(pIcon, sort_by_names);
		});
		newButton("SortByValues", clickSortByValues, delegate(Image pIcon)
		{
			checkIcon(pIcon, sort_by_values);
		});
		newButton("SortReversed", delegate
		{
			sort_order_reversed = !sort_order_reversed;
		}, delegate(Image pIcon)
		{
			checkIcon(pIcon, sort_order_reversed);
		});
		newButton("ShowAverages", delegate
		{
			show_averages = !show_averages;
		}, delegate(Image pIcon)
		{
			checkIcon(pIcon, isValueAverage());
		});
		newButton("PercentBasedOnSlowest", delegate
		{
			percentage_slowest = !percentage_slowest;
		}, delegate(Image pIcon)
		{
			checkIcon(pIcon, percentage_slowest);
		});
		newButton("HideZeroes", delegate
		{
			hide_zeroes = !hide_zeroes;
		}, delegate(Image pIcon)
		{
			checkIcon(pIcon, hide_zeroes);
		});
		newButton("ShowCounter", delegate
		{
			show_counter = !show_counter;
		}, delegate(Image pIcon)
		{
			checkIcon(pIcon, show_counter);
		});
		newButton("ShowMax", delegate
		{
			show_max = !show_max;
		}, delegate(Image pIcon)
		{
			checkIcon(pIcon, show_max);
		});
		newButton("ShowSeconds", delegate
		{
			state = DebugToolState.Values;
		}, delegate(Image pIcon)
		{
			checkIcon(pIcon, state == DebugToolState.Values);
		});
		newButton("ShowPercentages", delegate
		{
			state = DebugToolState.Percent;
		}, delegate(Image pIcon)
		{
			checkIcon(pIcon, state == DebugToolState.Percent);
		});
		newButton("ShowTimeSpent", delegate
		{
			state = DebugToolState.TimeSpent;
		}, delegate(Image pIcon)
		{
			checkIcon(pIcon, state == DebugToolState.TimeSpent);
		});
		newButton("ShowFrameBudget", delegate
		{
			state = DebugToolState.FrameBudget;
		}, delegate(Image pIcon)
		{
			checkIcon(pIcon, state == DebugToolState.FrameBudget);
		});
		newButton("Paused", delegate
		{
			paused = !paused;
		}, delegate(Image pIcon)
		{
			checkIcon(pIcon, paused);
		});
		newButton("EnableBenchmarks", delegate
		{
			Bench.bench_enabled = !Bench.bench_enabled;
		}, delegate(Image pIcon)
		{
			checkIcon(pIcon, Bench.bench_enabled);
		});
	}

	private void newButton(string pID, UnityAction pAction, DebugIconOptionAction pCheckIcon)
	{
		Transform tButton = base.transform.FindRecursive(pID);
		tButton.GetComponent<Button>().onClick.AddListener(pAction);
		list_actions.Add(pCheckIcon);
		list_icons.Add(tButton.GetComponent<Image>());
	}

	public bool isValueAverage()
	{
		return show_averages;
	}

	public bool isState(DebugToolState pState)
	{
		return state == pState;
	}

	private void updateIcons()
	{
		for (int i = 0; i < list_actions.Count; i++)
		{
			DebugIconOptionAction debugIconOptionAction = list_actions[i];
			Image tImage = list_icons[i];
			debugIconOptionAction(tImage);
		}
	}

	private void checkIcon(Image pImageIcon, bool pValue)
	{
		if (pValue)
		{
			pImageIcon.color = Color.white;
		}
		else
		{
			pImageIcon.color = Toolbox.color_transparent_grey;
		}
	}

	private void switchTool(int pIndex)
	{
		string tID = dropdown.options[pIndex].text;
		DebugToolAsset tAsset = AssetManager.debug_tool_library.get(tID);
		setAsset(tAsset);
	}

	public void setAsset(DebugToolAsset pAsset)
	{
		asset = pAsset;
		type = asset.type;
		benchmark_icons.gameObject.SetActive(asset.show_benchmark_buttons);
		if (asset.action_start != null)
		{
			asset.action_start(this);
		}
	}

	private void Update()
	{
		if (SmoothLoader.isLoading())
		{
			return;
		}
		updateIcons();
		double tCur = World.world.getCurSessionTime();
		if (!(tCur < last_update_timestamp + (double)asset.update_timeout) && !paused)
		{
			if (asset.action_update != null)
			{
				asset.action_update(this);
			}
			clearTexts();
			_ = dropdown.captionText.text;
			last_update_timestamp = tCur;
			if (asset.action_1 != null)
			{
				asset.action_1(this);
			}
			if (asset.action_2 != null)
			{
				asset.action_2(this);
			}
			updateSize();
			pool_texts.disableInactive();
			StartCoroutine(updateSizeAfterFrame());
		}
	}

	public IEnumerator updateSizeAfterFrame()
	{
		yield return CoroutineHelper.wait_for_end_of_frame;
		updateSize();
	}

	private void updateSize()
	{
		float tWidth = LayoutUtility.GetPreferredWidth(transform_texts.GetComponent<RectTransform>()) * 1.2f;
		float tHeight = LayoutUtility.GetPreferredHeight(transform_texts.GetComponent<RectTransform>()) + 40f;
		if (tWidth < 126f)
		{
			tWidth = 126f;
		}
		if (tHeight < 60f)
		{
			tHeight = 60f;
		}
		GetComponent<RectTransform>().sizeDelta = new Vector2(tWidth, tHeight);
	}

	public void clickSortByName()
	{
		sort_by_names = !sort_by_names;
		sort_by_values = !sort_by_names;
	}

	public void clickSortByValues()
	{
		sort_by_values = !sort_by_values;
		sort_by_names = !sort_by_values;
	}

	public int kingdomSorter(Kingdom k1, Kingdom k2)
	{
		return k2.units.Count.CompareTo(k1.units.Count);
	}

	public int citySorter(City c1, City c2)
	{
		return c2.getPopulationPeople().CompareTo(c1.getPopulationPeople());
	}

	internal void setText(string pT1, object pT2, float pBarValue = 0f, bool pShowBar = false, long pCounter = 0L, bool pShowCounter = false, bool pShowMax = false, string pMaxValue = "")
	{
		DebugToolTextElement tElement = pool_texts.getNext();
		string tStringRight = ((pT2 == null) ? "-" : pT2.ToString());
		if (pT2 != null)
		{
			if (pShowCounter && show_counter && (asset.split_benchmark || asset.show_last_count))
			{
				tStringRight = pCounter + " | " + tStringRight;
			}
			if (pShowMax)
			{
				tStringRight = pMaxValue + " | " + tStringRight;
			}
		}
		tElement.text_left.text = pT1;
		tElement.text_right.text = tStringRight;
		textCount++;
		if (pShowBar)
		{
			tElement.text_bar.gameObject.SetActive(value: true);
			if (pBarValue > 100f)
			{
				pBarValue = 101f;
			}
			float tWidth = pBarValue * 0.5f;
			tElement.text_bar.GetComponent<RectTransform>().sizeDelta = new Vector2(tWidth, 4.2f);
			if (pBarValue > 70f && pBarValue != 100f)
			{
				tElement.text_bar.color = Toolbox.color_debug_bar_red;
			}
			else
			{
				tElement.text_bar.color = Toolbox.color_debug_bar_blue;
			}
		}
		else
		{
			tElement.text_bar.gameObject.SetActive(value: false);
		}
	}

	internal void setSeparator()
	{
		DebugToolTextElement next = pool_texts.getNext();
		next.text_left.text = string.Empty;
		next.text_right.text = string.Empty;
		next.text_bar.gameObject.SetActive(value: false);
	}

	private void clearTexts()
	{
		textCount = 0;
		pool_texts.clear(pDisable: false);
	}

	public void clickClose()
	{
		Object.Destroy(base.gameObject, 0.01f);
	}

	public void clickDuplicate()
	{
		int tX = (int)base.transform.localPosition.x + 126 + 2;
		int tY = (int)base.transform.localPosition.y;
		DebugConfig.createTool(asset.id, tX, tY);
	}
}
