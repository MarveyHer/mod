using System;
using DG.Tweening;
using UnityEngine;

public abstract class BannerBase : MonoBehaviour, IBanner, IBaseMono, IRefreshElement
{
	private Sequence _sequence;

	protected virtual MetaType meta_type
	{
		get
		{
			throw new NotImplementedException(GetType().Name);
		}
	}

	public MetaCustomizationAsset meta_asset => AssetManager.meta_customization_library.getAsset(meta_type);

	public MetaTypeAsset meta_type_asset => AssetManager.meta_type_library.getAsset(meta_type);

	internal int option_1
	{
		get
		{
			return meta_asset.option_1_get();
		}
		set
		{
			meta_asset.option_1_set(value);
		}
	}

	internal int option_2
	{
		get
		{
			return meta_asset.option_2_get();
		}
		set
		{
			meta_asset.option_2_set(value);
		}
	}

	internal int color
	{
		get
		{
			return meta_asset.color_get();
		}
		set
		{
			meta_asset.color_set(value);
		}
	}

	public virtual void load(NanoObject pObject)
	{
	}

	public virtual NanoObject GetNanoObject()
	{
		return null;
	}

	public void jump(float pSpeed = 0.1f, bool pSilent = false)
	{
		float tStart = base.transform.localPosition.y;
		_sequence.Kill();
		_sequence = DOTween.Sequence();
		_sequence.Append(base.transform.DOLocalMoveY(tStart + 5f, pSpeed));
		_sequence.Append(base.transform.DOLocalMoveY(tStart, pSpeed));
		_sequence.AppendCallback(delegate
		{
			if (!pSilent)
			{
				SoundBox.click();
			}
		});
	}

	private void OnDisable()
	{
		_sequence.Kill();
	}

	public string getName()
	{
		return GetNanoObject().name;
	}

	public virtual void showTooltip()
	{
		throw new NotImplementedException();
	}

	T IBaseMono.GetComponent<T>()
	{
		return GetComponent<T>();
	}
}
