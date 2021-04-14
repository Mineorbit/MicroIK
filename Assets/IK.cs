using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK : MonoBehaviour
{
    public Transform ball_target;

    Transform arm1;
    Transform arm2;

    float l1 = 1;
    float l2 = 1;

    //Machine Tolerance
    float eps = 0.0025f;

    void Start()
    {
        arm1 = transform.Find("Arm1");
        arm2 = arm1.Find("Arm2");
    }


    void Update()
    {
        if(ball_target == null)
        {
            Debug.Log("Specify Target");
            return;
        }
        Vector3 targetPoint = GetTargetPoint(ball_target);

        var angles = ComputeAngles(targetPoint);

        arm1.eulerAngles = new Vector3(angles.q1,angles.b,0);
        arm2.localEulerAngles = new Vector3(angles.q2,0,0);


    }

    //Input Target Point in local RobotSpace
    //Output angles for 3-DoF arm
    (float b,float q1,float q2) ComputeAngles(Vector3 targetPoint)
    {

        float distance = targetPoint.magnitude;
        float baseAngle = 90 - (180 / Mathf.PI) * Mathf.Atan2(targetPoint.z, targetPoint.x);


        float tagAngle = (180 / Mathf.PI) * Mathf.Acos(targetPoint.y / distance);
        float beta = 180 - (180 / Mathf.PI) * Mathf.Acos(Mathf.Clamp((distance * distance - l1 * l1 - l2 * l2) / (-2 * l1 * l2), -1, 1));
        float alpha = -(180 / Mathf.PI) * Mathf.Acos(Mathf.Clamp((l2 * l2 - distance * distance - l1 * l1) / (-2 * distance * l1), -1, 1));
        return (baseAngle, tagAngle+alpha, beta);
    }

    //Input Target Transform in world Space
    //Output Best Target in local Space
    Vector3 GetTargetPoint(Transform target)
    {
        Vector3 difference = (target.position - transform.position);
        Vector3 diffNormalized = difference;
        diffNormalized.Normalize();
        return (difference.magnitude + eps > l1 + l2) ? diffNormalized * (l1 + l2) : difference;
    }
}
