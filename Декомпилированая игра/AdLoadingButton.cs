using UnityEngine;
using UnityEngine.UI;

public class AdLoadingButton : MonoBehaviour
{
	public Text button_text;

	public LocalizedText button_localized_text;

	public Button button;

	private Image button_image;

	public Sprite spriteOn;

	public Sprite spriteOff;

	private AdLoadingButtonState state;

	private void Awake()
	{
		button_image = button.GetComponent<Image>();
		button_localized_text = button_text.GetComponent<LocalizedText>();
		state = AdLoadingButtonState.None;
	}

	private void Update()
	{
		AdLoadingButtonState tState = AdLoadingButtonState.None;
		if (Config.isEditor && Config.editor_test_rewards_from_ads)
		{
			tState = AdLoadingButtonState.AdReady;
			state = tState;
			toggleState();
			return;
		}
		if (RewardedAds.isReady())
		{
			tState = AdLoadingButtonState.AdReady;
		}
		else if (!Config.adsInitialized)
		{
			tState = AdLoadingButtonState.Initializing;
		}
		else
		{
			tState = AdLoadingButtonState.AdLoading;
			RewardedAds.trimTimeout();
		}
		if (tState != state)
		{
			state = tState;
			toggleState();
		}
	}

	private void toggleState()
	{
		switch (state)
		{
		case AdLoadingButtonState.Initializing:
			button.interactable = false;
			button_localized_text.setKeyAndUpdate("waiting_for_ad");
			button_image.sprite = spriteOff;
			break;
		case AdLoadingButtonState.AdLoading:
			button.interactable = false;
			button_localized_text.setKeyAndUpdate("loading_ads");
			button_image.sprite = spriteOff;
			break;
		case AdLoadingButtonState.AdReady:
			button.interactable = true;
			button_localized_text.setKeyAndUpdate("watch_ad");
			button_image.sprite = spriteOn;
			break;
		}
	}
}
