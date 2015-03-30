using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.Configuration;
using System.Diagnostics;
using System.Net.Mail;
using System.Collections;
using System.Security.Cryptography;

namespace Auditoria
{
    public class TextLogger
    {
        /// <summary>
        /// Crea la configuracion del sistema de acuerdo con el archivo de configuracion Web.config
        /// </summary>
        protected static void configuracion()
        {
            LoggingConfiguration config = new LoggingConfiguration();
            FileTarget fileTarget = new FileTarget();
            Hashtable ConfiguracionNlog = (Hashtable)ConfigurationManager.GetSection("ConfiguracionNlog");
            fileTarget.FileName = ConfiguracionNlog["DirecDailyLogs"].ToString() + ".txt";
            fileTarget.Layout = ConfiguracionNlog["LayoutNlog"].ToString();
            
            
            config.AddTarget("logfile", fileTarget);

            LoggingRule rule = new LoggingRule("*", LogLevel.Info, fileTarget);
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static String getMethodInvoke()
        {
            String metodName = String.Empty;
            try
            {
                StackFrame frame = new StackFrame(2);
                var method = frame.GetMethod();
                var type = method.DeclaringType;
                var name = method.Name;
                metodName = name.ToString();
            }
            catch (Exception)
            {
                metodName = "Error no se pudo obtener el nombre del metodo";
            }
            return metodName;
        }
        
        /// <summary>
        /// Crea un registro en el archivo de texto del log con este registro es de Informacion
        /// </summary>
        /// <param name="logger">Nombre de la clase que invoco el archivo de configuracion</param>
        /// <param name="descripcion">descripcion del mensaje a registrarse en el archivo de configuracion</param>
        public static void LogInformation(Logger logger, string description)
        {
            try
            {
                configuracion();
                logger.Info("Metodo: " + getMethodInvoke() + "   Informacion:    -> " + description);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Crea un registro en el archivo de texto del log con este registro es de Informacion
        /// </summary>
        /// <param name="logger">Nombre de la clase que invoco el archivo de configuracion</param>
        /// <param name="descripcion">descripcion del mensaje a registrarse en el archivo de configuracion</param>
        public static void LogWarning(Logger logger, string description)
        {
            try
            {
                configuracion();
                logger.Info("Metodo: " + getMethodInvoke() + "   Advertencia:       -> " + description);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Crea un registro en el archivo de texto del log con este registro es de Error
        /// </summary>
        /// <param name="logger">Nombre de la clase que invoco el archivo de configuracion</param>
        /// <param name="description">descripcion del mensaje a registrarse en el archivo de configuracion</param>
        /// <param name="error">registra la informacion de la excepciones</param>
        public static void LogError(Logger logger, Exception error)
        {
            try
            {
                configuracion();             
                logger.Info("Metodo: " + getMethodInvoke() + "   Error:       -> " + error.ToString());
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Crea un registro en el archivo de texto del log con este registro es de Error
        /// </summary>
        /// <param name="logger">Nombre de la clase que invoco el archivo de configuracion</param>
        /// <param name="description">descripcion del mensaje a registrarse en el archivo de configuracion</param>
        /// <param name="error">registra la informacion de la excepciones</param>
        public static void LogError(Logger logger, Exception error, String message)
        {
            try
            {
                configuracion();
                StackFrame frame = new StackFrame(1);
                var method = frame.GetMethod();
                var type = method.DeclaringType;
                var name = method.Name;
                logger.Info("Metodo: " + getMethodInvoke() + "   Error:       -> " + error.ToString() + "     Mensaje: " + message);
            }
            catch (Exception)
            {

            }
        }
    }

    public class Correos
    {
        public static void Enviarmails(string para, string de, string asunto, string cuerpo, string ServidorCorreo, System.IO.Stream[] Adjuntos, string[] NombreArchivo)
        {
            //Armado del correo
            System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
            correo.From = new System.Net.Mail.MailAddress(de);
            para = para.Trim(Convert.ToChar(32));
            correo.To.Add(para);
            correo.Subject = asunto;
            correo.Body = cuerpo;
            correo.IsBodyHtml = false;
            correo.Priority = System.Net.Mail.MailPriority.Normal;
            // Declaracion del servidor
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            for (int cont = 0; cont < Adjuntos.Length; cont++)
            {
                if (Adjuntos[cont] != null)
                {
                    System.Net.Mail.Attachment archivo = new Attachment(Adjuntos[cont], NombreArchivo[cont]);
                    correo.Attachments.Add(archivo);
                }
            }
            smtp.Host = ServidorCorreo;
            smtp.Port = 25;
            //Envio de mail
            try
            {
                smtp.Send(correo);
            }
            catch (Exception ex)
            {
                TextLogger.LogError(LogManager.GetCurrentClassLogger(), ex, "Error en el envio de notificación al mail : " + para);
            }
        }

        public static void Enviarmails(string para, string de, string asunto, string cuerpo, string ServidorCorreo, string[] NombreArchivo)
        {
            //Armado del correo
            System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
            correo.From = new System.Net.Mail.MailAddress(de);
            para = para.Trim(Convert.ToChar(32));
            correo.To.Add(para);
            correo.Subject = asunto;
            correo.Body = cuerpo;
            correo.IsBodyHtml = false;
            correo.Priority = System.Net.Mail.MailPriority.Normal;
            // Declaracion del servidor
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            for (int cont = 0; cont < NombreArchivo.Length; cont++)
            {
                if (NombreArchivo[cont] != "")
                {
                    System.Net.Mail.Attachment archivo = new Attachment(NombreArchivo[cont]);
                    correo.Attachments.Add(archivo);
                }
            }
            smtp.Host = ServidorCorreo;
            smtp.Port = 25;
            //Envio de mail
            try
            {
                smtp.Send(correo);
            }
            catch (Exception ex)
            {
                TextLogger.LogError(LogManager.GetCurrentClassLogger(), ex, "Error en el envio de notificación al mail : " + para);
            }
        }

        public static void Enviarmails(string para, string de, string asunto, string cuerpo, string ServidorCorreo,string FlagTipoCorreo)
        {

            Hashtable configCorreos = (Hashtable)ConfigurationManager.GetSection("ConfiguracionCorreos");
            string FlagsCorreos     = configCorreos["FlagsCorreos"].ToString();
            Boolean informacion     = Convert.ToBoolean(Convert.ToInt16(FlagsCorreos.Substring(0,1)));
            Boolean advertencia     = Convert.ToBoolean(Convert.ToInt16(FlagsCorreos.Substring(1,1)));
            Boolean error           = Convert.ToBoolean(Convert.ToInt16(FlagsCorreos.Substring(2,1)));
            Boolean resultado       = false;
            if (FlagTipoCorreo == "I" && informacion)
                resultado = true;
            else if (FlagsCorreos=="A" && advertencia)
                resultado = true;
            else if (FlagsCorreos=="E" && error)
                resultado = true;
            if (resultado)
            {
                //Armado del correo
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(de);
                para = para.Trim(Convert.ToChar(32));
                correo.To.Add(para);
                correo.Subject = asunto;
                correo.Body = cuerpo;
                correo.IsBodyHtml = false;
                correo.Priority = System.Net.Mail.MailPriority.Normal;
                // Declaracion del servidor
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Host = ServidorCorreo;
                smtp.Port = 25;
                //Envio de mail
                try
                {
                    smtp.Send(correo);
                }
                catch (Exception ex)
                {
                    TextLogger.LogError(LogManager.GetCurrentClassLogger(), ex, "Error en el envio de notificación al mail : " + para);
                }
            }
            
        }
       
    }
}
