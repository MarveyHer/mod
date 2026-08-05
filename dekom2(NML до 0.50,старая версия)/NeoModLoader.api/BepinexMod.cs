using UnityEngine;

namespace NeoModLoader.api;

public class BepinexMod : VirtualMod
{
	private MonoBehaviour _modComponent;

	public MonoBehaviour GetModComponent()
	{
		return _modComponent;
	}

	public void OnLoad(ModDeclare pModDecl, MonoBehaviour pModComponent)
	{
		base.OnLoad(pModDecl, (pModComponent != null) ? ((Component)pModComponent).gameObject : null);
		_modComponent = pModComponent;
	}
}
