using UnityEngine;

namespace NeoModLoader.api;

public class VirtualMod : IMod
{
	private ModDeclare _declare;

	private GameObject _boundGameObject;

	public ModDeclare GetDeclaration()
	{
		return _declare;
	}

	public GameObject GetGameObject()
	{
		return _boundGameObject;
	}

	public string GetUrl()
	{
		return string.IsNullOrEmpty(_declare.RepoUrl) ? "https://github.com/WorldBoxOpenMods" : _declare.RepoUrl;
	}

	public void OnLoad(ModDeclare pModDecl, GameObject pGameObject)
	{
		_declare = pModDecl;
		_boundGameObject = pGameObject;
	}
}
