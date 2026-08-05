using System.Collections.Generic;
using UnityEngine;

public class DiplomacyManager : CoreSystemManager<DiplomacyRelation, DiplomacyRelationData>
{
	public static Kingdom kingdom_supreme;

	public static Kingdom kingdom_second;

	public static List<Kingdom> superpowers = new List<Kingdom>();

	private float diplomacyTick;

	private static List<Kingdom> _kingdom_sorter = new List<Kingdom>();

	private static List<DiplomacyRelation> _relations_remover = new List<DiplomacyRelation>();

	protected readonly Dictionary<string, DiplomacyRelation> _dict = new Dictionary<string, DiplomacyRelation>();

	public DiplomacyManager()
	{
		type_id = "diplomacy";
	}

	public override List<DiplomacyRelationData> save(List<DiplomacyRelation> pList = null)
	{
		List<DiplomacyRelationData> tSavingList = new List<DiplomacyRelationData>();
		using IEnumerator<DiplomacyRelation> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			DiplomacyRelation tRel = enumerator.Current;
			tRel.kingdom1 = World.world.kingdoms.get(tRel.data.kingdom1_id);
			tRel.kingdom2 = World.world.kingdoms.get(tRel.data.kingdom2_id);
			if (tRel.kingdom1 != null && tRel.kingdom2 != null)
			{
				tSavingList.Add(tRel.data);
			}
		}
		return tSavingList;
	}

	public override void loadFromSave(List<DiplomacyRelationData> pList)
	{
		for (int i = 0; i < pList.Count; i++)
		{
			DiplomacyRelationData tData = pList[i];
			Kingdom kingdom = World.world.kingdoms.get(tData.kingdom1_id);
			Kingdom tK2 = World.world.kingdoms.get(tData.kingdom2_id);
			if (kingdom != null && tK2 != null)
			{
				if (tData.id == -1)
				{
					tData.id = World.world.map_stats.getNextId(type_id);
				}
				loadObject(tData);
			}
		}
	}

	public override DiplomacyRelation loadObject(DiplomacyRelationData pData)
	{
		Kingdom tK1 = World.world.kingdoms.get(pData.kingdom1_id);
		Kingdom tK2 = World.world.kingdoms.get(pData.kingdom2_id);
		pData.rel_id = pData.kingdom1_id + "_" + pData.kingdom2_id;
		DiplomacyRelation tNewRelation = base.loadObject(pData);
		_dict.Add(pData.rel_id, tNewRelation);
		tNewRelation.kingdom1 = tK1;
		tNewRelation.kingdom2 = tK2;
		return tNewRelation;
	}

	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		if (!World.world.isPaused())
		{
			if (diplomacyTick > 0f)
			{
				diplomacyTick -= pElapsed;
			}
			else if (!World.world.cities.isLocked())
			{
				diplomacyTick = 2f;
				newDiplomacyTick();
			}
		}
	}

	public void newDiplomacyTick()
	{
		findSupremeKingdom();
		checkAchievements();
	}

	private void checkAchievements()
	{
		AchievementLibrary.world_war.check();
	}

	private void findSupremeKingdom()
	{
		kingdom_supreme = null;
		kingdom_second = null;
		if (World.world.kingdoms.Count != 0)
		{
			List<Kingdom> tKingdoms = _kingdom_sorter;
			tKingdoms.AddRange(World.world.kingdoms);
			for (int i = 0; i < tKingdoms.Count; i++)
			{
				Kingdom tKingdom = tKingdoms[i];
				tKingdom.power = tKingdom.countTotalWarriors() * 2 + tKingdom.countCities() * 5 + 1;
			}
			tKingdoms.Sort(sortByPower);
			kingdom_supreme = tKingdoms[0];
			if (tKingdoms.Count > 1)
			{
				kingdom_second = tKingdoms[1];
			}
			else
			{
				kingdom_second = null;
			}
			tKingdoms.Clear();
		}
	}

	public int sortByPower(Kingdom o1, Kingdom o2)
	{
		return o2.power.CompareTo(o1.power);
	}

	private War startTotalWar(Kingdom pAttacker, WarTypeAsset pType)
	{
		if (World.world.kingdoms.Count == 1)
		{
			return null;
		}
		foreach (War tWar in pAttacker.getWars())
		{
			if (tWar.isMainAttacker(pAttacker) && tWar.isTotalWar())
			{
				return null;
			}
		}
		if (pAttacker.hasAlliance())
		{
			pAttacker.getAlliance().leave(pAttacker);
		}
		War tNewWar = World.world.wars.newWar(pAttacker, null, pType);
		using ListPool<War> tWars = new ListPool<War>(pAttacker.getWars());
		foreach (ref War item in tWars)
		{
			War tWar2 = item;
			if (tWar2.isTotalWar())
			{
				continue;
			}
			if (tWar2.isAttacker(pAttacker))
			{
				if (!tWar2.isMainAttacker(pAttacker))
				{
					tWar2.leaveWar(pAttacker);
				}
				else
				{
					World.world.wars.endWar(tWar2, WarWinner.Merged);
				}
			}
			else if (tWar2.isDefender(pAttacker))
			{
				if (!tWar2.isMainDefender(pAttacker))
				{
					tWar2.leaveWar(pAttacker);
				}
				else
				{
					tWar2.lostWar(pAttacker);
				}
			}
		}
		WorldLog.logNewTotalWar(pAttacker);
		return tNewWar;
	}

	internal War startWar(Kingdom pAttacker, Kingdom pDefender, WarTypeAsset pAsset, bool pLog = true)
	{
		if (pAsset.total_war)
		{
			return startTotalWar(pAttacker, pAsset);
		}
		if (pAttacker == pDefender)
		{
			return null;
		}
		if (World.world.wars.getWar(pAttacker, pDefender) != null)
		{
			return null;
		}
		if (pLog)
		{
			WorldLog.logNewWar(pAttacker, pDefender);
		}
		War tNewWar = World.world.wars.newWar(pAttacker, pDefender, pAsset);
		if (pAsset.alliance_join)
		{
			Alliance tAllianceAttackers = pAttacker.getAlliance();
			Alliance tAllianceDefenders = pDefender.getAlliance();
			if (tAllianceAttackers != null)
			{
				foreach (Kingdom tKingdom in tAllianceAttackers.kingdoms_hashset)
				{
					tNewWar.joinAttackers(tKingdom);
				}
			}
			if (tAllianceDefenders != null)
			{
				foreach (Kingdom tKingdom2 in tAllianceDefenders.kingdoms_hashset)
				{
					tNewWar.joinDefenders(tKingdom2);
				}
			}
		}
		return tNewWar;
	}

	public void eventSpite(Kingdom pKingdom)
	{
		if (World.world.kingdoms.Count <= 1)
		{
			return;
		}
		using ListPool<Kingdom> toHighlight = new ListPool<Kingdom>(World.world.kingdoms.Count);
		War tWar = startWar(pKingdom, null, WarTypeLibrary.spite);
		if (tWar == null)
		{
			return;
		}
		pKingdom.affectKingByPowers();
		toHighlight.AddRange(tWar.getAttackers());
		toHighlight.AddRange(tWar.getDefenders());
		foreach (ref Kingdom item in toHighlight)
		{
			EffectsLibrary.highlightKingdomZones(item, Color.red);
		}
	}

	public void eventFriendship(Kingdom pKingdom)
	{
		War tWar = World.world.wars.getRandomWarFor(pKingdom);
		if (tWar == null)
		{
			return;
		}
		using ListPool<Kingdom> toHighlight = new ListPool<Kingdom>(World.world.kingdoms.Count);
		if (tWar.isTotalWar() || tWar.isMainAttacker(pKingdom) || tWar.isMainDefender(pKingdom))
		{
			toHighlight.AddRange(tWar.getAttackers());
			toHighlight.AddRange(tWar.getDefenders());
		}
		Alliance tAlliance = pKingdom.getAlliance();
		if (tAlliance == null)
		{
			tWar.leaveWar(pKingdom);
			toHighlight.Add(pKingdom);
			pKingdom.affectKingByPowers();
		}
		else
		{
			foreach (Kingdom tAllianceKingdom in tAlliance.kingdoms_hashset)
			{
				tWar.leaveWar(tAllianceKingdom);
				toHighlight.Add(tAllianceKingdom);
				tAllianceKingdom.affectKingByPowers();
			}
		}
		if (tWar.isTotalWar())
		{
			World.world.wars.endWar(tWar, WarWinner.Peace);
		}
		foreach (ref Kingdom item in toHighlight)
		{
			EffectsLibrary.highlightKingdomZones(item, Color.green);
		}
	}

	public KingdomOpinion getOpinion(Kingdom k1, Kingdom k2)
	{
		return getRelation(k1, k2).getOpinion(k1, k2);
	}

	public int sortID(Kingdom o1, Kingdom o2)
	{
		return o1.id.CompareTo(o2.id);
	}

	public DiplomacyRelation getRelation(Kingdom pK1, Kingdom pK2)
	{
		Kingdom tOrder1;
		Kingdom tOrder2;
		if (pK1.id.CompareTo(pK2.id) > 0)
		{
			tOrder1 = pK1;
			tOrder2 = pK2;
		}
		else
		{
			tOrder1 = pK2;
			tOrder2 = pK1;
		}
		string tID = tOrder1.id + "_" + tOrder2.id;
		if (tryGet(tID, out var tRelation))
		{
			return tRelation;
		}
		tRelation = newObject();
		tRelation.data.rel_id = tID;
		_dict.Add(tID, tRelation);
		tRelation.data.kingdom1_id = tOrder1.id;
		tRelation.data.kingdom2_id = tOrder2.id;
		tRelation.kingdom1 = tOrder1;
		tRelation.kingdom2 = tOrder2;
		return tRelation;
	}

	public void removeRelationsFor(Kingdom pKingdom)
	{
		using (IEnumerator<DiplomacyRelation> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				DiplomacyRelation tRelation = enumerator.Current;
				if (tRelation.kingdom1 == pKingdom || tRelation.kingdom2 == pKingdom)
				{
					_relations_remover.Add(tRelation);
				}
			}
		}
		foreach (DiplomacyRelation tRelation2 in _relations_remover)
		{
			removeObject(tRelation2);
		}
		_relations_remover.Clear();
	}

	public bool tryGet(string pID, out DiplomacyRelation pObject)
	{
		return _dict.TryGetValue(pID, out pObject);
	}

	public DiplomacyRelation get(string pID)
	{
		if (string.IsNullOrEmpty(pID))
		{
			return null;
		}
		tryGet(pID, out var tObject);
		return tObject;
	}

	public override void removeObject(DiplomacyRelation pObject)
	{
		_dict.Remove(pObject.data.rel_id);
		base.removeObject(pObject);
	}

	public override void clear()
	{
		diplomacyTick = 0f;
		kingdom_supreme = null;
		kingdom_second = null;
		_dict.Clear();
		base.clear();
	}
}
