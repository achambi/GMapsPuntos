using System;
using System.Data;
using NLog;
using Auditoria;

namespace DataAccess
{
    public class ConectorParametro
    {
        /// <summary>
        /// Trae los parametros por medio de un criterio
        /// </summary>
        /// <returns>un data table de clientes</returns>
        public DataTable GetManyParametro(String tipoParametro)
        {
            DataTable dtParametro;
            try
            {
                string conexionString = Conexion.ConexionGmaps();
                var storeProcedure = new StoreProcedure("[dbo].[SP_PARAMETRO_GET_MANY_BY_TIPO]");
                storeProcedure.AddParameter("@TIPO_PARAMETRO_NV", tipoParametro, DirectionValues.Input);
                dtParametro = storeProcedure.MakeQuery(conexionString);
                if (storeProcedure.ErrorMessage != String.Empty)
                    throw new Exception("Procedimiento Almacenado :[dbo].[SP_PARAMETRO_GET_ALL] Descripcion:" + storeProcedure.ErrorMessage.Trim());

            }
            catch (Exception ex)
            {
                TextLogger.LogError(LogManager.GetCurrentClassLogger(),ex, "Error En el metodo: getManyParametro");
                throw new Exception("Procedimiento Almacenado :[dbo].[SP_PARAMETRO_GET_ALL]" + ex);
            }
            return dtParametro;
        }
    }
}
