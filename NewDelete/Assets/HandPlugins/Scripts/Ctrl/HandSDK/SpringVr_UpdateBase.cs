using UnityEngine;
using System.Collections;

public class SpringVr_UpdateBase : SpringVr_Base {

    private void Update()
    {
        OnUpdate();
    }

    private void FixedUpdate()
    {
        OnFixedUpdate();
    }
    protected virtual void OnUpdate() { }
    protected virtual void OnFixedUpdate() { }
}
