using UnityEngine;

public class CrabLegJoint : MonoBehaviour
{
	[Header("Joints")]
	public Transform Joint0;

	public Transform Joint1;

	public Transform Hand;

	[Header("Target")]
	public Transform Target;

	private float length0;

	private float length1;

	public float targetDistance;

	public bool mirrored;

	internal Crabzilla crabzilla;

	public float angleMax;

	public float angleMin;

	public float defaultAngle;

	private float atan;

	private float jointAngle0;

	private float jointAngle1;

	public float angle0;

	public float angle1;

	public float groundAngleMin = 50f;

	public float groundAngleMax = 140f;

	internal Transform bodyPoint;

	public float actual_z_pos;

	internal void create()
	{
		actual_z_pos = base.transform.localPosition.z;
		length0 = Vector2.Distance(Joint0.position, Joint1.position);
		length1 = Vector2.Distance(Joint1.position, Hand.position);
		_ = mirrored;
		targetDistance = Vector2.Distance(Joint0.position, Target.position);
		Vector2 diff = Target.position - Joint0.position;
		atan = 0f - crabzilla.transform.rotation.eulerAngles.z + Mathf.Atan2(diff.y, diff.x) * 57.29578f;
		float cosAngle0 = (targetDistance * targetDistance + length0 * length0 - length1 * length1) / (2f * targetDistance * length0);
		defaultAngle = Mathf.Acos(cosAngle0) * 57.29578f;
		angleMin = defaultAngle + 20f;
		angleMax = defaultAngle + 20f;
		GameObject tBodyPoint = new GameObject("leg_point_" + base.transform.name);
		tBodyPoint.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, 0f);
		tBodyPoint.transform.parent = crabzilla.mainBody.transform;
		bodyPoint = tBodyPoint.transform;
	}

	public bool isAngleOk(float pMinAngle, float pMaxAngle)
	{
		angleMin = defaultAngle + pMinAngle;
		angleMax = defaultAngle + pMaxAngle;
		bool num = Toolbox.inBounds(angle0, angleMin, angleMax);
		Vector2 dir = Joint1.transform.position - Hand.transform.position;
		bool angle1_ok = Toolbox.inBounds(Mathf.Atan2(dir.y, dir.x) * 57.29578f, groundAngleMin, groundAngleMax);
		return num && angle1_ok;
	}

	internal void LateUpdate()
	{
		Vector3 tBPos = bodyPoint.position;
		tBPos.z = 0f;
		base.transform.position = tBPos;
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, actual_z_pos);
		targetDistance = Vector2.Distance(Joint0.position, Target.position);
		Vector2 diff = Target.position - Joint0.position;
		atan = 0f - crabzilla.transform.rotation.eulerAngles.z + Mathf.Atan2(diff.y, diff.x) * 57.29578f;
		if (length0 + length1 < targetDistance)
		{
			jointAngle0 = atan;
			jointAngle1 = 0f;
		}
		else
		{
			float cosAngle0 = (targetDistance * targetDistance + length0 * length0 - length1 * length1) / (2f * targetDistance * length0);
			angle0 = Mathf.Acos(cosAngle0) * 57.29578f;
			float cosAngle1 = (length1 * length1 + length0 * length0 - targetDistance * targetDistance) / (2f * length1 * length0);
			angle1 = Mathf.Acos(cosAngle1) * 57.29578f;
			if (mirrored)
			{
				jointAngle0 = atan + angle0;
				jointAngle1 = 180f + angle1;
			}
			else
			{
				jointAngle0 = atan - angle0;
				jointAngle1 = 180f - angle1;
			}
		}
		if (!float.IsNaN(jointAngle0))
		{
			Vector3 Euler0 = Joint0.transform.localEulerAngles;
			Euler0.z = jointAngle0;
			Joint0.transform.localEulerAngles = Euler0;
			Vector3 Euler1 = Joint1.transform.localEulerAngles;
			Euler1.z = jointAngle1;
			Joint1.transform.localEulerAngles = Euler1;
		}
	}
}
