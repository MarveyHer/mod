using FMOD.Studio;
using UnityEngine;

public class DebugTextGroupSystem : SpriteGroupSystem<GroupSpriteObject>
{
	private Vector2 _pos;

	public override void create()
	{
		base.create();
		base.transform.name = "Debug Text";
		GameObject tPrefab = (GameObject)Resources.Load("Prefabs/PrefabDebugText");
		prefab = tPrefab.GetComponent<GroupSpriteObject>();
	}

	protected override GroupSpriteObject createNew()
	{
		GroupSpriteObject groupSpriteObject = base.createNew();
		groupSpriteObject.GetComponent<DebugWorldText>().create();
		return groupSpriteObject;
	}

	public override void update(float pElapsed)
	{
		prepare();
		checkSoundsAttached();
		checkSounds();
		checkSoundsPlaying();
		checkActors();
		checkBoats();
		checkBuildings();
		checkCitiesOverlay();
		checkCitiesTasksOverlay();
		checkKingdoms();
		checkArmies();
		checkZones();
		base.update(pElapsed);
	}

	private void checkSoundsPlaying()
	{
		if (!DebugConfig.isOn(DebugOption.OverlaySoundsActive) || MapBox.isRenderMiniMap())
		{
			return;
		}
		foreach (DebugMusicBoxData tData in MusicBox.inst.debug_box.list)
		{
			if (tData.isPlaying())
			{
				GroupSpriteObject next = getNext();
				_pos.x = tData.x;
				_pos.y = tData.y;
				next.GetComponent<DebugWorldText>().setTextFmodSound(tData, Color.green);
				next.setPosOnly(ref _pos);
			}
		}
	}

	private void checkSounds()
	{
		if (!DebugConfig.isOn(DebugOption.OverlaySounds) || MapBox.isRenderMiniMap())
		{
			return;
		}
		foreach (DebugMusicBoxData tData in MusicBox.inst.debug_box.list)
		{
			GroupSpriteObject next = getNext();
			_pos.x = tData.x;
			_pos.y = tData.y;
			next.GetComponent<DebugWorldText>().setTextFmodSound(tData);
			next.setPosOnly(ref _pos);
		}
	}

	private void checkSoundsAttached()
	{
		if (!DebugConfig.isOn(DebugOption.OverlaySoundsAttached) || MapBox.isRenderMiniMap())
		{
			return;
		}
		foreach (EventInstance tInstance in MusicBox.inst.idle.currentAttachedSounds.Values)
		{
			GroupSpriteObject next = getNext();
			tInstance.get3DAttributes(out var tAttributes);
			_pos.x = tAttributes.position.x;
			_pos.y = tAttributes.position.y;
			next.GetComponent<DebugWorldText>().setTextFmodSound(tInstance);
			next.setPosOnly(ref _pos);
		}
		foreach (QuantumSpriteAsset item in AssetManager.quantum_sprites.list)
		{
			int tActive = item.group_system.countActive();
			QuantumSprite[] tQSprites = item.group_system.getAll();
			for (int i = 0; i < tActive; i++)
			{
				QuantumSprite tSprite = tQSprites[i];
				if (tSprite.fmod_instance.isValid())
				{
					tSprite.fmod_instance.get3DAttributes(out var tAttributes2);
					_pos.x = tAttributes2.position.x;
					_pos.y = tAttributes2.position.y;
					GroupSpriteObject next2 = getNext();
					next2.GetComponent<DebugWorldText>().setTextFmodSound(tSprite.fmod_instance);
					next2.setPosOnly(ref _pos);
				}
			}
		}
		Actor[] tArr = World.world.units.visible_units.array;
		int tLen = World.world.units.visible_units.count;
		for (int j = 0; j < tLen; j++)
		{
			Actor tActor = tArr[j];
			if (tActor.idle_loop_sound != null && tActor.idle_loop_sound.fmod_instance.isValid())
			{
				tActor.idle_loop_sound.fmod_instance.get3DAttributes(out var tAttributes3);
				_pos.x = tAttributes3.position.x;
				_pos.y = tAttributes3.position.y;
				GroupSpriteObject next3 = getNext();
				next3.GetComponent<DebugWorldText>().setTextFmodSound(tActor.idle_loop_sound.fmod_instance);
				next3.setPosOnly(ref _pos);
			}
		}
	}

	private void checkBoats()
	{
		if (!DebugConfig.isOn(DebugOption.OverlayBoatTransport))
		{
			return;
		}
		foreach (Actor tActor in World.world.units)
		{
			bool tShow = false;
			if (tActor.asset.is_boat)
			{
				tShow = true;
			}
			if (tShow)
			{
				GroupSpriteObject next = getNext();
				_pos.x = tActor.current_position.x;
				_pos.y = tActor.current_position.y;
				next.GetComponent<DebugWorldText>().setTextBoat(tActor);
				next.setPosOnly(ref _pos);
			}
		}
	}

	private void checkActors()
	{
		if ((!DebugConfig.isOn(DebugOption.OverlayActorCivs) && !DebugConfig.isOn(DebugOption.OverlayCursorActor) && !DebugConfig.isOn(DebugOption.OverlayActorGroupLeaderOnly) && !DebugConfig.isOn(DebugOption.OverlayActorFavoritesOnly) && !DebugConfig.isOn(DebugOption.OverlayActorMobs)) || MapBox.isRenderMiniMap())
		{
			return;
		}
		Actor[] tArr = World.world.units.visible_units.array;
		int tLen = World.world.units.visible_units.count;
		for (int i = 0; i < tLen; i++)
		{
			Actor tActor = tArr[i];
			bool tShow = false;
			if (DebugConfig.isOn(DebugOption.OverlayCursorActor) && UnitSelectionEffect.last_actor == tActor)
			{
				tShow = true;
			}
			if (DebugConfig.isOn(DebugOption.OverlayActorFavoritesOnly) && tActor.isFavorite())
			{
				tShow = true;
			}
			if (DebugConfig.isOn(DebugOption.OverlayActorGroupLeaderOnly) && tActor.is_army_captain)
			{
				tShow = true;
			}
			if (tActor.isSapient() && DebugConfig.isOn(DebugOption.OverlayActorCivs))
			{
				tShow = true;
			}
			if (!tActor.isSapient() && DebugConfig.isOn(DebugOption.OverlayActorMobs))
			{
				tShow = true;
			}
			if (tShow)
			{
				GroupSpriteObject next = getNext();
				_pos.x = tActor.current_position.x;
				_pos.y = tActor.current_position.y;
				next.GetComponent<DebugWorldText>().setTextActor(tActor);
				next.setPosOnly(ref _pos);
			}
		}
	}

	private void checkBuildings()
	{
		if ((!DebugConfig.isOn(DebugOption.OverlayTrees) && !DebugConfig.isOn(DebugOption.OverlayPlants) && !DebugConfig.isOn(DebugOption.OverlayCivBuildings) && !DebugConfig.isOn(DebugOption.OverlayOtherBuildings)) || MapBox.isRenderMiniMap())
		{
			return;
		}
		int tLen = World.world.buildings.countVisibleBuildings();
		Building[] tBuildings = World.world.buildings.getVisibleBuildings();
		for (int i = 0; i < tLen; i++)
		{
			Building tObj = tBuildings[i];
			if (tObj.asset.city_building)
			{
				if (!DebugConfig.isOn(DebugOption.OverlayCivBuildings))
				{
					continue;
				}
			}
			else if (tObj.asset.building_type == BuildingType.Building_Tree)
			{
				if (!DebugConfig.isOn(DebugOption.OverlayTrees))
				{
					continue;
				}
			}
			else if (tObj.asset.building_type == BuildingType.Building_Plant)
			{
				if (!DebugConfig.isOn(DebugOption.OverlayPlants))
				{
					continue;
				}
			}
			else if (!DebugConfig.isOn(DebugOption.OverlayOtherBuildings))
			{
				continue;
			}
			GroupSpriteObject next = getNext();
			_pos.x = tObj.current_position.x;
			_pos.y = tObj.current_position.y;
			next.GetComponent<DebugWorldText>().setTextBuilding(tObj);
			next.setPosOnly(ref _pos);
		}
	}

	private void checkZones()
	{
		if (!DebugConfig.isOn(DebugOption.DebugZones))
		{
			return;
		}
		foreach (TileZone tZone in World.world.zone_calculator.zones)
		{
			if (tZone.debug_show)
			{
				GroupSpriteObject next = getNext();
				_pos.x = tZone.centerTile.pos.x;
				_pos.y = tZone.centerTile.pos.y;
				next.GetComponent<DebugWorldText>().setTextZone(tZone);
				next.setPosOnly(ref _pos);
			}
		}
	}

	private void checkArmies()
	{
		if (!DebugConfig.isOn(DebugOption.OverlayArmies) || MapBox.isRenderMiniMap())
		{
			return;
		}
		foreach (Army tArmy in World.world.armies)
		{
			if (tArmy.hasCaptain())
			{
				Actor tCaptain = tArmy.getCaptain();
				GroupSpriteObject next = getNext();
				_pos.x = tCaptain.current_position.x;
				_pos.y = tCaptain.current_position.y;
				next.GetComponent<DebugWorldText>().setTextArmy(tArmy);
				next.setPosOnly(ref _pos);
			}
		}
	}

	private void checkCitiesOverlay()
	{
		if (!DebugConfig.isOn(DebugOption.OverlayCity))
		{
			return;
		}
		foreach (City tCity in World.world.cities)
		{
			GroupSpriteObject next = getNext();
			_pos.x = tCity.city_center.x;
			_pos.y = tCity.city_center.y;
			next.GetComponent<DebugWorldText>().setTextCity(tCity);
			next.setPosOnly(ref _pos);
		}
	}

	private void checkCitiesTasksOverlay()
	{
		if (!DebugConfig.isOn(DebugOption.OverlayCityTasks))
		{
			return;
		}
		foreach (City tCity in World.world.cities)
		{
			GroupSpriteObject next = getNext();
			_pos.x = tCity.city_center.x;
			_pos.y = tCity.city_center.y;
			next.GetComponent<DebugWorldText>().setTextCityTasks(tCity);
			next.setPosOnly(ref _pos);
		}
	}

	private void checkKingdoms()
	{
		if (!DebugConfig.isOn(DebugOption.OverlayKingdom))
		{
			return;
		}
		foreach (Kingdom tKingdom in World.world.kingdoms)
		{
			if (tKingdom.hasCapital())
			{
				GroupSpriteObject next = getNext();
				_pos.x = tKingdom.capital.city_center.x;
				_pos.y = tKingdom.capital.city_center.y;
				next.GetComponent<DebugWorldText>().setTextKingdom(tKingdom);
				next.setPosOnly(ref _pos);
			}
		}
	}
}
