using System;
using System.Configuration;
using Auditoria;

namespace DataAccess
{
    public class Conexion
    {
        public static string ConexionGmaps()
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings.Get("WindowsAutentication")))
            {
                return GetConexion.ConexionSql(ConfigurationManager.AppSettings.Get("GMServer"), ConfigurationManager.AppSettings.Get("GMDataBase"));
            }
            var pwd = MiRijndael.Encriptar(ConfigurationManager.AppSettings.Get("GMPassword"));
            return GetConexion.ConexionSql(ConfigurationManager.AppSettings.Get("GMServer"), ConfigurationManager.AppSettings.Get("GMDataBase"), ConfigurationManager.AppSettings.Get("GMUsuario"), pwd);
        }
    }
}
