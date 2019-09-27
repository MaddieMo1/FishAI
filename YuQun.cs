using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class YuQun : MonoBehaviour
{
    [Header("数量")]
    public int fishNum;

    [Header("预制体")]
    public GameObject fishPre;

    [Header("范围")]
    public float range;

    [Header("本身Tag")]
    public string thisTag;

    [Header("追逐对象Tag")]
    public string foodTag;

    [Header("敌人Tag")]
    public string enemyTag;

    [Header("逃避浮点")]
    public float escapeDis;

    [Header("逃避权重")]
    public float escapeWeight;

    [Header("捕食浮点")]
    public float predationDis;

    [Header("捕食权重")]
    public float predationWeight;

    [Header("保持创造的权重")]
    public float keepCreaterWeight;

    [Header("寻找朋友")]
    public float findFriendDis;

    [Header("到相同物体的距离")]
    public float toFriendsCenterWeight;

    [Header("保持朋友的Dis值")]
    public float keepFriendDis;

    [Header("保持朋友的 Dis 权重")]
    public float keepFriendDisWeight;

    [Header("保持朋友速度的权重")]
    public float keepFriendSpeedWeight;

    [Header("边缘权重")]
    public float randWeight;

    [Header("最大速度浮点")]
    public float maxSpeed;

    [Header("最大转动速度")]
    public float maxTurnSpeed;

    [Header("最大加速度")]
    public float maxAcceleration;

    [Header("最小速度")]
    public float minCD;

    [Header("最大速度")]
    public float maxCD;

    [Header("鱼群数组")]
    public List<Yu> yuqun;

    public Thread th1;
    public Thread th2;
    void Start()
    {
        //鱼群数组初始化
        yuqun = new List<Yu>();

        //遍历鱼群
        for (int i = 0; i < fishNum; i++)
        {
            //生成预制体 位置随机 旋转随机
            Yu yu = Instantiate(fishPre, transform.position + new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range)), Random.rotation).AddComponent<Yu>();
            
            //设置不受重力影响
            yu.gameObject.AddComponent<Rigidbody>().useGravity = false;

            //设置Tag
            yu.tag = thisTag;

            //设置父类
            yu.transform.SetParent(transform);
            yu.yuqun = this;

            //存入鱼群数组
            yuqun.Add(yu);
        }
    }

}
