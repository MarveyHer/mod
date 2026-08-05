using UnityEngine;
using UnityEngine.UI;

public class AchievementGoodie : MonoBehaviour
{
	[SerializeField]
	private Image _icon;

	[SerializeField]
	private Text _name;

	public void load(BaseUnlockableAsset pAsset, bool pUnlocked)
	{
		if (pUnlocked)
		{
			loadUnlocked(pAsset);
		}
		else
		{
			loadLocked(pAsset);
		}
	}

	private void loadLocked(BaseUnlockableAsset pAsset)
	{
		_icon.sprite = pAsset.getSprite();
		_icon.color = Toolbox.color_black;
	}

	private void loadUnlocked(BaseUnlockableAsset pAssets)
	{
		_icon.sprite = pAssets.getSprite();
		_name.GetComponent<LocalizedText>().setKeyAndUpdate(pAssets.getLocaleID());
		if (!(pAssets is ActorAsset tActorAsset))
		{
			if (pAssets is BaseAugmentationAsset tAugmentation)
			{
				BaseCategoryAsset tCategory = tAugmentation.getGroup();
				_name.color = tCategory?.getColor() ?? Toolbox.color_white;
			}
			else
			{
				_name.color = Toolbox.color_white;
			}
		}
		else
		{
			KingdomAsset tKingdomAsset = AssetManager.kingdoms.get(tActorAsset.kingdom_id_wild);
			_name.color = tKingdomAsset.default_kingdom_color.getColorText();
		}
	}
}
