<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="AdministradorPuntos.aspx.cs" Inherits="UserInterfaz.AdministradorPuntos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?key=AIzaSyBBBOCF9c2k3NDwkfYIlRsX1PI8V4w5zYs&sensor=FALSE">
    </script>
    <script type="text/javascript" src="Scripts/jquery-2.1.1.min.js">
    </script>
    <script type="text/javascript" src="Scripts/jquery_dropdown.js">
    </script>
    <script type="text/javascript">
        var map;
        var geocoder;
        var markermap;

        function initialize()
        {
            geocoder = new google.maps.Geocoder();
            var mapOptions =
            {
                center: new google.maps.LatLng(-16.46348017, -64.81933593),
                zoom: 6,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
            
            google.maps.event.addListener
            (
                map, "click",
                function (event)
                {      
                    setMarket(event.latLng);
                }
           );

        }
        
        function selectDepartamento()
        { 
            $("#<%=ddlDepartamentos.ClientID%>").on
            ("change",
                function (e)
                {
                    var optionSelected = $("option:selected", this);
                    var textSelected   = optionSelected.filter(":selected").text();
                    codeAddress("BOLIVIA," + textSelected,15);
                }
            );
        }

        function selectMoneda(event)
        {    
            setMarketChangeTextBox();
        }

        function setMarketChangeTextBox()
        {
            var lat = $("#<%=txtLatitud.ClientID%>").val();
            var lng = $("#<%=txtLongitud.ClientID%>").val();
            if (lat != "0.00" && lng != "0.00")
            {
                var latLng = new google.maps.LatLng(lat, lng);
                setMarket(latLng);
            }
        }
        function chededMinusvalido()
        {
            $("#<%=cBoxMinusvalido.ClientID%>").click
            (
                        function ()
                        {
                            setMarketChangeTextBox();
                        }
            );
        }

        function selectTipoPunto(event)
        {
           
            if (event.target.value == "1")
            {

                visibleHide("atmDataFields", true);
                visibleHide("agenAgenciaDataFields", false);
            }
            else {
                visibleHide("atmDataFields", false);
                visibleHide("agenAgenciaDataFields", true);
            }
            setMarketChangeTextBox();
        }

        function IniTipoPunto()
        {
            var ddlPuntodClient = "<%=ddlTipoPuntos.ClientID%>";
            var element = $("#" + ddlPuntodClient);
            var dato = element.val();
            if (dato == "1")
            {
                
                visibleHide("atmDataFields", true);
                visibleHide("agenAgenciaDataFields", false);
            }
            else
            {               
                visibleHide("atmDataFields", false);
                visibleHide("agenAgenciaDataFields", true);
            }
        }

        function visibleHide(element, flag)
        {
            if (flag)
                $("#" + element).show("slow");
            else
                $("#" + element).hide("slow");
        }

        function setMarket(location)
        {           
           if (markermap)
           {
                markermap.setMap(null);
           }
           var tipoPuntoId         = $("#<%=ddlTipoPuntos.ClientID%>").val();
           var monedaId            = $("#<%=ddlMoneda.ClientID%>").val();
           var flagMinusvalido     = ($("#<%=cBoxMinusvalido.ClientID%>").is(':checked'))?"1":"0";
           markermap = new google.maps.Marker
           (
             {
                 position : location,
                 map      : map,
                 title    : "punto",
                 animation: google.maps.Animation.DROP,
                 icon     : "App_Themes/GMapsTheme/images/ImageIcon/" + ((tipoPuntoId == "1") ? tipoPuntoId + monedaId + flagMinusvalido : tipoPuntoId) + ".png"
             }
           );
           $("#<%=txtLatitud.ClientID%>").val(location.k);
           $("#<%=txtLongitud.ClientID%>").val(location.B);
        }
       
        function codeAddress(campo,zoom)
        {
            var address = campo;
            geocoder.geocode
            (   { 'address': address },
                function (results, status)
                {
                    if (status == google.maps.GeocoderStatus.OK)
                    {
                        map.setCenter(results[0].geometry.location);
                        map.setZoom(zoom);
                        setMarket(results[0].geometry.location);                        
                    }
                    else
                    {
                        alert("Geocode was not successful for the following reason: " + status);
                    }
                }
          );
        }

        function getAdressButon()
        {                                                   
            $("#btnBuscarDireccion").click
                (
                    function ()
                    {
                        var departamento = $("#<%=ddlDepartamentos.ClientID%> option:selected").text();
                        var direccion    = $("#<%=txtBoxDireccion.ClientID%>").val();
                        codeAddress("BOLIVIA," + departamento+ ", " + direccion,15);
                    }
                );
        }

        $(document).ready
        (
            function ()
            {               
                try
                {
                    initialize();
                    $("#<%=ddlTipoPuntos.ClientID%>").msDropDown();
                    $("#<%=ddlMoneda.ClientID%>").msDropDown();
                    selectDepartamento();
                    getAdressButon();
                    IniTipoPunto();
                    chededMinusvalido();
                }
                catch (e)
                {
                    alert(e.message);
                }
            }
        );
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMaster" runat="server">        
    <h3>Adminitración de Puntos</h3>
    <fieldset>
    <legend>Datos Generales</legend>
    <form id="FormAdministradorPuntos" runat="server" >
        <table>
            <tr>
                <td><asp:Label ID="lblNombre" runat="server" Text="Nombre:"></asp:Label></td>
                <td><asp:TextBox ID="txtBoxNombre" runat="server" Width="220px"></asp:TextBox></td>
                <td><asp:RequiredFieldValidator ID="RequiredFieldValidatorTxtBoxNombre" runat="server" ErrorMessage="*" ControlToValidate="txtBoxNombre" SetFocusOnError ="true" ValidationGroup="FormValidation"></asp:RequiredFieldValidator></td>
                <td><asp:Label ID="lblTipoPunto" runat="server" Text="Tipo de Punto:"></asp:Label></td>    
                <td colspan="2"><asp:DropDownList ID="ddlTipoPuntos" runat="server" Width="231px"></asp:DropDownList></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblLatitud" runat="server" Text="Latitud:"></asp:Label></td>
                <td><asp:TextBox ID="txtLatitud"  runat="server" Width="220px">0.00</asp:TextBox></td>
                <td><asp:RequiredFieldValidator ID="RequiredFieldValidatortxtLatitud" runat="server" ErrorMessage="*" ControlToValidate="txtLatitud" SetFocusOnError ="true" ValidationGroup="FormValidation"></asp:RequiredFieldValidator></td>
                <td><asp:Label ID="lblLongitud" runat="server" Text="Longitud:"></asp:Label></td>
                <td><asp:TextBox ID="txtLongitud" runat="server" Width="220px">0.00</asp:TextBox></td>
                <td><asp:RequiredFieldValidator ID="RequiredFieldValidatortxtLongitud" runat="server" ErrorMessage="*" ControlToValidate="txtLongitud" SetFocusOnError ="true" ValidationGroup="FormValidation"></asp:RequiredFieldValidator></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblLatitud0" runat="server" Text="Departamento"></asp:Label></td>
                <td colspan="3">
                    <asp:DropDownList ID="ddlDepartamentos" runat="server" Width="358px" CssClass="formSelected"></asp:DropDownList>
                </td>
                <td>
                    &nbsp;</td>
                <td>&nbsp;</td>
            </tr>            
            <tr>
                <td><asp:Label ID="lblDireccion" runat="server" Text="Direccion:"></asp:Label></td>
                <td colspan="3"><asp:TextBox ID="txtBoxDireccion" runat="server" Width="350px" style="overflow:auto;" ></asp:TextBox></td>
                <td>
                    <input id="btnBuscarDireccion" type="button" value="Buscar en Google" class="formbuttonLeft" /></td>
                <td><asp:RequiredFieldValidator ID="RequiredFieldValidatorTxtBoxDireccion" runat="server" ErrorMessage="*" ControlToValidate="txtBoxDireccion" SetFocusOnError ="true" ValidationGroup="FormValidation"></asp:RequiredFieldValidator></td>
            </tr> 
            <tr>
                <td colspan="6">
                        <fieldset  id="atmDataFields">
                        <legend>Datos Cajero Automatico</legend>
                        <table width="100%">
                            <tr>
                                <td><asp:CheckBox ID="cBoxMinusvalido" runat="server" Text="Rampa Minusvalidos"/></td>
                                <td>
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/App_Themes/GMapsTheme/images/ImageIcon/minusvalido.jpg" Width="40px" />                          
                                </td>      
                            </tr>
                            <tr>
                                <td><asp:Label ID="Label1" runat="server" Text="Moneda Cajero:"></asp:Label></td>    
                                <td><asp:DropDownList ID="ddlMoneda" runat="server" Width="231px" ></asp:DropDownList></td>
                             </tr>  
                        </table>
                        </fieldset>
                </td>
            </tr> 
            <tr>
                <td colspan="6">
                        <fieldset  id="agenAgenciaDataFields">
                        <legend>Datos Agente - Agencia</legend>
                        <table width="100%">                            
                            <tr>
                                <td colspan="2">
                                    <fieldset>
                                    <legend>Horarios</legend>
                                        <table width="100%">
                                            <tr>
                                                <td><asp:CheckBox ID="CheckBox1" runat="server" Text="Lunes:"/></td>
                                                <td><asp:TextBox ID="txtBoxLunes" runat="server" Width="200px" style="overflow:auto;" ></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td><asp:CheckBox ID="CheckBox2" runat="server" Text="Martes:"/></td>
                                                <td><asp:TextBox ID="txtBoxMartes" runat="server" Width="200px" style="overflow:auto;" ></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td><asp:CheckBox ID="CheckBox3" runat="server" Text="Miercoles:"/></td>
                                                <td><asp:TextBox ID="txtBoxMiercoles" runat="server" Width="200px" style="overflow:auto;" ></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td><asp:CheckBox ID="CheckBox4" runat="server" Text="Jueves:"/></td>
                                                <td><asp:TextBox ID="txtBoxJueves" runat="server" Width="200px" style="overflow:auto;" ></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td><asp:CheckBox ID="CheckBox5" runat="server" Text="Viernes:"/></td>
                                                <td><asp:TextBox ID="txtBoxViernes" runat="server" Width="200px" style="overflow:auto;" ></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td><asp:CheckBox ID="CheckBox6" runat="server" Text="Sabado:" /></td>
                                                <td><asp:TextBox ID="txtBoxSabado" runat="server" Width="200px" style="overflow:auto;" ></asp:TextBox></td>
                                            </tr>   
                                        </table>
                                    </fieldset>
                                </td>    
                            </tr> 
                            <tr>
                                <td><asp:Label      ID="Label3"         runat="server" Text="Servidor Base Datos:"></asp:Label></td>    
                                <td><asp:TextBox    ID="txtBoxServBD"   runat="server" Width="350px"></asp:TextBox></td>
                            </tr> 
                        </table>
                        </fieldset>
                </td>
            </tr>          
            <tr>
                <td colspan="6">
                     <div class="BlockText">
                        <asp:Label ID="lblDescripcionGoogleMap" runat="server" Text="Seleccione por favor el punto geográfico en el mapa y haga clic cuando este listo en el botón guardar." SkinID="lblBlockText"></asp:Label>            
                    </div> 
                </td>
            </tr>
            <tr>
                <td colspan="6">
                   <asp:Button ID="btbGuardar" runat="server" Text="Guardar Punto" CssClass="formbutton" OnClick="btbGuardar_Click"/>
                </td>
            </tr>
            <tr>
                <td colspan="6"> 
                     <div id="map_canvas" style="height:400px;">
                     </div>
                </td>
            </tr>            
        </table>               
    </form>
    </fieldset>
    
</asp:Content>
