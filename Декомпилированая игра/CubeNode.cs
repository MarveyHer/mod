using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CubeNode : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler
{
	private const float SCALE_HIGHLIGHTED = 1.6f;

	private const float SCALE_NORMAL = 1f;

	private const float TOOLTIP_SCALE_MIN = 0.4f;

	private const float TOOLTIP_SCALE_MAX = 1f;

	public Vector4 logical_pos;

	internal List<CubeNode> connected_nodes = new List<CubeNode>();

	private List<CubeNodeConnection> connections = new List<CubeNodeConnection>();

	[SerializeField]
	private Image _image;

	[SerializeField]
	private Text _text;

	private CubeOverview _cube_overview;

	internal float render_depth;

	internal float scale_mod_spawn = 1f;

	internal float bonus_scale = 1f;

	internal bool highlighted;

	private float _timer_change;

	private TooltipData _tooltip_data;

	private CubeNodeAssetData _data;

	public BaseUnlockableAsset current_asset => _data.asset;

	private void Start()
	{
		_cube_overview = base.gameObject.GetComponentInParent<CubeOverview>();
		initClick();
		initTooltip();
	}

	public void update()
	{
		_timer_change -= Time.deltaTime;
	}

	public void setDebugText(string pText)
	{
		_text.text = pText;
	}

	public void clear()
	{
		connected_nodes.Clear();
		connections.Clear();
		_timer_change = 0f;
	}

	protected void initClick()
	{
		if (TryGetComponent<Button>(out var tButton))
		{
			tButton.onClick.AddListener(setPressed);
		}
	}

	protected void initTooltip()
	{
		if (TryGetComponent<TipButton>(out var tTipButton))
		{
			Object.Destroy(tTipButton);
		}
	}

	private void showTooltip()
	{
		_cube_overview.setLatestTouched(this);
		KnowledgeAsset tKnowledgeAsset = AssetManager.knowledge_library.get(_data.knowledge_type);
		_tooltip_data = tKnowledgeAsset.show_tooltip(base.transform, _data.asset);
	}

	public void setupAsset(CubeNodeAssetData pData)
	{
		if (!(_timer_change > 0f))
		{
			_timer_change = 2f;
			_data = pData;
			_image.sprite = _data.asset.getSprite();
		}
	}

	public void updateTooltip()
	{
		if (highlighted && Tooltip.isShowingFor(base.transform))
		{
			_tooltip_data.tooltip_scale = Mathf.Lerp(0.4f, 1f, render_depth);
		}
	}

	public void setHighlighted()
	{
		if (!highlighted)
		{
			highlighted = true;
			scale_mod_spawn = 1.6f;
			showTooltip();
		}
	}

	public void setPressed()
	{
		_cube_overview.isDragging();
	}

	public void setColor(Color pColor)
	{
		_image.color = pColor;
	}

	public void addConnection(CubeNode pNode, CubeNodeConnection pConnection)
	{
		connected_nodes.Add(pNode);
		connections.Add(pConnection);
	}

	public void OnInitializePotentialDrag(PointerEventData pEventData)
	{
		_cube_overview?.SendMessage("OnInitializePotentialDrag", pEventData);
	}

	public void OnBeginDrag(PointerEventData pEventData)
	{
		_cube_overview?.SendMessage("OnBeginDrag", pEventData);
	}

	public void OnDrag(PointerEventData pEventData)
	{
		_cube_overview?.SendMessage("OnDrag", pEventData);
	}

	public void OnEndDrag(PointerEventData pEventData)
	{
		_cube_overview?.SendMessage("OnEndDrag", pEventData);
	}
}
