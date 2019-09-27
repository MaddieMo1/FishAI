using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmCtrl : MonoBehaviour
{
    public Rigidbody rb;
    public YuQun yuqun;
    public Animator am;

    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        yuqun = transform.parent.GetComponent<YuQun>();
        am = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        //根据刚体速度矢量处于鱼群速度得到动画播放速度
        am.speed = rb.velocity.magnitude / yuqun.maxSpeed * 5;
    }
}
