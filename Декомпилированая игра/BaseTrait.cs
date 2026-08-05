using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

[Serializable]
public class BaseTrait<TTrait> : BaseAugmentationAsset, IDescription2Asset, IDescriptionAsset, ILocalizedAsset where TTrait : BaseTrait<TTrait>
{
	public float value;

	public WorldAction action_death;

	public WorldAction action_growth;

	public WorldAction action_birth;

	public GetHitAction action_get_hit;

	[DefaultValue(true)]
	public bool spawn_random_trait_allowed = true;

	[DefaultValue(5)]
	public int spawn_random_rate = 5;

	public bool special_icon_logic;

	public BaseStats base_stats_meta;

	public List<string> opposite_list;

	[NonSerialized]
	public HashSet<TTrait> opposite_traits;

	public string[] traits_to_remove_ids;

	[NonSerialized]
	public TTrait[] traits_to_remove;

	[DefaultValue("")]
	public string special_locale_description = string.Empty;

	[DefaultValue("")]
	public string special_locale_description_2 = string.Empty;

	[DefaultValue(true)]
	public bool has_localized_id = true;

	[DefaultValue(true)]
	public bool has_description_1 = true;

	[DefaultValue(true)]
	public bool has_description_2 = true;

	[DefaultValue(Rarity.R1_Rare)]
	public Rarity rarity = Rarity.R1_Rare;

	[DefaultValue(true)]
	public bool can_be_in_book = true;

	[DefaultValue("")]
	public string plot_id = string.Empty;

	[JsonIgnore]
	public List<ActorAsset> default_for_actor_assets;

	[JsonIgnore]
	public virtual string typed_id
	{
		get
		{
			throw new NotImplementedException(GetType().Name);
		}
	}

	[JsonIgnore]
	public PlotAsset plot_asset => AssetManager.plots_library.get(plot_id);

	public bool hasPlotAsset()
	{
		return !string.IsNullOrEmpty(plot_id);
	}

	public string getId()
	{
		return id;
	}

	public WorldAction getSpecialEffect()
	{
		return action_special_effect;
	}

	public float getSpecialEffectInterval()
	{
		return special_effect_interval;
	}

	public void addOpposite(string pID)
	{
		if (opposite_list == null)
		{
			opposite_list = new List<string>();
		}
		opposite_list.Add(pID);
	}

	public void addOpposites(IEnumerable<string> pListIDS)
	{
		if (opposite_list == null)
		{
			opposite_list = new List<string>(pListIDS);
		}
		else
		{
			opposite_list.AddRange(pListIDS);
		}
	}

	public void removeOpposite(string pID)
	{
		opposite_list.Remove(pID);
	}

	public override string getLocaleID()
	{
		if (!has_localized_id)
		{
			return null;
		}
		if (!string.IsNullOrEmpty(special_locale_id))
		{
			return special_locale_id;
		}
		return typed_id + "_" + id;
	}

	public string getDescriptionID()
	{
		if (!has_description_1)
		{
			return null;
		}
		if (!string.IsNullOrEmpty(special_locale_description))
		{
			return special_locale_description;
		}
		return typed_id + "_" + id + "_info";
	}

	public string getDescriptionID2()
	{
		if (!has_description_2)
		{
			return null;
		}
		if (!string.IsNullOrEmpty(special_locale_description_2))
		{
			return special_locale_description_2;
		}
		return typed_id + "_" + id + "_info_2";
	}

	public string getTranslatedName()
	{
		return LocalizedTextManager.getText(getLocaleID());
	}

	public string getTranslatedDescription()
	{
		string tID = getDescriptionID();
		if (LocalizedTextManager.stringExists(tID))
		{
			return LocalizedTextManager.getText(tID);
		}
		return null;
	}

	public string getTranslatedDescription2()
	{
		string tID = getDescriptionID2();
		if (LocalizedTextManager.stringExists(tID))
		{
			return LocalizedTextManager.getText(tID);
		}
		return null;
	}

	protected override bool isDebugUnlockedAll()
	{
		return DebugConfig.isOn(DebugOption.UnlockAllTraits);
	}

	protected virtual IEnumerable<ITraitsOwner<TTrait>> getRelatedMetaList()
	{
		throw new NotImplementedException(GetType().Name);
	}

	private ListPool<ITraitsOwner<TTrait>> getOwnersList()
	{
		ListPool<ITraitsOwner<TTrait>> tResult = new ListPool<ITraitsOwner<TTrait>>();
		TTrait tThisTrait = (TTrait)this;
		foreach (ITraitsOwner<TTrait> tObject in getRelatedMetaList())
		{
			if (tObject.hasTrait(tThisTrait))
			{
				tResult.Add(tObject);
			}
		}
		return tResult;
	}

	private (int pTotal, int pCivs, int pMobs) countTraitOwnersByCategories()
	{
		int fTotal = 0;
		int fCivs = 0;
		int fMobs = 0;
		TTrait tThisTrait = (TTrait)this;
		foreach (ITraitsOwner<TTrait> tObject in getRelatedMetaList())
		{
			if (tObject.hasTrait(tThisTrait))
			{
				fTotal++;
				if (isSapient(tObject))
				{
					fCivs++;
				}
				else
				{
					fMobs++;
				}
			}
		}
		return (pTotal: fTotal, pCivs: fCivs, pMobs: fMobs);
	}

	public virtual string getCountRows()
	{
		using ListPool<ITraitsOwner<TTrait>> tOwnersList = getOwnersList();
		int tPopulation = 0;
		foreach (ref ITraitsOwner<TTrait> item in tOwnersList)
		{
			ITraitsOwner<TTrait> tObject = item;
			tPopulation += ((IMetaObject)tObject).countUnits();
		}
		using StringBuilderPool tBuilder = new StringBuilderPool();
		tBuilder.Append(LocalizedTextManager.getText(typed_id + "_amount_text").Replace("$amount$", getColoredNumber(tOwnersList.Count.ToString())));
		tBuilder.AppendLine();
		tBuilder.Append(LocalizedTextManager.getText("population_amount").Replace("$amount$", getColoredNumber(tPopulation.ToString())));
		return tBuilder.ToString();
	}

	private string getColoredNumber(string pText)
	{
		return Toolbox.coloredString(pText, "#F3961F");
	}

	protected string getCountRowsByCategories()
	{
		(int, int, int) tCounts = countTraitOwnersByCategories();
		using StringBuilderPool tBuilder = new StringBuilderPool();
		tBuilder.Append(LocalizedTextManager.getText("trait_owners_civs").Replace("$amount$", getColoredNumber(tCounts.Item2.ToString())));
		tBuilder.AppendLine();
		tBuilder.Append(LocalizedTextManager.getText("trait_owners_mobs").Replace("$amount$", getColoredNumber(tCounts.Item3.ToString())));
		return tBuilder.ToString();
	}

	protected virtual bool isSapient(ITraitsOwner<TTrait> pObject)
	{
		throw new NotImplementedException(GetType().Name);
	}

	public bool ShouldSerializebase_stats_meta()
	{
		return !base_stats_meta.isEmpty();
	}

	public void setTraitInfoToGrinMark()
	{
		path_icon = "ui/Icons/subspecies_traits/subspecies_trait_grin_mark";
		special_locale_id = "subspecies_trait_grin_mark";
		special_locale_description = "subspecies_trait_grin_mark_info";
		special_locale_description_2 = "subspecies_trait_grin_mark_info_2";
		show_for_unlockables_ui = false;
		action_on_augmentation_add = (WorldActionTrait)Delegate.Combine(action_on_augmentation_add, new WorldActionTrait(WorldBehaviourActions.addForGrinReaper));
		action_on_augmentation_load = (WorldActionTrait)Delegate.Combine(action_on_augmentation_load, new WorldActionTrait(WorldBehaviourActions.addForGrinReaper));
		action_on_augmentation_remove = (WorldActionTrait)Delegate.Combine(action_on_augmentation_remove, new WorldActionTrait(WorldBehaviourActions.removeUsedForGrinReaper));
		action_on_object_remove = (WorldActionTrait)Delegate.Combine(action_on_object_remove, new WorldActionTrait(WorldBehaviourActions.removeUsedForGrinReaper));
	}
}
