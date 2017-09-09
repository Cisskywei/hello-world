using UnityEngine;
using UnityEngine.EventSystems;
using PaintCraft.Canvas;
using PaintCraft.Tools;
using System.Collections.Generic;
using UnityEngine.Serialization;
using PaintCraft;
using UnityEngine.Rendering;
using System.Collections;
using PaintCraft.Canvas.Configs;

namespace PaintCraft.Controllers{    
	[RequireComponent(typeof(Camera))]
    public class ScreenCameraController : InputController {		        
        public LineConfig LineConfig;
        public Camera Camera { get; private set; }
        public bool ZoomOnMouseScroll = false;
        [FormerlySerializedAs("CameraBounds")]
        public CameraSizeHandler CameraSize;

       
        CommandBuffer commandBuffer;
        public CommandBuffer CommandBuffer{
            get{
                return commandBuffer;
            }
        }

     


        void Start(){
          
            if (Canvas == null){
                Debug.LogError("you have to provide link to the Canvas for this component", gameObject);
            }

            Camera = GetComponent<Camera>();
            if (Camera == null){
                Debug.Log("you have to add camera component to this object", gameObject);
            }

            CameraSize.Init(Camera, Canvas);      


            commandBuffer = new CommandBuffer();
            commandBuffer.name = gameObject.name;
            Camera.AddCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
            Canvas.OnPageChange += OnPageChangeHandler;
        }

        void OnDestroy(){
            Canvas.OnPageChange -= OnPageChangeHandler;
        }

        void OnPageChangeHandler(PageConfig newPage){
            CameraSize.ResetSize();
        }

	    bool IsAnyPointerOverGameObject()
	    {
	        bool result = EventSystem.current.IsPointerOverGameObject();
	        foreach (var touch in Input.touches)
	        {
	            result |= EventSystem.current.IsPointerOverGameObject(touch.fingerId);
	        }

	        return result;
	    }
	    
	    void Update()
	    {                        
            HandleZoomOnScroll();
            if (EventSystem.current != null && IsAnyPointerOverGameObject() == false ){
                if (!HandleTouch()){                
                    HandleMouseEvents();
                }    
            }           
            CameraSize.LateUpdate();
        }
             
        void OnEnable(){
            StartCoroutine("ClearCommandBuffer");
        }

        void OnDisable(){
            StopCoroutine("ClearCommandBuffer");
        }

        IEnumerator ClearCommandBuffer(){
            while(true){
                yield return new WaitForEndOfFrame();
                if (CommandBuffer != null)
                {                    
                    CommandBuffer.Clear();
                }
            }
        }



        void HandleZoomOnScroll()
        {
            if (ZoomOnMouseScroll && Camera.pixelRect.Contains(Input.mousePosition))
            {                
                float mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel") * 100.0f;
                if (mouseScrollWheel != 0.0f)
                {
                    CameraSize.SetCameraNewOrthoSize(Camera.orthographicSize - mouseScrollWheel );
                    CameraSize.CheckBounds();
                }
            }
        }

        /// <summary>
        /// Handles the touch.
        /// </summary>
        /// <returns><c>true</c>, if touch was handled, <c>false</c> otherwise.</returns>
        bool HandleTouch(){           
            if (Input.touchCount > 0){                
                foreach (var touch in Input.touches)
                {                    
                    Vector3 worldPosition = GetWorldPosition(touch.position);
                    switch(touch.phase){
                        case TouchPhase.Began:
                            BeginLine(LineConfig, touch.fingerId, worldPosition, true);
                            break;
                        case TouchPhase.Canceled:
                        case TouchPhase.Ended:
                            EndLine(touch.fingerId, worldPosition);
                            break;
                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:
                            ContinueLine(touch.fingerId, worldPosition);
                            break;
                    }
                }

                return true;
            }
            return false;
        }

        void HandleMouseEvents()
        {            
            Vector3 worldPosition = GetWorldPosition(Input.mousePosition);
            if (Input.GetMouseButtonDown(0)){
                BeginLine(LineConfig, 0, worldPosition, true);
            } else if (Input.GetMouseButton(0)){
                ContinueLine(0, worldPosition);
            } else if (Input.GetMouseButtonUp(0)){                
                EndLine(0, worldPosition);
            }
        }
         
        public override bool DontAllowInteraction(Vector2 worldPosition)
        {
            bool result = !Camera.pixelRect.Contains(Camera.WorldToScreenPoint( worldPosition));
            result |= EventSystem.current.IsPointerOverGameObject();            
            return result;
        }

        Vector3 GetWorldPosition(Vector2 screenPosition)
        {
            Vector3 vector3ScreenPosition = screenPosition;
            vector3ScreenPosition.z = transform.position.z;
            return Camera.ScreenToWorldPoint(vector3ScreenPosition);
        }

	}
}
