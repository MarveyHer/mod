namespace NeoModLoader.General.Event;

public abstract class AbstractHandler<THandler> where THandler : AbstractHandler<THandler>
{
	private int error_hit = 0;

	public bool enabled { get; private set; } = true;

	internal void HitException()
	{
		error_hit++;
		if (error_hit > 10)
		{
			enabled = false;
		}
	}
}
