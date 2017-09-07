using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintCraft.Controllers;
using PaintCraft.Tools;

[RequireComponent(typeof(LineConfig))]
public class Draw3DController : InputController
{
    public Camera SelfCamera;
    public int LineUniqueId = 99442211;
    LineConfig LineConfig;

    void Start()
    {
        LineConfig = GetComponent<LineConfig>();

        InitRay();
    }


    #region implemented abstract members of InputController

    public override bool DontAllowInteraction(Vector2 worldPosition)
    {
        return false;
    }

    #endregion

    PaintcraftCanvas3DPlane paintcraftPlane;
    Vector2 paintcraftCoordinates;
    bool drawingStarted = false;
    void Update()
    {

        if (_isstartdraw)
        {
            if (DoRaycast(out paintcraftPlane, out paintcraftCoordinates))
            {
                drawingStarted = true;
                _isstartdraw = false;
                Canvas = paintcraftPlane.PaintcraftCanvas;
                BeginLine(LineConfig, LineUniqueId, paintcraftCoordinates);
            }
        }
        else if (drawingStarted)
        {
            //Ray ray = SelfCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            _ray.origin = penpoint.position;
            Debug.DrawRay(penpoint.position, _ray.direction, Color.red);
            Vector2 uv = paintcraftPlane.GetUVUsingPlane(_ray);
            Debug.Log(uv + " uv");
            paintcraftCoordinates = GetGlobalPositionFromUV(paintcraftPlane, uv);
            Debug.Log(paintcraftCoordinates + " paintcraftCoordinates");
            if (_isdrawgoing)
            {
                ContinueLine(LineUniqueId, paintcraftCoordinates);
            }
            else
            {
                EndLine(LineUniqueId, paintcraftCoordinates);
                drawingStarted = false;
            }
        }
    }


    bool DoRaycast(out PaintcraftCanvas3DPlane paintcraftPlane, out Vector2 paintcraftCoords)
    {
        paintcraftPlane = null;
        paintcraftCoords = new Vector2();
        //Ray ray = SelfCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        _ray.origin = penpoint.position;
        RaycastHit hitInfo;
        if (Physics.Raycast(_ray, out hitInfo))
        {
            paintcraftPlane = hitInfo.collider.gameObject.GetComponent<PaintcraftCanvas3DPlane>();
            if (paintcraftPlane == null)
            {
                return false;
            }

            Vector2 uv = paintcraftPlane.GetUVUsingPlane(_ray);
            paintcraftCoords = GetGlobalPositionFromUV(paintcraftPlane, uv);
            return true;
        }
        return false;
    }


    Vector2 GetGlobalPositionFromUV(PaintcraftCanvas3DPlane paintcraftPlane, Vector2 uv)
    {
        Vector3 result = paintcraftPlane.PaintcraftCanvas.transform.position;
        result.x -= paintcraftPlane.PaintcraftCanvas.Width / 2;
        result.y -= paintcraftPlane.PaintcraftCanvas.Height / 2;
        result.x += uv.x * paintcraftPlane.PaintcraftCanvas.Width;
        result.y += uv.y * paintcraftPlane.PaintcraftCanvas.Height;
        return result;
    }

    //
    Ray _ray;
    public Transform penpoint;
    public Transform drawboard;
    private bool _isstartdraw = false;
    private bool _isdrawgoing = false;
    public void StartDraw()
    {
        _isstartdraw = true;
        _isdrawgoing = true;
    }

    public void EndDraw()
    {
        _isstartdraw = false;
        _isdrawgoing = false;
    }

    public void InitRay()
    {
        _ray = new Ray(penpoint.position, -drawboard.up);
    }
}
