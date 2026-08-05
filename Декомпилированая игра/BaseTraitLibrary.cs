using System;
using System.Collections.Generic;

[Serializable]
public class BaseTraitLibrary<T> : BaseLibraryWithUnlockables<T> where T : BaseTrait<T>
{
	protected List<T> _pot_allowed_to_be_given_randomly = new List<T>();

	protected virtual string icon_path
	{
		get
		{
			throw new NotImplementedException(GetType().Name);
		}
	}

	public override void post_init()
	{
		base.post_init();
		list.Sort((T pT1, T pT2) => StringComparer.Ordinal.Compare(pT2.id, pT1.id));
		autoSetRarity();
		checkIcons();
	}

	protected virtual void autoSetRarity()
	{
		foreach (T tTrait in list)
		{
			if (tTrait.unlocked_with_achievement)
			{
				tTrait.rarity = Rarity.R3_Legendary;
				continue;
			}
			bool num = tTrait.action_death != null || tTrait.action_special_effect != null || tTrait.action_get_hit != null || tTrait.action_birth != null || tTrait.action_attack_target != null || tTrait.action_on_augmentation_add != null || tTrait.action_on_augmentation_remove != null || tTrait.action_on_augmentation_load != null;
			bool tHasDecisions = tTrait.decision_ids != null;
			bool tHasSpells = tTrait.spells_ids != null;
			bool tHasCombatActions = tTrait.combat_actions_ids != null;
			bool tHasTag = tTrait.base_stats.hasTags();
			bool tHasPlot = !string.IsNullOrEmpty(tTrait.plot_id);
			int tCount = 0;
			if (num)
			{
				tCount++;
			}
			if (tHasDecisions)
			{
				tCount++;
			}
			if (tHasSpells)
			{
				tCount++;
			}
			if (tHasCombatActions)
			{
				tCount++;
			}
			if (tHasTag)
			{
				tCount++;
			}
			if (tHasPlot)
			{
				tCount++;
			}
			if (tCount > 0)
			{
				if (tCount == 1)
				{
					tTrait.rarity = Rarity.R1_Rare;
				}
				else
				{
					tTrait.rarity = Rarity.R2_Epic;
				}
				tTrait.needs_to_be_explored = true;
			}
			else if (tTrait.rarity == Rarity.R0_Normal)
			{
				tTrait.needs_to_be_explored = false;
			}
		}
	}

	public override void linkAssets()
	{
		base.linkAssets();
		fillOppositeHashsetsWithAssets();
		linkDecisions();
		linkCombatActions();
		linkSpells();
		linkActorAssets();
		foreach (T tTrait in list)
		{
			if (tTrait.spawn_random_trait_allowed)
			{
				_pot_allowed_to_be_given_randomly.AddTimes(tTrait.spawn_random_rate, tTrait);
			}
		}
	}

	private void linkCombatActions()
	{
		foreach (T item in list)
		{
			item.linkCombatActions();
		}
	}

	private void linkSpells()
	{
		foreach (T item in list)
		{
			item.linkSpells();
		}
	}

	private void linkDecisions()
	{
		foreach (T tAsset in list)
		{
			if (tAsset.decision_ids != null)
			{
				tAsset.decisions_assets = new DecisionAsset[tAsset.decision_ids.Count];
				for (int i = 0; i < tAsset.decision_ids.Count; i++)
				{
					string tDecisionID = tAsset.decision_ids[i];
					DecisionAsset tDecisionAsset = AssetManager.decisions_library.get(tDecisionID);
					tAsset.decisions_assets[i] = tDecisionAsset;
				}
			}
		}
	}

	private void linkActorAssets()
	{
		foreach (ActorAsset tActorAsset in AssetManager.actor_library.list)
		{
			List<string> tTraits = getDefaultTraitsForMeta(tActorAsset);
			if (tTraits == null)
			{
				continue;
			}
			foreach (string tTraitId in tTraits)
			{
				T tTrait = get(tTraitId);
				if (tTrait.default_for_actor_assets == null)
				{
					tTrait.default_for_actor_assets = new List<ActorAsset>();
				}
				tTrait.default_for_actor_assets.Add(tActorAsset);
			}
		}
	}

	public override void editorDiagnostic()
	{
		checkOppositeErrors();
		foreach (T tTrait in list)
		{
			if (string.IsNullOrEmpty(tTrait.group_id))
			{
				BaseAssetLibrary.logAssetError("Group id not assigned", tTrait.id);
			}
			if (!tTrait.special_icon_logic && SpriteTextureLoader.getSprite(tTrait.path_icon) == null)
			{
				BaseAssetLibrary.logAssetError("Missing icon file", tTrait.path_icon);
			}
		}
		base.editorDiagnostic();
	}

	public override void editorDiagnosticLocales()
	{
		foreach (T tTrait in list)
		{
			checkLocale(tTrait, tTrait.getLocaleID());
			checkLocale(tTrait, tTrait.getDescriptionID());
			checkLocale(tTrait, tTrait.getDescriptionID2());
		}
	}

	private void checkOppositeErrors()
	{
		foreach (T tMainTrait in list)
		{
			HashSet<T> tMainOppositeList = tMainTrait.opposite_traits;
			if (tMainOppositeList == null)
			{
				continue;
			}
			foreach (T tOppositeTrait in tMainOppositeList)
			{
				HashSet<T> tOppositeTraitList = tOppositeTrait.opposite_traits;
				if (tOppositeTraitList == null || !tOppositeTraitList.Contains(tMainTrait))
				{
					logErrorOpposites(tMainTrait.id, tOppositeTrait.id);
				}
			}
		}
	}

	private void fillOppositeHashsetsWithAssets()
	{
		foreach (T tMainTrait in list)
		{
			if (tMainTrait.opposite_list == null || tMainTrait.opposite_list.Count <= 0)
			{
				continue;
			}
			tMainTrait.opposite_traits = new HashSet<T>(tMainTrait.opposite_list.Count);
			foreach (string tID in tMainTrait.opposite_list)
			{
				T tOppositeTrait = get(tID);
				tMainTrait.opposite_traits.Add(tOppositeTrait);
			}
		}
		foreach (T tMainTrait2 in list)
		{
			if (tMainTrait2.traits_to_remove_ids != null)
			{
				int tCount = tMainTrait2.traits_to_remove_ids.Length;
				tMainTrait2.traits_to_remove = new T[tCount];
				for (int i = 0; i < tCount; i++)
				{
					string tID2 = tMainTrait2.traits_to_remove_ids[i];
					T tTraitToAdd = get(tID2);
					tMainTrait2.traits_to_remove[i] = tTraitToAdd;
				}
			}
		}
	}

	private void checkIcons()
	{
		foreach (T tTrait in list)
		{
			if (string.IsNullOrEmpty(tTrait.path_icon))
			{
				tTrait.path_icon = icon_path + tTrait.getLocaleID();
			}
		}
	}

	public override T add(T pAsset)
	{
		T tNewAsset = base.add(pAsset);
		if (tNewAsset.base_stats == null)
		{
			tNewAsset.base_stats = new BaseStats();
		}
		if (tNewAsset.base_stats_meta == null)
		{
			tNewAsset.base_stats_meta = new BaseStats();
		}
		return tNewAsset;
	}

	public string addToGameplayReportShort(string pWhatFor)
	{
		string tResult = string.Empty;
		tResult = tResult + pWhatFor + "\n";
		foreach (T tAsset in list)
		{
			string tName = tAsset.id;
			if (!(tName == "Phenotype"))
			{
				string tDescription1 = tAsset.getTranslatedDescription();
				string tLineInfo = "\n" + tName;
				if (!string.IsNullOrEmpty(tDescription1))
				{
					tLineInfo = tLineInfo + ": " + tDescription1;
				}
				tResult += tLineInfo;
			}
		}
		return tResult + "\n\n";
	}

	public string addToGameplayReport(string pWhatFor)
	{
		string tResult = string.Empty;
		tResult = tResult + pWhatFor + "\n";
		foreach (T tAsset in list)
		{
			string tName = tAsset.getTranslatedName();
			if (!(tName == "Phenotype"))
			{
				string tDescription1 = tAsset.getTranslatedDescription();
				string tDescription2 = tAsset.getTranslatedDescription2();
				string tLineInfo = "\n" + tName;
				tLineInfo += "\n";
				if (!string.IsNullOrEmpty(tDescription1))
				{
					tLineInfo = tLineInfo + "1: " + tDescription1;
				}
				if (!string.IsNullOrEmpty(tDescription2))
				{
					tLineInfo = tLineInfo + "\n2: " + tDescription2;
				}
				tResult += tLineInfo;
			}
		}
		return tResult + "\n\n";
	}

	public T getRandomSpawnTrait()
	{
		return _pot_allowed_to_be_given_randomly.GetRandom();
	}

	protected virtual List<string> getDefaultTraitsForMeta(ActorAsset pAsset)
	{
		throw new NotImplementedException();
	}
}
