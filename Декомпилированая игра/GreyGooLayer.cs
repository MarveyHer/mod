using System.Collections.Generic;

public class GreyGooLayer : BaseModule
{
	private const float SPREAD_CHANCE = 0.05f;

	private const float REMOVE_CHANCE = 0.09f;

	private const float INTERVAL = 0.08f;

	private List<WorldTile> _to_remove = new List<WorldTile>();

	private List<WorldTile> _to_add = new List<WorldTile>();

	private bool _initiated;

	internal override void create()
	{
		base.create();
		hashset = new HashSet<WorldTile>();
	}

	internal override void clear()
	{
		base.clear();
		hashset.Clear();
		_to_remove.Clear();
		_to_add.Clear();
		_initiated = false;
	}

	public bool isActive()
	{
		return hashset.Count > 0;
	}

	private void init()
	{
		_initiated = true;
		WorldTile[] tiles_list = World.world.tiles_list;
		foreach (WorldTile tTile in tiles_list)
		{
			if (tTile.Type.grey_goo)
			{
				add(tTile);
			}
		}
	}

	public override void update(float pElapsed)
	{
		if (!_initiated)
		{
			init();
		}
		base.update(pElapsed);
		if (isActive() && !World.world.isPaused())
		{
			if (timer > 0f)
			{
				timer -= pElapsed;
				return;
			}
			timer = 0.08f;
			_to_remove.Clear();
			_to_add.Clear();
			updateGooTiles();
			removeFromHashset();
			addToHashset();
		}
	}

	private void updateGooTiles()
	{
		if (hashset.Count == 0)
		{
			return;
		}
		foreach (WorldTile tTile in hashset)
		{
			if (!tTile.Type.grey_goo)
			{
				_to_remove.Add(tTile);
				continue;
			}
			if (tTile.hasBuilding())
			{
				tTile.building.startDestroyBuilding();
			}
			if (Randy.randomChance(0.05f))
			{
				terraform(tTile);
				checkAroundTiles(tTile);
				_to_remove.Add(tTile);
			}
			else if (Randy.randomChance(0.05f))
			{
				checkAroundTiles(tTile);
			}
			else if (Randy.randomChance(0.09f) && areAroundTilesEmpty(tTile))
			{
				_to_remove.Add(tTile);
				if (tTile.Type.grey_goo)
				{
					terraform(tTile);
				}
			}
		}
	}

	private void removeFromHashset()
	{
		if (_to_remove.Count != 0)
		{
			for (int i = 0; i < _to_remove.Count; i++)
			{
				WorldTile tTile = _to_remove[i];
				remove(tTile);
			}
		}
	}

	private void addToHashset()
	{
		if (_to_add.Count != 0)
		{
			for (int i = 0; i < _to_add.Count; i++)
			{
				WorldTile tTile = _to_add[i];
				add(tTile);
			}
		}
	}

	private void checkAroundTiles(WorldTile pTile)
	{
		if (WorldLawLibrary.world_law_gaias_covenant.isEnabled())
		{
			return;
		}
		WorldTile[] neighbours = pTile.neighbours;
		foreach (WorldTile tTile in neighbours)
		{
			TileTypeBase tType = tTile.Type;
			if (!tType.grey_goo && !tType.IsType("pit_deep_ocean") && (!tType.IsType("deep_ocean") || tTile.hasBuilding()))
			{
				_to_add.Add(tTile);
			}
		}
	}

	private bool areAroundTilesEmpty(WorldTile pTile)
	{
		WorldTile[] neighbours = pTile.neighbours;
		foreach (WorldTile obj in neighbours)
		{
			TileTypeBase tType = obj.Type;
			if (obj.hasBuilding())
			{
				return false;
			}
			if (!tType.grey_goo && !tType.considered_empty_tile)
			{
				return false;
			}
		}
		return true;
	}

	private void makeGoo(WorldTile pTile)
	{
		pTile.unfreeze(99);
		MapAction.terraformMain(pTile, TileLibrary.grey_goo);
	}

	private void terraform(WorldTile pTile)
	{
		MapAction.terraformMain(pTile, TileLibrary.pit_deep_ocean, TerraformLibrary.grey_goo);
		MusicBox.playSound("event:/SFX/DESTRUCTION/GreyGooEat", pTile, pGameViewOnly: false, pVisibleOnly: true);
	}

	public void remove(WorldTile pTile)
	{
		hashset.Remove(pTile);
	}

	public void add(WorldTile pTile)
	{
		if (!pTile.Type.considered_empty_tile && hashset.Add(pTile))
		{
			makeGoo(pTile);
		}
	}
}
