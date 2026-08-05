using System.Collections.Generic;
using UnityEngine;

public class BookManager : CoreSystemManager<Book, BookData>
{
	public const string COVER_PATH = "books/book_covers/";

	public const string ICON_PATH = "books/book_icons/";

	private static Sprite[] _cached_covers;

	public BookManager()
	{
		type_id = "book";
	}

	public Book generateNewBook(Actor pActor)
	{
		City tCity = pActor.getCity();
		Building tBuilding = tCity.getBuildingWithBookSlot();
		if (tBuilding == null)
		{
			return null;
		}
		Book tBook = newBook(pActor);
		if (tBook == null)
		{
			return null;
		}
		World.world.game_stats.data.booksWritten++;
		World.world.map_stats.booksWritten++;
		pActor.changeHappiness("wrote_book");
		tBuilding.addBook(tBook);
		tCity.setStatusDirty();
		return tBook;
	}

	public string getNewCoverPath()
	{
		if (_cached_covers == null)
		{
			_cached_covers = SpriteTextureLoader.getSpriteList("books/book_covers/");
		}
		return _cached_covers.GetRandom().name;
	}

	private BookTypeAsset getPossibleBookType(Actor pActor)
	{
		using ListPool<BookTypeAsset> tPool = new ListPool<BookTypeAsset>(AssetManager.book_types.list.Count * 5);
		for (int iIndex = 0; iIndex < AssetManager.book_types.list.Count; iIndex++)
		{
			BookTypeAsset tBookType = AssetManager.book_types.list[iIndex];
			if (tBookType.requirement_check == null || tBookType.requirement_check(pActor, tBookType))
			{
				int tRate = tBookType.writing_rate;
				if (tBookType.rate_calc != null)
				{
					tRate = tBookType.rate_calc(pActor, tBookType);
				}
				tRate = Mathf.Min(tRate, 10);
				for (int i = 0; i < tRate; i++)
				{
					tPool.Add(tBookType);
				}
			}
		}
		if (tPool.Count == 0)
		{
			return null;
		}
		return tPool.GetRandom();
	}

	public Book newBook(Actor pActor)
	{
		BookTypeAsset tBookType = getPossibleBookType(pActor);
		if (tBookType == null)
		{
			return null;
		}
		Book book = newObject();
		ActorTrait tTraitActor = getBookTrait(pActor);
		LanguageTrait tTraitLanguage = pActor.language?.getTraitForBook();
		ReligionTrait tTraitReligion = pActor.religion?.getTraitForBook();
		CultureTrait tTraitCulture = pActor.culture?.getTraitForBook();
		book.newBook(pActor, tBookType, tTraitActor, tTraitCulture, tTraitLanguage, tTraitReligion);
		return book;
	}

	private ActorTrait getBookTrait(Actor pActor)
	{
		IReadOnlyCollection<ActorTrait> tTraits = pActor.getTraits();
		using ListPool<ActorTrait> tList = new ListPool<ActorTrait>(tTraits.Count);
		foreach (ActorTrait tTrait in tTraits)
		{
			if (tTrait.group_id == "mind")
			{
				tList.Add(tTrait);
			}
		}
		if (tList.Count == 0)
		{
			return null;
		}
		return tList.GetRandom();
	}

	public void copyBook(Book pBook)
	{
	}

	public void burnBook(Book pBook)
	{
		pBook.getLanguage()?.books.setDirty();
		pBook.getCulture()?.books.setDirty();
		pBook.getReligion()?.books.setDirty();
		removeObject(pBook);
	}

	public override void removeObject(Book pObject)
	{
		World.world.game_stats.data.booksBurnt++;
		World.world.map_stats.booksBurnt++;
		base.removeObject(pObject);
	}
}
