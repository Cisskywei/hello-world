/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FastOcean
{
	[Serializable]
	public class FBaseParameters
	{
        [Range(32, 254)]
        public int usedGridSize = 254;
        
        public bool projectedMesh = true;

        /// <summary>
        /// projectedMesh
        /// </summary>
        /// 
        public float oceanHeight = 0.0f;

        public Vector4 minBias = new Vector4(1,1,0.3f,15f);

        /// <summary>
        /// non projectedMesh
        /// </summary>
        public Vector3 boundSize = Vector3.one * 100f;

        public Vector3 boundPos = Vector3.zero;

        [Range(0,360)]
        public float boundRotate = 0f;
    }

	[Serializable]
	public class FGWParameters
	{
        public eFGWMode mode = eFGWMode.eGWM_RenderAndSimulation;

        [Range(0.01f, 10f)]
        public float gwScale = 0.63f; //all waves Scale

        [Range(0, 360)]
        public float gwDirection = 180f;

        [Range(0, 1)]
        public float gwFlow = 0.7f;

        [Range(0, 10)]
        public float gWChoppiness = 1.6f;
        
        [Range(0, 10)]
        public float gWSpeed = 1.0f;
        
        public double gWTime = 0.0f; 

        public Vector4 gWamplitude = new Vector4(0.03f, 0.01f, 0.06f, 0.06f);
		public Vector4 gWaveLength = new Vector4(0.84f, 0.87f, 0.79f, 0.64f);
	}

    [Serializable]
    public class FReflParameters
    {
        public eFRWQuality quality = eFRWQuality.eRW_Medium;
        // reflection
        public LayerMask reflectionMask;

        public float clipPlaneOffset = 0.07F;

        public bool blurEnabled = false;

        [Range(0.1f, 1f)]
        public float blurSpread = 0.5f;
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [ExecuteInEditMode]
    public class FOceanGrid : MonoBehaviour
    {
        private Camera projectorCamera = null;

        public FBaseParameters baseParam = null;
        public FGWParameters gwParam = null;
        public FReflParameters reflParam = null;
        public Material oceanMaterial = null;

        [NonSerialized]
        public Plane basePlane;

        [NonSerialized]
        public float usedOceanHeight;

        public bool renderEnabled = true;

        private Vector4 foCorners0, foCorners1, foCorners2, foCorners3;

        private int gridsize;
        private bool projectedMesh;
        private Vector3 boundSize;
        private float nSnell = 1.34f;
        private int skirt = 4;
        private float farPlaneBound = 0f;

        private Vector4 gDirectionAB;
        private Vector4 gDirectionCD;
        private Vector4 gAmplitude;
        private Vector4 gSpeed;
        private Vector4 gSteepness;

        private Vector4 gTime = Vector4.zero;
        private Vector4 gSteepnessAB = Vector4.zero;
        private Vector4 gSteepnessCD = Vector4.zero;
        private Vector4 gFreqAB = Vector4.zero;
        private Vector4 gFreqCD = Vector4.zero;
        
        private Vector2 waveDir = Vector2.zero;

        const float twoPI = 2f * Mathf.PI;
        const float halfPI = 0.5f * Mathf.PI;
        
        [SerializeField]
        public Texture2D fresnelLookUp;

        [NonSerialized]
        public RenderTexture buffer = null;

        private MeshFilter meshFilter = null;
        private Renderer meshRenderer = null;

        private bool isStarted = false;

        void GenBuffer()
        {
            CreateFresnelLookUp();

            bSimluateReady = false;
        }

        void OnEnable()
        {
#if UNITY_EDITOR
            transform.hideFlags = HideFlags.HideInInspector;
#endif
            if (!isStarted)
                Init();
        }

        void Init()
        {
            if (FOcean.instance == null)
            {
                return;
            }

            //to check for Performance
            if (oceanMaterial != null && !FOcean.instance.supportSM3)
            {
                gwParam.mode &= ~eFGWMode.eGWM_Render;
                baseParam.usedGridSize = Mathf.Clamp(baseParam.usedGridSize, 32, 128);
                Debug.LogWarning("waves Rendering is disabled, due to shader does not support for this platform.");
            }

            if (GameObject.Find(FOcean.projectorCameraName) == null)
            {
                projectorCamera = new GameObject(FOcean.projectorCameraName).AddComponent<Camera>();
                projectorCamera.transform.parent = FOcean.instance.transform;
                projectorCamera.enabled = false;
                projectorCamera.cullingMask = 0;
            }
            else
                projectorCamera = GameObject.Find(FOcean.projectorCameraName).GetComponent<Camera>();

            projectorCamera.targetTexture = null;

            Resources.UnloadUnusedAssets();

            GenBuffer();

            GenMesh();

            FOcean.instance.AddPG(this);

			isStarted = true;
        }

        void OnDestroy()
        {
            bSimluateReady = false;

            if (fresnelLookUp != null)
                DestroyImmediate(fresnelLookUp);
            fresnelLookUp = null;

            if (FOcean.instance != null)
                FOcean.instance.RemovePG(this);

            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                DestroyImmediate(meshFilter.sharedMesh);
            }

            meshRenderer = null;
        }

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (Camera.main == null)
                return;

            if (FOcean.instance == null)
                return;

            if (waveDir != Vector2.zero)
            {
                Gizmos.color = Color.yellow;
                float scale = Camera.main.farClipPlane * 0.01f;
                Vector3 drawPos = new Vector3(this.transform.position.x, usedOceanHeight, this.transform.position.z);
                Vector3 drawDir = new Vector3(waveDir.x, 0f, waveDir.y);
                Gizmos.DrawRay(drawPos, drawDir * scale);

                Vector3 right = Quaternion.LookRotation(drawDir) * Quaternion.Euler(0, 180 + 20f, 0) * new Vector3(0, 0, 1);
                Vector3 left = Quaternion.LookRotation(drawDir) * Quaternion.Euler(0, 180 - 20f, 0) * new Vector3(0, 0, 1);
                Gizmos.DrawRay(drawPos + drawDir * scale, right * 0.25f * scale);
                Gizmos.DrawRay(drawPos + drawDir * scale, left * 0.25f * scale);
            }

            if(!baseParam.projectedMesh)
            {
                Gizmos.color = Color.magenta;

                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(Vector3.zero, baseParam.boundSize);
            }

        }
#endif

        public void ForceReload(bool bReGen)
        {
            if (FOcean.instance == null)
                return;

            if (bReGen)
            {
                StopAllCoroutines();
                GenBuffer();
            }
        }

		public Vector3 GetDrift(Vector3 p)
		{
			return new Vector3(waveDir.x, 0f, waveDir.y) * gwParam.gWSpeed;
		}
        
        public Vector3 GetHoriWaveVector(float x, float z)
        {
			return new Vector3(waveDir.x, 0f, waveDir.y);
        }
        
        public void Update()
        {
            if (Camera.main == null)
                return;

            if (FOcean.instance == null)
                return;

            if (!isStarted)
                Init();

#if UNITY_EDITOR
            FOcean.instance.AddPG(this);
#endif
            if (!isStarted)
                return;

			CalcHorizonDir(gwParam.gwDirection, out waveDir.x, out waveDir.y);

			if (gwParam.mode != eFGWMode.eGWM_None)
            {
                float degAngle = gwParam.gwDirection - 90f;
                float fAngleBegin = degAngle - 90f * (2.0f - gwParam.gwFlow);
                float fAngleEnd = degAngle + 90f * (2.0f - gwParam.gwFlow);

                float ar = degAngle;
                float ab = Mathf.Lerp(fAngleBegin, fAngleEnd, 0.25f);
                float ag = Mathf.Lerp(fAngleBegin, fAngleEnd, 0.6f); // a bit strain
                float aa = Mathf.Lerp(fAngleBegin, fAngleEnd, 0.75f);

                Vector4 waveAngle4 = new Vector4(ar, ab, ag, aa) * Mathf.Deg2Rad;

                Vector4 frequency = FOcean.InverseV4(gwParam.gWaveLength * gwParam.gwScale);
                gAmplitude = gwParam.gWamplitude * gwParam.gwScale;

                gSpeed = FOcean.SqrtV4(new Vector4(Dispersion(frequency.x), Dispersion(frequency.y), Dispersion(frequency.z), Dispersion(frequency.w)));
                gDirectionAB = FOcean.CosInvAB(waveAngle4, frequency);
                gDirectionCD = FOcean.CosInvCD(waveAngle4, frequency);

                gSteepness = gwParam.gWChoppiness * Vector4.one * Mathf.Clamp01(gwParam.gwScale);

                //this for visual smooth and network sync, not only physics accurate
                gwParam.gWTime += Time.smoothDeltaTime;

                float scaleTime = (float)gwParam.gWTime * gwParam.gWSpeed;

                double gTimeA = scaleTime * gSpeed.x;
                double gTimeB = scaleTime * gSpeed.y;
                double gTimeC = scaleTime * gSpeed.z;
                double gTimeD = scaleTime * gSpeed.w;

                long cyclesA = (long)(gTimeA / twoPI);
                long cyclesB = (long)(gTimeB / twoPI);
                long cyclesC = (long)(gTimeC / twoPI);
                long cyclesD = (long)(gTimeD / twoPI);

                gTimeA -= cyclesA * twoPI;
                gTimeB -= cyclesB * twoPI;
                gTimeC -= cyclesC * twoPI;
                gTimeD -= cyclesD * twoPI;

                gTime = new Vector4((float)gTimeA, (float)gTimeB, (float)gTimeC, (float)gTimeD);

                gSteepnessAB = new Vector4(gSteepness.x * gAmplitude.x * gDirectionAB.x,
                                gSteepness.x * gAmplitude.x * gDirectionAB.y,
                                gSteepness.y * gAmplitude.y * gDirectionAB.z,
                                gSteepness.y * gAmplitude.y * gDirectionAB.w);

                gSteepnessCD = new Vector4(gSteepness.z * gAmplitude.z * gDirectionCD.x,
                                            gSteepness.z * gAmplitude.z * gDirectionCD.y,
                                            gSteepness.w * gAmplitude.w * gDirectionCD.z,
                                            gSteepness.w * gAmplitude.w * gDirectionCD.w);

                gFreqAB = new Vector4(frequency.x * gAmplitude.x * gDirectionAB.x,
                                    frequency.x * gAmplitude.x * gDirectionAB.y,
                                    frequency.y * gAmplitude.y * gDirectionAB.z,
                                    frequency.y * gAmplitude.y * gDirectionAB.w);

                gFreqCD = new Vector4(frequency.z * gAmplitude.z * gDirectionCD.x,
                                        frequency.z * gAmplitude.z * gDirectionCD.y,
                                        frequency.w * gAmplitude.w * gDirectionCD.z,
                                        frequency.w * gAmplitude.w * gDirectionCD.w);

                //mul frequency
                gDirectionAB = new Vector4(gDirectionAB.x * frequency.x, gDirectionAB.y * frequency.x, gDirectionAB.z * frequency.y, gDirectionAB.w * frequency.y);
                gDirectionCD = new Vector4(gDirectionCD.x * frequency.z, gDirectionCD.y * frequency.z, gDirectionCD.z * frequency.w, gDirectionCD.w * frequency.w);

            }

            if (Camera.main == null)
                return;

            if (FOcean.instance == null)
                return;

#if UNITY_EDITOR
            baseParam.minBias.z = Mathf.Clamp(baseParam.minBias.z, 0.01f, 1f);
            baseParam.minBias.w = Mathf.Max(0, baseParam.minBias.w);
#endif

            CheckParams();

            bSimluateReady = true;
        }


        [NonSerialized]
        public bool bSimluateReady = false;
        
        void Gerstner(out Vector3 offs, out Vector3 nrml, ref Vector3 tileableVtx)
        {
            Vector2 pos = new Vector2(tileableVtx.x, tileableVtx.z);
            GerstnerOffset4(ref pos, out offs);
            pos += new Vector2(offs.x, offs.z);
            nrml = GerstnerNormal4(ref pos);
        }

        public bool GetSurPoint(Vector3 p, out Vector3 sp)
        {
            if(FOcean.instance == null)
            {
                sp = p;
                return false;
            }

            Vector3 worldSpaceVertex = new Vector3(p.x, usedOceanHeight, p.z);

            if ((gwParam.mode & eFGWMode.eGWM_Simulation) == eFGWMode.eGWM_None)
            {
                sp = worldSpaceVertex;
                return true;
            }
            
            Vector2 pos = new Vector2(worldSpaceVertex.x, worldSpaceVertex.z);
            GerstnerOffset4(ref pos, out sp);
            sp += worldSpaceVertex;

            return true;
        }
        
        public bool GetSurPointNormal(Vector3 p, out Vector3 sp, out Vector3 n)
        {
            Vector3 worldSpaceVertex = new Vector3(p.x, usedOceanHeight, p.z);

            if ((gwParam.mode & eFGWMode.eGWM_Simulation) == eFGWMode.eGWM_None)
            {
                sp = worldSpaceVertex;
                n = Vector3.up;
                return true;
            }
            
            Gerstner(out sp, out n, ref worldSpaceVertex);

            sp += worldSpaceVertex;
            return true;
        }

		void CalcHorizonDir(float angle, out float x, out float y)
		{
			angle *= Mathf.Deg2Rad;
			x = -Mathf.Sin(angle);
			y =	Mathf.Cos(angle);
		}

        void DetermineWavesDir(bool bBump)
        {
            float orth = gwParam.gwDirection + Mathf.Lerp(0, 90f, 1f - gwParam.gwFlow);
			Vector2 bumpDir = Vector2.zero;
			CalcHorizonDir(orth, out bumpDir.x, out bumpDir.y);

            //therefore no using waveScale, waves speed (ω)is depend on Dispersion Relations
            Vector4 ftile = oceanMaterial.GetVector("_FoamTiling");
            Vector4 nf = Time.time * new Vector4(-waveDir.x * ftile.z, -waveDir.y * ftile.z, waveDir.y * ftile.w, -waveDir.x * ftile.w);
            if (FOcean.instance.mobile)
                nf = FOcean.FracV4(nf);

            oceanMaterial.SetVector("_FoamDirection", nf);

            if (bBump)
            {
                Vector4 tile = oceanMaterial.GetVector("_BumpTiling");
                Vector4 nb = Time.time * new Vector4(-bumpDir.x * tile.z, -bumpDir.y * tile.z, bumpDir.y * tile.w, -bumpDir.x * tile.w);
                if (FOcean.instance.mobile)
                    nb = FOcean.FracV4(nb);
                oceanMaterial.SetVector("_BumpDirection", nb);
            }
        }

        void DetermineFoam(bool bGWave)
        {
            if (!FOcean.instance.envParam.foamEnabled)
                return;

            float gradientSpeed = oceanMaterial.GetFloat("_FoamGSpeed");
            float foamGTime = Time.time * gradientSpeed;
            if (FOcean.instance.mobile)
                foamGTime = FOcean.Frac(foamGTime);

            Vector4 flow = oceanMaterial.GetVector("_Foam");
            float blendSpeed = oceanMaterial.GetFloat("_FoamBSpeed");
            float foamBTime = Time.time * blendSpeed;

            float foamBlendTime = FOcean.Frac(foamBTime);

            float foamFlow = foamBlendTime;
            float foamFlow2 = FOcean.Frac(foamBTime + 0.5f);

            //normal blend mode
            //foamFlow *= flow.w;
            //foamFlow2 *= flow.w;

            foamFlow = Mathf.Clamp01(Mathf.Abs(foamFlow * 2 - 1f) + 0.25f) * flow.w;
            foamFlow2 = Mathf.Abs(foamFlow2 - 0.5f) * flow.w;

            foamBlendTime = Mathf.Abs(foamBlendTime * 2 - 1);

            foamBlendTime = bGWave ? foamBlendTime : foamBlendTime + 0.5f;
            oceanMaterial.SetVector("_FoamTime", new Vector4(foamGTime, foamBlendTime, foamFlow, foamFlow2));
        }

        // Update is called once per frame
        public void SetupMaterial()
        {
            if (Camera.main == null)
                return;

            if (FOcean.instance == null)
                return;

            if (oceanMaterial == null)
                return;

            if (meshRenderer == null)
                return;

            basePlane = new Plane(Vector3.up, usedOceanHeight * Vector3.up);

            if (baseParam.projectedMesh)
            {
                Matrix4x4 m_Range = Matrix4x4.identity;
                //make new pvMat
                meshRenderer.enabled = GetMinMax(ref m_Range) && renderEnabled;

                if (!meshRenderer.enabled)
                {
                    return;
                }

                Vector2 cornertmp = Vector2.zero;
                foCorners0 = _calculeLocalPosition(ref cornertmp, ref m_Range);
                cornertmp = new Vector2(+1.0f, 0.0f);
                foCorners1 = _calculeLocalPosition(ref cornertmp, ref m_Range);
                cornertmp = new Vector2(0.0f, +1.0f);
                foCorners2 = _calculeLocalPosition(ref cornertmp, ref m_Range);
                cornertmp = new Vector2(+1.0f, +1.0f);
                foCorners3 = _calculeLocalPosition(ref cornertmp, ref m_Range);
            }
            else
            {
                meshRenderer.enabled = CheckMinMax() && renderEnabled;

                if (!meshRenderer.enabled)
                {
                    return;
                }

                offsetToGridPlane = Mathf.Abs(Camera.main.transform.position.y - usedOceanHeight);
            }

            //set material
            
            oceanMaterial.shader.maximumLOD = FOcean.instance.UnderWater() ? FOcean.instance.UnderLOD() : FOcean.instance.AboveLOD();

            DetermineWavesDir(true);

            if (baseParam.projectedMesh)
                oceanMaterial.SetVector("_FoCenter", new Vector4(Camera.main.transform.position.x, 0f, Camera.main.transform.position.z, 0f));

            oceanMaterial.SetVector("_FoCorners0", foCorners0);
            oceanMaterial.SetVector("_FoCorners1", foCorners1);
            oceanMaterial.SetVector("_FoCorners2", foCorners2);
            oceanMaterial.SetVector("_FoCorners3", foCorners3);

            float scale = gwParam.gwScale;
            oceanMaterial.SetTexture("_FresnelLookUp", fresnelLookUp);
            oceanMaterial.SetFloat("_InvFoScale", 1f / scale);

            if ((gwParam.mode & eFGWMode.eGWM_Render) != eFGWMode.eGWM_None)
            {
                oceanMaterial.SetVector("_GAmplitude", gAmplitude);
                oceanMaterial.SetVector("_GSteepness", gSteepness);
                oceanMaterial.SetVector("_GDirectionAB", gDirectionAB);
                oceanMaterial.SetVector("_GDirectionCD", gDirectionCD);
                oceanMaterial.SetVector("_GSteepnessAB", gSteepnessAB);
                oceanMaterial.SetVector("_GSteepnessCD", gSteepnessCD);
                oceanMaterial.SetVector("_GFreqAB", gFreqAB);
                oceanMaterial.SetVector("_GFreqCD", gFreqCD);
                oceanMaterial.SetVector("_FoTime", gTime);

                DetermineFoam(true);
            }
            else
                DetermineFoam(false);

            if (meshRenderer != null)
                meshRenderer.sharedMaterial = oceanMaterial;
        }

        // Check the point of intersection with the plane (0,y,0,0) and return the position in homogenous coordinates 
        Vector4 _calculeLocalPosition(ref Vector2 uv, ref Matrix4x4 m)
        {
            Vector4 ori = new Vector4(uv.x, uv.y, -1, 1);
            Vector4 dir = new Vector4(uv.x, uv.y, 1, 1);
            Vector4 localPos = Vector4.zero;

            ori = m * ori;
            dir = m * dir;
            float wh = ori.w * basePlane.distance;
            dir -= ori;
            float dwh = dir.w * basePlane.distance;
            float l = -(wh + ori.y) / (dir.y + dwh);
            localPos = ori + dir * l;

            return localPos;
        }

        public void ProjectToWorld(out Vector3 vert, float u, float v)
        {
            float _1_u = 1 - u;
            float _1_v = 1 - v;
            vert.x = _1_v * (_1_u * foCorners0.x + u * foCorners1.x) + v * (_1_u * foCorners2.x + u * foCorners3.x);
            vert.y = _1_v * (_1_u * foCorners0.y + u * foCorners1.y) + v * (_1_u * foCorners2.y + u * foCorners3.y);
            vert.z = _1_v * (_1_u * foCorners0.z + u * foCorners1.z) + v * (_1_u * foCorners2.z + u * foCorners3.z);

            float w = _1_v * (_1_u * foCorners0.w + u * foCorners1.w) + v * (_1_u * foCorners2.w + u * foCorners3.w);
            vert /= w;
        }

        [NonSerialized]
        public float offsetToGridPlane = 0.0f;

        Vector3[] frustum = new Vector3[8];
        Vector3[] proj_points = new Vector3[24];
        static int[] cube =
	    {
	        0, 1, 0, 2, 2, 3, 1, 3,
	        0, 4, 2, 6, 3, 7, 1, 5,
	        4, 6, 4, 5, 5, 7, 6, 7
	    };

		private bool GetMinMax(ref Matrix4x4 range)
        {
            if (!projectorCamera)
                return false;

            int i, n_points = 0, src, dst;

            Vector3 testLine;
            float dist;

            if (Application.isPlaying)
            {
                const float yError = -0.00001f;
                if (Camera.main.transform.forward.y == 0.0f) Camera.main.transform.forward += new Vector3(0f, yError, 0f);
            }

            // Set temporal rendering camera parameters
            projectorCamera.CopyFrom(Camera.main);
            projectorCamera.enabled = false;
            projectorCamera.cullingMask = 0;
            projectorCamera.depth = -1;
            projectorCamera.targetTexture = null;

            Vector3 localCamerapos = new Vector3(0f, Camera.main.transform.position.y, 0f);
            projectorCamera.transform.position = localCamerapos;
            this.gameObject.transform.position = new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z);
            this.gameObject.transform.rotation = Quaternion.identity;
            this.gameObject.transform.localScale = Vector3.one;

#if UNITY_EDITOR
            const float editFov = 179.0f; //far close to infi
            if (!Application.isPlaying)
            {
                projectorCamera.transform.forward = -Vector3.up;
                projectorCamera.transform.position += Vector3.up * baseParam.minBias.w;
                projectorCamera.fieldOfView = editFov;
            }
#endif

            float height_in_plane = basePlane.GetDistanceToPoint(projectorCamera.transform.position);
            offsetToGridPlane = Mathf.Abs(height_in_plane);

            Vector3 up = (baseParam.minBias.w + baseParam.oceanHeight) * Vector3.up;
            Vector3 down = (-baseParam.minBias.w + baseParam.oceanHeight) * Vector3.up;
     
			float farPlane;

            if (FOcean.instance.UnderWater())
            {
                Vector3 euler = projectorCamera.transform.eulerAngles;
                projectorCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);

                projectorCamera.transform.position = new Vector3(projectorCamera.transform.position.x, baseParam.oceanHeight * 2f - projectorCamera.transform.position.y, projectorCamera.transform.position.z);
            }

            if (Mathf.Abs(height_in_plane) < baseParam.minBias.w)
            {
                projectorCamera.transform.position += Vector3.up * (baseParam.minBias.w - offsetToGridPlane);

				farPlane = FOcean.instance.UpdateCameraPlane(this, 0);
            }
            else
            {
                float viewLayerRatio = Mathf.Cos(Mathf.Deg2Rad * Camera.main.fieldOfView);
                float viewLayer = offsetToGridPlane / viewLayerRatio;

                projectorCamera.transform.position += Vector3.up * (viewLayer - offsetToGridPlane);

                up = (viewLayer + baseParam.oceanHeight) * Vector3.up;
                down = (-viewLayer + baseParam.oceanHeight) * Vector3.up;
				farPlane = FOcean.instance.UpdateCameraPlane(this, viewLayer + baseParam.oceanHeight);
            }

            projectorCamera.farClipPlane = farPlane * baseParam.minBias.z;

            Plane UpperBoundPlane = new Plane(Vector3.up, up);
            Plane LowerBoundPlane = new Plane(Vector3.up, down);

            Matrix4x4 invviewproj = (projectorCamera.projectionMatrix * projectorCamera.worldToCameraMatrix).inverse;
            //into world space w = 1  
            frustum[0] = invviewproj.MultiplyPoint(new Vector3(-1, -1, -1));
            frustum[1] = invviewproj.MultiplyPoint(new Vector3(+1, -1, -1));
            frustum[2] = invviewproj.MultiplyPoint(new Vector3(-1, +1, -1));
            frustum[3] = invviewproj.MultiplyPoint(new Vector3(+1, +1, -1));
            frustum[4] = invviewproj.MultiplyPoint(new Vector3(-1, -1, +1));
            frustum[5] = invviewproj.MultiplyPoint(new Vector3(+1, -1, +1));
            frustum[6] = invviewproj.MultiplyPoint(new Vector3(-1, +1, +1));
            frustum[7] = invviewproj.MultiplyPoint(new Vector3(+1, +1, +1));
            
            for (i = 0; i < 12; i++)
            {
                src = cube[i * 2];
                dst = cube[i * 2 + 1];
                testLine = frustum[dst] - frustum[src];
                dist = testLine.magnitude;
                testLine.Normalize();
                Ray ray = new Ray(frustum[src], testLine);
                float interactdis = 0.0f;
                bool result = UpperBoundPlane.Raycast(ray, out interactdis);
                if (result && (interactdis < dist + 0.00001))
                {
                    proj_points[n_points++] = frustum[src] + interactdis * testLine;
                }
                result = LowerBoundPlane.Raycast(ray, out interactdis);
                if (result && (interactdis < dist + 0.00001))
                {
                    proj_points[n_points++] = frustum[src] + interactdis * testLine;
                }
            }

            // Check if any of the frustums vertices lie between the upper_bound and lower_bound planes
            for (i = 0; i < 8; i++)
            {
                if (UpperBoundPlane.GetDistanceToPoint(frustum[i]) * LowerBoundPlane.GetDistanceToPoint(frustum[i]) < 0)
                {
                    proj_points[n_points++] = frustum[i];
                }
            }

            if (n_points == 0)
                return false;

            Vector3 aimpoint, aimpoint2;

            // Aim the projector at the point where the camera view-vector intersects the plane
            // if the camera is aimed away from the plane, mirror it's view-vector against the plane
            float forwardy = projectorCamera.transform.forward.y;
            if (forwardy < 0.0f)
            {
                Ray ray = new Ray(projectorCamera.transform.position, projectorCamera.transform.forward);
                float interactdis = 0.0f;
                bool _result = basePlane.Raycast(ray, out interactdis);

                if (false == _result)
                {
                    return false;
                }

                aimpoint = projectorCamera.transform.position + interactdis * projectorCamera.transform.forward;
            }
            else
            {
                Vector3 flipped = projectorCamera.transform.forward -
                    2 * Vector3.up * projectorCamera.transform.forward.y;
                flipped.Normalize();
                float interactdis = 0.0f;
                Ray ray = new Ray(projectorCamera.transform.position, flipped);
                bool _result = basePlane.Raycast(ray, out interactdis);

                if (false == _result)
                {
                    return false;
                }

                aimpoint = projectorCamera.transform.position + interactdis * flipped;
            }

            // Force the point the camera is looking at in a plane, and have the projector look at it
            // works well against horizon, even when camera is looking upwards
            // doesn't work straight down/up
            float af;
            af = Mathf.Abs(Camera.main.transform.forward.y);
            aimpoint2 = localCamerapos + Mathf.Abs(Camera.main.transform.position.y - baseParam.oceanHeight) * Camera.main.transform.forward;

            aimpoint2 -= Vector3.up * (aimpoint2.y - baseParam.oceanHeight);

            // Fade between aimpoint & aimpoint2 depending on view angle
            aimpoint = aimpoint * af + aimpoint2 * (1.0f - af);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                projectorCamera.transform.forward = -Vector3.up;
                projectorCamera.transform.position += Vector3.up * baseParam.minBias.w;
                projectorCamera.fieldOfView = editFov;
            }
            else
                projectorCamera.transform.forward = (aimpoint - projectorCamera.transform.position);
#else 
	        projectorCamera.transform.forward = (aimpoint - projectorCamera.transform.position);
#endif
			Matrix4x4 pvMat = projectorCamera.projectionMatrix * projectorCamera.worldToCameraMatrix;
            for (i = 0; i < n_points; i++)
            {
                // Project the point onto the surface plane
                proj_points[i] -= Vector3.up * (proj_points[i].y - baseParam.oceanHeight);
                proj_points[i] = pvMat.MultiplyPoint(proj_points[i]);
            }

            //x,y minmax coordinates in projectorCamera clip space
            float x_min, y_min, x_max, y_max;

            // Get max/min x & y-values to determine how big the "projection window" must be
            x_min = proj_points[0].x;
            x_max = proj_points[0].x;
            y_min = proj_points[0].y;
            y_max = proj_points[0].y;

            for (i = 1; i < n_points; i++)
            {
                if (proj_points[i].x > x_max) x_max = proj_points[i].x;
                if (proj_points[i].x < x_min) x_min = proj_points[i].x;
                if (proj_points[i].y > y_max) y_max = proj_points[i].y;
                if (proj_points[i].y < y_min) y_min = proj_points[i].y;
            }

            // Build the packing matrix that spreads the grid across the "projection window"

            range.SetRow(0, new Vector4(x_max - x_min, 0, 0, x_min) * baseParam.minBias.x);
            range.SetRow(1, new Vector4(0, y_max - y_min + baseParam.minBias.y, 0, y_min - baseParam.minBias.y));
            range.SetRow(2, new Vector4(0, 0, 1, 0));
            range.SetRow(3, new Vector4(0, 0, 0, 1));

			range = pvMat.inverse * range;

            return true;
        }

        private bool CheckMinMax()
        {
            if (!projectorCamera)
                return false;

            int i, src, dst;

            Vector3 testLine;
            float dist;

            if (Application.isPlaying)
            {
                const float yError = -0.00001f;
                if (Camera.main.transform.forward.y == 0.0f) Camera.main.transform.forward += new Vector3(0f, yError, 0f);
            }

            // Set temporal rendering camera parameters
            projectorCamera.CopyFrom(Camera.main);
            projectorCamera.enabled = false;
            projectorCamera.cullingMask = 0;
            projectorCamera.depth = -1;
            projectorCamera.targetTexture = null;

            Vector3 localCamerapos = new Vector3(0f, Camera.main.transform.position.y, 0f);
            projectorCamera.transform.position = localCamerapos;

#if UNITY_EDITOR
            const float editFov = 179.0f; //far close to infi
            if (!Application.isPlaying)
            {
                projectorCamera.transform.forward = -Vector3.up;
                projectorCamera.transform.position += Vector3.up * baseParam.minBias.w;
                projectorCamera.fieldOfView = editFov;
            }
#endif

            float height_in_plane = basePlane.GetDistanceToPoint(projectorCamera.transform.position);
            offsetToGridPlane = Mathf.Abs(height_in_plane);

            Vector3 up = (baseParam.minBias.w + baseParam.oceanHeight) * Vector3.up;
            Vector3 down = (-baseParam.minBias.w + baseParam.oceanHeight) * Vector3.up;

            float farPlane;

            if (FOcean.instance.UnderWater())
            {
                Vector3 euler = projectorCamera.transform.eulerAngles;
                projectorCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);

                projectorCamera.transform.position = new Vector3(projectorCamera.transform.position.x, baseParam.oceanHeight * 2f - projectorCamera.transform.position.y, projectorCamera.transform.position.z);
            }

            if (Mathf.Abs(height_in_plane) < baseParam.minBias.w)
            {
                projectorCamera.transform.position += Vector3.up * (baseParam.minBias.w - offsetToGridPlane);

                farPlane = FOcean.instance.UpdateCameraPlane(this, 0);
            }
            else
            {
                float viewLayerRatio = Mathf.Cos(Mathf.Deg2Rad * Camera.main.fieldOfView);
                float viewLayer = offsetToGridPlane / viewLayerRatio;

                projectorCamera.transform.position += Vector3.up * (viewLayer - offsetToGridPlane);

                up = (viewLayer + baseParam.oceanHeight) * Vector3.up;
                down = (-viewLayer + baseParam.oceanHeight) * Vector3.up;
                farPlane = FOcean.instance.UpdateCameraPlane(this, viewLayer + baseParam.oceanHeight);
            }

            projectorCamera.farClipPlane = farPlane * baseParam.minBias.z;

            Plane UpperBoundPlane = new Plane(Vector3.up, up);
            Plane LowerBoundPlane = new Plane(Vector3.up, down);

            Matrix4x4 invviewproj = (projectorCamera.projectionMatrix * projectorCamera.worldToCameraMatrix).inverse;
            //into world space w = 1  
            frustum[0] = invviewproj.MultiplyPoint(new Vector3(-1, -1, -1));
            frustum[1] = invviewproj.MultiplyPoint(new Vector3(+1, -1, -1));
            frustum[2] = invviewproj.MultiplyPoint(new Vector3(-1, +1, -1));
            frustum[3] = invviewproj.MultiplyPoint(new Vector3(+1, +1, -1));
            frustum[4] = invviewproj.MultiplyPoint(new Vector3(-1, -1, +1));
            frustum[5] = invviewproj.MultiplyPoint(new Vector3(+1, -1, +1));
            frustum[6] = invviewproj.MultiplyPoint(new Vector3(-1, +1, +1));
            frustum[7] = invviewproj.MultiplyPoint(new Vector3(+1, +1, +1));
            
            for (i = 0; i < 12; i++)
            {
                src = cube[i * 2];
                dst = cube[i * 2 + 1];
                testLine = frustum[dst] - frustum[src];
                dist = testLine.magnitude;
                testLine.Normalize();
                Ray ray = new Ray(frustum[src], testLine);
                float interactdis = 0.0f;
                bool result = UpperBoundPlane.Raycast(ray, out interactdis);
                if (result && (interactdis < dist + 0.00001))
                {
                    return true;
                }
                result = LowerBoundPlane.Raycast(ray, out interactdis);
                if (result && (interactdis < dist + 0.00001))
                {
                    return true;
                }
            }

            // Check if any of the frustums vertices lie between the upper_bound and lower_bound planes
            for (i = 0; i < 8; i++)
            {
                if (UpperBoundPlane.GetDistanceToPoint(frustum[i]) * LowerBoundPlane.GetDistanceToPoint(frustum[i]) < 0)
                {
                    return true;
                }
            }

            return false;
        }

        void GerstnerOffset4(ref Vector2 xzVtx, out Vector3 offsets)
        {
            Vector4 dotABCD = new Vector4(
                Vector2.Dot(new Vector2(gDirectionAB.x, gDirectionAB.y), xzVtx), Vector2.Dot(new Vector2(gDirectionAB.z, gDirectionAB.w), xzVtx),
                    Vector2.Dot(new Vector2(gDirectionCD.x, gDirectionCD.y), xzVtx), Vector2.Dot(new Vector2(gDirectionCD.z, gDirectionCD.w), xzVtx));

            dotABCD += gTime;

            Vector4 COS = FOcean.CosV4(ref dotABCD);
            Vector4 SIN = FOcean.SinV4(ref dotABCD);

            offsets.x = Vector4.Dot(COS, new Vector4(gSteepnessAB.x, gSteepnessAB.z, gSteepnessCD.x, gSteepnessCD.z));
            offsets.z = Vector4.Dot(COS, new Vector4(gSteepnessAB.y, gSteepnessAB.w, gSteepnessCD.y, gSteepnessCD.w));
            offsets.y = Vector4.Dot(SIN, gAmplitude);
        }

        Vector3 GerstnerNormal4(ref Vector2 xzVtx)
        {
            Vector3 nrml = Vector3.up;

            Vector4 dotABCD = new Vector4(
                      Vector2.Dot(new Vector2(gDirectionAB.x, gDirectionAB.y), xzVtx), Vector2.Dot(new Vector2(gDirectionAB.z, gDirectionAB.w), xzVtx),
                          Vector2.Dot(new Vector2(gDirectionCD.x, gDirectionCD.y), xzVtx), Vector2.Dot(new Vector2(gDirectionCD.z, gDirectionCD.w), xzVtx));

            dotABCD += gTime;

            Vector4 COS = FOcean.CosV4(ref dotABCD);

            nrml.x -= Vector4.Dot(COS, new Vector4(gFreqAB.x, gFreqAB.z, gFreqCD.x, gFreqCD.z));
            nrml.z -= Vector4.Dot(COS, new Vector4(gFreqAB.y, gFreqAB.w, gFreqCD.y, gFreqCD.w));

            nrml.Normalize();

            return nrml;
        }

        public bool IsVisible()
        {
            if(meshRenderer == null)
               return false;

            return meshRenderer.enabled;
        }

        public void GenMesh()
        {
            meshFilter = this.GetComponent<MeshFilter>();
            if (meshFilter == null)
                return;

            if (meshFilter.sharedMesh != null)
            {
               DestroyImmediate(meshFilter.sharedMesh);
            }

            meshRenderer = meshFilter.GetComponent<Renderer>();
            meshRenderer.sharedMaterial = oceanMaterial;
#if UNITY_EDITOR
            meshRenderer.hideFlags = HideFlags.HideInInspector;
#endif
            meshRenderer.receiveShadows = false;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
#if UNITY_EDITOR
            meshFilter.hideFlags = HideFlags.HideInInspector;
#endif
            Mesh mesh = new Mesh();
#if UNITY_EDITOR
            mesh.hideFlags = HideFlags.DontSave;
#endif
            gridsize = baseParam.usedGridSize;
            projectedMesh = baseParam.projectedMesh;
            boundSize = baseParam.boundSize;
            skirt = FOcean.instance.envParam.skirt;
            farPlaneBound = Camera.main.farClipPlane;

            if (!projectedMesh)
            {
                this.gameObject.transform.position = baseParam.boundPos;
                this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, baseParam.boundRotate, 0f));
            }

            this.gameObject.transform.localScale = Vector3.one;

            usedOceanHeight = projectedMesh ? baseParam.oceanHeight : baseParam.boundPos.y;

            int numEle = 6 * baseParam.usedGridSize * baseParam.usedGridSize;
            int numVert = (baseParam.usedGridSize + 1) * (baseParam.usedGridSize + 1);

            int[] indices = new int[numEle];

            int i = 0;
            for (int v = 0; v < baseParam.usedGridSize; v++)
            {
                for (int u = 0; u < baseParam.usedGridSize; u++)
                {
                    if (u % 2 == 0)
                    {
                        // face 1 |/
                        indices[i++] = v * (baseParam.usedGridSize + 1) + u;
                        indices[i++] = (v + 1) * (baseParam.usedGridSize + 1) + u;
                        indices[i++] = v * (baseParam.usedGridSize + 1) + u + 1;

                        // face 2 /|
                        indices[i++] = (v + 1) * (baseParam.usedGridSize + 1) + u;
                        indices[i++] = (v + 1) * (baseParam.usedGridSize + 1) + u + 1;
                        indices[i++] = v * (baseParam.usedGridSize + 1) + u + 1;
                    }
                    else
                    {
                        // face 1 |\                                               //
                        indices[i++] = v * (baseParam.usedGridSize + 1) + u;
                        indices[i++] = (v + 1) * (baseParam.usedGridSize + 1) + u + 1;
                        indices[i++] = v * (baseParam.usedGridSize + 1) + u + 1;

                        // face 2 \|
                        indices[i++] = (v + 1) * (baseParam.usedGridSize + 1) + u + 1;
                        indices[i++] = v * (baseParam.usedGridSize + 1) + u;
                        indices[i++] = (v + 1) * (baseParam.usedGridSize + 1) + u;
                    }
                }
            }

            Vector3[] vertices = new Vector3[numVert];

            float du = 1.0f / (baseParam.usedGridSize);
            float dv = 1.0f / (baseParam.usedGridSize);

            float cv = 0.0f;
            for (int v = 0; v < (baseParam.usedGridSize + 1); v++)
            {
                float cu = 0.0f;
                for (int u = 0; u < (baseParam.usedGridSize + 1); u++)
                {
                    i = (baseParam.usedGridSize + 1) * v + u;

                    if (!baseParam.projectedMesh)
                    {
                        vertices[i].x = (cu - 0.5f) * boundSize.x;
                        vertices[i].z = (cv - 0.5f) * boundSize.z;
                    }
                    else
                    {
                        vertices[i].x = cv;
                        vertices[i].z = cu;
                    }
                    
                    if (u < skirt || v < skirt || v > baseParam.usedGridSize - skirt || u > baseParam.usedGridSize - skirt)
                        vertices[i].y = 1;
                    else
                        vertices[i].y = 0;

                    cu += du;
                }
                cv += dv;
            }
            
            mesh.vertices = vertices;
            mesh.triangles = indices;
   
            if (!baseParam.projectedMesh)
                mesh.bounds = new Bounds(Vector3.zero, boundSize);
            else
            {
                float projBound = farPlaneBound;
#if UNITY_EDITOR
                const float maxBound = 100000.0f; //far close to infi
                if (!Application.isPlaying)
                {
                    projBound = maxBound;
                }
#endif
                mesh.bounds = new Bounds(Vector3.zero, projBound * Vector3.one);
            }

            meshFilter.sharedMesh = mesh;
            meshRenderer.sharedMaterial = oceanMaterial;
        }


        const float WAVE_KM = 370.0f;
        const float WAVE_CM = 0.23f;

        float Sqr(float x) { return x * x; }

        //Gravity Wave Dispersion Relations
        //http://graphics.ucsd.edu/courses/rendering/2005/jdewall/tessendorf.pdf
        //ω^2(k) = gk(1 + k^2 * L^2)
        float Dispersion(float k) { return FOcean.g * k * (1.0f + Sqr(k / WAVE_KM)); }


        void CreateFresnelLookUp()
        {
            if (oceanMaterial == null)
            {
                nSnell = -Mathf.Infinity;
                return;
            }

            nSnell = oceanMaterial.GetFloat("_nSnell");
            const int size = 512;
            if (fresnelLookUp == null)
            {
                fresnelLookUp = new Texture2D(size, 1, TextureFormat.Alpha8, false, QualitySettings.activeColorSpace == ColorSpace.Linear);
                fresnelLookUp.filterMode = FilterMode.Bilinear;
                fresnelLookUp.wrapMode = TextureWrapMode.Clamp;
                fresnelLookUp.anisoLevel = 0;
#if UNITY_EDITOR
                fresnelLookUp.hideFlags = HideFlags.DontSave;
#endif
            }

            for (int x = 0; x < size; x++)
            {
                float fresnel = 0.0f;
                float costhetai = (float)x / (float)(size - 1);
                float thetai = Mathf.Acos(costhetai);
                float sinthetat = Mathf.Sin(thetai) / nSnell;
                float thetat = Mathf.Asin(sinthetat);

                if (thetai == 0.0f)
                {
                    fresnel = (nSnell - 1.0f) / (nSnell + 1.0f);
                    fresnel = fresnel * fresnel;
                }
                else
                {
                    float fs = Mathf.Sin(thetat - thetai) / Mathf.Sin(thetat + thetai);
                    float ts = Mathf.Tan(thetat - thetai) / Mathf.Tan(thetat + thetai);
                    fresnel = 0.5f * (fs * fs + ts * ts);
                }

                fresnelLookUp.SetPixel(x, 0, new Color(fresnel, fresnel, fresnel, fresnel));
            }

            fresnelLookUp.Apply();
        }

        public void OnWillRenderObject()
        {
            if (FOcean.instance == null)
                return;

            FOcean.instance.OnWillRender(this, oceanMaterial);
        }

        float tmpCheckTime = 0f;
        public void CheckParams()
        {
            if (!FOcean.instance.isStarted)
                return;

            if (tmpCheckTime >= Time.realtimeSinceStartup)
                return;

            tmpCheckTime = Time.realtimeSinceStartup + Time.fixedDeltaTime * 2.0f;

            if (projectedMesh != baseParam.projectedMesh)
            {
                if(projectedMesh)
                {
                   baseParam.boundPos = new Vector3(baseParam.boundPos.x, baseParam.oceanHeight, baseParam.boundPos.z);
                }
                else
                {
                   baseParam.oceanHeight = baseParam.boundPos.y;
                }

                GenMesh();
                return;
            }

            if (!projectedMesh)
            {
                this.gameObject.transform.position = baseParam.boundPos;
                this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, baseParam.boundRotate, 0f));
            }
            
            this.gameObject.transform.localScale = Vector3.one;

            usedOceanHeight = projectedMesh ? baseParam.oceanHeight : baseParam.boundPos.y;

            if (gridsize != baseParam.usedGridSize)
            {
                GenMesh();
                return;
            }

            if(skirt != FOcean.instance.envParam.skirt)
            {
                GenMesh();
                return;
            }

            if (nSnell != oceanMaterial.GetFloat("_nSnell"))
            {
                ForceReload(true);
                return;
            }

            if (boundSize != baseParam.boundSize)
            {
                if(!baseParam.projectedMesh)
                {
                    GenMesh();
                }

                return;
            }


            if(farPlaneBound != Camera.main.farClipPlane)
            {
                if (baseParam.projectedMesh)
                {
                    GenMesh();
                }

                return;
            }
        }
    }
}
