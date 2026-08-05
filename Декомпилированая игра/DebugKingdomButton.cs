using UnityEngine;
using UnityEngine.UI;

public class DebugKingdomButton : MonoBehaviour
{
	[SerializeField]
	private Button _button;

	[SerializeField]
	private Image _image;

	internal KingdomAsset kingdom_asset;

	[SerializeField]
	private Image _discrepancy_bad;

	[SerializeField]
	private Image _discrepancy_have;

	[SerializeField]
	private Image _discrepancy_normal;

	public Image image => _image;

	public void setAsset(KingdomAsset pAsset)
	{
		kingdom_asset = pAsset;
		_image.sprite = kingdom_asset.getSprite();
		setupTooltip();
		if (kingdom_asset.assets_discrepancies_bad != null)
		{
			_discrepancy_have.gameObject.SetActive(value: true);
		}
		else
		{
			_discrepancy_have.gameObject.SetActive(value: false);
		}
	}

	public void checkSelected(KingdomAsset pAssetMain)
	{
		_discrepancy_bad.gameObject.SetActive(value: false);
		_discrepancy_normal.gameObject.SetActive(value: false);
		if (kingdom_asset == pAssetMain)
		{
			image.color = Color.white;
			return;
		}
		if (kingdom_asset.assets_discrepancies != null && kingdom_asset.assets_discrepancies.Contains(pAssetMain.id))
		{
			_discrepancy_normal.gameObject.SetActive(value: true);
		}
		if (pAssetMain.assets_discrepancies_bad != null && pAssetMain.assets_discrepancies_bad.Contains(kingdom_asset.id))
		{
			_discrepancy_bad.gameObject.SetActive(value: true);
		}
		if (pAssetMain.isFoe(kingdom_asset))
		{
			image.color = new Color(0.2f, 0.2f, 0.2f);
		}
		else
		{
			image.color = Color.white;
		}
	}

	public void setupTooltip()
	{
		if (TryGetComponent<TipButton>(out var tTipButton))
		{
			tTipButton.hoverAction = showTooltip;
		}
	}

	private void showTooltip()
	{
		Tooltip.show(base.gameObject, "debug_kingdom_assets", new TooltipData
		{
			kingdom_asset = kingdom_asset
		});
	}

	public static void getTooltipDescription(KingdomAsset pAsset, out string pDescription, out string pDescription2)
	{
		pDescription = string.Empty;
		pDescription2 = string.Empty;
		if (pAsset.list_tags.Count > 0)
		{
			pDescription += "--- OWN TAGS ---\n".ColorHex(ColorStyleLibrary.m.color_text_grey_dark);
			foreach (string tTag in pAsset.list_tags)
			{
				pDescription += (tTag + "\n").ColorHex(ColorStyleLibrary.m.color_text_grey);
			}
		}
		if (pAsset.friendly_tags.Count > 0)
		{
			pDescription += "--- FRIENDLY TAGS ---\n".ColorHex(ColorStyleLibrary.m.color_text_grey_dark);
			foreach (string tTag2 in pAsset.friendly_tags)
			{
				pDescription += (tTag2 + "\n").ColorHex("#43FF43");
			}
		}
		if (pAsset.enemy_tags.Count > 0)
		{
			pDescription += "#--- ENEMY TAGS ---\n".ColorHex(ColorStyleLibrary.m.color_text_grey_dark);
			foreach (string tTag3 in pAsset.enemy_tags)
			{
				pDescription += (tTag3 + "\n").ColorHex("#FB2C21");
			}
		}
		if (pAsset.assets_discrepancies == null || pAsset.assets_discrepancies.Count <= 0)
		{
			return;
		}
		pDescription2 = $"!! Discrepancies {pAsset.assets_discrepancies.Count}!!\n".ColorHex("#D85BC5");
		int tCount = 0;
		foreach (string tID in pAsset.assets_discrepancies)
		{
			if (tID.Contains(pAsset.id) || pAsset.id.Contains(tID))
			{
				pDescription2 += tID.ColorHex("#FB2C21");
			}
			else
			{
				pDescription2 += tID;
			}
			if (pDescription2.Length > 150)
			{
				int tLeft = pAsset.assets_discrepancies.Count - tCount;
				pDescription2 += $" and {tLeft} more...!!!".ColorHex("#8CFF99");
				break;
			}
			if (tCount < pAsset.assets_discrepancies.Count - 1)
			{
				pDescription2 += ", ";
			}
			tCount++;
		}
	}
}
