using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintCraft.Tools;
using UnityEngine.Assertions;


namespace PaintCraft.Controllers{
    public class DrawingGameObjectController : MonoBehaviour {
        public int LineId; // must be unique acros all ines included touch ids
        public LineConfig LineConfig;
        public ScreenCameraController ScreenCameraController;

        bool _previousStatus = false;

        public bool IsEnabled;



        void Start(){
            Assert.IsNotNull(LineConfig);
            Assert.IsNotNull(ScreenCameraController);
        }

        void Update(){
            if (IsEnabled){
                if (!_previousStatus){
                    ScreenCameraController.BeginLine(LineConfig, LineId, transform.position);
                } else {
                    ScreenCameraController.ContinueLine(LineId, transform.position);
                }
            } else{
                if (_previousStatus){
                    ScreenCameraController.EndLine(LineId, transform.position);
                } else {
                    //do nothing
                }
            }
                
            _previousStatus = IsEnabled;
        }
                   

        public void SetStatus(bool status){
            IsEnabled = status;
        }
    }
}