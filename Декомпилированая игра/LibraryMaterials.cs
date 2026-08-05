using System.Collections.Generic;
using UnityEngine;

public class LibraryMaterials : MonoBehaviour
{
	public static LibraryMaterials instance;

	public const string mat_id_world_object = "mat_world_object";

	public const string mat_id_world_object_lit_always = "mat_world_object_lit";

	public Dictionary<string, Material> dict = new Dictionary<string, Material>();

	public Material mat_damaged;

	public Material mat_highlighted;

	public Material mat_world_object;

	public Material mat_world_object_lit;

	public Material mat_buildings;

	public Material mat_socialize;

	public Material mat_minis;

	public Material mat_tree;

	public Material mat_tree_celestial;

	public Material mat_jelly;

	public Material mat_buildings_light;

	public Material mat_lava_glow;

	public Material mat_overlapped_shadows;

	private float _shadow_alpha_target = 0.40392157f;

	private float _shadow_alpha = 0.40392157f;

	private Color _shadows_color;

	private List<Material> _night_affected_colors = new List<Material>();

	private float _time;

	private void Awake()
	{
		instance = this;
		mat_damaged = loadMaterial("materials/damaged", pCopy: true);
		mat_highlighted = loadMaterial("materials/highlighted", pCopy: true);
		mat_buildings = loadMaterial("materials/building", pCopy: true);
		mat_tree = loadMaterial("materials/tree", pCopy: true);
		mat_socialize = loadMaterial("materials/socialize");
		mat_minis = loadMaterial("materials/minis");
		mat_tree_celestial = loadMaterial("materials/tree_celestial", pCopy: true);
		mat_jelly = loadMaterial("materials/jelly", pCopy: true);
		mat_overlapped_shadows = loadMaterial("materials/OverlappedShadows", pCopy: true);
		mat_buildings_light = loadMaterial("materials/MatBuildingsLight");
		mat_world_object = loadMaterial("materials/mat_world_object");
		mat_world_object_lit = loadMaterial("materials/mat_world_object_lit");
		mat_lava_glow = loadMaterial("materials/lava_glow", pCopy: true);
		_night_affected_colors.Add(mat_buildings);
		_night_affected_colors.Add(mat_tree);
		_night_affected_colors.Add(mat_jelly);
		_night_affected_colors.Add(mat_world_object);
		_shadows_color = mat_overlapped_shadows.GetColor("_Color");
		AssetManager.status.linkMaterials();
		Shader.SetGlobalFloat("GlobalTime", 1f);
	}

	private Material loadMaterial(string pPath, bool pCopy = false)
	{
		Material tMat = Resources.Load<Material>(pPath);
		if (pCopy)
		{
			tMat = Object.Instantiate(tMat);
		}
		string tNameId = tMat.name;
		tNameId = tNameId.Replace("(Clone)", "");
		dict.Add(tNameId, tMat);
		return tMat;
	}

	internal void updateMat()
	{
		if (!World.world.isPaused())
		{
			_time += World.world.elapsed;
		}
		updateNight();
		Shader.SetGlobalFloat("GlobalTime", _time);
	}

	private void updateNight()
	{
		float tNightMod = World.world.era_manager.getNightMod();
		Color tColor = Toolbox.blendColor(Toolbox.color_night, Toolbox.color_white, tNightMod);
		foreach (Material night_affected_color in _night_affected_colors)
		{
			night_affected_color.color = tColor;
		}
		Color tColorOcean = Toolbox.blendColor(Toolbox.color_night, Toolbox.color_ocean, tNightMod);
		if (tNightMod > 0f)
		{
			tColorOcean.r -= 0.007843138f;
			tColorOcean.b -= 1f / 51f;
		}
		World.world.camera.backgroundColor = tColorOcean;
	}

	public void updateZoomoutValue(float pValue)
	{
		float tShaderVar = pValue;
		tShaderVar = 4f - tShaderVar * 3f;
		if (!DebugConfig.isOn(DebugOption.ScaleEffectEnabled))
		{
			tShaderVar = 1f;
		}
		_shadow_alpha = tShaderVar * _shadow_alpha_target;
		_shadows_color.a = _shadow_alpha;
		mat_overlapped_shadows.SetColor("_Color", _shadows_color);
	}
}
