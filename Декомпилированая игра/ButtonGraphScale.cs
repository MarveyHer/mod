using UnityEngine;
using UnityEngine.UI;

public class ButtonGraphScale : MonoBehaviour
{
	public Sprite sprite_on;

	public Sprite sprite_off;

	public GraphTimeScale button_scale;

	private GraphTimeScaleContainer _main_container;

	private GraphController _graph_controller;

	private Image _image;

	private void Awake()
	{
		GetComponent<Button>().onClick.AddListener(setScale);
		_image = GetComponent<Image>();
		_main_container = GetComponentInParent<GraphTimeScaleContainer>();
		_graph_controller = base.transform.parent.parent.GetComponentInChildren<GraphController>();
		checkSpriteStatus();
	}

	private void Update()
	{
		checkSpriteStatus();
	}

	private void checkSpriteStatus()
	{
		if (_main_container.current_scale == button_scale)
		{
			_image.sprite = sprite_on;
		}
		else
		{
			_image.sprite = sprite_off;
		}
	}

	public void setScale()
	{
		_main_container.setTimeScale(button_scale);
		_graph_controller.forceUpdateGraph();
	}
}
