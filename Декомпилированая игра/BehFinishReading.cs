using ai.behaviours;

public class BehFinishReading : BehCitizenActionCity
{
	protected override void setupErrorChecks()
	{
		base.setupErrorChecks();
		uses_books = true;
		uses_religions = true;
		uses_languages = true;
		uses_cultures = true;
	}

	public override BehResult execute(Actor pActor)
	{
		Book tBook = pActor.beh_book_target;
		if (tBook == null || !tBook.isAlive())
		{
			return BehResult.Stop;
		}
		checkBookTrait(pActor, tBook);
		checkBookValueBonuses(pActor, tBook);
		checkBookAttributes(pActor, tBook);
		checkSpecialBookRewards(pActor, tBook);
		tryToConvertActorToMetaFromBook(pActor, tBook);
		checkBookAssetAction(pActor, tBook);
		tryToGetMetaTraitsFromBook(pActor, tBook);
		tBook.increaseReadTimes();
		return BehResult.Continue;
	}

	private void checkBookAttributes(Actor pActor, Book pBook)
	{
		foreach (BaseStatsContainer pContainer in pBook.getBaseStats().getList())
		{
			if (pContainer.asset.actor_data_attribute)
			{
				pActor.data[pContainer.id] += pContainer.value;
			}
		}
	}

	private void checkBookAssetAction(Actor pActor, Book pBook)
	{
		BookTypeAsset tAsset = pBook.getAsset();
		tAsset.read_action?.Invoke(pActor, tAsset);
	}

	private void checkSpecialBookRewards(Actor pActor, Book pBook)
	{
		foreach (LanguageTrait tTrait in pBook.getLanguage().getTraits())
		{
			tTrait.read_book_trait_action?.Invoke(pActor, tTrait, pBook);
		}
	}

	private void checkBookValueBonuses(Actor pActor, Book pBook)
	{
		int tHappiness = pBook.getHappiness();
		int tExperience = pBook.getExperience();
		int tMana = pBook.getMana();
		if (pActor.hasCulture())
		{
			if (pActor.culture.hasTrait("reading_lovers") && tHappiness < 0)
			{
				tHappiness *= -1;
			}
			if (pActor.culture.hasTrait("attentive_readers"))
			{
				tExperience *= (int)((float)tExperience * CultureTraitLibrary.getValueFloat("attentive_readers"));
			}
		}
		pActor.changeHappiness("just_read_book", tHappiness);
		pActor.addExperience(tExperience);
		pActor.addMana(tMana);
	}

	private void checkBookTrait(Actor pActor, Book pBook)
	{
		if (Randy.randomBool())
		{
			ActorTrait tTrait = pBook.getBookTraitActor();
			if (tTrait != null)
			{
				pActor.addTrait(tTrait);
			}
		}
	}

	private void tryToConvertActorToMetaFromBook(Actor pActor, Book pBook)
	{
		tryToConvertActorToBookCulture(pActor, pBook);
		tryToConvertActorToBookLanguage(pActor, pBook);
		tryToConvertActorToBookReligion(pActor, pBook);
	}

	private void tryToGetMetaTraitsFromBook(Actor pActor, Book pBook)
	{
		if (pActor.isKing() || pActor.isCityLeader())
		{
			tryToGetMetaTraitFromBookCulture(pActor, pBook);
			tryToGetMetaTraitFromBookLanguage(pActor, pBook);
			tryToGetMetaTraitFromBookReligion(pActor, pBook);
		}
	}

	private void tryToGetMetaTraitFromBookCulture(Actor pActor, Book pBook)
	{
		if (pActor.hasCulture())
		{
			CultureTrait tTrait = pBook.getBookTraitCulture();
			if (tTrait != null && Randy.randomBool())
			{
				pActor.culture.addTrait(tTrait);
			}
		}
	}

	private void tryToGetMetaTraitFromBookLanguage(Actor pActor, Book pBook)
	{
		if (pActor.hasLanguage())
		{
			LanguageTrait tTrait = pBook.getBookTraitLanguage();
			if (tTrait != null && Randy.randomBool())
			{
				pActor.language.addTrait(tTrait);
			}
		}
	}

	private void tryToGetMetaTraitFromBookReligion(Actor pActor, Book pBook)
	{
		if (pActor.hasReligion())
		{
			ReligionTrait tTrait = pBook.getBookTraitReligion();
			if (tTrait != null && Randy.randomBool())
			{
				pActor.religion.addTrait(tTrait);
			}
		}
	}

	private void tryToConvertActorToBookReligion(Actor pActor, Book pBook)
	{
		Religion tBookReligion = pBook.getReligion();
		if (tBookReligion == null || pActor.religion == tBookReligion)
		{
			return;
		}
		using ListPool<Religion> tListPool = new ListPool<Religion>(6);
		if (pActor.hasReligion())
		{
			tListPool.AddTimes(3, pActor.religion);
			if (hasStylishWritingActor(pActor))
			{
				tListPool.AddTimes(getStylishWritingValue(), pActor.religion);
			}
		}
		tListPool.AddTimes(3, tBookReligion);
		if (hasStylishWritingBook(pBook))
		{
			tListPool.AddTimes(getStylishWritingValue(), tBookReligion);
		}
		Religion tNewReligion = tListPool.GetRandom();
		if (tNewReligion != pActor.religion)
		{
			pActor.tryToConvertToReligion(tNewReligion);
		}
	}

	private void tryToConvertActorToBookLanguage(Actor pActor, Book pBook)
	{
		Language tBookLanguage = pBook.getLanguage();
		if (tBookLanguage == null || pActor.language == tBookLanguage)
		{
			return;
		}
		using ListPool<Language> tPotLanguages = new ListPool<Language>();
		if (pActor.hasLanguage())
		{
			tPotLanguages.AddTimes(3, pActor.language);
			if (hasStylishWritingActor(pActor))
			{
				tPotLanguages.AddTimes(getStylishWritingValue(), pActor.language);
			}
		}
		tPotLanguages.AddTimes(3, tBookLanguage);
		if (hasStylishWritingBook(pBook))
		{
			tPotLanguages.AddTimes(getStylishWritingValue(), tBookLanguage);
		}
		Language tNewLanguage = tPotLanguages.GetRandom();
		if (tNewLanguage != pActor.language)
		{
			pActor.tryToConvertToLanguage(tNewLanguage);
		}
	}

	private void tryToConvertActorToBookCulture(Actor pActor, Book pBook)
	{
		Culture tCultureBook = pBook.getCulture();
		if (tCultureBook == null)
		{
			return;
		}
		Culture tCultureActor = pActor.culture;
		if (tCultureActor == tCultureBook)
		{
			return;
		}
		using ListPool<Culture> tListPool = new ListPool<Culture>();
		if (pActor.hasCulture())
		{
			tListPool.AddTimes(3, tCultureActor);
			if (hasStylishWritingActor(pActor))
			{
				tListPool.AddTimes(getStylishWritingValue(), tCultureActor);
			}
		}
		tListPool.AddTimes(3, tCultureBook);
		if (hasStylishWritingBook(pBook))
		{
			tListPool.AddTimes(getStylishWritingValue(), tCultureBook);
		}
		Culture tNewCulture = tListPool.GetRandom();
		if (tNewCulture != tCultureActor)
		{
			pActor.tryToConvertToCulture(tNewCulture);
		}
	}

	private bool hasStylishWritingActor(Actor pActor)
	{
		if (pActor.hasLanguage() && pActor.language.hasTrait("stylish_writing"))
		{
			return true;
		}
		return false;
	}

	private bool hasStylishWritingBook(Book pBook)
	{
		if (pBook.getLanguage().hasTrait("stylish_writing"))
		{
			return true;
		}
		return false;
	}

	private int getStylishWritingValue()
	{
		return LanguageTraitLibrary.getValue("stylish_writing");
	}
}
