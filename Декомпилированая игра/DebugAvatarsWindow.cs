using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugAvatarsWindow : MonoBehaviour
{
	private static readonly bool _test_mutations = false;

	private static readonly bool _test_eggs = true;

	private static readonly bool _test_hand_items = false;

	private static readonly bool _test_statuses = false;

	[SerializeField]
	private Transform _avatars_parent;

	[SerializeField]
	private UnitAvatarLoader _avatar_prefab;

	[SerializeField]
	private Image _autotest_button_icon;

	[SerializeField]
	private Sprite _sprite_play;

	[SerializeField]
	private Sprite _sprite_pause;

	private ObjectPoolGenericMono<UnitAvatarLoader> _avatars;

	private List<SubspeciesTrait> _pool_mutations = new List<SubspeciesTrait>();

	private List<SubspeciesTrait> _pool_eggs = new List<SubspeciesTrait>();

	private List<PhenotypeAsset> _pool_phenotype = new List<PhenotypeAsset>();

	private List<AvatarCombineHandItem> _pool_hand_renderers = new List<AvatarCombineHandItem>();

	private List<StatusAsset> _pool_statuses = new List<StatusAsset>();

	private AvatarsCombineDataContainer _combine_data = new AvatarsCombineDataContainer();

	private HashSet<string> _statuses = new HashSet<string>();

	private HashSet<long> _check_collisions = new HashSet<long>();

	private bool _autotest_state;

	private Coroutine _autotest_routine;

	private void Awake()
	{
		init();
	}

	private void init()
	{
		_avatars = new ObjectPoolGenericMono<UnitAvatarLoader>(_avatar_prefab, _avatars_parent);
		preparePools();
	}

	private void OnEnable()
	{
		showAvatars();
	}

	private void OnDisable()
	{
		clear();
	}

	private void clear()
	{
		_avatars.clear();
	}

	private void showAvatars()
	{
		foreach (ActorAsset tAsset in AssetManager.actor_library.list)
		{
			if (tAsset.has_override_sprite || !tAsset.has_sprite_renderer)
			{
				continue;
			}
			SubspeciesTrait tMutation = getRandomMutation();
			bool tIsAdult = getRandomIsAdult();
			ActorSex tSex = getRandomSex();
			ColorAsset tKingdomColor = AssetManager.kingdom_colors_library.list.GetRandom();
			bool tIsUnconscious = getRandomIsUnconscious();
			bool tIsLying = tIsUnconscious || getRandomIsLying();
			bool tIsHovering = getRandomIsHovering();
			bool tIsTouchingLiquid = getRandomIsTouchingLiquid() && !tIsHovering;
			bool tIsImmovable = getRandomIsImmovable();
			AvatarCombineHandItem tHandWeapon = getRandomItemPath();
			List<string> tStatuses = getRandomStatuses(out var tStopIdleAnimation);
			PhenotypeAsset tPhenotype = getRandomPhenotype();
			int tPhenotypeIndex = Actor.getRandomPhenotypeShade();
			SubspeciesTrait tEgg = getRandomEgg();
			bool tIsEgg = !tIsAdult && tEgg != null;
			ActorTextureSubAsset tTextureAsset;
			if (tMutation != null)
			{
				tTextureAsset = tMutation.texture_asset;
				BaseStats tMetaStats = tMutation.base_stats_meta;
				if (!tMetaStats.isEmpty() && tMetaStats.hasTag("always_idle_animation"))
				{
					tStopIdleAnimation = false;
				}
			}
			else
			{
				tTextureAsset = tAsset.texture_asset;
			}
			DynamicActorSpriteCreatorUI.getContainerForUI(tAsset, tIsAdult, tTextureAsset, tMutation, tIsEgg, tEgg);
			ActorAvatarData tData = new ActorAvatarData();
			tData.setData(tAsset, tMutation, tSex, Randy.randomInt(0, int.MaxValue), -1, null, tPhenotype.phenotype_index, tPhenotypeIndex, tKingdomColor, tIsEgg, pIsKing: false, pIsWarrior: false, pIsWise: false, tEgg, tIsAdult, tIsLying, tIsTouchingLiquid, pIsInsideBoat: false, tIsHovering, tIsImmovable, tIsUnconscious, tStopIdleAnimation, tHandWeapon?.hand_renderer, 1, tStatuses, null);
			_avatars.getNext().load(tData);
		}
	}

	private void preparePools()
	{
		foreach (SubspeciesTrait tTrait in AssetManager.subspecies_traits.list)
		{
			if (tTrait.is_mutation_skin)
			{
				_pool_mutations.Add(tTrait);
			}
			if (tTrait.phenotype_egg)
			{
				_pool_eggs.Add(tTrait);
			}
			if (tTrait.phenotype_skin)
			{
				PhenotypeAsset tPhenotype = AssetManager.phenotype_library.get(tTrait.id_phenotype);
				_pool_phenotype.Add(tPhenotype);
			}
		}
		foreach (EquipmentAsset tItem in AssetManager.items.pot_weapon_assets_all)
		{
			_pool_hand_renderers.Add(new AvatarCombineHandItem(tItem));
		}
		foreach (ResourceAsset tResource in AssetManager.resources.list)
		{
			_pool_hand_renderers.Add(new AvatarCombineHandItem(tResource));
		}
		foreach (UnitHandToolAsset tTool in AssetManager.unit_hand_tools.list)
		{
			_pool_hand_renderers.Add(new AvatarCombineHandItem(tTool));
		}
		foreach (StatusAsset tStatus in AssetManager.status.list)
		{
			if (tStatus.need_visual_render)
			{
				_pool_statuses.Add(tStatus);
			}
		}
	}

	private SubspeciesTrait getRandomMutation()
	{
		if (Randy.randomChance(0.75f))
		{
			return null;
		}
		return _pool_mutations.GetRandom();
	}

	private SubspeciesTrait getRandomEgg()
	{
		if (Randy.randomChance(0.9f))
		{
			return null;
		}
		return _pool_eggs.GetRandom();
	}

	private PhenotypeAsset getRandomPhenotype()
	{
		return _pool_phenotype.GetRandom();
	}

	private ActorSex getRandomSex()
	{
		if (Randy.randomChance(0.5f))
		{
			return ActorSex.Male;
		}
		return ActorSex.Female;
	}

	private bool getRandomIsAdult()
	{
		return Randy.randomBool();
	}

	private bool getRandomIsLying()
	{
		return Randy.randomChance(0.2f);
	}

	private bool getRandomIsTouchingLiquid()
	{
		return Randy.randomBool();
	}

	private bool getRandomIsHovering()
	{
		return Randy.randomChance(0.2f);
	}

	private bool getRandomIsImmovable()
	{
		return Randy.randomChance(0.2f);
	}

	private bool getRandomIsUnconscious()
	{
		return Randy.randomChance(0.2f);
	}

	private AvatarCombineHandItem getRandomItemPath()
	{
		if (Randy.randomChance(0.4f))
		{
			return null;
		}
		return _pool_hand_renderers.GetRandom();
	}

	private List<string> getRandomStatuses(out bool pStopIdleAnimation)
	{
		pStopIdleAnimation = false;
		List<string> tStatuses = new List<string>();
		foreach (StatusAsset tStatus in AssetManager.status.list)
		{
			if (tStatus.need_visual_render && !Randy.randomChance(0.95f))
			{
				if (tStatus.base_stats.hasTag("stop_idle_animation"))
				{
					pStopIdleAnimation = true;
				}
				tStatuses.Add(tStatus.id);
			}
		}
		return tStatuses;
	}

	public void toggleAutotest()
	{
		_autotest_state = !_autotest_state;
		if (_autotest_state)
		{
			_autotest_button_icon.sprite = _sprite_pause;
			_autotest_routine = StartCoroutine(autotestRoutine());
		}
		else
		{
			_autotest_button_icon.sprite = _sprite_play;
			StopCoroutine(_autotest_routine);
		}
	}

	private T getFromPool<T>(List<T> pPool, int pGlobalIndex, string pId) where T : class
	{
		int tIndex = _combine_data.getListIndex(pGlobalIndex, pId);
		if (pPool.Count - 1 < tIndex)
		{
			return null;
		}
		return pPool[tIndex];
	}

	private bool getBool(int pGlobalIndex, string pId)
	{
		return _combine_data.getListIndex(pGlobalIndex, pId) == 1;
	}

	private IEnumerator autotestRoutine()
	{
		_combine_data.clear();
		_statuses.Clear();
		_check_collisions.Clear();
		_combine_data.add("tAdult", 2);
		_combine_data.add("tTouchingLiquid", 2);
		_combine_data.add("tLying", 2);
		_combine_data.add("tImmovable", 2);
		_combine_data.add("tUnconscious", 2);
		_combine_data.add("tSex", 2);
		if (_test_mutations)
		{
			_combine_data.add("_pool_mutations", _pool_mutations.Count);
		}
		if (_test_eggs)
		{
			_combine_data.add("_pool_eggs", _pool_eggs.Count);
		}
		if (_test_hand_items)
		{
			_combine_data.add("_pool_hand_renderers", _pool_hand_renderers.Count);
		}
		if (_test_statuses)
		{
			_combine_data.add("_pool_statuses", _pool_statuses.Count);
		}
		int tTotal = _combine_data.totalCombinations();
		for (int i = 0; i < tTotal; i++)
		{
			bool tAdult = getBool(i, "tAdult");
			bool tTouchingLiquid = getBool(i, "tTouchingLiquid");
			bool tLying = getBool(i, "tLying");
			bool tImmovable = getBool(i, "tImmovable");
			bool tUnconscious = getBool(i, "tUnconscious");
			ActorSex tSex = ((!getBool(i, "tSex")) ? ActorSex.Female : ActorSex.Male);
			bool tStopIdleAnimation = false;
			bool tAlwaysIdleAnimation = false;
			long tHashCode = (tAdult ? 1 : 2) + (tTouchingLiquid ? 1 : 2) * 10 + (tLying ? 1 : 2) * 100 + (tImmovable ? 1 : 2) * 1000 + (tUnconscious ? 1 : 2) * 10000 + ((tSex == ActorSex.Male) ? 1 : 2) * 100000 + (tStopIdleAnimation ? 1 : 2) * 1000000;
			SubspeciesTrait tMutation = null;
			if (_test_mutations)
			{
				tMutation = getFromPool(_pool_mutations, i, "_pool_mutations");
				tHashCode += _pool_mutations.IndexOf(tMutation) * 100000000;
				BaseStats tBaseStatsMeta = tMutation.base_stats_meta;
				if (!tBaseStatsMeta.isEmpty() && tBaseStatsMeta.hasTag("always_idle_animation"))
				{
					tAlwaysIdleAnimation = true;
				}
			}
			SubspeciesTrait tEgg = null;
			if (tMutation == null && _test_eggs)
			{
				tEgg = getFromPool(_pool_eggs, i, "_pool_eggs");
				tHashCode += _pool_eggs.IndexOf(tEgg) * 10000000000L;
			}
			bool tIsEgg = tEgg != null;
			IHandRenderer tItemRenderer;
			if (!tIsEgg && _test_hand_items)
			{
				AvatarCombineHandItem tItem = getFromPool(_pool_hand_renderers, i, "_pool_hand_renderers");
				tHashCode += _pool_hand_renderers.IndexOf(tItem) * 10000000000000L;
				tItemRenderer = tItem.hand_renderer;
			}
			else
			{
				tItemRenderer = null;
				tHashCode += _pool_hand_renderers.Count * 10000000000000L;
			}
			StatusAsset tStatus = null;
			if (_test_statuses)
			{
				tStatus = getFromPool(_pool_statuses, i, "_pool_statuses");
				tHashCode += _pool_statuses.IndexOf(tStatus) * 10000000000000000L;
			}
			int tHash = 1;
			foreach (UnitAvatarLoader item in _avatars.getListTotal())
			{
				_statuses.Clear();
				StatusAsset tRandomStatus = ((_test_statuses && Randy.randomBool()) ? _pool_statuses.GetRandom() : null);
				StatusAsset tRandomStatus2 = ((_test_statuses && Randy.randomBool()) ? _pool_statuses.GetRandom() : null);
				if (tStatus != null)
				{
					_statuses.Add(tStatus.id);
					if (tStatus.base_stats.hasTag("stop_idle_animation"))
					{
						tStopIdleAnimation = true;
					}
				}
				if (tRandomStatus != null)
				{
					_statuses.Add(tRandomStatus.id);
					if (tRandomStatus.base_stats.hasTag("stop_idle_animation"))
					{
						tStopIdleAnimation = true;
					}
				}
				if (tRandomStatus2 != null)
				{
					_statuses.Add(tRandomStatus2.id);
					if (tRandomStatus2.base_stats.hasTag("stop_idle_animation"))
					{
						tStopIdleAnimation = true;
					}
				}
				tHash++;
				ActorAvatarData tOldData = item.getData();
				ActorAsset tAsset = tOldData.asset;
				ActorTextureSubAsset tTextureAsset = ((tMutation == null) ? tAsset.texture_asset : tMutation.texture_asset);
				DynamicActorSpriteCreatorUI.getContainerForUI(tAsset, tAdult, tTextureAsset, tMutation, tIsEgg, tEgg);
				if (tAlwaysIdleAnimation)
				{
					tStopIdleAnimation = false;
				}
				ActorAvatarData tNewData = new ActorAvatarData();
				tNewData.setData(tOldData.asset, tMutation, tSex, Randy.randomInt(0, int.MaxValue), -1, null, tOldData.phenotype_index, tOldData.phenotype_skin_shade, tOldData.kingdom_color, tIsEgg, pIsKing: false, pIsWarrior: false, pIsWise: false, tEgg, tAdult, tLying, tTouchingLiquid, pIsInsideBoat: false, tOldData.is_hovering, tImmovable, tUnconscious, tStopIdleAnimation, tItemRenderer, tHash, _statuses, null);
				item.load(tNewData);
			}
			_check_collisions.Add(tHashCode);
			Debug.Log(string.Format("tested: {0}/{1}, hashset: {2}/{3} adult: {4}, liquid: {5}, lying: {6}, immovable: {7}, uncon: {8}, sex: {9}, mut: {10}, egg: {11}, item: {12}, status: {13}", i + 1, tTotal, _check_collisions.Count, tTotal, tAdult, tTouchingLiquid, tLying, tImmovable, tUnconscious, tSex, tMutation?.id ?? "null", tEgg?.id ?? "null", tItemRenderer, tStatus?.id ?? "null"));
			yield return null;
		}
	}
}
