using System;
using System.Collections.Generic;
using System.Text;
using static Poliglota_MEF1D.Classes;
using static Poliglota_MEF1D.Tools;
using static Poliglota_MEF1D.Sel;
using static Poliglota_MEF1D.MathTools;
using System.Numerics;

//LO QUE NOS FALTO:  
//---------------------------------------------------------------------------------+
//No alcanzamos a terminar de pasar las clases Matrix y Vector                     |
//al parecer no hay un equivalente directo en C#                                   |
//encontramos una libreria llamada MathNet que tiene las clases Matrix y Vector    |
//pero usa mas parametros y tiene otros metodos                                    |
//se nos complico implementar la libreria MathNet                                  |
//---------------------------------------------------------------------------------+
// Tampoco encontramos un equivalente de la libreria Ifstream  :(                  |
//---------------------------------------------------------------------------------+


namespace Poliglota_MEF1D
{
    class Main2
    {
        public static int Main()
        {
			List<Matrix> localKs = new List<Matrix>();
			List<Vector> localbs = new List<Vector>();

			//Se preparan tambiÃ©n las variables para la K y la b globales, y una para las incÃ³gnitas de
			//temperatura, que es donde se almacenarÃ¡ la respuesta.
			Matrix K = new Matrix();
			Vector b = new Vector();
			Vector T = new Vector();

			//Se coloca primero una introducciÃ³n con todas las caracterÃ­sticas de la implementaciÃ³n
			Console.Write("IMPLEMENTACIÃ“N DEL MÃ‰TODO DE LOS ELEMENTOS FINITOS\n");
			Console.Write("\t- TRANSFERENCIA DE CALOR\n");
			Console.Write("\t- 1 DIMENSIÃ“N\n");
			Console.Write("\t- FUNCIONES DE FORMA LINEALES\n");
			Console.Write("\t- PESOS DE GALERKIN\n");
			Console.Write("*********************************************************************************\n\n");

			//Se crea un objeto mesh, que contendrÃ¡ toda la informaciÃ³n de la malla
			mesh m = new mesh();
			//Se procede a obtener toda la informaciÃ³n de la malla y almacenarla en m
			leerMallayCondiciones(m);

			//Se procede a crear la K local y la b local de cada elemento, almacenando estas
			//estructuras en los vectores localKs y localbs
			crearSistemasLocales(m, localKs, localbs);
			//Descomentar la siguiente lÃ­nea para observar las Ks y bs creadas
			//showKs(localKs); showbs(localbs);

			//Se inicializan con ceros la K global y la b global
			zeroes(K, m.getSize(NODES));
			zeroes(b, m.getSize(NODES));
			//Se procede al proceso de ensamblaje
			ensamblaje(m, localKs, localbs, K, b);
			//Descomentar la siguiente lÃ­nea para observar las estructuras ensambladas
			//showMatrix(K); showVector(b);

			//Se aplican primero las condiciones de contorno de Neumann
			applyNeumann(m, b);
			//Descomentar la siguiente lÃ­nea para observar los cambios en b
			//showVector(b);

			//Luego se aplican las condiciones de contorno de Dirichlet
			applyDirichlet(m, K, b);
			//Descomentar la siguiente lÃ­nea para observar el SEL final luego
			//de los cambios provocados por Dirichlet
			//showMatrix(K); showVector(b);

			//Se prepara con ceros el vector T que contendrÃ¡ las respuestas
			zeroes(T, b.size());
			//Finalmente, se procede a resolver el SEL
			calculate(K, b, T);

			//Se informa la respuesta:
			Console.Write("La respuesta es: \n");
			showVector(T);

			return 0;
		}


	}

}
