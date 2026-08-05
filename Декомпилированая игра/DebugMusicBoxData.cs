using FMOD.Studio;

public class DebugMusicBoxData
{
	public const float INTERVAL = 3f;

	public float timer = 3f;

	public string path;

	public float x;

	public float y;

	public EventInstance instance;

	public bool isPlaying()
	{
		instance.getPlaybackState(out var tState);
		return tState == PLAYBACK_STATE.PLAYING;
	}
}
