using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.Events;
public class VrGazebo : MonoBehaviour
{
    public Image imgGaze;
    public float tiempoTotal = 2;
    bool gvrStatus;
    static bool start=true;
    float gvrTimer;
    public int distanceOfRay = 100;
    public UnityEvent GVRClick;
    public Camera wa;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public static void RestartClock()
    {
        start = true;
    }
    // Update is called once per frame
    void Update()
    {
          if (Camera.current == wa)
        {
            if (gvrStatus && start)
            {
                gvrTimer += Time.deltaTime;
                imgGaze.fillAmount = gvrTimer / tiempoTotal;
            }

            if (GvrPointerInputModule.CurrentRaycastResult.gameObject != null)
            {
                //Debug.Log(GvrPointerInputModule.CurrentRaycastResult.gameObject.tag);
                if (GvrPointerInputModule.CurrentRaycastResult.gameObject.CompareTag("Seleccion")==false && GvrPointerInputModule.CurrentRaycastResult.gameObject.CompareTag("Play") == false)
                {
                    gvrTimer = 0;
                    imgGaze.fillAmount = 0;
                }
                if (imgGaze.fillAmount == 1)
                {
                    if (GvrPointerInputModule.CurrentRaycastResult.gameObject.CompareTag("Seleccion"))
                    {
                        gvrTimer = 0;
                        imgGaze.fillAmount = 0;
                        GvrPointerInputModule.CurrentRaycastResult.gameObject.gameObject.GetComponent<CambiarVr>().CambiarModo();
                    }
                    else
                    {
                        if (GvrPointerInputModule.CurrentRaycastResult.gameObject.CompareTag("Play"))
                        {
                            start = false;
                            gvrTimer = 0;
                            imgGaze.fillAmount = 0;
                            UI.getBigBang().GetComponent<UI>().runInVR();
                        }
                    }
                }
            }
        }
    }
    public void GVROn()
    {
        gvrStatus = true;
    }
    public void GVROff()
    {
        gvrStatus = false;
        gvrTimer = 0;
        imgGaze.fillAmount = 0;
    }
}
