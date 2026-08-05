using System;
using System.Collections.Generic;

namespace NeoModLoader.api;

public abstract class ModFeature : IModFeature
{
	public IModFeatureManager ModFeatureManager { get; set; }

	public virtual ModFeatureRequirementList RequiredModFeatures { get; } = new List<Type>();

	public virtual ModFeatureRequirementList OptionalModFeatures { get; } = new List<Type>();

	public abstract bool Init();

	public virtual bool PostInit()
	{
		return true;
	}

	protected bool TryGetFeature<T>(out T feature) where T : ModFeature
	{
		return ModFeatureManager.TryGetFeature<T>(this, out feature);
	}

	protected T GetFeature<T>() where T : ModFeature
	{
		return ModFeatureManager.GetFeature<T>(this);
	}

	protected bool IsFeatureLoaded<T>() where T : ModFeature
	{
		return ModFeatureManager.IsFeatureLoaded<T>();
	}
}
