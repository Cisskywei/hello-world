using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuncLifeCycle {

    public delegate void VoidParam();
    public VoidParam onUpdate;

	// Update is called once per frame
	public void OnUpdate () {
		if(onUpdate!=null)
        {
            onUpdate();
        }
	}

    public void AddUpdate(VoidParam func)
    {
        onUpdate += func;
    }

    public void RemoveUpdate(VoidParam func)
    {
        onUpdate -= func;
    }
}
