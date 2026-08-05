using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;

[Serializable]
public class MapStats
{
	public string name = "WorldBox";

	public string description = "";

	public string player_name;

	public string player_mood;

	public SaveCustomData custom_data;

	[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
	public double world_time;

	public int history_current_year = -1;

	[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string world_age_id;

	public int world_age_slot_index;

	public double world_age_started_at;

	public double same_world_age_started_at;

	public float current_world_ages_duration;

	public float current_age_progress;

	public bool is_world_ages_paused;

	[DefaultValue(1f)]
	public float world_ages_speed_multiplier = 1f;

	public string[] world_ages_slots;

	public long housesBuilt;

	public long housesDestroyed;

	public long population;

	public long creaturesBorn;

	public long creaturesCreated;

	public long subspeciesCreated;

	public long subspeciesExtinct;

	public long languagesCreated;

	public long languagesForgotten;

	public long booksWritten;

	public long booksRead;

	public long booksBurnt;

	public long culturesCreated;

	public long culturesForgotten;

	public long religionsCreated;

	public long religionsForgotten;

	public long kingdomsCreated;

	public long kingdomsDestroyed;

	public long citiesCreated;

	public long citiesConquered;

	public long citiesRebelled;

	public long citiesDestroyed;

	public long alliancesMade;

	public long alliancesDissolved;

	public long warsStarted;

	public long peacesMade;

	public long familiesCreated;

	public long armiesCreated;

	public long armiesDestroyed;

	public long familiesDestroyed;

	public long clansCreated;

	public long clansDestroyed;

	public long plotsStarted;

	public long plotsSucceeded;

	public long plotsForgotten;

	public double exploding_mushrooms_enabled_at;

	[DefaultValue(1L)]
	public long id_unit = 1L;

	[DefaultValue(1L)]
	public long id_building = 1L;

	[DefaultValue(1L)]
	public long id_kingdom = 1L;

	[DefaultValue(1L)]
	public long id_city = 1L;

	[DefaultValue(1L)]
	public long id_culture = 1L;

	[DefaultValue(1L)]
	public long id_clan = 1L;

	[DefaultValue(1L)]
	public long id_alliance = 1L;

	[DefaultValue(1L)]
	public long id_war = 1L;

	[DefaultValue(1L)]
	public long id_projectile = 1L;

	[DefaultValue(1L)]
	public long id_status = 1L;

	[DefaultValue(1L)]
	public long id_plot = 1L;

	[DefaultValue(1L)]
	public long id_book = 1L;

	[DefaultValue(1L)]
	public long id_subspecies = 1L;

	[DefaultValue(1L)]
	public long id_family = 1L;

	[DefaultValue(1L)]
	public long id_army = 1L;

	[DefaultValue(1L)]
	public long id_language = 1L;

	[DefaultValue(1L)]
	public long id_religion = 1L;

	[DefaultValue(1L)]
	public long id_item = 1L;

	[DefaultValue(1L)]
	public long id_diplomacy = 1L;

	[DefaultValue(1L)]
	public long life_dna = 1L;

	[NonSerialized]
	public long current_infected;

	[NonSerialized]
	public long current_mobs;

	[NonSerialized]
	public long current_houses;

	[NonSerialized]
	public long current_vegetation;

	[NonSerialized]
	public long current_infected_plague;

	private int _last_year = -1;

	private int _last_month = -1;

	private float _timer_stats = 0.1f;

	public static string[] possible_formats = new string[20]
	{
		"pr_", "st_", "w_", "a_", "c_", "u_", "b_", "k_", "c_", "cl_",
		"p_", "bo_", "sp_", "lang_", "rel_", "it_", "f_", "fa_", "army_", "d_"
	};

	[Preserve]
	[Obsolete("use .world_age_id instead", true)]
	public string era_id
	{
		set
		{
			if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(world_age_id))
			{
				world_age_id = value;
			}
		}
	}

	public long deaths { get; set; }

	public long deaths_age { get; set; }

	public long deaths_hunger { get; set; }

	public long deaths_eaten { get; set; }

	public long deaths_plague { get; set; }

	public long deaths_poison { get; set; }

	public long deaths_infection { get; set; }

	public long deaths_tumor { get; set; }

	public long deaths_acid { get; set; }

	public long deaths_fire { get; set; }

	public long deaths_divine { get; set; }

	public long deaths_weapon { get; set; }

	public long deaths_gravity { get; set; }

	public long deaths_drowning { get; set; }

	public long deaths_water { get; set; }

	public long deaths_explosion { get; set; }

	public long metamorphosis { get; set; }

	public long evolutions { get; set; }

	public long deaths_other { get; set; }

	public long deaths_smile { get; set; }

	[JsonIgnore]
	public int year => _last_year;

	[Preserve]
	[JsonProperty("year")]
	[Obsolete("use .world_time instead", true)]
	public int year_obsolete
	{
		set
		{
			if (value != 0)
			{
				world_time += (float)value * 60f;
			}
		}
	}

	[Preserve]
	[JsonProperty("month")]
	[Obsolete("use .world_time instead", true)]
	public int month_obsolete
	{
		set
		{
			if (value != 0)
			{
				world_time += (float)value * 5f;
			}
		}
	}

	[Preserve]
	[JsonProperty("worldTime")]
	[Obsolete("use .world_time instead", true)]
	public double worldTime_obsolete
	{
		set
		{
			if (value != 0.0)
			{
				world_time += value;
			}
		}
	}

	public MapStats()
	{
		checkDefault();
	}

	private void checkDefault()
	{
		if (world_ages_slots == null)
		{
			world_ages_slots = new string[8];
		}
		if (string.IsNullOrEmpty(player_name))
		{
			player_name = "The Creator";
		}
		if (string.IsNullOrEmpty(player_mood))
		{
			setDefaultMood();
		}
		if (custom_data == null)
		{
			custom_data = new SaveCustomData();
		}
	}

	public void generateLifeDNA()
	{
		CultureInfo tCulture = CultureInfo.InvariantCulture;
		long tDateTimeAsLong = long.Parse(DateTime.UtcNow.ToString("yyyyMMddHH", tCulture));
		life_dna = tDateTimeAsLong;
	}

	internal void updateStatsForPanel(float pElapsed)
	{
		if (_timer_stats > 0f)
		{
			_timer_stats -= pElapsed;
			return;
		}
		_timer_stats = 0.1f;
		recalcCounters();
	}

	internal void updateWorldTime(float pElapsed)
	{
		world_time += pElapsed;
		int tYearNow = Date.getCurrentYear();
		int tMonthNow = Date.getCurrentMonth();
		if (_last_year != tYearNow)
		{
			World.world.updateObjectAge();
		}
		if (_last_year != tYearNow)
		{
			_last_month = -1;
		}
		_last_year = tYearNow;
		_last_month = tMonthNow;
	}

	public void load()
	{
		checkDefault();
		_last_year = Date.getCurrentYear();
		_last_month = Date.getCurrentMonth();
	}

	private void recalcCounters()
	{
		current_infected = 0L;
		current_mobs = 0L;
		current_houses = 0L;
		current_vegetation = 0L;
		current_infected_plague = 0L;
		List<Actor> tActorList = World.world.units.getSimpleList();
		int i = 0;
		for (int tLen = tActorList.Count; i < tLen; i++)
		{
			Actor tActor = tActorList[i];
			if (tActor.hasTrait("plague"))
			{
				current_infected_plague++;
			}
			if (tActor.isSick())
			{
				current_infected++;
			}
			if (tActor.asset.count_as_unit && !tActor.isSapient())
			{
				current_mobs++;
			}
		}
		List<Building> tBuildingList = World.world.buildings.getSimpleList();
		int j = 0;
		for (int tLen2 = tBuildingList.Count; j < tLen2; j++)
		{
			Building tBuilding = tBuildingList[j];
			if (tBuilding.isCiv())
			{
				current_houses++;
			}
			else if (tBuilding.asset.is_vegetation)
			{
				current_vegetation++;
			}
		}
	}

	public long getNextId(string pType)
	{
		long tResult = 0L;
		switch (pType)
		{
		case "projectile":
			tResult = id_projectile++;
			break;
		case "statuses":
			tResult = id_status++;
			break;
		case "war":
			tResult = id_war++;
			break;
		case "alliance":
			tResult = id_alliance++;
			break;
		case "culture":
			tResult = id_culture++;
			break;
		case "unit":
			tResult = id_unit++;
			break;
		case "building":
			tResult = id_building++;
			break;
		case "kingdom":
			tResult = id_kingdom++;
			break;
		case "city":
			tResult = id_city++;
			break;
		case "clan":
			tResult = id_clan++;
			break;
		case "plot":
			tResult = id_plot++;
			break;
		case "book":
			tResult = id_book++;
			break;
		case "subspecies":
			tResult = id_subspecies++;
			break;
		case "language":
			tResult = id_language++;
			break;
		case "religion":
			tResult = id_religion++;
			break;
		case "item":
			tResult = id_item++;
			break;
		case "family":
			tResult = id_family++;
			break;
		case "army":
			tResult = id_army++;
			break;
		case "diplomacy":
			tResult = id_diplomacy++;
			break;
		default:
			Debug.LogError("NO pType for id " + pType);
			break;
		}
		return tResult;
	}

	public static string formatId(string pType, long pID)
	{
		switch (pType)
		{
		case "projectile":
			return "pr_" + pID;
		case "statuses":
			return "st_" + pID;
		case "war":
			return "w_" + pID;
		case "alliance":
			return "a_" + pID;
		case "culture":
			return "c_" + pID;
		case "unit":
			return "u_" + pID;
		case "building":
			return "b_" + pID;
		case "kingdom":
			return "k_" + pID;
		case "city":
			return "c_" + pID;
		case "clan":
			return "cl_" + pID;
		case "plot":
			return "p_" + pID;
		case "book":
			return "bo_" + pID;
		case "subspecies":
			return "sp_" + pID;
		case "language":
			return "lang_" + pID;
		case "religion":
			return "rel_" + pID;
		case "item":
			return "it_" + pID;
		case "family":
			return "fa_" + pID;
		case "army":
			return "army_" + pID;
		case "diplomacy":
			return "d_" + pID;
		default:
			Debug.LogError("NO pType for id " + pType);
			return "???_" + pID;
		}
	}

	public void debug(DebugTool pTool)
	{
		pTool.setText("(d)worldTime:", world_time, 0f, pShowBar: false, 0L);
		pTool.setText("(f)worldTime:", getWorldTime(), 0f, pShowBar: false, 0L);
		pTool.setText("cur month:", Date.getCurrentMonth(), 0f, pShowBar: false, 0L);
		pTool.setText("cur year:", Date.getCurrentYear(), 0f, pShowBar: false, 0L);
		pTool.setText("last_year:", _last_year, 0f, pShowBar: false, 0L);
		pTool.setText("last_month:", _last_month, 0f, pShowBar: false, 0L);
		pTool.setSeparator();
		pTool.setText("months since 0:", Date.getMonthsSince(0.0), 0f, pShowBar: false, 0L);
		pTool.setText("years since 0:", Date.getYearsSince(0.0), 0f, pShowBar: false, 0L);
		pTool.setText("months since now:", Date.getMonthsSince(world_time), 0f, pShowBar: false, 0L);
		pTool.setText("years since now:", Date.getYearsSince(world_time), 0f, pShowBar: false, 0L);
		pTool.setText("month time:", Date.getMonthTime(), 0f, pShowBar: false, 0L);
		pTool.setSeparator();
		pTool.setText("getDate 0:", Date.getDate(0.0), 0f, pShowBar: false, 0L);
		pTool.setText("getYearDate 0:", Date.getYearDate(0.0), 0f, pShowBar: false, 0L);
		pTool.setText("getYear 0:", Date.getYear(0.0), 0f, pShowBar: false, 0L);
		pTool.setText("getYear0 0:", Date.getYear0(0.0), 0f, pShowBar: false, 0L);
		pTool.setText("getDate now:", Date.getDate(world_time), 0f, pShowBar: false, 0L);
		pTool.setText("getYearDate now:", Date.getYearDate(world_time), 0f, pShowBar: false, 0L);
		pTool.setText("getYear now:", Date.getYear(world_time), 0f, pShowBar: false, 0L);
		pTool.setText("getYear0 now:", Date.getYear0(world_time), 0f, pShowBar: false, 0L);
		pTool.setSeparator();
		pTool.setText("max_float:", float.MaxValue, 0f, pShowBar: false, 0L);
	}

	public float getWorldTime()
	{
		return (float)world_time;
	}

	public void initNewWorld()
	{
		generateLifeDNA();
		generatePlayerName();
		setDefaultMood();
		name = NameGenerator.getName("world_name");
		AssetManager.gene_library.regenerateBasicDNACodesWithLifeSeed(life_dna);
	}

	private void generatePlayerName()
	{
		player_name = NameGenerator.getName("player_name", ActorSex.Male, pForceLegacy: false, null, null, pIgnoreBlackList: true);
	}

	private void setDefaultMood()
	{
		player_mood = "serene";
	}

	public ArchitectMood getArchitectMood()
	{
		if (string.IsNullOrEmpty(player_mood))
		{
			player_mood = "serene";
		}
		ArchitectMood tMood = AssetManager.architect_mood_library.get(player_mood);
		if (tMood == null)
		{
			tMood = AssetManager.architect_mood_library.get("serene");
		}
		return tMood;
	}
}
