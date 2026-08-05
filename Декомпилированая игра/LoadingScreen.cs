using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
	public delegate void TransitionAction();

	public Image background;

	public CanvasGroup canvasGroup;

	public Text percents;

	public LocalizedText topText;

	public LocalizedText tipText;

	public Image bar;

	public Image mask;

	private AsyncOperation asyncLoad;

	private bool appearDone;

	public bool inGameScreen;

	internal bool modeIn;

	public TransitionAction action;

	private float outTimer;

	public Canvas canvas;

	public Text loadingHelperText;

	private static int _last_tip = -1;

	private static int _max_tip = 0;

	private float lastBgWidth;

	private float lastBgHeight;

	private float lastCScale;

	public bool debugg;

	private void setupBg()
	{
		float wScreen = Screen.width;
		float hScreen = Screen.height;
		if (lastBgHeight != hScreen || lastBgWidth != wScreen || canvas.scaleFactor != lastCScale)
		{
			lastBgWidth = wScreen;
			lastBgHeight = hScreen;
			lastCScale = canvas.scaleFactor;
			float wBg = (float)background.mainTexture.width * canvas.scaleFactor;
			float hBg = (float)background.mainTexture.height * canvas.scaleFactor;
			float tModWidth = (float)Screen.width / wBg;
			float tModHeight = (float)Screen.height / hBg;
			if (tModWidth > tModHeight)
			{
				background.transform.localScale = new Vector3(tModWidth, tModWidth, 1f);
			}
			else
			{
				background.transform.localScale = new Vector3(tModHeight, tModHeight, 1f);
			}
		}
	}

	private void Awake()
	{
		InitLibraries.initMainLibs();
		Config.enableAutoRotation(pValue: false);
		base.transform.localPosition = default(Vector3);
		if (inGameScreen)
		{
			outTimer = 0.3f;
			canvasGroup.alpha = 1f;
			appearDone = true;
			bar.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else
		{
			canvasGroup.alpha = 0f;
			bar.transform.localScale = new Vector3(0f, 1f, 1f);
		}
	}

	private void startAction()
	{
		ScrollWindow.hideAllEvent(pWithAnimation: false);
		modeIn = false;
		if (Config.isMobile && !Config.hasPremium)
		{
			Debug.Log("PremiumElementsChecker.goodForInterstitialAd(): " + PremiumElementsChecker.goodForInterstitialAd());
			if (PremiumElementsChecker.goodForInterstitialAd())
			{
				if (PlayInterstitialAd.instance.isReady())
				{
					PlayInterstitialAd.instance.showAd();
					PremiumElementsChecker.setInterstitialAdTimer();
				}
				else
				{
					PlayInterstitialAd.instance.initAds();
				}
			}
		}
		action();
	}

	internal void startTransition(TransitionAction pAction)
	{
		Config.enableAutoRotation(pValue: false);
		action = pAction;
		bar.gameObject.SetActive(value: false);
		percents.gameObject.SetActive(value: false);
		topText.gameObject.SetActive(value: false);
		tipText.gameObject.SetActive(value: false);
		mask.gameObject.SetActive(value: false);
		base.gameObject.SetActive(value: true);
		canvasGroup.alpha = 0f;
		modeIn = true;
	}

	private void OnEnable()
	{
		string textID = "loading_screen_" + Randy.randomInt(1, 22);
		topText.key = textID;
		tipText.key = getTipID();
		topText.updateText();
		tipText.updateText();
		topText.gameObject.SetActive(value: true);
		tipText.gameObject.SetActive(value: true);
	}

	internal static string getTipID()
	{
		if (_max_tip == 0)
		{
			for (int i = 0; i < 1000 && LocalizedTextManager.stringExists(getTip(i)); i++)
			{
				_max_tip = i;
			}
		}
		int tTip = Randy.randomInt(0, _max_tip + 1);
		if (tTip == _last_tip)
		{
			return getTipID();
		}
		_last_tip = tTip;
		return getTip(tTip);
	}

	internal static string getTip(int pTip)
	{
		string tTipString = pTip.ToString();
		return "tip" + Toolbox.fillLeft(tTipString, 3, '0');
	}

	private void Update()
	{
		if (!string.IsNullOrEmpty(SmoothLoader.latest_called_id))
		{
			loadingHelperText.text = SmoothLoader.latest_called_id + ":" + SmoothLoader.latest_time;
		}
		else
		{
			loadingHelperText.text = "";
		}
		if (inGameScreen)
		{
			if (modeIn)
			{
				if (canvasGroup.alpha >= 1f)
				{
					startAction();
				}
				canvasGroup.alpha += Time.deltaTime * 2f;
				return;
			}
			if (outTimer > 0f)
			{
				outTimer -= Time.deltaTime;
				return;
			}
			if (canvasGroup.alpha <= 0f)
			{
				Config.enableAutoRotation(pValue: true);
				base.gameObject.SetActive(value: false);
			}
			if (!SmoothLoader.isLoading())
			{
				canvasGroup.alpha -= Time.fixedDeltaTime * 2f;
			}
			return;
		}
		if (!appearDone)
		{
			canvasGroup.alpha += Time.deltaTime;
			if (!(canvasGroup.alpha >= 1f))
			{
				return;
			}
			appearDone = true;
			StartCoroutine(LoadGame());
		}
		float tVal = bar.transform.localScale.x;
		if (bar.transform.localScale.x < asyncLoad.progress)
		{
			tVal = bar.transform.localScale.x + Time.deltaTime * 2f;
			if (tVal > asyncLoad.progress)
			{
				tVal = asyncLoad.progress;
			}
			bar.transform.localScale = new Vector3(tVal, 1f, 1f);
		}
		percents.text = Mathf.CeilToInt(asyncLoad.progress * 100f) + " %";
		if (tVal >= 0.9f)
		{
			if (!asyncLoad.allowSceneActivation)
			{
				Analytics.LogEvent("preloading_done");
			}
			asyncLoad.allowSceneActivation = true;
		}
	}

	private IEnumerator LoadGame()
	{
		asyncLoad = SceneManager.LoadSceneAsync("World");
		asyncLoad.allowSceneActivation = false;
		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}
}
