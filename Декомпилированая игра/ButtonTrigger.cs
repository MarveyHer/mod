using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct ButtonTrigger(Button pButton, EventTrigger.Entry pEntry, int pIndex)
{
	public Button button { get; } = pButton;

	public EventTrigger.Entry entry { get; } = pEntry;

	public int index { get; } = pIndex;
}
