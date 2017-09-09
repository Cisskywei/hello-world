using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PaintCraft.Utils;
using PaintCraft.Canvas;
using PaintCraft.Canvas.Configs;
using PaintCraft;
using System;
using UnityEngine.Assertions;


namespace PaintCraft.Controllers{

    public class CanvasController : MonoBehaviour
    {		
		public float Width {get; private set;}
		public float Height {get; private set;}
		public Vector2 Size{ 
			get{
				return new Vector2((float) Width, (float) Height);
			}
		}

        public float CamMaxZoomInPercent = 500.0f;

        public PageConfig PageConfig;

        [HideInInspector]
		public Texture2D OutlineTexture;
        [HideInInspector]
		public Texture2D RegionTexture;
		public float OutlineLayerOffset = -25.0f;
       
		[HideInInspector]
        public BackLayerController BackLayerController;
		[HideInInspector]
		public OutlineLayerController OutlineLayerController;

        public CanvasCameraController CanvasCameraController;
		public float BackLayerOffset = 100.0f;

		
		public Material OutlineMaterial;
		public float BrushOffset = 50.0f;
		public int BrushLayerId = 9;
		public int TempRenderLayerId = 10;
		public UndoManager UndoManager;

        public int HistorySize = 10;

        public int PreviewIconWidth = 440;
        public int PreviewIconHeight = 330;

		public Color DefaultBGColor = Color.black;       	

        public Vector2 RenderTextureSize { get; private set; }
        public bool ForceClearOnStart = false;

        public int InputTresholdMargin = 300; //terminate line if input is farther than 100 pixel from the edge

        public Bounds inputBounds;

        public Action<PageConfig> OnPageChange;

        [NonSerialized]
        public bool hasOutline = false;
    
        void Awake(){
            if (DefaultBGColor.a < 1.0f && DefaultBGColor.a > 0.0f)
            {
                Debug.LogWarning("You can use only opaque color (a=1.0f) or totaly transparent (color = (0,0,0,0)). As your alpha is less than 1 i'll change Default BG color to (0,0,0,0)");
                DefaultBGColor = Color.clear;
            }

            if (AppData.SelectedPageConfig != null)
            {
                PageConfig = AppData.SelectedPageConfig;
            }

            if (PageConfig == null){
                Debug.LogError("You have to provide page config for this component or set AppData.SelectedPageConfig", gameObject);
                return;
            }

            SetActivePageConfig(PageConfig);
        }

        public void SetActivePageConfig(PageConfig pageConfig){
            PageConfig = pageConfig;


		    Width = PageConfig.GetSize().x;
		    Height = PageConfig.GetSize().y;
            inputBounds = new Bounds(transform.position, new Vector3(Width + InputTresholdMargin * 2, Height + InputTresholdMargin * 2, 1));


            if (Application.platform == RuntimePlatform.WSAPlayerARM || Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WSAPlayerARM ||
                Application.platform == RuntimePlatform.IPhonePlayer)
		    {
                //Debug.LogFormat("Scren size: {0} {1}", Screen.width, Screen.height);
		        float maxWidth = Mathf.Max(Screen.width, Screen.height);
		        if (maxWidth < (float) Width)
		        {
                    Debug.Log("im on slow mobile. Limit canvas size to the screen size");
		            float maxHeight = maxWidth * (float) Height/(float) Width;
		            RenderTextureSize = new Vector2(maxWidth, maxHeight);
		            HistorySize = 5;
                } else {
                    RenderTextureSize = new Vector2(Width, Height);
                }	       
		    }
		    else
		    {
                RenderTextureSize = new Vector2(Width, Height);
            }

            if (tmpTexture2D == null){
                tmpTexture2D = new Texture2D((int)RenderTextureSize.x, (int)RenderTextureSize.y, TextureFormat.RGB24, false);    
            } else {                
                tmpTexture2D.Resize((int)RenderTextureSize.x, (int)RenderTextureSize.y);
                tmpTexture2D.Apply();
            }


            if (typeof(AdvancedPageConfig).IsAssignableFrom(PageConfig.GetType()))
		    {
                OutlineTexture = (PageConfig as AdvancedPageConfig).OutlineTexture;
                RegionTexture = (PageConfig as AdvancedPageConfig).RegionTexture;               
                hasOutline = OutlineTexture != null;
            } else {
                hasOutline = false;
                OutlineTexture = null;
                RegionTexture = null;
            }


			if (BackLayerController == null){
				BackLayerController = GOUtil.CreateGameObject<BackLayerController>("BackLayer", gameObject, BackLayerOffset);
                BackLayerController   .Init(this);
                BackLayerController.gameObject.layer = 0;
            } else {
                BackLayerController.SetNewSize();
            }

            if (hasOutline){
                if (OutlineLayerController == null){
                    Assert.IsNotNull(OutlineMaterial, "outline material must be set");
                    OutlineLayerController = GOUtil.CreateGameObject<OutlineLayerController>("Outline", gameObject, OutlineLayerOffset);
                    OutlineLayerController.gameObject.layer = 0;
                    OutlineLayerController.Init(this);
                } else {
                    OutlineLayerController.gameObject.SetActive(true);
                    OutlineLayerController.SetNewSize();
                }   
            } else {
                if (OutlineLayerController != null){
                    OutlineLayerController.gameObject.SetActive(false);
                }
            }
			



            if (CanvasCameraController.Initialized == false){
                CanvasCameraController.Init(this);    
            } else {
                CanvasCameraController.SetNewSize();
            }

            if (UndoManager == null){
                UndoManager = new UndoManager(this, HistorySize);
            } else {
                UndoManager.Reinit(RenderTextureSize);
            }
            LoadFromDiskOrClear();
            if (OnPageChange != null){
                OnPageChange.Invoke(pageConfig);
            }
        }
               



        IEnumerator Start(){            
            if (ForceClearOnStart){
                yield return null;
                ClearCanvas();
            }
        }

        public void ClearCanvas(){            
            CanvasCameraController.ClearRenderTexture(()=>SaveChangesToDisk());
		}

		public void Undo(){
			UndoManager.Undo();
		}

		public void Redo(){
			UndoManager.Redo();
		}


        string SaveDirectory
        {
            get
            {
                string dir = Path.Combine( Application.persistentDataPath , "Saves");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                return dir;
            }
        }

        string SaveFilePath
        {
            get
            {
                return Path.Combine( SaveDirectory ,PageConfig.UniqueId + ".jpg");
            }
        }

        Texture2D tmpTexture2D;

        public Texture2D TmpTexture2D
        {
            get
            {                
                return tmpTexture2D;
            }
        }

        Texture2D tmpTextureIcon2D;

        public Texture2D TmpTextureIcon2D
        {
            get
            {
                if (tmpTextureIcon2D == null)
                {
                    tmpTextureIcon2D = new Texture2D(PreviewIconWidth
                        , PreviewIconHeight, TextureFormat.RGB24, false);
                }
                return tmpTextureIcon2D;
            }
        }

        public void SaveChangesToDisk()
        {
            StartCoroutine(DoSaveChangesToDisk());
        }

            

        IEnumerator DoSaveChangesToDisk()
        {
            yield return new WaitForEndOfFrame();
            RenderTexture tmp = RenderTexture.active;
            RenderTexture.active = BackLayerController.RenderTexture;

            TmpTexture2D.ReadPixels(new Rect(0,0,BackLayerController.RenderTexture.width, BackLayerController.RenderTexture.height),0,0,false );
            
            File.WriteAllBytes(SaveFilePath, TmpTexture2D.EncodeToJPG(100));

            RenderTexture downscaledRT = RenderTexture.GetTemporary(440, 330);
            Graphics.Blit(CanvasCameraController.Camera.targetTexture, downscaledRT);
            RenderTexture.active = downscaledRT;

            TmpTextureIcon2D.ReadPixels(new Rect(0, 0, 440, 330), 0, 0, false);
            File.WriteAllBytes(PageConfig.IconSavePath, TmpTextureIcon2D.EncodeToJPG(100));
            RenderTexture.ReleaseTemporary(downscaledRT);
            RenderTexture.active = tmp;
        }

        public bool LoadFromDiskOrClear()
        {
            if (File.Exists(SaveFilePath) && !string.IsNullOrEmpty(PageConfig.name))
            {
                if (TmpTexture2D.LoadImage(File.ReadAllBytes(SaveFilePath)))
                {
                    CanvasCameraController.Camera.targetTexture.DiscardContents();
                    Graphics.Blit(TmpTexture2D, CanvasCameraController.Camera.targetTexture);
                    return true;
                }
            }
            
            ClearCanvas();
            return false;                        
        }

        void Update()
        {
            HandleChangeScreenSize();
        }

        int oldWIdth = -1, oldHeight = -1;

        void HandleChangeScreenSize()
        {
            if (Screen.width != oldWIdth || Screen.height != oldHeight)
            {
                oldWIdth = Screen.width;
                oldHeight = Screen.height;
                LoadFromDiskOrClear();
            }
        }

        public bool isCoordWithinRect(Vector3 position){
            position.z = transform.position.z;
            return inputBounds.Contains(position);

        }

    }




    public enum CanvasSizeType{
		ScreenSize,
		FixedSize,
		OutlineImageSize
	}
}
