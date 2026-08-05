using System.Collections.Generic;

public class ClanManager : MetaSystemManager<Clan, ClanData>
{
	public ClanManager()
	{
		type_id = "clan";
	}

	public Clan newClan(Actor pFounder, bool pAddDefaultTraits)
	{
		World.world.game_stats.data.clansCreated++;
		World.world.map_stats.clansCreated++;
		Clan tNewClan = newObject();
		tNewClan.newClan(pFounder, pAddDefaultTraits);
		MetaHelper.addRandomTrait(tNewClan, AssetManager.clan_traits);
		pFounder.setClan(tNewClan);
		if (pFounder.isKing())
		{
			pFounder.kingdom.trySetRoyalClan();
		}
		convertFamilyToClan(pFounder, tNewClan);
		addRandomTraitFromBiomeToClan(tNewClan, pFounder.current_tile);
		return tNewClan;
	}

	private void convertFamilyToClan(Actor pFounder, Clan pNewClan)
	{
		if (!pFounder.hasFamily())
		{
			return;
		}
		foreach (Actor tFamilyMember in pFounder.getChildren())
		{
			if (!tFamilyMember.hasClan())
			{
				tFamilyMember.setClan(pNewClan);
			}
		}
	}

	public override void removeObject(Clan pClan)
	{
		foreach (Kingdom tKingdom in World.world.kingdoms.list)
		{
			if (tKingdom.data.royal_clan_id == pClan.getID() && pClan.getRenown() >= 10)
			{
				tKingdom.logRoyalClanLost(pClan);
			}
		}
		World.world.game_stats.data.clansDestroyed++;
		World.world.map_stats.clansDestroyed++;
		base.removeObject(pClan);
	}

	public void addRandomTraitFromBiomeToClan(Clan pClan, WorldTile pTile)
	{
		pClan.addRandomTraitFromBiome(pTile, pTile.Type.biome_asset?.spawn_trait_clan, AssetManager.clan_traits);
	}

	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		foreach (Clan tClan in list)
		{
			if (!tClan.hasChief())
			{
				tClan.checkMembersForNewChief();
			}
		}
	}

	protected override void updateDirtyUnits()
	{
		List<Actor> tActorList = World.world.units.units_only_alive;
		for (int i = 0; i < tActorList.Count; i++)
		{
			Actor tUnit = tActorList[i];
			Clan tClan = tUnit.clan;
			if (tClan != null && tClan.isDirtyUnits())
			{
				tClan.listUnit(tUnit);
			}
		}
	}
}
