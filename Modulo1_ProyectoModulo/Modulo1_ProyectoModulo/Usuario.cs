using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace Modulo1_ProyectoModulo
{
    class Usuario
    {
        //Hago la conexion con la BBDD
        static string connectionString = ConfigurationManager.ConnectionStrings["Videoclub"].ConnectionString;
        static SqlConnection conexion = new SqlConnection(connectionString);
        static string query;
        static SqlCommand comando;
        static SqlDataReader registros;

        //Atributos
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Contraseña { get; set; }
        public string FechaNacimiento { get; set; }

        //Constructores
        public Usuario()
        {

        }
      
        //---------------------------METODOS--------------------------


        //Metodo para iniciar sesión 
        public void LogIn()
        {
            Console.Clear();
            Console.WriteLine("\n------------------------------------------------\n");
            Console.WriteLine("Has seleccionado iniciar sesión.\n\nIntroduce tu email:");
            Console.WriteLine("\n------------------------------------------------\n");
            string userEmail = Console.ReadLine();
            //Llamo a la funcion donde compruebo si el email es correcto y si existe en la BBDD
            bool emailValid = CheckExistingEmail(userEmail);
            if (emailValid == true)
            {
                //Si el email es válido, le pide la contraseña para poder iniciar sesión
                Console.WriteLine("\nIntroduce tu contraseña: ");
                string userPass = Console.ReadLine();

                //Comprueba si la contraseña que escribe coincide con la guardada
                conexion.Open();
                query = $"SELECT * FROM Usuario WHERE Contraseña = '{userPass}' AND Email = '{userEmail}'";
                comando = new SqlCommand(query, conexion);
                registros = comando.ExecuteReader();

                if (registros.Read() == false)
                {
                    //Si no coincide no deja iniciar sesión
                    conexion.Close();
                    Console.Clear();
                    Program.ErrorEncounter("Contraseña incorrecta, por favor, vuelva a intentarlo.");
                    Program.MakeTime(2000);
                    LogIn();
                }
                else
                {
                    //Si coincide, inicia sesión con tu suario y va al menú principal. Desde aquí se pasa por parametros el Email(PK)
                    //y la contraseña.
                    conexion.Close();
                    Program.OperationSucces("Sesión iniciada con éxito.");
                    Program.MakeTime(2000);
                    Program.MainMenu(userPass, userEmail);
                }
                conexion.Close();

            }
            else
            {
                Console.Clear();
                Program.ErrorEncounter("Email incorrecto, por favor, vuelva a intentarlo.");
                Program.MakeTime(2000);
                LogIn();
            }
        }


        //Metodo para crear usuario
        public void CreateUser()
        {
            Console.Clear();
            Console.WriteLine("\n------------------------------------------------\n");
            Console.WriteLine("Has seleccionado crear usuario.\n\nIntroduce el email que deseas utilizar:");
            Console.WriteLine("\n------------------------------------------------\n");
            string userEmail = Console.ReadLine();
            //Compruebo si el email existe en la BBDD y si es válido
            bool EmailValid = CheckValidEmail(userEmail);

            if (EmailValid == true)
            {
                bool correctPass = false;
                try
                {
                    do
                    {
                        //Si no existe en la BBDD y es válido, le pido que cree una contraseña
                        Console.WriteLine("\n------------------------------------------------\n");
                        Console.WriteLine("Introduce tu contraseña:");
                        string userPass = Console.ReadLine();
                        //Le pido que coincida con la introducida anteriormente
                        Console.WriteLine("\nIntroduce de nuevo tu contraseña:");
                        string userPassChecker = Console.ReadLine();
                        if (userPass == userPassChecker)
                        {
                            //Si coincide le pido los datos personales
                            correctPass = true;
                            Console.WriteLine("\nIntroduce tu nombre:");
                            string userName = Console.ReadLine();
                            Console.WriteLine("\nIntroduce tu apellido:");
                            string userLastName = Console.ReadLine();
                            bool birthValid = false;
                            do
                            {
                                Console.WriteLine("\nIntroduce tu fecha de nacimiento: (dd/mm/aaaa)");
                                string userBirth = Console.ReadLine();
                                //Llamo al método que comprueba si el día de nacimiento es un día real
                                birthValid = CheckBirth(userBirth);
                                if (birthValid == true)
                                {
                                    //Si es valido inserto todos los datos a la BBDD
                                    conexion.Open();
                                    query = $"INSERT INTO Usuario(Email, Nombre, Apellido, Contraseña, FechaNacimiento) VALUES ('{userEmail}', '{userName}', '{userLastName}', '{userPass}', '{userBirth}')";
                                    comando = new SqlCommand(query, conexion);
                                    comando.ExecuteNonQuery();
                                    conexion.Close();
                                    Console.WriteLine("\n------------------------------------------------\n");
                                    Program.OperationSucces("Registro realizado con éxito.");
                                    Program.MakeTime(1500);
                                    Program.MenuLogInRegister();
                                }
                                else
                                {
                                    //Si no es válido envío al menú anterior
                                    Console.Clear();
                                    Program.ErrorEncounter("Esa fecha de nacimiento no es válida.");
                                    Program.MakeTime(1500);
                                    Program.MenuLogInRegister();
                                }
                            } while (birthValid == false);
                        }
                        else
                        {
                            //Si no coinciden las contraseñas, le devuelve al menu anterior
                            Console.Clear();
                            Program.ErrorEncounter("Las contraseñas no coinciden, vuelve a intentarlo.");
                            Program.MakeTime(1500);
                        }
                    } while (correctPass == false);
                }
                catch 
                {
                    Console.Clear();
                    Program.ErrorEncounter("Ha ocurrido un error");
                    Program.MakeTime(1500);
                    Program.MenuLogInRegister();
                }
            }
            else
            {
                Console.Clear();
                Program.ErrorEncounter("El email introducido no es valido.");
                Program.MakeTime(1500);
                Program.MenuLogInRegister();
            }

        }


        //Metodo para validar si el Email es valido y si existe en la BBDD (Se utiliza en el inicio de sesión)
        public bool CheckExistingEmail(string email)
        {
            //Se crea una booleana llamada emailValid que se iguala al resultado de una funcion traida por System.ComponentModel.DataAnnotations
            //a la cual le llega el Email introducido por el usuario
            bool emailValid = new EmailAddressAttribute().IsValid(email);
            if (emailValid == true)
            {
                //Si el email es válido abre la conexion con la BBDD para comprobar si existe uno con el mismo nombre
                conexion.Open();
                query = $"SELECT Email FROM Usuario WHERE Email = '{email}'";
                comando = new SqlCommand(query, conexion);
                registros = comando.ExecuteReader();
                if (registros.Read() == false)
                {
                    //Si no encuentra nada no será valido, porque no habrá una cuenta creada que tenga ese Email(PK) asignado, es decir, no tendra usuario
                    conexion.Close();
                    emailValid = false;
                    return emailValid;
                }
                else
                {
                    //Si lo encuntra devuekve true porque si que hay cuenta asociada a ese Email(PK)
                    conexion.Close();
                    emailValid = true;
                    return emailValid;
                }
            }
            else
            {
                //Pero si el email no existe devuelves falso
                emailValid = false;
                return emailValid;
            }
        }


        //Metodo para validar si el Email es valido y si existe en la BBDD (Se utiliza en el registro de usuario)
        public bool CheckValidEmail(string email)
        {
            //Se crea una booleana llamada emailValid que se iguala al resultado de una funcion traida por System.ComponentModel.DataAnnotations
            //a la cual le llega el Email introducido por el usuario
            bool emailValid = new EmailAddressAttribute().IsValid(email);
            if (emailValid == true)
            {
                //Si el email es válido abre la conexion con la BBDD para comprobar si existe uno con el mismo nombre
                conexion.Open();
                query = $"SELECT Email FROM Usuario WHERE Email = '{email}'";
                comando = new SqlCommand(query, conexion);
                registros = comando.ExecuteReader();
                if (registros.Read() == false)
                {
                    //Si no encuentra nada será un email valido, porque no existe en la BBDD un usuario con el mismo Email(PK)
                    conexion.Close();
                    emailValid = true;
                    return emailValid;
                }
                else
                {
                    //Si lo encuntra devuelve falso y no es un Email valido
                    conexion.Close();
                    emailValid = false;
                    return emailValid;
                }
            }
            else
            {
                //Pero si el email no existe devuelves falso
                emailValid = false;
                return emailValid;
            }
        }


        //Metodo que se usa para comprobar si la fecha de nacimiento del usuario es una fecha real
        public bool CheckBirth(string userBirth)
        {
            //En este método se utiliza el Using System.Globalization 
            var dateFormats = new[] { "dd.MM.yyyy", "dd-MM-yyyy", "dd/MM/yyyy" };
            DateTime scheduleDate;
            bool birthValid = DateTime.TryParseExact(
                userBirth,
                dateFormats,
                DateTimeFormatInfo.InvariantInfo,
                DateTimeStyles.None,
                out scheduleDate);


            //Me devuelve si es una fecha real con TRUE
            if (birthValid)
                return birthValid;

            //Me devuelve si es una fecha falsa con FALSE
            else
                return birthValid;

        }


        //------------------------------------METODOS ANTIGUOS-------------------------------------


        //public bool CheckExistingEmailMio(string email)
        //{
        //    bool emailValid = false;
        //    bool emailExist = true;

        //    if (email.Contains("@") && email.EndsWith(".com") || email.EndsWith(".es"))
        //    {
        //        emailValid = true;
        //        conexion.Open();
        //        query = $"SELECT Email FROM Usuario WHERE Email = '{email}'";
        //        comando = new SqlCommand(query, conexion);
        //        registros = comando.ExecuteReader();
        //        if (registros.Read() == false)
        //        {
        //            conexion.Close();
        //            emailExist = false;
        //            return emailExist;
        //        }
        //        else
        //        {
        //            conexion.Close();
        //            emailExist = true;
        //            return emailExist;
        //        }
        //    }
        //    else
        //    {
        //        emailValid = false;
        //        return emailValid;
        //    }
        //}





        //public bool CheckValidEmailMio(string email)
        //{
        //bool emailValid = new EmailAddressAttribute().IsValid(email);
        //if (emailValid == true)
        //{
        //    Console.WriteLine("Este email existe");
        //}
        //else
        //{
        //    Console.WriteLine("Este email NO existe");
        //}
        ////bool emailValid = false;
        //bool emailExist = true;

        //if (email.Contains("@") && email.EndsWith(".com") || email.EndsWith(".es"))
        //{
        //    emailValid = true;
        //    conexion.Open();
        //    query = $"SELECT Email FROM Usuario WHERE Email = '{email}'";
        //    comando = new SqlCommand(query, conexion);
        //    registros = comando.ExecuteReader();
        //    if (registros.Read() == false)
        //    {
        //        conexion.Close();
        //        emailExist = true;
        //        return emailExist;
        //    }
        //    else
        //    {
        //        conexion.Close();
        //        emailExist = false;
        //        return emailExist;
        //    }
        //}
        //else
        //{
        //    emailValid = false;
        //    return emailValid;
        //}
        //}


    }
}
