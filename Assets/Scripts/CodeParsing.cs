using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeParsing : MonoBehaviour
{
    private const string carOP = "<>=";
    public static bool checkCodeStructure(string[] codigo,int[] spaces)
    {
        if (!codigo[0].Contains("programa"))
        {
            Debug.Log("Sali en programa");
            UI.runtimeErrorMsg = "1";
            return false;
        }
        if (codigo.Length < 1) return true;
        int j = 1;
        //Va a buscar a comenzar o a llegar al final, lo que ocurra primero. Si llega al final, no tendria que estar indentado.
        //Pero si para por comenzar, entraria al for, dado a que se cumple la condicion. Ahi evaluo si esta indentado o no
        while ((codigo[j]!="comenzar") && (j != (codigo.Length - 1)))
        {
            j++;
            Debug.Log("Sali en loop del medio");
        }
        j++;
        int i;
        for (i = j; i < (codigo.Length-1); i++)
        {
            if (spaces[i] < 1){
                if (codigo[i] != "finalizar")
                {
                    Debug.Log("Sali en finalizar 1");
                    Debug.Log(codigo[i]);
                    Debug.Log(spaces[i]);
                    Debug.Log(i);
                    Debug.Log(codigo.Length);
                    UI.runtimeErrorMsg = (i + 1).ToString();
                    return false;
                }
            }
        }
        if (codigo[i] == "finalizar") return true;
        else
        {
            for (i = (codigo.Length-1); i == 0; i--)
            {
                if (codigo[i] != "") 
                {
                    if (codigo[i] != "finalizar")
                    {
                        UI.runtimeErrorMsg = (i+1).ToString();
                        return false;
                    }
                    else return true;
                }
            }
            Debug.Log("Sali en finalizar 2");
            UI.runtimeErrorMsg = (i + 1).ToString();
            return false;
        }
    }
    public static ArrayList parameterProcessor(string info)
    {
        ArrayList parametros= new ArrayList();
        ArrayList operadores = new ArrayList();
        int markParam = 0;
        bool startedOp = false;
        int i = 0;
        for (i = 0; i < info.Length;i++)
        {
            if (startedOp == false)
            {
                if (carOP.IndexOf(info[i]) != -1)
                {
                    startedOp = true;
                }
            }
            else
            {
                startedOp = false;
                if (((info[i] == '=') && ((info[i - 1] == '<') || (info[i - 1] == '>')))||((info[i]=='>')&&(info[i-1]=='<')))
                {
                    operadores.Add(info[i - 1] + info[i]);
                    parametros.Add(info.Substring(markParam, i - 1));
                    markParam = i + 1;
                }
                else
                {
                    operadores.Add(info[i - 1]);
                    parametros.Add(info.Substring(markParam, i));
                    markParam = i;
                }
            }
        }
        parametros.Add(info.Substring(markParam, i));
        ArrayList res = new ArrayList();
        res[0] = parametros;
        res[1] = operadores;
        return res;
    }
}
