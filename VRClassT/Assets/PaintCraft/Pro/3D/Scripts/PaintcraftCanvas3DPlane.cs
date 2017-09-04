using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintCraft.Controllers;
using UnityEngine.Assertions;
using PaintCraft.Canvas.Configs;

public class PaintcraftCanvas3DPlane : MonoBehaviour {
    public CanvasController PaintcraftCanvas;
    public bool SyncAspect;
	// Use this for initialization
	void Start () {
        Assert.IsNotNull(PaintcraftCanvas, "canvas must be set");

        MeshRenderer mr = GetComponent<MeshRenderer>()	;
        mr.material = new Material(mr.material);
        mr.material.mainTexture = PaintcraftCanvas.BackLayerController.RenderTexture;

        var advancedPageConfig = PaintcraftCanvas.PageConfig as AdvancedPageConfig;
        if (advancedPageConfig != null){
            mr.material.SetTexture("_OutlineTex", advancedPageConfig.OutlineTexture);
        }

        if (SyncAspect){
            float aspect = PaintcraftCanvas.Width / PaintcraftCanvas.Height;
            this.transform.localScale = new Vector3(this.transform.localScale.z * aspect, 1, this.transform.localScale.z);
        }
	}

    public Vector2 GetUVUsingPlane(Ray ray){
        Plane plane = new Plane(transform.TransformDirection(Vector3.up), transform.position);
        float enter;

        if(plane.Raycast(ray, out enter)){
            Vector3 globalPoint = ray.GetPoint(enter);
            Debug.Log(globalPoint + " -- globalPoint");
            Vector3 result = transform.InverseTransformPoint(globalPoint);
            result.x += 5f;
            result.z += 5f;
            return new Vector2(1 - result.x /10f, 1 - result.z/10f);
        } else {
            return new Vector2(0f,0f);
        }
    }
	
}
