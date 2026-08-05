using UnityEngine;
using UnityEngine.UI;

public class ButtonGraphScalePlusMinus : MonoBehaviour
{
	public ButtonGraphScaleType button_scale_type;

	private GraphTimeScaleContainer _main_container;

	private GraphController _graph_controller;

	private void Awake()
	{
		GetComponent<Button>().onClick.AddListener(setScale);
		_main_container = GetComponentInParent<GraphTimeScaleContainer>();
		_graph_controller = base.transform.parent.parent.GetComponentInChildren<GraphController>();
	}

	public void setScale()
	{
		if (button_scale_type == ButtonGraphScaleType.Plus)
		{
			_main_container.timeScaleMinus();
		}
		else
		{
			_main_container.timeScalePlus();
		}
		_graph_controller.forceUpdateGraph();
	}
}
