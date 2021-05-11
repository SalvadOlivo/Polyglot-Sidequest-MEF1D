using System;
using System.Collections.Generic;
using System.Text;
using static System.Math;
using static Poliglota_MEF1D.Tools;
using static Poliglota_MEF1D.MathTools;
using static Poliglota_MEF1D.Sel;

namespace Poliglota_MEF1D
{
	class Classes
	{

		public enum lines
		{
			NOLINE,
			SINGLELINE,
			DOUBLELINE
		}
		public enum modes
		{
			NOMODE,
			INT_FLOAT,
			INT_INT_INT
		}
		public enum parameters
		{
			ELEMENT_LENGTH,
			THERMAL_CONDUCTIVITY,
			HEAT_SOURCE
		}
		public enum sizes
		{
			NODES,
			ELEMENTS,
			DIRICHLET,
			NEUMANN
		}

		//Clase abstracta que representa un objeto en la malla
		public abstract class item
		{
			protected int id; //identificador
			protected float x; //coordenada en X (basta con este dato por estar en 1 dimensiÃ³n)
			protected int node1; //identificador de nodo
			protected int node2; //segundo identificador de nodo
			protected float value; //valor asociado al objeto
								   //Getters para los atributos
			public int getId()
			{
				return id;
			}

			public float getX()
			{
				return x;
			}

			public int getNode1()
			{
				return node1;
			}

			public int getNode2()
			{
				return node2;
			}

			public float getValue()
			{
				return value;
			}

			//MÃ©todos abstractos para instanciar los atributos de acuerdo a las necesidades

			//Caso en que se utiliza un entero y un real
			public abstract void setIntFloat(int n, float r);

			//Caso en que se utilizan tres enteros
			public abstract void setIntIntInt(int n1, int n2, int n3);

		}

		//Clase que representa cada nodo de la malla
		public class node : item
		{

			//Un nodo usa un entero y un real: su identificador, y su coordenada en X
			public override void setIntFloat(int identifier, float x_coordinate)
			{
				id = identifier;
				x = x_coordinate;
			}

			public override void setIntIntInt(int n1, int n2, int n3)
			{
			}

		}

		//Clase que representa un elemento en la malla
		public class element : item
		{

			public override void setIntFloat(int n1, float r)
			{
			}

			//Un elemento usa tres enteros: su identificador, y los identificadores de sus nodos
			public override void setIntIntInt(int identifier, int firstnode, int secondnode)
			{
				id = identifier;
				node1 = firstnode;
				node2 = secondnode;
			}

		}

		//Clase que representa una condiciÃ³n impuesta en un nodo de la malla
		public class condition : item
		{

			//Una condiciÃ³n usa un entero y un real: un identificador de nodo, y un valor a aplicar
			public override void setIntFloat(int node_to_apply, float prescribed_value)
			{
				node1 = node_to_apply;
				value = prescribed_value;
			}

			public override void setIntIntInt(int n1, int n2, int n3)
			{
			}

		}

		//Clase que representa la malla del problema
		public class mesh
		{
			private float[] parameters = new float[3]; //Para este caso, los valores de l, k y Q
			private int[] sizes = new int[4]; //La cantidad de nodos, elementos, condiciones de dirichlet y neumann
			private node[] node_list; //Arreglo de nodos
			private element[] element_list; //Arreglo de elementos
			private condition[] dirichlet_list; //Arreglo de condiciones de Dirichlet
			private condition[] neumann_list; //Arreglo de condiciones de Neumann
											  //MÃ©todo para instanciar el arreglo de parÃ¡metros, almacenando los
											  //valores de l, k y Q, en ese orden
			public void setParameters(float l, float k, float Q)
			{
				parameters[(int)parameters.ELEMENTS_LENGTH] = 1;
				parameters[(int)parameters.THERMAL_CONDUCTIVITY] = k;
				parameters[(int)parameters.HEAT_SOURCE] = Q;
			}

			//MÃ©todo para instanciar el arreglo de cantidades, almacenando la cantidad
			//de nodos, de elementos, y de condiciones (de Dirichlet y de Neumann)
			public void setSizes(int nnodes, int neltos, int ndirich, int nneu)
			{
				sizes[(int)sizes.NODES] = nnodes;
				sizes[(int)sizes.ELEMENTS] = neltos;
				sizes[(int)sizes.DIRICHLET] = ndirich;
				sizes[(int)sizes.NEUMANN] = nneu;
			}

			//MÃ©todo para obtener una cantidad en particular
			public int getSize(int s)
			{
				return sizes[s];
			}

			//MÃ©todo para obtener un parÃ¡metro en particular
			public float getParameter(int p)
			{
				return parameters[p];
			}

			//MÃ©todo para instanciar los cuatro atributos arreglo, usando
			//las cantidades definidas
			public void createData()
			{
				node_list = Arrays.InitializeWithDefaultInstances<node>(sizes[(int)sizes.NODES]);
				element_list = Arrays.InitializeWithDefaultInstances<element>(sizes[(int)sizes.ELEMENTS]);
				dirichlet_list = Arrays.InitializeWithDefaultInstances<condition>(sizes[(int)sizes.DIRICHLET]);
				neumann_list = Arrays.InitializeWithDefaultInstances<condition>(sizes[(int)sizes.NEUMANN]);
			}

			//Getters para los atributos arreglo
			public node getNodes()
			{
				return node_list;
			}
			public element getElements()
			{
				return element_list;
			}
			public condition getDirichlet()
			{
				return dirichlet_list;
			}
			public condition getNeumann()
			{
				return neumann_list;
			}

			//MÃ©todo para obtener un nodo en particular
			public node getNode(int i)
			{
				// Se determinó que la siguiente línea contiene una llamada al constructor de copia; esto debe verificarse
				//y debe crearse un constructor de copia
				//ORIGINAL: return node_list[i];
				return new node(node_list[i]);
			}

			//MÃ©todo para obtener un elemento en particular
			public element getElement(int i)
			{
				// Se determinó que la siguiente línea contiene una llamada al constructor de copia; esto debe verificarse
				//y debe crearse un constructor de copia
				//ORIGINAL: return element_list[i];
				return new element(element_list[i]);
			}

			//MÃ©todo para obtener una condiciÃ³n en particular
			//(ya sea de Dirichlet o de Neumann)
			public condition getCondition(int i, int type)
			{
				if (type == (int)sizes.DIRICHLET)
				{
					// Se determinó que la siguiente línea contiene una llamada al constructor de copia; esto debe verificarse
					//y debe crearse un constructor de copia
					//ORIGINAL: return dirichlet_list[i];
					return new condition(dirichlet_list[i]);
				}
				else
				{
					// Se determinó que la siguiente línea contiene una llamada al constructor de copia; esto debe verificarse
					//y debe crearse un constructor de copia
					//ORIGINA: return neumann_list[i];
					return new condition(neumann_list[i]);
				}
			}
		}


		internal static class Arrays
		{
			public static T[] InitializeWithDefaultInstances<T>(int length) where T : new()
			{
				T[] array = new T[length];
				for (int i = 0; i < length; i++)
				{
					array[i] = new T();
				}
				return array;
			}

			public static void DeleteArray<T>(T[] array) where T : System.IDisposable
			{
				foreach (T element in array)
				{
					if (element != null)
						element.Dispose();
				}
			}
		}



	}
}
