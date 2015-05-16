using UnityEngine;
using System.Collections;

public abstract class RobotBehaviour : MonoBehaviour {

	// Posicion y cantidad de papeles y flores
	public int posCa   = -1;
	public int posAv   = -1;
	public int heading = -1;
	public int papeles = -1;
	public int flores  = -1;
	
	/**
	 * Desplaza el robot en una posicion hacia adelante segun su heading
	 */
	public abstract IEnumerator mover();

	/**
	 * Realiza una rotacion del robot de 90 grados hacia la derecha sobre su eje
	 */
	public abstract IEnumerator derecha();

}
