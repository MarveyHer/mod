using ai;
using ai.behaviours;

public class BehFinishTalk : BehaviourActionActor
{
	public BehFinishTalk()
	{
		socialize = true;
	}

	public override BehResult execute(Actor pActor)
	{
		Actor tTarget = pActor.beh_actor_target?.a;
		if (tTarget == null)
		{
			return BehResult.Stop;
		}
		if (!stillCanTalk(tTarget))
		{
			return BehResult.Stop;
		}
		finishTalk(pActor, tTarget);
		return BehResult.Continue;
	}

	private bool stillCanTalk(Actor pTarget)
	{
		if (!pTarget.isAlive())
		{
			return false;
		}
		if (pTarget.isLying())
		{
			return false;
		}
		return true;
	}

	private void finishTalk(Actor pActor, Actor pTarget)
	{
		pActor.resetSocialize();
		pTarget.resetSocialize();
		bool num = Randy.randomChance(0.7f);
		int tBonusValue = ((!num) ? (-15) : 10);
		pActor.changeHappiness("just_talked", tBonusValue);
		pTarget.changeHappiness("just_talked", tBonusValue);
		pActor.addStatusEffect("recovery_social");
		pTarget.addStatusEffect("recovery_social");
		if (num)
		{
			ActorTool.checkFallInLove(pActor, pTarget);
		}
		if (num)
		{
			ActorTool.checkBecomingBestFriends(pActor, pTarget);
		}
		checkMetaSpread(pActor, pTarget);
		if (pActor.hasCulture() && pActor.culture.hasTrait("youth_reverence") && throwDiceForGift(pActor, pTarget) && pActor.isAdult() && pTarget.getAge() < pActor.getAge())
		{
			makeGift(pActor, pTarget);
		}
		if (pActor.hasCulture() && pActor.culture.hasTrait("elder_reverence") && throwDiceForGift(pActor, pTarget) && pActor.isAdult() && pTarget.getAge() > pActor.getAge())
		{
			makeGift(pActor, pTarget);
		}
		checkPassLearningAttributes(pActor, pTarget);
		pTarget.timer_action = (pActor.timer_action = Randy.randomFloat(1.1f, 3.3f));
	}

	private void checkAttribue(Actor pActor, Actor pTarget, string pAttributeID)
	{
		if (Randy.randomChance(0.3f))
		{
			if (pActor.stats[pAttributeID] > pTarget.stats[pAttributeID])
			{
				pTarget.stats[pAttributeID]++;
			}
			else if (pActor.stats[pAttributeID] < pTarget.stats[pAttributeID])
			{
				pActor.stats[pAttributeID]++;
			}
		}
	}

	private void checkPassLearningAttributes(Actor pActor, Actor pTarget)
	{
		checkAttribue(pActor, pTarget, "intelligence");
		checkAttribue(pActor, pTarget, "warfare");
		checkAttribue(pActor, pTarget, "diplomacy");
		checkAttribue(pActor, pTarget, "stewardship");
	}

	private void checkMetaSpread(Actor pActor, Actor pTarget)
	{
		if (pActor.hasSubspecies() && pTarget.hasSubspecies())
		{
			tryToSpreadCulture(pActor, pTarget);
			tryToSpreadLanguage(pActor, pTarget);
			tryToSpreadReligion(pActor, pTarget);
		}
	}

	private void tryToSpreadCulture(Actor pActor, Actor pTarget)
	{
		if (!pActor.subspecies.has_advanced_memory || !pTarget.subspecies.has_advanced_memory)
		{
			return;
		}
		Culture tCultureToSet = decideCulture(pActor, pTarget);
		if (tCultureToSet != null)
		{
			pActor.tryToConvertToCulture(tCultureToSet);
			pTarget.tryToConvertToCulture(tCultureToSet);
			if (tCultureToSet.hasTrait("pep_talks") && Randy.randomChance(0.5f))
			{
				pActor.addStatusEffect("inspired");
				pTarget.addStatusEffect("inspired");
			}
			if (tCultureToSet.hasTrait("expertise_exchange"))
			{
				pActor.addExperience(CultureTraitLibrary.getValue("expertise_exchange"));
				pTarget.addExperience(CultureTraitLibrary.getValue("expertise_exchange"));
			}
			if (tCultureToSet.hasTrait("gossip_lovers"))
			{
				pActor.changeHappiness("just_talked_gossip");
				pTarget.changeHappiness("just_talked_gossip");
			}
		}
	}

	private void tryToSpreadLanguage(Actor pActor, Actor pTarget)
	{
		if (pActor.subspecies.has_advanced_communication && pTarget.subspecies.has_advanced_communication)
		{
			Language tLanguageToSet = decideLanguage(pActor, pTarget);
			if (tLanguageToSet != null)
			{
				pActor.tryToConvertToLanguage(tLanguageToSet);
				pTarget.tryToConvertToLanguage(tLanguageToSet);
			}
		}
	}

	private void tryToSpreadReligion(Actor pActor, Actor pTarget)
	{
		if (pActor.subspecies.has_advanced_memory && pTarget.subspecies.has_advanced_memory)
		{
			Religion tReligionToSet = decideReligion(pActor, pTarget);
			if (tReligionToSet != null)
			{
				pActor.tryToConvertToReligion(tReligionToSet);
				pTarget.tryToConvertToReligion(tReligionToSet);
			}
		}
	}

	private Religion decideReligion(Actor pActor1, Actor pActor2)
	{
		Religion tReligion1 = pActor1.religion;
		Religion tReligion2 = pActor2.religion;
		if (tReligion1 == null && tReligion2 == null)
		{
			return null;
		}
		if (tReligion1 == null)
		{
			return tReligion2;
		}
		if (tReligion2 == null)
		{
			return tReligion1;
		}
		using ListPool<Religion> tPotReligions = new ListPool<Religion>();
		tPotReligions.Add(tReligion1);
		tPotReligions.Add(tReligion2);
		if (pActor1.hasCity() && pActor1.religion == pActor1.city.getReligion())
		{
			tPotReligions.Add(pActor1.religion);
		}
		if (pActor1.kingdom.hasReligion() && pActor1.religion == pActor1.kingdom.getReligion())
		{
			tPotReligions.Add(pActor1.religion);
		}
		if (pActor2.hasCity() && pActor2.religion == pActor2.city.getReligion())
		{
			tPotReligions.Add(pActor2.religion);
		}
		if (pActor2.kingdom.hasReligion() && pActor2.religion == pActor2.kingdom.getReligion())
		{
			tPotReligions.Add(pActor2.religion);
		}
		return tPotReligions.GetRandom();
	}

	private Language decideLanguage(Actor pActor1, Actor pActor2)
	{
		Language tLanguage1 = pActor1.language;
		Language tLanguage2 = pActor2.language;
		if (tLanguage1 == null && tLanguage2 == null)
		{
			return null;
		}
		if (tLanguage1 == null)
		{
			return tLanguage2;
		}
		if (tLanguage2 == null)
		{
			return tLanguage1;
		}
		using ListPool<Language> tPotLanguages = new ListPool<Language>();
		int tAmount_1 = 3;
		int tAmount_2 = 3;
		if (pActor1.hasLanguage() && pActor1.language.hasTrait("melodic"))
		{
			tAmount_1 += LanguageTraitLibrary.getValue("melodic");
		}
		if (pActor2.hasLanguage() && pActor2.language.hasTrait("melodic"))
		{
			tAmount_2 += LanguageTraitLibrary.getValue("melodic");
		}
		if (pActor1.hasCity() && pActor1.language == pActor1.city.getLanguage())
		{
			tAmount_1++;
		}
		if (pActor1.kingdom.hasLanguage() && pActor1.language == pActor1.kingdom.getLanguage())
		{
			tAmount_1++;
		}
		if (pActor2.hasCity() && pActor2.language == pActor2.city.getLanguage())
		{
			tAmount_2++;
		}
		if (pActor2.kingdom.hasLanguage() && pActor2.language == pActor2.kingdom.getLanguage())
		{
			tAmount_2++;
		}
		tPotLanguages.AddTimes(tAmount_1, tLanguage1);
		tPotLanguages.AddTimes(tAmount_2, tLanguage2);
		return tPotLanguages.GetRandom();
	}

	private Culture decideCulture(Actor pActor1, Actor pActor2)
	{
		Culture tCulture1 = pActor1.culture;
		Culture tCulture2 = pActor2.culture;
		if (tCulture1 == null && tCulture2 == null)
		{
			return null;
		}
		if (tCulture1 == null)
		{
			return tCulture2;
		}
		if (tCulture2 == null)
		{
			return tCulture1;
		}
		using ListPool<Culture> tPotCultures = new ListPool<Culture>();
		int tAmount_1 = 3;
		int tAmount_2 = 3;
		if (pActor1.hasLanguage() && pActor1.language.hasTrait("melodic"))
		{
			tAmount_1 += LanguageTraitLibrary.getValue("melodic");
		}
		if (pActor2.hasLanguage() && pActor2.language.hasTrait("melodic"))
		{
			tAmount_2 += LanguageTraitLibrary.getValue("melodic");
		}
		if (pActor1.hasCity() && pActor1.culture == pActor1.city.getCulture())
		{
			tAmount_1++;
		}
		if (pActor1.kingdom.hasCulture() && pActor1.culture == pActor1.kingdom.getCulture())
		{
			tAmount_1++;
		}
		if (pActor2.hasCity() && pActor2.culture == pActor2.city.getCulture())
		{
			tAmount_2++;
		}
		if (pActor2.kingdom.hasCulture() && pActor2.culture == pActor2.kingdom.getCulture())
		{
			tAmount_2++;
		}
		tPotCultures.AddTimes(tAmount_1, tCulture1);
		tPotCultures.AddTimes(tAmount_2, tCulture2);
		return tPotCultures.GetRandom();
	}

	private bool throwDiceForGift(Actor pActor, Actor pTarget)
	{
		bool num = pActor.isRelatedTo(pTarget) || pActor.isImportantTo(pTarget);
		float tChance = 0.2f;
		if (num)
		{
			tChance += 0.3f;
		}
		return Randy.randomChance(tChance);
	}

	private void makeGift(Actor pActor, Actor pTarget)
	{
		bool tItemGift = pTarget.tryToAcceptGift(pActor);
		int tRandomMoney = pActor.getMoneyForGift();
		if (tRandomMoney > 0)
		{
			pTarget.addMoney(tRandomMoney);
		}
		if (tRandomMoney > 0 || tItemGift)
		{
			pActor.changeHappiness("just_gave_gift");
			pTarget.changeHappiness("just_received_gift");
		}
	}
}
