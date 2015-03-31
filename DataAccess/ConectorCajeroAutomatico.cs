using System;
using Auditoria;
using NLog;
using Entities;
namespace DataAccess
{
    public class ConectorCajeroAutomatico
    {
        public String ConexionString  { get; set; }

        /// <summary>
        /// Constructor por defecto llama a la cadena de conexion para instanciarla
        /// </summary>
        public ConectorCajeroAutomatico()
        {
            ConexionString = Conexion.ConexionGmaps();
        }

        /// <summary>
        /// Inserta nuevo cajero automatico
        /// </summary>
        /// <param name="cajerAutomatico">entidad cajero automatico</param>
        /// <returns>devuelve un booleano true o false</returns>
        public Boolean InsertCajeroAutomatico(CajerAutomatico cajerAutomatico)
        {
            Boolean resultado;
            try
            {

                var spInsertCajero = new StoreProcedure("[dbo].[SP_CAJERO_AUTOMATICO_INSERT]");
                spInsertCajero.AddParameter("@NOMBRE_NV"         , cajerAutomatico.Nombre,          DirectionValues.Input);
                spInsertCajero.AddParameter("@TIPO_PUNTO_ID_IN"  , cajerAutomatico.TipoPuntoId,     DirectionValues.Input);
                spInsertCajero.AddParameter("@DEPARTAMENTO_ID_IN", cajerAutomatico.DepartamentoId,  DirectionValues.Input);
                spInsertCajero.AddParameter("@LATITUD_DC"        , cajerAutomatico.Latitud,         DirectionValues.Input);
                spInsertCajero.AddParameter("@LONGITUD_DC"       , cajerAutomatico.Longitud,        DirectionValues.Input);
                spInsertCajero.AddParameter("@DIRECCION_NV"      , cajerAutomatico.Direccion,       DirectionValues.Input);
                spInsertCajero.AddParameter("@FLAG_MINUSVALIDO"  , cajerAutomatico.FlagMinusvalido, DirectionValues.Input);
                spInsertCajero.AddParameter("@MONEDA_ID_IN"      , cajerAutomatico.MonedaId,        DirectionValues.Input);
                resultado                     = spInsertCajero.ExecuteStoredProcedure(ConexionString);
                if (spInsertCajero.ErrorMessage != String.Empty)
                    throw new Exception("Procedimiento Almacenado :[dbo].[SP_CAJERO_AUTOMATICO_INSERT] Descripcion:" + spInsertCajero.ErrorMessage.Trim());

            }
            catch (Exception ex)
            {
                TextLogger.LogError(LogManager.GetCurrentClassLogger(), ex, "Error En el metodo: InsertCajeroAutomatico");
                throw new Exception("Procedimiento Almacenado :[dbo].[SP_CAJERO_AUTOMATICO_INSERT]" + ex);
            }
            return resultado;
        }
    }
}
