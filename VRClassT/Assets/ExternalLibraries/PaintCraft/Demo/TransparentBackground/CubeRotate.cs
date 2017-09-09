using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PaintCraft.Demo
{
	public class CubeRotate : MonoBehaviour
	{
		public Vector3 Speed;
		void Update ()
		{
			transform.Rotate(Speed * Time.deltaTime);
		}
	}
}