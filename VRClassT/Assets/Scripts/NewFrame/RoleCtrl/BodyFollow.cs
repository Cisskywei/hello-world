using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyFollow : MonoBehaviour {

    public Transform target;

    public float speed = 4;

    // Use this for initialization
    void Start () {
        _pos = transform.position;
        _rot = transform.eulerAngles;
    }
	
	// Update is called once per frame
	void Update () {
        PosMoveFlow();
    }

    private Vector3 _angledir;
    private Vector3 _pos;
    private Vector3 _rot;
    private Quaternion _rotadd;
    private void PosMoveFlow()
    {
        _angledir = (target.position - transform.position);
        transform.rotation = Quaternion.FromToRotation(transform.up, _angledir) * transform.rotation;
        _pos.x = target.position.x;
        _pos.y = target.position.y - 1.32f;
        _pos.z = target.position.z;
        transform.position = Vector3.Slerp(transform.position, _pos,Time.deltaTime* speed);

        _rotadd = Quaternion.FromToRotation(transform.forward, target.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation *= _rotadd, Time.deltaTime * speed);
    }

    //class SmoothFollowerObj
    //{
    //    private Vector3 targetPosition;
    //    private Vector3 position;
    //    private Vector3 velocity;
    //    private float smoothingTime;
    //    private float prediction;

    //    public SmoothFollowerObj(float smoothingTime)
    //    {
    //        targetPosition = Vector3.zero;
    //        position = Vector3.zero;
    //        velocity = Vector3.zero;
    //        this.smoothingTime = smoothingTime;
    //        prediction = 1;
    //    }

    //    public SmoothFollowerObj(float smoothingTime, float prediction)
    //    {
    //        targetPosition = Vector3.zero;
    //        position = Vector3.zero;
    //        velocity = Vector3.zero;
    //        this.smoothingTime = smoothingTime;
    //        this.prediction = prediction;
    //    }

    //    // 更新位置信息
    //    public Vector3 Update(Vector3 targetPositionNew, float deltaTime)
    //    {
    //        Vector3 targetVelocity = (targetPositionNew - targetPosition) / deltaTime;//获取目标移动的方向向量
    //        targetPosition = targetPositionNew;

    //        float d = Mathf.Min(1, deltaTime / smoothingTime);
    //        velocity = velocity * (1 - d) + (targetPosition + targetVelocity * prediction - position) * d;

    //        position += velocity * Time.deltaTime;
    //        return position;
    //    }

    //    //根据传递进来的数据，重置本地参数
    //    public Vector3 Update(Vector3 targetPositionNew, float deltaTime, bool reset)
    //    {
    //        if (reset)
    //        {
    //            targetPosition = targetPositionNew;
    //            position = targetPositionNew;
    //            velocity = Vector3.zero;
    //            return position;
    //        }
    //        return Update(targetPositionNew, deltaTime);
    //    }

    //    public Vector3 GetPosition() { return position; }
    //    public Vector3 GetVelocity() { return velocity; }
    //}
}
