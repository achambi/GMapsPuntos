using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;

namespace DataAccess
{

    #region Clase: ObtenerConexion donde se crean cadenas de conexion a base de datos.

    /// <summary>
    ///     Clase para armar una cadena de conexion a partir de los datos que se requiere
    /// </summary>
    public class GetConexion
    {
        /// <summary>
        ///     Funcion para armar una cadena de conexion a SQL
        /// </summary>
        /// <param name="server">Nombre del servidor de base de datos</param>
        /// <param name="baseDatos">Nombre de la base de datos</param>
        /// <param name="usuario">Usuario de la basde de datos</param>
        /// <param name="password">Password del usuario de base de datos</param>
        public static string ConexionSql(string server, string baseDatos, string usuario, string password)
        {
            var conguardar =
                new SqlConnectionStringBuilder("Server= " + server + "; DataBase=" + baseDatos + " ; User Id=" + usuario +
                                               ";Password=" + password + ";");
            return conguardar.ConnectionString;
        }

        /// <summary>
        ///     Funcion para armar una cadena de conexion a SQL con Windows Autentication
        /// </summary>
        /// <param name="server">Nombre del servidor de base de datos</param>
        /// <param name="baseDatos">Nombre de la base de datos</param>
        public static string ConexionSql(string server, string baseDatos)
        {
            var conguardar =
                new SqlConnectionStringBuilder("Data Source=" + server + ";Initial Catalog=" + baseDatos +
                                               ";Integrated Security=True");
            return conguardar.ConnectionString;
        }

        /// <summary>
        ///     Funcion para armar una cadena de conexion Sybase, utilizando un DNS
        /// </summary>
        /// <param name="dsn">Nombre del DSN a invocar para la conexion ODBC</param>
        /// <param name="usuario">Usuario de la basde de datos</param>
        /// <param name="password">Password del usuario de base de datos</param>
        public static string ConexionSybase(string dsn, string usuario, string password)
        {
            return "DSN=" + dsn + ";UID=" + usuario + ";PWD=" + password + ";";
        }
    }

    #endregion

    #region Clase: Transaction donde se puede realizar transacciones de varios procedimientos almacenados en lote

    /// <summary>
    ///     Clase que permite ejecutar varios Store Procedures controlando la Transaccionalidad de la operacion
    ///     Store Procedures de Insert, Update y Delete
    /// </summary>
    public class Transaction
    {
        /// <summary>
        ///     Coleccion de procedimientos almacenados
        /// </summary>
        private readonly List<StoreProcedure> batch = new List<StoreProcedure>();

        /// <summary>
        ///     Atributo Error, contiene el error en caso de fallar el proceso con la BD
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     Atributo lista de Store Procedures
        /// </summary>
        public List<StoreProcedure> Batch
        {
            get { return batch; }
        }

        /// <summary>
        ///     Funcion para ejecutar la Transaccion con la base de datos
        /// </summary>
        /// }
        /// <param name="cadenaConexion">Cadena de Conexion a la base de datos donde se ejecutara el proceso</param>
        public bool EjecutarTransaccion(string cadenaConexion)
        {
            // Se verifica que se tiene StoreProcedure a ejecutar en la coleccion
            if (Batch.Count > 0)
            {
                var conexion = new SqlConnection(cadenaConexion);
                SqlTransaction Transaccion;
                conexion.Open();
                //Se inicia la transaccion
                Transaccion = conexion.BeginTransaction();
                try
                {
                    //Se recorre los StoreProcedure que se tienen
                    for (int cont = 0; cont < Batch.Count; cont++)
                    {
                        var comando = new SqlDataAdapter(Batch[cont].NameSp, conexion);
                        comando.SelectCommand.CommandType = CommandType.StoredProcedure;
                        comando.SelectCommand.CommandTimeout = 600000;
                        //Se añaden los parametros que tiene el Store Procedure
                        for (int cont2 = 0; cont2 < Batch[cont].ListParameters.Count; cont2++)
                        {
                            if (Batch[cont].ListParameters[cont2].Value == null)
                                comando.SelectCommand.Parameters.AddWithValue(Batch[cont].ListParameters[cont2].Name,
                                    DBNull.Value);
                            else
                                comando.SelectCommand.Parameters.AddWithValue(Batch[cont].ListParameters[cont2].Name,
                                    Batch[cont].ListParameters[cont2].Value);
                        }
                        comando.SelectCommand.Transaction = Transaccion;
                        comando.SelectCommand.ExecuteNonQuery();
                    }
                    //Se Hace el commit
                    Transaccion.Commit();
                    //Si no ha habido errores se guarda en la variable MensajeError una cadena vacia
                    ErrorMessage = String.Empty;
                    //Se retorna True indicando q' se realizo el proceso
                    return true;
                }
                catch (SqlException error)
                {
                    //En caso de un error se realiza un roll back
                    Transaccion.Rollback();
                    //Se guarda en la variable MensajesError el error q' se produjo
                    ErrorMessage = error.Message;
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
            return true;
        }
    }

    #endregion

    #region Clase: StoreProcedure donde esta las ejecuciones de procedimientos almacenados

    /// <summary>
    ///     Clase que permite ejecutar un Store Procedure tanto para Insert,
    ///     Update, Delete y hacer una consulta de datos
    /// </summary>
    public class StoreProcedure
    {
        #region 1. Atributos de la clasee

        #region 1.1 NameSP

        /// <summary>
        ///     Variable que contiene el nombre del Store Procedure
        /// </summary>
        private string nameSp = String.Empty;

        /// <summary>
        ///     Atributo nombre del Store Procedure
        /// </summary>
        public string NameSp
        {
            get { return nameSp; }
            set { nameSp = value; }
        }

        #endregion

        #region 1.2 ListParameters

        /// <summary>
        ///     Coleccion de Parametros
        /// </summary>
        private List<ParameterSp> listParameters = new List<ParameterSp>();

        /// <summary>
        ///     Atributo Lista de parametros
        /// </summary>
        public List<ParameterSp> ListParameters
        {
            get { return listParameters; }
            set { listParameters = value; }
        }

        #endregion

        #region 1.3 ErrorMessage

        /// <summary>
        ///     Variable que contiene el error que se ocaciono
        /// </summary>
        private string errorMessage = String.Empty;

        /// <summary>
        ///     Atributo Error generado despues de un proceso
        /// </summary>
        public string ErrorMessage
        {
            get { return errorMessage; }
        }

        #endregion

        #endregion

        #region 2. Contructor

        /// <summary>
        ///     Constructor de la clase
        /// </summary>
        /// <param name="nombreStoreProcedure">Nombre del Store Procedure</param>
        public StoreProcedure(string nombreStoreProcedure)
        {
            nameSp = nombreStoreProcedure;
        }

        #endregion

        #region 3. Agregar y traer parametros del procedimiento almacenado

        /// <summary>
        ///     Metodo para agregar los parametros a enviar al Store Procedure
        /// </summary>
        public void AddParameter(string parameterName, object parameterValue, DirectionValues parameterDirection)
        {
            listParameters.Add(new ParameterSp(parameterName, parameterValue, parameterDirection));
        }

        public ParameterSp GetItem(string parameterName)
        {
            return listParameters.FirstOrDefault(parameterSp => parameterSp.Name == parameterName);
        }

        #endregion

        #region 4. Ejecuta consultas o procedimientos almacenados

        /// <summary>
        ///     Funcion para ejecutar el Store Procedure de Insert, Update, Delete
        /// </summary>
        /// <param name="cadenaConexion">Cadena de conexion a la base de datos donde se desea ejecutar el Store Procedure</param>
        /// <returns>Indicador booleano del resultado de la ejecucion</returns>
        public Boolean ExecuteStoredProcedure(string cadenaConexion)
        {
            var conexion = new SqlConnection(cadenaConexion);
            var comando = new SqlCommand(nameSp, conexion);
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
                    var parameter = new SqlParameter(listParameters[cont].Name, listParameters[cont].Value);
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
            catch (SqlException error)
            {
                //Se guarda en la variable MensajeError el error q' se produjo
                errorMessage = error.Message;
                //Se cierra la conexion
                conexion.Close();
                SqlConnection.ClearAllPools();
                //Se Retorna falso en caso de que no se haya ejecutado el proceso
                return false;
            }
        }

        /// <summary>
        ///     Funcion para ejecutar un Store Procedure que devuelva un resultado
        /// </summary>
        /// <param name="cadenaConexion">Cadena de conexion a la base de datos donde se desea ejecutar el Store Procedure</param>
        /// <returns>Datatable no tipado</returns>
        public DataTable MakeQuery(string cadenaConexion)
        {
            var consulta = new DataTable();
            if (cadenaConexion.Length > 0)
            {
                var conexion = new SqlConnection(cadenaConexion);
                var comando = new SqlDataAdapter(nameSp, conexion);
                comando.SelectCommand.CommandType = CommandType.StoredProcedure;
                comando.SelectCommand.CommandTimeout = 600000;
                for (int cont = 0; cont < listParameters.Count; cont++)
                {
                    if (listParameters[cont].Value == null)
                        comando.SelectCommand.Parameters.AddWithValue(listParameters[cont].Name, DBNull.Value);
                    else
                    {
                        var parameter = new SqlParameter(listParameters[cont].Name, listParameters[cont].Value);
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
                        }
                        comando.SelectCommand.Parameters.Add(parameter);
                    }
                }
                try
                {
                    conexion.Open();
                    comando.Fill(consulta);
                    errorMessage = String.Empty;
                }
                catch (SqlException error)
                {
                    errorMessage = error.Message;
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
            return consulta;
        }

        /// <summary>
        ///     Funcion para ejecutar una consulta con conexion de tipo ODBC
        /// </summary>
        /// <param name="cadenaConexion">Cadena de conexion a la base de datos donde se desea ejecutar el Store Procedure</param>
        /// <returns>Datatable no tipado</returns>
        public DataTable MakeQueryOdbc(string cadenaConexion)
        {
            var consulta = new DataTable();
            var conexion = new OdbcConnection(cadenaConexion);
            var comando = new OdbcDataAdapter(nameSp, conexion);
            comando.SelectCommand.CommandTimeout = 600000;
            for (int cont = 0; cont < listParameters.Count; cont++)
                comando.SelectCommand.Parameters.AddWithValue(listParameters[cont].Name, listParameters[cont].Value);
            try
            {
                conexion.Open();
                comando.Fill(consulta);
                errorMessage = String.Empty;
            }
            catch (SqlException error)
            {
                errorMessage = error.Message;
            }
            finally
            {
                conexion.Close();
            }
            return consulta;
        }

        #endregion
    }

    #endregion

    #region Clase: ParameterSP de parametros del procedimiento almacenado

    /// <summary>
    ///     Clase de tipo Parametros de un SP
    /// </summary>
    public class ParameterSp
    {
        /// <summary>
        ///     Constructor de la clase
        /// </summary>
        /// <param name="parameterName">Nombre del Parametro del SP</param>
        /// <param name="parameterValue">Valor a enviarse en el parametro</param>
        /// <param name="parameterDirection">Direccion puede ser Input, Output y InputOutput</param>
        public ParameterSp(string parameterName, object parameterValue, DirectionValues parameterDirection)
        {
            Name = parameterName;
            Value = parameterValue;
            Direction = parameterDirection;
        }

        /// <summary>
        ///     Variable con el nombre del parametro
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Variable con el valor del parametro
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        ///     Direccion del parametro que puede ser INPUT OUTPUT INPUTOUPUT
        /// </summary>
        public DirectionValues Direction { get; set; }
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