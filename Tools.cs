using System;
using System.Collections.Generic;
using System.Text;
using static Poliglota_MEF1D.Classes;
using static Poliglota_MEF1D.MathTools;
using static Poliglota_MEF1D.Sel;


namespace Poliglota_MEF1D
{
	class Tools
	{
		//Esta funciÃ³n recibe:
		//- Un flujo de archivo de texto para extraer la informaciÃ³n
		//- La cantidad de lÃ­neas a omitir (puede ser 1 o 2)
		//- La cantidad de filas de informaciÃ³n a extraer
		//- El arreglo de objetos a llenar con la informaciÃ³n extraida
		//La funciÃ³n extrae del archivo de texto los datos de interÃ©s en la malla
		//de acuerdo a los parÃ¡metros enviados.
		private void obtenerDatos(istream file, int nlines, int n, int mode, item[] item_list)
		{
			//Se prepara una variable string para leer las lÃ­neas a omitir
			string line;
			//Siempre se omite al menos una lÃ­nea
			file >> line;
			//Si es necesario omitir una lÃ­nea adicional, se efectÃºa
			if (nlines == DOUBLELINE)
			{
				file >> line;
			}

			//Se itera una cantidad de veces igual a la cantidad de datos a extraer
			//que serÃ¡ igual a la cantidad de objetos a instanciar
			for (int i = 0; i < n; i++)
			{
				//Se verifica la cantidad y tipos de datos a extraer por fila
				switch (mode)
				{
					//Se extrae un entero y un real por fila
					case INT_FLOAT:
						int e;
						float r;
						file >> e >> r;
						//Se instancian el entero y el real del objeto actual
						item_list[i].setIntFloat(e, r);
						break;
					//Se extraen tres enteros
					case INT_INT_INT:
						int e1;
						int e2;
						int e3;
						file >> e1 >> e2 >> e3;
						//Se instancia los tres enteros en el objeto actual
						item_list[i].setIntIntInt(e1, e2, e3);
						break;
				}
			}
		}

		//Esta funciÃ³n recibe:
		//- El objeto mesh con toda la informaciÃ³n de la malla
		//La funciÃ³n solicita el nombre del archivo que contiene la informaciÃ³n de la malla
		//y procede a extraer todos los datos para colocarlos adecuadamente dentro del objeto mesh
		private void leerMallayCondiciones(mesh m)
		{
			//Se prepara un arreglo para el nombre del archivo
			string filename = new string(new char[15]);
			//Se prepara un flujo para el archivo
			ifstream file = new ifstream();
			//Se preparan variables para extraer los parÃ¡metros del problema y las cantidades de
			//datos en la malla (nodos, elementos, condiciones de Dirichlet, condiciones de Neumann)
			float l;
			float k;
			float Q;
			int nnodes;
			int neltos;
			int ndirich;
			int nneu;

			//Se obliga al usuario a ingresar correctamente el nombre del archivo
			do
			{
				Console.Write("Ingrese el nombre del archivo que contiene los datos de la malla: ");
				filename = ConsoleInput.ReadToWhiteSpace(true)[0];
				//Se intenta abrir el archivo
				file.open(filename);
			} while (file == null); //Si no fue posible abrir el archivo, se intenta de nuevo

			//Se leen y guardan los parÃ¡metros y cantidades
			file >> l >> k >> Q;
			file >> nnodes >> neltos >> ndirich >> nneu;

			//Se instancian los parÃ¡metros y cantidades en el objeto mesh
			m.setParameters(l, k, Q);
			m.setSizes(nnodes, neltos, ndirich, nneu);
			//En base a las cantidades, se preparan arreglos de objetos para guardar
			//el resto de la informaciÃ³n
			m.createData();

			//Se extraen, siguiendo el formato del archivo, la informaciÃ³n de:
			//- Los nodos de la malla
			//- Los elementos de la malla
			//- Las condiciones de Dirichlet impuestas
			//- Las condiciones de Neumann impuestas
			obtenerDatos(file, SINGLELINE, nnodes, INT_FLOAT, m.getNodes());
			obtenerDatos(file, DOUBLELINE, neltos, INT_INT_INT, m.getElements());
			obtenerDatos(file, DOUBLELINE, ndirich, INT_FLOAT, m.getDirichlet());
			obtenerDatos(file, DOUBLELINE, nneu, INT_FLOAT, m.getNeumann());

			//Se cierra el archivo antes de terminar
			file.close();
		}
		internal static class ConsoleInput
		{
			private static bool goodLastRead = false;
			public static bool LastReadWasGood
			{
				get
				{
					return goodLastRead;
				}
			}

			public static string ReadToWhiteSpace(bool skipLeadingWhiteSpace)
			{
				string input = "";

				char nextChar;
				while (char.IsWhiteSpace(nextChar = (char)System.Console.Read()))
				{
					//accumulate leading white space if skipLeadingWhiteSpace is false:
					if (!skipLeadingWhiteSpace)
						input += nextChar;
				}
				//the first non white space character:
				input += nextChar;

				//accumulate characters until white space is reached:
				while (!char.IsWhiteSpace(nextChar = (char)System.Console.Read()))
				{
					input += nextChar;
				}

				goodLastRead = input.Length > 0;
				return input;
			}

			public static string ScanfRead(string unwantedSequence = null, int maxFieldLength = -1)
			{
				string input = "";

				char nextChar;
				if (unwantedSequence != null)
				{
					nextChar = '\0';
					for (int charIndex = 0; charIndex < unwantedSequence.Length; charIndex++)
					{
						if (char.IsWhiteSpace(unwantedSequence[charIndex]))
						{
							//ignore all subsequent white space:
							while (char.IsWhiteSpace(nextChar = (char)System.Console.Read()))
							{
							}
						}
						else
						{
							//ensure each character matches the expected character in the sequence:
							nextChar = (char)System.Console.Read();
							if (nextChar != unwantedSequence[charIndex])
								return null;
						}
					}

					input = nextChar.ToString();
					if (maxFieldLength == 1)
						return input;
				}

				while (!char.IsWhiteSpace(nextChar = (char)System.Console.Read()))
				{
					input += nextChar;
					if (maxFieldLength == input.Length)
						return input;
				}

				return input;
			}
		}

	}
}