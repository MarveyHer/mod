using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicBox : MonoBehaviour
{
	private const int MUSIC_ZONES_SIZE = 3;

	private const int IDLE_SOUND_TIMER_MIN = 5;

	private const int IDLE_SOUND_TIMER_MAX = 12;

	public static MusicBox inst;

	private readonly HashSet<string> _flags_to_enable = new HashSet<string>();

	private EventInstance _music_event;

	internal MusicBoxDebug debug_box;

	private float _timer;

	private const float INTERVAL_UPDATE = 1f;

	public static bool music_on = true;

	public static bool sounds_on = true;

	public static bool debug_sounds = true;

	private VCA _vca_sound_effects;

	private VCA _vca_music;

	private VCA _vca_ui;

	private Bus _bus_master;

	private Bus _bus_idle;

	private float _volume_idle = 1f;

	private EVENT_CALLBACK _music_callback;

	private TimelineInfo _timeline_info;

	private GCHandle _timeline_handle;

	public static bool new_world_on_start_played = false;

	private readonly Dictionary<string, EventInstance> _environment_sounds = new Dictionary<string, EventInstance>();

	private readonly Dictionary<string, EventInstance> _drawing_sounds = new Dictionary<string, EventInstance>();

	private static readonly Dictionary<string, bool> _events_cache = new Dictionary<string, bool>();

	private static readonly Dictionary<string, GUID> _events_guids = new Dictionary<string, GUID>();

	private static GameObject _sound_object;

	private int _tiles_sand;

	private int _tiles_shallow_water;

	public MusicState music_state;

	private MusicBoxLibrary _lib;

	public MusicBoxIdle idle;

	private GameObject _camera_listener;

	private bool _created;

	private static FMOD.Studio.System _studio_system => RuntimeManager.StudioSystem;

	private static bool fmod_disabled
	{
		get
		{
			if (!music_on)
			{
				return !sounds_on;
			}
			return false;
		}
	}

	private void Awake()
	{
		create();
	}

	internal void create()
	{
		if (_created)
		{
			return;
		}
		_created = true;
		inst = this;
		debug_box = new MusicBoxDebug();
		_lib = AssetManager.music_box;
		idle = new MusicBoxIdle();
		ScrollWindow.addCallbackHide(hideWindowCallback);
		if (!fmod_disabled)
		{
			try
			{
				_bus_master = RuntimeManager.GetBus("bus:/");
				if (_bus_master.isValid())
				{
					_bus_master.setVolume(0f);
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError("MusicBox failed to init: " + ex);
				music_on = false;
				sounds_on = false;
				return;
			}
			Platform tPlatform = Settings.Instance.FindCurrentPlatform();
			if (debug_sounds)
			{
				Platform.PropertyAccessors.LiveUpdate.Set(tPlatform, TriStateBool.Enabled);
				Platform.PropertyAccessors.Overlay.Set(tPlatform, TriStateBool.Development);
			}
			else
			{
				Platform.PropertyAccessors.LiveUpdate.Set(tPlatform, TriStateBool.Disabled);
				Platform.PropertyAccessors.Overlay.Set(tPlatform, TriStateBool.Disabled);
			}
			createMusicEvent();
			assignCallback();
			startMusic();
		}
		reserveFlag(MusicBoxLibrary.Neutral_001.id);
		clearParams();
		_sound_object = new GameObject("musicbox_pan");
		_camera_listener = new GameObject("fmod_listener");
		_camera_listener.transform.parent = Camera.main.transform;
		_camera_listener.AddComponent<StudioListener>();
	}

	private void setMusicState(MusicState pState)
	{
		music_state = pState;
		if (pState == MusicState.Menu)
		{
			reserveFlag("Menu");
		}
	}

	private void checkDrawingSounds()
	{
		if (!sounds_on)
		{
			return;
		}
		bool tStop = false;
		if (InputHelpers.mouseSupported)
		{
			if (!Input.GetMouseButton(0))
			{
				tStop = true;
			}
			else if (!ControllableUnit.isControllingUnit() && World.world.isOverUI())
			{
				tStop = true;
			}
		}
		else if (Input.touchCount == 0)
		{
			tStop = true;
		}
		if (tStop)
		{
			inst.stopDrawingSounds();
		}
	}

	private void checkIdleVolume()
	{
		if (World.world.isPaused())
		{
			_volume_idle -= Time.deltaTime;
			if (_volume_idle < 0f)
			{
				_volume_idle = 0f;
			}
		}
		else
		{
			_volume_idle += Time.deltaTime;
			if (_volume_idle > 1f)
			{
				_volume_idle = 1f;
			}
		}
		if (!_bus_idle.isValid())
		{
			_bus_idle = RuntimeManager.GetBus("bus:/Idle");
		}
		checkBusVolume(_volume_idle, _bus_idle);
	}

	private void checkVolumes()
	{
		bool tValid = _vca_sound_effects.isValid();
		if (!tValid)
		{
			_vca_sound_effects = RuntimeManager.GetVCA("vca:/Sound Effects");
			_vca_music = RuntimeManager.GetVCA("vca:/Music");
			_vca_ui = RuntimeManager.GetVCA("vca:/UI");
			_bus_master = RuntimeManager.GetBus("bus:/");
			if (!tValid)
			{
				return;
			}
		}
		checkBusVolume("volume_master_sound", _bus_master);
		checkVcaVolume("volume_sound_effects", _vca_sound_effects);
		checkVcaVolume("volume_music", _vca_music);
		checkVcaVolume("volume_ui", _vca_ui);
	}

	private void checkBusVolume(float pVolume, Bus pBus)
	{
		pBus.getVolume(out var tCurrentVolume);
		if (tCurrentVolume != pVolume)
		{
			pBus.setVolume(pVolume);
		}
	}

	private void checkBusVolume(string pOptionParam, Bus pBus)
	{
		float tVolumeFromOptions = (float)PlayerConfig.getIntValue(pOptionParam) / 100f;
		pBus.getVolume(out var tCurrentVolume);
		if (tCurrentVolume != tVolumeFromOptions)
		{
			pBus.setVolume(tVolumeFromOptions);
		}
	}

	private void checkVcaVolume(string pOptionParam, VCA pVCA)
	{
		float tVolumeFromOptions = (float)PlayerConfig.getIntValue(pOptionParam) / 100f;
		pVCA.getVolume(out var tCurrentVolume);
		if (tCurrentVolume != tVolumeFromOptions)
		{
			pVCA.setVolume(tVolumeFromOptions);
		}
	}

	public void update(float pElapsed)
	{
		if (fmod_disabled)
		{
			return;
		}
		Bench.bench("music_box", "music_box_total");
		Bench.bench("check_volume", "music_box");
		checkVolumes();
		checkIdleVolume();
		Bench.benchEnd("check_volume", "music_box", pSaveCounter: false, 0L);
		Bench.bench("update_idle", "music_box");
		idle.update(pElapsed);
		Bench.benchEnd("update_idle", "music_box", pSaveCounter: false, 0L);
		Bench.bench("update_debug", "music_box");
		debug_box.update();
		Bench.benchEnd("update_debug", "music_box", pSaveCounter: false, 0L);
		Bench.bench("update_drawing", "music_box");
		checkDrawingSounds();
		Bench.benchEnd("update_drawing", "music_box", pSaveCounter: false, 0L);
		Bench.bench("update_fmod_params", "music_box");
		Vector3 tListenerPos = new Vector3(0f, 0f, World.world.camera.orthographicSize * 1.5f);
		_camera_listener.transform.localPosition = tListenerPos;
		updateMainFmodParams();
		Bench.benchEnd("update_fmod_params", "music_box", pSaveCounter: false, 0L);
		if (_timer > 0f)
		{
			_timer -= pElapsed;
			return;
		}
		_timer = 1f;
		Bench.bench("clearParams", "music_box");
		clearParams();
		Bench.benchEnd("clearParams", "music_box", pSaveCounter: false, 0L);
		Bench.bench("drawFmodDebugZones", "music_box");
		drawFmodDebugZones();
		Bench.benchEnd("drawFmodDebugZones", "music_box", pSaveCounter: false, 0L);
		Bench.bench("countZonesUnits", "music_box");
		countUnitsInZones();
		Bench.benchEnd("countZonesUnits", "music_box", pSaveCounter: false, 0L);
		Bench.bench("countSpecialTiles", "music_box");
		countSpecialTilesInChunks();
		Bench.benchEnd("countSpecialTiles", "music_box", pSaveCounter: false, 0L);
		Bench.bench("checkUnitsParams", "music_box");
		checkUnitsParams();
		Bench.benchEnd("checkUnitsParams", "music_box", pSaveCounter: false, 0L);
		Bench.bench("checkCamera", "music_box");
		checkCamera();
		Bench.benchEnd("checkCamera", "music_box", pSaveCounter: false, 0L);
		Bench.bench("music_params_1", "music_box");
		foreach (MusicBoxContainerTiles tCont in _lib.c_list_params)
		{
			if (tCont.enabled)
			{
				enableMusicParameter(tCont.asset.id);
			}
			else
			{
				disableMusicParameter(tCont.asset.id);
			}
		}
		Bench.benchEnd("music_params_1", "music_box", pSaveCounter: false, 0L);
		Bench.bench("music_params_2", "music_box");
		foreach (MusicBoxContainerUnits tCont2 in _lib.c_dict_units.Values)
		{
			if (tCont2.enabled)
			{
				enableMusicParameter(tCont2.asset.id);
			}
			else
			{
				disableMusicParameter(tCont2.asset.id);
			}
		}
		Bench.benchEnd("music_params_2", "music_box", pSaveCounter: false, 0L);
		Bench.bench("flags", "music_box");
		if (_flags_to_enable.Any())
		{
			foreach (string tFlag in _flags_to_enable)
			{
				enableMusicParameter(tFlag);
			}
			_flags_to_enable.Clear();
		}
		Bench.benchEnd("flags", "music_box", pSaveCounter: false, 0L);
		Bench.bench("check_environment", "music_box");
		foreach (MusicBoxContainerTiles tContainer in _lib.c_list_environments)
		{
			checkEnvironmentSound(tContainer);
		}
		Bench.benchEnd("check_environment", "music_box", pSaveCounter: false, 0L);
		Bench.benchEnd("music_box", "music_box_total", pSaveCounter: false, 0L);
	}

	private void updateMainFmodParams()
	{
		if (World.world.quality_changer.isLowRes())
		{
			_studio_system.setParameterByName("MiniMap", 1f);
		}
		else
		{
			_studio_system.setParameterByName("MiniMap", 0f);
		}
		float tZoomLow = World.world.quality_changer.getZoomRatioLow();
		float tZoomHigh = World.world.quality_changer.getZoomRatioHigh();
		float tZoomFull = World.world.quality_changer.getZoomRatioFull();
		_studio_system.setParameterByName("Zoom_Low", tZoomLow);
		_studio_system.setParameterByName("Zoom_High", tZoomHigh);
		_studio_system.setParameterByName("Zoom_Full", tZoomFull);
	}

	public static void clearAllSounds()
	{
		if (!fmod_disabled)
		{
			inst.idle.clearAllSounds();
			inst.debug_box.clear();
		}
	}

	public void clearParams()
	{
		foreach (Kingdom tKingdom in World.world.kingdoms)
		{
			if (_lib.c_dict_civs.TryGetValue(tKingdom.getSpecies(), out var tContainer))
			{
				tContainer.kingdom_exists = true;
			}
		}
		_tiles_sand = 0;
		_tiles_shallow_water = 0;
		foreach (MusicBoxContainerCivs value in _lib.c_dict_civs.Values)
		{
			value.clear();
		}
		foreach (MusicAsset item in _lib.list)
		{
			item.container_tiles?.clear();
		}
		foreach (MusicBoxContainerUnits value2 in _lib.c_dict_units.Values)
		{
			value2.clear();
		}
		DebugLayer.fmod_zones_to_draw.Clear();
	}

	private void hideWindowCallback(string pWindowID)
	{
	}

	private void assignCallback()
	{
		_music_callback = beatEventCallback;
		_timeline_info = new TimelineInfo();
		_timeline_handle = GCHandle.Alloc(_timeline_info, GCHandleType.Pinned);
		_music_event.setUserData(GCHandle.ToIntPtr(_timeline_handle));
		_music_event.setCallback(_music_callback, EVENT_CALLBACK_TYPE.TIMELINE_MARKER | EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
	}

	public static EventInstance getNewInstance(string pID)
	{
		return RuntimeManager.CreateInstance(pID);
	}

	public static EventInstance attachToObject(string pID, GameObject pObject, bool pPlay = true)
	{
		if (!sounds_on)
		{
			return default(EventInstance);
		}
		EventInstance tInstance = getNewInstance(pID);
		RuntimeManager.AttachInstanceToGameObject(tInstance, pObject.transform);
		if (pPlay)
		{
			tInstance.start();
		}
		return tInstance;
	}

	private void createMusicEvent()
	{
		_music_event = getNewInstance("event:/MUSIC/ConsolidatedMusicEvent");
	}

	private void startMusic()
	{
		if (music_on)
		{
			_music_event.start();
		}
	}

	[MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
	private static RESULT beatEventCallback(EVENT_CALLBACK_TYPE pType, IntPtr pInstancePtr, IntPtr pParameterPtr)
	{
		RESULT tResult = inst._music_event.getUserData(out var tTimelineInfoPtr);
		if (tResult != RESULT.OK)
		{
			UnityEngine.Debug.LogError("Timeline Callback error: " + tResult);
		}
		else if (tTimelineInfoPtr != IntPtr.Zero)
		{
			TimelineInfo timelineInfo = (TimelineInfo)GCHandle.FromIntPtr(tTimelineInfoPtr).Target;
			if (pType == EVENT_CALLBACK_TYPE.TIMELINE_MARKER)
			{
				timelineInfo.lastMarker = ((TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(pParameterPtr, typeof(TIMELINE_MARKER_PROPERTIES))).name;
				inst.markerReached(timelineInfo.lastMarker);
			}
		}
		return RESULT.OK;
	}

	private void loadBanks()
	{
	}

	private void checkEnvironmentSound(MusicBoxContainerTiles pContainer)
	{
		MusicAsset tAsset = pContainer.asset;
		bool tPlay = true;
		if (tAsset.mini_map_only)
		{
			if (!World.world.quality_changer.isLowRes())
			{
				tPlay = false;
			}
		}
		else if (World.world.quality_changer.isLowRes())
		{
			tPlay = false;
		}
		else if (tAsset.min_zoom <= World.world.camera.orthographicSize)
		{
			tPlay = false;
		}
		if (tPlay && tAsset.min_tiles_to_play != 0 && pContainer.amount < tAsset.min_tiles_to_play)
		{
			tPlay = false;
		}
		pContainer.enabled = tPlay;
		if (tPlay)
		{
			playEnvironmentSound(pContainer);
		}
		else
		{
			stopEnvironmentSound(pContainer);
		}
	}

	public static void playIdleSoundVisibleOnly(string pSoundPath, WorldTile pTile)
	{
		if (sounds_on)
		{
			playSoundVisibleOnly(pSoundPath, pTile);
		}
	}

	public static void playSoundVisibleOnly(string pSoundPath, WorldTile pTile)
	{
		if (sounds_on)
		{
			playSound(pSoundPath, pTile, pGameViewOnly: true, pVisibleOnly: true);
		}
	}

	public static void playSound(string pSoundPath, WorldTile pTile, bool pGameViewOnly = false, bool pVisibleOnly = false)
	{
		if (!string.IsNullOrEmpty(pSoundPath) && (!pVisibleOnly || pTile.zone.visible))
		{
			playSound(pSoundPath, pTile.pos.x, pTile.pos.y, pGameViewOnly);
		}
	}

	public static void playSoundWorld(string pSoundPath)
	{
	}

	public static void playSoundUI(string pSoundPath)
	{
		playSound(pSoundPath);
	}

	public static EventInstance PlayOneShot(GUID pGuid, Vector3 pPosition, bool pSet3D = true)
	{
		EventInstance tEventInstance = RuntimeManager.CreateInstance(pGuid);
		if (pSet3D)
		{
			tEventInstance.set3DAttributes(pPosition.To3DAttributes());
		}
		else
		{
			Vector3 tCamCenter = World.world.move_camera.transform.position;
			float tZoomDepth = World.world.move_camera.main_camera.orthographicSize;
			Vector3 tPosition = new Vector3(tCamCenter.x, tCamCenter.y, tZoomDepth);
			tEventInstance.set3DAttributes(tPosition.To3DAttributes());
		}
		tEventInstance.start();
		tEventInstance.release();
		return tEventInstance;
	}

	private static bool isEventExists(string pEventPath)
	{
		if (!_events_cache.TryGetValue(pEventPath, out var tExists))
		{
			tExists = RuntimeManager.StudioSystem.getEvent(pEventPath, out var _) == RESULT.OK;
			_events_cache.Add(pEventPath, tExists);
			if (!tExists)
			{
				UnityEngine.Debug.LogWarning("[FMOD] Missing event : " + pEventPath);
			}
			else
			{
				_events_guids[pEventPath] = RuntimeManager.PathToGUID(pEventPath);
			}
		}
		return tExists;
	}

	public static void playSound(string pSoundPath, float pX = -1f, float pY = -1f, bool pGameViewOnly = false, bool pVisibleOnly = false)
	{
		if (sounds_on && (!pGameViewOnly || !World.world.quality_changer.isLowRes()) && isEventExists(pSoundPath))
		{
			GUID tSoundGUID = _events_guids[pSoundPath];
			EventInstance? tState = null;
			try
			{
				tState = ((pX == -1f || pY == -1f) ? new EventInstance?(PlayOneShot(tSoundGUID, Vector3.zero, pSet3D: false)) : new EventInstance?(PlayOneShot(tSoundGUID, new Vector3(pX, pY, 0f))));
			}
			catch (EventNotFoundException)
			{
			}
			if (DebugConfig.isOn(DebugOption.OverlaySounds) || DebugConfig.isOn(DebugOption.OverlaySoundsActive))
			{
				inst.debug_box.add(pSoundPath.Split('/').Last(), pX, pY, tState.Value);
			}
		}
	}

	public void playEnvironmentSound(MusicBoxContainerTiles pContainer)
	{
		if (sounds_on)
		{
			MusicAsset tAsset = pContainer.asset;
			EventInstance tInstance;
			if (_environment_sounds.ContainsKey(tAsset.fmod_path))
			{
				tInstance = _environment_sounds[tAsset.fmod_path];
			}
			else
			{
				tInstance = getNewInstance(tAsset.fmod_path);
				_environment_sounds.Add(tAsset.fmod_path, tInstance);
			}
			setPan(tInstance, pContainer.cur_pan.x, pContainer.cur_pan.y);
			if (!isPlaying(tInstance))
			{
				tInstance.start();
			}
		}
	}

	public void stopEnvironmentSound(MusicBoxContainerTiles pContainer)
	{
		MusicAsset tAsset = pContainer.asset;
		if (_environment_sounds.ContainsKey(tAsset.fmod_path))
		{
			EventInstance tInstance = _environment_sounds[tAsset.fmod_path];
			if (isPlaying(tInstance))
			{
				tInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
		}
	}

	public void playDrawingSound(string pSoundPath, float pX = -1f, float pY = -1f)
	{
		if (sounds_on)
		{
			EventInstance tInstance;
			if (_drawing_sounds.ContainsKey(pSoundPath))
			{
				tInstance = _drawing_sounds[pSoundPath];
			}
			else
			{
				tInstance = getNewInstance(pSoundPath);
				_drawing_sounds.Add(pSoundPath, tInstance);
			}
			setPan(tInstance, pX, pY);
			tInstance.setParameterByName("cursor_speed", MapBox.cursor_speed.fmod_speed);
			if (!isPlaying(tInstance))
			{
				tInstance.start();
			}
		}
	}

	public static void setPan(EventInstance pInstance, float pX, float pY)
	{
		if (pX != -1f || pY != -1f)
		{
			float tZ = 0f;
			_sound_object.transform.position = new Vector3(pX, pY, tZ);
			ATTRIBUTES_3D attributes3D = _sound_object.To3DAttributes();
			pInstance.set3DAttributes(attributes3D);
		}
	}

	public void stopDrawingSound(string pID)
	{
		if (_drawing_sounds.ContainsKey(pID))
		{
			EventInstance tInstance = _drawing_sounds[pID];
			if (isPlaying(tInstance))
			{
				tInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
		}
	}

	public void stopDrawingSounds()
	{
		foreach (EventInstance tInstance in _drawing_sounds.Values)
		{
			if (isPlaying(tInstance))
			{
				tInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
		}
	}

	public static bool isPlaying(EventInstance pInstance)
	{
		pInstance.getPlaybackState(out var state);
		return state != PLAYBACK_STATE.STOPPED;
	}

	private void drawFmodDebugZones()
	{
	}

	private void countUnitsInZones()
	{
		foreach (MapChunk tChunk in World.world.zone_camera.getVisibleChunks())
		{
			if (!tChunk.objects.isEmpty())
			{
				countUnits(tChunk);
			}
		}
	}

	private void checkCamera()
	{
		if ((_tiles_sand < 50 || _tiles_shallow_water < 50) && _tiles_sand >= 100 && _tiles_shallow_water < 20)
		{
			MusicBoxLibrary.Locations_Desert.container_tiles.amount = _tiles_sand + _tiles_shallow_water;
		}
		_lib.c_list_params.Sort(sorter);
		float tTotalTiles = 0f;
		for (int i = 0; i < _lib.c_list_params.Count; i++)
		{
			MusicBoxContainerTiles tCont = _lib.c_list_params[i];
			tCont.enabled = false;
			tTotalTiles += (float)tCont.amount;
		}
		float tPercentUsed = 0f;
		int tMax = 0;
		for (int j = 0; j < _lib.c_list_params.Count; j++)
		{
			MusicBoxContainerTiles tCont2 = _lib.c_list_params[j];
			tCont2.calculatePan();
			tCont2.percent = (float)tCont2.amount / tTotalTiles;
			tPercentUsed += tCont2.percent;
			if (tCont2.amount > 50)
			{
				if (tMax >= 2)
				{
					break;
				}
				tCont2.enabled = true;
				tMax++;
			}
		}
	}

	private void checkUnitsParams()
	{
		MusicBoxContainerUnits tContPriorityMedium = null;
		MusicBoxContainerUnits tContPriorityHigh = null;
		foreach (MusicBoxContainerUnits tCont in _lib.c_dict_units.Values)
		{
			tCont.asset.special_delegate_units?.Invoke(tCont);
			if (tCont.units > 0)
			{
				if (tCont.asset.priority == MusicLayerPriority.High)
				{
					tContPriorityHigh = tCont;
				}
				else if (tCont.asset.priority == MusicLayerPriority.Medium)
				{
					tContPriorityMedium = tCont;
				}
			}
		}
		if (tContPriorityHigh != null)
		{
			tContPriorityMedium = null;
		}
		if (tContPriorityHigh != null || tContPriorityMedium != null)
		{
			foreach (MusicBoxContainerUnits tCont2 in _lib.c_dict_units.Values)
			{
				if ((tContPriorityHigh == null || tCont2 != tContPriorityHigh) && (tContPriorityMedium == null || tCont2 != tContPriorityMedium))
				{
					tCont2.units = 0;
				}
			}
		}
		foreach (MusicBoxContainerUnits tCont3 in _lib.c_dict_units.Values)
		{
			if (tCont3.units > 0)
			{
				tCont3.enabled = true;
			}
		}
	}

	public static int sorter(MusicBoxContainerTiles pV1, MusicBoxContainerTiles pV2)
	{
		return pV2.amount.CompareTo(pV1.amount);
	}

	private void countSpecialTilesInChunks()
	{
		List<MapChunk> tVisibleChunks = World.world.zone_camera.getVisibleChunks();
		int i = 0;
		for (int tLen = tVisibleChunks.Count; i < tLen; i++)
		{
			MapChunk tChunk = tVisibleChunks[i];
			countSpecialTilesForZone(tChunk);
		}
	}

	private void countSpecialTilesForZone(MapChunk pChunk)
	{
		List<MusicBoxTileData> tTileTypesCount = pChunk.getSimpleData();
		TileTypeBase[] tTileTypes = TileLibrary.array_tiles;
		int i = 0;
		for (int tLen = tTileTypesCount.Count; i < tLen; i++)
		{
			MusicBoxTileData tData = tTileTypesCount[i];
			TileTypeBase tType = tTileTypes[tData.tile_type_id];
			int tAmount = tData.amount;
			if (tAmount == 0)
			{
				continue;
			}
			List<MusicAsset> tListMusicAssets = tType.music_assets;
			if (tListMusicAssets != null)
			{
				int j = 0;
				for (int tLenJ = tListMusicAssets.Count; j < tLenJ; j++)
				{
					tListMusicAssets[j].container_tiles.count(tAmount, pChunk.world_center_x, pChunk.world_center_y);
				}
			}
		}
	}

	private void countUnits(MapChunk pChunk)
	{
		foreach (long tKingdomID in pChunk.objects.kingdoms)
		{
			Kingdom tKingdom = World.world.kingdoms.get(tKingdomID);
			if (tKingdom != null)
			{
				ActorAsset tActorAsset = tKingdom.getActorAsset();
				if (tActorAsset != null && tActorAsset.has_music_theme)
				{
					_lib.c_dict_units[tActorAsset.music_theme].units++;
				}
			}
		}
	}

	private void enableMusicParameter(string pID)
	{
		setMusicParameter(pID, 1f);
	}

	private void disableMusicParameter(string pID)
	{
		setMusicParameter(pID, 0f);
	}

	private void setMusicParameter(string pID, float pValue)
	{
		_music_event.setParameterByName(pID, pValue);
	}

	private void markerReached(string pMarker)
	{
		if (pMarker == "Intro")
		{
			return;
		}
		MusicAsset tAsset = _lib.get(pMarker);
		if (tAsset != null)
		{
			if (tAsset.disable_param_after_start)
			{
				disableMusicParameter(pMarker);
			}
			if (tAsset.action != null)
			{
				tAsset.action();
			}
		}
	}

	public static void reserveFlag(string pID, bool pValue = true)
	{
		if (music_on)
		{
			inst._timer = -1f;
			inst._flags_to_enable.Add(pID);
		}
	}

	public static void debug_fmod(DebugTool pTool)
	{
		if (!fmod_disabled)
		{
			_studio_system.getBankList(out var banks);
			RESULT tResult2 = _studio_system.getEvent("event:/MUSIC/ConsolidatedMusicEvent", out var _);
			int tTimelinePos = -1;
			float tParam_new_world = -1f;
			PLAYBACK_STATE getPlaybackState = PLAYBACK_STATE.STARTING;
			inst._music_event.getParameterByName("new_world", out tParam_new_world);
			inst._music_event.getTimelinePosition(out tTimelinePos);
			inst._music_event.getPlaybackState(out getPlaybackState);
			pTool.setText("Zoom_Low:", World.world.quality_changer.getZoomRatioLow(), 0f, pShowBar: false, 0L);
			pTool.setText("Zoom_High:", World.world.quality_changer.getZoomRatioHigh(), 0f, pShowBar: false, 0L);
			pTool.setText("Zoom_Full:", World.world.quality_changer.getZoomRatioFull(), 0f, pShowBar: false, 0L);
			pTool.setSeparator();
			pTool.setText("idle_sim_objects:", inst.idle.CountCurrentSounds(), 0f, pShowBar: false, 0L);
			pTool.setText("music state:", inst.music_state, 0f, pShowBar: false, 0L);
			pTool.setText("IsInitialized:", RuntimeManager.IsInitialized, 0f, pShowBar: false, 0L);
			pTool.setText("Banks count:", banks.Length, 0f, pShowBar: false, 0L);
			pTool.setText("AnySampleDataLoading:", RuntimeManager.AnySampleDataLoading(), 0f, pShowBar: false, 0L);
			pTool.setText("Bank Master:", RuntimeManager.HasBankLoaded("Master"), 0f, pShowBar: false, 0L);
			pTool.setText("Bank Master.strings:", RuntimeManager.HasBankLoaded("Master.strings"), 0f, pShowBar: false, 0L);
			pTool.setText("MUSIC_EVENT by name:", tResult2.ToString(), 0f, pShowBar: false, 0L);
			pTool.setText("tParam_new_world:", tParam_new_world, 0f, pShowBar: false, 0L);
			pTool.setText("tTimelinePos:", tTimelinePos, 0f, pShowBar: false, 0L);
			pTool.setText("getPlaybackState:", getPlaybackState.ToString(), 0f, pShowBar: false, 0L);
		}
	}

	public void debug_params(DebugTool pTool)
	{
		if (fmod_disabled)
		{
			return;
		}
		float tParamValue = 0f;
		for (int i = 0; i < _lib.list.Count; i++)
		{
			string tParam = _lib.list[i].id;
			inst._music_event.getParameterByName(tParam, out tParamValue);
			if (tParamValue == 1f)
			{
				pTool.setText(tParam + ":", tParamValue, 0f, pShowBar: false, 0L);
			}
		}
	}

	public void debug_world_params(DebugTool pTool)
	{
		if (fmod_disabled)
		{
			return;
		}
		foreach (MusicBoxContainerCivs tCont in _lib.c_dict_civs.Values)
		{
			if (tCont.active)
			{
				pTool.setText(tCont.asset.id, tCont.buildings + " " + tCont.kingdom_exists + " " + tCont.active, 0f, pShowBar: false, 0L);
			}
		}
		foreach (MusicAsset item in _lib.list)
		{
			MusicBoxContainerTiles tCont2 = item.container_tiles;
			if (tCont2 != null && tCont2.enabled)
			{
				pTool.setText(tCont2.asset.id, tCont2.amount + " " + tCont2.enabled + " " + tCont2.percent.ToText() + "%", 0f, pShowBar: false, 0L);
			}
		}
		pTool.setText("", "", 0f, pShowBar: false, 0L);
	}

	public void debug_unit_params(DebugTool pTool)
	{
		if (fmod_disabled || _lib.c_dict_units.Count == 0)
		{
			return;
		}
		foreach (MusicBoxContainerUnits tCont in _lib.c_dict_units.Values)
		{
			if (tCont.units != 0)
			{
				pTool.setText(tCont.asset.id, tCont.units + " " + tCont.enabled, 0f, pShowBar: false, 0L);
			}
		}
		pTool.setText("", "", 0f, pShowBar: false, 0L);
	}
}
