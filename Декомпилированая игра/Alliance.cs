using System.Collections.Generic;
using db;
using UnityEngine;

public class Alliance : MetaObject<AllianceData>
{
	public List<Kingdom> kingdoms_list = new List<Kingdom>();

	public HashSet<Kingdom> kingdoms_hashset = new HashSet<Kingdom>();

	public int power;

	protected override MetaType meta_type => MetaType.Alliance;

	public override BaseSystemManager manager => World.world.alliances;

	public void createNewAlliance()
	{
		string tName = NameGenerator.getName("alliance_name");
		setName(tName);
		generateNewMetaObject();
	}

	protected sealed override void setDefaultValues()
	{
		base.setDefaultValues();
		power = 0;
	}

	public override int countTotalMoney()
	{
		int tResult = 0;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			tResult += tKingdom.countTotalMoney();
		}
		return tResult;
	}

	public override int countHappyUnits()
	{
		if (kingdoms_list.Count == 0)
		{
			return 0;
		}
		int tResult = 0;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			tResult += tKingdom.countHappyUnits();
		}
		return tResult;
	}

	public override int countSick()
	{
		int tResult = 0;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			tResult += tKingdom.countSick();
		}
		return tResult;
	}

	public override int countHungry()
	{
		int tResult = 0;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			tResult += tKingdom.countHungry();
		}
		return tResult;
	}

	public override int countStarving()
	{
		int tResult = 0;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			tResult += tKingdom.countStarving();
		}
		return tResult;
	}

	public override int countChildren()
	{
		int tResult = 0;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			tResult += tKingdom.countChildren();
		}
		return tResult;
	}

	public override int countAdults()
	{
		int tResult = 0;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			tResult += tKingdom.countAdults();
		}
		return tResult;
	}

	public override int countHomeless()
	{
		int tResult = 0;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			tResult += tKingdom.countHomeless();
		}
		return tResult;
	}

	public override IEnumerable<Family> getFamilies()
	{
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			foreach (Family family in tKingdom.getFamilies())
			{
				yield return family;
			}
		}
	}

	public override bool hasFamilies()
	{
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			if (tKingdoms[i].hasFamilies())
			{
				return true;
			}
		}
		return false;
	}

	public override int countMales()
	{
		int tResult = 0;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			tResult += tKingdom.countMales();
		}
		return tResult;
	}

	public override int countFemales()
	{
		int tResult = 0;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			tResult += tKingdom.countFemales();
		}
		return tResult;
	}

	public override int countHoused()
	{
		int tResult = 0;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			tResult += tKingdom.countHoused();
		}
		return tResult;
	}

	public void setType(AllianceType pType)
	{
		data.alliance_type = pType;
	}

	public bool isForcedType()
	{
		return data.alliance_type == AllianceType.Forced;
	}

	public bool isNormalType()
	{
		return data.alliance_type == AllianceType.Normal;
	}

	protected override ColorLibrary getColorLibrary()
	{
		return AssetManager.kingdom_colors_library;
	}

	public override void generateBanner()
	{
		Sprite[] tBgs = World.world.alliances.getBackgroundsList();
		data.banner_background_id = Randy.randomInt(0, tBgs.Length);
		Sprite[] tIcons = World.world.alliances.getIconsList();
		data.banner_icon_id = Randy.randomInt(0, tIcons.Length);
	}

	public void addFounders(Kingdom pKingdom1, Kingdom pKingdom2)
	{
		data.founder_kingdom_name = pKingdom1.data.name;
		data.founder_kingdom_id = pKingdom1.getID();
		data.founder_actor_name = pKingdom1.king?.getName();
		data.founder_actor_id = pKingdom1.king?.getID() ?? (-1);
		join(pKingdom1, pRecalc: true, pForce: true);
		join(pKingdom2, pRecalc: true, pForce: true);
	}

	public void update()
	{
		power = 0;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			power += tKingdom.power;
		}
	}

	public bool checkActive()
	{
		bool tChanged = false;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = tKingdoms.Count - 1; i >= 0; i--)
		{
			Kingdom tKingdom = tKingdoms[i];
			if (!tKingdom.isAlive())
			{
				leave(tKingdom, pRecalc: false);
				kingdoms_list.RemoveAt(i);
				tChanged = true;
			}
		}
		if (tChanged)
		{
			recalculate();
		}
		if (kingdoms_list.Count >= 2)
		{
			return true;
		}
		return false;
	}

	public void dissolve()
	{
		foreach (Kingdom item in kingdoms_hashset)
		{
			item.allianceLeave(this);
		}
		kingdoms_hashset.Clear();
	}

	public void recalculate()
	{
		kingdoms_list.Clear();
		kingdoms_list.AddRange(kingdoms_hashset);
		mergeWars();
	}

	public bool canJoin(Kingdom pKingdom)
	{
		foreach (Kingdom tAllianceKingdom in kingdoms_hashset)
		{
			if (!pKingdom.isOpinionTowardsKingdomGood(tAllianceKingdom))
			{
				return false;
			}
		}
		return true;
	}

	public bool join(Kingdom pKingdom, bool pRecalc = true, bool pForce = false)
	{
		if (hasKingdom(pKingdom))
		{
			return false;
		}
		if (!pForce && !canJoin(pKingdom))
		{
			return false;
		}
		kingdoms_hashset.Add(pKingdom);
		if (hasWars())
		{
			if (hasWarsWith(pKingdom))
			{
				foreach (War tWar in getAttackerWars())
				{
					if (tWar.isDefender(pKingdom))
					{
						tWar.leaveWar(pKingdom);
					}
				}
				foreach (War tWar2 in getDefenderWars())
				{
					if (tWar2.isAttacker(pKingdom))
					{
						tWar2.leaveWar(pKingdom);
					}
				}
			}
			foreach (War attackerWar in getAttackerWars())
			{
				attackerWar.joinAttackers(pKingdom);
			}
			foreach (War tWar3 in getDefenderWars())
			{
				if (!tWar3.isTotalWar())
				{
					tWar3.joinDefenders(pKingdom);
				}
			}
		}
		if (pKingdom.hasEnemies())
		{
			foreach (War tWar4 in pKingdom.getWars())
			{
				if (tWar4.isTotalWar())
				{
					continue;
				}
				if (tWar4.isMainAttacker(pKingdom))
				{
					foreach (Kingdom tKingdom in kingdoms_list)
					{
						tWar4.joinAttackers(tKingdom);
					}
				}
				if (!tWar4.isMainDefender(pKingdom))
				{
					continue;
				}
				foreach (Kingdom tKingdom2 in kingdoms_list)
				{
					tWar4.joinDefenders(tKingdom2);
				}
			}
		}
		pKingdom.allianceJoin(this);
		if (pRecalc)
		{
			recalculate();
		}
		data.timestamp_member_joined = World.world.getCurWorldTime();
		return true;
	}

	public void leave(Kingdom pKingdom, bool pRecalc = true)
	{
		kingdoms_hashset.Remove(pKingdom);
		if (hasWars())
		{
			foreach (War tWar in getAttackerWars())
			{
				if (!tWar.isMainAttacker(pKingdom))
				{
					tWar.leaveWar(pKingdom);
					continue;
				}
				foreach (Kingdom tKingdom in kingdoms_hashset)
				{
					tWar.leaveWar(tKingdom);
				}
			}
			foreach (War tWar2 in getDefenderWars())
			{
				if (!tWar2.isMainDefender(pKingdom))
				{
					tWar2.leaveWar(pKingdom);
					continue;
				}
				foreach (Kingdom tKingdom2 in kingdoms_hashset)
				{
					tWar2.leaveWar(tKingdom2);
				}
			}
		}
		pKingdom.allianceLeave(this);
		if (pRecalc)
		{
			recalculate();
		}
	}

	public override void save()
	{
		base.save();
		data.kingdoms = new List<long>();
		foreach (Kingdom tKingdom in kingdoms_hashset)
		{
			data.kingdoms.Add(tKingdom.id);
		}
	}

	public override void loadData(AllianceData pData)
	{
		base.loadData(pData);
		foreach (long tKingdomID in data.kingdoms)
		{
			Kingdom tKingdom = World.world.kingdoms.get(tKingdomID);
			if (tKingdom != null)
			{
				kingdoms_hashset.Add(tKingdom);
			}
		}
		recalculate();
	}

	public int countBuildings()
	{
		int tResult = 0;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			tResult += tKingdom.countBuildings();
		}
		return tResult;
	}

	public int countZones()
	{
		int tResult = 0;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			tResult += tKingdom.countZones();
		}
		return tResult;
	}

	public override int countUnits()
	{
		return countPopulation();
	}

	public int countPopulation()
	{
		int tResult = 0;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			tResult += tKingdom.getPopulationPeople();
		}
		return tResult;
	}

	public int countCities()
	{
		int tResult = 0;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			tResult += tKingdom.countCities();
		}
		return tResult;
	}

	public int countKingdoms()
	{
		return kingdoms_hashset.Count;
	}

	public string getMotto()
	{
		if (string.IsNullOrEmpty(data.motto))
		{
			data.motto = NameGenerator.getName("alliance_mottos");
		}
		return data.motto;
	}

	public int countWarriors()
	{
		int tResult = 0;
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			tResult += tKingdom.countTotalWarriors();
		}
		return tResult;
	}

	public static bool isSame(Alliance pAlliance1, Alliance pAlliance2)
	{
		if (pAlliance1 == null || pAlliance2 == null)
		{
			return false;
		}
		return pAlliance1 == pAlliance2;
	}

	public bool hasWarsWith(Kingdom pKingdom)
	{
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tAllianceKingdom = tKingdoms[i];
			if (pKingdom.isInWarWith(tAllianceKingdom))
			{
				return true;
			}
		}
		return false;
	}

	public bool hasSupremeKingdom()
	{
		if (DiplomacyManager.kingdom_supreme == null)
		{
			return false;
		}
		return hasKingdom(DiplomacyManager.kingdom_supreme);
	}

	public bool hasKingdom(Kingdom pKingdom)
	{
		return kingdoms_hashset.Contains(pKingdom);
	}

	public bool hasSharedBordersWithKingdom(Kingdom pKingdom)
	{
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			if (DiplomacyHelpers.areKingdomsClose(pKingdom, tKingdom))
			{
				return true;
			}
		}
		return false;
	}

	public bool hasWars()
	{
		return World.world.wars.hasWars(this);
	}

	public IEnumerable<War> getWars(bool pRandom = false)
	{
		return World.world.wars.getWars(this, pRandom);
	}

	public void mergeWars()
	{
		if (!hasWars())
		{
			return;
		}
		using ListPool<War> tWars = new ListPool<War>(getWars());
		for (int i = 0; i < tWars.Count; i++)
		{
			War tWar1 = tWars[i];
			if (tWar1.hasEnded())
			{
				continue;
			}
			for (int j = i + 1; j < tWars.Count; j++)
			{
				War tWar2 = tWars[j];
				if (!tWar2.hasEnded() && tWar1.isSameAs(tWar2))
				{
					if (tWar1.data.created_time < tWar2.data.created_time)
					{
						World.world.wars.endWar(tWar2, WarWinner.Merged);
					}
					else
					{
						World.world.wars.endWar(tWar1, WarWinner.Merged);
					}
					mergeWars();
					return;
				}
			}
		}
	}

	public IEnumerable<War> getAttackerWars()
	{
		foreach (War tWar in getWars())
		{
			foreach (Kingdom tKingdom in kingdoms_list)
			{
				if (tWar.isAttacker(tKingdom))
				{
					yield return tWar;
					break;
				}
			}
		}
	}

	public IEnumerable<War> getDefenderWars()
	{
		foreach (War tWar in getWars())
		{
			foreach (Kingdom tKingdom in kingdoms_list)
			{
				if (tWar.isDefender(tKingdom))
				{
					yield return tWar;
					break;
				}
			}
		}
	}

	public override IEnumerable<Actor> getUnits()
	{
		List<Kingdom> tKingdoms = kingdoms_list;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			Kingdom tKingdom = tKingdoms[i];
			foreach (Actor unit in tKingdom.getUnits())
			{
				yield return unit;
			}
		}
	}

	public override bool isReadyForRemoval()
	{
		return false;
	}

	public override Actor getRandomUnit()
	{
		return kingdoms_list.GetRandom().getRandomUnit();
	}

	public Sprite getBackgroundSprite()
	{
		return World.world.alliances.getBackgroundsList()[data.banner_background_id];
	}

	public Sprite getIconSprite()
	{
		return World.world.alliances.getIconsList()[data.banner_icon_id];
	}

	public override void Dispose()
	{
		DBInserter.deleteData(getID(), "alliance");
		kingdoms_list.Clear();
		kingdoms_hashset.Clear();
		base.Dispose();
	}
}
