using UnityEngine;
using UnityEngine.UI;

public class GraphTextTimeScale : MonoBehaviour
{
	public GraphTimeScaleContainer graph_time_scale_container;

	private Text _text;

	public void Awake()
	{
		_text = GetComponent<Text>();
	}

	public void Update()
	{
		string tText = Toolbox.formatNumber(AssetManager.graph_time_library.get(graph_time_scale_container.current_scale.ToString()).max_time_frame);
		tText += graph_time_scale_container.getIndexString();
		_text.text = tText;
	}
}
