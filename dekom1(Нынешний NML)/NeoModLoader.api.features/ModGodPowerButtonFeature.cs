using NeoModLoader.General;
using UnityEngine;

namespace NeoModLoader.api.features;

public abstract class ModGodPowerButtonFeature<TGodPowerFeature, TPowersTabFeature> : ModButtonFeature<TPowersTabFeature> where TGodPowerFeature : ModAssetFeature<GodPower> where TPowersTabFeature : ModPowerTabFeature
{
	public override ModFeatureRequirementList RequiredModFeatures => base.RequiredModFeatures + typeof(TGodPowerFeature);

	public abstract string SpritePath { get; }

	protected override PowerButton InitObject()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		return PowerButtonCreator.CreateGodPowerButton(GetFeature<TGodPowerFeature>().Object.id, Resources.Load<Sprite>(SpritePath), ((Component)base.Tab).transform);
	}
}
