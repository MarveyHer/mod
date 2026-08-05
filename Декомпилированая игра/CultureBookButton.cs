using UnityEngine;
using UnityEngine.UI;

public class CultureBookButton : MonoBehaviour
{
	private Book _book;

	public Image cover;

	public Image icon;

	private bool _created;

	private void Start()
	{
		create();
	}

	private void create()
	{
		if (!_created)
		{
			_created = true;
			setupTooltip();
		}
	}

	public void setupTooltip()
	{
		if (TryGetComponent<TipButton>(out var tTipButton))
		{
			tTipButton.setHoverAction(showTooltip);
		}
	}

	internal void load(long pBookID)
	{
		Book tBook = World.world.books.get(pBookID);
		load(tBook);
	}

	internal void load(Book pBook)
	{
		_book = pBook;
		BookTypeAsset tTypeAsset = _book.getAsset();
		string tIconPath = "books/book_icons/" + tTypeAsset.path_icons + _book.data.path_icon;
		string pPath = "books/book_covers/" + _book.data.path_cover;
		Sprite tSpriteIcon = SpriteTextureLoader.getSprite(tIconPath);
		Sprite tSpriteCover = SpriteTextureLoader.getSprite(pPath);
		icon.sprite = tSpriteIcon;
		cover.sprite = tSpriteCover;
		base.gameObject.name = _book.getAsset().id;
	}

	private void showTooltip()
	{
		Tooltip.show(base.gameObject, "book", new TooltipData
		{
			book = _book
		});
	}
}
