namespace NeoModLoader.General.Event.Handlers;

public abstract class CultureCreateHandler : AbstractHandler<CultureCreateHandler>
{
	public abstract void Handle(Culture pCulture, Actor pActor, City pCity);
}
