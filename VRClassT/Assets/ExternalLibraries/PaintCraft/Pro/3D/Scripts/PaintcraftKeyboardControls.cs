using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintCraft.Controllers;


namespace PaintCraft.Pro.Controllers{
    public class PaintcraftKeyboardControls : MonoBehaviour {
        public Camera       SelfCamera;


    	// Update is called once per frame
    	void Update () {
            if (Input.GetKeyDown(KeyCode.C)){
                CanvasController canvas = GetCanvas();
                if (canvas != null){
                    canvas.ClearCanvas();
                }
            }	
            if (Input.GetKeyDown(KeyCode.R)){
                CanvasController canvas = GetCanvas();
                if (canvas != null && canvas.UndoManager.HasRedo()){
                    canvas.Redo();
                }
            }
            if (Input.GetKeyDown(KeyCode.U)){
                CanvasController canvas = GetCanvas();
                if (canvas != null && canvas.UndoManager.HasUndo()){
                    canvas.Undo();
                }
            }
    	}


        CanvasController GetCanvas(){

            Ray ray = SelfCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo)){
                PaintcraftCanvas3DPlane paintcraftPlane = hitInfo.collider.gameObject.GetComponent<PaintcraftCanvas3DPlane>();
                if (paintcraftPlane == null){
                    return null;
                }               
                return paintcraftPlane.PaintcraftCanvas;
            }
            return null;
        }

    }
}
