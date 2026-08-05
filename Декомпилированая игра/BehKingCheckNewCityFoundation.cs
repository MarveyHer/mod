using System.Collections.Generic;
using ai.behaviours;
using UnityEngine;

public class BehKingCheckNewCityFoundation : BehaviourActionActor
{
	private const int MAX_MOVED = 6;

	private List<TileZone> _next_wave = new List<TileZone>();

	private List<TileZone> _wave = new List<TileZone>();

	private HashSet<TileZone> _checked_zones = new HashSet<TileZone>();

	private static Color _color1 = new Color(1f, 0f, 0f, 0.3f);

	private static Color _color2 = new Color(0f, 0f, 1f, 0.3f);

	private static Color _color3 = new Color(1f, 0.92f, 0.016f, 0.3f);

	private static Color _color4 = new Color(0f, 1f, 0f, 0.3f);

	protected override void setupErrorChecks()
	{
		base.setupErrorChecks();
		uses_families = true;
		uses_cities = true;
		uses_kingdoms = true;
	}

	public override BehResult execute(Actor pActor)
	{
		Kingdom tKingdom = pActor.kingdom;
		if (!tKingdom.hasCapital())
		{
			return BehResult.Stop;
		}
		if (hasCitiesWithoutPopulation(tKingdom))
		{
			return BehResult.Stop;
		}
		BehaviourActionBase<Actor>.world.city_zone_helper.city_place_finder.recalc();
		if (!BehaviourActionBase<Actor>.world.city_zone_helper.city_place_finder.hasPossibleZones())
		{
			return BehResult.Stop;
		}
		using ListPool<City> tPossibleCitiesForExpansion = getCityListForExpansion(tKingdom);
		if (tPossibleCitiesForExpansion.Count == 0)
		{
			return BehResult.Stop;
		}
		TileZone tZoneToPlaceCity = findZoneForExpansion(pActor, tPossibleCitiesForExpansion, out var tCityToExpandFrom);
		if (tZoneToPlaceCity == null)
		{
			return BehResult.Stop;
		}
		City tNewCity = BehaviourActionBase<Actor>.world.cities.buildNewCity(pActor, tZoneToPlaceCity);
		moveSomeUnitsToNewCity(tNewCity, tCityToExpandFrom);
		return BehResult.Continue;
	}

	private bool hasCitiesWithoutPopulation(Kingdom pKingdom)
	{
		WorldTile tCapitalTile = pKingdom.capital.getTile();
		if (tCapitalTile == null)
		{
			return false;
		}
		foreach (City tCity in pKingdom.getCities())
		{
			if (tCity.countUnits() <= 30)
			{
				WorldTile tCityTile = tCity.getTile();
				if (tCityTile != null && tCityTile.reachableFrom(tCapitalTile))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void moveSomeUnitsToNewCity(City pNewCity, City pFromCity)
	{
		int tUnitsToMove = Mathf.Min(pFromCity.units.Count, 6);
		int tUnitsMoved = 0;
		foreach (Actor tUnit in pFromCity.units.LoopRandom())
		{
			if (isPossibleToMoveUnitToCity(tUnit, pNewCity))
			{
				moveToCity(tUnit, pNewCity);
				tUnitsMoved++;
				int tNewFamilyMoved = checkUnitFamilyAndLovers(tUnit, pNewCity, tUnitsMoved);
				tUnitsMoved += tNewFamilyMoved;
				if (tUnitsMoved >= tUnitsToMove)
				{
					break;
				}
			}
		}
	}

	private int checkUnitFamilyAndLovers(Actor pActor, City pCity, int pMovedAlready)
	{
		int tNewMoved = 0;
		if (pActor.hasLover())
		{
			Actor tLover = pActor.lover;
			if (isPossibleToMoveUnitToCity(tLover, pCity))
			{
				moveToCity(tLover, pCity);
				tNewMoved++;
			}
		}
		if (pActor.hasFamily())
		{
			foreach (Actor tFamilyMember in pActor.family.units)
			{
				if (isPossibleToMoveUnitToCity(tFamilyMember, pCity))
				{
					moveToCity(tFamilyMember, pCity);
					tNewMoved++;
				}
				if (tNewMoved + pMovedAlready >= 6)
				{
					break;
				}
			}
		}
		return tNewMoved;
	}

	private void moveToCity(Actor pActor, City pCity)
	{
		pActor.stopBeingWarrior();
		pActor.joinCity(pCity);
		pActor.cancelAllBeh();
	}

	private bool isPossibleToMoveUnitToCity(Actor pUnit, City pCity)
	{
		if (pUnit.isRekt())
		{
			return false;
		}
		if (!pUnit.isAdult())
		{
			return false;
		}
		if (pUnit.isCityLeader())
		{
			return false;
		}
		if (pUnit.isKing())
		{
			return false;
		}
		if (pUnit.isArmyGroupLeader())
		{
			return false;
		}
		if (pUnit.army != null)
		{
			return false;
		}
		if (pUnit.hasLover())
		{
			if (pUnit.lover.isKing())
			{
				return false;
			}
			if (pUnit.lover.isCityLeader())
			{
				return false;
			}
		}
		if (pUnit.city == pCity)
		{
			return false;
		}
		return true;
	}

	private TileZone findZoneForExpansion(Actor pActor, ListPool<City> pPossibleCitiesToExpandFrom, out City pCityExpandFrom)
	{
		pCityExpandFrom = null;
		TileZone tResult = null;
		foreach (ref City item in pPossibleCitiesToExpandFrom)
		{
			City tCity = item;
			TileZone tGoodZone = findZoneForCityOnTheSameIsland2(pActor, tCity);
			if (tGoodZone != null)
			{
				pCityExpandFrom = tCity;
				tResult = tGoodZone;
				break;
			}
		}
		if (tResult == null)
		{
			foreach (ref City item2 in pPossibleCitiesToExpandFrom)
			{
				City tCity2 = item2;
				if (tCity2.hasTransportBoats())
				{
					TileZone tGoodZone2 = findZoneForCityOnFarIsland(pActor, tCity2);
					if (tGoodZone2 != null)
					{
						pCityExpandFrom = tCity2;
						tResult = tGoodZone2;
						break;
					}
				}
			}
		}
		return tResult;
	}

	private TileZone findZoneForCityOnFarIsland(Actor pActor, City pCity)
	{
		TileZone tBestZone = null;
		int tBestDist = int.MaxValue;
		WorldTile tCityTile = pCity.getTile();
		if (tCityTile == null)
		{
			return null;
		}
		Vector2Int tCityPos = tCityTile.pos;
		foreach (TileZone tZone in BehaviourActionBase<Actor>.world.city_zone_helper.city_place_finder.zones)
		{
			int tTempDist = Toolbox.SquaredDistVec2(tZone.centerTile.pos, tCityPos);
			if (tTempDist <= tBestDist && tCityTile.reachableFrom(tZone.centerTile) && tZone.checkCanSettleInThisBiomes(pActor.subspecies))
			{
				tBestDist = tTempDist;
				tBestZone = tZone;
			}
		}
		return tBestZone;
	}

	private TileZone findZoneForCityOnTheSameIsland2(Actor pActor, City pMainCity)
	{
		WorldTile tMainTile = pMainCity.getTile();
		if (tMainTile == null)
		{
			return null;
		}
		using ListPool<TileZone> tGoodZones = new ListPool<TileZone>();
		bool tDebug = DebugConfig.isOn(DebugOption.CitySettleCalc);
		if (tDebug)
		{
			DebugHighlight.clear();
		}
		foreach (City tCity in pMainCity.kingdom.getCities())
		{
			if (tCity != pMainCity)
			{
				continue;
			}
			WorldTile tCityTile = tCity.getTile();
			if (tCityTile == null || !tMainTile.isSameIsland(tCityTile))
			{
				continue;
			}
			foreach (TileZone tZone in tCity.neighbour_zones)
			{
				if (!tZone.hasCity())
				{
					_wave.Add(tZone);
					_checked_zones.Add(tZone);
				}
			}
		}
		int tCurWave = 0;
		while (_wave.Count > 0)
		{
			if (tDebug)
			{
				switch (tCurWave)
				{
				case 0:
					DebugHighlight.newHighlightList(_color1, _wave);
					break;
				case 1:
					DebugHighlight.newHighlightList(_color2, _wave);
					break;
				case 2:
					DebugHighlight.newHighlightList(_color3, _wave);
					break;
				case 3:
					DebugHighlight.newHighlightList(_color4, _wave);
					break;
				}
			}
			startWave(tCurWave, tMainTile, tGoodZones, pActor);
			if (_next_wave.Count > 0)
			{
				_wave.AddRange(_next_wave);
				_next_wave.Clear();
				tCurWave++;
				if (tCurWave >= 4)
				{
					break;
				}
			}
		}
		_wave.Clear();
		_checked_zones.Clear();
		if (tGoodZones.Count == 0)
		{
			return null;
		}
		return tGoodZones.GetRandom();
	}

	private void startWave(int pWave, WorldTile pCityTile, ListPool<TileZone> pGoodZones, Actor pActor)
	{
		List<TileZone> tWave = _wave;
		HashSet<TileZone> tCheckedZones = _checked_zones;
		while (tWave.Count > 0)
		{
			TileZone tParentZone = tWave.Pop();
			tCheckedZones.Add(tParentZone);
			if (pWave > 2 && tParentZone.isGoodForNewCity(pActor) && tParentZone.centerTile.isSameIsland(pCityTile))
			{
				pGoodZones.Add(tParentZone);
			}
			TileZone[] tNeighbours = tParentZone.neighbours;
			foreach (TileZone tNeighbour in tNeighbours)
			{
				if (tCheckedZones.Add(tNeighbour) && !tNeighbour.hasCity())
				{
					_next_wave.Add(tNeighbour);
				}
			}
		}
	}

	private ListPool<City> getCityListForExpansion(Kingdom pKingdom)
	{
		ListPool<City> tPossibleCitiesForExpansion = new ListPool<City>(pKingdom.countCities());
		foreach (City tCity in pKingdom.getCities())
		{
			if (tCity.getTile() != null && tCity.status.population_adults >= 30 && !tCity.needSettlers())
			{
				tPossibleCitiesForExpansion.Add(tCity);
			}
		}
		tPossibleCitiesForExpansion.Shuffle();
		return tPossibleCitiesForExpansion;
	}

	private TileZone findCityForMigration(City pCity)
	{
		WorldTile tCityTile = pCity.getTile();
		if (tCityTile == null)
		{
			return null;
		}
		foreach (City tCity in pCity.kingdom.getCities().LoopRandom())
		{
			if (tCity == pCity)
			{
				continue;
			}
			WorldTile tTargetTile = tCity.getTile();
			if (tTargetTile != null && tCityTile.reachableFrom(tTargetTile) && tCity.needSettlers())
			{
				TileZone tBestZone = tCity.getTile()?.zone;
				if (tBestZone != null)
				{
					return tBestZone;
				}
			}
		}
		return null;
	}
}
