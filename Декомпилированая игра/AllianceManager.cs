using System.Collections.Generic;
using UnityEngine;

public class AllianceManager : MetaSystemManager<Alliance, AllianceData>
{
	public Sprite[] _cached_banner_backgrounds;

	public Sprite[] _cached_banner_icons;

	private List<Alliance> _to_dissolve = new List<Alliance>();

	public AllianceManager()
	{
		type_id = "alliance";
	}

	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		using (IEnumerator<Alliance> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Alliance tAlliance = enumerator.Current;
				tAlliance.clearCursorOver();
				if (!tAlliance.checkActive())
				{
					_to_dissolve.Add(tAlliance);
				}
				else
				{
					tAlliance.update();
				}
			}
		}
		foreach (Alliance tAlliance2 in _to_dissolve)
		{
			dissolveAlliance(tAlliance2);
		}
		_to_dissolve.Clear();
	}

	public void dissolveAlliance(Alliance pAlliance)
	{
		World.world.game_stats.data.alliancesDissolved++;
		World.world.map_stats.alliancesDissolved++;
		WorldLog.logAllianceDisolved(pAlliance);
		pAlliance.dissolve();
		removeObject(pAlliance);
	}

	private void addTest()
	{
	}

	public bool forceAlliance(Kingdom pKingdom1, Kingdom pKingdom2)
	{
		Alliance tCurAlliance = pKingdom1.getAlliance();
		if (tCurAlliance == null)
		{
			tCurAlliance = pKingdom2.getAlliance();
		}
		bool tNew = false;
		if (tCurAlliance == null)
		{
			tCurAlliance = newAlliance(pKingdom1, pKingdom2);
			tNew = true;
		}
		else
		{
			tCurAlliance.join(pKingdom1, pRecalc: true, pForce: true);
			tCurAlliance.join(pKingdom2, pRecalc: true, pForce: true);
		}
		tCurAlliance.setType(AllianceType.Forced);
		return tNew;
	}

	public void useDiscordPower(Alliance pAlliance, City pCity)
	{
		Kingdom tKingdom = pCity.kingdom;
		pAlliance.leave(tKingdom);
		EffectsLibrary.highlightKingdomZones(tKingdom, Color.white);
		if (pAlliance.kingdoms_hashset.Count == 0)
		{
			dissolveAlliance(pAlliance);
		}
	}

	public Alliance newAlliance(Kingdom pKingdom, Kingdom pKingdom2)
	{
		World.world.game_stats.data.alliancesMade++;
		World.world.map_stats.alliancesMade++;
		Alliance alliance = newObject();
		alliance.createNewAlliance();
		alliance.addFounders(pKingdom, pKingdom2);
		WorldLog.logAllianceCreated(alliance);
		return alliance;
	}

	public Sprite[] getBackgroundsList()
	{
		if (_cached_banner_backgrounds == null)
		{
			_cached_banner_backgrounds = SpriteTextureLoader.getSpriteList("alliances/backgrounds/");
		}
		return _cached_banner_backgrounds;
	}

	public Sprite[] getIconsList()
	{
		if (_cached_banner_icons == null)
		{
			_cached_banner_icons = SpriteTextureLoader.getSpriteList("alliances/icons/");
		}
		return _cached_banner_icons;
	}

	public bool anyAlliances()
	{
		return Count > 0;
	}

	public override void clear()
	{
		base.clear();
	}

	protected override void updateDirtyUnits()
	{
	}
}
