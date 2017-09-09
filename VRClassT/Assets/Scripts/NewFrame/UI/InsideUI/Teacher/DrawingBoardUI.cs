using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawingBoardUI : OutUIBase {

    public Transform vivecamera;

    public FlowUI flow;

    public Vector3 currentscale;
    public Vector3 origintscale;
    public float adddx;

    public RectTransform rect;

    private void Awake()
    {
        rect = gameObject.GetComponent<RectTransform>();
        origintscale = rect.localScale;
        currentscale = origintscale;
        adddx = origintscale.x * 0.1f;
    }

    // Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //}

    // 初始化界面
    public override void ShowSelf(params System.Object[] args)
    {
        // 设置位置
        //TODO
        SetPostion();

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        Quaternion r = gameObject.GetComponent<RectTransform>().rotation;
        r.x = 0;
        r.z = 0;
        gameObject.GetComponent<RectTransform>().rotation = r;
    }

    public override void HideSelf()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }

        //UIManager.getInstance().ShowFirstUI(true);
    }

    public void CloseSelf()
    {
        OutUiManager.getInstance().ShowUI(OutUiManager.UIList.Teaching);
    }

    public void SetPostion()
    {
        if(flow != null)
        {
            flow.ChangeMove(false);
        }
        
        //Vector3 n = Utility.getInstance().CalculateAheadDistancePoint(vivecamera, 2);
        //transform.position = n;
    }

    public void Bigger()
    {
        currentscale.x += adddx;
        currentscale.y += adddx;
        currentscale.z += adddx;

        rect.localScale = currentscale;
    }

    public void Smaller()
    {
        currentscale.x -= adddx;
        currentscale.y -= adddx;
        currentscale.z -= adddx;

        rect.localScale = currentscale;
    }

}
