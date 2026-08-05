using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyMetaBanners : ArmyElement, IBaseMetaBanners
{
	[SerializeField]
	private CityBanner _banner_city;

	[SerializeField]
	private AllianceBanner _banner_alliance;

	[SerializeField]
	private KingdomBanner _banner_kingdom;

	protected List<MetaBannerElement> banners = new List<MetaBannerElement>();

	private const float DELAY = 0.025f;

	private int _visible_banners;

	public int visible_banners => _visible_banners;

	protected override void Awake()
	{
		base.Awake();
		banners.Add(new MetaBannerElement
		{
			banner = _banner_kingdom,
			check = () => base.army.hasKingdom(),
			nano = () => base.army.getKingdom()
		});
		banners.Add(new MetaBannerElement
		{
			banner = _banner_alliance,
			check = () => base.army.hasKingdom() && base.army.getKingdom().hasAlliance(),
			nano = () => base.army.getKingdom().getAlliance()
		});
		banners.Add(new MetaBannerElement
		{
			banner = _banner_city,
			check = () => base.army.hasCity(),
			nano = () => base.army.getCity()
		});
		((IBaseMetaBanners)this).enableClickAnimation();
	}

	protected override IEnumerator showContent()
	{
		banners.Sort((MetaBannerElement x, MetaBannerElement y) => x.banner.transform.GetSiblingIndex().CompareTo(y.banner.transform.GetSiblingIndex()));
		yield return new WaitForSecondsRealtime(0.025f);
		foreach (MetaBannerElement tBannerAsset in banners)
		{
			if (tBannerAsset.check())
			{
				track_objects.Add(tBannerAsset.nano());
				metaBannerShow(tBannerAsset);
			}
		}
	}

	protected override void clear()
	{
		base.clear();
		_visible_banners = 0;
		foreach (MetaBannerElement tBannerAsset in banners)
		{
			metaBannerHide(tBannerAsset);
		}
	}

	public void metaBannerShow(MetaBannerElement pAsset)
	{
		pAsset.banner.gameObject.SetActive(value: true);
		pAsset.banner.load(pAsset.nano());
		_visible_banners++;
	}

	public void metaBannerHide(MetaBannerElement pAsset)
	{
		if (pAsset.banner.gameObject.activeSelf)
		{
			pAsset.banner.gameObject.SetActive(value: false);
		}
	}

	public IReadOnlyCollection<MetaBannerElement> getBanners()
	{
		return banners;
	}
}
