using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HoveringBgIconManager : MonoBehaviour
{
	[SerializeField]
	private HoveringIcon _icon_prefab;

	private ObjectPoolGenericMono<HoveringIcon> _pool_icons;

	private CanvasGroup _canvas_group;

	private RectTransform _rect;

	private List<RectTransform> _places = new List<RectTransform>();

	[SerializeField]
	public bool _random_scale = true;

	[SerializeField]
	private Transform _icon_pool;

	[SerializeField]
	private Transform _icons;

	private static HoveringBgIconManager _instance;

	private void Awake()
	{
		if (_pool_icons == null)
		{
			_instance = this;
			_rect = GetComponent<RectTransform>();
			_canvas_group = GetComponent<CanvasGroup>();
			_pool_icons = new ObjectPoolGenericMono<HoveringIcon>(_icon_prefab, _icon_pool);
			for (int i = 0; i < _icons.childCount; i++)
			{
				RectTransform tChild = _icons.GetChild(i) as RectTransform;
				_places.Add(tChild);
				tChild.gameObject.name = "Placing " + i;
			}
		}
	}

	private void OnDisable()
	{
		_pool_icons.clear();
	}

	public void fadeIn()
	{
		_icons.gameObject.SetActive(value: true);
		_canvas_group.DOFade(1f, 0.2f);
		_canvas_group.interactable = true;
		_canvas_group.blocksRaycasts = true;
	}

	public void fadeOut()
	{
		_canvas_group.interactable = false;
		_canvas_group.blocksRaycasts = false;
		_canvas_group.DOFade(0f, 0.2f);
		clear();
		resetPlaces();
		_icons.gameObject.SetActive(value: false);
	}

	private void resetPlaces()
	{
		if (!Randy.randomBool())
		{
			float tCenterX = _rect.rect.width / 2f;
			float tCenterY = _rect.rect.height / 2f;
			Vector3 tCenter = new Vector3(tCenterX, tCenterY, 0f);
			for (int i = 0; i < _places.Count; i++)
			{
				RectTransform rectTransform = _places[i];
				rectTransform.DOKill();
				rectTransform.anchoredPosition = tCenter;
			}
		}
	}

	private void shufflePlaces()
	{
		resetPlaces();
		float tMaxX = _rect.rect.width;
		float tMaxY = _rect.rect.height;
		for (int i = 0; i < _places.Count; i++)
		{
			_places[i].DOAnchorPos(duration: Randy.randomFloat(0.15f, 0.35f), endValue: new Vector3(Randy.randomFloat(0f, tMaxX), Randy.randomFloat(0f, tMaxY), 0f));
		}
	}

	public void animate(WindowAsset pWindowAsset)
	{
		clear();
		shufflePlaces();
		float tStartAngle = Randy.randomFloat(0f, 360f);
		string tMainPath = "ui/Icons/";
		using ListPool<string> tResultList = new ListPool<string>(16);
		Delegate[] invocationList = pWindowAsset.get_hovering_icons.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			foreach (string tPath in ((HoveringBGIconsGetter)invocationList[i])(pWindowAsset))
			{
				if (tPath.EndsWith("/"))
				{
					Sprite[] tSprites = SpriteTextureLoader.getSpriteList(tMainPath + tPath);
					for (int j = 0; j < tSprites.Length; j++)
					{
						string tResultString = tMainPath + tPath + tSprites[j].name;
						tResultList.Add(tResultString);
					}
				}
				else
				{
					string tResultString2 = tMainPath + tPath;
					tResultList.Add(tResultString2);
				}
			}
		}
		foreach (RectTransform tPlace in _places)
		{
			string tIconToLoadPath = tResultList.GetRandom();
			HoveringIcon tIcon = _pool_icons.getNext();
			tIcon.clear();
			tIcon.transform.SetParent(tPlace, worldPositionStays: false);
			tIcon.rect.anchoredPosition = Vector3.zero;
			tIcon.transform.rotation = Quaternion.identity;
			tIcon.image.sprite = SpriteTextureLoader.getSprite(tIconToLoadPath);
			if (_random_scale)
			{
				float tScale = Randy.randomFloat(0.4f, 1f);
				tIcon.transform.localScale = new Vector3(tScale, tScale, tScale);
			}
			else
			{
				tIcon.transform.localScale = tPlace.localScale;
			}
			Vector3 tCurScale = tIcon.transform.localScale;
			tIcon.image.color = new Color(tCurScale.x, tCurScale.x, tCurScale.x, 1f);
			tStartAngle += Randy.randomFloat(20f, 130f);
			tIcon.transform.eulerAngles = new Vector3(0f, 0f, tStartAngle);
			tIcon.init();
		}
	}

	public static void show()
	{
		_instance.fadeIn();
	}

	public static void hide()
	{
		_instance.fadeOut();
	}

	public static void showWindow(WindowAsset pWindowAsset)
	{
		_instance.animate(pWindowAsset);
	}

	public static void dropAll()
	{
		foreach (HoveringIcon tIcon in _instance._pool_icons.getListTotal())
		{
			if (tIcon.gameObject.activeSelf)
			{
				UiCreature tCreature = tIcon.GetComponent<UiCreature>();
				if (!tCreature.dropped)
				{
					tCreature.click();
				}
			}
		}
	}

	public static void randomDrop()
	{
		using ListPool<UiCreature> tList = new ListPool<UiCreature>(_instance._pool_icons.countActive());
		foreach (HoveringIcon tIcon in _instance._pool_icons.getListTotal())
		{
			if (tIcon.gameObject.activeSelf)
			{
				UiCreature tCreature = tIcon.GetComponent<UiCreature>();
				if (!tCreature.dropped)
				{
					tList.Add(tCreature);
				}
			}
		}
		if (tList.Count != 0)
		{
			tList.GetRandom().click();
		}
	}

	private void clear()
	{
		_pool_icons.clear();
		_pool_icons.resetParent();
	}
}
