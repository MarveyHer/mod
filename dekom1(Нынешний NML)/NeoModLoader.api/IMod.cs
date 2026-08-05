using UnityEngine;

namespace NeoModLoader.api;

public interface IMod
{
	ModDeclare GetDeclaration();

	GameObject GetGameObject();

	string GetUrl();

	void OnLoad(ModDeclare pModDecl, GameObject pGameObject);
}
