using System;
using System.Collections.Generic;
using System.Data;
using Entities;
using DataAccess;
using NLog;
using Auditoria;

namespace BussinesLogic
{
    public class GestorParametro
    {
        /// <summary>
        /// Trae todos los datos de los clientes
        /// </summary>
        /// <returns>Data Table con los datos de los clientes</returns>
        public ListParametro GetManyParametro(String tipoParametro)
        {
            ListParametro listParametro;
            try
            {
                var conector = new ConectorParametro();                
                DataTable dtCliente         = conector.GetManyParametro(tipoParametro);
                listParametro               = new ListParametro { success = true, message = String.Empty };
                listParametro.listParametro = new List<Parametro>();
                foreach (DataRow row in dtCliente.Rows)
                {
                    var item           = new Parametro();
                    item.parametroId         = Convert.ToInt32(row["PARAMETRO_ID_IN"]);
                    item.parametroCodigo     = Convert.ToInt32(row["PARAMETRO_CODIGO_IN"]);
                    item.tipoParametro       = Convert.ToString(row["TIPO_PARAMETRO_NV"]);
                    item.valorParametroCorto = Convert.ToString(row["VALOR_DE_CORTO_NV"]);
                    item.valorParametro      = Convert.ToString(row["VALOR_DES_LARGA_NV"]);
                    listParametro.listParametro.Add(item);
                }
            }
            catch (Exception ex)
            {
                listParametro = new ListParametro { success = false, message = ex.Message };
                TextLogger.LogError(LogManager.GetCurrentClassLogger(), ex, "Error En el metodo: getManyParametro");
            }
            return listParametro;
        }
    }
}
