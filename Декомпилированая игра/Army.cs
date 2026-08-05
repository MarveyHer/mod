using System.Collections.Generic;
using JetBrains.Annotations;

public class Army : MetaObject<ArmyData>
{
	private Actor _captain;

	private WorldTile _prev_captain_position;

	private City _city;

	private Kingdom _kingdom;

	protected override MetaType meta_type => MetaType.Army;

	public override BaseSystemManager manager => World.world.armies;

	public override ActorAsset getActorAsset()
	{
		return getKingdom().getActorAsset();
	}

	public void createArmy(Actor pActor, City pCity)
	{
		_city = pCity;
		_kingdom = _city.kingdom;
		setCaptain(pActor);
		generateNewMetaObject();
		generateName();
	}

	public void checkCity()
	{
		if (_city.kingdom != _kingdom)
		{
			_kingdom = _city.kingdom;
			updateColor(_kingdom?.getColor());
			generateName(_kingdom);
		}
	}

	public void onKingdomNameChange()
	{
		generateName();
	}

	protected override void generateColor()
	{
		if (isAlive())
		{
			Kingdom tKingdom = getKingdom();
			if (!tKingdom.isRekt())
			{
				data.setColorID(tKingdom.data.color_id);
			}
		}
	}

	public override void trackName(bool pPostChange = false)
	{
		if (!string.IsNullOrEmpty(data.name) && (!pPostChange || (data.past_names != null && data.past_names.Count != 0)))
		{
			ArmyData armyData = data;
			if (armyData.past_names == null)
			{
				armyData.past_names = new List<NameEntry>();
			}
			if (data.past_names.Count == 0)
			{
				NameEntry tNewEntry = new NameEntry(data.name, pCustom: false, data.original_color_id, data.created_time);
				data.past_names.Add(tNewEntry);
			}
			else if (!(data.past_names.Last().name == data.name))
			{
				NameEntry tNewEntry2 = new NameEntry(data.name, data.custom_name, data.color_id);
				data.past_names.Add(tNewEntry2);
			}
		}
	}

	private void generateName(Kingdom pKingdom = null)
	{
		if (data.custom_name && !string.IsNullOrEmpty(data.name))
		{
			return;
		}
		Kingdom tKingdom = null;
		tKingdom = ((pKingdom == null) ? getKingdom() : pKingdom);
		if (tKingdom == null)
		{
			setName("Forgotten Army");
			return;
		}
		string tKingdomName = tKingdom.name ?? "";
		string text = data.name;
		if (text != null && text.StartsWith(tKingdomName + " "))
		{
			return;
		}
		using ListPool<string> tPoolArmies = new ListPool<string>(World.world.armies.Count);
		foreach (Army tArmy in World.world.armies)
		{
			if (tArmy != this && tArmy.getKingdom() == tKingdom)
			{
				tPoolArmies.Add(tArmy.name);
			}
		}
		int tCountID = 1;
		string tNewArmyName;
		while (true)
		{
			string tRoman = tCountID.ToRoman();
			tNewArmyName = tKingdomName + " " + tRoman;
			if (!tPoolArmies.Contains(tNewArmyName))
			{
				break;
			}
			tCountID++;
		}
		setName(tNewArmyName);
	}

	public Actor getCaptain()
	{
		return _captain;
	}

	public override void save()
	{
		base.save();
		data.id_city = _city?.id ?? (-1);
		data.id_kingdom = _city?.kingdom?.id ?? _kingdom?.id ?? (-1);
		data.id_captain = (hasCaptain() ? _captain.data.id : (-1));
	}

	public override void loadData(ArmyData pData)
	{
		base.loadData(pData);
		_city = World.world.cities.get(pData.id_city);
		if (_city != null)
		{
			_city.setArmy(this);
		}
		_kingdom = World.world.kingdoms.get(pData.id_kingdom);
		if (string.IsNullOrEmpty(name))
		{
			generateName();
		}
	}

	public void loadDataCaptains()
	{
		Actor tActor = World.world.units.get(data.id_captain);
		if (tActor != null && tActor.army == this)
		{
			setCaptain(tActor, pFromLoad: true);
		}
		updateColor(getColor());
	}

	public override void generateBanner()
	{
	}

	protected override ColorLibrary getColorLibrary()
	{
		return AssetManager.kingdom_colors_library;
	}

	public override ColorAsset getColor()
	{
		return getKingdom().getColor();
	}

	public void clearCity()
	{
		_city = null;
		data.id_city = -1L;
	}

	public void disband()
	{
		for (int i = 0; i < base.units.Count; i++)
		{
			base.units[i].stopBeingWarrior();
		}
		setCaptain(null);
	}

	public void updateCaptains()
	{
		if (data.past_captains == null || data.past_captains.Count == 0)
		{
			return;
		}
		foreach (LeaderEntry tEntry in data.past_captains)
		{
			Actor tRuler = World.world.units.get(tEntry.id);
			if (!tRuler.isRekt())
			{
				tEntry.name = tRuler.name;
			}
		}
	}

	public void addCaptain(Actor pActor)
	{
		ArmyData armyData = data;
		if (armyData.past_captains == null)
		{
			armyData.past_captains = new List<LeaderEntry>();
		}
		captainLeft();
		data.past_captains.Add(new LeaderEntry
		{
			id = pActor.getID(),
			name = pActor.name,
			color_id = (getKingdom()?.data.color_id ?? data.color_id),
			timestamp_ago = World.world.getCurWorldTime()
		});
		if (data.past_captains.Count > 30)
		{
			data.past_captains.Shift();
		}
	}

	public void captainLeft()
	{
		if (data.past_captains != null && data.past_captains.Count != 0)
		{
			LeaderEntry tLast = data.past_captains.Last();
			if (!(tLast.timestamp_end >= tLast.timestamp_ago))
			{
				tLast.timestamp_end = World.world.getCurWorldTime();
				updateCaptains();
			}
		}
	}

	public void setCaptain(Actor pActor, bool pFromLoad = false)
	{
		_captain = pActor;
		if (data == null)
		{
			return;
		}
		if (pActor.isRekt())
		{
			data.id_captain = -1L;
			if (!pFromLoad)
			{
				captainLeft();
			}
		}
		else
		{
			data.id_captain = pActor.getID();
			if (!pFromLoad)
			{
				addCaptain(pActor);
			}
		}
	}

	public void checkCaptainExistence()
	{
		Actor tCaptain = getCaptain();
		if (!tCaptain.isRekt() && tCaptain.current_tile != null)
		{
			_prev_captain_position = tCaptain.current_tile;
		}
		if (tCaptain.isRekt())
		{
			setCaptain(null);
		}
		findCaptain();
	}

	public void checkCaptainRemoval(Actor pActor)
	{
		if (_captain == pActor)
		{
			setCaptain(null);
		}
	}

	public int countMelee()
	{
		int tResult = 0;
		for (int i = 0; i < base.units.Count; i++)
		{
			Actor tActor = base.units[i];
			if (tActor.isAlive())
			{
				if (!tActor.hasWeapon())
				{
					tResult++;
				}
				else if (tActor.getWeaponAsset().attack_type == WeaponType.Melee)
				{
					tResult++;
				}
			}
		}
		return tResult;
	}

	public int countRange()
	{
		int tResult = 0;
		for (int i = 0; i < base.units.Count; i++)
		{
			Actor tActor = base.units[i];
			if (tActor.isAlive() && tActor.hasWeapon() && tActor.getWeaponAsset().attack_type == WeaponType.Range)
			{
				tResult++;
			}
		}
		return tResult;
	}

	public bool isGroupInCityAndHaveLeader()
	{
		if (!isAlive())
		{
			return false;
		}
		if (base.units.Count == 0)
		{
			return true;
		}
		if (!hasCaptain())
		{
			return false;
		}
		Actor tCaptain = getCaptain();
		if (tCaptain.isInsideSomething())
		{
			return false;
		}
		if (!tCaptain.current_zone.isSameCityHere(_city))
		{
			return false;
		}
		return true;
	}

	private void findCaptain()
	{
		if (isLocked())
		{
			return;
		}
		if (hasCaptain())
		{
			if (getCaptain().isKingdomCiv())
			{
				return;
			}
			setCaptain(null);
		}
		if (base.units.Count != 0)
		{
			Actor tBest = null;
			tBest = ((_prev_captain_position != null) ? getNearbyUnitForCaptain(_prev_captain_position) : getRandomActorForCaptain());
			if (tBest != null)
			{
				setCaptain(tBest);
			}
		}
	}

	private Actor getRandomActorForCaptain()
	{
		foreach (Actor tActor in base.units.LoopRandom())
		{
			if (!tActor.isRekt() && tActor.army == this)
			{
				return tActor;
			}
		}
		return null;
	}

	private Actor getNearbyUnitForCaptain(WorldTile pLastPosition)
	{
		Actor tBest = null;
		int tBestFastDist = int.MaxValue;
		List<Actor> list = base.units;
		for (int i = 0; i < list.Count; i++)
		{
			Actor tActor = list[i];
			if (tActor.army == this && !tActor.isRekt())
			{
				int tFastDist = Toolbox.SquaredDistTile(tActor.current_tile, pLastPosition);
				if (tFastDist < tBestFastDist)
				{
					tBest = tActor;
					tBestFastDist = tFastDist;
				}
			}
		}
		return tBest;
	}

	public string getDebug()
	{
		string tResult = base.units.Count.ToString() ?? "";
		if (_captain != null)
		{
			tResult = tResult + " " + _captain.getName() + "(" + _captain.getAge() + ")";
		}
		return tResult;
	}

	[CanBeNull]
	public Kingdom getKingdom()
	{
		Kingdom tKingdom = null;
		if (hasCaptain())
		{
			tKingdom = getCaptain().kingdom;
		}
		if (tKingdom == null)
		{
			tKingdom = (_city.isRekt() ? _kingdom : _city.kingdom);
		}
		return tKingdom;
	}

	public bool hasKingdom()
	{
		return !_kingdom.isRekt();
	}

	public bool hasCaptain()
	{
		return !_captain.isRekt();
	}

	public City getCity()
	{
		return _city;
	}

	public bool hasCity()
	{
		return !_city.isRekt();
	}

	public override bool isReadyForRemoval()
	{
		if (base.units.Count > 0)
		{
			return false;
		}
		if (hasCaptain())
		{
			return false;
		}
		if (hasCity())
		{
			return false;
		}
		if (!base.isReadyForRemoval())
		{
			return false;
		}
		return true;
	}

	public override void Dispose()
	{
		base.Dispose();
		base.units.Clear();
		_captain = null;
		_prev_captain_position = null;
		_city = null;
		_kingdom = null;
	}

	public override string ToString()
	{
		if (data == null)
		{
			return "[Army is null]";
		}
		using StringBuilderPool tBuilder = new StringBuilderPool();
		tBuilder.Append($"[Army:{base.id} ");
		if (!isAlive())
		{
			tBuilder.Append("[DEAD] ");
		}
		tBuilder.Append("\"" + name + "\" ");
		Kingdom tKingdom = getKingdom();
		tBuilder.Append($"Kingdom:{tKingdom?.id ?? (-1)} ");
		if (hasCity())
		{
			tBuilder.Append($"{_city} ");
		}
		if (tKingdom != _kingdom)
		{
			tBuilder.Append($"_kingdom:{_kingdom} ");
		}
		if (hasCaptain())
		{
			tBuilder.Append($"Captain:{_captain?.id ?? (-1)} ");
			if (_captain?.kingdom != tKingdom)
			{
				tBuilder.Append($"CaptainKingdom:{_captain?.kingdom?.id ?? (-1)} ");
			}
		}
		tBuilder.Append($"Units:{base.units.Count} ");
		if (manager.isUnitsDirty())
		{
			tBuilder.Append("[Dirty] ");
		}
		return tBuilder.ToString().Trim() + "]";
	}
}
