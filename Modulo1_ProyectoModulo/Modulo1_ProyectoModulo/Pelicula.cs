using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Modulo1_ProyectoModulo
{
    class Pelicula
    {
        //Hago la conexion con la BBDD
        static string connectionString = ConfigurationManager.ConnectionStrings["Videoclub"].ConnectionString;
        static SqlConnection conexion = new SqlConnection(connectionString);
        static string query;
        static SqlCommand comando;
        static SqlDataReader registros;

        //Atributos
        public string Titulo { get; set; }
        public string Sinopsis { get; set; }
        public int EdadRecomendada { get; set; }
        public string Estado { get; set; }
        public int Indice { get; set; }

        //Constructores
        public Pelicula()
        {

        }

        //---------------------------METODOS--------------------------

        //Metodo para mostrar toda la lista de películas en base a la edad del usuario
        public void AvailableMovies(string userPass, string userEmail)
        {
            Console.Clear();
            Console.WriteLine("\n------------------------------------------------\n");
            Console.WriteLine("Aquí se pueden ver todas las películas disponible para tu edad");
            Console.WriteLine("\n------------------------------------------------\n");
            //Aquí llamo al metodo CalculateUserYears para calcular la edad del usuari y me la guardo en una variable
            int edadRecomendada = CalculateUserYears(userPass, userEmail);


            //Instncio una lista de peliculas
            List<Pelicula> peliculasList = new List<Pelicula>();
            int index = 0;

            //Llamo a la BBDD para sacar todas la peliculas que tenga la edad recomendada igual o menor a la del usuario y las ordeno 
            //en orden descendiente
            conexion.Open();
            query = $"SELECT * FROM Pelicula WHERE EdadRecomendada <= '{edadRecomendada}' ORDER BY EdadRecomendada DESC";
            comando = new SqlCommand(query, conexion);
            registros = comando.ExecuteReader();
            if (registros.Read())
            {
                do
                {
                    //Si me encuentra algo lo mete en la lista con un indice que se va sumando a medida que se recorre el DoWhile
                    index++;
                    peliculasList.Add(new Pelicula() { Indice = index, Titulo = registros["Titulo"].ToString(), Sinopsis = registros["Sinopsis"].ToString(), EdadRecomendada = Convert.ToInt32(registros["EdadRecomendada"]), Estado = registros["Estado"].ToString() });

                } while (registros.Read());
                conexion.Close();
            }
            else
            {
                //Si no encuntra nada, es que no hay peliculas disponibles para ese usuario
                conexion.Close();
                Console.Clear();
                Program.ErrorEncounter("No se han encontrado peliculas para tí");
                Program.MakeTime(2000);
                Program.MainMenu(userPass, userEmail);
            }

            //Aquí se recorre la lista de peliculas y se va imprimiendo
            foreach (var pelicula in peliculasList)
            {
                Console.WriteLine($"\nIndice: {pelicula.Indice}\nTitulo: {pelicula.Titulo}\nEdad recomendada: {pelicula.EdadRecomendada}\n\n<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>");
            }


            Console.WriteLine("\n\n\n------------------------------------------------\n");
            Console.WriteLine("\nSeleccione el indice de la película para acceder a su informacion:");
            Console.WriteLine("\n------------------------------------------------\n");

            int userSelection = 0;
            try
            {
                //Con esto muestro la pelicula que tiene el mismo indice que ha introducido el usuario
                userSelection = Convert.ToInt32(Console.ReadLine());
                Console.Clear();
                Pelicula obj = peliculasList.Find(c => c.Indice == userSelection);
                Console.WriteLine("\n<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>\n");
                Console.WriteLine($"\nTitulo: {obj.Titulo}\n\nEdad recomendada: {obj.EdadRecomendada}\n\nSinopsis: {obj.Sinopsis}\n\nEstado: {obj.Estado}");
                Console.WriteLine("\n<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>\n");


                //Le pido que le de a enter para continuar una vez ha terminado de leer
                Console.WriteLine("\nEnter para continuar\n");
                Console.ReadLine();
                Program.OperationSucces("Volviendo al menú principal\n");
                Program.MakeTime(2000);
                Program.MainMenu(userPass, userEmail);
            }
            catch
            {
                Console.Clear();
                Program.ErrorEncounter("Error al seleccionar la película");
                Program.MakeTime(3000);
                Program.MainMenu(userPass, userEmail);
            }

        }


        //Metodo para calcular la edad del usuario
        public int CalculateUserYears(string userPass, string userEmail)
        {
            string fechaNacimiento = "";
            //Le pido a la BBDD la información del  usuario con el Email y la contraseña y que me devuelva su fecha de nacimiento
            conexion.Open();
            query = $"SELECT * FROM Usuario WHERE Contraseña = '{userPass}' AND Email = '{userEmail}'";
            comando = new SqlCommand(query, conexion);
            registros = comando.ExecuteReader();
            if (registros.Read())
            {
                //Si encuntra algo guarda la fecha de nacimiento en una variable
                fechaNacimiento = registros["FechaNacimiento"].ToString();
                conexion.Close();
            }
            else
            {
                //Si no encnetra nada dará un error
                conexion.Close();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nHa ocurrido un error");
                Console.ForegroundColor = ConsoleColor.Gray;
                Program.MakeTime(2000);
                Program.MainMenu(userPass, userEmail);
            }

            //Aqui calculo la edad del usuario restando el año actual menos el año de nacimiento del usuario
            int añoNacimiento = Convert.ToInt32(fechaNacimiento.Substring(6, 4));
            int añoActual = Convert.ToInt32(DateTime.Now.ToShortDateString().Substring(6, 4));
            int edadRecomendada = añoActual - añoNacimiento;

            //Me devuelve la edad del usuario
            return edadRecomendada;
        }
    }
}
