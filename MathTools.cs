using System;
using System.Collections.Generic;
using static System.Math;
using System.Text;
using static Poliglota_MEF1D.Classes;
using static Poliglota_MEF1D.Tools;
using static Poliglota_MEF1D.Sel;

namespace Poliglota_MEF1D
{
	class MathTools
	{
		//Se define un arreglo de reales como un vector
		//Se define un arreglo de vectores como una matriz

		//La funciÃ³n recibe:
		//- Una matriz (se asume que serÃ¡ cuadrada)
		//- La dimensiÃ³n de la matriz
		//La funciÃ³n crea una matriz cuadrada n x n llena de ceros
		private void zeroes(List<List<float>> M, int n)
		{
			//Se crean n filas
			for (int i = 0; i < n; i++)
			{
				//Se crea una fila de n ceros
				List<float> row = new List<float>(n);
				//Se ingresa la fila en la matriz
				M.Add(row);
			}
		}

		//La funciÃ³n recibe:
		//- Un vector (se asume columna)
		//- La dimensiÃ³n del vector
		//La funciÃ³n crea un vector n x 1 lleno de ceros
		private void zeroes(List<float> v, int n)
		{
			//Se itera n veces
			for (int i = 0; i < n; i++)
			{
				//En cada iteraciÃ³n se agrega un cero al vector
				v.Add(0.0F);
			}
		}

		//La funciÃ³n recibe:
		//- Una matriz
		//- Una matriz que serÃ¡ la copia de la primera
		//La funciÃ³n copiarÃ¡ todo el contenido de la primera matriz en
		//la segunda, respetando las posiciones
		private void copyMatrix(List<List<float>> A, List<List<float>> copy)
		{
			//Se inicializa la copia con ceros
			//asegurÃ¡ndose de sus dimensiones
			zeroes(copy, A.Count);
			//Se recorre la matriz original
			for (int i = 0; i < A.Count; i++)
			{
				for (int j = 0; j < A[0].Count; j++)
				{
					//Se coloca la celda actual de la matriz original
					//en la misma posiciÃ³n dentro de la copia
					copy[i][j] = A[i][j];
				}
			}
		}

		//La funciÃ³n recibe:
		//- Una matriz
		//- Un vector
		//- Un vector para la respuesta
		//La funciÃ³n asume que las dimensiones de la matriz y los vectores son las
		//adecuadas para que la multiplicaciÃ³n sea posible
		private void productMatrixVector(List<List<float>> A, List<float> v, List<float> R)
		{
			//Se aplica bÃ¡sicamente la formulaciÃ³n que puede
			//consultarse en el siguiente enlace (entrar con cuenta UCA):
			//          https://goo.gl/PEzWWe

			//Se itera una cantidad de veces igual al nÃºmero de filas de la matriz
			for (int f = 0; f < A.Count; f++)
			{
				//Se inicia un acumulador
				float cell = 0.0F;
				//Se calcula el valor de la celda de acuerdo a la formulaciÃ³n
				for (int c = 0; c < v.Count; c++)
				{
					cell += A[f][c] * v[c];
				}
				//Se coloca el valor calculado en su celda correspondiente en la respuesta
				R[f] += cell;
			}
		}

		//La funciÃ³n recibe:
		//- Un escalar (valor real)
		//- Una matriz
		//- Una matriz para la respuesta
		//La funciÃ³n multiplica cada uno de los elementos de la matriz por el escalar,
		//ubicando los resultados en la matriz de respuesta
		private void productRealMatrix(float real, List<List<float>> M, List<List<float>> R)
		{
			//Se prepara la matriz de respuesta con las mismas dimensiones de la
			//matriz
			zeroes(R, M.Count);
			//Se recorre la matriz original
			for (int i = 0; i < M.Count; i++)
			{
				for (int j = 0; j < M[0].Count; j++)
				{
					//La celda actual se multiplica por el real, y se almacena
					//el resultado en la matriz de respuesta
					R[i][j] = real * M[i][j];
				}
			}
		}

		//La funciÃ³n recibe:
		//- Una matriz
		//- Un Ã­ndice de fila i
		//- Un Ã­ndice de columna j
		//La funciÃ³n elimina en la matriz la fila i, y la columna j
		private void getMinor(List<List<float>> M, int i, int j)
		{
			//Se elimina la fila i
			M.RemoveAt(i); //Uso de begin para obtener un iterator a la posiciÃ³n de interÃ©s
						   //Se recorren las filas restantes
			for (int i = 0; i < M.Count; i++)
			{
				//En cada fila se elimina la columna j
				//No existe un equivalente directo al método de 'borrar' de vector STL en C #
				M[i].erase(M[i].GetEnumerator() + j);
			}
		}

		//La funciÃ³n recibe:
		//- Una matriz
		//La funciÃ³n calcula el determinante de la matriz de forma recursiva
		private float determinant(List<List<float>> M)
		{
			//Caso trivial: si la matriz solo tiene una celda, ese valor es el determinante
			if (M.Count == 1)
			{
				return M[0][0];
			}
			else
			{


				//Se inicia un acumulador
				float det = 0.0F;
				//Se recorre la primera fila
				for (int i = 0; i < M[0].Count; i++)
				{
					//Se obtiene el menor de la posiciÃ³n actual
					List<List<float>> minor = new List<List<float>>();
					copyMatrix(new List<List<float>>(M), minor);
					getMinor(minor, 0, i);

					//Se calculala contribuciÃ³n de la celda actual al determinante
					//(valor alternante * celda actual * determinante de menor actual)
					det += Math.Pow(-1, i) * M[0][i] * determinant(new List<List<float>>(minor));
				}
				return det;
			}
		}

		//La funciÃ³n recibe:
		//- Una matriz
		//- Una matriz que contendrÃ¡ los cofactores de la primera
		private void cofactors(List<List<float>> M, List<List<float>> Cof)
		{
			//La matriz de cofactores se define asÃ­:
			//(Entrar con cuenta UCA)
			//          https://goo.gl/QK7BZo

			//Se prepara la matriz de cofactores para que sea de las mismas
			//dimensiones de la matriz original
			zeroes(Cof, M.Count);
			//Se recorre la matriz original
			for (int i = 0; i < M.Count; i++)
			{
				for (int j = 0; j < M[0].Count; j++)
				{
					//Se obtiene el menor de la posiciÃ³n actual
					List<List<float>> minor = new List<List<float>>();
					copyMatrix(new List<List<float>>(M), minor);
					getMinor(minor, i, j);
					//Se calcula el cofactor de la posiciÃ³n actual
					//      alternante * determinante del menor de la posiciÃ³n actual
					Cof[i][j] = Math.Pow(-1, i + j) * determinant(new List<List<float>>(minor));
				}
			}
		}

		//La funciÃ³n recibe:
		//- Una matriz
		//- Una matriz que contendrÃ¡ a la primera pero transpuesta
		//La funciÃ³n transpone la primera matriz y almacena el resultado en la segunda
		private void transpose(List<List<float>> M, List<List<float>> T)
		{
			//Se prepara la matriz resultante con las mismas dimensiones
			//de la matriz original
			zeroes(T, M.Count);
			//Se recorre la matriz original
			for (int i = 0; i < M.Count; i++)
			{
				for (int j = 0; j < M[0].Count; j++)
				{
					//La posiciÃ³n actual se almacena en la posiciÃ³n con Ã­ndices
					//invertidos de la matriz resultante
					T[j][i] = M[i][j];
				}
			}
		}

		//La funciÃ³n recibe:
		//- Una matriz
		//- Una matriz que contendrÃ¡ la inversa de la primera matriz
		//La matriz calcula la inversa de la primera matriz, y almacena el resultado
		//en la segunda
		private void inverseMatrix(List<List<float>> M, List<List<float>> Minv)
		{
			//Se utiliza la siguiente fÃ³rmula:
			//      (M^-1) = (1/determinant(M))*Adjunta(M)
			//             Adjunta(M) = transpose(Cofactors(M))

			//Se preparan las matrices para la de cofactores y la adjunta
			List<List<float>> Cof = new List<List<float>>();
			List<List<float>> Adj = new List<List<float>>();
			//Se calcula el determinante de la matriz
			float det = determinant(new List<List<float>>(M));
			//Si el determinante es 0, se aborta el programa
			//No puede dividirse entre 0 (matriz no invertible)
			if (det == 0F)
			{
				Environment.Exit(1);
			}
			//Se calcula la matriz de cofactores
			cofactors(new List<List<float>>(M), Cof);
			//Se calcula la matriz adjunta
			transpose(new List<List<float>>(Cof), Adj);
			//Se aplica la fÃ³rmula para la matriz inversa
			productRealMatrix(1 / det, new List<List<float>>(Adj), Minv);
		}



	}
}
