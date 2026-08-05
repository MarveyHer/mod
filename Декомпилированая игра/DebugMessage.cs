using System.Collections.Generic;
using UnityEngine;

public class DebugMessage : MonoBehaviour
{
	public GameObject prefab;

	public static bool log_enabled;

	public static DebugMessage instance;

	public List<DebugMessageFly> list;

	private List<DebugMessageFly> messagesToMove = new List<DebugMessageFly>();

	private void Start()
	{
		instance = this;
		list = new List<DebugMessageFly>();
	}

	public void moveAll(DebugMessageFly pMessage)
	{
		messagesToMove.Clear();
		foreach (DebugMessageFly tMessage in list)
		{
			if (!(tMessage == pMessage) && Toolbox.Dist(0f, tMessage.transform.localPosition.y, 0f, pMessage.transform.localPosition.y) < 1f)
			{
				messagesToMove.Add(tMessage);
			}
		}
		foreach (DebugMessageFly item in messagesToMove)
		{
			item.moveUp();
		}
	}

	public DebugMessageFly getOldMessage(Transform pTransform)
	{
		foreach (DebugMessageFly tMessage in list)
		{
			if (tMessage.originTransform == pTransform)
			{
				return tMessage;
			}
		}
		return null;
	}

	public static void log(Transform pTransofrm, string pMessage)
	{
		if (Debug.isDebugBuild && log_enabled)
		{
			DebugMessageFly tOldMsg = instance.getOldMessage(pTransofrm);
			if (tOldMsg != null)
			{
				tOldMsg.addString(pMessage);
				return;
			}
			TextMesh component = Object.Instantiate(instance.prefab).gameObject.GetComponent<TextMesh>();
			component.gameObject.GetComponent<MeshRenderer>().sortingOrder = 100;
			component.transform.parent = instance.transform;
			DebugMessageFly tMsg = component.GetComponent<DebugMessageFly>();
			tMsg.originTransform = pTransofrm;
			tMsg.addString(pMessage);
			instance.list.Add(tMsg);
		}
	}
}
