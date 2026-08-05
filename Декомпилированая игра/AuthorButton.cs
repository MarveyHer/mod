using UnityEngine;

public class AuthorButton : MonoBehaviour
{
	public string authorId;

	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}

	public void showWorldNetAuthorListWindow()
	{
	}
}
