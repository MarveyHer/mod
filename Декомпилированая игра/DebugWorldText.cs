using FMOD.Studio;
using life.taxi;
using UnityEngine;

public class DebugWorldText : MonoBehaviour
{
	public TextMesh text_mesh;

	public TextMesh text_mesh_bg_clone;

	private string _color_sounds_attached = "#FF1F44";

	private string _color_sounds = "#607BFF";

	private string _color_actors = "#FF8F44";

	private string _color_building = "#00FFFF";

	private string _color_city = "#A0FF93";

	private string _color_kingdom = "#FF4242";

	private string cur_string;

	private string cur_color;

	public void create()
	{
		text_mesh_bg_clone.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Debug");
		text_mesh_bg_clone.GetComponent<Renderer>().sortingOrder = 1;
		text_mesh.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Debug");
		text_mesh.GetComponent<Renderer>().sortingOrder = 2;
	}

	private void prepare(string pID, string pColor, float pSize = 0.25f)
	{
		text_mesh.color = Color.white;
		cur_string = pID;
		cur_color = "<color=" + pColor + ">";
		text_mesh_bg_clone.characterSize = pSize;
		text_mesh.characterSize = pSize;
	}

	private void add(string pTitle, object pText)
	{
		cur_string = cur_string + pTitle + ": " + cur_color + pText?.ToString() + "</color>\n";
	}

	public void setTextFmodSound(DebugMusicBoxData pData)
	{
		setTextFmodSound(pData, Color.white);
	}

	public void setTextFmodSound(DebugMusicBoxData pData, Color pColor)
	{
		float tSize = pData.timer / 3f;
		prepare("#fmod\n", _color_sounds, 0.5f);
		cur_string = "mb:" + pData.path;
		Color tColor = pColor;
		tColor.a = tSize;
		fin();
		text_mesh.color = tColor;
		text_mesh_bg_clone.color = tColor;
	}

	public void setTextFmodSound(EventInstance pInstance)
	{
		pInstance.getDescription(out var eventDescription);
		eventDescription.getPath(out var tPath);
		prepare("#fmod\n", _color_sounds_attached, 0.5f);
		add("name", tPath);
		fin();
	}

	public void setTextZone(TileZone pZone)
	{
		prepare("#zone\n", _color_actors, 0.5f);
		(string, int)[] debug_args = pZone.debug_args;
		for (int i = 0; i < debug_args.Length; i++)
		{
			(string, int) tTuple = debug_args[i];
			add(tTuple.Item1, tTuple.Item2);
		}
		fin();
	}

	public void setTextBoat(Actor pActor)
	{
		Boat tBoat = pActor.getSimpleComponent<Boat>();
		TaxiRequest tRequest = tBoat.taxi_request;
		if (tBoat.hasPassengers() || tRequest != null)
		{
			prepare("#boat\n", _color_kingdom, 0.8f);
		}
		else
		{
			prepare("#boat\n", _color_actors, 0.4f);
		}
		if (pActor.ai.job != null)
		{
			add("job", pActor.ai.job.id + "(" + pActor.ai.task_index + "/" + pActor.ai.job.tasks.Count + ")");
		}
		if (pActor.hasTask())
		{
			string tActionIndex = " [" + pActor.ai.action_index + "/" + pActor.ai.task?.list.Count + "]";
			add("task", pActor.ai.task.id + " " + tActionIndex);
			string tAction = pActor.ai.action?.GetType().ToString();
			if (tAction != null)
			{
				tAction = tAction.Replace("ai.behaviours.", "");
			}
			add("action", tAction);
		}
		add("timer", tBoat.actor.timer_action);
		fin();
	}

	private void debugForce(Actor pActor)
	{
		add("force xy", pActor.velocity.x + "-" + pActor.velocity.y);
		add("force z", pActor.velocity.z);
		add("zPosition", pActor.position_height);
		add("force_speed", pActor.velocity_speed);
		add("under_force", pActor.under_forces);
		add("mass", pActor.stats["mass"]);
	}

	public void setTextActor(Actor pActor)
	{
		prepare("#unit\n", _color_actors, 0.2f);
		add("name", pActor.data.name);
		add("timer_action", pActor.timer_action);
		if (pActor.isCarryingResources())
		{
			add("inv.count", pActor.inventory.countResources());
			add("inv.render", pActor.inventory.getItemIDToRender());
		}
		add("stats", pActor.asset.id);
		add("id", pActor.data.id);
		add("alive", pActor.isAlive());
		add("health", pActor.getHealth() + "/" + pActor.getMaxHealth());
		add("traits", pActor.countTraits());
		if (pActor.hasAnyStatusEffect())
		{
			add("statuses", pActor.countStatusEffects());
		}
		if (pActor.ai.job != null)
		{
			add("job", pActor.ai.job.id + "(" + pActor.ai.task_index + "/" + pActor.ai.job.tasks.Count + ")");
		}
		if (pActor.hasTask())
		{
			add("task", pActor.ai.task.id);
			string tAction = pActor.ai.action?.GetType().ToString();
			if (tAction != null)
			{
				tAction = tAction.Replace("ai.behaviours.", "");
			}
			tAction = tAction + pActor.ai.action_index + "/" + pActor.ai.task?.list.Count;
			add("action", tAction);
		}
		fin();
	}

	public void setTextArmy(Army pArmy)
	{
		prepare("#army\n", _color_building, 0.3f);
		add("captain", pArmy.getCaptain().getName());
		add("id", pArmy.id);
		add("units", pArmy.countUnits());
		add("alive", pArmy.isAlive());
		if (pArmy.getCity().isAlive())
		{
			add("city", pArmy.getCity().name);
		}
		else
		{
			add("city", "DESTROYED, SHOULD BE NULL");
		}
		fin();
	}

	public void setTextBuilding(Building pObj)
	{
		prepare("#build\n", _color_building, 0.3f);
		add("objectID", pObj.data.id);
		add("state", pObj.data.state);
		add("animationState", pObj.animation_state);
		add("ownership", pObj.state_ownership);
		add("kingdom", pObj.kingdom.id);
		if (pObj.asset.hasHousingSlots())
		{
			add("housing", pObj.countResidents() + "/" + pObj.asset.housing_slots);
		}
		fin();
	}

	public void setTextCity(City pObj)
	{
		prepare("#city\n", _color_city, 1.5f);
		bool tError = false;
		string tErrorWrongID = "";
		foreach (string tDictID in pObj.buildings_dict_id.Keys)
		{
			if (tError)
			{
				break;
			}
			foreach (Building tB in pObj.buildings_dict_id[tDictID])
			{
				if (!tB.isAlive())
				{
					tError = true;
					tErrorWrongID += "dead,";
				}
				if (tB.asset.id != tDictID)
				{
					tError = true;
					tErrorWrongID = tErrorWrongID + "wrong stats " + tB.asset.id;
				}
				if (tError)
				{
					break;
				}
			}
		}
		int tCountFiremen = 0;
		foreach (Actor unit in pObj.units)
		{
			if (unit.isTask("put_out_fire"))
			{
				tCountFiremen++;
			}
		}
		add("on_fire", pObj.isCityUnderDangerFire());
		add("danger", pObj.isInDanger());
		add("firemen", tCountFiremen);
		add("total", pObj.status.population + "/" + pObj.getPopulationMaximum());
		add("units", pObj.units.Count);
		add("buildings", pObj.buildings.Count);
		add("orders_psbl", pObj._debug_last_possible_build_orders);
		add("orders_no_res", pObj._debug_last_possible_build_orders_no_resources);
		add("order_last", pObj._debug_last_build_order_try);
		add("house_zone_limit", pObj.getHouseCurrent() + "/" + pObj.getHouseLimit());
		if (pObj.ai.job != null)
		{
			add("job", pObj.ai.job.id + "(" + pObj.ai.task_index + "/" + pObj.ai.job.tasks.Count + ")");
		}
		if (pObj.ai.task != null)
		{
			add("task", pObj.ai.task.id);
		}
		else
		{
			add("task", "-");
		}
		if (tError)
		{
			add("ERROR", tErrorWrongID);
		}
		fin();
	}

	public void setTextCityTasks(City pCity)
	{
		prepare("#city_tasks\n", _color_city, 0.5f);
		add("trees:", pCity.tasks.trees);
		add("stone:", pCity.tasks.minerals);
		add("minerals:", pCity.tasks.minerals);
		add("bushes:", pCity.tasks.bushes);
		add("plants:", pCity.tasks.plants);
		add("hives:", pCity.tasks.hives);
		add("farm_fields:", pCity.tasks.farm_fields);
		add("wheats:", pCity.tasks.wheats);
		add("ruins:", pCity.tasks.ruins);
		add("poops:", pCity.tasks.poops);
		add("roads:", pCity.tasks.roads);
		add("fire:", pCity.tasks.fire);
		add("", "");
		int tTotal = 0;
		int tTotalOcuppied = 0;
		foreach (CitizenJobAsset tAsset in pCity.jobs.jobs.Keys)
		{
			int tCount = pCity.jobs.jobs[tAsset];
			int tOccupied = 0;
			if (pCity.jobs.occupied.ContainsKey(tAsset))
			{
				tOccupied = pCity.jobs.occupied[tAsset];
			}
			tTotal += tCount;
			tTotalOcuppied += tOccupied;
			add(tAsset.id + ":", tOccupied + "/" + tCount);
		}
		foreach (CitizenJobAsset tAsset2 in pCity.jobs.occupied.Keys)
		{
			if (!pCity.jobs.jobs.ContainsKey(tAsset2))
			{
				int tOccupied2 = pCity.jobs.occupied[tAsset2];
				tTotalOcuppied += tOccupied2;
				add(tAsset2.id + ":", tOccupied2 + "/" + 0);
			}
		}
		int tTotalAdults = 0;
		int tTotalWorkers = 0;
		foreach (Actor tActor in pCity.units)
		{
			if (tActor.isAdult())
			{
				tTotalAdults++;
			}
			if (tActor.hasTask() && tActor.citizen_job != null)
			{
				tTotalWorkers++;
			}
		}
		add("total:", tTotalOcuppied + "/" + tTotal);
		add("pop|adults|workers:", pCity.units.Count + " | " + tTotalAdults + " | " + tTotalWorkers);
		fin();
	}

	public void setTextKingdom(Kingdom pObj)
	{
		prepare("#kingdom\n", _color_kingdom, 2f);
		add("total", pObj.getPopulationPeople() + "/" + pObj.getPopulationTotalPossible());
		add("units", pObj.units.Count);
		add("buildings", pObj.buildings.Count);
		add("timer_action", pObj.timer_action);
		add("timer_new_king", pObj.data.timer_new_king);
		if (pObj.ai.job != null)
		{
			add("job", pObj.ai.job.id + "(" + pObj.ai.task_index + "/" + pObj.ai.job.tasks.Count + ")");
		}
		if (pObj.ai.task != null)
		{
			add("task", pObj.ai.task.id);
		}
		else
		{
			add("task", "-");
		}
		fin();
	}

	private void fin()
	{
		text_mesh.text = cur_string;
		text_mesh_bg_clone.text = cur_string;
	}
}
