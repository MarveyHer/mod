using UnityEngine;

namespace NeoModLoader.api;

public class AttachedModComponent : MonoBehaviour, IMod
{
	private ModDeclare _declare;

	public ModDeclare GetDeclaration()
	{
		return _declare;
	}

	public GameObject GetGameObject()
	{
		return ((Component)this).gameObject;
	}

	public string GetUrl()
	{
		return string.IsNullOrEmpty(_declare.RepoUrl) ? "https://github.com/WorldBoxOpenMods" : _declare.RepoUrl;
	}

	public void OnLoad(ModDeclare pModDecl, GameObject pGameObject)
	{
		_declare = pModDecl;
	}
}
