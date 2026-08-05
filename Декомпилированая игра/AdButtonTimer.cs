using UnityEngine;
using UnityEngine.UI;

public class AdButtonTimer : MonoBehaviour
{
	internal static AdButtonTimer instance;

	public Text timer;

	public Button button;

	public Image icon;

	private double adTimer;

	private Color transparent = new Color(1f, 1f, 1f, 0.3f);

	private int tRecalc;

	private void Awake()
	{
		instance = this;
		adTimer = 10.0;
	}

	internal static void setAdTimer()
	{
		if (PlayerConfig.instance != null)
		{
			double tDiff = PlayerConfig.instance.data.nextAdTimestamp;
			tDiff -= Epoch.Current();
			instance.adTimer = tDiff;
			if (instance.adTimer < 0.0 || PlayerConfig.instance.data.nextAdTimestamp == -1.0)
			{
				instance.adTimer = -1.0;
			}
		}
	}

	private void OnEnable()
	{
		setAdTimer();
		updateButton();
	}

	private void Update()
	{
		if (Config.hasPremium)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		if (adTimer > 0.0)
		{
			adTimer -= Time.deltaTime;
		}
		updateButton();
	}

	private void updateButton()
	{
		if (tRecalc > 0)
		{
			tRecalc--;
		}
		else
		{
			tRecalc = 10;
			setAdTimer();
		}
		if (adTimer > 0.0)
		{
			timer.gameObject.SetActive(value: true);
			timer.text = Toolbox.formatTimer((float)adTimer);
			icon.color = transparent;
		}
		else
		{
			timer.gameObject.SetActive(value: false);
			icon.color = Color.white;
		}
	}
}
