using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCalc;

public class CodeParsing : MonoBehaviour
{
    public class ObjectListValue
    {
        public string Value { get; set; }
        public string Type { get; set; }
    }
    private const string carOP = "~<>=&|";
    private static int getPosCa()
    {
        return Mathf.RoundToInt(Init.getRobotBehaviour().getRobotPosition().z);
    }
    private static int getPosAv()
    {
        return Mathf.RoundToInt(Init.getRobotBehaviour().getRobotPosition().x);
    }
    private static int HayFlorEnLaEsquina()
    {
        Vector3 pos = Init.getRobotBehaviour().getRobotPosition();
        int i = Init.city[(int)pos.x - 1, (int)pos.z - 1].flowers;
        if (i > 0) return 1;
        else return 0;
    }
    private static int HayFlorEnLaBolsa()
    {
        int i = Init.getRobotBehaviour().flores;
        if (i > 0) return 1;
        else return 0;
    }
    private static int HayPapelEnLaEsquina()
    {
        Vector3 pos = Init.getRobotBehaviour().getRobotPosition();
        int i = Init.city[(int)pos.x - 1, (int)pos.z - 1].papers;
        if (i > 0) return 1;
        else return 0;
    }
    private static int HayPapelEnLaBolsa()
    {
        int i = Init.getRobotBehaviour().papeles;
        if (i > 0) return 1;
        else return 0;
    }
    public static bool checkCodeStructure(string[] codigo,int[] spaces)
    {
        if (!codigo[0].Contains("programa"))
        {
            Debug.Log("Sali en programa");
            UI.runtimeErrorMsg = "1";
            return false;
        }
        if (codigo.Length < 1) return true;
        if ((!codigo[1].Contains("areas"))|| (!codigo[2].Contains("AreaC")))
        {
            Debug.Log("Sali en areas");
            UI.runtimeErrorMsg = "1";
            return false;
        }

        int j = 3;
        //Va a buscar a comenzar o a llegar al final, lo que ocurra primero. Si llega al final, no tendria que estar indentado.
        //Pero si para por comenzar, entraria al for, dado a que se cumple la condicion. Ahi evaluo si esta indentado o no
        while ((codigo[j]!="comenzar") && (j != (codigo.Length - 1)))
        {
            j++;
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
            for (i = (codigo.Length-1); i > 0; i--)
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
    public static void VariableManager(ref int codeLine,ArrayList ar)
    {
        bool found = false;
        int limit = 0;
        int i;
        string aux;
        string[] parame;
        for (i = codeLine+1; i < ar.Count; i++)
        {
            aux = (string)ar[i];
            if (aux.Contains("comenzar"))
            {
                found = true;
                limit = i;
                i = ar.Count;
            }
        }
        if (found == true)
        {
            for(i = codeLine+1;i < limit; i++)
            {
                aux = (string)ar[i];
                aux = aux.Replace(" ", "").Replace(":", " ");
                parame = aux.Split(" "[0]);
                Init.Variables.Add(new Init.VariableR(parame[0], parame[1]));
            }
            codeLine= limit-1;
        }
    }
    public static int checkCondition(string frase)
    {
        int i = 0;
        int wordstart = 0;
        bool startedOp = false;
        List<ObjectListValue> parameterList = new List<ObjectListValue>();
        for (i = 0; i < frase.Length; i++)
        {
            if (frase[i] == '(')
            {
                if (startedOp == true)
                {
                    parameterList.Add(new ObjectListValue { Value = frase[i - 1].ToString(), Type = "Operator" });
                    startedOp = false;
                }
                int fin = frase.LastIndexOf(')');
                string par1 = frase.Substring(i, fin + 1 - i);
                char[] par1aux = par1.ToCharArray();
                par1aux[0] = ' ';
                par1aux[par1.Length - 1] = ' ';
                par1 = new string(par1aux);
                int valor = checkCondition(par1.Replace(" ", ""));
                parameterList.Add(new ObjectListValue { Value = valor.ToString(), Type = "Value" });
                i = frase.LastIndexOf(')');
                wordstart = i;
            }
            else
            {
                if (startedOp == true)
                {
                    if (((frase[i] == '=') && ((frase[i - 1] == '<') || (frase[i - 1] == '>'))) || ((frase[i] == '>') && (frase[i - 1] == '<')))
                    {
                        parameterList.Add(new ObjectListValue { Value = (frase[i - 1] + frase[i]).ToString(), Type = "Operator" });
                    }
                    else
                    {
                        parameterList.Add(new ObjectListValue { Value = frase[i - 1].ToString(), Type = "Operator" });
                        wordstart = i;
                    }
                    startedOp = false;
                }
                else
                {
                    if (carOP.IndexOf(frase[i]) != -1)
                    {
                        startedOp = true;
                        if (i > 0)
                        {
                            if (frase[i - 1] != ')')
                            {
                                string par2 = frase.Substring(wordstart, i - wordstart);
                                parameterList.Add(new ObjectListValue { Value = par2, Type = "Value" });
                            }
                        }
                    }
                }
            }
        }
        if ((i >= frase.Length) && (frase[frase.Length - 1] != ')'))
        {
            string par3 = frase.Substring(wordstart, frase.Length - wordstart);
            parameterList.Add(new ObjectListValue { Value = par3, Type = "Value" });
        }
        List<int> result = new List<int>();
        int j;
        for (int f = 0; f < parameterList.Count; f++)
        {
            if (parameterList[f].Type == "Value")
            {
                if (parameterList[f].Value.Contains("PosCa"))
                    parameterList[f].Value = parameterList[f].Value.Replace("PosCa", getPosCa().ToString());
                if (parameterList[f].Value.Contains("PosAv"))
                    parameterList[f].Value = parameterList[f].Value.Replace("PosAv", getPosAv().ToString());
                if (parameterList[f].Value.Contains("HayFlorEnLaBolsa"))
                    parameterList[f].Value = parameterList[f].Value.Replace("HayFlorEnLaBolsa", HayFlorEnLaBolsa().ToString());
                if (parameterList[f].Value.Contains("HayFlorEnLaEsquina"))
                    parameterList[f].Value = parameterList[f].Value.Replace("HayFlorEnLaEsquina", HayFlorEnLaEsquina().ToString());
                if (parameterList[f].Value.Contains("HayPapelEnLaBolsa"))
                    parameterList[f].Value = parameterList[f].Value.Replace("HayPapelEnLaBolsa", HayPapelEnLaBolsa().ToString());
                if (parameterList[f].Value.Contains("HayPapelEnLaEsquina"))
                    parameterList[f].Value = parameterList[f].Value.Replace("HayPapelEnLaEsquina", HayPapelEnLaEsquina().ToString());
            }
        }
        if (parameterList.Count == 1)
        {
            Expression help1 = new Expression(parameterList[0].Value);
            object help2 = help1.Evaluate();
            if ((int)help2 > 0) result.Add(1);
            else result.Add(0);
        }

        for (j = 0; j < parameterList.Count; j++)
        {
            if (parameterList[j].Type == "Operator")
            {
                object par1;
                object par2;
                Expression par11;
                Expression par22;
                switch (parameterList[j].Value)
                {
                    case ">=":
                        par11 = new Expression(parameterList[j - 1].Value);
                        par1 = par11.Evaluate();
                        par22 = new Expression(parameterList[j + 1].Value);
                        par2 = par22.Evaluate();
                        if ((int)par1 >= (int)par2)
                        {
                            result.Add(1);
                        }
                        else result.Add(0);
                        //Voy a usar NCalc para realizar esto, integrar al proyecto el viernes
                        break;
                    case "<=":
                        par11 = new Expression(parameterList[j - 1].Value);
                        par1 = par11.Evaluate();
                        par22 = new Expression(parameterList[j + 1].Value);
                        par2 = par22.Evaluate();
                        if ((int)par1 >= (int)par2)
                        {
                            result.Add(1);
                        }
                        else result.Add(0);
                        //Voy a usar NCalc para realizar esto, integrar al proyecto el viernes
                        break;
                    case "<":
                        par11 = new Expression(parameterList[j - 1].Value);
                        par1 = par11.Evaluate();
                        par22 = new Expression(parameterList[j + 1].Value);
                        par2 = par22.Evaluate();
                        if ((int)par1 < (int)par2)
                        {
                            result.Add(1);
                        }
                        else result.Add(0);
                        //Voy a usar NCalc para realizar esto, integrar al proyecto el viernes
                        break;
                    case ">":
                        par11 = new Expression(parameterList[j - 1].Value);
                        par1 = par11.Evaluate();
                        par22 = new Expression(parameterList[j + 1].Value);
                        par2 = par22.Evaluate();
                        if ((int)par1 > (int)par2)
                        {
                            result.Add(1);
                        }
                        else result.Add(0);
                        //Voy a usar NCalc para realizar esto, integrar al proyecto el viernes
                        break;
                    case "=":
                        par11 = new Expression(parameterList[j - 1].Value);
                        par1 = par11.Evaluate();
                        par22 = new Expression(parameterList[j + 1].Value);
                        par2 = par22.Evaluate();
                        if ((int)par1 == (int)par2)
                        {
                            result.Add(1);
                        }
                        else result.Add(0);
                        //Voy a usar NCalc para realizar esto, integrar al proyecto el viernes
                        break;
                    case "~":
                        par11 = new Expression(parameterList[j + 1].Value);

                        par1 = par11.Evaluate();
                        if ((int)par1 == 0)
                        {
                            result.Add(1);
                        }
                        else result.Add(0);
                        //Voy a usar NCalc para realizar esto, integrar al proyecto el viernes
                        break;
                    case "|":
                        par11 = new Expression(parameterList[j - 1].Value);
                        par1 = par11.Evaluate();
                        par22 = new Expression(parameterList[j + 1].Value);
                        par2 = par22.Evaluate();
                        if (((int)par1 == 1) || ((int)par2 == 1))
                        {
                            result.Add(1);
                        }
                        else result.Add(0);
                        break;
                    case "&":
                        par11 = new Expression(parameterList[j - 1].Value);
                        par1 = par11.Evaluate();
                        par22 = new Expression(parameterList[j + 1].Value);
                        par2 = par22.Evaluate();
                        if (((int)par1 == 1) && ((int)par2 == 1))
                        {
                            result.Add(1);
                        }
                        else result.Add(0);
                        break;
                    default:
                        //Catch exception, error
                        break;
                }
            }
        }
        if (result.Count > 1)
        {
            int temp = 1;
            for (int k = 0; k < result.Count; k++)
            {
                if (result[k] == 0) temp = 0;
            }
            return temp;
        }
        else return result[0];
    }
}
