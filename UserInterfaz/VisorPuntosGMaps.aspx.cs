using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BussinesLogic;
using Entities;
using System.Data;
using System.Configuration;

namespace UserInterfaz
{
    public partial class VisorPuntosGMaps : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CargaTiposPunto();
            Session.Add("SelectMenu", "Vista Puntos");
            if (!IsPostBack)
            {
                CargaDepartamento();
            }
        }

        protected void CargaTiposPunto()
        {
            GestorParametro gestorParametro                    = new GestorParametro();
            ListParametro listParametro                        = gestorParametro.GetManyParametro("TIPO_PUNTO_ID_IN");
            if (listParametro.success)
            {
                ddlTipoPuntos.DataSource                       = listParametro.listParametro;
                ddlTipoPuntos.DataValueField                   = "parametroCodigo";
                ddlTipoPuntos.DataTextField                    = "valorParametro";
                ddlTipoPuntos.DataBind();
                for (int i = 0; i < ddlTipoPuntos.Items.Count; i++)
                    ddlTipoPuntos.Items[i].Attributes["title"] = "App_Themes/GMapsTheme/images/ImageIcon/" + listParametro.listParametro[i].valorParametroCorto;
            }
        }

        protected void CargaDepartamento()
        {
            GestorParametro gestorParametro = new GestorParametro();
            ListParametro listParametro = gestorParametro.GetManyParametro("DEPARTAMENTO");
            if (listParametro.success)
            {
                ddlDepartamentos.DataSource     = listParametro.listParametro;
                ddlDepartamentos.DataValueField = "parametroId";
                ddlDepartamentos.DataTextField  = "valorParametro";
                ddlDepartamentos.DataBind();
                
            }
        }
    }
}