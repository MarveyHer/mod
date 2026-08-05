namespace NeoModLoader.General.Game.extensions;

public interface ICustomData
{
	SerializedCustomData Serialize();

	void Deserialize(SerializedCustomData data);
}
