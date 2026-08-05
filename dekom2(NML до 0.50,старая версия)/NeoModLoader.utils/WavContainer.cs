using Newtonsoft.Json;

namespace NeoModLoader.utils;

internal struct WavContainer(string Path, bool _3D, float Volume, int LoopCount = 0, bool Ramp = false, SoundType Type = SoundType.Sound)
{
	[JsonIgnore]
	public string Path = Path;

	[JsonProperty("3D")]
	public bool _3D = _3D;

	public float Volume = Volume;

	public SoundType Type = Type;

	public int LoopCount = LoopCount;

	public bool Ramp = Ramp;
}
