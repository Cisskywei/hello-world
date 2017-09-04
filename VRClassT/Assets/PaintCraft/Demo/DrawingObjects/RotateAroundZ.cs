using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace PaintCraft.Demo
{
	public class RotateAroundZ : MonoBehaviour
	{
		public float speed = 1.0f;


		// Update is called once per frame
		void Update()
		{
			transform.RotateAround(transform.position, Vector3.back, speed * Time.deltaTime);
		}
	}
}