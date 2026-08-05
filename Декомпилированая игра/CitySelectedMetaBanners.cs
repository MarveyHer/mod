public class CitySelectedMetaBanners : CityMetaBanners, ISelectedTabBanners<City>
{
	public void update(City pCity)
	{
		meta_object = pCity;
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
