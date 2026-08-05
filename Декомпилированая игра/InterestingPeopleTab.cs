using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InterestingPeopleTab : WindowMetaElementBase
{
	private const float TWEEN_DURATION = 0.15f;

	public InterestingPeopleElement most_kills;

	public InterestingPeopleElement most_children;

	public InterestingPeopleElement most_births;

	public InterestingPeopleElement oldest;

	public InterestingPeopleElement fastest;

	public InterestingPeopleElement strongest;

	public InterestingPeopleElement weakest;

	public InterestingPeopleElement smartest;

	public InterestingPeopleElement dumbest;

	public InterestingPeopleElement richest;

	public InterestingPeopleElement most_known;

	public InterestingPeopleElement biggest_level;

	public InterestingPeopleElement happiest;

	public InterestingPeopleElement saddest;

	public InterestingPeopleElement hungriest;

	public InterestingPeopleElement fullest;

	public InterestingPeopleElement youngest;

	public InterestingPeopleElement most_health;

	public InterestingPeopleElement lowest_health;

	private readonly List<Actor> _unit_most_kills = new List<Actor>();

	private readonly List<Actor> _unit_most_children = new List<Actor>();

	private readonly List<Actor> _unit_most_births = new List<Actor>();

	private readonly List<Actor> _unit_oldest = new List<Actor>();

	private readonly List<Actor> _unit_fastest = new List<Actor>();

	private readonly List<Actor> _unit_strongest = new List<Actor>();

	private readonly List<Actor> _unit_weakest = new List<Actor>();

	private readonly List<Actor> _unit_smartest = new List<Actor>();

	private readonly List<Actor> _unit_dumbest = new List<Actor>();

	private readonly List<Actor> _unit_richest = new List<Actor>();

	private readonly List<Actor> _unit_most_known = new List<Actor>();

	private readonly List<Actor> _unit_biggest_level = new List<Actor>();

	private readonly List<Actor> _unit_saddest = new List<Actor>();

	private readonly List<Actor> _unit_happiest = new List<Actor>();

	private readonly List<Actor> _unit_hungriest = new List<Actor>();

	private readonly List<Actor> _unit_fullest = new List<Actor>();

	private readonly List<Actor> _unit_youngest = new List<Actor>();

	private readonly List<Actor> _unit_most_health = new List<Actor>();

	private readonly List<Actor> _unit_lowest_health = new List<Actor>();

	private List<Actor>[] _all_unit_lists;

	private InterestingPeopleElement[] _all_elements;

	private IInterestingPeopleWindow _interesting_people_window;

	private List<Tweener> _tweeners = new List<Tweener>();

	protected override void Awake()
	{
		_interesting_people_window = GetComponentInParent<IInterestingPeopleWindow>();
		_all_elements = new InterestingPeopleElement[19]
		{
			biggest_level, fastest, fullest, happiest, hungriest, most_births, most_children, most_kills, most_known, oldest,
			richest, saddest, smartest, dumbest, strongest, weakest, youngest, most_health, lowest_health
		};
		_all_unit_lists = new List<Actor>[19]
		{
			_unit_biggest_level, _unit_fastest, _unit_fullest, _unit_happiest, _unit_hungriest, _unit_most_births, _unit_most_children, _unit_most_kills, _unit_most_known, _unit_oldest,
			_unit_richest, _unit_saddest, _unit_smartest, _unit_dumbest, _unit_strongest, _unit_weakest, _unit_youngest, _unit_most_health, _unit_lowest_health
		};
		base.Awake();
	}

	protected override IEnumerator showContent()
	{
		IEnumerable<Actor> tActors = _interesting_people_window.getInterestingUnitsList();
		return renderElements(tActors);
	}

	private IEnumerator renderElements(IEnumerable<Actor> pList)
	{
		int tMaxKills = 1;
		int tMaxChildren = 1;
		int tMaxAge = 0;
		int tMinAge = int.MaxValue;
		int tMaxMoney = 1;
		int tMaxSpeed = 1;
		int tMaxDamage = 1;
		int tMinDamage = int.MaxValue;
		int tMaxIntelligence = 1;
		int tMinIntelligence = int.MaxValue;
		int tMaxRenown = 1;
		int tMaxLevel = 1;
		int tMinSad = -10;
		int tMaxHappy = 10;
		int tMinNutrition = 30;
		int tMaxNutrition = 60;
		int tMaxBirths = 1;
		int tMaxHealth = 1;
		int tMinHealth = int.MaxValue;
		using ListPool<Actor> tUnits = new ListPool<Actor>(pList);
		tUnits.RemoveAll((Actor actor) => !actor.isAlive() || actor.asset.is_boat);
		tUnits.Sort(ListSorters.sortUnitByKills);
		tUnits.Sort(ListSorters.sortUnitByAgeOldFirst);
		foreach (ref Actor item in tUnits)
		{
			Actor tActor = item;
			if (tActor.data.kills > tMaxKills)
			{
				tMaxKills = tActor.data.kills;
				_unit_most_kills.Clear();
				_unit_most_kills.Add(tActor);
			}
			else if (tActor.data.kills == tMaxKills && _unit_most_kills.Count < 3)
			{
				_unit_most_kills.Add(tActor);
			}
			if (tActor.current_children_count > tMaxChildren)
			{
				tMaxChildren = tActor.current_children_count;
				_unit_most_children.Clear();
				_unit_most_children.Add(tActor);
			}
			else if (tActor.current_children_count == tMaxChildren && _unit_most_children.Count < 3)
			{
				_unit_most_children.Add(tActor);
			}
			if (tActor.data.births > tMaxBirths)
			{
				tMaxBirths = tActor.data.births;
				_unit_most_births.Clear();
				_unit_most_births.Add(tActor);
			}
			else if (tActor.data.births == tMaxBirths && _unit_most_births.Count < 3)
			{
				_unit_most_births.Add(tActor);
			}
			if (tActor.stats["speed"] > (float)tMaxSpeed)
			{
				tMaxSpeed = (int)tActor.stats["speed"];
				_unit_fastest.Clear();
				_unit_fastest.Add(tActor);
			}
			else if ((int)tActor.stats["speed"] == tMaxSpeed && _unit_fastest.Count < 3)
			{
				_unit_fastest.Add(tActor);
			}
			int tHealth = tActor.getHealth();
			if (tHealth > tMaxHealth)
			{
				tMaxHealth = tHealth;
				_unit_most_health.Clear();
				_unit_most_health.Add(tActor);
			}
			else if (tHealth == tMaxHealth && _unit_most_health.Count < 3)
			{
				_unit_most_health.Add(tActor);
			}
			if (tHealth < tMinHealth)
			{
				tMinHealth = tHealth;
				_unit_lowest_health.Clear();
				_unit_lowest_health.Add(tActor);
			}
			else if (tHealth == tMinHealth && _unit_lowest_health.Count < 3)
			{
				_unit_lowest_health.Add(tActor);
			}
			int tDamage = (int)tActor.stats["damage"];
			if (tDamage > tMaxDamage)
			{
				tMaxDamage = tDamage;
				_unit_strongest.Clear();
				_unit_strongest.Add(tActor);
			}
			else if (tDamage == tMaxDamage && _unit_strongest.Count < 3)
			{
				_unit_strongest.Add(tActor);
			}
			if (tDamage < tMinDamage)
			{
				tMinDamage = tDamage;
				_unit_weakest.Clear();
				_unit_weakest.Add(tActor);
			}
			else if (tDamage == tMinDamage && _unit_weakest.Count < 3)
			{
				_unit_weakest.Add(tActor);
			}
			int tIntelligence = (int)tActor.stats["intelligence"];
			if (tIntelligence > tMaxIntelligence)
			{
				tMaxIntelligence = tIntelligence;
				_unit_smartest.Clear();
				_unit_smartest.Add(tActor);
			}
			else if (tIntelligence == tMaxIntelligence && _unit_smartest.Count < 3)
			{
				_unit_smartest.Add(tActor);
			}
			if (tIntelligence < tMinIntelligence)
			{
				tMinIntelligence = tIntelligence;
				_unit_dumbest.Clear();
				_unit_dumbest.Add(tActor);
			}
			else if (tIntelligence == tMinIntelligence && _unit_dumbest.Count < 3)
			{
				_unit_dumbest.Add(tActor);
			}
			if (tActor.money > tMaxMoney)
			{
				tMaxMoney = tActor.money;
				_unit_richest.Clear();
				_unit_richest.Add(tActor);
			}
			else if (tActor.money == tMaxMoney && _unit_richest.Count < 3)
			{
				_unit_richest.Add(tActor);
			}
			if (tActor.renown > tMaxRenown)
			{
				tMaxRenown = tActor.renown;
				_unit_most_known.Clear();
				_unit_most_known.Add(tActor);
			}
			else if (tActor.renown == tMaxRenown && _unit_most_known.Count < 3)
			{
				_unit_most_known.Add(tActor);
			}
			if (tActor.data.level > tMaxLevel)
			{
				tMaxLevel = tActor.data.level;
				_unit_biggest_level.Clear();
				_unit_biggest_level.Add(tActor);
			}
			else if (tActor.data.level == tMaxLevel && _unit_biggest_level.Count < 3)
			{
				_unit_biggest_level.Add(tActor);
			}
			if (tActor.hasEmotions())
			{
				int tHappiness = tActor.getHappiness();
				if (tHappiness > tMaxHappy)
				{
					tMaxHappy = tHappiness;
					_unit_happiest.Clear();
					_unit_happiest.Add(tActor);
				}
				else if (tHappiness == tMaxHappy && _unit_happiest.Count < 3)
				{
					_unit_happiest.Add(tActor);
				}
				if (tHappiness < tMinSad)
				{
					tMinSad = tHappiness;
					_unit_saddest.Clear();
					_unit_saddest.Add(tActor);
				}
				else if (tHappiness == tMinSad && _unit_saddest.Count < 3)
				{
					_unit_saddest.Add(tActor);
				}
			}
			int tNutritution = tActor.data.nutrition;
			if (tNutritution > tMaxNutrition)
			{
				tMaxNutrition = tNutritution;
				_unit_fullest.Clear();
				_unit_fullest.Add(tActor);
			}
			else if (tNutritution == tMaxNutrition && _unit_fullest.Count < 3)
			{
				_unit_fullest.Add(tActor);
			}
			if (tNutritution < tMinNutrition)
			{
				tMinNutrition = tNutritution;
				_unit_hungriest.Clear();
				_unit_hungriest.Add(tActor);
			}
			else if (tNutritution == tMinNutrition && _unit_hungriest.Count < 3)
			{
				_unit_hungriest.Add(tActor);
			}
			int tActorAge = tActor.getAge();
			if (tActorAge > tMaxAge)
			{
				tMaxAge = tActorAge;
				_unit_oldest.Clear();
				_unit_oldest.Add(tActor);
			}
			else if (tActorAge == tMaxAge && _unit_oldest.Count < 3)
			{
				_unit_oldest.Add(tActor);
			}
			if (tActorAge < tMinAge)
			{
				tMinAge = tActorAge;
				_unit_youngest.Clear();
				_unit_youngest.Add(tActor);
			}
			else if (tActorAge == tMinAge && _unit_youngest.Count < 3)
			{
				_unit_youngest.Add(tActor);
			}
		}
		List<Actor>[] all_unit_lists = _all_unit_lists;
		foreach (List<Actor> tList in all_unit_lists)
		{
			track_objects.AddRange(tList);
		}
		yield return render(_unit_most_known, most_known, tMaxRenown);
		yield return render(_unit_biggest_level, biggest_level, tMaxLevel);
		yield return render(_unit_oldest, oldest, tMaxAge, 0);
		if (tMinAge != tMaxAge)
		{
			yield return render(_unit_youngest, youngest, tMinAge, 0);
		}
		yield return render(_unit_most_kills, most_kills, tMaxKills);
		yield return render(_unit_richest, richest, tMaxMoney);
		yield return render(_unit_most_children, most_children, tMaxChildren);
		yield return render(_unit_most_births, most_births, tMaxBirths);
		yield return render(_unit_happiest, happiest, tMaxHappy);
		yield return render(_unit_saddest, saddest, tMinSad, -1000);
		yield return render(_unit_hungriest, hungriest, tMinNutrition, 0);
		yield return render(_unit_fullest, fullest, tMaxNutrition);
		yield return render(_unit_smartest, smartest, tMaxIntelligence);
		yield return render(_unit_dumbest, dumbest, tMinIntelligence, -1000);
		yield return render(_unit_fastest, fastest, tMaxSpeed);
		yield return render(_unit_strongest, strongest, tMaxDamage);
		yield return render(_unit_weakest, weakest, tMinDamage, -1000);
		yield return render(_unit_most_health, most_health, tMaxHealth);
		if (tMinHealth != tMaxHealth)
		{
			yield return render(_unit_lowest_health, lowest_health, tMinHealth, 0);
		}
	}

	private IEnumerator render(List<Actor> pActor, InterestingPeopleElement pElement, int pValue, int pMinValue = 2)
	{
		if (pValue < pMinValue || pActor.Count == 0)
		{
			pElement.gameObject.SetActive(value: false);
			yield break;
		}
		pElement.gameObject.SetActive(value: true);
		foreach (Actor tActor in pActor)
		{
			if (tActor.isAlive())
			{
				pElement.show(tActor, pValue);
				yield return new WaitForSecondsRealtime(0.025f);
			}
		}
	}

	private void finishTweens()
	{
		foreach (Tweener tweener in _tweeners)
		{
			tweener.Kill(complete: true);
		}
		_tweeners.Clear();
	}

	protected override void clear()
	{
		base.clear();
		finishTweens();
		InterestingPeopleElement[] all_elements = _all_elements;
		for (int i = 0; i < all_elements.Length; i++)
		{
			all_elements[i].gameObject.SetActive(value: false);
		}
		List<Actor>[] all_unit_lists = _all_unit_lists;
		for (int i = 0; i < all_unit_lists.Length; i++)
		{
			all_unit_lists[i].Clear();
		}
	}
}
