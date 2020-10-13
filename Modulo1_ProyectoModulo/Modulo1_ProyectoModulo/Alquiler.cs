using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Modulo1_ProyectoModulo
{
    class Alquiler
    {
        //Hago la conexion con la BBDD
        static string connectionString = ConfigurationManager.ConnectionStrings["Videoclub"].ConnectionString;
        static SqlConnection conexion = new SqlConnection(connectionString);
        static string query;
        static SqlCommand comando;
        static SqlDataReader registros;

        //Atributos
        public int CodAlquiler { get; set; }
        public string EmailID { get; set; }
        public string TItuloID { get; set; }
        public string FechaAquiler { get; set; }
        public string FechaDevLimite { get; set; }
        public string FechaDevUsuario { get; set; }

        //Constructores
        public Alquiler()
        {

        }

        //---------------------------METODOS--------------------------


        //Metodo para elegir la pelicula que queires alquilar
        public void RentMovie(string userPass, string userEmail)
        {
            Pelicula pelicula = new Pelicula();
            Console.Clear();
            Console.WriteLine("\n------------------------------------------------\n");
            Console.WriteLine("Elige una película para alquilarla:");
            Console.WriteLine("\n------------------------------------------------\n");
            int edadRecomendada = pelicula.CalculateUserYears(userPass, userEmail);

            //Instancio una lista de películas
            List<Pelicula> peliculasListForRent = new List<Pelicula>();
            int index = 0;

            //Llamo a la BBDD para coger la lista de peliculas que esten disponibles y pueda ver el usuario
            conexion.Open();
            query = $"SELECT * FROM Pelicula WHERE Estado = 'Disponible' AND EdadRecomendada < '{edadRecomendada}' ORDER BY EdadRecomendada DESC";
            comando = new SqlCommand(query, conexion);
            registros = comando.ExecuteReader();
            if (registros.Read())
            {
                do
                {
                    //Añado los datos a la lista anteriormente creada junto con un indice
                    index++;
                    peliculasListForRent.Add(new Pelicula() { Indice = index, Titulo = registros["Titulo"].ToString(), Sinopsis = registros["Sinopsis"].ToString(), EdadRecomendada = Convert.ToInt32(registros["EdadRecomendada"]), Estado = registros["Estado"].ToString() });

                } while (registros.Read());
                conexion.Close();
            }
            else
            {
                //Esto ocurre cuando no hay peliculas disponibles
                conexion.Close();
                Console.Clear();
                Program.ErrorEncounter("No se han encontrado peliculas para tí");
                Program.MakeTime(2000);
                Program.MainMenu(userPass, userEmail);
            }
            conexion.Close();

            //Recorro la lista de peliculas y las imprimo 
            foreach (var pelicula2 in peliculasListForRent)
            {
                Console.WriteLine($"\nÍndice: {pelicula2.Indice}\nTitulo: {pelicula2.Titulo}\nEdad recomendada: {pelicula2.EdadRecomendada}\n\n<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>");
            }
            Console.WriteLine("\n\n\n------------------------------------------------\n");
            Console.WriteLine("\nSeleccione el índice de la película para alquilarla:");
            Console.WriteLine("\n------------------------------------------------\n");

            int userSelection = 0;
            try
            {
                //Le pido al usuario que escriba el indice de la película que quiere alquilar
                userSelection = Convert.ToInt32(Console.ReadLine());
                Console.Clear();

                //Muestro la película que ha elegido
                Pelicula obj = peliculasListForRent.Find(c => c.Indice == userSelection);
                Console.WriteLine("\n<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>\n");
                Console.WriteLine($"\nTitulo: {obj.Titulo}\n\nEdad recomendada: {obj.EdadRecomendada}\n\nSinopsis: {obj.Sinopsis}\n\nEstado: {obj.Estado}");
                Console.WriteLine("\n<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>\n");


                //Le pido con confirme la acción
                Console.WriteLine("\n¿Quiere alquilar esta película?\nPresiona S/N para confirmar la operación.\n");
                string userSelectionSN = "";

                userSelectionSN = Console.ReadLine().ToLower();
                if (userSelectionSN == "s")
                {
                    //Si elige que si, actualizo el estado de la pelicula a NO DISPONIBLE
                    conexion.Open();
                    query = $"UPDATE Pelicula SET Estado = 'No disponible' WHERE Titulo = '{obj.Titulo}'";
                    comando = new SqlCommand(query, conexion);
                    comando.ExecuteNonQuery();
                    conexion.Close();

                    //Aquí guardo el día que ha realizado el alquiler y genero la fecha límite(sumando 5 dias a la fecha del alquiler)
                    string todaysDate = DateTime.Now.ToShortDateString();
                    string limitDate = DateTime.Now.AddDays(5).ToShortDateString();


                    //Inserto el registro en la tabla alquiler junto con todos los datos
                    conexion.Open();
                    query = $"INSERT INTO Alquiler(EmailID, TituloID, FechaAlquiler, FechaDevolucionLimite) VALUES ('{userEmail}', '{obj.Titulo}', '{todaysDate}', '{limitDate}')";
                    comando = new SqlCommand(query, conexion);
                    comando.ExecuteNonQuery();
                    conexion.Close();

                    Program.OperationSucces("Operación realizada con éxito.\nVolviendo al menu principal.\n");
                    Program.MakeTime(3000);
                    Program.MainMenu(userPass, userEmail);
                }
                else if (userSelectionSN == "n")
                {
                    //Si elige que NO, se cancela la operacion y vuelve al menu principal
                    Console.Clear();
                    Program.ErrorEncounter("Operacion cancelada.\n");
                    Program.OperationSucces("Volviendo al menu principal.\n");
                    Program.MakeTime(2000);
                    Program.MainMenu(userPass, userEmail);
                }

            }
            catch
            {
                Console.Clear();
                Program.ErrorEncounter("Error al seleccionar la película");
                Program.MakeTime(3000);
                Program.MainMenu(userPass, userEmail);
            }

        }


        //Metodo para mostrar las peñículas que el usuario tiene alquiladas
        public void UserRentedMovies(string userPass, string userEmail)
        {
            List<Pelicula> peliculasAlquiladas = new List<Pelicula>();
            Console.Clear();
            Console.WriteLine("\n------------------------------------------------\n");
            Console.WriteLine("Estas son las películas que tienes alquiladas: ");
            Console.WriteLine("\n------------------------------------------------\n");


            conexion.Open();
            query = $"SELECT * FROM Alquiler WHERE EmailID = '{userEmail}' AND FechaDevolucionUsuario is null ORDER BY FechaDevolucionLimite DESC";
            comando = new SqlCommand(query, conexion);
            registros = comando.ExecuteReader();
            if (registros.Read())
            {
                do
                {

                    DateTime fechaLimite = Convert.ToDateTime(registros["FechaDevolucionLimite"].ToString());
                    int resultado = DateTime.Compare(fechaLimite, DateTime.Now);
                    //Si el primero es menor al segundo resultado < 0, si el primero es igual al segundo implica que resultado = 0
                    //y si el primero es mayor que el segundo resultado > 1

                    if (resultado >= 0)
                    {
                        string codAlquiler = registros["CodAlquiler"].ToString();
                        DateTime fechaAlquiler = Convert.ToDateTime(registros["FechaAlquiler"].ToString());
                        Console.WriteLine("\n<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>\n");
                        Console.WriteLine($"Codigo alquiler: {codAlquiler}");
                        Console.WriteLine($"Título: {registros["TituloID"].ToString()}");
                        Console.WriteLine($"Fecha alquiler: {fechaAlquiler.ToShortDateString()}");
                        Console.WriteLine($"Fecha límite: {fechaLimite.ToShortDateString()}");
                        Console.WriteLine("\n<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>\n");
                    }
                    else
                    {
                        string codAlquiler = registros["CodAlquiler"].ToString();
                        DateTime fechaAlquiler = Convert.ToDateTime(registros["FechaAlquiler"].ToString());
                        Console.WriteLine("\n<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>\n");
                        Console.WriteLine($"Título: {registros["TituloID"].ToString()}");
                        Console.WriteLine($"Fecha alquiler: {fechaAlquiler.ToShortDateString()}");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Fecha límite: {fechaLimite.ToShortDateString()}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("\n<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>\n");
                    }


                } while (registros.Read());
                conexion.Close();
            }
            else
            {
                conexion.Close();
                Console.Clear();
                Program.ErrorEncounter("No se han encontrado películas alquiladas");
                Program.MakeTime(2000);
                Program.MainMenu(userPass, userEmail);
            }

            Console.WriteLine("\n------------------------------------------------\n");
            Console.WriteLine("Seleccione la película que desea devolver:\n(Introduzca el codigo de alquiler)");
            Console.WriteLine("\n------------------------------------------------\n");
            try
            {
                string userSelection = Console.ReadLine();

                conexion.Open();
                query = $"SELECT * FROM Alquiler WHERE CodAlquiler = '{userSelection}' AND FechaDevolucionUsuario is null AND EmailID = '{userEmail}' AND CodAlquiler is not null";
                comando = new SqlCommand(query, conexion);
                registros = comando.ExecuteReader();
                if (registros.Read())
                {
                    conexion.Close();

                    conexion.Open();
                    query = $"UPDATE Alquiler SET FechaDevolucionUsuario = '{DateTime.Now.ToShortDateString()}' WHERE CodAlquiler = '{userSelection}' AND FechaDevolucionUsuario is null AND EmailID = '{userEmail}' AND CodAlquiler is not null";
                    comando = new SqlCommand(query, conexion);
                    comando.ExecuteNonQuery();
                    conexion.Close();

                    conexion.Open();
                    query = $"UPDATE Pelicula SET Estado = 'Disponible' FROM Pelicula FULL OUTER JOIN Alquiler ON Titulo = Alquiler.TituloID WHERE CodAlquiler = '{userSelection}'";
                    comando = new SqlCommand(query, conexion);
                    comando.ExecuteNonQuery();
                    conexion.Close();

                    Program.OperationSucces("Operación realizada con éxito.\nvolviendo al menu principal.");
                    Program.MakeTime(2000);
                    Program.MainMenu(userPass, userEmail);
                }
                else
                {
                    conexion.Close();
                    Console.Clear();
                    Program.ErrorEncounter("Se ha producido un error");
                    Program.MakeTime(2000);
                    Program.MainMenu(userPass, userEmail);
                }
            }
            catch
            {
                conexion.Close();
                Console.Clear();
                Program.ErrorEncounter("Se ha producido un error");
                Program.MakeTime(2000);
                Program.MainMenu(userPass, userEmail);
            }
        }
    }
}
