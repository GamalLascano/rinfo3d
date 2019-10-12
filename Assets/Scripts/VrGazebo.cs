using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.Events;
public class VrGazebo : MonoBehaviour
{
    public Image imgGaze;
    //Tiempo utilizado para que el timer tome accion
    public float tiempoTotal = 2;
    bool gvrStatus;
    static bool start=true;
    float gvrTimer;
    //Distancia del rayo casteado por la camara
    public int distanceOfRay = 100;
    public UnityEvent GVRClick;
    public Camera wa;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    //Utilizado externamente para el reloj
    public static void RestartClock()
    {
        start = true;
    }
    // Update is called once per frame
    void Update()
    {
        //Si la camara actual es la camara vr
          if (Camera.current == wa)
        {
            // Si ambos booleanos de activacion estan activos, contara el timer
            if (gvrStatus && start)
            {
                gvrTimer += Time.deltaTime;
                imgGaze.fillAmount = gvrTimer / tiempoTotal;
            }

            if (GvrPointerInputModule.CurrentRaycastResult.gameObject != null)
            {
                //Si la camara no esta apuntando a un objeto interactuable en vr, reseteo el timer
                if (GvrPointerInputModule.CurrentRaycastResult.gameObject.CompareTag("Seleccion")==false && GvrPointerInputModule.CurrentRaycastResult.gameObject.CompareTag("Play") == false && GvrPointerInputModule.CurrentRaycastResult.gameObject.CompareTag("Accept") == false)
                {
                    gvrTimer = 0;
                    imgGaze.fillAmount = 0;
                }
                //Si se miro a un objeto por un cierto tiempo determinado...
                if (imgGaze.fillAmount == 1)
                {
                    //Volvera a modo no vr
                    if (GvrPointerInputModule.CurrentRaycastResult.gameObject.CompareTag("Seleccion"))
                    {
                        gvrTimer = 0;
                        imgGaze.fillAmount = 0;
                        GvrPointerInputModule.CurrentRaycastResult.gameObject.gameObject.GetComponent<CambiarVr>().CambiarModo();
                    }
                    else
                    {
                        //Correra la aplicacion en VR
                        if (GvrPointerInputModule.CurrentRaycastResult.gameObject.CompareTag("Play"))
                        {
                            start = false;
                            gvrTimer = 0;
                            imgGaze.fillAmount = 0;
                            UI.getBigBang().GetComponent<UI>().runInVR();
                        }
                        else
                        {
                            //Aceptara un cartel de informar
                            if (GvrPointerInputModule.CurrentRaycastResult.gameObject.CompareTag("Accept"))
                            {
                                start = false;
                                gvrTimer = 0;
                                imgGaze.fillAmount = 0;
                                UI.acceptInforme();
                            }
                        }
                    }
                }
            }
            else
            {
                gvrTimer = 0;
                imgGaze.fillAmount = 0;
            }
        }
    }
    //Modulos no utilizados, pueden ser reemplazados por una solucion utilizando RestartClock()
    /** Empieza el timer para utilizar el reticulo */
    public void GVROn()
    {
        gvrStatus = true;
    }
    /** Para el timer, y resetea el timer */
    public void GVROff()
    {
        gvrStatus = false;
        gvrTimer = 0;
        imgGaze.fillAmount = 0;
    }
}
