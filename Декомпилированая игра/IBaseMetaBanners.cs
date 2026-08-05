using System.Collections.Generic;

public interface IBaseMetaBanners
{
	void metaBannerShow(MetaBannerElement pAsset);

	void metaBannerHide(MetaBannerElement pAsset);

	IReadOnlyCollection<MetaBannerElement> getBanners();

	void enableClickAnimation()
	{
		foreach (MetaBannerElement banner in getBanners())
		{
			banner.banner.GetComponent<TipButton>().showOnClick = true;
		}
	}
}
