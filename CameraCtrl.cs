using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    [Header("Look At 物体")]
    public Transform target;

    [Header("距离")]
    public float dis;

    void Update()
    {
        Vector3 center = Vector3.zero;
        //遍历所有子物体
        foreach (Transform item in target)
        {
            //所有子物体中心点相加
            center += item.position;
        }
        //所有子物体的中心点位置
        center /= target.childCount;
        transform.position = new Vector3(transform.position.x, center.y, transform.position.z);
        transform.LookAt(center);
        //摄像机位置变更
        transform.position = center - transform.forward * dis;
    }
}
