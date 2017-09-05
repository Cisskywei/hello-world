using UnityEngine;
using System.Collections;

public abstract class SpringVr_WriteDataBase : MonoBehaviour {

    public byte shakeFingers = 0x0;
    public byte isShake = 0x0;
    void Start()
    {
        OnStart();
    }
    void Update()
    {
        OnUpdate();
    }
    protected virtual void OnStart() { }
    protected virtual void OnUpdate() { }
}
