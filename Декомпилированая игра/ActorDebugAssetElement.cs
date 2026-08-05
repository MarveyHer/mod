using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorDebugAssetElement : BaseDebugAssetElement<ActorAsset>
{
	public Image icon_left;

	public Image icon_right;

	public ActorDebugAnimationElement animation_idle;

	public ActorDebugAnimationElement animation_walk;

	public ActorDebugAnimationElement animation_swim;

	public Image egg;

	public Sprite no_animation_baby;

	private AnimationContainerUnit _animation_container_adult;

	private AnimationContainerUnit _animation_container_baby;

	private int _phenotype_index;

	private int _phenotype_shade_id;

	public override void setData(ActorAsset pAsset)
	{
		asset = pAsset;
		Sprite tIcon = asset.getSpriteIcon();
		icon_left.sprite = tIcon;
		icon_right.sprite = tIcon;
		title.text = asset.id;
		egg.gameObject.SetActive(value: true);
		initAnimations();
		initStats();
	}

	protected override void initAnimations()
	{
		if (asset.hasDefaultEggForm())
		{
			egg.gameObject.SetActive(value: true);
			SubspeciesTrait tEggAsset = AssetManager.subspecies_traits.get(asset.getDefaultEggID());
			egg.sprite = SpriteTextureLoader.getSprite(tEggAsset.sprite_path);
		}
		else
		{
			egg.gameObject.SetActive(value: false);
		}
		animation_idle.setData(asset);
		animation_walk.setData(asset);
		animation_swim.setData(asset);
		if (asset.special)
		{
			Sprite tIcon = icon_left.sprite;
			animation_idle.adult.image.sprite = tIcon;
			animation_walk.adult.image.sprite = tIcon;
			animation_swim.adult.image.sprite = tIcon;
			egg.gameObject.SetActive(value: false);
			stopAnimations();
			return;
		}
		if (asset.use_phenotypes)
		{
			_phenotype_index = AssetManager.phenotype_library.get(asset.debug_phenotype_colors).phenotype_index;
			_phenotype_shade_id = Actor.getRandomPhenotypeShade();
		}
		else
		{
			_phenotype_index = 0;
			_phenotype_shade_id = 0;
		}
		if (asset.is_boat)
		{
			AnimationDataBoat tBoatAnim = ActorAnimationLoader.loadAnimationBoat(asset.id);
			setAnimation(DynamicActorSpriteCreatorUI.getBoatAnimation(tBoatAnim), animation_idle, asset.animation_idle_speed, pIsAdult: true, pHasAnimation: true, pShouldHaveAnimation: true);
			setAnimation(tBoatAnim.normal, animation_walk, asset.animation_walk_speed, pIsAdult: true, pHasAnimation: true, pShouldHaveAnimation: true);
			setAnimation(tBoatAnim.broken, animation_swim, asset.animation_swim_speed, pIsAdult: true, pHasAnimation: true, pShouldHaveAnimation: true);
			return;
		}
		string[] array = asset.animation_idle;
		bool tShouldIdle = array != null && array.Length != 0;
		string[] array2 = asset.animation_walk;
		bool tShouldWalk = array2 != null && array2.Length != 0;
		string[] array3 = asset.animation_swim;
		bool tShouldSwim = array3 != null && array3.Length != 0;
		_animation_container_adult = DynamicActorSpriteCreatorUI.getContainerForUI(asset, pAdult: true, asset.texture_asset);
		setAnimation(_animation_container_adult.idle, animation_idle, asset.animation_idle_speed, pIsAdult: true, _animation_container_adult.has_idle, tShouldIdle);
		setAnimation(_animation_container_adult.walking, animation_walk, asset.animation_walk_speed, pIsAdult: true, _animation_container_adult.has_walking, tShouldWalk);
		List<string> default_subspecies_traits = asset.default_subspecies_traits;
		if (default_subspecies_traits != null && !default_subspecies_traits.Contains("hovering") && !asset.flying)
		{
			setAnimation(_animation_container_adult.swimming, animation_swim, asset.animation_swim_speed, pIsAdult: true, _animation_container_adult.has_swimming, tShouldSwim);
		}
		else
		{
			animation_swim.adult.image.color = Color.clear;
			animation_swim.adult.enabled = false;
		}
		if (asset.has_baby_form)
		{
			_animation_container_baby = DynamicActorSpriteCreatorUI.getContainerForUI(asset, pAdult: false, asset.texture_asset);
			setAnimation(_animation_container_baby.idle, animation_idle, asset.animation_idle_speed, pIsAdult: false, _animation_container_baby.has_idle, tShouldIdle);
			setAnimation(_animation_container_baby.walking, animation_walk, asset.animation_walk_speed, pIsAdult: false, _animation_container_baby.has_walking, tShouldWalk);
			if (!asset.default_subspecies_traits.Contains("hovering") && !asset.flying)
			{
				setAnimation(_animation_container_baby.swimming, animation_swim, asset.animation_swim_speed, pIsAdult: false, _animation_container_baby.has_swimming, tShouldSwim);
				return;
			}
			animation_swim.baby.image.color = Color.clear;
			animation_swim.baby.enabled = false;
		}
	}

	public override void update()
	{
		if (base.gameObject.activeSelf)
		{
			animation_idle.update();
			animation_walk.update();
			animation_swim.update();
		}
	}

	public override void stopAnimations()
	{
		animation_idle.stopAnimations();
		animation_walk.stopAnimations();
		animation_swim.stopAnimations();
	}

	public override void startAnimations()
	{
		animation_idle.startAnimations();
		animation_walk.startAnimations();
		animation_swim.startAnimations();
	}

	private void setAnimation(ActorAnimation pAnimation, ActorDebugAnimationElement pElement, float pAnimationSpeed, bool pIsAdult, bool pHasAnimation, bool pShouldHaveAnimation)
	{
		SpriteAnimation tSpriteAnimation = (pIsAdult ? pElement.adult : pElement.baby);
		if (!pShouldHaveAnimation)
		{
			tSpriteAnimation.image.color = Color.clear;
			tSpriteAnimation.image.sprite = null;
			tSpriteAnimation.enabled = false;
			return;
		}
		if (!pHasAnimation)
		{
			tSpriteAnimation.image.color = Color.white;
			tSpriteAnimation.image.sprite = (pIsAdult ? no_animation : no_animation_baby);
			tSpriteAnimation.enabled = false;
			return;
		}
		AnimationContainerUnit tContainer = (pIsAdult ? _animation_container_adult : _animation_container_baby);
		Sprite[] tReadyFrames = new Sprite[pAnimation.frames.Length];
		ColorAsset tKingdomColor = AssetManager.kingdoms.get(asset.kingdom_id_wild).debug_color_asset;
		for (int i = 0; i < pAnimation.frames.Length; i++)
		{
			tReadyFrames[i] = DynamicActorSpriteCreatorUI.getUnitSpriteForUI(asset, pAnimation.frames[i], tContainer, pIsAdult, AssetsDebugManager.actors_sex, _phenotype_index, _phenotype_shade_id, tKingdomColor, 0L, 0);
		}
		tSpriteAnimation.enabled = true;
		tSpriteAnimation.setFrames(tReadyFrames);
		tSpriteAnimation.timeBetweenFrames = 1f / pAnimationSpeed;
		pElement.startAnimations();
	}

	protected override void initStats()
	{
		base.initStats();
		BaseStats tOverviewStats = asset.getStatsForOverview();
		showStat("health", tOverviewStats["health"]);
		showStat("damage", tOverviewStats["damage"]);
		showStat("speed", tOverviewStats["speed"]);
		showStat("lifespan", tOverviewStats["lifespan"]);
	}

	protected override void showAssetWindow()
	{
		base.showAssetWindow();
		ScrollWindow.showWindow("actor_asset");
	}
}
