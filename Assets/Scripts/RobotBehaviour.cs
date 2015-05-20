using UnityEngine;
using System.Collections;

public abstract class RobotBehaviour : MonoBehaviour {

	// Posicion y cantidad de papeles y flores
	public int posCa   = -1;
	public int posAv   = -1;
	public int heading = -1;
	public int papeles = -1;
	public int flores  = -1;

	/** Argumentos a recibir en una operacion dada */
	protected ArrayList arguments = new ArrayList();

	public void resetArguments() {
		arguments.Clear();
	}

	public void addArgument(object anArgument) {
		arguments.Add(anArgument);
	}

	/**
	 * Desplaza el robot en una posicion hacia adelante segun su heading
	 */
	public abstract IEnumerator mover();

	/**
	 * Realiza una rotacion del robot de 90 grados hacia la derecha sobre su eje
	 */
	public abstract IEnumerator Derecha();

	/**
	 * Informa texto en pantalla
	 */
	public abstract IEnumerator Informar();
	
}
