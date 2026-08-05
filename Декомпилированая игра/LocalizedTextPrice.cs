using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class LocalizedTextPrice : MonoBehaviour
{
	public static string price_current = "???";

	public static string price_old = string.Empty;

	public static string discount = string.Empty;

	public Text text_old_price;

	public Text text_current_price;

	public GameObject discount_bg;

	public Text text_percent;

	private const string IN_APP_ID = "premium";

	internal void updateText(bool pCheckText = true)
	{
		if (!string.IsNullOrEmpty(discount))
		{
			showDiscount(discount);
		}
		string tString = "";
		if (InAppManager.instance != null && InAppManager.instance?.controller?.products != null)
		{
			Product product = InAppManager.instance.controller.products.WithID("premium");
			if (product != null)
			{
				tString = product.metadata.localizedPriceString;
			}
		}
		else
		{
			tString = price_current;
		}
		text_current_price.text = tString;
		if (!string.IsNullOrEmpty(price_old))
		{
			text_old_price.text = price_old;
			text_old_price.gameObject.SetActive(value: true);
		}
	}

	private void showDiscount(string pString)
	{
		text_percent.text = pString;
		discount_bg.gameObject.SetActive(value: true);
	}

	private void setDefault()
	{
		discount_bg.gameObject.SetActive(value: false);
		text_current_price.gameObject.SetActive(value: true);
		text_current_price.text = "??";
		text_old_price.gameObject.SetActive(value: false);
	}

	private void OnEnable()
	{
		setDefault();
		updateText();
	}
}
