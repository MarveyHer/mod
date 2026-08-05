public class ListWindowStatistics : StatisticsRows
{
	public MetaType meta_type;

	protected override void init()
	{
		foreach (StatisticsAsset tAsset in AssetManager.statistics_library.list)
		{
			if (!tAsset.list_window_meta_type.isNone() && meta_type == tAsset.list_window_meta_type)
			{
				addStatRow(tAsset);
			}
		}
	}
}
