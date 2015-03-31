using System;
using Auditoria;
using NLog;
using DataAccess;
using Entities;
namespace BussinesLogic
{
    public class GestorAgenteAgencia
    {
        

        /// <summary>
        /// Adiciona nuevos cajeros y agencias
        /// </summary>
        /// <param name="agenteAgencia"></param>
        /// <returns></returns>
        public Resultado AddAgenteAgencia(AgenteAgencia agenteAgencia)
        {
            try
            {
                var conector = new ConectorAgenteAgencia();
                return (!conector.InsertAgenteAgencia(agenteAgencia)) ? new Resultado { success = false, message = "Existio un error al realizar la insercion" } : new Resultado { success = true, message = "Se inserto Correctamente" };
            }
            catch (Exception ex)
            {
                TextLogger.LogError(LogManager.GetCurrentClassLogger(), ex, "Error En el metodo: AddAgenteAgencia");
                return new Resultado { success = false, message = "Existio un error tecnico al realizar la insercion de la Agencia o Agente" };
            }
        }
    }
}
