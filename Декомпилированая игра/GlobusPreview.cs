using System.Collections;
using System.IO;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GlobusPreview : MonoBehaviour
{
	public bool use_current_world_info;

	public Image main_image_1;

	public Image main_image_2;

	public GameObject images_parent;

	public Image clouds;

	public Sprite preview_default;

	private float _tweenSpeed = 18f;

	private float _gap_size = 25f;

	private float _box_size = 100f;

	private void OnEnable()
	{
		if (Config.game_loaded)
		{
			if (use_current_world_info)
			{
				setCurrentWorldSprite();
			}
			else if (SaveManager.currentWorkshopMapData != null)
			{
				setWorkshopSlotSprite();
			}
			else
			{
				startLoadCurrentSaveSlotSprite();
			}
			startTweenGlobus();
		}
	}

	private void startLoadCurrentSaveSlotSprite()
	{
		StartCoroutine(loadSaveSlotImage());
	}

	private void setCurrentWorldSprite()
	{
		Sprite tSprite = PreviewHelper.getCurrentWorldPreview();
		setSprites(tSprite);
	}

	private void setWorkshopSlotSprite()
	{
		Sprite tSprite = PreviewHelper.loadWorkshopMapPreview();
		setSprites(tSprite);
	}

	private void setSprites(Sprite pSprite)
	{
		makeGradient(pSprite);
		main_image_1.sprite = pSprite;
		main_image_2.sprite = pSprite;
	}

	private void showDefaultImage()
	{
		main_image_1.sprite = preview_default;
		main_image_2.sprite = preview_default;
	}

	private IEnumerator loadSaveSlotImage()
	{
		string path = SaveManager.getCurrentPreviewPath();
		if (string.IsNullOrEmpty(path) || !File.Exists(path))
		{
			showDefaultImage();
			yield break;
		}
		using UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture("file://" + path);
		yield return webRequest.SendWebRequest();
		if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
		{
			showDefaultImage();
			yield break;
		}
		Texture2D tTexture = DownloadHandlerTexture.GetContent(webRequest);
		tTexture.name = "save_slot_preview_" + Path.GetFileNameWithoutExtension(path);
		Sprite tSprite = Sprite.Create(tTexture, new Rect(0f, 0f, tTexture.width, tTexture.height), new Vector2(0.5f, 0.5f));
		setSprites(tSprite);
	}

	private void makeGradient(Sprite pSprite)
	{
		float tGradientWidth = (float)pSprite.texture.width * 0.1f;
		Texture2D tTexture = pSprite.texture;
		tTexture.name = "gradient_" + tTexture.name;
		for (int xx = 0; (float)xx < tGradientWidth; xx++)
		{
			for (int yy = 0; yy < tTexture.height; yy++)
			{
				int tX = xx;
				Color tColor = tTexture.GetPixel(tX, yy);
				tColor.a = (float)tX / tGradientWidth;
				tTexture.SetPixel(tX, yy, tColor);
				tX = pSprite.texture.width - xx;
				tColor = tTexture.GetPixel(tX, yy);
				tColor.a = (float)xx / tGradientWidth;
				tTexture.SetPixel(tX, yy, tColor);
			}
		}
		tTexture.Apply();
	}

	private void startTweenGlobus()
	{
		float tDist = _box_size + _gap_size;
		float tTime = tDist / _tweenSpeed;
		images_parent.transform.DOKill();
		images_parent.transform.localPosition = new Vector3(_gap_size, 0f, 0f);
		images_parent.transform.DOLocalMove(new Vector3(0f - tDist, 0f, 0f), tTime).SetEase(Ease.Linear).onComplete = tweenLoop;
	}

	private void tweenLoop()
	{
		float tDist = _box_size + _gap_size;
		float tTime = tDist / _tweenSpeed;
		images_parent.transform.localPosition = new Vector3(0f, 0f, 0f);
		images_parent.transform.DOLocalMove(new Vector3(0f - tDist, 0f, 0f), tTime).SetEase(Ease.Linear).onComplete = tweenLoop;
	}
}
