public interface IBanner : IBaseMono, IRefreshElement
{
	MetaCustomizationAsset meta_asset { get; }

	MetaTypeAsset meta_type_asset { get; }

	NanoObject GetNanoObject();

	void load(NanoObject pObject);

	string getName();

	void showTooltip();

	void jump(float pSpeed = 0.1f, bool pSilent = false)
	{
	}

	void IRefreshElement.refresh()
	{
		NanoObject tNano = GetNanoObject();
		if (tNano != null && tNano.isAlive())
		{
			load(tNano);
		}
	}
}
