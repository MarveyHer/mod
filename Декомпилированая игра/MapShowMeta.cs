using UnityEngine;
using UnityEngine.UI;

public class MapShowMeta : MonoBehaviour
{
	public WorldElement worldElementPrefab;

	public WorldElement element;

	public Transform transformContent;

	public GameObject loadingSpinner;

	public GameObject errorImage;

	public GameObject textStatusBG;

	public Text textStatusMessage;

	public GameObject playButton;

	public GameObject favButton;

	public Text playButtonText;

	public GameObject deleteButton;

	public GameObject reportButton;

	public GameObject bottomButtons;

	public Image iconFavorite;

	private bool startSpinning;

	private float angle;

	private void Update()
	{
		if (startSpinning)
		{
			angle -= Time.deltaTime * 180f;
			iconFavorite.transform.localEulerAngles = new Vector3(0f, 0f, angle);
		}
		else if (angle != 0f)
		{
			angle -= Time.deltaTime * 720f;
			if (angle < -360f)
			{
				angle = 0f;
			}
			iconFavorite.transform.localEulerAngles = new Vector3(0f, 0f, angle);
		}
	}

	public void pressFavorite()
	{
	}

	public void copyToClipboard()
	{
	}
}
