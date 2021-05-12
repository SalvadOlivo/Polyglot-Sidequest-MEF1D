using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Numerics;
using static Poliglota_MEF1D.Classes;
using static Poliglota_MEF1D.MathTools;
using static Poliglota_MEF1D.Tools;

namespace Poliglota_MEF1D
{
    class Sel
    {

		//La funciÃ³n recibe:
		//- Una matriz
		//La funciÃ³n muestra en pantalla el contenida de la matrix, fila por fila
		private void showMatrix(Matrix K)
		{
			for (int i = 0; i < K.at(0).size(); i++)
			{
				Console.Write("[\t");
				for (int j = 0; j < K.size(); j++)
				{
					Console.Write(K.at(i).at(j));
					Console.Write("\t");
				}
				Console.Write("]\n");
			}
		}

		//La funciÃ³n recibe un arreglo de matrices, y las muestra
		//en pantalla una por una
		private void showKs(List<Matrix> Ks)
		{
			for (int i = 0; i < Ks.Count; i++)
			{
				Console.Write("K del elemento ");
				Console.Write(i + 1);
				Console.Write(":\n");
				showMatrix(new List<Matrix>(Ks[i]));
				Console.Write("*************************************\n");
			}
		}

		//La funciÃ³n recibe:
		//- Un vector
		//La funciÃ³n asume que recibe un vector columna y muestra su contenido
		//en pantalla en una sola fila
		private void showVector(Vector b)
		{
			Console.Write("[\t");
			for (int i = 0; i < b.size(); i++)
			{
				Console.Write(b.at(i));
				Console.Write("\t");
			}
			Console.Write("]\n");
		}

		//La funciÃ³n recibe un arreglo de vectores, y los muestra
		//en pantalla uno por uno
		private void showbs(List<Vector> bs)
		{
			for (int i = 0; i < bs.Count; i++)
			{
				Console.Write("b del elemento ");
				Console.Write(i + 1);
				Console.Write(":\n");
				showVector(new List<Vector>(bs[i]));
				Console.Write("*************************************\n");
			}
		}

		//La funciÃ³n recibe:
		//- Un elemento
		//- El objeto mesh
		//La funciÃ³n construye la matrix local K para el elemento
		//especificado de acuerdo a la formulaciÃ³n del problema
		private Matrix createLocalK(int element, mesh m)
		{
			//Se prepara la matriz y sus dos filas (se sabe que es una matriz 2x2)
			Matrix K = new Matrix();
			Vector row1 = new Vector();
			Vector row2 = new Vector();

			//De acuerdo a la formulaciÃ³n, la matriz local K tiene la forma:
			//          (k/l)*[ 1 -1 ; -1 1 ]

			//Se extraen del objeto mesh los valores de k y l
			float k = m.getParameter(THERMAL_CONDUCTIVITY);
			float l = m.getParameter(ELEMENT_LENGTH);
			//Se crean las filas
			row1.push_back(k / l);
			row1.push_back(-k / l);
			row2.push_back(-k / l);
			row2.push_back(k / l);
			//Se insertan las filas en la matriz
			K.push_back(row1);
			K.push_back(row2);

			return new Matrix(K);
		}

		//La funciÃ³n recibe:
		//- Un elemento
		//- El objeto mesh
		//La funciÃ³n construye el vector local b para el elemento
		//especificado de acuerdo a la formulaciÃ³n del problema
		private Vector createLocalb(int element, mesh m)
		{
			//Se prepara el vector b (se sabe que serÃ¡ un vector 2x1)
			Vector b = new Vector();

			//Se sabe que el vector local b tendrÃ¡ la forma:
			//          (Q*l/2)*[ 1 ; 1 ]

			//Se extraen del objeto mesh los valores de Q y l
			float Q = m.getParameter(HEAT_SOURCE);
			float l = m.getParameter(ELEMENT_LENGTH);
			//Se insertan los datos en el vector
			b.push_back(Q * l / 2);
			b.push_back(Q * l / 2);

			return new Vector(b);
		}

		//La funciÃ³n recibe:
		//- El objeto mesh
		//- Un arreglo de matrices
		//- Un arreglo de vectores
		//La funciÃ³n construye una K y una b locales para cada elemento de la malla,
		//y los almacena en su arreglo correspondiente
		private void crearSistemasLocales(mesh m, List<Matrix> localKs, List<Vector> localbs)
		{
			//Se recorren los elementos
			for (int i = 0; i < m.getSize(ELEMENTS); i++)
			{
				//Por cada elemento, se crea su K y su b
				localKs.Add(createLocalK(i, m));
				localbs.Add(createLocalb(i, m));
			}
		}

		//La funciÃ³n recibe:
		//- El elemento actual
		//- La matriz local K
		//- La matriz global K
		//La funciÃ³n inserta la K local en la K global de acuerdo a los nodos
		//del elemento
		private void assemblyK(element e, Matrix localK, Matrix K)
		{
			//Se determinan los nodos del elemento actual como los Ã­ndices de la K global
			int index1 = e.getNode1() - 1;
			int index2 = e.getNode2() - 1;

			//Se utilizan los Ã­ndices para definir las celdas de la submatriz
			//a la que se agrega la matriz local del elemento actual
			K.at(index1).at(index1) += localK.at(0).at(0);
			K.at(index1).at(index2) += localK.at(0).at(1);
			K.at(index2).at(index1) += localK.at(1).at(0);
			K.at(index2).at(index2) += localK.at(1).at(1);
		}

		//La funciÃ³n recibe:
		//- El elemento actual
		//- El vector local b
		//- El vector global b
		//La funciÃ³n inserta la b local en la b global de acuerdo a los nodos
		//del elemento
		private void assemblyb(element e, Vector localb, Vector b)
		{
			//Se determinan los nodos del elemento actual como los Ã­ndices de la b global
			int index1 = e.getNode1() - 1;
			int index2 = e.getNode2() - 1;

			//Se utilizan los Ã­ndices para definir las celdas del subvector
			//al que se agrega el vector local del elemento actual
			b.at(index1) += localb.at(0);
			b.at(index2) += localb.at(1);
		}

		//La funciÃ³n recibe:
		//- El objeto mesh
		//- El arreglo de Ks locales
		//- El arreglo de bs locales
		//- La matriz K global
		//- El vector b global
		//La funciÃ³n se encarga de ensamblar adecuadamente todos los sistemas locales en
		//la K y la b globales
		private void ensamblaje(mesh m, List<Matrix> localKs, List<Vector> localbs, Matrix K, Vector b)
		{
			//Se recorren todos los elementos de la malla, uno por uno
			for (int i = 0; i < m.getSize(ELEMENTS); i++)
			{
				//Se extrae del objeto mesh el elemento actual
				element e = m.getElement(i);
				//Se ensamblan la K y la b del elemento actual en las variables globales
				assemblyK(new element(e), new List<Matrix>(localKs[i]), K);
				assemblyb(new element(e), new List<Vector>(localbs[i]), b);
			}
		}

		//La funciÃ³n recibe:
		//- El objeto mesh
		//- El vector b global
		//La funciÃ³n aplica en la b global las condiciones de Neumann en las
		//posiciones que correspondan
		private void applyNeumann(mesh m, Vector b)
		{
			//Se recorren las condiciones de Neumann, una por una
			for (int i = 0; i < m.getSize(NEUMANN); i++)
			{
				//Se extrae la condiciÃ³n de Neumann actual
				condition c = m.getCondition(i, NEUMANN);
				//En la posiciÃ³n de b indicada por el nodo de la condiciÃ³n,
				//se agrega el valor indicado por la condiciÃ³n
				b.at(c.getNode1() - 1) += c.getValue();
			}
		}

		//La funciÃ³n recibe:
		//- El objeto mesh
		//- La matriz K global
		//- El vector b global
		//La funciÃ³n aplica en la K y b globales las condiciones de Dirichlet, eliminando
		//las filas correspondientes, y enviando desde el lado izquierdo del SEL al lado
		//derecho los valores de las columnas correspondientes
		private void applyDirichlet(mesh m, Matrix K, Vector b)
		{
			//Se recorren las condiciones de Dirichlet, una por una
			for (int i = 0; i < m.getSize(DIRICHLET); i++)
			{
				//Se extrae la condiciÃ³n de Dirichlet actual
				condition c = m.getCondition(i, DIRICHLET);
				//Se establece el nodo de la condiciÃ³n como el Ã­ndice
				//para K y b globales donde habrÃ¡ modificaciones
				int index = c.getNode1() - 1;

				//Se elimina la fila correspondiente al nodo de la condiciÃ³n
				K.erase(K.begin() + index); //Se usa un iterator a la posiciÃ³n inicial, y se
				b.erase(b.begin() + index); //le agrega la posiciÃ³n de interÃ©s

				//Se recorren las filas restantes, una por una, de modo que
				//el dato correspondiente en cada fila a la columna del nodo de la
				//condiciÃ³n, se multiplique por el valor de Dirichlet, y se envÃ­e al
				//lado derecho del SEL con su signo cambiado
				for (int row = 0; row < K.size(); row++)
				{
					//Se extrae el valor ubicado en la columna correspondiente
					//al nodo de la condiciÃ³n
					float cell = K.at(row).at(index);
					//Se elimina la columna correspondiente
					//al nodo de la condiciÃ³n
					K.at(row).erase(K.at(row).begin() + index);
					//El valor extraÃ­do se multiplica por el valor de la condiciÃ³n,
					//se le cambia el signo, y se agrega al lado derecho del SEL
					b.at(row) += -1 * c.getValue() * cell;
				}
			}
		}

		//La funciÃ³n recibe:
		//- La matriz K global
		//- El vector b global
		//- El vector T que contendrÃ¡ los valores de las incÃ³gnitas
		//La funciÃ³n se encarga de resolver el SEL del problema
		private void calculate(Matrix K, Vector b, Vector T)
		{
			//Se utiliza lo siguiente:
			//      K*T = b
			// (K^-1)*K*T = (K^-1)*b
			//     I*T = (K^-1)*b
			//      T = (K^-1)*b

			//Se prepara la inversa de K
			Matrix Kinv = new Matrix();
			//Se calcula la inversa de K
			inverseMatrix(K, Kinv);
			//Se multiplica la inversa de K por b, y el resultado se almacena en T
			productMatrixVector(Kinv, b, T);
		}


	}
}
