using UnityEngine;
using System.Collections;

public class I18N : MonoBehaviour {
	private static string defaultLang = "en_US";
	private static string gameLang = "";
	
	private static string noTranslationText= "No Translation";
	private static Hashtable langs = new Hashtable
	{
		{"en_US", English.lang},
		{"es_AR", Spanish.lang}
	};

	private static void setUserLanguage(){
		switch(Application.systemLanguage) {
			case SystemLanguage.English: setLang("en_US"); break; 	// en_US: "English (United States)",	
			case SystemLanguage.Spanish: setLang("es_AR"); break; 	// es_AR: "Spanish (Argentina)"}
			default: setLang("en_US"); break;
		};
	}
	
	public static string getCurrentLang(){
		if(gameLang == "")
			setUserLanguage();		
		return gameLang;
	}
	
	public static void setLang(string lang){
		if (langs.ContainsKey (lang))
			gameLang = lang;
		else
			gameLang = defaultLang;
	}
	
	public static string getValue(string key){
		Hashtable lang = (Hashtable) langs[getCurrentLang()];
		string value = (string) lang[key];
		if(value.Length == 0)
			return string.Format(noTranslationText, key);
		else
			return string.Format(value, lang);
	}

	public class English {
		public static Hashtable lang = new Hashtable {
		
			// General
			{"gamename",		"RINFO-3D"	},

			// Menu principal
			{"reset",           "Reset"},
			{"run",             "Run"},
			{"open",			"Open"},
			{"save",			"Save"},
			{"settings",        "Settings"},
			{"edit_title",      "EDITING"},

			// Menu RUN
			{"pause",           "Pause"},
			{"resume",          "Resume"},
			{"stop",            "Stop"},
			{"cam2d",           "Cam:2D"},
			{"camhead",         "Cam:Head"},
			{"cam3d",           "Cam:3D"},
			{"posavenue",       "PosAv"},
			{"posstreet",       "PosSt"},
			{"heading",     	"Heading"},
			{"north",       	"N"},
			{"south",       	"S"},
			{"east",       		"E"},
			{"west",       		"W"},
			{"flowers",       	"Flowers"},
			{"papers",			"Papers"},
			{"speed",			"Speed"},
			{"zoom",			"Zoom"},
			{"exec_line",		"Executing line"},

			// Menu SETTINGS
			{"set_title",       "SETTINGS"},
			{"avenue",          "Avenue"},
			{"street",   		"Street"},
			{"count",         	"Count"},
			{"set",         	"SET!"}

			// Menu OPEN
			// --> to do

			// Menu SAVE
			// --> to do*/
		};
	}

	public class Spanish {
		public static Hashtable lang = new Hashtable{
			
			// General
			{"gamename",		"LIDI-RINFO-3D"	},

			// Menu principal
			{"reset",           "Reiniciar"},
			{"run",             "Ejecutar"},
			{"open",			"Abrir"},
			{"save",			"Guardar"},
			{"settings",        "Configuracion"},
			{"edit_title",      "EDITANDO"},

			
			// Menu RUN
			{"pause",           "Pausar"},
			{"resume",          "Continuar"},
			{"stop",            "Detener"},
			{"cam2d",           "Cam:2D"},
			{"camhead",         "Cam:Head"},
			{"cam3d",           "Cam:3D"},
			{"posavenue",       "PosAv"},
			{"posstreet",       "PosCa"},
			{"heading",     	"Orientacion"},
			{"north",       	"N"},
			{"south",       	"S"},
			{"east",       		"E"},
			{"west",       		"O"},
			{"flowers",       	"Flores"},
			{"papers",			"Papeles"},
			{"speed",			"Velocidad"},
			{"zoom",			"Zoom"},
			{"exec_line",		"Ejecutando linea"},
			
			// Menu SETTINGS
			{"set_title",       "CONFIGURACION"},
			{"avenue",          "Avenida"},
			{"street",   		"Calle"},
			{"count",         	"Cantidad"},
			{"set",         	"SET!"}
			
			// Menu OPEN
			// --> to do
			
			// Menu SAVE
			// --> to do*/
		};
	}
}
