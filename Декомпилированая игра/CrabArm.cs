using System.Collections.Generic;
using UnityEngine;

public class CrabArm : MonoBehaviour
{
	internal Crabzilla crabzilla;

	public SpriteRenderer laser;

	public Transform laserPoint;

	public GameObject joint;

	public List<Sprite> laserSprites;

	public bool mirrored;

	private const float LASER_INTERVAL = 0.07f;

	private float _laser_timer = 0.07f;

	private int _laser_frame_index;

	private void Start()
	{
		laser.enabled = false;
	}

	internal void update(float pElapsed)
	{
		Vector3 tArmPos = World.world.camera.WorldToScreenPoint(crabzilla.armTarget.transform.position);
		tArmPos.z = 5.23f;
		Vector3 tJointPos = World.world.camera.WorldToScreenPoint(joint.transform.position);
		tArmPos.x -= tJointPos.x;
		tArmPos.y -= tJointPos.y;
		float tAngle = Mathf.Atan2(tArmPos.y, tArmPos.x) * 57.29578f + 90f;
		if (mirrored)
		{
			tAngle += 180f;
		}
		joint.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, tAngle));
		updateLaser(pElapsed);
		if (crabzilla.isBeamEnabled())
		{
			float tX = laserPoint.transform.position.x;
			float tY = laserPoint.transform.position.y;
			MusicBox.inst.playDrawingSound("event:/SFX/UNIQUE/Crabzilla/CrabzillaLazer", tX, tY);
			World.world.stack_effects.light_blobs.Add(new LightBlobData
			{
				position = new Vector2(laser.transform.position.x, laser.transform.position.y),
				radius = 1.5f
			});
			if (_laser_frame_index > 6 && _laser_frame_index < 10)
			{
				damageWorld();
			}
		}
	}

	private void damageWorld()
	{
		float tX = laserPoint.transform.position.x;
		float tY = laserPoint.transform.position.y;
		WorldTile tTile = World.world.GetTile((int)tX, (int)tY);
		if (tTile != null)
		{
			MapAction.damageWorld(tTile, 4, AssetManager.terraform.get("crab_laser"));
		}
	}

	private void updateLaser(float pTime)
	{
		_laser_timer -= pTime;
		if (crabzilla.isBeamEnabled())
		{
			if (_laser_timer <= 0f)
			{
				_laser_frame_index++;
				if (_laser_frame_index >= 10)
				{
					_laser_frame_index = 6;
				}
			}
		}
		else if (_laser_frame_index != 0)
		{
			_laser_frame_index++;
			if (_laser_frame_index > 13)
			{
				_laser_frame_index = 0;
			}
		}
		if (_laser_timer <= 0f)
		{
			_laser_timer = 0.07f;
		}
		if (laser.sprite.name != laserSprites[_laser_frame_index].name)
		{
			laser.sprite = laserSprites[_laser_frame_index];
		}
		laser.enabled = _laser_frame_index != 0 || crabzilla.isBeamEnabled();
	}
}
