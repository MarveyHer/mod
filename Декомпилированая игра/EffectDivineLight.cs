public class EffectDivineLight : BaseAnimatedObject
{
	public SpriteAnimation raySpawn;

	public SpriteAnimation rayIdle;

	public SpriteAnimation baseSpawn;

	public SpriteAnimation baseIdle;

	public bool isOn;

	private DivineLightState state;

	public override void Awake()
	{
		base.Awake();
		setState(DivineLightState.SpawnFirstStage);
	}

	private void setState(DivineLightState pState)
	{
		state = pState;
		switch (state)
		{
		case DivineLightState.SpawnFirstStage:
			raySpawn.gameObject.SetActive(value: true);
			rayIdle.gameObject.SetActive(value: false);
			baseSpawn.gameObject.SetActive(value: false);
			baseIdle.gameObject.SetActive(value: false);
			break;
		case DivineLightState.SpawnSecondStage:
			raySpawn.gameObject.SetActive(value: false);
			rayIdle.gameObject.SetActive(value: true);
			baseSpawn.gameObject.SetActive(value: true);
			baseIdle.gameObject.SetActive(value: false);
			break;
		case DivineLightState.Idle:
			raySpawn.gameObject.SetActive(value: false);
			rayIdle.gameObject.SetActive(value: true);
			baseSpawn.gameObject.SetActive(value: false);
			baseIdle.gameObject.SetActive(value: true);
			break;
		case DivineLightState.Hide:
			raySpawn.gameObject.SetActive(value: true);
			rayIdle.gameObject.SetActive(value: false);
			baseSpawn.gameObject.SetActive(value: true);
			baseIdle.gameObject.SetActive(value: false);
			break;
		}
	}

	private void stopEffet()
	{
	}

	private void useEffect()
	{
	}

	private void Update()
	{
		if (isOn)
		{
			raySpawn.playType = AnimPlayType.Forward;
			baseSpawn.playType = AnimPlayType.Forward;
			if (raySpawn.isLastFrame())
			{
				raySpawn.gameObject.SetActive(value: false);
				rayIdle.gameObject.SetActive(value: true);
			}
			else
			{
				raySpawn.gameObject.SetActive(value: true);
				rayIdle.gameObject.SetActive(value: false);
			}
			if (baseSpawn.isLastFrame())
			{
				baseSpawn.gameObject.SetActive(value: false);
				baseIdle.gameObject.SetActive(value: true);
			}
			else
			{
				baseSpawn.gameObject.SetActive(value: true);
				baseIdle.gameObject.SetActive(value: false);
			}
		}
		else
		{
			raySpawn.playType = AnimPlayType.Backward;
			baseSpawn.playType = AnimPlayType.Backward;
			rayIdle.gameObject.SetActive(value: false);
			baseIdle.gameObject.SetActive(value: false);
			if (raySpawn.isFirstFrame())
			{
				raySpawn.gameObject.SetActive(value: false);
			}
			else
			{
				raySpawn.gameObject.SetActive(value: true);
			}
			if (baseSpawn.isFirstFrame())
			{
				baseSpawn.gameObject.SetActive(value: false);
			}
			else
			{
				baseSpawn.gameObject.SetActive(value: true);
			}
		}
		if (baseSpawn.gameObject.activeSelf)
		{
			baseSpawn.update(World.world.delta_time);
		}
		if (baseIdle.gameObject.activeSelf)
		{
			baseIdle.update(World.world.delta_time);
		}
		if (raySpawn.gameObject.activeSelf)
		{
			raySpawn.update(World.world.delta_time);
		}
		if (rayIdle.gameObject.activeSelf)
		{
			rayIdle.update(World.world.delta_time);
		}
		isOn = false;
	}

	public void playOn(WorldTile pTile)
	{
		base.gameObject.transform.localPosition = pTile.posV3;
		isOn = true;
	}
}
