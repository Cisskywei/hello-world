using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetFlow : MonoBehaviour {

    public Transform player;
    public float maxDis = 5;
    Animator anim;

    private SmoothFollowerObj posFollow;//控制位置平滑移动
    private SmoothFollowerObj lookFollow;//控制朝向平滑转换


    public Vector3 positionVector;//角色位置移动的时候，方向向量
    public Vector3 lookVector;//角色朝向变化的时候，朝向向量

    private Vector3 lastVelocityDir;//上一次移动的方向
    private Vector3 lastPos;//之前移动的目标点位置

    // Use this for initialization
    void Start()
    {
        anim = this.GetComponent<Animator>();//获取动画控制器
        posFollow = new SmoothFollowerObj(0.1f, 0.5f);
        lookFollow = new SmoothFollowerObj(0.1f, 0.0f);
        posFollow.Update(transform.position, 0, true);//初始化负值
        lookFollow.Update(player.transform.position, 0, true);

        positionVector = new Vector3(0, 0, 0.4f);
        lookVector = new Vector3(0, 0, 1.5f);

        lastVelocityDir = player.transform.forward;
        lastPos = player.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        float dis = Vector3.Distance(transform.position, player.position);
        if (dis > maxDis)//如果玩家和宠物之间的距离大于允许的最大距离，控制宠物向玩家移动
        {
            PetMoveFlow();//宠物移动的逻辑
     //       anim.SetBool("Run", true);
        }
        else
        {
    //        anim.SetBool("Run", false);
        }

   //     transform.LookAt(player.position, Vector3.up);

    }

    private void PetMoveFlow()
    {
        lastVelocityDir += (player.transform.position - lastPos) * 5;
        lastPos = player.transform.position;
        lastVelocityDir += player.transform.forward * Time.deltaTime;
        lastVelocityDir = lastVelocityDir.normalized;
        Vector3 horizontal = transform.position - player.transform.position;
        Vector3 horizontal2 = horizontal;
        Vector3 vertical = player.transform.up;
        Vector3.OrthoNormalize(ref vertical, ref horizontal2);
        if (horizontal.sqrMagnitude > horizontal2.sqrMagnitude) horizontal = horizontal2;
        transform.position = posFollow.Update(
            player.transform.position + horizontal * Mathf.Abs(positionVector.z) + vertical * positionVector.y,
            Time.deltaTime
        );

        horizontal = lastVelocityDir;
        Vector3 look = lookFollow.Update(player.transform.position + horizontal * lookVector.z - vertical * lookVector.y, Time.deltaTime); // + horizontal * lookVector.z
        Debug.DrawRay(transform.position, look, Color.red);
        transform.rotation = Quaternion.FromToRotation(transform.up, transform.position - look) * transform.rotation;
        Debug.DrawRay(transform.position, transform.forward, Color.green);
    }

    class SmoothFollowerObj
    {

        private Vector3 targetPosition;
        private Vector3 position;
        private Vector3 velocity;
        private float smoothingTime;
        private float prediction;

        public SmoothFollowerObj(float smoothingTime)
        {
            targetPosition = Vector3.zero;
            position = Vector3.zero;
            velocity = Vector3.zero;
            this.smoothingTime = smoothingTime;
            prediction = 1;
        }

        public SmoothFollowerObj(float smoothingTime, float prediction)
        {
            targetPosition = Vector3.zero;
            position = Vector3.zero;
            velocity = Vector3.zero;
            this.smoothingTime = smoothingTime;
            this.prediction = prediction;
        }

        // 更新位置信息
        public Vector3 Update(Vector3 targetPositionNew, float deltaTime)
        {
            Vector3 targetVelocity = (targetPositionNew - targetPosition) / deltaTime;//获取目标移动的方向向量
            targetPosition = targetPositionNew;

            float d = Mathf.Min(1, deltaTime / smoothingTime);
            velocity = velocity * (1 - d) + (targetPosition + targetVelocity * prediction - position) * d;

            position += velocity * Time.deltaTime;
            return position;
        }

        //根据传递进来的数据，重置本地参数
        public Vector3 Update(Vector3 targetPositionNew, float deltaTime, bool reset)
        {
            if (reset)
            {
                targetPosition = targetPositionNew;
                position = targetPositionNew;
                velocity = Vector3.zero;
                return position;
            }
            return Update(targetPositionNew, deltaTime);
        }

        public Vector3 GetPosition() { return position; }
        public Vector3 GetVelocity() { return velocity; }
    }

}
