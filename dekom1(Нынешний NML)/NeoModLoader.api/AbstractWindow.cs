using NeoModLoader.General;
using UnityEngine;

namespace NeoModLoader.api;

public abstract class AbstractWindow<T> : MonoBehaviour where T : AbstractWindow<T>
{
	protected bool Initialized;

	protected bool IsOpened;

	protected bool IsFirstOpen = true;

	public static T Instance { get; protected set; }

	protected Transform ContentTransform { get; set; }

	protected Transform BackgroundTransform { get; set; }

	public static string WindowId { get; protected set; }

	public static T CreateAndInit(string pWindowId)
	{
		WindowId = pWindowId;
		ScrollWindow scrollWindow = WindowCreator.CreateEmptyWindow(pWindowId, pWindowId + " Title");
		GameObject gameObject = ((Component)scrollWindow).gameObject;
		Instance = gameObject.AddComponent<T>();
		((Component)Instance).gameObject.SetActive(false);
		Instance.BackgroundTransform = ((Component)scrollWindow).transform.Find("Background");
		((Component)Instance.BackgroundTransform.Find("Scroll View")).gameObject.SetActive(true);
		Instance.ContentTransform = Instance.BackgroundTransform.Find("Scroll View/Viewport/Content");
		Instance.Init();
		Instance.Initialized = true;
		return Instance;
	}

	protected abstract void Init();

	private void OnEnable()
	{
		if (Initialized)
		{
			if (IsFirstOpen)
			{
				IsFirstOpen = false;
				OnFirstEnable();
			}
			OnNormalEnable();
			IsOpened = true;
		}
	}

	private void OnDisable()
	{
		if (Initialized)
		{
			IsOpened = false;
			OnNormalDisable();
		}
	}

	public virtual void OnNormalDisable()
	{
	}

	public virtual void OnFirstEnable()
	{
	}

	public virtual void OnNormalEnable()
	{
	}
}
