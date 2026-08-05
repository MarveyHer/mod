using System;
using System.Globalization;
using System.IO;
using Humanizer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoSaveElement : MonoBehaviour, IPointerMoveHandler, IEventSystemHandler
{
	[SerializeField]
	private Image _preview;

	[SerializeField]
	private Text _save_name;

	[SerializeField]
	private Text _save_time_ago;

	[SerializeField]
	private CountUpOnClick _kingdoms;

	[SerializeField]
	private CountUpOnClick _cities;

	[SerializeField]
	private CountUpOnClick _population;

	[SerializeField]
	private CountUpOnClick _mobs;

	[SerializeField]
	private CountUpOnClick _age;

	[SerializeField]
	private Button _button;

	[SerializeField]
	private GameObject _premium_icon;

	private string _world_path;

	private MapMetaData _meta_data;

	private void Awake()
	{
		_button.OnHoverOut(delegate
		{
			if (InputHelpers.mouseSupported)
			{
				Tooltip.hideTooltip();
			}
		});
	}

	public void OnPointerMove(PointerEventData pData)
	{
		if (InputHelpers.mouseSupported && !Tooltip.anyActive())
		{
			tooltipAction();
		}
	}

	private void tooltipAction()
	{
		if (_meta_data != null && Config.tooltips_active)
		{
			_meta_data.temp_date_string = SaveManager.getMapCreationTime(_world_path);
			Tooltip.show(_button, "map_meta", new TooltipData
			{
				map_meta = _meta_data
			});
		}
	}

	public void load(AutoSaveData pData)
	{
		_world_path = pData.path;
		string tPathPreviewImage = SaveManager.generatePngSmallPreviewPath(pData.path);
		if (!string.IsNullOrEmpty(tPathPreviewImage) && File.Exists(tPathPreviewImage))
		{
			byte[] pngBytes = File.ReadAllBytes(tPathPreviewImage);
			Texture2D tTexture = new Texture2D(32, 32);
			tTexture.anisoLevel = 0;
			tTexture.filterMode = FilterMode.Point;
			if (tTexture.LoadImage(pngBytes))
			{
				Sprite sprite_small_preview = Sprite.Create(tTexture, new Rect(0f, 0f, 32f, 32f), new Vector2(0.5f, 0.5f));
				_preview.sprite = sprite_small_preview;
			}
		}
		_meta_data = SaveManager.getMetaFor(pData.path);
		_save_name.text = _meta_data.mapStats.name;
		_save_name.color = _meta_data.mapStats.getArchitectMood().getColorText();
		_kingdoms.setValue(_meta_data.kingdoms);
		_cities.setValue(_meta_data.cities);
		_population.setValue(_meta_data.population);
		_mobs.setValue(_meta_data.mobs);
		_age.setValue(Date.getYear(_meta_data.mapStats.world_time));
		string result = "";
		string tLang = "";
		try
		{
			DateTime d = Epoch.toDateTime(pData.timestamp);
			CultureInfo tCulture = LocalizedTextManager.getCulture();
			DateTime tFutureTime = DateTime.UtcNow.AddDays(7.0);
			if (d.Year < 2017)
			{
				result = "GREG";
			}
			else if (d > tFutureTime)
			{
				result = "DREDD";
			}
			else if (LocalizedTextManager.cultureSupported())
			{
				DateTime input = d;
				CultureInfo culture = tCulture;
				result = input.Humanize(utcDate: true, null, culture);
			}
			else
			{
				string tShortDatePattern = tCulture.DateTimeFormat.ShortDatePattern;
				result = d.ToString(tShortDatePattern, tCulture);
			}
		}
		catch (Exception message)
		{
			Debug.Log("failed with " + tLang);
			Debug.LogError(message);
		}
		_save_time_ago.text = result;
		base.gameObject.name = "AutoSaveElement_" + pData.timestamp;
	}

	public void clickLoadAutoSave()
	{
		SaveManager.setCurrentPath(_world_path);
		ScrollWindow.showWindow("load_world");
	}

	private void OnDisable()
	{
		_meta_data = null;
		if (_preview != null)
		{
			_preview.sprite = null;
		}
	}
}
