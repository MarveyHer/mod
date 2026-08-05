namespace NeoModLoader.api.features;

public abstract class ModObjectFeature<TObject> : ModFeature
{
	public TObject Object { get; private set; }

	public override bool Init()
	{
		TObject val = InitObject();
		if (val == null)
		{
			return false;
		}
		Object = val;
		return true;
	}

	protected abstract TObject InitObject();

	public static implicit operator TObject(ModObjectFeature<TObject> feature)
	{
		return feature.Object;
	}
}
