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
	 * Metodo a implementar
	 */
	public abstract void mover();

	/**
	 * Metodo a implementar
	 */
	public abstract void derecha();

}
