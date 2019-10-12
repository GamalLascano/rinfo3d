using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CambiarVr : MonoBehaviour
{
    //Usado para conectar a un objeto para desactivar modo vr
    public void CambiarModo()
    {
        UI.desactivarVR();
    }

}
