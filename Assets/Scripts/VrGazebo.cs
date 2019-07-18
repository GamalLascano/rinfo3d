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
    float gvrTimer;
    public int distanceOfRay = 100;
    private RaycastHit hitaso;
    public UnityEvent GVRClick;
    public Camera wa;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
          if (XRSettings.enabled == true)
          {
            if (gvrStatus)
            {
                gvrTimer += Time.deltaTime;
                imgGaze.fillAmount = gvrTimer / tiempoTotal;
            }
            Ray ray = wa.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Debug.Log(GvrPointerInputModule.CurrentRaycastResult.gameObject.tag);
            if (GvrPointerInputModule.CurrentRaycastResult.gameObject != null)
            {
                if (imgGaze.fillAmount == 1 && GvrPointerInputModule.CurrentRaycastResult.gameObject.CompareTag("Seleccion"))
                {

                    //GVRClick.Invoke();
                    GvrPointerInputModule.CurrentRaycastResult.gameObject.gameObject.GetComponent<CambiarVr>().CambiarModo();

                    //GameObject.FindGameObjectWithTag("Seleccion").GetComponent<CambiarVr>().CambiarModo();

                }
            }
            if (Physics.Raycast(ray, out hitaso, distanceOfRay))
            {
                Debug.Log("Aaaaaaaaaaaaa");
                if (imgGaze.fillAmount == 1 && hitaso.transform.CompareTag("Seleccion"))
                {
                    
                    //GVRClick.Invoke();
                    hitaso.transform.gameObject.GetComponent<CambiarVr>().CambiarModo();

                    //GameObject.FindGameObjectWithTag("Seleccion").GetComponent<CambiarVr>().CambiarModo();

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
