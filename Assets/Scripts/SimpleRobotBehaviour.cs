﻿using UnityEngine;
using System.Collections;

public class SimpleRobotBehaviour : RobotBehaviour {

	/**
	 * Metodo a implementar
	 */
	public override IEnumerator mover()
	{
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando mover!");
		return base.mover();
	}


	/**
	 * Metodo a implementar
	 */
	public override IEnumerator Derecha()
	{
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando derecha!");
		return base.Derecha();
	}	


	/**
	 * Metodo a implementar
	 */
	public override IEnumerator Informar() {
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando informar!");
		return base.Informar ();
	}

}