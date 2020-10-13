using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;

namespace Modulo1_ProyectoModulo
{
    class Program
    {
        //Hago la conexion con la BBDD
        static string connectionString = ConfigurationManager.ConnectionStrings["Videoclub"].ConnectionString;
        static SqlConnection conexion = new SqlConnection(connectionString);
        static string query;
        static SqlCommand comando;
        static SqlDataReader registros;

        static void Main(string[] args)
        {
            //Llamo al primer menu donde están el login y el register
            MenuLogInRegister();
        }

        //---------------------------METODOS--------------------------

        //En este metodo es un menú donde se selecciona el login o el registro
        public static void MenuLogInRegister()
        {
            Console.Clear();
            Usuario usuario = new Usuario();
            Console.WriteLine("\n------------------------------------------------\n");
            Console.WriteLine("Introduzca qué acción desea realizar: \n\n1.Iniciar sesión \n2.Crear usuario \n3.Cerrar aplicación");
            Console.WriteLine("\n------------------------------------------------\n");
            int userSelection = 0;

            try
            {
                userSelection = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                Program.ErrorEncounter("Opcion no disponible.");
                MakeTime(1500);
                MenuLogInRegister();
            }
           
            switch (userSelection)
            {
                case 1:
                    usuario.LogIn();
                    break;
                case 2:
                    usuario.CreateUser();
                    break;
                case 3:
                    Program.OperationSucces("Cerrando aplicación");
                    MakeTime(2000);
                    break;
                default:
                    Program.ErrorEncounter("Opcion no disponible.");
                    MakeTime(1500);
                    MenuLogInRegister();
                    break;
            }
        }


        //Metodo del menú principal una vez has iniciado sesión por eso le entran tanto la contraseña del usuario como su Email(PK)
        //con la intención de que todo salga en funcion del Email
        public static void MainMenu(string userPass, string userEmail)
        {
            Pelicula pelicula = new Pelicula();
            Alquiler alquiler = new Alquiler();
            Console.WriteLine("\n------------------------------------------------\n");
            Console.WriteLine("Introduzca que acción desea realizar: \n\n1.Alquilar película \n2.Ver películas diponibles \n3.Mis alquileres \n4.Cerrar sesión");
            Console.WriteLine("\n------------------------------------------------\n");
            int userSelection = 0;
            try
            {
                userSelection = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                Program.ErrorEncounter("Opcion no disponible.");
                MakeTime(1500);
                MainMenu(userPass, userEmail);
            }
            switch (userSelection)
            {
                case 1:
                    alquiler.RentMovie(userPass, userEmail);
                    break;
                case 2:
                    pelicula.AvailableMovies(userPass, userEmail);
                    break;
                case 3:
                    alquiler.UserRentedMovies(userPass, userEmail);
                    break;
                case 4:
                    MenuLogInRegister();
                    break;
                default:
                    Program.ErrorEncounter("Opcion no disponible.");
                    MakeTime(1500);
                    MainMenu(userPass, userEmail);
                    break;
            }
        }


        //Metodo que recibe una frase personalizada por parametro y la escribe en rojo
        public static void ErrorEncounter(string frasePersonalizada)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{frasePersonalizada}");
            Console.ForegroundColor = ConsoleColor.Gray;
        }


        //Metodo que recibe una frase personalizada por parametro y la escribe en verde
        public static void OperationSucces(string frasePersonalizada)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n{frasePersonalizada}");
            Console.ForegroundColor = ConsoleColor.Gray;
        }


        //Método donde le entra un int de tiempo en milisegundos y a su vez limpia la consola 
        public static void MakeTime(int time)
        {
            System.Threading.Thread.Sleep(time);
            Console.Clear();
        }
    }
}
