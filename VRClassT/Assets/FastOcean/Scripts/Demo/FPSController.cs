/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

//////////////////////////////////////////////////////////////
// FollowTransform.cs
//////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

namespace FastOcean
{
    public class FPSController : Controller
	{
	    public Camera targetCamera = null;		// Transform to follow
	    public float speed = 100;		// Speed to follow
       
	    void Awake()
	    {
	        JoyStick.On_JoystickMove += JoystickMove;
	    }

        void OnDestroy()
        {
            JoyStick.On_JoystickMove -= JoystickMove;
        }

        public void JoystickMove(Vector2 move)
	    {
	        if (FOcean.instance == null)
	            return;

	        FOceanGrid grid = FOcean.instance.mainPG;
	        if (grid == null)
	            return;

	        float tmpSpeed = speed;
	        if (FOcean.instance != null && grid != null)
	        {
	            float ratio = grid.offsetToGridPlane / grid.baseParam.minBias.w;
	            if (ratio > 1)
	            {
	                tmpSpeed *= ratio;
	            }
	        }

	        if (move.y > 0)
	        {
	            transform.position += new Vector3(targetCamera.transform.forward.x, 0, targetCamera.transform.forward.z) * tmpSpeed * Time.deltaTime;
	        }
	        else if (move.y < 0)
	        {
	            transform.position -= new Vector3(targetCamera.transform.forward.x, 0, targetCamera.transform.forward.z) * tmpSpeed * Time.deltaTime;
	        }
	        if (move.x > 0)
	        {
	            transform.position += new Vector3(targetCamera.transform.right.x, 0, targetCamera.transform.right.z) * tmpSpeed * Time.deltaTime;
	        }
	        else if (move.x < 0)
	        {
	            transform.position -= new Vector3(targetCamera.transform.right.x, 0, targetCamera.transform.right.z) * tmpSpeed * Time.deltaTime;
	        }
	    }

	    void Update () 
	    {
	        if (FOcean.instance == null)
	            return;

	        FOceanGrid grid = FOcean.instance.mainPG;
	        if (grid == null)
	            return;

	        float tmpSpeed = speed;
	        if (FOcean.instance != null && grid != null)
	        {
	            float ratio = grid.offsetToGridPlane / grid.baseParam.minBias.w;
	            if (ratio > 1)
	            {
	                tmpSpeed *= ratio;
	            }
	        }

	        if (Input.GetKeyUp(KeyCode.R))
	        {
	            FOcean.instance.ForceReload(true);
	        }

	        if (Input.GetAxis("Vertical") > 0)
	        {
	            transform.position += new Vector3(targetCamera.transform.forward.x, 0, targetCamera.transform.forward.z) * tmpSpeed * Time.deltaTime;
	        }
	        else if (Input.GetAxis("Vertical") < 0)
	        {
	            transform.position -= new Vector3(targetCamera.transform.forward.x, 0, targetCamera.transform.forward.z) * tmpSpeed * Time.deltaTime;
	        }
	        if (Input.GetAxis("Horizontal") > 0)
	        {
	            transform.position += new Vector3(targetCamera.transform.right.x, 0, targetCamera.transform.right.z) * tmpSpeed * Time.deltaTime;
	        }
	        else if (Input.GetAxis("Horizontal") < 0)
	        {
	            transform.position -= new Vector3(targetCamera.transform.right.x, 0, targetCamera.transform.right.z) * tmpSpeed * Time.deltaTime;
	        }


	        UpdateGUI(grid);
	    }
	}
}
