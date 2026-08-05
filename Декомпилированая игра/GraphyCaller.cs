using UnityEngine;

public class GraphyCaller : MonoBehaviour
{
	private int clicked;

	public void click()
	{
		clicked++;
		if (clicked > 10)
		{
			bool tIsOn = DebugConfig.isOn(DebugOption.DebugButton);
			DebugConfig.setOption(DebugOption.DebugButton, !tIsOn);
			DebugConfig.instance.debugButton.SetActive(!tIsOn);
		}
	}

	public void clickConsole()
	{
		clicked++;
		if (clicked > 10)
		{
			World.world.console.Show();
		}
	}

	private void OnEnable()
	{
		clicked = 0;
	}
}
