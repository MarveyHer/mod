using UnityEngine;

public class KnowledgeWindow : TabbedWindow
{
	[SerializeField]
	private Transform _elements_parent;

	[SerializeField]
	private KnowledgeElement _element_prefab;

	[SerializeField]
	private StatBar _progress_bar;

	[SerializeField]
	private CubeOverview _cube_overview_big;

	[SerializeField]
	private WindowMetaTab _cube_tab;

	protected override void create()
	{
		base.create();
		foreach (KnowledgeAsset tAsset in AssetManager.knowledge_library.list)
		{
			if (tAsset.show_in_knowledge_window)
			{
				KnowledgeElement knowledgeElement = Object.Instantiate(_element_prefab, _elements_parent);
				knowledgeElement.setAsset(tAsset);
				knowledgeElement.setCube(_cube_overview_big, _cube_tab);
			}
		}
	}

	private void OnEnable()
	{
		int tValue = 0;
		int tMax = 0;
		foreach (KnowledgeAsset tAsset in AssetManager.knowledge_library.list)
		{
			if (tAsset.show_in_knowledge_window)
			{
				tValue += tAsset.countUnlockedByPlayer();
				tMax += tAsset.countTotal();
			}
		}
		_progress_bar.setBar(tValue, tMax, "/" + tMax.ToText());
	}
}
