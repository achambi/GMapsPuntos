<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="VisorPuntosGMaps.aspx.cs" Inherits="UserInterfaz.VisorPuntosGMaps" %>
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
       var markermaps;
       function initialize()
       {
            geocoder = new google.maps.Geocoder();
            var mapOptions =
            {
                center   : new google.maps.LatLng(-16.46348017, -64.81933593),
                zoom     : 5,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
       }

       $(document).ready
       (
           function ()
           {
               try
               {
                   initialize();
                   $("#<%=ddlTipoPuntos.ClientID%>").msDropDown();
                   selectDepartamento();
                   fillMapDataButton();
               }
               catch (e)
               {
                    alert(e.message);
               }
            }
        );

        function selectDepartamento()
        {
            $("#<%=ddlDepartamentos.ClientID%>").on
             ("change",
                 function (e)
                 {
                     var optionSelected = $("option:selected", this);
                     var textSelected = optionSelected.filter(":selected").text();
                     codeAddress((textSelected == "--TODOS LOS DEPARTAMENTOS--") ? "BOLIVIA" : "BOLIVIA," + textSelected,(textSelected == "--TODOS LOS DEPARTAMENTOS--") ?5:7);
                 }
             );
        }

        function fillMapData()
        {
            if (markermaps) 
                clearMarkers();
            var jsonMarkets;
            var tipoPunto      = $("#<%=ddlTipoPuntos.ClientID%>");
            var departamentoId = $("#<%=ddlDepartamentos.ClientID%>");
            var urlRequest = "http://localhost:29861/MarketAdapter.aspx?tipoPunto=" + tipoPunto.val() + "&departamentoId=" + departamentoId.val();
            $.ajax
            (
                {
                    url     :   urlRequest,
                    dataType:   "json",
                    success :   function (response)
                                {
                                    jsonMarkets = response;
                                    if (jsonMarkets.success)
                                    {
                                        var infoWindow = new google.maps.InfoWindow();
                                        markermaps = new Array();
                                        for (i = 0; i < jsonMarkets.listPunto.length; i++)
                                        {
                                            var data      = jsonMarkets.listPunto[i];
                                            var myLatlng  = new google.maps.LatLng(data.Latitud, data.Longitud);
                                            markermaps[i] = new google.maps.Marker
                                            (
                                                {
                                                    position: myLatlng,
                                                    map     : map,
                                                    title   : data.Nombre,
                                                    icon    : "App_Themes/GMapsTheme/images/ImageIcon/" + data.IconUrl
                                                }
                                            );
                                            (
                                                function (marker, data)
                                                {
                                                    google.maps.event.addListener
                                                    (
                                                                marker,
                                                                "click",
                                                                function (e)
                                                                {
                                                                    infoWindow.setContent(data.htmlDescripcion);
                                                                    infoWindow.open(map, marker);
                                                                }
                                                   );
                                                    google.maps.event.addListener
                                                    (            marker,
                                                                'dblclick',
                                                                function ()
                                                                {
                                                                    map.setZoom(15);
                                                                    map.setCenter(marker.getPosition());
                                                                    alert("alerta doble clic");
                                                                }
                                                    );

                                                }
                                            )(markermaps[i], data);
                                        }
                                    }
                                }
                }
            );            
        }

        function fillMapDataButton()
        {
            var button = $("#btnBuscar").click
                (
                    function ()
                    {
                        fillMapData();
                    }
                );
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
                    }
                    else
                    {
                        alert("Geocode was not successful for the following reason: " + status);
                    }
                }
            );
        }

        // Sets the map on all markers in the array.
        function setAllMap(mapItem)
        {
            for (var i = 0; i < markermaps.length; i++)
            {
                markermaps[i].setMap(mapItem);
            }
        }

        // Removes the markers from the map, but keeps them in the array.
        function clearMarkers()
        {
            setAllMap(null);
        }

        // Deletes all markers in the array by removing references to them.
        function deleteMarkers()
        {
            clearMarkers();
            markermaps = [];
        }

        function visibleHide(element, flag)
        {
            if (flag)
                $("#" + element).show("slow");
            else
                $("#" + element).hide("slow");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMaster" runat="server">
<fieldset>
  <legend>Visor de Puntos</legend>
  <form id="FormGMaps" runat="server">                       
    <table>
        <tr>
            <td>
                <asp:Label ID="lblTipo" runat="server" Text="Tipos de Puntos:"></asp:Label>
            </td>
            <td>
                 <asp:DropDownList ID="ddlTipoPuntos" runat="server" Width="231px"></asp:DropDownList>            
            </td>
            <td>
                 <input id="btnBuscar" type="button" value="Buscar Puntos" class="formbuttonLeft" />
            </td>
        </tr>
        <tr>
            <td>
                    <asp:Label ID="lblLatitud0" runat="server" Text="Departamento"></asp:Label>
                </td>
            <td colspan="2">
                    <asp:DropDownList ID="ddlDepartamentos" runat="server" Width="470px" CssClass="formSelected"></asp:DropDownList>
                </td>
        </tr> 
        <tr>
            <td colspan="3">
                <h3>Modificacion de Punto</h3>
                <table>
                <tr>
                        <td>
                        <fieldset id="fieldDatosPunto">
                        <legend>Datos Punto Geografico</legend>
                         <table>
                <tr>
                    <td><asp:Label ID="lblNombre" runat="server" Text="Nombre:"></asp:Label></td>
                    <td><asp:TextBox ID="txtBoxNombre" runat="server" Width="220px"></asp:TextBox></td>
                    <td><asp:RequiredFieldValidator ID="RequiredFieldValidatorTxtBoxNombre" runat="server" ErrorMessage="*" ControlToValidate="txtBoxNombre" SetFocusOnError ="true" ValidationGroup="FormValidation"></asp:RequiredFieldValidator></td>
                    <td><asp:Label ID="lblTipoPunto" runat="server" Text="Tipo de Punto:"></asp:Label></td>    
                    <td colspan="2"><asp:DropDownList ID="DropDownList1" runat="server" Width="231px" Enabled="true"></asp:DropDownList></td>
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
                    <td><asp:Label ID="lblDireccion" runat="server" Text="Direccion:"></asp:Label></td>
                    <td colspan="3"><asp:TextBox ID="txtBoxDireccion" runat="server" Width="350px" style="overflow:auto;" ></asp:TextBox></td>
                    <td>
                        &nbsp;</td>
                    <td><asp:RequiredFieldValidator ID="RequiredFieldValidatorTxtBoxDireccion" runat="server" ErrorMessage="*" ControlToValidate="txtBoxDireccion" SetFocusOnError ="true" ValidationGroup="FormValidation"></asp:RequiredFieldValidator></td>
                </tr>         
            </table>
                        </fieldset>
                        </td>
                 </tr>   
                <tr>
                    <td>
                        <fieldset id="fieldDatosCajero">
                                <legend>Datos Cajero Automatico</legend>
                                <table width="100%" id="atmDataFields">
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
                </table>
            </td>
        </tr>        
        <tr>
            <td colspan="3">
                <asp:ScriptManager ID="ScriptManager1" runat="server">
                </asp:ScriptManager>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">            
                    <ContentTemplate>
                        <fieldset>
                        <legend>Mapa</legend>  
                          <div id="map_canvas" style="height:400px;">
                          </div>  
                        </fieldset>         
                    </ContentTemplate>                   
                </asp:UpdatePanel> 
            </td>
        </tr>
    </table>                     
    
  </form> 
</fieldset>  
</asp:Content>
