using UnityEngine;

public class BooksNoItems : MonoBehaviour
{
	private GameObject _inner;

	private IBooksWindow _books_window;

	private void Awake()
	{
		_inner = base.transform.GetChild(0).gameObject;
		_books_window = GetComponentInParent<IBooksWindow>();
	}

	private void OnEnable()
	{
		_inner.SetActive(!_books_window.hasBooks());
	}
}
