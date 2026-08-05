using System;
using System.Collections.Generic;
using ai;
using EpPathFinding.cs;
using UnityEngine;

public class ActorMove
{
	private const float CHUNK_MULTIPLIER = 4f;

	public static ExecuteEvent goTo(Actor pActor, WorldTile pTileTarget, bool pPathOnLiquid = false, bool pWalkOnBlocks = false, bool pPathOnLava = false, int pLimitPathfindingRegions = 0)
	{
		pActor.clearOldPath();
		ActorAsset tActorAsset = pActor.asset;
		bool tIsBoat = tActorAsset.is_boat;
		if (!DebugConfig.isOn(DebugOption.SystemUnitPathfinding))
		{
			pActor.current_path.Add(pTileTarget);
			return ExecuteEvent.True;
		}
		if (tIsBoat && !pTileTarget.isGoodForBoat())
		{
			return ExecuteEvent.False;
		}
		if (pActor.isFlying())
		{
			pActor.current_path.Add(pTileTarget);
			return ExecuteEvent.True;
		}
		WorldTile tCurrentTile = pActor.current_tile;
		bool tIsSameIsland = tCurrentTile.isSameIsland(pTileTarget);
		bool tIsWaterCreature = pActor.isWaterCreature();
		bool tIsInLiquid = pActor.isInLiquid();
		bool tFireImmune = pActor.isImmuneToFire();
		if (tIsSameIsland && !tIsInLiquid)
		{
			pPathOnLiquid = false;
		}
		AStarParam tPathfindingParam = World.world.pathfinding_param;
		tPathfindingParam.resetParam();
		tPathfindingParam.block = pWalkOnBlocks;
		tPathfindingParam.lava = tFireImmune;
		tPathfindingParam.fire = tFireImmune;
		if (pActor.hasStatus("burning") || tCurrentTile.isOnFire())
		{
			tPathfindingParam.fire = true;
		}
		if (tCurrentTile.Type.lava)
		{
			tPathfindingParam.lava = true;
		}
		if (pPathOnLava)
		{
			tPathfindingParam.lava = true;
		}
		tPathfindingParam.ocean = tIsWaterCreature;
		if (pPathOnLiquid && !pActor.isDamagedByOcean())
		{
			tPathfindingParam.ocean = true;
		}
		else if (tIsInLiquid)
		{
			tPathfindingParam.ocean = true;
		}
		tPathfindingParam.ground = !tIsWaterCreature || tActorAsset.force_land_creature;
		if (tIsWaterCreature && !pActor.isInWater())
		{
			tPathfindingParam.ground = true;
		}
		tPathfindingParam.boat = tIsBoat && tCurrentTile.isGoodForBoat();
		tPathfindingParam.limit = true;
		if (PathfinderTools.tryToGetSimplePath(tCurrentTile, pTileTarget, pActor.current_path, tActorAsset, tPathfindingParam))
		{
			int tTileLimit = (int)((float)pLimitPathfindingRegions * 4f);
			List<WorldTile> tRaycastPath = PathfinderTools.getLastRaycastResult();
			if (pLimitPathfindingRegions > 0 && tRaycastPath.Count > tTileLimit)
			{
				tRaycastPath.RemoveRange(tTileLimit, tRaycastPath.Count - tTileLimit);
				pActor.current_path.Add(tRaycastPath.Last());
			}
			else
			{
				pActor.current_path.Add(pTileTarget);
			}
		}
		World.world.path_finding_visualiser.showPath(null, pActor.current_path);
		if (pActor.isUsingPath())
		{
			pActor.setTileTarget(pTileTarget);
			return ExecuteEvent.True;
		}
		if (!tIsSameIsland)
		{
			TileTypeBase tTargetType = pTileTarget.Type;
			if ((!tTargetType.ground || !tPathfindingParam.ground) && (!tTargetType.ocean || !tPathfindingParam.ocean) && (!tTargetType.lava || !tPathfindingParam.lava) && (!tTargetType.block || !tPathfindingParam.block))
			{
				return ExecuteEvent.False;
			}
			TileIsland tActorIsland = tCurrentTile.region.island;
			TileIsland tTargetIsland = pTileTarget.region.island;
			if (tTargetIsland.getTileCount() < tActorIsland.getTileCount())
			{
				if (!tTargetIsland.isConnectedWith(tActorIsland))
				{
					return ExecuteEvent.False;
				}
				tPathfindingParam.end_to_start_path = true;
			}
			else if (!tActorIsland.isConnectedWith(tTargetIsland))
			{
				return ExecuteEvent.False;
			}
		}
		bool tUseRegionSearchLock = DebugConfig.isOn(DebugOption.UseGlobalPathLock);
		if (tUseRegionSearchLock)
		{
			if (tIsBoat)
			{
				tUseRegionSearchLock = true;
			}
			else if (!tIsSameIsland)
			{
				tUseRegionSearchLock = false;
			}
		}
		PathFinderResult tResult = PathFinderResult.PathFound;
		if (tUseRegionSearchLock)
		{
			switch (World.world.region_path_finder.getGlobalPath(tCurrentTile, pTileTarget, tIsBoat))
			{
			case PathFinderResult.SamePlace:
				pActor.current_path.Add(pTileTarget);
				return ExecuteEvent.True;
			case PathFinderResult.NotFound:
				return ExecuteEvent.True;
			case PathFinderResult.DifferentIslands:
				return ExecuteEvent.True;
			}
			if (World.world.region_path_finder.last_globalPath != null)
			{
				pActor.current_path_global = World.world.region_path_finder.last_globalPath;
				lockRegionsForAStarSearch(pActor, pLimitPathfindingRegions);
			}
			else
			{
				tCurrentTile.region.used_by_path_lock = true;
				tCurrentTile.region.region_path_id = 0;
			}
		}
		tPathfindingParam.use_global_path_lock = tUseRegionSearchLock;
		WorldTile tAstarTarget = pTileTarget;
		if (tUseRegionSearchLock && pActor.current_path_global != null && pActor.current_path_global.Count > 4)
		{
			int tIndex = 4;
			tAstarTarget = pActor.current_path_global[tIndex].getRandomTile();
		}
		World.world.calcPath(tCurrentTile, tAstarTarget, pActor.current_path);
		if (AStarFinder.result_split_path)
		{
			pActor.split_path = SplitPathStatus.Prepare;
		}
		if (tUseRegionSearchLock)
		{
			cleanRegionSearch(pActor);
		}
		if (pLimitPathfindingRegions > 0 && pActor.current_path_global != null)
		{
			pTileTarget = pActor.current_path_global.Last().getRandomTile();
		}
		pActor.setTileTarget(pTileTarget);
		return ExecuteEvent.True;
	}

	private static void lockRegionsForAStarSearch(Actor pActor, int pLimitPathfindingRegions)
	{
		int tRegionPathID = 0;
		List<MapRegion> pRegionsPath = pActor.current_path_global;
		int tLen = getLimitedPathfindingRegions(pRegionsPath, pLimitPathfindingRegions);
		for (int i = 0; i < tLen; i++)
		{
			MapRegion mapRegion = pRegionsPath[i];
			mapRegion.used_by_path_lock = true;
			mapRegion.region_path_id = tRegionPathID++;
		}
		if (tLen < pRegionsPath.Count)
		{
			pRegionsPath.RemoveRange(tLen, pRegionsPath.Count - tLen);
		}
	}

	private static int getLimitedPathfindingRegions(List<MapRegion> pPath, int pLimitPathfindingRegions)
	{
		if (pLimitPathfindingRegions <= 0)
		{
			return pPath.Count;
		}
		return Mathf.Min(pPath.Count, pLimitPathfindingRegions);
	}

	private static void cleanRegionSearch(Actor pActor)
	{
		if (pActor.current_path_global != null)
		{
			List<MapRegion> pRegionsPath = pActor.current_path_global;
			for (int i = 0; i < pRegionsPath.Count; i++)
			{
				MapRegion mapRegion = pRegionsPath[i];
				mapRegion.used_by_path_lock = false;
				mapRegion.region_path_id = -1;
			}
		}
		MapRegion region = pActor.current_tile.region;
		region.used_by_path_lock = false;
		region.region_path_id = -1;
	}

	public static ExecuteEvent goToCurved(Actor pActor, params WorldTile[] pTargets)
	{
		Span<Vector3> span = ((pTargets.Length >= 128) ? ((Span<Vector3>)new Vector3[pTargets.Length]) : stackalloc Vector3[pTargets.Length]);
		Span<Vector3> tTargets = span;
		for (int i = 0; i < pTargets.Length; i++)
		{
			tTargets[i] = pTargets[i].posV3;
		}
		pActor.clearOldPath();
		float tTotalDistance = 0f;
		for (int j = 1; j < pTargets.Length; j++)
		{
			WorldTile tPrevTile = pTargets[j - 1];
			WorldTile tNextTile = pTargets[j];
			tTotalDistance += Toolbox.DistTile(tPrevTile, tNextTile);
		}
		float tPoints = (int)(tTotalDistance / 4f);
		tPoints = Mathf.Clamp(tPoints, 5f, 100f);
		for (int k = 0; (float)k < tPoints; k++)
		{
			Vector2 tPos = Toolbox.cubeBezierN(Toolbox.easeInOutQuart((float)(k + 1) / tPoints), tTargets);
			Toolbox.clampToMap(ref tPos);
			WorldTile tTile = World.world.GetTileSimple((int)tPos.x, (int)tPos.y);
			if (pActor.current_path.Count <= 0 || pActor.current_path.Last() != tTile)
			{
				pActor.current_path.Add(tTile);
			}
		}
		WorldTile tFinalTarget = pTargets.Last();
		pActor.current_path.Add(tFinalTarget);
		World.world.path_finding_visualiser.showPath(null, pActor.current_path);
		pActor.setTileTarget(tFinalTarget);
		return ExecuteEvent.True;
	}
}
