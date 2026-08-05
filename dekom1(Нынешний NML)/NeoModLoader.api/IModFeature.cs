namespace NeoModLoader.api;

public interface IModFeature
{
	IModFeatureManager ModFeatureManager { get; set; }

	ModFeatureRequirementList RequiredModFeatures { get; }

	ModFeatureRequirementList OptionalModFeatures { get; }

	bool Init();

	bool PostInit();
}
