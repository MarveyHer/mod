public class KingdomSelectedMetaBanners : KingdomMetaBanners, ISelectedTabBanners<Kingdom>
{
	public void update(Kingdom pKingdom)
	{
		meta_object = pKingdom;
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
		return visible_banners;
	}
}
