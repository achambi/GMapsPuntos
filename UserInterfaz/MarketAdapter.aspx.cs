using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web.Script.Serialization;
using BussinesLogic;
using Entities;

namespace UserInterfaz 
{
    public partial class marketAdapter : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int tipoPuntoId           = 0;
            int departamentoId        = 0;

            if(Request.QueryString.Get("tipoPunto")!=string.Empty)
            {
                tipoPuntoId           = Convert.ToInt32(Request.QueryString.Get("tipoPunto"));
                departamentoId        = Convert.ToInt32(Request.QueryString.Get("departamentoId"));
            }
            Response.Expires                          = 0;
            Response.ContentType                      = "application/json";
            XmlDocument oDocument                     = new XmlDocument();
            StringBuilder sb                          = new StringBuilder();
            GestorPuntos gestorPuntos                 = new GestorPuntos();
            ListPunto ListPunto                       = gestorPuntos.getManyPuntoByTipoId(tipoPuntoId, departamentoId);
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string jsonString                         = javaScriptSerializer.Serialize(ListPunto);
            Response.Write(jsonString);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
        }
    }
}