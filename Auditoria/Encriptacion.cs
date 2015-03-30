using System;
using System.Security.Cryptography;
using System.Configuration;
using NLog;

namespace Auditoria
{
    /// <summary>
    /// Summary description for Transformacion.
    /// </summary>
    public class MiRijndael
    {
        public static byte[] Encriptar(string strEncriptar, byte[] bytPK)
        {
            Rijndael miRijndael = Rijndael.Create();
            byte[] encrypted = null;
            byte[] returnValue = null;

            try
            {
                miRijndael.Key = bytPK;
                miRijndael.GenerateIV();

                byte[] toEncrypt = System.Text.Encoding.UTF8.GetBytes(strEncriptar);
                encrypted = (miRijndael.CreateEncryptor()).TransformFinalBlock(toEncrypt, 0, toEncrypt.Length);

                returnValue = new byte[miRijndael.IV.Length + encrypted.Length];
                miRijndael.IV.CopyTo(returnValue, 0);
                encrypted.CopyTo(returnValue, miRijndael.IV.Length);

            }
            catch { }
            finally { miRijndael.Clear(); }

            return returnValue;
        }

        public static string Encriptar(string strEncriptar)
        {
            try 
	        {

                return Convert.ToBase64String(Encriptar(strEncriptar, (new PasswordDeriveBytes(ConfigurationManager.AppSettings.Get("key"), null)).GetBytes(32)));
	        }
	        catch (Exception ex)
	        {

                TextLogger.LogError(LogManager.GetCurrentClassLogger(),ex, "Error al extraer la llave de encriptacion");
                throw new Exception("Error al extraer la llave de encriptacion: " + ex.ToString());
	        } 
        }

        public static string Desencriptar(byte[] bytDesEncriptar, byte[] bytPK)
        {
            Rijndael miRijndael = Rijndael.Create();
            byte[] tempArray = new byte[miRijndael.IV.Length];
            byte[] encrypted = new byte[bytDesEncriptar.Length - miRijndael.IV.Length];
            string returnValue = string.Empty;

            try
            {
                miRijndael.Key = bytPK;

                Array.Copy(bytDesEncriptar, tempArray, tempArray.Length);
                Array.Copy(bytDesEncriptar, tempArray.Length, encrypted, 0, encrypted.Length);
                miRijndael.IV = tempArray;

                returnValue = System.Text.Encoding.UTF8.GetString((miRijndael.CreateDecryptor()).TransformFinalBlock(encrypted, 0, encrypted.Length));

            }
            catch { }
            finally { miRijndael.Clear(); }

            return returnValue;
        }

        public static string Desencriptar(string strDesEncriptar)
        {
            
            try
            {
                return Desencriptar(Convert.FromBase64String(strDesEncriptar), (new PasswordDeriveBytes(ConfigurationManager.AppSettings.Get("key"), null)).GetBytes(32));
            }
            catch (Exception ex)
	        {
                TextLogger.LogError(LogManager.GetCurrentClassLogger(),ex, "Error al extraer la llave de encriptacion");
                throw new Exception("Error al extraer la llave de encriptacion: " + ex.ToString());
	        } 
        }
    }
}
