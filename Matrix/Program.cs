using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Matrix
{
    class Matrix
    {
        public Matrix(String fileName = "") // podstawowy konstruktor matrycy
        {
            _path = ("../../../../_matryce/" + fileName); // pliki txt z matrycami otwierane sa w folderze _matryce
            _numbers = new List<int>();
            getValues();
            set2Darray();
        }

        private Matrix(int[,] numbers, int rowsCount, int columnsCount) // prwyatny overload konstruktora dla overrideow operatorow gdy nie mamy okreslonej nazwy pliku
        {
            colCount = columnsCount;
            rowCount = rowsCount;
            _numbers2D = new int[rowCount, colCount];
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    _numbers2D[i, j] = numbers[i, j];
                }
            }
        }

        private void getValues() // funkcja okresla ilosc rzedow, kolumn oraz zapisuje do listy liczby zawarte w pliku txt
        {
            string lines = System.IO.File.ReadAllText(_path);
            string[] strArr = lines.Split(new String[] { " ", "\n", "\r\n", "\r" }, StringSplitOptions.None); // podzielenie string'a na array string'a zawierajacy tylko liczby. String dzielony jest przez space oraz przejscie do nowej lini w pliku txt
            int[] intArr = Array.ConvertAll(strArr, str => int.Parse(str)); // konwertowanie array string'a na array int'a
            foreach (var element in intArr)
            {
                _numbers.Add((int)element); // dodanie do listy _numbers liczby z intArr, potrzebne do okreslenia dwuwymiarowego array'a
            }

            rowCount = File.ReadLines(_path).Count(); // okreslenie ilosci rzedow poprzez ilosc lini w pliku txt
            colCount = _numbers.Count / rowCount; // okreslenie ilosci kolumn
        }

        private void set2Darray() // funkcja tworzy dwu wymiarowy array w celu prostszego wykonywania operacji na matrycach
        {
            _numbers2D = new int[rowCount, colCount];
            int k = 0;
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    _numbers2D[i, j] = _numbers[k];
                    k++;
                }
            }
        }

        public void printValues()
        {
            for(int i = 0; i < rowCount; i++)
            {
                for(int j = 0; j < colCount; j++)
                {
                    Console.Write($"{_numbers2D[i, j]} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void transposeMatrix() // transpozycja matrycy
        {
            int temp = colCount;
            colCount = rowCount;
            rowCount = temp;

            int[,] temp2Darray = new int[rowCount, colCount];
            for(int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    temp2Darray[i, j] = _numbers2D[j, i];
                }
            }
            _numbers2D = temp2Darray;
        }

        public override string ToString() // przeciaznie operatora wyswietlania toString()
        {
            string matrixMessage = "\n";
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    matrixMessage += (_numbers2D[i, j] + " ");
                }
                matrixMessage += "\n";
            }
            return matrixMessage;
        }

        public static Matrix operator + (Matrix matrix1, Matrix matrix2) // przeciazenie metody dodawania dwoch matryc
        {
            if (matrix1.rowCount != matrix2.rowCount || matrix1.colCount != matrix2.colCount)
            {
                throw new ArgumentException("Matrixes aren't the same size!"); // handling przypadku gdy rozmiary matryc nie sa takie same
            }
            int tempRowCount = matrix1.rowCount;
            int tempColCount = matrix1.colCount;
            int[,] tempMatrix = new int[matrix1.rowCount, matrix1.colCount];

            for (int i = 0; i < matrix1.rowCount; i++)
            {
                for (int j = 0; j < matrix1.colCount; j++)
                {
                    tempMatrix[i, j] = matrix1._numbers2D[i, j] + matrix2._numbers2D[i, j];
                }
            }
            return new Matrix(tempMatrix, tempRowCount, tempColCount); // uzycie prywatnego konstruktora ktory nie bierze pod uwage sciezki pliku
        }

        public static Matrix operator * (Matrix matrix1, Matrix matrix2) // przeciazenie metody mnozenia w przypadku mnozenia matrycy przez druga matryce
        {
            if (matrix1.colCount != matrix2.rowCount)
            {
                throw new ArgumentException("Column count of first matrix aren't the same as Row count of second matrix!"); // handling przypadku gdy ilosc kolumn 1 matrycy nie jest rowna ilosci rzedow 2 matrycy
            }
            int tempRowCount = matrix1.rowCount;
            int tempColCount = matrix2.colCount;
            int[,] tempMatrix = new int[tempRowCount, tempColCount];
            int x = 0;

            for (int i = 0; i < tempRowCount; i++)
            {
                for (int j = 0; j < tempColCount; j++)
                {
                    for(int k = 0; k < matrix1.colCount; k++)
                    {
                        x += (matrix1._numbers2D[i, k] * matrix2._numbers2D[k, j]);
                    }
                    tempMatrix[i, j] = x;
                    x = 0;
                }
            }

            return new Matrix(tempMatrix, tempRowCount, tempColCount); // uzycie prywatnego konstruktora ktory nie bierze pod uwage sciezki pliku
        }

        public static Matrix operator * (int number, Matrix matrix) // przeciazenie metody mnozenia w przypadku mnozenia liczby przez matryce
        {
            int tempRowCount = matrix.rowCount;
            int tempColCount = matrix.colCount;
            int[,] tempMatrix = new int[tempRowCount, tempColCount];

            for (int i = 0; i < matrix.rowCount; i++)
            {
                for (int j = 0; j < matrix.colCount; j++)
                {
                    tempMatrix[i, j] = number * matrix._numbers2D[i, j];
                }
            }

            return new Matrix(tempMatrix, tempRowCount, tempColCount); // uzycie prywatnego konstruktora ktory nie bierze pod uwage sciezki pliku
        }

        private int colCount, rowCount;
        private String _path;
        private List<int> _numbers;
        private int[,] _numbers2D;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Matrix matrix1 = new Matrix("matrix.txt");
            matrix1.printValues();
            Matrix matrix2 = new Matrix("matrix2.txt");
            matrix2.printValues();

            //matrix1.transposeMatrix();
            //matrix2.transposeMatrix();

            //Matrix matrix3 = matrix1 + matrix2;
            //matrix3.printValues();

            Console.WriteLine("Wynik mnozenia pierwszej matrycy przez 5: " + 5 * matrix1);

            Console.WriteLine("Wynik mnozenia dwoch matryc: " + matrix1 * matrix2);
            


            Console.ReadKey();
        }
    }
}
