using UnityEngine;

public class CultureSelectedOnomasticsNames : OnomasticsNameGenerator
{
	[SerializeField]
	private GameObject _main_container;

	[SerializeField]
	private GameObject _separator;

	private Culture _culture;

	private string _last_template;

	private MetaType _meta_type => MetaType.Unit;

	public void load(Culture pCulture)
	{
		string tTemplate = getTemplateString(pCulture);
		if (_culture != pCulture || !(tTemplate == _last_template))
		{
			_culture = pCulture;
			_last_template = tTemplate;
			clickRegenerate();
		}
	}

	public void update()
	{
		bool tIsRekt = _culture.isRekt();
		_main_container.SetActive(!tIsRekt);
		_separator.SetActive(!tIsRekt);
		if (!tIsRekt)
		{
			OnomasticsData tData = _culture.getOnomasticData(_meta_type);
			updateNameGeneration(tData);
		}
	}

	public void click()
	{
		clickRegenerate();
	}

	private string getTemplateString(Culture pCulture)
	{
		return pCulture.getOnomasticData(_meta_type).getShortTemplate();
	}
}
