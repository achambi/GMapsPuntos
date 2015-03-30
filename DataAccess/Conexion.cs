using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                return GetConexion.ConexionSQL(ConfigurationManager.AppSettings.Get("GMServer"), ConfigurationManager.AppSettings.Get("GMDataBase"));
            }
            else 
            {
                string pwd = MiRijndael.Encriptar(ConfigurationManager.AppSettings.Get("GMPassword"));
                return GetConexion.ConexionSQL(ConfigurationManager.AppSettings.Get("GMServer"), ConfigurationManager.AppSettings.Get("GMDataBase"), ConfigurationManager.AppSettings.Get("GMUsuario"), pwd);    
            }
            
        }
    }
}
