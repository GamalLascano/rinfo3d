using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeParsing : MonoBehaviour
{
    bool checkCodeStructure(string[] codigo,int[] spaces)
    {
        if (!codigo[0].Contains("programa"))
        {
            return false;
        }
        if (codigo.Length < 1) return true;
        int j = 1;
        //Va a buscar a comenzar o a llegar al final, lo que ocurra primero. Si llega al final, no tendria que estar indentado.
        //Pero si para por comenzar, entraria al for, dado a que se cumple la condicion. Ahi evaluo si esta indentado o no
        while ((codigo[j]!="comenzar") || ((codigo.Length - 1) != j))
        {
            j++;
        }
        for (int i = j; i < codigo.Length; i++)
        {
            if (spaces[j] < 1){
                if (codigo[j]!="finalizar") return false;
            }
        }
        if (codigo[j] == "finalizar") return true;
        else return false;
    }
}
