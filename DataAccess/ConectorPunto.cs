using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Auditoria;
using NLog;
using Entities;

namespace DataAccess
{
    public class ConectorPuntos
    {
        public Boolean insertPunto(Punto punto)
        {
            Boolean resultado = false;
            try
            {
                string ConexionString = Conexion.ConexionGmaps();
                StoreProcedure storeProcedure = new StoreProcedure("[dbo].[SP_PUNTO_INSERT]");
                storeProcedure.AddParameter("@NOMBRE_NV"         , punto.Nombre,         DirectionValues.Input);
                storeProcedure.AddParameter("@TIPO_PUNTO_ID_IN"  , punto.TipoPuntoId,    DirectionValues.Input);
                storeProcedure.AddParameter("@DEPARTAMENTO_ID_IN", punto.DepartamentoId, DirectionValues.Input);
                storeProcedure.AddParameter("@LATITUD_DC"        , punto.Latitud,        DirectionValues.Input);
                storeProcedure.AddParameter("@LONGITUD_DC"       , punto.Longitud,       DirectionValues.Input);
                storeProcedure.AddParameter("@DIRECCION_NV"    , punto.Direccion,        DirectionValues.Input);
                resultado                     = storeProcedure.executeStoredProcedure(ConexionString);
                if (storeProcedure.ErrorMessage != String.Empty)
                    throw new Exception("Procedimiento Almacenado :[dbo].[SP_PUNTO_INSERT] Descripcion:" + storeProcedure.ErrorMessage.Trim());

            }
            catch (Exception ex)
            {
                TextLogger.LogError(LogManager.GetCurrentClassLogger(), ex, "Error En el metodo: InsertDireccionCliente");
                throw new Exception("Procedimiento Almacenado :[dbo].[SP_PUNTO_INSERT]" + ex.ToString());
            }
            return resultado;
        }

        /// <summary>
        /// Trae los puntos por medio de un criterio
        /// </summary>
        /// <returns>un data table de clientes</returns>
        public DataTable getManyPuntoByTipoId(int tipoPuntoId,int departamentoId)
        {
            DataTable DTPuntos = null;
            try
            {
                string ConexionString         = Conexion.ConexionGmaps();
                StoreProcedure storeProcedure = new StoreProcedure("[dbo].[SP_PUNTO_GET_MANY_BY_TIPO]");
                storeProcedure.AddParameter("@TIPO_PUNTO_ID_IN"     , tipoPuntoId,    DirectionValues.Input);
                storeProcedure.AddParameter("@DEPARTAMENTO_ID_IN"   , departamentoId, DirectionValues.Input);
                DTPuntos                      = storeProcedure.makeQuery(ConexionString);
                if (storeProcedure.ErrorMessage != String.Empty)
                    throw new Exception("Procedimiento Almacenado :[dbo].[SP_PUNTO_GET_MANY_BY_TIPO] Descripcion:" + storeProcedure.ErrorMessage.Trim());

            }
            catch (Exception ex)
            {
                TextLogger.LogError(LogManager.GetCurrentClassLogger(), ex, "Error En el metodo: getManyPuntoByTipoId");
                throw new Exception("Procedimiento Almacenado :[dbo].[SP_PUNTO_GET_MANY_BY_TIPO]" + ex.ToString());
            }
            return DTPuntos;
        }
    }
}
