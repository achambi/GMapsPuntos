using System;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Auditoria;

namespace DataAccess
{
    #region Clase: ObtenerConexion donde se crean cadenas de conexion a base de datos.
    /// <summary>
    /// Clase para armar una cadena de conexion a partir de los datos que se requiere
    /// </summary>    
    public class GetConexion
    {
        /// <summary>
        /// Funcion para armar una cadena de conexion a SQL
        /// </summary>
        /// <param name="Server">Nombre del servidor de base de datos</param>
        /// <param name="BaseDatos">Nombre de la base de datos</param>
        /// <param name="Usuario">Usuario de la basde de datos</param>
        /// <param name="Password">Password del usuario de base de datos</param>
        public static string ConexionSQL(string Server, string BaseDatos, string Usuario, string Password)
        {
            SqlConnectionStringBuilder conguardar = new SqlConnectionStringBuilder("Server= " + Server + "; DataBase=" + BaseDatos + " ; User Id=" + Usuario + ";Password=" + Password + ";");
            return conguardar.ConnectionString;
        }

        /// <summary>
        /// Funcion para armar una cadena de conexion a SQL con Windows Autentication
        /// </summary>
        /// <param name="Server">Nombre del servidor de base de datos</param>
        /// <param name="BaseDatos">Nombre de la base de datos</param>
        /// <param name="Usuario">Usuario de la basde de datos</param>
        /// <param name="Password">Password del usuario de base de datos</param>
        public static string ConexionSQL(string Server, string BaseDatos)
        {
            SqlConnectionStringBuilder conguardar = new SqlConnectionStringBuilder("Data Source=" + Server + ";Initial Catalog=" + BaseDatos + ";Integrated Security=True");
            return conguardar.ConnectionString;
        }

        /// <summary>
        /// Funcion para armar una cadena de conexion Sybase, utilizando un DNS
        /// </summary>
        /// <param name="DSN">Nombre del DSN a invocar para la conexion ODBC</param>        
        /// <param name="Usuario">Usuario de la basde de datos</param>
        /// <param name="Password">Password del usuario de base de datos</param>
        public static string ConexionSybase(string DSN, string Usuario, string Password)
        {
            return "DSN=" + DSN + ";UID=" + Usuario + ";PWD=" + Password + ";";
        }        
    }
    #endregion

    #region Clase: Transaction donde se puede realizar transacciones de varios procedimientos almacenados en lote
    /// <summary>
    /// Clase que permite ejecutar varios Store Procedures controlando la Transaccionalidad de la operacion
    /// Store Procedures de Insert, Update y Delete
    /// </summary>    
    public class Transaction
    {   
        /// <summary>
        ///  Atributo Error, contiene el error en caso de fallar el proceso con la BD
        /// </summary>
        public string ErrorMessage { get; set; }
                
        /// <summary>
        /// Coleccion de procedimientos almacenados
        /// </summary>       
        private System.Collections.Generic.List<StoreProcedure> batch = new List<StoreProcedure>();

        /// <summary>
        /// Atributo lista de Store Procedures
        /// </summary>    
        public System.Collections.Generic.List<StoreProcedure> Batch
        {
            get { return batch; }
        }
        
        /// <summary>
        /// Constructor de la clase
        /// </summary>       
        public Transaction()
        {
        }
               
        /// <summary>
        /// Funcion para ejecutar la Transaccion con la base de datos
        /// </summary>}
        /// <param name="CadenaConexion">Cadena de Conexion a la base de datos donde se ejecutara el proceso</param>
        public bool EjecutarTransaccion(string CadenaConexion)
        {
            // Se verifica que se tiene StoreProcedure a ejecutar en la coleccion
            if (Batch.Count > 0)
            {
                SqlConnection conexion = new SqlConnection(CadenaConexion);
                SqlTransaction Transaccion;
                conexion.Open();
                //Se inicia la transaccion
                Transaccion = conexion.BeginTransaction();
                try
                {
                    //Se recorre los StoreProcedure que se tienen
                    for (int cont = 0; cont < Batch.Count; cont++)
                    {
                        SqlDataAdapter Comando = new SqlDataAdapter(Batch[cont].NameSP, conexion);
                        Comando.SelectCommand.CommandType = CommandType.StoredProcedure;
                        Comando.SelectCommand.CommandTimeout = 600000;
                        //Se añaden los parametros que tiene el Store Procedure
                        for (int cont2 = 0; cont2 < Batch[cont].ListParameters.Count; cont2++)
                        {
                            if (Batch[cont].ListParameters[cont2].Value == null)
                                Comando.SelectCommand.Parameters.AddWithValue(Batch[cont].ListParameters[cont2].Name, DBNull.Value);
                            else
                                Comando.SelectCommand.Parameters.AddWithValue(Batch[cont].ListParameters[cont2].Name, Batch[cont].ListParameters[cont2].Value);
                        }
                        Comando.SelectCommand.Transaction = Transaccion;
                        Comando.SelectCommand.ExecuteNonQuery();
                    }
                    //Se Hace el commit
                    Transaccion.Commit();
                    //Si no ha habido errores se guarda en la variable MensajeError una cadena vacia
                    ErrorMessage = String.Empty;
                    //Se retorna True indicando q' se realizo el proceso
                    return true;
                }
                catch (SqlException Error)
                {
                    //En caso de un error se realiza un roll back
                    Transaccion.Rollback();
                    //Se guarda en la variable MensajesError el error q' se produjo
                    ErrorMessage = Error.Message;
                    //Se retorna falso indicado q' no se ejecuto el proceso
                    return false;
                }
                finally
                {
                    conexion.Close();
                    //Limpieza de los pool de conexion, para depurar cualquier conexion que no se cerro
                    SqlConnection.ClearAllPools();
                }   
            }
            else
            {
                return true;
            }
        }

    }
    #endregion

    #region Clase: StoreProcedure donde esta las ejecuciones de procedimientos almacenados
    /// <summary>
    /// Clase que permite ejecutar un Store Procedure tanto para Insert, 
    /// Update, Delete y hacer una consulta de datos
    /// </summary> 
    public class StoreProcedure
    {
        #region 1. Atributos de la clasee

        #region 1.1 NameSP

        /// <summary>
        /// Variable que contiene el nombre del Store Procedure
        /// </summary>
        private string nameSP = String.Empty;
        
        /// <summary>
        /// Atributo nombre del Store Procedure
        /// </summary>
        public string NameSP
        {
            get { return nameSP; }
            set { nameSP = value; }
        }
        
        #endregion

        #region 1.2 ListParameters
        /// <summary>
        /// Coleccion de Parametros        
        /// </summary>
        private System.Collections.Generic.List<ParameterSP> listParameters = new List<ParameterSP>();

        /// <summary>
        /// Atributo Lista de parametros
        /// </summary>         
        public System.Collections.Generic.List<ParameterSP> ListParameters
        {
            get { return listParameters; }
            set { listParameters = value; }
        }       
     
        #endregion

        #region 1.3 ErrorMessage
        /// <summary>
        /// Variable que contiene el error que se ocaciono
        /// </summary>
        private string errorMessage = String.Empty;

        /// <summary>
        /// Atributo Error generado despues de un proceso
        /// </summary>
        public string ErrorMessage
        {
            get { return errorMessage; }
        }
        #endregion
       
        #endregion

        #region 2. Contructor
        /// <summary>
        /// Constructor de la clase
        /// </summary> 
        /// <param name="NombreStoreProcedure">Nombre del Store Procedure</param>
        public StoreProcedure(string NombreStoreProcedure)
        {
            this.nameSP = NombreStoreProcedure;
        }
        #endregion
        
        #region 3. Agregar y traer parametros del procedimiento almacenado
        /// <summary>
        /// Metodo para agregar los parametros a enviar al Store Procedure
        /// </summary> 
        /// <param name="NombreParametro">Nombre del Parametro</param>
        /// <param name="ValorParametro">Valor para el parametro</param>
        public void AddParameter(string parameterName, object parameterValue, DirectionValues parameterDirection)
        {
            listParameters.Add(new ParameterSP(parameterName, parameterValue, parameterDirection));
        }
        
        public ParameterSP getItem(string parameterName)
        {
            foreach (ParameterSP parameterSP in listParameters)
            {
                if (parameterSP.Name == parameterName)
                    return parameterSP;                
            }
            return null;
        }

        #endregion

        #region 4. Ejecuta consultas o procedimientos almacenados
       
        /// <summary>
        /// Funcion para ejecutar el Store Procedure de Insert, Update, Delete
        /// </summary>
        /// <param name="CadenaConexion">Cadena de conexion a la base de datos donde se desea ejecutar el Store Procedure</param>
        /// <returns>Indicador booleano del resultado de la ejecucion</returns>
        public Boolean executeStoredProcedure(string CadenaConexion)
        {
            SqlConnection conexion = new SqlConnection(CadenaConexion);
            SqlCommand comando = new SqlCommand(nameSP, conexion);
            //Se indica que es del tipo StoreProcedure
            comando.CommandType = CommandType.StoredProcedure;
            comando.CommandTimeout = 600000;
            //Se declaran los parametros en el SQLCommand
            for (int cont = 0; cont < listParameters.Count; cont++)
            {
                if (listParameters[cont].Value == null)
                    comando.Parameters.AddWithValue(listParameters[cont].Name, DBNull.Value);                
                else
                {
                    SqlParameter parameter = new SqlParameter(listParameters[cont].Name, listParameters[cont].Value);
                    switch (listParameters[cont].Direction)
                    {
                        case DirectionValues.Input:
                            parameter.Direction = ParameterDirection.Input;
                            break;
                        case DirectionValues.Ouput:
                            parameter.Direction = ParameterDirection.Output;
                            parameter.Size = Int32.MaxValue;
                            break;
                        case DirectionValues.InputOuput:
                            parameter.Direction = ParameterDirection.InputOutput;
                            parameter.Size = Int32.MaxValue;
                            break;
                        default:
                            break;
                    }
                    comando.Parameters.Add(parameter);                    
                }
                
            }
            try
            {
                //Se habre la conexion
                conexion.Open();
                //se ejecuta el query
                comando.ExecuteNonQuery();
                //Se cierra la conexion
                conexion.Close();
                //Si no ha habido errores se guarda en la variable MensajeError una cadena vacia
                errorMessage = String.Empty;
                //Limpieza de los pool de conexion, para depurar cualquier conexion que no se cerro
                SqlConnection.ClearAllPools();
                for (int i = 0; i < listParameters.Count; i++)
                {
                    if (listParameters[i].Direction == DirectionValues.Ouput)
                        listParameters[i].Value = comando.Parameters[listParameters[i].Name].Value;
                }

                //Se Retorna true indicando que se ejecuto el proceso                
                return true;
            }
            catch (SqlException Error)
            {
                //Se guarda en la variable MensajeError el error q' se produjo
                errorMessage = Error.Message;
                //Se cierra la conexion
                conexion.Close();
                SqlConnection.ClearAllPools();
                //Se Retorna falso en caso de que no se haya ejecutado el proceso
                return false;
            }
        }
        
        /// <summary>
        /// Funcion para ejecutar un Store Procedure que devuelva un resultado
        /// </summary>
        /// <param name="CadenaConexion">Cadena de conexion a la base de datos donde se desea ejecutar el Store Procedure</param>
        /// <returns>Datatable no tipado</returns>
        public DataTable makeQuery(string CadenaConexion)
        {
            DataTable Consulta = new DataTable();
            if (CadenaConexion.Length > 0)
            {
                SqlConnection conexion = new SqlConnection(CadenaConexion);
                SqlDataAdapter Comando = new SqlDataAdapter(nameSP, conexion);
                Comando.SelectCommand.CommandType = CommandType.StoredProcedure;
                Comando.SelectCommand.CommandTimeout = 600000;
                for (int cont = 0; cont < listParameters.Count; cont++)
                {
                    if (listParameters[cont].Value == null)
                        Comando.SelectCommand.Parameters.AddWithValue(listParameters[cont].Name, DBNull.Value);
                    else
                    {
                        SqlParameter parameter = new SqlParameter(listParameters[cont].Name, listParameters[cont].Value);
                        switch (listParameters[cont].Direction)
                        {
                            case DirectionValues.Input:
                                parameter.Direction = ParameterDirection.Input;
                                break;
                            case DirectionValues.Ouput:
                                parameter.Direction = ParameterDirection.Output;
                                parameter.Size = Int32.MaxValue;
                                break;
                            case DirectionValues.InputOuput:
                                parameter.Direction = ParameterDirection.InputOutput;
                                parameter.Size = Int32.MaxValue;
                                break;
                            default:
                                break;
                        }
                        Comando.SelectCommand.Parameters.Add(parameter);                    
                    }
                    
                }
                try
                {
                    conexion.Open();
                    Comando.Fill(Consulta);
                    errorMessage = String.Empty;
                }
                catch (SqlException Error)
                {
                    errorMessage = Error.Message;
                }
                finally
                {
                    conexion.Close();
                    //Limpieza de los pool de conexion, para depurar cualquier conexion que no se cerro
                    SqlConnection.ClearAllPools();
                }
            }
            else
                errorMessage = "No se recibio la cadena de conexion";
            return Consulta;
        }
       
        /// <summary>
        /// Funcion para ejecutar una consulta con conexion de tipo ODBC
        /// </summary>
        /// <param name="CadenaConexion">Cadena de conexion a la base de datos donde se desea ejecutar el Store Procedure</param>
        /// <returns>Datatable no tipado/returns>
        public DataTable makeQueryODBC(string CadenaConexion)
        {
            DataTable Consulta = new DataTable();
            OdbcConnection conexion = new OdbcConnection(CadenaConexion);
            OdbcDataAdapter Comando = new OdbcDataAdapter(nameSP, conexion);
            Comando.SelectCommand.CommandTimeout = 600000;
            for (int cont = 0; cont < listParameters.Count; cont++)
                Comando.SelectCommand.Parameters.AddWithValue(listParameters[cont].Name, listParameters[cont].Value);
            try
            {
                conexion.Open();
                Comando.Fill(Consulta);
                errorMessage = String.Empty;
            }
            catch (SqlException Error)
            {
                errorMessage = Error.Message;
            }
            finally
            {
                conexion.Close();                
            }
            return Consulta;
        }
        #endregion
    }
    #endregion

    #region Clase: ParameterSP de parametros del procedimiento almacenado
    /// <summary>
    /// Clase de tipo Parametros de un SP
    /// </summary>    
    public class ParameterSP
    {
       
        /// <summary>
        /// Variable con el nombre del parametro
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Variable con el valor del parametro 
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Direccion del parametro que puede ser INPUT OUTPUT INPUTOUPUT
        /// </summary>
        public DirectionValues Direction { get; set; }

        /// <summary>
        /// Constructor de la clase
        /// </summary> 
        /// <param name="ParameterName">Nombre del Parametro del SP</param>
        /// <param name="ParameterValue">Valor a enviarse en el parametro</param>
        public ParameterSP(string ParameterName, object ParameterValue, DirectionValues ParameterDirection)
        {
            this.Name = ParameterName;
            this.Value = ParameterValue;
            this.Direction = ParameterDirection;
        } 
    }

    [Flags]
    public enum DirectionValues
    {
        Input,
        Ouput,
        InputOuput,
    }
    #endregion
}
