using UnityEngine;

public static class CoroutineHelper
{
	public static WaitForSecondsRealtime wait_for_0_5_s => new WaitForSecondsRealtime(0.5f);

	public static WaitForSecondsRealtime wait_for_0_01_s => new WaitForSecondsRealtime(0.01f);

	public static WaitForSecondsRealtime wait_for_0_05_s => new WaitForSecondsRealtime(0.05f);

	public static WaitForSecondsRealtime wait_for_0_025_s => new WaitForSecondsRealtime(0.025f);

	public static YieldInstruction wait_for_end_of_frame => new WaitForEndOfFrame();

	public static YieldInstruction wait_for_next_frame => null;
}
