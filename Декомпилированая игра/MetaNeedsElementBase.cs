using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MetaNeedsElementBase : WindowMetaElementBase, IRefreshElement
{
	[SerializeField]
	private GameObject _container;

	[SerializeField]
	private Text _text;

	private Actor _actor;

	private IMetaWindow _window;

	protected override void Awake()
	{
		base.Awake();
		setupTooltip();
		setupClickAction();
		_window = GetComponentInParent<IMetaWindow>();
	}

	protected override IEnumerator showContent()
	{
		if (_window.getCoreObject() is IMetaObject tMeta && tMeta.isAlive())
		{
			string tFinalText = getText(tMeta, out var tActorResult);
			_text.text = tFinalText;
			_actor = tActorResult;
			if (!string.IsNullOrEmpty(tFinalText))
			{
				_container.gameObject.SetActive(value: true);
			}
		}
		yield break;
	}

	protected virtual string getText(IMetaObject pMeta, out Actor pActorResult)
	{
		throw new NotImplementedException();
	}

	private void setupTooltip()
	{
		if (_container.TryGetComponent<TipButton>(out var tTipButton))
		{
			tTipButton.hoverAction = tooltipAction;
		}
	}

	private void setupClickAction()
	{
		if (_container.TryGetComponent<Button>(out var tButton))
		{
			tButton.onClick.AddListener(buttonAction);
		}
	}

	private void tooltipAction()
	{
		if (!_actor.isRekt())
		{
			_actor.showTooltip(this);
		}
	}

	private void buttonAction()
	{
		if (!_actor.isRekt())
		{
			ActionLibrary.openUnitWindow(_actor);
		}
	}

	protected override void clear()
	{
		base.clear();
		_container.gameObject.SetActive(value: false);
	}
}
