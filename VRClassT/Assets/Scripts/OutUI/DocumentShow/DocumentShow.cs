using PaintCraft.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DocumentShow : OutUIBase {

    public Transform panelparent;
    public GameObject pptprefab;
    public GameObject writeprefab;
    public GameObject togprefab;

    // 画板
    public GameObject canvas3dplane;
    public Transform canvas3dparent;
    // 笔尖的位置
    public Transform penpoint;

    // 笔 画板控制页面
    public GameObject pen;
    public GameObject penctrl;

    public Transform toggroup;
    //public Dropdown grouplist;

    // 界面相关
    public FileDataListUI filelist;
    // 加号toggle 列表
    [SerializeField]
    public GameObject[] addbtnlist;
    //// toggle 列表
    //[SerializeField]
    //public GameObject[] togglelist;

    private string _currentname = string.Empty;
    private string _write = "Write";
    private string _ppt = "PPT";

    private Vector3 currentscale;
    private Vector3 origintscale;
    private float adddx;

    private RectTransform rect;

    private void Awake()
    {
        rect = gameObject.GetComponent<RectTransform>();
        origintscale = rect.localScale;
        currentscale = origintscale;
        adddx = origintscale.x * 0.1f;
    }

    public void OnClickAddWrite(GameObject drop)
    {
        filelist.HideSelf();

        Dropdown grouplist = drop.GetComponent<Dropdown>();
        // 弹出列表选择界面
        _currentname = grouplist.captionText.text;

        if (_currentname == _write)
        {
            AddWrite();
        }
        else if (_currentname == _write)
        {
            ShowFileList();
        }
    }

    public void ShowFileList()
    {
        if(filelist == null)
        {
            return;
        }

        filelist.ShowSelf(new System.Object[] { ComonEnums.ContentDataType.PPt });

        // 弹出文件列表界面
        if (filelist.clickfilelisten == null)
        {
            filelist.RegisterOpenListen(AddPPt);
        }
    }

    // 点击加号调用的添加app函数
    private void AddPPt(DownLoadItemInfor fileinfor)
    {
        GameObject o = GameObject.Instantiate(pptprefab, panelparent);
        PPtShowUI pptshow = o.GetComponent<PPtShowUI>();
        pptshow.filelist = this.filelist;
        pptshow.ShowSelf(new System.Object[] { fileinfor.filename});

        generateTog(togprefab, toggroup, o, "PPT");
        HideAddBtnList();
    }

    private void AddWrite()
    {
        Debug.Log("添加写字板");
        // 添加画板界面
        GameObject o = GameObject.Instantiate(writeprefab, panelparent);
    //    o.SetActive(false);
        // 添加画板
        Draw3DPad draw = o.GetComponent<Draw3DPad>();
        GameObject c = GameObject.Instantiate(canvas3dplane, canvas3dparent);
        CanvasController cc = c.GetComponent<CanvasController>();
        draw.dc.Canvas = cc;
        //draw.dc.penpoint = this.penpoint;

        draw.pen = this.pen;
        draw.penctrl = this.penctrl;
        draw.ShowPenAndCtrl(true);

        PaintcraftCanvas3DPlane pcp = o.GetComponentInChildren<PaintcraftCanvas3DPlane>();
        pcp.PaintcraftCanvas = cc;
   //     o.SetActive(true);

        generateTog(togprefab, toggroup, o, "Write");
        HideAddBtnList();
    }

    // 生成toggle
    private void generateTog(GameObject tog, Transform group, GameObject target, string togtext)
    {
        GameObject o = GameObject.Instantiate(tog, group);
        Toggle t = o.GetComponent<Toggle>();
        t.GetComponentInChildren<Text>().text = togtext;

        ToggleGroup tg = group.GetComponent<ToggleGroup>();
        t.group = tg;
        t.onValueChanged.AddListener(
            delegate(bool ison)
            {
                target.SetActive(ison);
            }
            );
    }

    // 排序已经存在的加号按钮列表 并且相应隐藏
    public void HideAddBtnList()
    {
        if(addbtnlist == null | addbtnlist.Length <= 0)
        {
            return;
        }

        bool hideone = false;
        for(int i=0;i< addbtnlist.Length;i++)
        {
            if(addbtnlist[i].activeSelf)
            {
                if(!hideone)
                {
                    addbtnlist[i].SetActive(false);
                    hideone = true;
                }
                else
                {
                    addbtnlist[i].transform.SetAsLastSibling();
                }
            }
        }
    }

    // 按钮 监听
    public void OnClickClose()
    {
        filelist.HideSelf();

        if (UserInfor.getInstance().isTeacher)
        {
            ArrayList msg = new ArrayList();
            msg.Add((Int64)CommandDefine.FirstLayer.Lobby);
            msg.Add((Int64)CommandDefine.SecondLayer.OpenPPt);
            msg.Add((Int64)0);
            CommandSend.getInstance().Send((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, msg);

            OutUiManager.getInstance().ShowUI(OutUiManager.UIList.Teaching);
        }else
        {
            OutUiManager.getInstance().ShowUI(OutUiManager.UIList.StudentUI);
        }
    }

    public void OnClickBig()
    {
        filelist.HideSelf();

        //currentscale.x += adddx;
        //currentscale.y += adddx;
        //currentscale.z += adddx;

        //rect.localScale = currentscale;
    }

    public void OnClickSmall()
    {
        filelist.HideSelf();

        //currentscale.x -= adddx;
        //currentscale.y -= adddx;
        //currentscale.z -= adddx;

        //rect.localScale = currentscale;
    }
}
