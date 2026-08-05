using UnityEngine;
using UnityEngine.UI;

public class NameplateText : MonoBehaviour
{
	private NameplateManager _manager;

	[SerializeField]
	private Image _icon_species;

	[SerializeField]
	private Image _icon_special;

	[SerializeField]
	private Image _icon_favorite;

	[SerializeField]
	private Image _background_image;

	[SerializeField]
	private KingdomBanner _banner_kingdoms;

	[SerializeField]
	private CultureBanner _banner_culture;

	[SerializeField]
	private LanguageBanner _banner_language;

	[SerializeField]
	private AllianceBanner _banner_alliance;

	[SerializeField]
	private ReligionBanner _banner_religion;

	[SerializeField]
	private SubspeciesBanner _banner_subspecies;

	[SerializeField]
	private FamilyBanner _banner_family;

	[SerializeField]
	private ClanBanner _banner_clan;

	[SerializeField]
	private ArmyBanner _banner_army;

	[SerializeField]
	private CityBanner _banner_city;

	[SerializeField]
	private RectTransform _container_capture;

	public HorizontalLayoutGroup layout_group;

	private NameplateAsset _asset;

	private bool _show_icon_species;

	private bool _show_icon_special;

	private bool _show_icon_favorite;

	private bool _show_banner_army;

	private bool _show_banner_kingdom;

	private bool _show_banner_culture;

	private bool _show_banner_alliance;

	private bool _show_banner_clan;

	private bool _show_banner_religion;

	private bool _show_banner_family;

	private bool _show_banner_subspecies;

	private bool _show_banner_language;

	private bool _show_banner_city;

	private bool _show_capture_counter;

	private CanvasGroup _canvas_group;

	[SerializeField]
	private RectTransform _rect_background;

	[SerializeField]
	private Text _text_name;

	[SerializeField]
	private Text _conquer_text;

	private RectTransform _rect;

	private RectTransform _text_rect;

	private bool _showing;

	internal bool priority_capital;

	internal int priority_population;

	internal bool favorited;

	internal Rect map_text_rect_click;

	internal Rect map_text_rect_overlap;

	public NanoObject nano_object;

	private float _last_scale;

	private string _old_text;

	private float _text_width;

	private bool _active_check_dirty;

	private NameplateRenderingType _last_mode;

	private Vector2 _last_position;

	private bool is_full => _last_mode == NameplateRenderingType.Full;

	public bool is_mini => _last_mode == NameplateRenderingType.BannerOnly;

	public Vector2 getLastScreenPosition()
	{
		return _last_position;
	}

	private void Awake()
	{
		_rect = GetComponent<RectTransform>();
		_text_rect = _text_name.GetComponent<RectTransform>();
		_canvas_group = GetComponent<CanvasGroup>();
	}

	public void prepare(NameplateAsset pAsset, NanoObject pMeta, float pGlobalScale, NameplateRenderingType pNameplateMode, bool pNanoObjectSet, NanoObject pSelectedNanoObject)
	{
		if (pNanoObjectSet)
		{
			pNameplateMode = ((pSelectedNanoObject == pMeta) ? NameplateRenderingType.Full : NameplateRenderingType.BannerOnly);
		}
		if (pNameplateMode != _last_mode)
		{
			clearCaches();
			_active_check_dirty = true;
			_last_mode = pNameplateMode;
			switch (_last_mode)
			{
			case NameplateRenderingType.Full:
				_background_image.transform.localScale = new Vector3(1f, 1f, 1f);
				_background_image.enabled = true;
				break;
			case NameplateRenderingType.BannerOnly:
				_background_image.transform.localScale = new Vector3(pAsset.banner_only_mode_scale, pAsset.banner_only_mode_scale, 1f);
				_background_image.enabled = false;
				break;
			}
		}
		updateScale(pMeta, pGlobalScale, pNanoObjectSet, pSelectedNanoObject);
		resetElements();
		setShowing(pVal: true);
		setAssetAndMeta(pAsset, pMeta);
		if (((IFavoriteable)pMeta).isFavorite())
		{
			showFavoriteIcon();
		}
		else
		{
			_show_icon_favorite = false;
		}
		checkSetActive(_icon_favorite, _show_icon_favorite);
	}

	private void updateScale(NanoObject pMeta, float pGlobalScale, bool pNanoObjectSet, NanoObject pSelectedNanoObject)
	{
		float tNewScaleValue = pGlobalScale;
		if (pNanoObjectSet)
		{
			tNewScaleValue = ((pSelectedNanoObject != pMeta) ? (pGlobalScale * 0.8f) : (pGlobalScale * 1.2f));
		}
		if (_last_scale != tNewScaleValue)
		{
			_last_scale = tNewScaleValue;
			base.transform.localScale = new Vector3(tNewScaleValue, tNewScaleValue, 1f);
		}
	}

	public void forceScale(Vector3 pScale)
	{
		_last_scale = pScale.x;
		base.transform.localScale = pScale;
	}

	public void newNameplate(NameplateManager pManager, string pName)
	{
		clearFull();
		_manager = pManager;
		base.name = pName;
	}

	public bool isShowing()
	{
		return _showing;
	}

	public void setShowing(bool pVal)
	{
		_showing = pVal;
	}

	public void checkActive()
	{
		if (_showing)
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
		}
		else if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
		if (_showing && _active_check_dirty)
		{
			_active_check_dirty = false;
			checkActiveElements();
		}
	}

	private void checkActiveElements()
	{
		checkSetActive(_container_capture, _show_capture_counter);
		checkSetActive(_icon_species, _show_icon_species);
		checkSetActive(_icon_special, _show_icon_special);
		checkSetActive(_banner_alliance, _show_banner_alliance);
		checkSetActive(_banner_clan, _show_banner_clan);
		checkSetActive(_banner_culture, _show_banner_culture);
		checkSetActive(_banner_kingdoms, _show_banner_kingdom);
		checkSetActive(_banner_religion, _show_banner_religion);
		checkSetActive(_banner_family, _show_banner_family);
		checkSetActive(_banner_language, _show_banner_language);
		checkSetActive(_banner_subspecies, _show_banner_subspecies);
		checkSetActive(_banner_army, _show_banner_army);
		checkSetActive(_banner_city, _show_banner_city);
	}

	private void checkSetActive(Component pComponent, bool pShouldBeActive)
	{
		checkSetActive(pComponent.gameObject, pShouldBeActive);
	}

	private void checkSetActive(GameObject pObject, bool pShouldBeActive)
	{
		if (pShouldBeActive)
		{
			if (!pObject.activeSelf)
			{
				pObject.SetActive(value: true);
			}
		}
		else if (pObject.activeSelf)
		{
			pObject.SetActive(value: false);
		}
	}

	public void clearFull()
	{
		nano_object = null;
		clearCaches();
		setShowing(pVal: false);
		resetElements();
	}

	public void clearCaches()
	{
		_asset = null;
		_last_position = Globals.POINT_IN_VOID_2;
		_last_scale = -1f;
		_old_text = "!";
		_last_mode = NameplateRenderingType.Clear;
	}

	private void resetElements()
	{
		priority_capital = false;
		priority_population = 0;
		favorited = false;
		_show_capture_counter = false;
		_show_icon_favorite = false;
		_show_icon_species = false;
		_show_icon_special = false;
		_show_banner_alliance = false;
		_show_banner_clan = false;
		_show_banner_culture = false;
		_show_banner_army = false;
		_show_banner_kingdom = false;
		_show_banner_religion = false;
		_show_banner_subspecies = false;
		_show_banner_language = false;
		_show_banner_family = false;
		_show_banner_city = false;
	}

	private void setupMeta(MetaObjectData pData, ColorAsset pColorAsset)
	{
		favorited = pData.favorite;
		Color tColor = pColorAsset.getColorText();
		_text_name.color = tColor;
		updateAlpha(pData);
	}

	private void updateAlpha(MetaObjectData pData)
	{
		float tAlpha = ((!checkShouldDrawObject(pData)) ? 0.5f : 1f);
		if (_canvas_group.alpha != tAlpha)
		{
			_canvas_group.alpha = tAlpha;
		}
	}

	internal void showTextKingdom(Kingdom pMetaObject, Vector2 pPosition)
	{
		setupMeta(pMetaObject.data, pMetaObject.getColor());
		int tUnitsCount = pMetaObject.getPopulationPeople();
		string tStringName = getStringForNameplate(pMetaObject.name, tUnitsCount);
		if (is_full)
		{
			if (DebugConfig.isOn(DebugOption.ShowWarriorsCityText))
			{
				tStringName = tStringName + " | " + pMetaObject.countTotalWarriors() + "/" + pMetaObject.countWarriorsMax();
			}
			if (DebugConfig.isOn(DebugOption.ShowCityWeaponsText))
			{
				tStringName = tStringName + " | w" + pMetaObject.countWeapons();
			}
		}
		setText(tStringName, pPosition);
		setPriority(tUnitsCount);
		showSpecies(pMetaObject.getSpriteIcon());
		_show_banner_kingdom = true;
		_banner_kingdoms.load(pMetaObject);
		if (is_full)
		{
			Clan tClan = pMetaObject.getKingClan();
			if (tClan != null)
			{
				_show_banner_clan = true;
				_banner_clan.load(tClan);
			}
		}
	}

	internal void showTextReligion(Religion pMetaObject, Vector3 pPosition)
	{
		setupMeta(pMetaObject.data, pMetaObject.getColor());
		int tUnitsCount = pMetaObject.units.Count;
		string tStringName = getStringForNameplate(pMetaObject.name, tUnitsCount);
		setText(tStringName, pPosition);
		setPriority(tUnitsCount);
		_show_banner_religion = true;
		_banner_religion.load(pMetaObject);
		showSpecies(pMetaObject.getActorAsset().getSpriteIcon());
	}

	internal void showTextSubspecies(Subspecies pMetaObject, Vector3 pPosition)
	{
		setupMeta(pMetaObject.data, pMetaObject.getColor());
		int tUnitsCount = pMetaObject.units.Count;
		string tStringName = getStringForNameplate(pMetaObject.name, tUnitsCount);
		setText(tStringName, pPosition);
		setPriority(tUnitsCount);
		_show_banner_subspecies = true;
		_banner_subspecies.load(pMetaObject);
		showSpecies(pMetaObject.getSpriteIcon());
	}

	internal void showTextFamily(Family pMetaObject, Vector3 pPosition)
	{
		setupMeta(pMetaObject.data, pMetaObject.getColor());
		int tUnitsCount = pMetaObject.units.Count;
		string tStringName = getStringForNameplate(pMetaObject.name, tUnitsCount);
		setText(tStringName, pPosition);
		setPriority(tUnitsCount);
		_show_banner_family = true;
		_banner_family.load(pMetaObject);
	}

	internal void showTextArmy(Army pMetaObject, Vector3 pPosition)
	{
		setupMeta(pMetaObject.data, pMetaObject.getColor());
		int tUnitsCount = pMetaObject.units.Count;
		string tStringName = getStringForNameplateLine(pMetaObject.name, tUnitsCount);
		setText(tStringName, pPosition);
		setPriority(tUnitsCount);
		_show_banner_army = true;
		_banner_army.load(pMetaObject);
		if (pMetaObject.hasCaptain())
		{
			showSpecies(pMetaObject.getCaptain().getActorAsset().getSpriteIcon());
		}
		else
		{
			showSpecies(pMetaObject.getActorAsset().getSpriteIcon());
		}
	}

	internal void showTextCulture(Culture pMetaObject, Vector3 pPosition)
	{
		setupMeta(pMetaObject.data, pMetaObject.getColor());
		int tUnitsCount = pMetaObject.units.Count;
		string tStringName = getStringForNameplate(pMetaObject.name, tUnitsCount);
		setText(tStringName, pPosition);
		setPriority(tUnitsCount);
		_show_banner_culture = true;
		_banner_culture.load(pMetaObject);
		showSpecies(pMetaObject.getActorAsset().getSpriteIcon());
	}

	internal void showTextLanguage(Language pMetaObject, Vector3 pPosition)
	{
		setupMeta(pMetaObject.data, pMetaObject.getColor());
		int tUnitsCount = pMetaObject.units.Count;
		string tStringName = getStringForNameplate(pMetaObject.name, tUnitsCount);
		setText(tStringName, pPosition);
		setPriority(tUnitsCount);
		_show_banner_language = true;
		_banner_language.load(pMetaObject);
		showSpecies(pMetaObject.getActorAsset().getSpriteIcon());
	}

	internal void showTextAlliance(Alliance pMetaObject, City pCity)
	{
		setupMeta(pMetaObject.data, pMetaObject.getColor());
		int tUnitsCount = pMetaObject.countPopulation();
		_show_icon_species = false;
		string tStringName = getStringForNameplate(pMetaObject.name, tUnitsCount);
		if (is_full && DebugConfig.isOn(DebugOption.ShowWarriorsCityText))
		{
			tStringName = tStringName + " | " + pMetaObject.countWarriors();
		}
		setText(tStringName, pCity.city_center);
		setPriority(tUnitsCount);
		_show_banner_alliance = true;
		_banner_alliance.load(pMetaObject);
	}

	internal void showTextClanFluid(Clan pMetaObject, Vector3 pPosition)
	{
		setupMeta(pMetaObject.data, pMetaObject.getColor());
		int tUnitsCount = pMetaObject.units.Count;
		string tStringName = getStringForNameplate(pMetaObject.name, tUnitsCount);
		setText(tStringName, pPosition);
		setPriority(tUnitsCount);
		_show_banner_clan = true;
		_banner_clan.load(pMetaObject);
		showSpecies(pMetaObject.getActorAsset().getSpriteIcon());
	}

	internal void showTextClanCity(Clan pMetaObject, City pCity)
	{
		setupMeta(pMetaObject.data, pMetaObject.getColor());
		int tUnitsCount = pMetaObject.units.Count;
		string tStringName = getStringForNameplate(pMetaObject.name, tUnitsCount);
		setText(tStringName, pCity.city_center);
		setPriority(tUnitsCount);
		ActorAsset tActorAsset = pMetaObject.getActorAsset();
		showSpecies(tActorAsset.getSpriteIcon());
		_show_banner_clan = true;
		_banner_clan.load(pMetaObject);
	}

	internal void showTextCity(City pMetaObject, Vector2 pPosition)
	{
		setupMeta(pMetaObject.data, pMetaObject.kingdom.getColor());
		if (pMetaObject.isCapitalCity())
		{
			setNameplateSprite("ui/nameplates/nameplate_city_capital");
		}
		else
		{
			setNameplateSprite("ui/nameplates/nameplate_city");
		}
		int tUnitsCount = pMetaObject.getPopulationPeople();
		string tStringName = getStringForNameplate(pMetaObject.name, tUnitsCount);
		if (is_full)
		{
			if (DebugConfig.isOn(DebugOption.ShowWarriorsCityText))
			{
				tStringName = tStringName + " | " + pMetaObject.countWarriors() + "/" + pMetaObject.getMaxWarriors();
				if (Config.isEditor)
				{
					string tPercent = "  :  " + (int)(pMetaObject.getArmyMaxMultiplier() * 100f) + "%";
					tStringName += tPercent;
				}
			}
			if (DebugConfig.isOn(DebugOption.ShowCityWeaponsText))
			{
				tStringName = tStringName + " | w" + pMetaObject.countWeapons();
			}
			if (DebugConfig.isOn(DebugOption.ShowFoodCityText))
			{
				tStringName = tStringName + " | F" + pMetaObject.getTotalFood();
			}
		}
		setText(tStringName, pPosition);
		if (pMetaObject.getMainSubspecies() != null)
		{
			showSpecies(pMetaObject.getMainSubspecies().getActorAsset().getSpriteIcon());
		}
		if (pMetaObject.last_visual_capture_ticks != 0)
		{
			_show_capture_counter = true;
			_active_check_dirty = true;
			if (pMetaObject.being_captured_by != null && pMetaObject.being_captured_by.isAlive())
			{
				_conquer_text.color = pMetaObject.being_captured_by.getColor().getColorText();
			}
			_conquer_text.text = pMetaObject.last_visual_capture_ticks + "%";
		}
		else
		{
			_show_capture_counter = false;
			_active_check_dirty = true;
		}
		if (_show_capture_counter)
		{
			Vector2 tAnchoredPos = ((!is_full) ? new Vector2(3f, -25f) : new Vector2(0f, -1f));
			_container_capture.anchoredPosition = tAnchoredPos;
		}
		_show_banner_city = true;
		_banner_city.load(pMetaObject);
		priority_capital = pMetaObject.isCapitalCity();
		setPriority(tUnitsCount);
	}

	private void showSpecies(string pPath)
	{
		showSpecies(SpriteTextureLoader.getSprite(pPath));
	}

	private void showSpecies(Sprite pSprite)
	{
		if (!is_mini)
		{
			_show_icon_species = true;
			_icon_species.sprite = pSprite;
		}
	}

	public void showFavoriteIcon()
	{
		_show_icon_favorite = true;
	}

	private void showSpecial(string pPath)
	{
		_show_icon_special = true;
		_icon_special.sprite = SpriteTextureLoader.getSprite(pPath);
	}

	private void setText(string pNewText, Vector3 pPos, int pAdditionalWidth = 10)
	{
		if (is_mini)
		{
			pAdditionalWidth = 0;
		}
		updatePositionAndRect(pPos);
		if (!(_old_text == pNewText))
		{
			_old_text = pNewText;
			_text_name.text = pNewText;
			_text_width = _text_name.preferredWidth + (float)pAdditionalWidth;
			float tHeight = _rect_background.sizeDelta.y;
			_text_rect.sizeDelta = new Vector2(_text_width, tHeight);
			_last_position = Globals.POINT_IN_VOID_2;
		}
	}

	private void updatePositionAndRect(Vector3 pPos)
	{
		Vector2 tNewPos = transformPosition(pPos);
		if (tNewPos != _last_position)
		{
			_last_position = tNewPos;
			base.transform.position = tNewPos;
			recalcScaledOverlapRect(_rect_background, ref map_text_rect_click);
			float tShrinkX = map_text_rect_click.width * 0f * 0.5f;
			float tShrinkY = map_text_rect_click.height * 0f * 0.5f;
			map_text_rect_overlap.x = map_text_rect_click.x + tShrinkX;
			map_text_rect_overlap.y = map_text_rect_click.y + tShrinkY;
			map_text_rect_overlap.width = map_text_rect_click.width - tShrinkX * 2f;
			map_text_rect_overlap.height = map_text_rect_click.height - tShrinkY * 2f;
		}
	}

	private string getStringForNameplate(string pName, int pCount)
	{
		if (is_mini)
		{
			return string.Empty;
		}
		return pName + " " + pCount;
	}

	private string getStringForNameplateLine(string pName, int pCount)
	{
		if (is_mini)
		{
			return string.Empty;
		}
		return pName + " - " + pCount;
	}

	public bool overlapsWithOtherPlate(NameplateText pText)
	{
		return map_text_rect_overlap.Overlaps(pText.map_text_rect_overlap);
	}

	private void recalcScaledOverlapRect(RectTransform pRectBackground, ref Rect pMapTextRect)
	{
		recalcScaledOverlapRectSimple(pRectBackground, ref pMapTextRect);
	}

	private void recalcScaledOverlapRectCorners(RectTransform pRectBackground, ref Rect pMapTextRect)
	{
		Vector3[] tCorners = new Vector3[4];
		pRectBackground.GetWorldCorners(tCorners);
		float tMinX = Mathf.Min(tCorners[0].x, tCorners[1].x, tCorners[2].x, tCorners[3].x);
		float tMaxX = Mathf.Max(tCorners[0].x, tCorners[1].x, tCorners[2].x, tCorners[3].x);
		float tMinY = Mathf.Min(tCorners[0].y, tCorners[1].y, tCorners[2].y, tCorners[3].y);
		float tMaxY = Mathf.Max(tCorners[0].y, tCorners[1].y, tCorners[2].y, tCorners[3].y);
		pMapTextRect.x = tMinX;
		pMapTextRect.y = tMinY;
		pMapTextRect.width = tMaxX - tMinX;
		pMapTextRect.height = tMaxY - tMinY;
	}

	private void recalcScaledOverlapRectSimple(RectTransform pRectBackground, ref Rect pMapTextRect)
	{
		float tScale = _manager.cached_canvas_scale;
		Vector2 tSizeDelta = pRectBackground.sizeDelta;
		if (is_mini)
		{
			tSizeDelta.Set(60f, 60f);
		}
		float tRectTransformWidth = tSizeDelta.x * 0.55f * tScale;
		float tRectTransformHeight = tSizeDelta.y * 0.55f * tScale;
		Vector3 tPosition = pRectBackground.position;
		pMapTextRect.x = tPosition.x - tRectTransformWidth * 0.5f;
		pMapTextRect.y = tPosition.y - tRectTransformHeight * 0.5f;
		pMapTextRect.width = tRectTransformWidth;
		pMapTextRect.height = tRectTransformHeight;
	}

	private Vector2 transformPosition(Vector3 pVec)
	{
		return World.world.camera.WorldToScreenPoint(pVec);
	}

	private bool checkShouldDrawObject(MetaObjectData pData)
	{
		if (_manager.cached_favorites_only && !pData.favorite)
		{
			return false;
		}
		return true;
	}

	public void setAssetAndMeta(NameplateAsset pAsset, NanoObject pNano)
	{
		nano_object = pNano;
		if (_asset != pAsset)
		{
			_active_check_dirty = true;
			_asset = pAsset;
			setNameplateSprite(pAsset.path_sprite);
			RectOffset tPaddingOffset = layout_group.padding;
			if (is_mini)
			{
				tPaddingOffset.left = 0;
				tPaddingOffset.right = 0;
			}
			else
			{
				tPaddingOffset.left = pAsset.padding_left;
				tPaddingOffset.right = pAsset.padding_right;
			}
			tPaddingOffset.top = pAsset.padding_top;
		}
	}

	public void setNameplateSprite(string pPath)
	{
		Sprite tSprite = SpriteTextureLoader.getSprite(pPath);
		_background_image.sprite = tSprite;
	}

	private void setPriority(int pValue)
	{
		priority_population = pValue;
	}
}
