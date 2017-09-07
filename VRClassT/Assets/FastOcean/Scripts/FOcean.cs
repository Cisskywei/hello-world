/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/


#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
#define MOBILE
#endif

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace FastOcean
{
    public enum eFGWMode
    {
        eGWM_None = 0x0,
        eGWM_Render = 0x1,
        eGWM_Simulation = 0x2,
        eGWM_RenderAndSimulation = 0x3,
    }

    public enum eFTRSpace
    {
        eTRS_Mask = 0,
        eTRS_Screen,
    }

    public enum eFRWQuality
    {
        eRW_High = 0,
        eRW_Medium = 1,
        eRW_Low = 2,
    }

    public enum eFUnderWaterMode
    {
        eUM_Blend,
        eUM_Simple,
        eUM_None,
    }

    public enum eFDepthBlendMode
    {
        eDB_DepthBlend,
        eDB_AlphaBlend,
        eDB_None,
    }

    public enum eFUnderWater
    {
        eUW_Above,
        eUW_AboveIntersect,
        eUW_UnderIntersect,
        eUW_Under,
    }

    public enum eFSunMode
    {
        eSM_Phong,
        eSM_BlinnPhong,
        eSM_None,
    }

	[Serializable]
	public class FEnvParameters
	{
        public Light sunLight;
        [Range(0, 1)]
        public float sunIntensity = 1.0f;

        public eFSunMode sunMode = eFSunMode.eSM_Phong;

        public LayerMask sunOccluMask;

        public eFDepthBlendMode depthBlendMode = eFDepthBlendMode.eDB_DepthBlend;

        public eFUnderWaterMode underWaterMode = eFUnderWaterMode.eUM_Blend;

        public float depthFade = 250f;
        public float surfaceFade = 200f;

        [Range(0f, 0.1f)]
        public float distortMag = 0.01f;
        public float distortFrq = 200f;

        public Color underAmb = new Color(0.72f, 0.74f, 0.835f, 1f);

        public Texture2D underDistortMap = null;
        public Shader underWaterShader = null;

        public Mesh underButtom = null;

        [Range(2, 16)]
        public int skirt = 4;

        public bool foamEnabled = true;
	}

    [Serializable]
    public class FShaderPack
    {   
        public Shader fade = null;
        public Shader blur = null;
        public Shader cull = null;
    }


    /// <summary>
    /// trailMap
    /// </summary>
    /// 
    [Serializable]
    public class FTMParameters
    {
        public eFTRSpace renderSpace = eFTRSpace.eTRS_Mask;
        [Range(256, 1024)]
        public int trailMaskSize = 512;
        [Range(0.01f, 1)]
        public float trailMaskScale = 0.2f;
        public float trailMaskFade = 10.0f;
        public float trailIntensity = 1.5f;
    }

    [Serializable]
    public class FLayerDefinition
    {
        //Definition
        public int transparentlayer = 1;
        public int waterlayer = 4;
        public int uilayer = 5;
        public int traillayer = 6;
        public int terrainlayer = 7;
    }

	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	public class FOcean : MonoBehaviour
    {
        public const string version = "1.0.5";

        //Shader LOD
        public const int LOD = 200;//only for editor
        public const int ABOVELODSM3 = LOD + 4;
        public const int ABOVELOD = LOD + 3;
        public const int UNDERLODSM3 = LOD + 2;
        public const int UNDERLOD = LOD + 1;
	    public const int OCEANLOD = LOD - 1;
	    public const int UNDEROCEANLOD = LOD - 2;
        public const int UNDEROCEANLOD2 = LOD - 3;
        public const int GlARELOD = LOD - 4;
        
	    public const float g = 9.80665f;
        
        public const string projectorCameraName = "FOProjectorCamera";
        public const string trailCameraName = "FOTrailCamera";
        public const string reflCameraName = "FOReflectionCamera";
        public const string oceanCameraName = "FOceanMapCamera";

        //for debug
        [NonSerialized]
        public bool needSM3 = true;

#if UNITY_EDITOR
        [NonSerialized]
        public bool drawButtomGizmos = false;
#endif

        public FEnvParameters envParam = null;

	    private FReflection reflection;

        public Transform trailer = null;

        public FTMParameters tmParam = null;

	    public static FOcean instance = null;

	    private FOceanGrid mainGrid;

	    private HashSet<FOceanGrid> grids = new HashSet<FOceanGrid>();

        public FShaderPack shaderPack;

        public FLayerDefinition layerDef = new FLayerDefinition();
        
        [NonSerialized]
	    public Material matFade = null;
        [NonSerialized]
        public Material matBlur = null;
        [NonSerialized]
        public Material matCull = null;

        private LinkedListNode<RenderTexture> queueNode = null;
        private LinkedList<RenderTexture> queueRTs = new LinkedList<RenderTexture>();

        /// <summary>
        /// oceanMap
        /// </summary>
        private Camera oceanCamera = null;
        private Camera trailCamera = null;

        private RenderTexture trailmap = null;
        private RenderTexture trailmask = null;
        [NonSerialized]
        public RenderTexture oceanmap = null;
        [NonSerialized]
        public RenderTexture glaremap = null;

        [NonSerialized]
        public bool isStarted = false;

        private bool m_bSunGlare = false;

        private eFUnderWaterMode m_underMode = eFUnderWaterMode.eUM_Blend;

        private eFTRSpace m_renderSpace = eFTRSpace.eTRS_Mask;
        private int m_trailMaskSize = 512;
        private Transform m_trailer = null;

        [NonSerialized]
        public RenderTextureFormat rtR8Format = RenderTextureFormat.R8;

        [NonSerialized]
        public HashSet<MonoBehaviour> needDepthBehaviour = new HashSet<MonoBehaviour>();

        [NonSerialized]
        public HashSet<MonoBehaviour> needSunBehaviour = new HashSet<MonoBehaviour>();

        [NonSerialized]
        public HashSet<MonoBehaviour> needGlareBehaviour = new HashSet<MonoBehaviour>();

        [NonSerialized]
        public float occluSunFactor = 1.0f;
        private eFUnderWater underState = eFUnderWater.eUW_Above;

        [NonSerialized]
        public int targetResWidth = -1;
        [NonSerialized]
        public int targetResHeight = -1;

        private int m_targetWidth = -1;
        private int m_targetHeight = -1;
        
        public static RenderTexture target = null;

        private void AcquireComponents()
	    {
	        if (Camera.main == null)
	            return;

            if (instance == null)
                return;

            transform.gameObject.layer = layerDef.waterlayer;

	        // set up camera
	        if (trailer != null)
	        {
                Camera.main.cullingMask &= ~(1 << layerDef.traillayer);
	        }
	        else
	        {
                Camera.main.cullingMask |= (1 << layerDef.traillayer);
	        }

            if (!envParam.sunLight)
            {
                Light[] lights = Light.GetLights(LightType.Directional, -1);
                if (lights != null && lights.Length > 0)
                {
                    envParam.sunLight = lights[0];
                }
            }
	    }

	    [NonSerialized]
        public bool supportSM3, mobile;

	    void CheckInstance()
	    { 
	        if (instance == null)
	        {
	            instance = this;
	        }
	        else if (instance != this)
	        {
	            Debug.LogWarning("Only can have one FOcean Script instance in scene, Please check!");
	            GameObject.DestroyImmediate(this.gameObject);
	        }
	    }

        Material CreateMaterial(ref Shader shader, string shaderName)
        {
            Material newMat = null;
            if (shader == null)
            {
                Debug.LogWarningFormat("ShaderName: " + shaderName.ToString() + " is missing, would find the shader, then please save FOcean prefab.");
                shader = Shader.Find(shaderName);
            }

            if (shader == null)
            {
                Debug.LogError("FOcean CreateMaterial Failed.");
                return newMat;
            }

            newMat = new Material(shader);
#if UNITY_EDITOR
            newMat.hideFlags = HideFlags.DontSave;
#endif

            return newMat;
        }

        public void GenAllMaterial()
        {
            matFade = CreateMaterial(ref shaderPack.fade, "Hidden/FastOcean/Fade");
            matBlur = CreateMaterial(ref shaderPack.blur, "Hidden/FastOcean/BlurEffectConeTap");
            matCull = CreateMaterial(ref shaderPack.cull, "Hidden/FastOcean/CullMask");
        }


        public void DestroyAllMaterial()
        {
            DestroyMaterial(ref matFade);
            DestroyMaterial(ref matBlur);
            DestroyMaterial(ref matCull);
        }

        void Awake()
	    {
            CheckInstance();

            supportSM3 = needSM3 && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth) &&
                SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RHalf);

            rtR8Format = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8) ? RenderTextureFormat.R8 : RenderTextureFormat.Default;

            if (supportSM3)
            {
                supportSM3 = SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2 &&
                    SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGL2;
            }

            GenAllMaterial();

  //          Physics.gravity = Vector3.zero;

#if MOBILE
            mobile = true;
            
            if (Application.isPlaying)
	        {
                if (envParam.depthBlendMode == eFDepthBlendMode.eDB_DepthBlend && !supportSM3)
		        {
                    envParam.depthBlendMode = eFDepthBlendMode.eDB_AlphaBlend;
                    Debug.LogWarning("depthBlend is not supported, turn to alphaBlend.");
		        }

                if (envParam.underWaterMode == eFUnderWaterMode.eUM_Blend)
                {
                    envParam.underWaterMode = eFUnderWaterMode.eUM_Simple;
                    //Debug.LogWarning("underWaterEffect is simplified, it may cause impact on performance.");
                }
            }
#else
            if (Application.isPlaying)
	        {
                if (envParam.depthBlendMode == eFDepthBlendMode.eDB_DepthBlend && !supportSM3)
		        {
                    envParam.depthBlendMode = eFDepthBlendMode.eDB_AlphaBlend;
                    Debug.LogWarning("depthBlend is not supported, turn to alphaBlend.");
		        }

                if (envParam.underWaterMode == eFUnderWaterMode.eUM_Blend && !supportSM3)
                {
                    envParam.underWaterMode = eFUnderWaterMode.eUM_Simple;
                    Debug.LogWarning("underWaterEffect is simplified.");
                }
			}

            mobile = false;
#endif
            
            //Debug.LogWarning(SystemInfo.supportedRenderTargetCount);

        }

        void OnEnable()
        {
#if UNITY_EDITOR
            transform.hideFlags = HideFlags.HideInInspector;
#endif
        }

        // Use this for initialization
        void Start()
	    {
	        AcquireComponents();

#if UNITY_EDITOR
            transform.hideFlags = HideFlags.HideInInspector;
#endif

            queueRTs.Clear();

            if (trailCamera == null)
            {
                GameObject go = GameObject.Find(FOcean.trailCameraName);

                if (!go)
                {
                    go = new GameObject(FOcean.trailCameraName, typeof(Camera));
                    go.transform.parent = transform;
                }
                if (!go.GetComponent(typeof(Camera)))
                    go.AddComponent(typeof(Camera));
                trailCamera = go.GetComponent<Camera>();
            }

            trailCamera.backgroundColor = Color.black;
            trailCamera.clearFlags = CameraClearFlags.SolidColor;
            trailCamera.renderingPath = RenderingPath.Forward;

            trailCamera.cullingMask = 1 << layerDef.traillayer;
            trailCamera.enabled = false;

            if (oceanCamera == null)
            {
                GameObject go = GameObject.Find(FOcean.oceanCameraName);

                if (!go)
                {
                    go = new GameObject(FOcean.oceanCameraName, typeof(Camera));
                    go.transform.parent = transform;
                }
                if (!go.GetComponent(typeof(Camera)))
                    go.AddComponent(typeof(Camera));
                oceanCamera = go.GetComponent<Camera>();
            }

            oceanCamera.backgroundColor = Color.clear;
            oceanCamera.clearFlags = CameraClearFlags.SolidColor;

            oceanCamera.cullingMask = 1 << layerDef.waterlayer;
            oceanCamera.enabled = false;
            oceanCamera.renderingPath = RenderingPath.Forward;
            oceanCamera.targetTexture = null;

            if (mobile)
            {
                tmParam.renderSpace = eFTRSpace.eTRS_Mask;
                tmParam.trailMaskSize = Mathf.Min(tmParam.trailMaskSize, 512);

                //envParam.glareResolution = eGMResolution.eGMR_Low;
            }

            GenBuffer();

            UnderStateCache();

            isStarted = true;
	    }

        private Vector2 GetTargetRes(out int useResWidth, out int useResHeight)
        {
            if (targetResWidth != -1)
                useResWidth = targetResWidth;
            else
                useResWidth = Screen.width;

            if (targetResHeight != -1)
                useResHeight = targetResHeight;
            else
                useResHeight = Screen.height;

            if (targetResHeight != -1 && targetResWidth != -1)
                return new Vector2((float)useResHeight / Screen.height,(float)useResWidth / Screen.width);
            else
                return Vector2.one;
        }

        void GenBuffer()
        {
            m_bSunGlare = needGlareBehaviour.Count > 0 && envParam.sunMode != eFSunMode.eSM_None;
            m_underMode = envParam.underWaterMode;
            m_trailer = trailer;
            m_trailMaskSize = tmParam.trailMaskSize;
            m_renderSpace = tmParam.renderSpace;

            simpleUnderFlag = !supportSM3 || mobile || envParam.underWaterMode != eFUnderWaterMode.eUM_Blend;

            Vector2 factorRes = GetTargetRes(out m_targetWidth, out m_targetHeight);

            if (trailer != null)
            {
                if (tmParam.renderSpace == eFTRSpace.eTRS_Screen)
                {
                    trailmap = new RenderTexture(m_targetWidth, m_targetHeight, 0);
                    trailmap.filterMode = FilterMode.Bilinear;
                    trailmap.useMipMap = true;
                    trailmap.Create();
                    trailmap.DiscardContents();
#if UNITY_EDITOR
                    trailmap.hideFlags = HideFlags.DontSave;
#endif

                    queueRTs.AddLast(trailmap);
                }

                int resMaskW = (int)(tmParam.trailMaskSize * factorRes.x);
                int resMaskH = (int)(tmParam.trailMaskSize * factorRes.y);
                trailmask = new RenderTexture(resMaskW, resMaskH, 0);
                trailmask.filterMode = mobile ? FilterMode.Bilinear : FilterMode.Trilinear;
                trailmask.useMipMap = true;
                trailmask.wrapMode = TextureWrapMode.Clamp;
                trailmask.Create();
                trailmask.DiscardContents();
#if UNITY_EDITOR
                trailmask.hideFlags = HideFlags.DontSave;
#endif
                queueRTs.AddLast(trailmask);
            }

            if (!supportSM3 || mobile)
                return;

            RenderTexture tmp = RenderTexture.active;
            
            if (m_bSunGlare || envParam.underWaterMode == eFUnderWaterMode.eUM_Blend)
            {
                oceanmap = new RenderTexture(m_targetWidth, m_targetHeight, 16, RenderTextureFormat.RHalf);
                oceanmap.filterMode = FilterMode.Point;
                oceanmap.useMipMap = false;
                oceanmap.Create();
                oceanmap.DiscardContents();
#if UNITY_EDITOR
                oceanmap.hideFlags = HideFlags.DontSave;
#endif
                // to clear color
                RenderTexture.active = oceanmap;
                GL.Clear(true, true, Color.clear);

                queueRTs.AddLast(oceanmap);
            }

            if (m_bSunGlare)
            {
				//downsample
                int shifter = 2;

                //keep screen width for glare size
                glaremap = new RenderTexture(Screen.width >> shifter, Screen.height >> shifter, 0, rtR8Format);
                glaremap.filterMode = FilterMode.Point;
                glaremap.useMipMap = false;
                glaremap.Create();
                glaremap.DiscardContents();
#if UNITY_EDITOR
                glaremap.hideFlags = HideFlags.DontSave;
#endif
                // to clear color
                RenderTexture.active = glaremap;
                GL.Clear(true, true, Color.clear);

                queueRTs.AddLast(glaremap);
            }

            RenderTexture.active = tmp;
        }

        void RelBuffer()
        {
            queueRTs.Clear();
            
            if (oceanCamera != null)
                oceanCamera.targetTexture = null;

            if (trailCamera != null)
                trailCamera.targetTexture = null;

            if (trailmap != null)
            {
                trailmap.Release();
                DestroyImmediate(trailmap);
                trailmap = null;
            }

            if (trailmask != null)
            {
                trailmask.Release();
                DestroyImmediate(trailmask);
                trailmask = null;
            }

            if (oceanmap != null)
            {
                oceanmap.Release();
                DestroyImmediate(oceanmap);
                oceanmap = null;
            }

            if (glaremap != null)
            {
                glaremap.Release();
                DestroyImmediate(glaremap);
                glaremap = null;
            }
        }

        public FOceanGrid mainPG
        {
            get
            {
                return mainGrid;
            }
        }

        bool PointInOABB(Vector3 point, FOceanGrid grid)
        {
            Transform transform = grid.transform;
            Vector3 size = grid.baseParam.boundSize;
            float h = grid.usedOceanHeight;

            float rangeY = (size.y * 0.5f);

            if (point.y > h + rangeY || point.y < h - rangeY)
                return false;

            point = transform.InverseTransformPoint(point);

            float rangeX = (size.x * 0.5f);
            float rangeZ = (size.z * 0.5f);
            if (point.x < rangeX && point.x > -rangeX &&
               point.z < rangeZ && point.z > -rangeZ)
                return true;
            else
                return false;
        }

        private FOceanGrid FindMainGrid()
        {
            Vector3 p = Camera.main.transform.position + Camera.main.transform.forward * Camera.main.nearClipPlane;
            var _e = grids.GetEnumerator();
            FOceanGrid closestPG = null;
            float dis = Mathf.Infinity;
            while (_e.MoveNext())
            {
                if (!_e.Current.baseParam.projectedMesh)
                {
                    if (PointInOABB(p, _e.Current))
                    {
                        return _e.Current;
                    }
                }
                else
                {
                    if (_e.Current.offsetToGridPlane < dis)
                    {
                        dis = _e.Current.offsetToGridPlane;
                        closestPG = _e.Current;
                    }
                }
            }

            return closestPG;
        }

	    public FOceanGrid ClosestGrid(Vector3 p)
	    {
            var _e = grids.GetEnumerator();
            FOceanGrid closestPG = null;
			float dis = Mathf.Infinity;
            while (_e.MoveNext())
            {
                if (!_e.Current.baseParam.projectedMesh)
                {
                    if (PointInOABB(p, _e.Current))
                    {
                        return _e.Current;
                    }
                }
                else
                {
					if(mainGrid != null && mainGrid.baseParam.projectedMesh)
					{
                    	closestPG = mainGrid;
					}
					else if (_e.Current.offsetToGridPlane < dis)
					{
						dis = _e.Current.offsetToGridPlane;
						closestPG = _e.Current;
					}
                }
            }

            return closestPG;
	    }

        public FOceanGrid ClosestGrid(Transform t)
        {
            var _e = grids.GetEnumerator();
            FOceanGrid closestPG = null;
			float dis = Mathf.Infinity;
            while (_e.MoveNext())
            {
                if (!_e.Current.baseParam.projectedMesh)
                {
                    if (PointInOABB(t.position, _e.Current))
                    {
                        return _e.Current;
                    }
                }
                else
                {
					if(mainGrid != null && mainGrid.baseParam.projectedMesh)
					{
						closestPG = mainGrid;
					}
					else if (_e.Current.offsetToGridPlane < dis)
					{
						dis = _e.Current.offsetToGridPlane;
						closestPG = _e.Current;
					}
                }
            }

            return closestPG;
        }

        public bool GlareMapEnabled(FOceanGrid grid)
        {
            if (!Application.isPlaying)
                return false;

			if (grid == null)
                return false;
            
            if (envParam.sunMode == eFSunMode.eSM_None)
                return false;

            if (IntersectWater())
                return false;

			if (grid != mainGrid)
				return false;

            if (!supportSM3)
                return false;

            if (mobile)
                return false;

            return needGlareBehaviour.Count > 0;
        }

        public HashSet<FOceanGrid> GetGrids()
        {
            return grids;
        }

        public bool OceanMapEnabled(FOceanGrid grid)
        {
            if (!Application.isPlaying)
                return false;

            bool bNoSunGlare = !m_bSunGlare;
            if (envParam.underWaterMode == eFUnderWaterMode.eUM_Blend)
            {
                if (bNoSunGlare && !IntersectWater())
                    return false;
            }
            else
            {
                if(IntersectWater())
                    return false;
                else if (bNoSunGlare)
                    return false;
            }

            if (!supportSM3)
                return false;
            
            if (grid == null)
                return false;

			if (grid != mainGrid)
				return false;

            if (mobile)
                return false;

            return true;
        }

        public bool UnderWater()
        {
            if (simpleUnderFlag)
                return underState == eFUnderWater.eUW_Under;

            return (underState == eFUnderWater.eUW_Under || newUnderState == eFUnderWater.eUW_UnderIntersect) && newUnderState != eFUnderWater.eUW_AboveIntersect;
        }

        public bool IntersectWater()
        {
            if (simpleUnderFlag)
                return underState == eFUnderWater.eUW_Under;

            return underState == eFUnderWater.eUW_Under || newUnderState == eFUnderWater.eUW_AboveIntersect || newUnderState == eFUnderWater.eUW_UnderIntersect;
        }

        public bool OnlyUnderWater()
        {
            return underState == eFUnderWater.eUW_Under && newUnderState != eFUnderWater.eUW_AboveIntersect && newUnderState != eFUnderWater.eUW_UnderIntersect;
        }

	    public Color GetBaseColor()
	    {
			if (mainGrid != null && mainGrid.oceanMaterial != null)
				return mainGrid.oceanMaterial.GetColor("_FoBaseColor");

	        return Color.black;
	    }

        public Color GetDeepColor()
        {
            if (mainGrid != null && mainGrid.oceanMaterial != null)
                return mainGrid.oceanMaterial.GetColor("_FoDeepColor");

            return Color.black;
        }

        public void SetBaseColor(Color c)
		{
			if (mainGrid != null && mainGrid.oceanMaterial != null)
			    mainGrid.oceanMaterial.SetColor("_FoBaseColor", c);
		}

        public void SetDeepColor(Color c)
        {
            if (mainGrid != null && mainGrid.oceanMaterial != null)
                mainGrid.oceanMaterial.SetColor("_FoDeepColor", c);
        }

        public void SetShallowColor(Color c)
		{
			if (mainGrid != null && mainGrid.oceanMaterial != null)
				mainGrid.oceanMaterial.SetColor("_FoShallowColor", c);
		}
        
        public void ForceReload(bool bReGen)
        {
			if (!isStarted)
				return;

			if (instance == null)
			{
				instance = this;
			}
			
            if(bReGen)
            {
                DestroyAllMaterial();
                GenAllMaterial();
            }

            RelBuffer();
            GenBuffer();

            var _e = grids.GetEnumerator();
            while (_e.MoveNext())
            {
                _e.Current.ForceReload(bReGen);
            }
        }

	    public void ForceUpdate()
	    {
	       var _e = grids.GetEnumerator();
	       if (instance == null)
	       {
	           instance = this;
               instance.ForceReload(true);
	       }

	        Update();

	        _e = grids.GetEnumerator();
	        while (_e.MoveNext())
	        {
	            _e.Current.Update();
	        }

            LateUpdate();
	    }

        public void ResetTime()
        {
            var _e = grids.GetEnumerator();
            while (_e.MoveNext())
            {
                _e.Current.gwParam.gWTime = 0;
            }
        }


        float tmpCheckTime = 0f;

        public void CheckParams()
        {
            if(!isStarted)
                return;

            if (tmpCheckTime >= Time.realtimeSinceStartup)
                return;

            tmpCheckTime = Time.realtimeSinceStartup + Time.fixedDeltaTime * 2.0f;

            if (mobile)
            {
                if (envParam.depthBlendMode == eFDepthBlendMode.eDB_DepthBlend && !supportSM3)
                {
                    envParam.depthBlendMode = eFDepthBlendMode.eDB_AlphaBlend;
                }

                if (envParam.underWaterMode == eFUnderWaterMode.eUM_Blend)
                {
                    envParam.underWaterMode = eFUnderWaterMode.eUM_Simple;
                }
            }

            bool bSunGlare = needGlareBehaviour.Count > 0 && envParam.sunMode != eFSunMode.eSM_None;
            if (m_bSunGlare != bSunGlare)
            {
                ForceReload(false);
                return;
            }

            if (m_underMode != envParam.underWaterMode)
            {
                UnderStateReset();

                if (envParam.underWaterMode == eFUnderWaterMode.eUM_Blend)
                {
                    ForceReload(false);
                    return;
                }

                m_underMode = envParam.underWaterMode;
            }


            if (m_trailer != trailer)
            {
                ForceReload(false);
                return;
            }

            if (m_trailMaskSize != tmParam.trailMaskSize)
            {
                ForceReload(false);
                return;
            }

            if (m_renderSpace != tmParam.renderSpace)
            {
                ForceReload(false);
                return;
            }

            int useResWidth;
            int useResHeight;
            GetTargetRes(out useResWidth, out useResHeight);

            if (useResWidth != m_targetWidth || useResHeight != m_targetHeight)
            {
                ForceReload(false);
                return;
            }
        }

        eFUnderWater newUnderState = eFUnderWater.eUW_Above;
        bool simpleUnderFlag = false;
        // Update is called once per frame
        void Update()
	    {

            CheckInstance();

#if UNITY_EDITOR
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
#endif

            if (Camera.main == null)
	            return;

            if (queueNode != null)
            {
                if (queueNode.Value != null && !queueNode.Value.IsCreated())
                {
                    if (Application.isPlaying)
                        ForceReload(false);

                    queueNode = null;
                    return;
                }

                queueNode = queueNode.Next;
            }
            else
                queueNode = queueRTs.First;


            CheckParams();
            
            //check underwater
            newUnderState = CheckUnder();

            if (newUnderState == eFUnderWater.eUW_Above && newUnderState == underState)
            {
                UnderStateCache();
            }

            if (newUnderState != underState && newUnderState != eFUnderWater.eUW_AboveIntersect && newUnderState != eFUnderWater.eUW_UnderIntersect)
            {
                UnderStateChanged();
            }

            if (UnderWater())
            {
                RenderSettings.ambientLight = envParam.underAmb;
            }
          
	        //find main grid
            mainGrid = FindMainGrid();

	        if (mainGrid != null && mainGrid.IsVisible())
	            UpdateMainGrid();
	    }

        CameraClearFlags tmpFlags;
        Color tmpAmb;
        Color tmpBgColor;
        Color tmpFogColor;
        FogMode tmpFogMode;
        bool tmpUnderFog;

        static Vector3[] fofrustum = new Vector3[5];
        public eFUnderWater CheckUnder()
        {
            eFUnderWater us = eFUnderWater.eUW_Above;

            if (envParam.underWaterMode == eFUnderWaterMode.eUM_None)
                return us;

            Matrix4x4 invviewproj = (Camera.main.projectionMatrix * Camera.main.worldToCameraMatrix).inverse;

            fofrustum[0] = invviewproj.MultiplyPoint(new Vector3(-1, -1, -1));
            fofrustum[1] = invviewproj.MultiplyPoint(new Vector3(+1, -1, -1));
            fofrustum[2] = invviewproj.MultiplyPoint(new Vector3(-1, +1, -1));
            fofrustum[3] = invviewproj.MultiplyPoint(new Vector3(+1, +1, -1));
            fofrustum[4] = Camera.main.transform.position;

            bool allUnder = true;

            int i = 0;
            for (; i < 5; i++)
            {
                Vector3 contactP = fofrustum[i];
                Vector3 d = Vector3.zero;

                if (!GetSurDisplace(contactP, out d, mainGrid))
                {
                    allUnder = false;
                    continue;
                }

                if (d.y > 0f)
                {
                    if (i == 4)
                        us = eFUnderWater.eUW_UnderIntersect;
                    else
                        us = eFUnderWater.eUW_AboveIntersect;
                }
                else
                    allUnder = false;
            }

            if(allUnder)
            {
                us = eFUnderWater.eUW_Under;
            }

            return us;
        }

#if UNITY_EDITOR
        static bool[] nearstate = new bool[5];
        public void OnDrawGizmos()
        {
            for (int i = 0; i < 5; i++)
            {
                Vector3 contactP = fofrustum[i];
                Vector3 d = Vector3.zero;
                if (!GetSurDisplace(contactP, out d, mainGrid))
                    continue;

                nearstate[i] = d.y > 0;
            }

            Gizmos.color = nearstate[0] ? Color.red : Color.green;
            Gizmos.DrawSphere(fofrustum[0], 0.1f);

            Gizmos.color = nearstate[1] ? Color.red : Color.green;
            Gizmos.DrawSphere(fofrustum[1], 0.1f);

            Gizmos.color = nearstate[2] ? Color.red : Color.green;
            Gizmos.DrawSphere(fofrustum[2], 0.1f);

            Gizmos.color = nearstate[3] ? Color.red : Color.green;
            Gizmos.DrawSphere(fofrustum[3], 0.1f);

            Gizmos.color = nearstate[4] ? Color.red : Color.green;
            Gizmos.DrawSphere(Camera.main.transform.position, 0.1f);

            if (Camera.main == null)
                return;

            Gizmos.matrix = Matrix4x4.identity;

            Transform camtr = Camera.main.transform;
            float camNear = Camera.main.nearClipPlane;
            float camFar = Camera.main.farClipPlane;
            float camFov = Camera.main.fieldOfView;
            float camAspect = Camera.main.aspect;


            float fovWHalf = camFov * 0.5f;

            Vector3 toRight = camtr.right * camNear * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
            Vector3 toTop = camtr.up * camNear * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

            Vector3 topLeft = (camtr.forward * camNear - toRight + toTop);
            float camScale = topLeft.magnitude;

            Vector3 topLeftN = topLeft;
            topLeftN.Normalize();
            topLeftN *= camScale;

            Vector3 topRightN = (camtr.forward * camNear + toRight + toTop);
            topRightN.Normalize();
            topRightN *= camScale;

            Vector3 bottomRightN = (camtr.forward * camNear + toRight - toTop);
            bottomRightN.Normalize();
            bottomRightN *= camScale;

            Vector3 bottomLeftN = (camtr.forward * camNear - toRight - toTop);
            bottomLeftN.Normalize();
            bottomLeftN *= camScale;

            camScale *= camFar / camNear;

            topLeft.Normalize();
            topLeft *= camScale;

            Vector3 topRight = (camtr.forward * camNear + toRight + toTop);
            topRight.Normalize();
            topRight *= camScale;

            Vector3 bottomRight = (camtr.forward * camNear + toRight - toTop);
            bottomRight.Normalize();
            bottomRight *= camScale;

            Vector3 bottomLeft = (camtr.forward * camNear - toRight - toTop);
            bottomLeft.Normalize();
            bottomLeft *= camScale;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(camtr.position, camtr.position + topLeft);
            Gizmos.DrawLine(camtr.position, camtr.position + topRight);
            Gizmos.DrawLine(camtr.position, camtr.position + bottomLeft);
            Gizmos.DrawLine(camtr.position, camtr.position + bottomRight);

            Gizmos.DrawLine(camtr.position + topLeft, camtr.position + topRight);
            Gizmos.DrawLine(camtr.position + topRight, camtr.position + bottomRight);
            Gizmos.DrawLine(camtr.position + bottomRight, camtr.position + bottomLeft);
            Gizmos.DrawLine(camtr.position + bottomLeft, camtr.position + topLeft);

            Gizmos.DrawLine(camtr.position + topLeftN, camtr.position + topRightN);
            Gizmos.DrawLine(camtr.position + topRightN, camtr.position + bottomRightN);
            Gizmos.DrawLine(camtr.position + bottomRightN, camtr.position + bottomLeftN);
            Gizmos.DrawLine(camtr.position + bottomLeftN, camtr.position + topLeftN);

            if (!drawButtomGizmos)
                return;

            if(oceanCamera != null && envParam.underButtom != null && matCull != null)
            {
                if (OnlyUnderWater())
                {
                    Gizmos.color = Color.cyan;

                    Matrix4x4 matrixButtom = Matrix4x4.identity;
                    matrixButtom.SetTRS(oceanCamera.transform.position, Quaternion.identity, Vector3.one * Camera.main.farClipPlane * 0.5f);
                    Gizmos.matrix = matrixButtom;
                    Gizmos.DrawWireMesh(envParam.underButtom);
                }
                else if (IntersectWater())
                {
                    Gizmos.color = Color.cyan;

                    Matrix4x4 matrixButtom = Matrix4x4.identity;
                    matrixButtom.SetTRS(oceanCamera.transform.position - Vector3.up * Camera.main.farClipPlane * 0.5f, Quaternion.identity, new Vector3(1f, 0.5f, 1f) * Camera.main.farClipPlane);
                    Gizmos.matrix = matrixButtom;
                    Gizmos.DrawWireMesh(envParam.underButtom);
                }
            }
        }
#endif

        void UnderStateChanged()
        {
            if (newUnderState == eFUnderWater.eUW_Under)
            {
                UnderStateReset(false);
            }
            else
            {
                Camera.main.clearFlags = tmpFlags;
                RenderSettings.ambientLight = tmpAmb;
                Camera.main.backgroundColor = tmpBgColor;
                
                RenderSettings.fog = tmpUnderFog;
                RenderSettings.fogColor = tmpFogColor;
                RenderSettings.fogMode = tmpFogMode;
            }

            underState = newUnderState;
        }

        public void UnderStateCache()
        {
            tmpFlags = Camera.main.clearFlags;
            tmpAmb = RenderSettings.ambientLight;
            tmpBgColor = Camera.main.backgroundColor;

            tmpFogColor = RenderSettings.fogColor;
            tmpFogMode = RenderSettings.fogMode;
            tmpUnderFog = RenderSettings.fog;
        }

        public void UnderStateReset(bool checkUnder = true)
        {
            if (underState != eFUnderWater.eUW_Under && checkUnder)
                return;

            simpleUnderFlag = !supportSM3 || mobile || envParam.underWaterMode != eFUnderWaterMode.eUM_Blend;

            //for simple
            if (simpleUnderFlag)
            {
                RenderSettings.fog = true;

                Camera.main.clearFlags = CameraClearFlags.SolidColor;
                RenderSettings.fogMode = FogMode.Exponential;
            }
            else
            {
                Camera.main.clearFlags = CameraClearFlags.Skybox;
            }

            RenderSettings.ambientLight = envParam.underAmb;
            RenderSettings.fogColor = GetBaseColor();
            Camera.main.backgroundColor = RenderSettings.fogColor;
        }

        void LateUpdate()
        {
            if (mainGrid == null)
                return;

            if (Camera.main == null)
                return;

            if (mainGrid.oceanMaterial == null)
            {
                Debug.LogWarning("oceanMaterial == null, Please Set That!");
                return;
            }

            var _e = grids.GetEnumerator();
            while (_e.MoveNext())
            {
                _e.Current.SetupMaterial();
            }

            if (!mainGrid.IsVisible())
            {
                RenderTexture tmp = RenderTexture.active;
                if (oceanmap)
                {
                    RenderTexture.active = oceanmap;
                    GL.Clear(true, true, UnderWater() ? Color.white : Color.clear);
                }

                if (glaremap)
                {
                    RenderTexture.active = glaremap;
                    GL.Clear(true, true, Color.clear);
                }

                if (trailmask)
                {
                    RenderTexture.active = trailmask;
                    GL.Clear(true, true, Color.clear);
                }

                if (trailmap)
                {
                    RenderTexture.active = trailmap;
                    GL.Clear(true, true, Color.clear);
                }

                RenderTexture.active = tmp;
                return;
            }

            RenderTrails();
            RenderOceanMap();
            RenderGlareMap();
        }

        public float UpdateCameraPlane(FOceanGrid grid, float fAdd)
        {
			return Mathf.Max(Camera.main.nearClipPlane, Camera.main.farClipPlane + fAdd);
        }

	    private void UpdateMainGrid()
	    {
	        AcquireComponents();

			if (!mainGrid.oceanMaterial)
                return;

			if (mainGrid.bSimluateReady) 
			{
                Physics.gravity = -Vector3.up * g;
			} 
			else
			{
				//Physics.gravity = Vector3.zero;
			}

	        CheckDepth(mainGrid);
            
            FUnderWater ueffect = (FUnderWater)Camera.main.gameObject.GetComponent(typeof(FUnderWater));
            if (!ueffect)
            {
                ueffect = Camera.main.gameObject.AddComponent<FUnderWater>();
            }

            ueffect.enabled = !simpleUnderFlag && IntersectWater();

            CheckSun();
	    }

	    public void UpdateMaterial(FOceanGrid grid, Material material)
	    {
            if (material == null)
                return;

            if (!reflection)
            {
                reflection = transform.GetComponent<FReflection>();
                if (reflection == null)
                {
                    reflection = gameObject.AddComponent<FReflection>();
                }
            }

            if (envParam.sunLight != null)
	        {
                material.SetVector("_FoSunDir", envParam.sunLight.transform.forward);
                material.SetColor("_FoSunColor", envParam.sunLight.color);
	        }
            else
            {
                material.SetVector("_FoSunDir", -Vector3.up);
                material.SetColor("_FoSunColor", Color.black);
            }

            material.SetFloat("_FoSunInt", occluSunFactor);

	        if (envParam.foamEnabled && !UnderWater())
	        {
                material.EnableKeyword("FO_FOAM_ON");
	        }
	        else
	        {
	            material.DisableKeyword("FO_FOAM_ON");
	        }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                material.EnableKeyword("FO_EDITING");
            }
            else
            {
                material.DisableKeyword("FO_EDITING");
            }
#endif

            if (trailer != null)
	        {
                if (tmParam.renderSpace == eFTRSpace.eTRS_Screen)
                {
                    material.EnableKeyword("FO_TRAILSCREEN_ON");
                    material.DisableKeyword("FO_TRAIL_ON");
                }
                else
                {
                    material.EnableKeyword("FO_TRAIL_ON");
                    material.DisableKeyword("FO_TRAILSCREEN_ON");
                }
	        }
	        else
	        {
	            material.DisableKeyword("FO_TRAIL_ON");
                material.DisableKeyword("FO_TRAILSCREEN_ON");
	        }
            
            if (envParam.sunMode == eFSunMode.eSM_Phong)
            {
                material.EnableKeyword("FO_PHONG_ON");
                material.DisableKeyword("FO_BLINNPHONG_ON");
            }
            else if (envParam.sunMode == eFSunMode.eSM_BlinnPhong)
            {
                material.EnableKeyword("FO_BLINNPHONG_ON");
                material.DisableKeyword("FO_PHONG_ON");
            }
            else
            {
                material.DisableKeyword("FO_BLINNPHONG_ON");
                material.DisableKeyword("FO_PHONG_ON");
            }

            if (IntersectWater())
            {
                if (simpleUnderFlag)
                {
                    material.SetFloat("_Skirt", 0f);
                }
                else
                {
                    material.SetFloat("_Skirt", grid.offsetToGridPlane + Camera.main.farClipPlane);
                }
            }

            if (envParam.depthBlendMode != eFDepthBlendMode.eDB_None)
            {
                if (supportSM3 && envParam.depthBlendMode == eFDepthBlendMode.eDB_DepthBlend)
                {
                    material.EnableKeyword("FO_DEPTHBLEND_ON");
                }
                else
                {
                    material.DisableKeyword("FO_DEPTHBLEND_ON");
                }

                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            }
	        else
	        {
	            material.DisableKeyword("FO_DEPTHBLEND_ON");

                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            }

            if (envParam.underWaterMode == eFUnderWaterMode.eUM_Blend)
            {
                material.SetInt("_CullAbove", (int)UnityEngine.Rendering.CullMode.Off);
                material.SetInt("_CullUnder", (int)UnityEngine.Rendering.CullMode.Off);
            }
            else
            {
                material.SetInt("_CullAbove", (int)UnityEngine.Rendering.CullMode.Back);
                material.SetInt("_CullUnder", (int)UnityEngine.Rendering.CullMode.Front);
            }


            if ((grid.gwParam.mode & eFGWMode.eGWM_Render) != eFGWMode.eGWM_None)
	        {
				material.EnableKeyword("FO_GERSTNER_ON");
	        }
	        else
	        {
				material.DisableKeyword("FO_GERSTNER_ON");
	        }
            
            if (grid.baseParam.projectedMesh)
            {
                material.EnableKeyword("FO_PROJECTED_ON");
            }
            else
            {
                material.DisableKeyword("FO_PROJECTED_ON");
            }
        }

	    public int AboveLOD()
	    {
            if (!Application.isPlaying)
                return FOcean.LOD;

	        return supportSM3 ? FOcean.ABOVELODSM3 : FOcean.ABOVELOD;
	    }

	    public int UnderLOD()
	    {
            if (!Application.isPlaying)
                return FOcean.LOD;

	        return supportSM3 ? FOcean.UNDERLODSM3 : FOcean.UNDERLOD;
	    }

	    public int OceanMapLOD()
	    {
	        return IntersectWater() ? (newUnderState == eFUnderWater.eUW_AboveIntersect ? FOcean.UNDEROCEANLOD2 : FOcean.UNDEROCEANLOD) : FOcean.OCEANLOD;
	    }

	    public int GlareMapLOD()
	    {
	        return FOcean.GlARELOD;
	    }

        void CheckSun()
	    {
			 if (envParam.sunLight == null)
				return;

            if (envParam.sunMode == eFSunMode.eSM_None && needSunBehaviour.Count == 0)
                return;

            float sunInt = envParam.sunIntensity * envParam.sunLight.intensity;

            Vector3 origin = Camera.main.transform.position;
             Ray ray = new Ray(origin, -envParam.sunLight.transform.forward);
			 RaycastHit hit;
             Physics.Raycast(ray, out hit, Camera.main.farClipPlane, envParam.sunOccluMask);

			 if (hit.distance > 0.0f)
				occluSunFactor = sunInt * hit.distance / Camera.main.farClipPlane;
	         else
				occluSunFactor = sunInt;
	    }

	    public void AddPG(FOceanGrid grid)
	    {
	        if (!grids.Contains(grid))
	        {
                Material mat = grid.oceanMaterial;
	            UpdateMaterial(grid, mat);
	            grids.Add(grid);
	        }
	    }

	    public void RemovePG(FOceanGrid grid)
	    {
	         grids.Remove(grid);
	    }

        static Vector2[] blurOffsets = new Vector2[4]; 
        public static void BlurTapCone(RenderTexture src, RenderTexture dst, Material mat, float blurSpread)
        {
#if MOBILE
            if (dst != null)
                dst.DiscardContents();
            else
                return;
#endif
            if (mat == null)
                return;

            float off = 0.5f + blurSpread;
            blurOffsets[0] = new Vector2(-off, -off);
            blurOffsets[1] = new Vector2(-off, off);
            blurOffsets[2] = new Vector2(off, off);
            blurOffsets[3] = new Vector2(off, -off);

            Graphics.BlitMultiTap(src, dst, mat, blurOffsets);
        }

	    public static void Blit(RenderTexture src, RenderTexture dst, Material mat)
	    {
	#if MOBILE
            if (dst != null)
                dst.DiscardContents();
            else
                return;
	#endif
	        if (mat != null)
	            Graphics.Blit(src, dst, mat);
	        else
	            Graphics.Blit(src, dst);
	    }

        public static void BlitDontClear(RenderTexture src, RenderTexture dst, Material mat)
        {
#if MOBILE
            if (target != null)
            {
                if (dst != null)
                    dst.DiscardContents();
                else
                    return;
            }
#endif
            if (mat != null)
                Graphics.Blit(src, dst, mat);
            else
                Graphics.Blit(src, dst);
        }

	    public static void Blit(RenderTexture src, RenderTexture dst, Material mat, int pass)
	    {
	#if MOBILE
            if (dst != null)
	            dst.DiscardContents();
            else
                return;
	#endif
	        if (mat != null)
	            Graphics.Blit(src, dst, mat, pass);
	        else
	            Graphics.Blit(src, dst);
	    }

        public static void BlitDontClear(RenderTexture src, RenderTexture dst, Material mat, int pass)
        {
#if MOBILE
            if (target != null)
            {
                if (dst != null)
                    dst.DiscardContents();
                else
                    return;
            }
#endif
            if (mat != null)
                Graphics.Blit(src, dst, mat, pass);
            else
                Graphics.Blit(src, dst);
        }


        public static void CustomGraphicsBlit(RenderTexture src, RenderTexture dst, Material fxMaterial, int passNr)
        {
#if MOBILE
            if (target != null)
            {
                if (dst != null)
                    dst.DiscardContents();
                else
                    return;
            }
#endif
            RenderTexture.active = dst;

            fxMaterial.SetTexture("_MainTex", src);

            GL.PushMatrix();
            GL.LoadOrtho();

            fxMaterial.SetPass(passNr);

            GL.Begin(GL.QUADS);

            GL.MultiTexCoord2(0, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 3.0f); // BL

            GL.MultiTexCoord2(0, 1.0f, 0.0f);
            GL.Vertex3(1.0f, 0.0f, 2.0f); // BR

            GL.MultiTexCoord2(0, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f); // TR

            GL.MultiTexCoord2(0, 0.0f, 1.0f);
            GL.Vertex3(0.0f, 1.0f, 0.0f); // TL

            GL.End();
            GL.PopMatrix();
        }


        private bool IsNeedDepth(FOceanGrid grid)
	    {
            if (!supportSM3)
                return false;

            return  envParam.depthBlendMode == eFDepthBlendMode.eDB_DepthBlend || envParam.underWaterMode == eFUnderWaterMode.eUM_Blend ||
                    needDepthBehaviour.Count > 0;
	    }

	    public void CheckDepth(FOceanGrid grid)
	    {
	#if !UNITY_EDITOR
	        Camera cur = Camera.main;
	#else 
	        Camera cur = Camera.current;
	#endif 
	        if (cur == null)
	            return;

	        if (grid == null)
	            return;

            if (cur != Camera.main)
            {
    //#if !UNITY_EDITOR
    //            cur.depthTextureMode = DepthTextureMode.None;
    //#endif
                return;
            }

	        if (IsNeedDepth(grid))
	            cur.depthTextureMode |= DepthTextureMode.Depth;
	        else
	            cur.depthTextureMode = DepthTextureMode.None;
	        
	    }

		void DestroyMaterial(ref Material mat)
		{
			if(mat != null)
			   DestroyImmediate(mat);

			mat = null;
		}

	    void OnDestroy()
	    {
            RelBuffer();

            DestroyAllMaterial();

            mainGrid = null;

	        instance = null;
        }


        void RenderTrails()
        {
            if (trailer == null)
                return;

            if (trailmask == null)
                return;

            if (!trailmask.IsCreated())
                return;

            if (matFade == null)
                return;

            if (trailCamera == null)
                return;

            if (Camera.main == null)
                return;

            bool tmpfog = RenderSettings.fog;

            RenderSettings.fog = false;

            trailCamera.enabled = true;

            float worldsize = tmParam.trailMaskSize * tmParam.trailMaskScale * 0.5f;

            // render in project space
            trailCamera.orthographicSize = worldsize;
            trailCamera.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
            trailCamera.transform.position = new Vector3(trailer.position.x, Camera.main.farClipPlane * 0.5f, trailer.position.z);

            trailCamera.nearClipPlane = Camera.main.nearClipPlane;
            trailCamera.farClipPlane = Camera.main.farClipPlane;
            trailCamera.orthographic = true;
            trailCamera.depthTextureMode = DepthTextureMode.None;
            trailCamera.backgroundColor = Color.black;
            trailCamera.clearFlags = CameraClearFlags.SolidColor;
            trailCamera.cullingMask = 1 << layerDef.traillayer;
            trailCamera.renderingPath = RenderingPath.Forward;

            trailCamera.targetTexture = trailmask;
            trailCamera.Render();

            // render mask texture
            //float bordWidth = 2f / trailmask.width;
            RenderTexture tmpRt = RenderTexture.GetTemporary(trailmask.width, trailmask.height, 0, trailmask.format);
            matFade.SetTexture("u_Input", trailmask);
            //matFade.SetFloat("up", 1f - bordWidth);
            //matFade.SetFloat("down", bordWidth);
            matFade.SetFloat("u_FoFade", tmParam.trailMaskFade);
            FOcean.Blit(null, tmpRt, matFade);

            
            if (trailmap != null && trailmap.IsCreated())
            {
                FOcean.Blit(tmpRt, trailmask, null);

                // render in screen space
                float depth = trailCamera.depth;
                trailCamera.CopyFrom(Camera.main);
                trailCamera.depth = depth;
                trailCamera.renderingPath = RenderingPath.Forward;
                trailCamera.depthTextureMode = DepthTextureMode.None;
                trailCamera.backgroundColor = Color.black;
                trailCamera.clearFlags = CameraClearFlags.SolidColor;
                trailCamera.cullingMask = 1 << layerDef.traillayer;

                trailCamera.targetTexture = trailmap;
                trailCamera.Render();
            }
            else
            {
                FOcean.BlurTapCone(tmpRt, trailmask, matBlur, 0f);
            }

            var _e = grids.GetEnumerator();
            while (_e.MoveNext())
            {
                Material oceanMaterial = _e.Current.oceanMaterial;

                float tmpInt = QualitySettings.activeColorSpace == ColorSpace.Linear ? 2f : 1f;
                if (trailmap != null && trailmap.IsCreated())
                {
                    oceanMaterial.SetTexture("_TrailTex", trailmap);
                }
                else
                {
                    tmpInt *= 2f;
                }
                
                oceanMaterial.SetFloat("_TrailIntensity", tmParam.trailIntensity * tmpInt);

                oceanMaterial.SetTexture("_TrailMask", trailmask);
                oceanMaterial.SetVector("_TrailOffset", new Vector4(trailer.position.x - worldsize,
                    trailer.position.z - worldsize, 1f / (worldsize * 2.0f), 0));
            }

            RenderTexture.ReleaseTemporary(tmpRt);

            trailCamera.enabled = false;

            RenderSettings.fog = tmpfog;
        }
			
        void RenderOceanMap()
        {
			if (!OceanMapEnabled(mainGrid))
				return;

            if (oceanmap != null && oceanmap.IsCreated())
            {
                bool tmpfog = RenderSettings.fog;

                RenderSettings.fog = false;

                float depth = oceanCamera.depth;
                oceanCamera.CopyFrom(Camera.main);
                oceanCamera.depth = depth;

                oceanCamera.depthTextureMode = DepthTextureMode.None;
                oceanCamera.backgroundColor = Color.clear;
                oceanCamera.clearFlags = CameraClearFlags.SolidColor;
                oceanCamera.cullingMask = 1 << layerDef.waterlayer;
                oceanCamera.targetTexture = oceanmap;
                oceanCamera.renderingPath = RenderingPath.Forward;

				var _e = grids.GetEnumerator();
				while (_e.MoveNext())
				{
					if(mainGrid == _e.Current)
						continue;

					_e.Current.gameObject.layer = 0;
				}

				int tmpLod = mainGrid.oceanMaterial.shader.maximumLOD;

				mainGrid.oceanMaterial.shader.maximumLOD = OceanMapLOD();

                oceanCamera.enabled = true;

                //draw buttom
                if (OnlyUnderWater())
                {
                    Matrix4x4 matrixButtom = Matrix4x4.identity;
                    matrixButtom.SetTRS(oceanCamera.transform.position, Quaternion.identity, Vector3.one * Camera.main.farClipPlane * 0.5f);
                    Graphics.DrawMesh(envParam.underButtom, matrixButtom, matCull, layerDef.waterlayer, oceanCamera);
                }
                else if (IntersectWater())
                {
                    Matrix4x4 matrixButtom = Matrix4x4.identity;
                    matrixButtom.SetTRS(oceanCamera.transform.position - Vector3.up * Camera.main.farClipPlane * 0.5f, Quaternion.identity, new Vector3(1f, 0.5f, 1f) * Camera.main.farClipPlane);
                    Graphics.DrawMesh(envParam.underButtom, matrixButtom, matCull, layerDef.waterlayer, oceanCamera);
                }

                oceanCamera.Render();
                oceanCamera.enabled = false;

				mainGrid.oceanMaterial.shader.maximumLOD = tmpLod;

			    _e = grids.GetEnumerator();
				while (_e.MoveNext())
				{
					if(mainGrid == _e.Current)
						continue;

                    _e.Current.gameObject.layer = layerDef.waterlayer;
				}

                RenderSettings.fog = tmpfog;
            }
        }

        void RenderGlareMap()
        {
			if (!GlareMapEnabled(mainGrid))
				return;
			
            if (glaremap != null && glaremap.IsCreated())
            {
                float depth = oceanCamera.depth;
                oceanCamera.CopyFrom(Camera.main);
                oceanCamera.depth = depth;

                oceanCamera.depthTextureMode = DepthTextureMode.None;
                oceanCamera.backgroundColor = Color.clear;
                oceanCamera.clearFlags = CameraClearFlags.SolidColor;
                oceanCamera.cullingMask = 1 << layerDef.waterlayer;
                oceanCamera.renderingPath = RenderingPath.Forward;
                oceanCamera.targetTexture = glaremap;

				var _e = grids.GetEnumerator();
				while (_e.MoveNext())
				{
					if(mainGrid == _e.Current)
					   continue;

					_e.Current.gameObject.layer = 0;
				}

				int tmpLod = mainGrid.oceanMaterial.shader.maximumLOD;

				mainGrid.oceanMaterial.shader.maximumLOD = GlareMapLOD();

                oceanCamera.enabled = true;
                oceanCamera.Render();
                oceanCamera.enabled = false;

				mainGrid.oceanMaterial.shader.maximumLOD = tmpLod;

			    _e = grids.GetEnumerator();
				while (_e.MoveNext())
				{
					if(mainGrid == _e.Current)
						continue;

                    _e.Current.gameObject.layer = layerDef.waterlayer;
				}
            }
        }

	    public void OnWillRender(FOceanGrid grid, Material material)
	    {
            if (material == null)
                return;

	#if UNITY_EDITOR
	        CheckDepth(grid);
	#endif

            if (grid == null)
	            return;

	        UpdateMaterial(grid, material);

	        if (Camera.current != Camera.main)
			{

    #if UNITY_EDITOR
                //when !Application.isPlaying, may not clearTexs in Update, so clear here
                if (reflection && !Application.isPlaying)
                    reflection.WaterTileClear(material);

                if (material)
	            {
                    material.DisableKeyword("FO_TRAIL_ON");
                    material.DisableKeyword("FO_TRAILSCREEN_ON");
	            }

                if (Camera.current == oceanCamera)
                    return;

                if (Camera.current == trailCamera)
                    return;

	#else
				return;
	#endif
			}

            if (reflection)
            {
                if(!UnderWater())
                   reflection.WaterTileBeingRendered(Camera.current, grid.usedOceanHeight, material, grid.reflParam);
                else
                   reflection.WaterTileClear(material);
            }
	    }

        public bool GetSurDisplaceNormal(Vector3 worldPoint, out Vector3 d, out Vector3 n, Transform trans)
        {
            FOceanGrid grid = ClosestGrid(trans);

            if (grid == null)
            {
                d = Vector3.zero;
                n = Vector3.up;
                return false;
            }

            Vector3 ch;
            bool ret = grid.GetSurPointNormal(worldPoint, out ch, out n);
            d = ch - worldPoint;
            return ret;
        }

        public bool GetSurDisplaceNormal(Vector3 worldPoint, out Vector3 d, out Vector3 n, FOceanGrid grid)
        {
            if (grid == null)
            {
                d = Vector3.zero;
                n = Vector3.up;
                return false;
            }

            Vector3 ch;
            bool ret = grid.GetSurPointNormal(worldPoint, out ch, out n);
            d = ch - worldPoint;
            return ret;
        }

        public bool GetSurDisplace(Vector3 worldPoint, out Vector3 d, Transform trans)
        {
            FOceanGrid grid = ClosestGrid(trans);

            if (grid == null)
            {
                d = Vector3.zero;
                return false;
            }

            Vector3 ch;
            bool ret = grid.GetSurPoint(worldPoint, out ch);
            d = ch - worldPoint;
            return ret;
        }

        public bool GetSurDisplace(Vector3 worldPoint, out Vector3 d, FOceanGrid grid)
        {
            if (grid == null)
            {
                d = Vector3.zero;
                return false;
            }

            Vector3 ch;
            bool ret = grid.GetSurPoint(worldPoint, out ch);
            d = ch - worldPoint;
            return ret;
        }

        public bool GetSurPointNormal(Vector3 worldPoint, out Vector3 p, out Vector3 n, Transform trans)
        {
            FOceanGrid grid = ClosestGrid(trans);

            if (grid == null)
            {
                p = worldPoint;
                n = Vector3.up;
                return false;
            }

            return grid.GetSurPointNormal(worldPoint, out p, out n);
        }

        public bool GetSurPointNormal(Vector3 worldPoint, out Vector3 p, out Vector3 n, FOceanGrid grid)
	    {
            if (grid == null)
	        {
                p = worldPoint;
	            n = Vector3.up;
	            return false;
	        }

            return grid.GetSurPointNormal(worldPoint, out p, out n);
	    }

        public bool GetSurPoint(Vector3 worldPoint, out Vector3 p, Transform trans)
        {
            FOceanGrid grid = ClosestGrid(trans);

            if (grid == null)
            {
                p = worldPoint;
                return false;
            }

            return grid.GetSurPoint(worldPoint, out p);
        }

        public bool GetSurPoint(Vector3 worldPoint, out Vector3 p, FOceanGrid grid)
        {
            if (grid == null)
            {
                p = worldPoint;
                return false;
            }

            return grid.GetSurPoint(worldPoint, out p);
        }

	    public static Vector4 CosV4(ref Vector4 V)
	    {
	        return new Vector4(Mathf.Cos(V.x), Mathf.Cos(V.y), Mathf.Cos(V.z), Mathf.Cos(V.w));
	    }

        public static Vector4 SinV4(ref Vector4 V)
	    {
	        return new Vector4(Mathf.Sin(V.x), Mathf.Sin(V.y), Mathf.Sin(V.z), Mathf.Sin(V.w));
	    }

	    public static Vector4 CosInvAB(Vector4 V, Vector4 F)
	    {
	        return Vector4.Scale(new Vector4(F.x, F.x, F.y, F.y), new Vector4(Mathf.Cos(V.x), Mathf.Sin(V.x), Mathf.Cos(V.y), Mathf.Sin(V.y)));
	    }

	    public static Vector4 CosInvCD(Vector4 V, Vector4 F)
	    {
	        return Vector4.Scale(new Vector4(F.z, F.z, F.w, F.w), new Vector4(Mathf.Cos(V.z), Mathf.Sin(V.z), Mathf.Cos(V.w), Mathf.Sin(V.w)));
	    }

	    public static Vector4 InverseV4(Vector4 V)
	    {
	        return new Vector4(1f / V.x, 1f / V.y, 1f / V.z, 1f / V.w);
	    }

        public static Vector4 SqrtV4(Vector4 V)
        {
            return new Vector4(Mathf.Sqrt(V.x), Mathf.Sqrt(V.y), Mathf.Sqrt(V.z), Mathf.Sqrt(V.w));
        }

        public static float Frac(float value)
        {
            return value - (float)Math.Truncate(value);
        }

        public static Vector4 FracV4(Vector4 V)
        {
            return new Vector4(Frac(V.x), Frac(V.y), Frac(V.z), Frac(V.w));
        }

        public static Color SuppleColor(Color c)
        {
            float max = Mathf.Max(Mathf.Max(c.r, c.g), c.b);

            float r = Mathf.Clamp01((c.r - max) * 255 + 1);
            float g = Mathf.Clamp01((c.g - max) * 255 + 1);
            float b = Mathf.Clamp01((c.b - max) * 255 + 1);

            r = Mathf.Lerp(0.5f * c.r, c.r, r);
            g = Mathf.Lerp(0.5f * c.g, c.g, g);
            b = Mathf.Lerp(0.5f * c.b, c.b, b);

            return new Color(r, g, b);
        }
    }
}
