using System.Collections.Generic;

public class BooksElement : WindowMetaElementBase
{
	protected List<long> books;

	private IBooksWindow _books_window;

	protected override void Awake()
	{
		_books_window = GetComponentInParent<IBooksWindow>();
		base.Awake();
	}

	protected override void OnEnable()
	{
		books = _books_window.getBooks();
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		books = null;
	}

	public override bool checkRefreshWindow()
	{
		if (books != null)
		{
			foreach (long tBookID in books)
			{
				if (World.world.books.get(tBookID).isRekt())
				{
					return true;
				}
			}
		}
		return base.checkRefreshWindow();
	}
}
