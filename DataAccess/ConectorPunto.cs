using System;
using System.Data;
using Auditoria;
using NLog;
using Entities;

namespace DataAccess
{
    public class ConectorPuntos
    {
        public Boolean InsertPunto(Punto punto)
        {
            Boolean resultado;
            try
            {
                string conexionString = Conexion.ConexionGmaps();
                var storeProcedure = new StoreProcedure("[dbo].[SP_PUNTO_INSERT]");
                storeProcedure.AddParameter("@NOMBRE_NV"         , punto.Nombre,         DirectionValues.Input);
                storeProcedure.AddParameter("@TIPO_PUNTO_ID_IN"  , punto.TipoPuntoId,    DirectionValues.Input);
                storeProcedure.AddParameter("@DEPARTAMENTO_ID_IN", punto.DepartamentoId, DirectionValues.Input);
                storeProcedure.AddParameter("@LATITUD_DC"        , punto.Latitud,        DirectionValues.Input);
                storeProcedure.AddParameter("@LONGITUD_DC"       , punto.Longitud,       DirectionValues.Input);
                storeProcedure.AddParameter("@DIRECCION_NV"    , punto.Direccion,        DirectionValues.Input);
                resultado                     = storeProcedure.ExecuteStoredProcedure(conexionString);
                if (storeProcedure.ErrorMessage != String.Empty)
                    throw new Exception("Procedimiento Almacenado :[dbo].[SP_PUNTO_INSERT] Descripcion:" + storeProcedure.ErrorMessage.Trim());

            }
            catch (Exception ex)
            {
                TextLogger.LogError(LogManager.GetCurrentClassLogger(), ex, "Error En el metodo: InsertDireccionCliente");
                throw new Exception("Procedimiento Almacenado :[dbo].[SP_PUNTO_INSERT]" + ex);
            }
            return resultado;
        }

        /// <summary>
        /// Trae los puntos por medio de un criterio
        /// </summary>
        /// <returns>un data table de clientes</returns>
        public DataTable GetManyPuntoByTipoId(int tipoPuntoId,int departamentoId)
        {
            DataTable dtPuntos;
            try
            {
                string conexionString         = Conexion.ConexionGmaps();
                var storeProcedure = new StoreProcedure("[dbo].[SP_PUNTO_GET_MANY_BY_TIPO]");
                storeProcedure.AddParameter("@TIPO_PUNTO_ID_IN"     , tipoPuntoId,    DirectionValues.Input);
                storeProcedure.AddParameter("@DEPARTAMENTO_ID_IN"   , departamentoId, DirectionValues.Input);
                dtPuntos                      = storeProcedure.MakeQuery(conexionString);
                if (storeProcedure.ErrorMessage != String.Empty)
                    throw new Exception("Procedimiento Almacenado :[dbo].[SP_PUNTO_GET_MANY_BY_TIPO] Descripcion:" + storeProcedure.ErrorMessage.Trim());

            }
            catch (Exception ex)
            {
                TextLogger.LogError(LogManager.GetCurrentClassLogger(), ex, "Error En el metodo: getManyPuntoByTipoId");
                throw new Exception("Procedimiento Almacenado :[dbo].[SP_PUNTO_GET_MANY_BY_TIPO]" + ex);
            }
            return dtPuntos;
        }
    }
}
