using UnityEngine;

public class ButtonPremium : MonoBehaviour
{
	public void clickPremium()
	{
		PlayerConfig.setFirebaseProp("clicked_buy_premium", "yes");
		Analytics.LogEvent("clicked_buy_premium");
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			ScrollWindow.showWindow("premium_purchase_error");
		}
		else
		{
			InAppManager.instance.buyPremium();
		}
	}
}
