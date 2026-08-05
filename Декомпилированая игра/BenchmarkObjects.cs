using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BenchmarkObjects : MonoBehaviour
{
	public static BenchmarkObjects instance;

	public List<Sprite> some_sprites = new List<Sprite>();

	public TestActorGameObject prefab_unity_object;

	private List<TestActorGameObject> actors_unity = new List<TestActorGameObject>();

	internal List<TestActorSimpleObject> actors_simple = new List<TestActorSimpleObject>();

	internal List<TestActorSimpleObject> actors_simple_visible = new List<TestActorSimpleObject>();

	public int total_unity_objects;

	public int total_simple_objects;

	public int total_simple_objects_visible;

	public BenchmarkObjects()
	{
		instance = this;
	}

	private void Update()
	{
		update(Time.deltaTime);
		total_unity_objects = actors_unity.Count;
		total_simple_objects = actors_simple.Count;
		total_simple_objects_visible = actors_simple_visible.Count;
	}

	public void addObjectsSimple(int pAmount = 2000)
	{
		for (int i = 0; i < pAmount; i++)
		{
			TestActorSimpleObject tActor = new TestActorSimpleObject();
			tActor.create(some_sprites);
			actors_simple.Add(tActor);
		}
	}

	public void addObjectsUnity(int pAmount = 2000)
	{
		for (int i = 0; i < pAmount; i++)
		{
			TestActorGameObject tActor = Object.Instantiate(prefab_unity_object);
			tActor.create(some_sprites);
			tActor.transform.parent = base.transform;
			actors_unity.Add(tActor);
		}
	}

	public void killAll()
	{
		foreach (TestActorGameObject item in actors_unity)
		{
			Object.Destroy(item.gameObject, 0.01f);
		}
		actors_unity.Clear();
		actors_simple.Clear();
	}

	public void randomRespawn()
	{
		foreach (TestActorGameObject item in actors_unity)
		{
			item.randomRespawn();
		}
		foreach (TestActorSimpleObject item2 in actors_simple)
		{
			item2.randomRespawn();
		}
	}

	public void update(float pElapsed)
	{
		updateKeys();
		updateUnityActors(pElapsed);
		updateSimpleActors(pElapsed);
		updateVisibility(pElapsed);
	}

	private void updateKeys()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			addObjectsSimple();
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			addObjectsUnity();
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			randomRespawn();
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			killAll();
		}
	}

	private void updateUnityActors(float pElapsed)
	{
		for (int i = 0; i < actors_unity.Count; i++)
		{
			actors_unity[i].update(pElapsed);
		}
	}

	private void updateSimpleActors(float pElapsed)
	{
		Parallel.ForEach(actors_simple, World.world.parallel_options, delegate(TestActorSimpleObject pActor)
		{
			pActor.update(pElapsed);
		});
	}

	private void updateVisibility(float pElapsed)
	{
		actors_simple_visible.Clear();
		float tZoneSize = 8f;
		for (int i = 0; i < actors_simple.Count; i++)
		{
			TestActorSimpleObject tTestActorSimpleObject = actors_simple[i];
			float pos_x = tTestActorSimpleObject.pos_x;
			float tPosY = tTestActorSimpleObject.pos_y;
			int tZoneX = Mathf.FloorToInt(pos_x / tZoneSize);
			int tZoneY = Mathf.FloorToInt(tPosY / tZoneSize);
			TileZone tZone = World.world.zone_calculator.getZone(tZoneX, tZoneY);
			if (tZone != null && tZone.visible)
			{
				actors_simple_visible.Add(tTestActorSimpleObject);
			}
		}
	}
}
