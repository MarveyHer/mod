using UnityEngine;

public class ButtonGraphListCompare : MonoBehaviour
{
	public void compareListItems()
	{
		ScrollWindow tCurrentWindow = ScrollWindow.getCurrentWindow();
		IComponentList tComponent = tCurrentWindow.GetComponentInChildren<IComponentList>(includeInactive: true);
		if (tComponent == null)
		{
			Debug.LogError("IComponentList missing in " + tCurrentWindow.gameObject.name, tCurrentWindow.gameObject);
			return;
		}
		using ListPool<NanoObject> tElements = tComponent.getElements();
		if (tElements.Count > 0)
		{
			Config.selected_objects_graph.Clear();
			for (int i = 0; i < tElements.Count && i < 3; i++)
			{
				NanoObject tElement = tElements[i];
				Config.selected_objects_graph.Add(tElement);
			}
		}
		ScrollWindow.showWindow("chart_comparer");
	}
}
