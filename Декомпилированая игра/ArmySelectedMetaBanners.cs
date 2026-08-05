public class ArmySelectedMetaBanners : ArmyMetaBanners, ISelectedTabBanners<Army>
{
	public void update(Army pArmy)
	{
		meta_object = pArmy;
		clear();
		foreach (MetaBannerElement tBannerAsset in banners)
		{
			if (tBannerAsset.check())
			{
				metaBannerShow(tBannerAsset);
			}
		}
	}

	protected override void OnEnable()
	{
	}

	public int countVisibleBanners()
	{
		return base.visible_banners;
	}
}
