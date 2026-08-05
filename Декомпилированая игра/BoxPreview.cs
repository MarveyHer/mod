using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BoxPreview : MonoBehaviour
{
	[SerializeField]
	private Sprite _preview_default;

	[SerializeField]
	private Image _icon_gift;

	[SerializeField]
	private Image _icon_premium;

	[SerializeField]
	private Image _icon_broken;

	[SerializeField]
	private Image _icon_modded;

	[SerializeField]
	private Image _cursed_bg;

	[SerializeField]
	private Image _cursed_overlay;

	[SerializeField]
	private GameObject _favorited;

	[SerializeField]
	private Image _preview_image;

	[SerializeField]
	private Button _button;

	[SerializeField]
	private Text _text_id;

	private bool _wantLoad_preview;

	private float _timer_preview;

	private string _world_path;

	private int _slot_id;

	private MapMetaData _metaData;

	private void Awake()
	{
		_button.OnHover(delegate
		{
			if (InputHelpers.mouseSupported)
			{
				showHoverTooltip();
			}
		});
		_button.OnHoverOut(delegate
		{
			if (InputHelpers.mouseSupported)
			{
				Tooltip.hideTooltip();
			}
		});
	}

	public void setSlot(int pID)
	{
		_metaData = null;
		_text_id.text = "#" + pID;
		_slot_id = pID;
		_world_path = SaveManager.getSlotSavePath(pID);
		if (SaveManager.doesSaveExist(_world_path))
		{
			_metaData = SaveManager.getMetaFor(_world_path);
		}
		_preview_image.sprite = _preview_default;
		_icon_gift.gameObject.SetActive(value: false);
		_icon_premium.gameObject.SetActive(value: false);
		_icon_broken.gameObject.SetActive(value: false);
		_icon_modded.gameObject.SetActive(value: false);
		_cursed_bg.enabled = false;
		_cursed_overlay.enabled = false;
		if (_metaData != null)
		{
			if (_metaData.saveVersion > Config.WORLD_SAVE_VERSION)
			{
				_icon_broken.gameObject.SetActive(value: true);
			}
			if (_metaData.modded)
			{
				_icon_modded.gameObject.SetActive(value: true);
			}
			if (_metaData.cursed)
			{
				_cursed_bg.enabled = true;
				_cursed_overlay.enabled = true;
			}
		}
		_wantLoad_preview = true;
		_timer_preview = 0.02f * (float)pID;
		base.gameObject.name = "BoxPreview " + pID;
		bool tIsFavorite = PlayerConfig.instance.data.favorite_world == pID;
		_favorited.SetActive(tIsFavorite);
	}

	private void showHoverTooltip()
	{
		if (_metaData != null && Config.tooltips_active)
		{
			_metaData.temp_date_string = SaveManager.getMapCreationTime(_world_path);
			Tooltip.show(_button, "map_meta", new TooltipData
			{
				map_meta = _metaData
			});
		}
	}

	private void Update()
	{
		if (_wantLoad_preview)
		{
			if (_timer_preview > 0f)
			{
				_timer_preview -= Time.deltaTime;
				return;
			}
			_wantLoad_preview = false;
			StartCoroutine(loadSaveSlotImage());
		}
	}

	public void showDefaultImage()
	{
		_preview_image.sprite = _preview_default;
	}

	private void showPreview(Texture2D pTexture)
	{
		Sprite tSprite = Sprite.Create(Toolbox.ScaleTexture(pTexture, 100, 100), new Rect(0f, 0f, 100f, 100f), new Vector2(0.5f, 0.5f));
		_preview_image.sprite = tSprite;
	}

	private IEnumerator loadSaveSlotImage()
	{
		string tPath = SaveManager.generatePngPreviewPath(_world_path);
		if (string.IsNullOrEmpty(tPath) || !File.Exists(tPath))
		{
			showDefaultImage();
			yield break;
		}
		yield return CoroutineHelper.wait_for_next_frame;
		Texture2D tTexture = new Texture2D(100, 100);
		tTexture.name = "preview_" + _slot_id;
		try
		{
			byte[] pngBytes = File.ReadAllBytes(tPath);
			if (tTexture.LoadImage(pngBytes))
			{
				if (tTexture == null)
				{
					Debug.LogError(base.gameObject.name + " texture is null from " + tPath);
					showDefaultImage();
				}
				else
				{
					showPreview(tTexture);
				}
			}
			else
			{
				Debug.LogError(base.gameObject.name + " cannot load image from " + tPath);
				showDefaultImage();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(base.gameObject.name + " " + ex.Message + " when trying to load " + tPath);
			showDefaultImage();
		}
		UnityEngine.Object.Destroy(tTexture);
	}

	public void click()
	{
		if (ScrollWindow.isAnimationActive())
		{
			return;
		}
		if (Input.GetKey(KeyCode.LeftShift))
		{
			Application.OpenURL("file://" + _world_path);
			return;
		}
		SaveManager.setCurrentPathAndId(_world_path, _slot_id);
		if (SaveManager.currentSlotExists())
		{
			ScrollWindow.showWindow("save_slot");
		}
		else
		{
			ScrollWindow.showWindow("save_slot_new");
		}
	}
}
