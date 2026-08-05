using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LevelPreviewButton : MonoBehaviour
{
	public bool premiumOnly = true;

	public bool worldNetUpload;

	public Image premiumIcon;

	public Image rewardAdIcon;

	public Button button;

	public SlotButtonCallback slotData;

	public Sprite defaultSprite;

	private ButtonAnimation buttonAnimation;

	public bool loaded;

	public bool loading;

	public bool autoload;

	public void click()
	{
		if (ScrollWindow.isAnimationActive())
		{
			return;
		}
		if (buttonAnimation == null)
		{
			buttonAnimation = base.transform.parent.parent.parent.GetComponent<ButtonAnimation>();
		}
		buttonAnimation.clickAnimation();
		SaveManager.setCurrentSlot(slotData.slotID);
		if (worldNetUpload)
		{
			if (SaveManager.currentSlotExists() && SaveManager.currentPreviewExists() && SaveManager.currentMetaExists())
			{
				ScrollWindow.showWindow("worldnet_upload_world_name");
			}
		}
		else if (SaveManager.currentSlotExists())
		{
			ScrollWindow.showWindow("save_slot");
		}
		else
		{
			ScrollWindow.showWindow("save_slot_new");
		}
	}

	public void checkTextureDestroy()
	{
		if (button.image.sprite.texture != defaultSprite.texture)
		{
			Object.Destroy(button.image.sprite.texture);
		}
	}

	private void OnEnable()
	{
		if (autoload)
		{
			reloadImage();
		}
	}

	private void OnDisable()
	{
		if (!(button?.image?.sprite?.texture == defaultSprite.texture))
		{
			Object.Destroy(button?.image?.sprite?.texture);
			Object.Destroy(button?.image?.sprite);
		}
	}

	public void reloadImage()
	{
		if (this == null || !base.isActiveAndEnabled || (loaded && button?.image?.sprite != null) || loading)
		{
			return;
		}
		loading = true;
		if (SaveManager.currentWorkshopMapData != null)
		{
			loadWorkshopMapPreview();
			return;
		}
		bool saveExists = SaveManager.currentSlotExists();
		if (slotData.slotID == -1 && !saveExists)
		{
			loadImage(PreviewHelper.getCurrentWorldPreview());
		}
		else
		{
			StartCoroutine(loadSaveSlotImage(slotData.slotID));
		}
	}

	private void loadWorkshopMapPreview()
	{
		loadImage(PreviewHelper.loadWorkshopMapPreview());
	}

	private IEnumerator loadSaveSlotImage(int slotID)
	{
		string path = SaveManager.getPngSlotPath(slotID);
		if (string.IsNullOrEmpty(path) || !File.Exists(path))
		{
			loadImage(null);
			yield break;
		}
		using UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture("file://" + path);
		yield return webRequest.SendWebRequest();
		if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
		{
			Debug.LogError(base.gameObject.name + " " + webRequest.error + " " + path);
			loadImage(null);
		}
		else
		{
			Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
			Sprite tSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			loadImage(tSprite);
		}
	}

	public void loadImage(Sprite pSource)
	{
		if (this == null || !base.isActiveAndEnabled)
		{
			loaded = false;
			loading = false;
			return;
		}
		if (!premiumOnly || Config.hasPremium)
		{
			premiumIcon.gameObject.SetActive(value: false);
		}
		bool tMapFound = false;
		if (pSource != null)
		{
			tMapFound = true;
			pSource.texture.anisoLevel = 0;
			pSource.texture.filterMode = FilterMode.Point;
		}
		else
		{
			pSource = defaultSprite;
		}
		button.image.sprite = pSource;
		base.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(pSource.rect.width, pSource.rect.height);
		RectTransform component = button.transform.parent.parent.GetComponent<RectTransform>();
		float tModWidth = 1f;
		float tModHeight = 1f;
		tModWidth = component.sizeDelta.x / pSource.rect.width;
		tModHeight = component.sizeDelta.y / pSource.rect.height;
		float tMod = ((tModWidth > tModHeight) ? tModWidth : tModHeight);
		Transform parent = base.transform.parent;
		if (!tMapFound)
		{
			tMod = 1f;
		}
		parent.localScale = new Vector3(tMod, tMod, 1f);
		loaded = true;
		loading = false;
	}
}
