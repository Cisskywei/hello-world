using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GrabbableInterface {

    void OnGrabbed(BasicGrabbable grabbedObj);
    void OnRelease(BasicGrabbable releasedObj);
    void OnDrop(BasicGrabbable releasedObj);

}
