using NeoModLoader.General;
using UnityEngine;
using UnityEngine.Events;

namespace NeoModLoader.api.features;

public abstract class ModWindowButtonFeature<TWindowFeature, TPowersTabFeature> : ModButtonFeature<TPowersTabFeature> where TWindowFeature : ModObjectFeature<ScrollWindow> where TPowersTabFeature : ModPowerTabFeature
{
	public override ModFeatureRequirementList RequiredModFeatures => base.RequiredModFeatures + typeof(TWindowFeature);

	protected ScrollWindow Window => GetFeature<TWindowFeature>();

	public abstract UnityAction WindowOpenAction { get; }

	public abstract string SpritePath { get; }

	protected override PowerButton InitObject()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return PowerButtonCreator.CreateSimpleButton(((Object)Window).name, WindowOpenAction, Resources.Load<Sprite>(SpritePath), ((Component)base.Tab).transform);
	}
}
