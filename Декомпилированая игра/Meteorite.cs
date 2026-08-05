using UnityEngine;

public class Meteorite : BaseEffect
{
	private SpriteRenderer _shadow_renderer;

	public Vector3 rotationSpeed = new Vector3(0f, 0f, 50f);

	private float _falling_speed = 200f;

	public GameObject mainSprite;

	public GameObject shadowSprite;

	private int _radius;

	private float _shadow_alpha;

	private float _timer_smoke = 0.01f;

	public string terraform_asset = "meteorite";

	private Actor _owner;

	public override void Awake()
	{
		base.Awake();
		_shadow_renderer = shadowSprite.GetComponent<SpriteRenderer>();
	}

	internal override void create()
	{
		base.create();
	}

	public void spawnOn(WorldTile pTile, string pTerraformId, Actor pActor)
	{
		terraform_asset = pTerraformId;
		tile = pTile;
		_radius = 20;
		base.transform.position = new Vector3(pTile.posV3.x, pTile.posV3.y);
		current_position.x = Randy.randomFloat(-200f, 200f);
		current_position.y = Randy.randomFloat(200f, 250f);
		updateMainSpritePos();
		setShadowAlpha(0f);
	}

	private void updateMainSpritePos()
	{
		Vector3 tVec = new Vector3
		{
			x = current_position.x,
			y = current_position.y
		};
		float tDistToGround = current_position.y;
		tVec.z = tDistToGround;
		mainSprite.transform.localPosition = tVec;
	}

	protected void smoothMovement(Vector2 end, float pElapsed)
	{
		if ((current_position - end).sqrMagnitude > float.Epsilon)
		{
			current_position = Vector2.MoveTowards(current_position, end, _falling_speed * pElapsed);
			updateMainSpritePos();
			shadowSprite.transform.localPosition = new Vector2(current_position.x, shadowSprite.transform.localPosition.y);
		}
		else
		{
			explode();
		}
	}

	public override void update(float pElapsed)
	{
		smoothMovement(Vector2.zero, pElapsed);
		_shadow_alpha += World.world.elapsed * 0.2f;
		setShadowAlpha(_shadow_alpha);
		mainSprite.transform.Rotate(rotationSpeed * World.world.elapsed);
		shadowSprite.transform.Rotate(rotationSpeed * World.world.elapsed);
		if (_timer_smoke > 0f)
		{
			_timer_smoke -= World.world.elapsed;
			return;
		}
		EffectsLibrary.spawnAt("fx_fire_smoke", mainSprite.transform.position, 1f);
		_timer_smoke = 0.05f;
	}

	protected void setShadowAlpha(float pVal)
	{
		_shadow_alpha = pVal;
		if (_shadow_alpha < 0f)
		{
			_shadow_alpha = 0f;
		}
		Color tColor = _shadow_renderer.color;
		tColor.a = _shadow_alpha;
		_shadow_renderer.color = tColor;
	}

	private void explode()
	{
		World.world.game_stats.data.meteoritesLaunched++;
		MapAction.damageWorld(tile, _radius, AssetManager.terraform.get(terraform_asset), _owner);
		EffectsLibrary.spawnExplosionWave(tile.posV3, _radius);
		Vector3 tVec = new Vector3(tile.pos.x, tile.pos.y - 2);
		float tScale = Randy.randomFloat(0.8f, 0.9f);
		EffectsLibrary.spawnAt("fx_explosion_meteorite", tVec, tScale);
		addRandomMineral(tile);
		addRandomMineral(tile.zone.getRandomTile());
		addRandomMineral(tile.zone.getRandomTile());
		addRandomMineral(tile.zone.getRandomTile());
		addRandomMineral(tile.zone.getRandomTile());
		controller.killObject(this);
	}

	private void addRandomMineral(WorldTile pTile)
	{
		if (pTile != null)
		{
			World.world.buildings.addBuilding("mineral_adamantine", pTile, pCheckForBuild: true);
		}
	}

	public static void spawnMeteoriteDisaster(WorldTile pTile, Actor pActor = null)
	{
		EffectsLibrary.spawn("fx_meteorite", pTile, "meteorite_disaster", null, 0f, -1f, -1f, pActor);
	}

	public static void spawnMeteorite(WorldTile pTile, Actor pActor = null)
	{
		EffectsLibrary.spawn("fx_meteorite", pTile, "meteorite", null, 0f, -1f, -1f, pActor);
	}
}
