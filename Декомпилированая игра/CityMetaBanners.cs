using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityMetaBanners : CityElement, IBaseMetaBanners
{
	[SerializeField]
	private KingdomBanner _banner_kingdom;

	[SerializeField]
	private ClanBanner _banner_clan;

	[SerializeField]
	private AllianceBanner _banner_alliance;

	[SerializeField]
	private LanguageBanner _banner_language;

	[SerializeField]
	private CultureBanner _banner_culture;

	[SerializeField]
	private ReligionBanner _banner_religion;

	[SerializeField]
	private SubspeciesBanner _banner_subspecies;

	[SerializeField]
	private ArmyBanner _banner_army;

	protected List<MetaBannerElement> banners = new List<MetaBannerElement>();

	private const float DELAY = 0.025f;

	protected int visible_banners;

	protected override void Awake()
	{
		base.Awake();
		banners.Add(new MetaBannerElement
		{
			banner = _banner_kingdom,
			check = () => !base.city.kingdom.isRekt() && !base.city.kingdom.isNeutral(),
			nano = () => base.city.kingdom
		});
		banners.Add(new MetaBannerElement
		{
			banner = _banner_clan,
			check = () => base.city.hasLeader() && base.city.leader.hasClan(),
			nano = () => base.city.leader.clan
		});
		banners.Add(new MetaBannerElement
		{
			banner = _banner_alliance,
			check = () => base.city.kingdom.hasAlliance(),
			nano = () => base.city.kingdom.getAlliance()
		});
		banners.Add(new MetaBannerElement
		{
			banner = _banner_language,
			check = () => base.city.hasLanguage(),
			nano = () => base.city.getLanguage()
		});
		banners.Add(new MetaBannerElement
		{
			banner = _banner_culture,
			check = () => base.city.hasCulture(),
			nano = () => base.city.getCulture()
		});
		banners.Add(new MetaBannerElement
		{
			banner = _banner_religion,
			check = () => base.city.hasReligion(),
			nano = () => base.city.getReligion()
		});
		banners.Add(new MetaBannerElement
		{
			banner = _banner_subspecies,
			check = () => !base.city.getMainSubspecies().isRekt(),
			nano = () => base.city.getMainSubspecies()
		});
		banners.Add(new MetaBannerElement
		{
			banner = _banner_army,
			check = () => base.city.hasArmy(),
			nano = () => base.city.getArmy()
		});
		((IBaseMetaBanners)this).enableClickAnimation();
	}

	protected override IEnumerator showContent()
	{
		banners.Sort((MetaBannerElement x, MetaBannerElement y) => x.banner.transform.GetSiblingIndex().CompareTo(y.banner.transform.GetSiblingIndex()));
		yield return new WaitForSecondsRealtime(0.025f);
		if (base.city.kingdom.isNeutral())
		{
			yield break;
		}
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
		foreach (MetaBannerElement tBannerAsset in banners)
		{
			metaBannerHide(tBannerAsset);
		}
		visible_banners = 0;
	}

	public void metaBannerShow(MetaBannerElement pAsset)
	{
		pAsset.banner.gameObject.SetActive(value: true);
		pAsset.banner.load(pAsset.nano());
		visible_banners++;
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
