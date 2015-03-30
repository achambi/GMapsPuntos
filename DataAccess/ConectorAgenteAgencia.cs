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
    public class ConectorAgenteAgencia
    {
        public String ConexionString  { get; set; }

        /// <summary>
        /// Constructor por defecto llama a la cadena de conexion para instanciarla
        /// </summary>
        public ConectorAgenteAgencia()
        {
            ConexionString = Conexion.ConexionGmaps();
        }

        /// <summary>
        /// Inserta nueva Agencia o Agente
        /// </summary>
        /// <param name="agenteAgencia">objeto Agencia Agente</param>
        /// <returns>True si es exitoso la incersion y false si es fallida</returns>
        public Boolean InsertAgenteAgencia(AgenteAgencia agenteAgencia)
        {
            Boolean resultado = false;
            Transaction transaction = new Transaction();
            try
            {

                StoreProcedure spInsertAgenAgencia = new StoreProcedure("[dbo].[SP_AGEN_AGENCIA_INSERT]");
                spInsertAgenAgencia.AddParameter("@NOMBRE_NV"         , agenteAgencia.Nombre,          DirectionValues.Input);
                spInsertAgenAgencia.AddParameter("@TIPO_PUNTO_ID_IN"  , agenteAgencia.TipoPuntoId,     DirectionValues.Input);
                spInsertAgenAgencia.AddParameter("@DEPARTAMENTO_ID_IN", agenteAgencia.DepartamentoId,  DirectionValues.Input);
                spInsertAgenAgencia.AddParameter("@LATITUD_DC"        , agenteAgencia.Latitud,         DirectionValues.Input);
                spInsertAgenAgencia.AddParameter("@LONGITUD_DC"       , agenteAgencia.Longitud,        DirectionValues.Input);
                spInsertAgenAgencia.AddParameter("@DIRECCION_NV"      , agenteAgencia.Direccion,       DirectionValues.Input);
                spInsertAgenAgencia.AddParameter("@NOMBRE_SERVIDOR"   , agenteAgencia.NombreServidor,  DirectionValues.Input);
                spInsertAgenAgencia.AddParameter("@AGEN_AGENCIA_ID_IN", agenteAgencia.AgenteAgenciaId, DirectionValues.Ouput);
                resultado = spInsertAgenAgencia.executeStoredProcedure(ConexionString);
                if (spInsertAgenAgencia.ErrorMessage != String.Empty)
                    throw new Exception("Procedimiento Almacenado :[dbo].[SP_AGEN_AGENCIA_INSERT] Descripcion:" + spInsertAgenAgencia.ErrorMessage.Trim());
                agenteAgencia.AgenteAgenciaId=Convert.ToInt32(spInsertAgenAgencia.getItem("@AGEN_AGENCIA_ID_IN").Value);
                foreach (Horario item in agenteAgencia.ListHorarios)
                {
                    StoreProcedure spHorario = new StoreProcedure("[dbo].[SP_HORARIO_INSERT]");
                    spHorario.AddParameter("@AGEN_AGENCIA_ID_IN", agenteAgencia.AgenteAgenciaId,DirectionValues.Input);
                    spHorario.AddParameter("@DIA_ID_IN"         , item.diaId,                   DirectionValues.Input);
                    spHorario.AddParameter("@HORARIO_DESC"      , item.HorarioDescripcion,      DirectionValues.Input);
                    transaction.Batch.Add(spHorario);
                }
                transaction.EjecutarTransaccion(ConexionString);
                if (transaction.ErrorMessage != String.Empty)
                    throw new Exception("Procedimiento Almacenado :[dbo].[SP_HORARIO_INSERT] Descripcion:" + transaction.ErrorMessage.Trim());
                resultado = true;
            }
            catch (Exception ex)
            {
                TextLogger.LogError(LogManager.GetCurrentClassLogger(), ex, "Error En el metodo: InsertAgenteAgencia");
                throw new Exception("Procedimiento Almacenado :[dbo].[SP_AGEN_AGENCIA_INSERT]" + ex.ToString());
            }
            return resultado;
        }
    }
}
