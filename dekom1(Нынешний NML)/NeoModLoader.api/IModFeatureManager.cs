namespace NeoModLoader.api;

public interface IModFeatureManager : IStagedLoad
{
	bool IsFeatureLoaded<T>() where T : IModFeature;

	T GetFeature<T>(IModFeature askingModFeature) where T : IModFeature;

	bool TryGetFeature<T>(IModFeature askingModFeature, out T feature) where T : IModFeature;

	void InstantiateFeatures();
}
