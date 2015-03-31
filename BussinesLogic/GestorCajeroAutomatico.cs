using System;
using Auditoria;
using NLog;
using DataAccess;
using Entities;
namespace BussinesLogic
{
    public class GestorCajeroAutomatico
    {
        /// <summary>
        /// Adiciona Nuevos cajeros automaticos
        /// </summary>
        /// <returns>True si fue exitoso y False si no se pudo insertar</returns>
        public Resultado AddCajero(CajerAutomatico cajerAutomatico)
        {
            try
            {
                var conector = new ConectorCajeroAutomatico();
                return (!conector.InsertCajeroAutomatico(cajerAutomatico)) ? new Resultado { success = false, message = "Existio un error al realizar la insercion" } : new Resultado { success = true, message = "Se inserto Correctamente" };
            }
            catch (Exception ex)
            {
                TextLogger.LogError(LogManager.GetCurrentClassLogger(), ex, "Error En el metodo: AddPunto");
                return new Resultado { success = false, message = "Existio un error tecnico al realizar la insercion del cajero automatico" };
            }
        }
    }
}
