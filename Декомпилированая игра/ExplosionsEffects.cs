using System.Collections.Generic;
using UnityEngine;

public class ExplosionsEffects : MapLayer
{
	private Dictionary<WorldTile, TileTypeBase> explosionDict;

	private Dictionary<WorldTile, TileTypeBase> explosionDictCurrent;

	public List<WorldTile> explosionQueue;

	private List<WorldTile> explosionQueueCurrent;

	private float timerExplosionQueue;

	public float interval = 0.01f;

	internal Queue<WorldTile> nextWave = new Queue<WorldTile>();

	internal HashSetWorldTile hashset_bombs = new HashSetWorldTile();

	internal List<WorldTile> delayedBombs = new List<WorldTile>();

	internal List<WorldTile> timedBombs = new List<WorldTile>();

	internal override void create()
	{
		colorValues = new Color(1f, 1f, 1f, 1f);
		colors_amount = 60;
		explosionQueue = new List<WorldTile>();
		explosionQueueCurrent = new List<WorldTile>();
		explosionDict = new Dictionary<WorldTile, TileTypeBase>();
		explosionDictCurrent = new Dictionary<WorldTile, TileTypeBase>();
		hashsetTiles = new HashSetWorldTile();
		base.create();
	}

	internal override void clear()
	{
		explosionQueue.Clear();
		explosionQueueCurrent.Clear();
		explosionDict.Clear();
		explosionDictCurrent.Clear();
		hashsetTiles.Clear();
		timedBombs.Clear();
		delayedBombs.Clear();
		nextWave.Clear();
		hashset_bombs.Clear();
		base.clear();
	}

	internal void activateDelayedBomb(WorldTile pBomb)
	{
		if (!delayedBombs.Contains(pBomb))
		{
			delayedBombs.Add(pBomb);
			pBomb.delayed_bomb_type = pBomb.Type.id;
			pBomb.delayed_timer_bomb = 0.09f;
		}
	}

	internal void addTimedTnt(WorldTile pTile)
	{
		if (!timedBombs.Contains(pTile))
		{
			pTile.delayed_timer_bomb = 5f;
			timedBombs.Add(pTile);
		}
	}

	internal void explodeBomb(WorldTile pBombTile, bool pForce = false)
	{
		if (hashset_bombs.Contains(pBombTile))
		{
			return;
		}
		if (pBombTile.Type.explodable_delayed && !pForce)
		{
			activateDelayedBomb(pBombTile);
			return;
		}
		World.world.startShake();
		nextWave.Enqueue(pBombTile);
		while (nextWave.Count > 0)
		{
			WorldTile tTile = nextWave.Dequeue();
			hashset_bombs.Add(tTile);
			if (tTile.Type.explodable && !tTile.Type.explodable_delayed)
			{
				tTile.explosion_wave = tTile.Type.explode_range;
				tTile.explosion_power = tTile.Type.explode_range;
			}
			if (tTile.explosion_wave <= 0)
			{
				continue;
			}
			for (int i = 0; i < tTile.neighbours.Length; i++)
			{
				WorldTile tNeighbour = tTile.neighbours[i];
				if (tNeighbour.explosion_wave > 0 && hashset_bombs.Contains(tNeighbour))
				{
					if (tNeighbour.explosion_wave < tTile.explosion_wave && tNeighbour.Type.explodable)
					{
					}
				}
				else
				{
					hashset_bombs.Add(tNeighbour);
					tNeighbour.explosion_wave = tTile.explosion_wave - 1;
					tNeighbour.explosion_power = tTile.explosion_power;
					nextWave.Enqueue(tNeighbour);
				}
			}
		}
		if (hashset_bombs.Count < 20)
		{
			MusicBox.playSound("event:/SFX/EXPLOSIONS/ExplosionSmall", pBombTile);
		}
		else
		{
			MusicBox.playSound("event:/SFX/EXPLOSIONS/ExplosionMiddle", pBombTile);
		}
		using ListPool<WorldTile> tTempTiles = new ListPool<WorldTile>(hashset_bombs);
		foreach (ref WorldTile item in tTempTiles)
		{
			WorldTile tTile2 = item;
			MapAction.explodeTile(tTile2, (tTile2.explosion_power - tTile2.explosion_wave) * 10, tTile2.explosion_power * 10, pBombTile, AssetManager.terraform.get("bomb"));
		}
	}

	public void prepareNewExplosion(WorldTile pTile)
	{
		if (!explosionDict.ContainsKey(pTile))
		{
			explosionQueue.Add(pTile);
			explosionDict.Add(pTile, pTile.Type);
		}
	}

	private void updateExplosionQueue()
	{
		if (timerExplosionQueue > 0f)
		{
			timerExplosionQueue -= World.world.elapsed;
			return;
		}
		timerExplosionQueue = 0.1f;
		if (explosionQueue.Count != 0)
		{
			for (int i = 0; i < explosionQueue.Count; i++)
			{
				WorldTile tTile = explosionQueue[i];
				explosionQueueCurrent.Add(tTile);
				explosionDictCurrent.Add(tTile, explosionDict[tTile]);
			}
			explosionQueue.Clear();
			explosionDict.Clear();
			for (int j = 0; j < explosionQueueCurrent.Count; j++)
			{
				WorldTile tTile2 = explosionQueueCurrent[j];
				MapAction.damageWorld(tTile2, explosionDictCurrent[tTile2].explode_range, AssetManager.terraform.get("bomb"));
				MusicBox.playSound("event:/SFX/EXPLOSIONS/ExplosionMiddle", tTile2);
			}
			explosionQueueCurrent.Clear();
			explosionDictCurrent.Clear();
		}
	}

	public override void update(float pElapsed)
	{
		checkAutoDisable();
		if (timedBombs.Count <= 0 || World.world.isPaused())
		{
			return;
		}
		int i = 0;
		while (i < timedBombs.Count)
		{
			WorldTile tTile = timedBombs[i];
			if (tTile.delayed_timer_bomb > 0f)
			{
				tTile.delayed_timer_bomb -= pElapsed;
				i++;
				continue;
			}
			timedBombs.RemoveAt(i);
			if (tTile.Type.explodable_timed)
			{
				MapAction.damageWorld(tTile, tTile.Type.explode_range, AssetManager.terraform.get("bomb"));
				MusicBox.playSound("event:/SFX/EXPLOSIONS/ExplosionMiddle", tTile);
			}
		}
	}

	public override void draw(float pElapsed)
	{
		if ((bool)sprRnd || delayedBombs.Count > 0)
		{
			UpdateDirty(pElapsed);
		}
	}

	protected override void UpdateDirty(float pElapsed)
	{
		if (delayedBombs.Count > 0)
		{
			int i = 0;
			while (i < delayedBombs.Count)
			{
				WorldTile tBomb = delayedBombs[i];
				tBomb.delayed_timer_bomb -= World.world.elapsed;
				if (tBomb.delayed_timer_bomb <= 0f)
				{
					tBomb.delayed_timer_bomb = -100f;
					delayedBombs.Remove(tBomb);
					TileTypeBase tType = (string.IsNullOrEmpty(tBomb.delayed_bomb_type) ? TopTileLibrary.tnt_timed : AssetManager.top_tiles.get(tBomb.delayed_bomb_type));
					MapAction.damageWorld(tBomb, tType.explode_range, AssetManager.terraform.get("bomb"));
					MusicBox.playSound("event:/SFX/EXPLOSIONS/ExplosionMiddle", tBomb);
				}
				else
				{
					i++;
				}
			}
		}
		if (hashset_bombs.Count > 0)
		{
			foreach (WorldTile hashset_bomb in hashset_bombs)
			{
				hashset_bomb.explosion_wave = 0;
				hashset_bomb.explosion_power = 0;
			}
			hashset_bombs.Clear();
		}
		if (timer > 0f)
		{
			timer -= World.world.elapsed;
			return;
		}
		timer = interval;
		if (hashsetTiles.Count == 0)
		{
			return;
		}
		using ListPool<WorldTile> tilesToRemove = new ListPool<WorldTile>();
		foreach (WorldTile tTile in hashsetTiles)
		{
			if (tTile.explosion_fx_stage > 0)
			{
				if (Randy.randomBool())
				{
					pixels[tTile.data.tile_id] = Toolbox.clear;
				}
				else
				{
					pixels[tTile.data.tile_id] = colors[tTile.explosion_fx_stage - 1];
				}
				tTile.explosion_fx_stage--;
				if (tTile.explosion_fx_stage <= 0)
				{
					tTile.explosion_fx_stage = 0;
					tilesToRemove.Add(tTile);
				}
			}
		}
		if (tilesToRemove.Count > 0)
		{
			for (int j = 0; j < tilesToRemove.Count; j++)
			{
				WorldTile tTile2 = tilesToRemove[j];
				hashsetTiles.Remove(tTile2);
			}
		}
		updatePixels();
	}

	internal void setDirty(WorldTile pTile, float pDist, float pRadius)
	{
		int newVal = (int)(60f * (1f - pDist / pRadius));
		if (newVal != 0 && newVal >= pTile.explosion_fx_stage)
		{
			hashsetTiles.Add(pTile);
			pTile.explosion_fx_stage = newVal;
		}
	}
}
