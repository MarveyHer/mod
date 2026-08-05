using UnityEngine;

public static class CursorTooltipHelper
{
	private static float _timeout = 0f;

	private static float _timeout_interval = 0.2f;

	public static bool is_over_meta;

	public static void update()
	{
		if (!InputHelpers.mouseSupported)
		{
			return;
		}
		if (World.world.isBusyWithUI())
		{
			cancel();
			return;
		}
		if (isInputHappening())
		{
			cancel();
			return;
		}
		bool tShown = false;
		tShown = updateGameplayTooltip();
		if (!tShown)
		{
			tShown = updateMapTooltip();
		}
		if (!tShown)
		{
			cancel();
		}
	}

	private static bool updateGameplayTooltip()
	{
		if (!PlayerConfig.optionBoolEnabled("tooltip_units"))
		{
			return false;
		}
		if (!MapBox.isRenderGameplay())
		{
			return false;
		}
		Actor tActor = UnitSelectionEffect.last_actor;
		if (tActor == null)
		{
			return false;
		}
		if (!tActor.isAlive())
		{
			return false;
		}
		if (_timeout > 0f)
		{
			_timeout -= World.world.delta_time;
			return true;
		}
		string tType = "actor";
		if (!HotkeyLibrary.many_mod.isHolding() || !showTooltipForSelectedMeta(tActor))
		{
			if (tActor.isKing())
			{
				tType = "actor_king";
			}
			else if (tActor.isCityLeader())
			{
				tType = "actor_leader";
			}
			Tooltip.hideTooltip(tActor, pOnlySimObjects: true, tType);
			Tooltip.show(tActor, tType, new TooltipData
			{
				actor = tActor,
				tooltip_scale = 0.7f,
				is_sim_tooltip = true,
				sound_allowed = false
			});
		}
		return true;
	}

	private static bool showTooltipForSelectedMeta(Actor pActor)
	{
		MetaType tMeta = Zones.getCurrentMapBorderMode();
		TooltipData tData = new TooltipData
		{
			tooltip_scale = 0.7f,
			is_sim_tooltip = true
		};
		object tMetaTarget = null;
		string tType;
		switch (tMeta)
		{
		case MetaType.Alliance:
			if (!pActor.kingdom.hasAlliance())
			{
				return false;
			}
			tType = "alliance";
			tData.alliance = pActor.kingdom.getAlliance();
			tMetaTarget = pActor.kingdom.getAlliance();
			break;
		case MetaType.Kingdom:
			if (!pActor.isKingdomCiv())
			{
				return false;
			}
			tType = "kingdom";
			tData.kingdom = pActor.kingdom;
			tMetaTarget = pActor.kingdom;
			break;
		case MetaType.City:
			if (!pActor.hasCity())
			{
				return false;
			}
			tType = "city";
			tData.city = pActor.city;
			tMetaTarget = pActor.city;
			break;
		case MetaType.Clan:
			if (!pActor.hasClan())
			{
				return false;
			}
			tType = "clan";
			tData.clan = pActor.clan;
			tMetaTarget = pActor.clan;
			break;
		case MetaType.Culture:
			if (!pActor.hasCulture())
			{
				return false;
			}
			tType = "culture";
			tData.culture = pActor.culture;
			tMetaTarget = pActor.culture;
			break;
		case MetaType.Family:
			if (!pActor.hasFamily())
			{
				return false;
			}
			tType = "family";
			tData.family = pActor.family;
			tMetaTarget = pActor.family;
			break;
		case MetaType.Language:
			if (!pActor.hasLanguage())
			{
				return false;
			}
			tType = "language";
			tData.language = pActor.language;
			tMetaTarget = pActor.language;
			break;
		case MetaType.Religion:
			if (!pActor.hasReligion())
			{
				return false;
			}
			tType = "religion";
			tData.religion = pActor.religion;
			tMetaTarget = pActor.religion;
			break;
		case MetaType.Subspecies:
			if (!pActor.hasSubspecies())
			{
				return false;
			}
			tType = "subspecies";
			tData.subspecies = pActor.subspecies;
			tMetaTarget = pActor.subspecies;
			break;
		default:
			return false;
		}
		Tooltip.hideTooltip(tMetaTarget, pOnlySimObjects: true, tType);
		Tooltip.show(tMetaTarget, tType, tData);
		return true;
	}

	private static bool updateMapTooltip()
	{
		if (!PlayerConfig.optionBoolEnabled("tooltip_zones"))
		{
			return false;
		}
		if (!MapBox.isRenderMiniMap())
		{
			return false;
		}
		if (!Zones.showMapBorders())
		{
			return false;
		}
		if (_timeout > 0f)
		{
			_timeout -= World.world.delta_time;
			return true;
		}
		bool tShowing = false;
		WorldTile tMouseTile = World.world.getMouseTilePosCachedFrame();
		MetaTypeAsset tMetaAsset = World.world.getCachedMapMetaAsset();
		if (tMouseTile != null && tMetaAsset != null)
		{
			tShowing = tMetaAsset.check_cursor_tooltip(tMouseTile.zone, tMetaAsset, tMetaAsset.getZoneOptionState());
		}
		return tShowing;
	}

	private static void cancel()
	{
		Tooltip.hideTooltip(null, pOnlySimObjects: true, string.Empty);
		resetTimout();
	}

	private static bool isInputHappening()
	{
		if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
		{
			return true;
		}
		if (Input.mouseScrollDelta.y != 0f)
		{
			return true;
		}
		if (HotkeyLibrary.many_mod.isHolding())
		{
			return false;
		}
		return Input.anyKey;
	}

	private static void resetTimout()
	{
		_timeout = _timeout_interval;
	}
}
