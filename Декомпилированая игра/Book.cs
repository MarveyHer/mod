public class Book : CoreSystemObject<BookData>
{
	private BaseStats _base_stats_read_action = new BaseStats();

	public override BaseSystemManager manager => World.world.books;

	public void newBook(Actor pByActor, BookTypeAsset pBookType, ActorTrait pTraitActor, CultureTrait pTraitCulture, LanguageTrait pTraitLanguage, ReligionTrait pTraitReligion)
	{
		string tName = NameGenerator.generateNameFromTemplate(pBookType.name_template, pByActor);
		setName(tName);
		data.book_type = pBookType.id;
		data.path_cover = World.world.books.getNewCoverPath();
		data.path_icon = pBookType.getNewIconPath();
		data.author_name = pByActor.getName();
		data.author_id = pByActor.getID();
		data.author_clan_name = pByActor.clan?.name;
		data.author_clan_id = pByActor.clan?.id ?? (-1);
		data.author_kingdom_name = pByActor.kingdom?.name;
		data.author_kingdom_id = pByActor.kingdom?.id ?? (-1);
		data.author_city_name = pByActor.city?.name;
		data.author_city_id = pByActor.city?.id ?? (-1);
		data.language_id = pByActor.language.id;
		data.language_name = pByActor.language.name;
		data.trait_id_actor = pTraitActor?.id;
		data.trait_id_language = pTraitLanguage?.id;
		data.trait_id_culture = pTraitCulture?.id;
		data.trait_id_religion = pTraitReligion?.id;
		pByActor.language.books.setDirty();
		if (pBookType.save_culture)
		{
			data.culture_id = pByActor.culture.id;
			data.culture_name = pByActor.culture.name;
			pByActor.culture.books.setDirty();
		}
		if (pBookType.save_religion)
		{
			data.religion_id = pByActor.religion?.id ?? (-1);
			data.religion_name = pByActor.religion?.name;
			pByActor.religion?.books.setDirty();
		}
		pByActor.language.data.books_written++;
		if (pByActor.hasClan())
		{
			pByActor.clan.data.books_written++;
		}
		recalcBaseStats();
	}

	public BaseStats getBaseStats()
	{
		return _base_stats_read_action;
	}

	private void recalcBaseStats()
	{
		_base_stats_read_action.clear();
		_base_stats_read_action.mergeStats(getAsset().base_stats);
	}

	public bool isReadyToBeRead()
	{
		if (World.world.getWorldTimeElapsedSince(data.timestamp_read_last_time) > 10f)
		{
			return true;
		}
		return false;
	}

	public override void loadData(BookData pData)
	{
		base.loadData(pData);
		recalcBaseStats();
	}

	public Religion getReligion()
	{
		return World.world.religions.get(data.religion_id);
	}

	public Language getLanguage()
	{
		return World.world.languages.get(data.language_id);
	}

	public Culture getCulture()
	{
		return World.world.cultures.get(data.culture_id);
	}

	public ActorTrait getBookTraitActor()
	{
		if (string.IsNullOrEmpty(data.trait_id_actor))
		{
			return null;
		}
		return AssetManager.traits.get(data.trait_id_actor);
	}

	public LanguageTrait getBookTraitLanguage()
	{
		if (string.IsNullOrEmpty(data.trait_id_language))
		{
			return null;
		}
		return AssetManager.language_traits.get(data.trait_id_language);
	}

	public CultureTrait getBookTraitCulture()
	{
		if (string.IsNullOrEmpty(data.trait_id_culture))
		{
			return null;
		}
		return AssetManager.culture_traits.get(data.trait_id_culture);
	}

	public ReligionTrait getBookTraitReligion()
	{
		if (string.IsNullOrEmpty(data.trait_id_religion))
		{
			return null;
		}
		return AssetManager.religion_traits.get(data.trait_id_religion);
	}

	public int getHappiness()
	{
		int tResult = (int)_base_stats_read_action["happiness"];
		if (getLanguage().hasTrait("beautiful_calligraphy"))
		{
			tResult = (int)((float)tResult * LanguageTraitLibrary.getValueFloat("beautiful_calligraphy"));
		}
		return tResult;
	}

	public int getExperience()
	{
		int tResult = (int)_base_stats_read_action["experience"];
		Language tLanguage = getLanguage();
		if (tLanguage.hasTrait("scribble"))
		{
			if (tResult > 1)
			{
				tResult = 1;
			}
		}
		else if (tLanguage.hasTrait("nicely_structured_grammar"))
		{
			tResult = (int)((float)tResult * LanguageTraitLibrary.getValueFloat("nicely_structured_grammar"));
		}
		return tResult;
	}

	public int getMana()
	{
		return (int)_base_stats_read_action["mana"];
	}

	protected sealed override void setDefaultValues()
	{
		base.setDefaultValues();
	}

	public BookTypeAsset getAsset()
	{
		return AssetManager.book_types.get(data.book_type);
	}

	public void readIt()
	{
		data.timestamp_read_last_time = World.world.getCurWorldTime();
	}

	public void increaseReadTimes()
	{
		data.times_read++;
		World.world.game_stats.data.booksRead++;
		World.world.map_stats.booksRead++;
		readIt();
	}

	public override void Dispose()
	{
		base.Dispose();
		_base_stats_read_action.clear();
	}
}
