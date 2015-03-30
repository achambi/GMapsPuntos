using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BancaMovil;
using Bitacoras;
using NLog;
using Entidades.CobranzaEntity;
namespace BussinesLogic
{
    public class GestorPuntosBM
    {
        /// <summary>
        /// Trae todos los puntos de sucursales, atm y agentes del bcp para google maps
        /// </summary>
        /// <returns>Data Table con todos los tipos de puntos en Banca Movil</returns>
        public DataTable getAllPuntos()
        {
            DataTable DTTiposPunto=null;
            try 
	        {
                ConectorBancaMovil conector = new ConectorBancaMovil();
                DTTiposPunto = conector.getAllTipoPunto();
	        }
	        catch (Exception ex)
	        {
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error En el metodo: getPuntosBM", ex);
                throw new Exception("Error al realizar la consulta en la base de datos");
	        }
            return DTTiposPunto;
        }

        /// <summary>
        /// trae las pociciones en google Maps de los puntos de un determinado tipo
        /// </summary>
        /// <param name="tipoPunto">Tipo de punto 1, 2 y 3</param>
        /// <returns>Data table de todos los puntos de un determinado tipo en banca Movil</returns>
        public DataTable getPuntosBM(String tipoDescripcion)
        {
            DataTable DTPuntosBM = null;
            try
            {
                ConectorBancaMovil conector = new ConectorBancaMovil();
                DTPuntosBM = conector.getPuntosByTipo(tipoDescripcion);
            }
            catch (Exception ex)
            {
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error En el metodo: getPuntosBM", ex);
                throw new Exception("Error al realizar la consulta en la base de datos");
            }
            return DTPuntosBM;
        }

        public Boolean AddPunto(PuntoBM puntoBM)
        {
            Boolean resultado = false;
            try
            {
                ConectorBancaMovil conector = new ConectorBancaMovil();
                resultado = conector.AddPunto(puntoBM);
            }
            catch (Exception ex)
            {
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error En el metodo: AddPunto", ex);
                throw new Exception("Error al insertar un punto en la base de datos");
            }
            return resultado;
        }

    }
}
