using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveUI : MonoBehaviour {

    [SerializeField]
    public Vector3 movefrom;
    [SerializeField]
    public Vector3 moveto;

    public bool isui = false;

    private bool _isshowinfor = false;

    // Use this for initialization
    void Start()
    {
        if(isui)
        {
            movefrom = gameObject.GetComponent<RectTransform>().localPosition;
        }
        else
        {
            movefrom = transform.localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            OnClick();
        }

        if(isui)
        {
            Debug.Log(transform.localPosition);
        }
    }

    public void ShowInfor()
    {
        if(isui)
        {
            gameObject.GetComponent<RectTransform>().DOMove(moveto, 2);
        }
        else
        {
            transform.DOLocalMove(moveto, 2);
        }
    }

    public void HideInfor()
    {
        if(isui)
        {
            gameObject.GetComponent<RectTransform>().DOMove(movefrom, 2);
        }
        else
        {
            transform.DOLocalMove(movefrom, 2);
        }
    }

    public void OnClick()
    {
        _isshowinfor = !_isshowinfor;

        if (_isshowinfor)
        {
            ShowInfor();
        }
        else
        {
            HideInfor();
        }
    }

    public void OnClick(Toggle to)
    {
        _isshowinfor = to.isOn;

        if (_isshowinfor)
        {
            ShowInfor();
        }
        else
        {
            HideInfor();
        }
    }
}
