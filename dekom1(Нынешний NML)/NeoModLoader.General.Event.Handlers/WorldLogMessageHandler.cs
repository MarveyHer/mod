using UnityEngine;

namespace NeoModLoader.General.Event.Handlers;

public abstract class WorldLogMessageHandler : AbstractHandler<WorldLogMessageHandler>
{
	public abstract void Handle(ref WorldLogMessage pMessage, ref string pText, ref Color pColor, ref bool pColorField, bool pColorTags);
}
