public class ActorSelectedMetaBanners : UnitMetaBanners, ISelectedTabBanners<Actor>
{
	public void update(Actor pActor)
	{
		setActor(pActor);
		clear();
		foreach (MetaBannerElement tBannerAsset in _banners)
		{
			if (tBannerAsset.check())
			{
				metaBannerShow(tBannerAsset);
			}
		}
	}

	protected override void checkSetActor()
	{
	}

	protected override void OnEnable()
	{
	}

	protected override void checkSetWindow()
	{
	}

	public int countVisibleBanners()
	{
		return base.visible_banners;
	}
}
