using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entidades.CobranzaEntity;
using Data_Access;
using NLog;
using Bitacoras;
using System.Data;

namespace BussinesLogic
{
    public class GestorCliente
    {
        /// <summary>
        /// Adiciona Nuevos Cliente
        /// </summary>
        /// <param name="puntoBM"></param>
        /// <returns>True si fue exitoso y False si no se pudo insertar</returns>
        public static ResultadoProceso insertCliente(Cliente cliente)
        {
            ResultadoProceso resultado = null;
            try
            {
                ConectorCliente conector = new ConectorCliente();
                resultado= new ResultadoProceso();
                if(!conector.ifClientExist(cliente.idc, cliente.extensionIdc))
                {
                    resultado.success=conector.insertCliente(cliente);
                    resultado.message=(resultado.success)?"Se inserto los datos del cliente correctamente":"Error al insertar los datos del cliente";
                }
                else
                {                    
                    resultado.message="El cliente ya se encuentra registrado";
                }                
            }
            catch (Exception ex)
            {
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error En el metodo: insertCliente", ex);                
                throw new Exception("Error al realizar la consulta en la base de datos");
            }
            return resultado;
        }

        /// <summary>
        /// Trae todos los datos de los clientes
        /// </summary>
        /// <returns>Data Table con los datos de los clientes</returns>
        public static List<Cliente> getAllCliente()
        {
            List<Cliente> listCliente = null;
            DataTable DTCliente = null;
            try
            {
                ConectorCliente conector = new ConectorCliente();
                listCliente = new List<Cliente>();
                DTCliente = conector.getAllCliente();
                foreach (DataRow row in DTCliente.Rows)
                {
                    Cliente item = new Cliente();
                    item.clienteId  = Convert.ToInt32(row["CL_Cliente_Id"]);
			        item.Nombre     = Convert.ToString(row["CL_Nombre_nv"]);
                    item.apPaterno  = Convert.ToString(row["CL_ApPaterno_nv"]);
                    item.apMaterno  = Convert.ToString(row["CL_ApMaterno_nv"]);
                    item.idc        = Convert.ToString(row["CL_Idc"]);
                    item.extensionIdc       = Convert.ToString(row["CL_Extension_Idc"]);
                    item.fechaModificacion  = Convert.ToDateTime(row["CL_Fecha_Modificacion"]);
                    item.fechaCreacion      = Convert.ToDateTime(row["CL_Fecha_Creacion"]);
                    item.estado             = Convert.ToBoolean(row["CL_Estado"]);
                    item.ultimoUsuario      = Convert.ToString(row["CL_UltimoUsuario"]);
                    listCliente.Add(item);
                }
            }
            catch (Exception ex)
            {
                listCliente = null;
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error En el metodo: getAllCliente", ex);
                throw new Exception("Error al realizar la consulta en la base de datos");
            }
            return listCliente;
        }
    }
}
