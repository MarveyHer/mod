using UnityEngine;
using UnityEngine.UI;

public class MapSizeTextUpdater : MonoBehaviour
{
	public Text text_counter;

	private void Update()
	{
		updateVars();
	}

	private void updateVars()
	{
		Text component = GetComponent<Text>();
		string tTextContent = LocalizedTextManager.getText(AssetManager.map_sizes.get(Config.customMapSize).getLocaleID());
		component.text = tTextContent.ToUpper();
		component.GetComponent<LocalizedText>().checkSpecialLanguages();
		string[] tMapSizes = MapSizeLibrary.getSizes();
		int tCurIndex = tMapSizes.IndexOf(Config.customMapSize);
		text_counter.text = tCurIndex + 1 + "/" + tMapSizes.Length;
	}
}
