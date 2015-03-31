using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BussinesLogic;
using Entities;
namespace UserInterfaz
{
    public partial class AdministradorPuntos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        { 
            txtLatitud.Attributes.Add("readonly", "readonly");
            txtLongitud.Attributes.Add("readonly", "readonly");
            CargaTiposPunto();
            CargaMoneda();
            Session.Add("SelectMenu", "Admministracion de Puntos");
            if (!IsPostBack)
            {
                CargaDepartamento();
            }
        }
        
        protected void CargaTiposPunto()
        {
            ListParametro listParametro  = null;
            List<Parametro> paremtroList = null;
            if (!IsPostBack)
            {
                GestorParametro gestorParametro  = new GestorParametro();
                 listParametro                   = gestorParametro.GetManyParametro("TIPO_PUNTO_ID_IN");
                if (listParametro.success)
                {
                    listParametro.listParametro.RemoveAt(0);
                    ddlTipoPuntos.DataSource     = listParametro.listParametro;
                    ddlTipoPuntos.DataValueField = "parametroCodigo";
                    ddlTipoPuntos.DataTextField  = "valorParametro";
                    ddlTipoPuntos.DataBind();
                    Session.Add("listParametros",listParametro.listParametro);
                }
            }
            if(Session["listParametros"]!=null)
            {
                paremtroList = (List<Parametro>)Session["listParametros"];
                for (int i = 0; i < ddlTipoPuntos.Items.Count; i++)
                    ddlTipoPuntos.Items[i].Attributes["title"] = "App_Themes/GMapsTheme/images/ImageIcon/" + paremtroList[i].valorParametroCorto;
                ddlTipoPuntos.Attributes.Add("onchange", "selectTipoPunto(event);");
            }
        }
  
        protected void btbGuardar_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                try
                {
                    int tipoPunto=Convert.ToInt32(ddlTipoPuntos.SelectedValue.ToString());
                    Resultado resultado = null;
                    switch (tipoPunto)
                    {   
                        case 1:
                                GestorCajeroAutomatico gestorC   = new GestorCajeroAutomatico();
                                CajerAutomatico cajerAutomatico = new CajerAutomatico();
                                cajerAutomatico.DepartamentoId  = Convert.ToInt32(ddlDepartamentos.SelectedValue.ToString());
                                cajerAutomatico.Direccion       = txtBoxDireccion.Text.Trim();
                                cajerAutomatico.FlagMinusvalido = cBoxMinusvalido.Checked;
                                cajerAutomatico.Latitud         = Convert.ToDecimal(txtLatitud.Text.Trim());
                                cajerAutomatico.Longitud        = Convert.ToDecimal(txtLongitud.Text.Trim());
                                cajerAutomatico.Nombre          = txtBoxNombre.Text.Trim();
                                cajerAutomatico.TipoPuntoId     = Convert.ToInt32(tipoPunto + ddlMoneda.SelectedValue.ToString()+Convert.ToInt16(cBoxMinusvalido.Checked));
                                cajerAutomatico.MonedaId        = Convert.ToInt32(ddlMoneda.SelectedValue.ToString());
                                resultado                       = gestorC.AddCajero(cajerAutomatico);
                                if (resultado.success)
                                {
                                    string script = @"alert('Se inserto correctamente el Cajero Automatico');";
                                    ScriptManager.RegisterStartupScript(this, GetType(), "MensajeCajero", script, true);
                                }
                                else
                                {
                                    string script = @"alert('Exitio un error al insertar el Cajero Automatico');";
                                    ScriptManager.RegisterStartupScript(this, GetType(), "MensajeCajero", script, true);
                                }
                        break;
                        case 2:
                                GestorAgenteAgencia gestorA    = new GestorAgenteAgencia();
                                AgenteAgencia agenteAgencia    = new AgenteAgencia();
                                agenteAgencia.DepartamentoId   = Convert.ToInt32(ddlDepartamentos.SelectedValue.ToString());
                                agenteAgencia.Direccion        = txtBoxDireccion.Text.Trim();
                                agenteAgencia.Latitud          = Convert.ToDecimal(txtLatitud.Text.Trim());
                                agenteAgencia.Longitud         = Convert.ToDecimal(txtLongitud.Text.Trim());
                                agenteAgencia.Nombre           = txtBoxNombre.Text.Trim();
                                agenteAgencia.NombreServidor   = txtBoxServBD.Text.Trim();
                                agenteAgencia.TipoPuntoId      = Convert.ToInt32(tipoPunto);
                                agenteAgencia.ListHorarios     = new List<Horario>();                                
                                agenteAgencia.ListHorarios.Add(new Horario() { diaId = 25 ,HorarioDescripcion = txtBoxLunes.Text.Trim()});
                                agenteAgencia.ListHorarios.Add(new Horario() { diaId = 26, HorarioDescripcion = txtBoxMartes.Text.Trim()});
                                agenteAgencia.ListHorarios.Add(new Horario() { diaId = 27, HorarioDescripcion = txtBoxMartes.Text.Trim() });
                                agenteAgencia.ListHorarios.Add(new Horario() { diaId = 30, HorarioDescripcion = txtBoxMartes.Text.Trim() });
                                agenteAgencia.ListHorarios.Add(new Horario() { diaId = 31, HorarioDescripcion = txtBoxMartes.Text.Trim() });
                                agenteAgencia.ListHorarios.Add(new Horario() { diaId = 32, HorarioDescripcion = txtBoxMartes.Text.Trim() });
                                resultado                     = gestorA.AddAgenteAgencia(agenteAgencia);
                                if (resultado.success)
                                {
                                    string script = @"alert('Se inserto correctamente el Agente o Agencia');";
                                    ScriptManager.RegisterStartupScript(this, GetType(), "Mensaje AgenAgencia", script, true);
                                }
                                else
                                {
                                    string script = @"alert('Exitio un error al insertar el Agente o Agencia');";
                                    ScriptManager.RegisterStartupScript(this, GetType(), "Mensaje AgenAgencia", script, true);
                                }
                        break;
                                default:
                                        break;
                    }
                    CleanControl(this.Controls);        
                }
                catch (Exception)
                {

                }

            }
        }

        protected void CargaDepartamento()
        {
            GestorParametro gestorParametro                    = new GestorParametro();
            ListParametro listParametro                        = gestorParametro.GetManyParametro("DEPARTAMENTO");
            if (listParametro.success)
            {
                listParametro.listParametro.RemoveAt(0);
                ddlDepartamentos.DataSource                    = listParametro.listParametro;
                ddlDepartamentos.DataValueField                = "parametroId";
                ddlDepartamentos.DataTextField                 = "valorParametro";
                ddlDepartamentos.DataBind();
            }
        }

        protected void CargaMoneda()
        {
            GestorParametro gestorParametro = new GestorParametro();
            ListParametro listParametro     = gestorParametro.GetManyParametro("MONEDA");
            if (listParametro.success)
            {
                ddlMoneda.DataSource        = listParametro.listParametro;
                ddlMoneda.DataValueField    = "parametroId";
                ddlMoneda.DataTextField     = "valorParametro";
                ddlMoneda.DataBind();
                for (int i = 0; i < ddlMoneda.Items.Count; i++)
                    ddlMoneda.Items[i].Attributes["title"] = "App_Themes/GMapsTheme/images/ImageIcon/" + listParametro.listParametro[i].valorParametroCorto;
                ddlMoneda.Attributes.Add("onchange", "selectMoneda(event);");
            }
        }

         /// <summary>
        /// LImpia todos los controles de el formulario
        /// </summary>
        /// <param name="controles">Collecion de Controles</param>
        public void CleanControl(ControlCollection controles)
        {
            foreach (Control control in controles)
            {
                if (control is TextBox)
                    ((TextBox)control).Text = string.Empty;
                else if (control is DropDownList)
                    ((DropDownList)control).ClearSelection();
                else if (control is RadioButtonList)
                    ((RadioButtonList)control).ClearSelection();
                else if (control is CheckBoxList)
                    ((CheckBoxList)control).ClearSelection();
                else if (control is RadioButton)
                    ((RadioButton)control).Checked = false;
                else if (control is CheckBox)
                    ((CheckBox)control).Checked = false;
                else if (control.HasControls())
                    //Esta linea detécta un Control que contenga otros Controles
                    //Así ningún control se quedará sin ser limpiado.
                    CleanControl(control.Controls);
            }
        }
    }
}