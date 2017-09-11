using UnityEngine;
using System.Collections;

public class RadialMenuActive : MonoBehaviour
{

    public GameObject radialMenu;
    [Range(0, 1)]
    public float radialScale;

    private Vector3 openedScale;
    private Vector3 closedScale;

    private float openTime;
    private bool opening = false;
    private bool closing = false;

    void Start()
    {
        openedScale = new Vector3(1, 1, 1);
        closedScale = new Vector3(0, 0, 0);
    }

    public void OnOpen()
    {
        radialMenu.SetActive(true);
        openTime = Time.time;
        opening = true;
    }

    public void OnClose()
    {
        openTime = Time.time;
        closing = true;
    }

    void Update()
    {
        if (opening)
        {
            radialMenu.transform.localScale = Vector3.Lerp(closedScale, openedScale, Time.time - openTime);
            if (Time.time - openTime > 1.0f)
            {
                opening = false;
            }
        }
        if (closing)
        {
            radialMenu.transform.localScale = Vector3.Lerp(openedScale, closedScale, Time.time - openTime);
            if (Time.time - openTime > 1.0f)
            {
                closing = false;
                radialMenu.SetActive(false);
            }
        }
    }
}
