using UnityEngine;

public class LivingIcon : MonoBehaviour
{
	private Vector3 init_position;

	private float speed_back;

	private float speed_away;

	private float return_timer;

	public static int killed_mod = 1;

	private void Awake()
	{
		init_position = base.transform.position;
	}

	public void kill()
	{
		killed_mod++;
		base.enabled = false;
	}

	public void Update()
	{
		Vector3 tMousePos = Input.mousePosition;
		float num = Vector2.Distance(base.transform.position, tMousePos);
		float tDistToFly = 80 + killed_mod * 10;
		if (num < tDistToFly)
		{
			if (speed_away == 0f && killed_mod > 6)
			{
				speed_away = killed_mod * 10;
			}
			speed_away += 200f * Time.deltaTime * (float)killed_mod;
		}
		else if (speed_away > 0f)
		{
			speed_away -= 500f * Time.deltaTime;
			if (speed_away < 0f)
			{
				speed_away = 0f;
			}
		}
		if (speed_away > 0f)
		{
			base.transform.position = Vector2.MoveTowards(base.transform.position, tMousePos, -1f * speed_away * Time.deltaTime);
			return_timer = 1f;
			speed_back = 0f;
			rotate();
		}
		else if (return_timer > 0f)
		{
			return_timer -= Time.deltaTime;
		}
		else if (Vector2.Distance(base.transform.position, init_position) > 1f)
		{
			speed_back += Time.deltaTime * 400f;
			base.transform.position = Vector2.MoveTowards(base.transform.position, init_position, Time.deltaTime * speed_back);
		}
		else
		{
			speed_back = 0f;
		}
		void rotate()
		{
			Vector3 tVec = base.transform.eulerAngles;
			tVec.z += 10f;
			base.transform.eulerAngles = tVec;
		}
	}
}
