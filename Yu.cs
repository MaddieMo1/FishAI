using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yu : MonoBehaviour
{
    //不在面板显示 但是序列化
    [HideInInspector]
    public YuQun yuqun;

    private Rigidbody rb;
    [Header("速度")]
    public float cd;
    [Header("捕食周期")]
    public float t;

    //随机速度
    private Vector3 randVelocity;
    //流动速度
    private Vector3 curVelocity;
    void Start()
    {
        //获取本身刚体组件
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Compute();
        //设置刚体速度矢量
        rb.velocity = Vector3.RotateTowards(rb.velocity, curVelocity, yuqun.maxTurnSpeed * Time.deltaTime, yuqun.maxAcceleration * Time.deltaTime);
        //设置本身旋转
        transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    public void Compute()
    {
        //初始化捕食周期
        RandomVelocity();
        //朋友数组
        List<Yu> friends = GetFriends();

        //所有向量相加
        Vector3 v3 = PredationVelocity() + EscapeVelocity() + KeepFriendsVelocity(friends) + KeepCreaterDisVelocity()+ ToCenterVelocity(friends) + KeepFriendsDisVelocity(friends) + randVelocity;

        //鱼群最大平均速度
        v3 /= 7 * yuqun.maxSpeed;

        //对比 并返回最大长度
        v3 = ClampVelocity(v3);

        //流动速度赋值
        curVelocity = v3;
    }

    //随机捕食周期
    void RandomVelocity()
    {
        t -= 0.02f;
        if (t <= 0)
        {
            t = cd;
            cd = Random.Range(yuqun.minCD, yuqun.maxCD);

            //随机边缘权重
            randVelocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * yuqun.randWeight;
        }
    }

    //获得朋友数组
    List<Yu> GetFriends()
    {
        //鱼群数组
        GameObject[] friends = GameObject.FindGameObjectsWithTag(yuqun.thisTag);

        //朋友数组
        List<Yu> friendsList = new List<Yu>();

        //遍历所有相同Tag的预制体
        for (int i = 0; i < friends.Length; i++)
        {
            if (friends[i] == gameObject) continue;
            if (Vector3.Distance(friends[i].transform.position, transform.position) < yuqun.findFriendDis)
            {
                //存储在朋友数组
                friendsList.Add(friends[i].GetComponent<Yu>());
            }
        }
        return friendsList;
    }

    //保持创造速度距离
    Vector3 KeepCreaterDisVelocity()
    {
        //返回鱼群和本身位置的差值并和保持创造的权重 相乘
        return (yuqun.transform.position - transform.position) * 0.1f * yuqun.keepCreaterWeight;
    }

    //保持朋友速度
    Vector3 KeepFriendsVelocity(List<Yu> friends)
    {
        Vector3 friendsVel = Vector3.zero;
        //检测朋友数组是否为空
        if (friends.Count == 0) return friendsVel;

        foreach (var item in friends)
        {
            //所有朋友速度矢量相加
            friendsVel += item.rb.velocity;
        }
        //求出朋友数组平均速度变量
        friendsVel /= friends.Count;

        //返回 朋友数组平均速度变量 和 保持朋友速度的权重 相乘
        return friendsVel.normalized * yuqun.keepFriendSpeedWeight;
    }

    //中心速度
    Vector3 ToCenterVelocity(List<Yu> friends)
    {
        Vector3 center = Vector3.zero;
        //检测朋友数组是否为空
        if (friends.Count == 0) return center;

        foreach (var item in friends)
        {
            //所有朋友速度矢量相加
            center += item.transform.position;
        }
        //求出朋友数组平均速度变量
        center /= friends.Count;

        //返回 朋友数组平均速度变量 和 到相同物体的距离 相乘
        return (center - transform.position).normalized * yuqun.toFriendsCenterWeight;
    }

    //和朋友保持的距离
    Vector3 KeepFriendsDisVelocity(List<Yu> friends)
    {
        float dis = 999;
        Yu yu = null;
        foreach (var item in friends)
        {
            //返回本身的距离和朋友数组 之间的平均值
            float d = Vector3.Distance(transform.position, item.transform.position);

            //如果平均值小于 Dis
            if (d < dis)
            {
                //Dis 等于平均值 并且 item 等于 yu
                dis = d;
                yu = item;
            }
        }
        if (yu != null)
        {
            //中心点位置计算
            return (yu.transform.position - transform.position).normalized * Mathf.Clamp((dis - yuqun.keepFriendDis) / yuqun.keepFriendDis, -1f, 1f) * yuqun.keepFriendDisWeight;
        }
        else
        {
            //否则 位置等于 0
            return Vector3.zero;
        }
    }

    //逃跑速度
    Vector3 EscapeVelocity()
    {
        if (string.IsNullOrEmpty(yuqun.enemyTag)) return Vector3.zero;
        GameObject[] gos = GameObject.FindGameObjectsWithTag(yuqun.enemyTag);
        float dis = 9999;
        GameObject yu = null;
        foreach (var item in gos)
        {
            float d = Vector3.Distance(transform.position, item.transform.position);
            if (d < dis)
            {
                dis = d;
                yu = item;
            }
        }
        if (yu != null)
        {
            float xd = Mathf.Clamp((yuqun.escapeDis - dis) / yuqun.escapeDis, 0f, 1f) * 1.5f;
            return (transform.position - yu.transform.position).normalized* xd * xd * yuqun.escapeWeight;
        }
        else
        {
            return Vector3.zero;
        }
    }


    //捕食速度
    Vector3 PredationVelocity()
    {
        //判断是否为空值
        if (string.IsNullOrEmpty(yuqun.foodTag)) return Vector3.zero;

        //捕食数组
        GameObject[] gos = GameObject.FindGameObjectsWithTag(yuqun.foodTag);

        float dis = 9999;

        GameObject yu = null;

        foreach (var item in gos)
        {
            //捕食速度
            float d = Vector3.Distance(transform.position, item.transform.position);
            if (d < dis)
            {
                dis = d;
                yu = item;
            }
        }
        if (yu != null)
        {
            //捕食加速度
            float xd = Mathf.Clamp((yuqun.predationDis - dis) / yuqun.predationDis, 0f, 1f) * 1.5f;

            //捕食速度计算
            return (yu.transform.position - transform.position).normalized* xd * xd * yuqun.predationWeight;
        }
        else
        {
            return Vector3.zero;
        }
    }

    //固定周期
    Vector3 ClampVelocity(Vector3 velocity)
    {
        //返回最大长度
        return Vector3.ClampMagnitude(velocity, yuqun.maxSpeed);
    }
}
