using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CambiarVr : MonoBehaviour
{
   // int Modo = 1;
    public void CambiarModo()
    {
        UI.desactivarVR();
        //if ((PlayerPrefs.GetInt("Vr Mode")) == 1)
        //{
        //    Modo = 0;
        //}
        //if ((PlayerPrefs.GetInt("Vr Mode")) == 0)
        //{
        //    Modo = 1;
        //}
        //PlayerPrefs.SetInt("Vr Mode", Modo);
    }
    //void Update()
    //{

    //    if(PlayerPrefs.GetInt("Vr Mode") == 0)
    //    {
    //        XRSettings.enabled = true;
    //    }
    //    if (PlayerPrefs.GetInt("Vr Mode") == 1)
    //    {
    //        XRSettings.enabled = false;
    //    }
    //}
}
