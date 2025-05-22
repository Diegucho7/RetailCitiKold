using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using RetailCitiKold.Domain.DataAccess.Context;
using RetailCitiKold.Domain.Dtos.Request;
using RetailCitiKold.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace RetailCitiKold.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class FacturaOrdenCompraController : ControllerBase

    {

        private readonly EProductionDbContext _context;
        private readonly IntegrationDbContext _contextIntegrationDb;

        // private readonly IFacturaOdernDeCompraRepository _repository;

        public FacturaOrdenCompraController(/*IFacturaOdernDeCompraRepository repository, */EProductionDbContext context, IntegrationDbContext contextIntegrationDb)
        {
            // _repository = repository;
            _context = context;
            _contextIntegrationDb = contextIntegrationDb;
        }


        // GET: FacturaOrdenCompraController

        // POST: FacturaOrdenCompraController/Create


        [HttpGet("factComprobanteCompra/{id}")]
        public async Task<ActionResult<FacturaCompraDto>> GetfactComprobanteCompra(int id)
        {
            //try
            //{
            //    var result = await _repository.AsentarComprobante(id);
            //    return result ? Ok(result) : BadRequest();
            //}
            //catch (Exception)
            //{
            //    return BadRequest();
            //}
            try
            {
                var Comprobante = await _context.Comprobante.FindAsync(id);
                var ComprobanteCompra = await _context.ComprobanteCompra.FindAsync(id);
                var ComprobanteCompraDetalle = await _context.ComprobanteCompraDetalle.Where(a => a.id_comprobanteCompra == id).ToListAsync();


                var SustentoTributario = await _context.SustentoTributario.FindAsync(ComprobanteCompra.id_sustentoTributario);



                var persona = await _context.Persona.FindAsync(ComprobanteCompra.id_proveedor);
                var TipoComprobante = await _context.TipoComprobante.FindAsync(Comprobante.id_tipoComprobante);


                var usuario = await _context.User.FindAsync(Comprobante.id_usuarioCreacion);



                var resultado = await (from comprobante in _context.ComprobanteCompraDetalle
                                       join items in _context.Item
                                       on comprobante.id_item equals items.id
                                       join medida in _context.UnidadMedida
                                       on comprobante.id_unidadMedida equals medida.id
                                       where comprobante.id_comprobanteCompra == id
                                       select new ItemFacatDto
                                       {
                                           CodigoItem = items.codigo,
                                           NombreItem = items.nombre,
                                           UnidadMedida = medida.simbolo,
                                           Cantidad = comprobante.cantidad,
                                           Precio = comprobante.precio,
                                           IVA = (from imp in _context.ImpuestoItem
                                                  join tipo in _context.TipoImpuesto
                                                  on imp.id_tipoImpuesto equals tipo.id
                                                  join ta in _context.Tarifa
                                                  on imp.id_tarifa equals ta.id
                                                  where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 1
                                                  select ta.porcentaje == 0 ? "N" :
                                                         ta.porcentaje < 0 ? "S" :
                                                         (string?)null).FirstOrDefault(),
                                           ValorItemIVA = (
                                                            (from imp in _context.ImpuestoItem
                                                             join tipo in _context.TipoImpuesto
                                                             on imp.id_tipoImpuesto equals tipo.id
                                                             join ta in _context.Tarifa
                                                             on imp.id_tarifa equals ta.id
                                                             where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 1
                                                             select imp.id_tarifa == 1 ? ta.porcentaje : (decimal?)null).FirstOrDefault())
                                                             * comprobante.subtotalNeto / 100,
                                           SubTotal = comprobante.subtotalNeto,
                                           TotalItem = comprobante.subtotalNeto + (
                                                (from imp in _context.ImpuestoItem
                                                 join tipo in _context.TipoImpuesto
                                                 on imp.id_tipoImpuesto equals tipo.id
                                                 join ta in _context.Tarifa
                                                 on imp.id_tarifa equals ta.id
                                                 where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 1
                                                 select imp.id_tarifa == 1 ? ta.porcentaje : (decimal?)null).FirstOrDefault()
                                            ) * comprobante.subtotalNeto / 100,
                                           TipoRetencion = (from imp in _context.ImpuestoItem
                                                            join tipo in _context.TipoImpuesto
                                                            on imp.id_tipoImpuesto equals tipo.id
                                                            join ta in _context.Tarifa
                                                            on imp.id_tarifa equals ta.id
                                                            where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 4
                                                            select tipo.codigoSri == 1 ? "F" :
                                                                   tipo.codigoSri == 2 ? "R" :
                                                                   (string?)null).FirstOrDefault(),
                                           CodigoRetencion = (from imp in _context.ImpuestoItem
                                                              join tipo in _context.TipoImpuesto
                                                              on imp.id_tipoImpuesto equals tipo.id
                                                              join ta in _context.Tarifa
                                                              on imp.id_tarifa equals ta.id
                                                              where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 4
                                                              select imp.id_tarifa == 57 ? ta.codigoSRI :
                                                                     //tipo.id == 1 ? "R" :
                                                                     (string?)null).FirstOrDefault(),
                                           PorcentajeRetencion = (from imp in _context.ImpuestoItem
                                                                  join tipo in _context.TipoImpuesto
                                                                  on imp.id_tipoImpuesto equals tipo.id
                                                                  join ta in _context.Tarifa
                                                                  on imp.id_tarifa equals ta.id
                                                                  where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 4
                                                                  select imp.id_tarifa == 57 ? ta.porcentaje :
                                                                         //tipo.id == 1 ? "R" :
                                                                         (decimal?)null).FirstOrDefault(),
                                           MontoRetenido = (from imp in _context.ImpuestoItem
                                                            join tipo in _context.TipoImpuesto on imp.id_tipoImpuesto equals tipo.id
                                                            join ta in _context.Tarifa on imp.id_tarifa equals ta.id
                                                            where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 4
                                                            select imp.id_tarifa == 57 ? (decimal?)ta.porcentaje : null).FirstOrDefault() * comprobante.subtotalNeto / 100
                                       }).ToListAsync();
                var totalMontoRetenido = resultado.Sum(r => r.MontoRetenido ?? 0);
                var totalIva = resultado.Sum(r => r.ValorItemIVA ?? 0);

                var dto = new FacturaCompraDto
                {
                    CodigoPrincipal = Comprobante.id.ToString(),
                    NoDocumento = Comprobante.numeroComprobante,
                    FechaDocumento = DateOnly.FromDateTime(DateTime.Parse(Comprobante.fechaEmision)),
                    TipoDocumento = TipoComprobante.nombre,
                    Observacion = Comprobante.observacion,
                    IdentificacionProveedor = persona.numeroIdentificacion,
                    UsuarioIngreso = usuario.fullname,
                    CentroCosto = persona.numeroIdentificacion,
                    SustentoTributario = SustentoTributario.nombre,
                    BaseCERO = ComprobanteCompra.baseIvaCero,
                    BaseIVA = ComprobanteCompra.baseGrabaIva,
                    IVA = ComprobanteCompra.baseGrabaIva > 0 ? "S" : "N",
                    ValorIVA = totalIva,
                    TotalBienIVA = ComprobanteCompra.baseGrabaIva,
                    TotalBienServicio = ComprobanteCompra.totalDocumento,
                    TotalPagar = ComprobanteCompra.totalAPagar,
                    ValorRetencion = totalMontoRetenido,
                    NetoPagar = ComprobanteCompra.totalAPagar,

                    Items = resultado,

                    CCeProceso = true,
                    CNoError = null



                };

                return StatusCode(200, (dto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { details = ex.Message });

            }


        }



        [HttpPost("factComprobanteCompra/{id}")]
        public async Task<ActionResult<FacturaCompraDto>> PostfactComprobanteCompra(int id, [FromBody] OrdenCompraLoginPost data)
        {
            using var client = new HttpClient();

            // Paso 1: Hacer login a través del endpoint real
            var loginBody = new
            {
                username = data.Username,
                password = data.Password
            };

            //var loginResponse = await client.PostAsync("https://localhost:7114/login",
            var loginResponse = await client.PostAsync("https://lanecmovil.com:8447/WebAPI/api/login/authenticate",
                 new StringContent(JsonConvert.SerializeObject(loginBody), Encoding.UTF8, "application/json"));

            if (!loginResponse.IsSuccessStatusCode)
                return Unauthorized("Login fallido desde /login");

            var loginResultString = await loginResponse.Content.ReadAsStringAsync();
            dynamic loginResult = JsonConvert.DeserializeObject(loginResultString);
            string token = JsonConvert.DeserializeObject<string>(loginResultString);


            if (string.IsNullOrEmpty(token))
            {

                return Unauthorized("No se pudo obtener un token válido.");
            }

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token);




            try
            {
                var Comprobante = await _context.Comprobante.FindAsync(id);
                var ComprobanteCompra = await _context.ComprobanteCompra.FindAsync(id);
                var ComprobanteCompraDetalle = await _context.ComprobanteCompraDetalle.Where(a => a.id_comprobanteCompra == id).ToListAsync();


                var SustentoTributario = await _context.SustentoTributario.FindAsync(ComprobanteCompra.id_sustentoTributario);


                var persona = await _context.Persona.FindAsync(ComprobanteCompra.id_proveedor);
                var TipoComprobante = await _context.TipoComprobante.FindAsync(Comprobante.id_tipoComprobante);


                var usuario = await _context.User.FindAsync(Comprobante.id_usuarioCreacion);



                var resultado = await (from comprobante in _context.ComprobanteCompraDetalle
                                       join items in _context.Item
                                       on comprobante.id_item equals items.id
                                       join medida in _context.UnidadMedida
                                       on comprobante.id_unidadMedida equals medida.id
                                       where comprobante.id_comprobanteCompra == id
                                       select new ItemFacatDto
                                       {
                                           CodigoItem = items.codigo,
                                           NombreItem = items.nombre,
                                           UnidadMedida = medida.simbolo,
                                           Cantidad = comprobante.cantidad,
                                           Precio = comprobante.precio,
                                           IVA = (from imp in _context.ImpuestoItem
                                                  join tipo in _context.TipoImpuesto
                                                  on imp.id_tipoImpuesto equals tipo.id
                                                  join ta in _context.Tarifa
                                                  on imp.id_tarifa equals ta.id
                                                  where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 1
                                                  select ta.porcentaje == 0 ? "N" :
                                                         ta.porcentaje < 0 ? "S" :
                                                         (string?)null).FirstOrDefault(),
                                           ValorItemIVA = (
                                                            (from imp in _context.ImpuestoItem
                                                             join tipo in _context.TipoImpuesto
                                                             on imp.id_tipoImpuesto equals tipo.id
                                                             join ta in _context.Tarifa
                                                             on imp.id_tarifa equals ta.id
                                                             where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 1
                                                             select imp.id_tarifa == 1 ? ta.porcentaje : (decimal?)null).FirstOrDefault())
                                                             * comprobante.subtotalNeto / 100,
                                           SubTotal = comprobante.subtotalNeto,
                                           TotalItem = comprobante.subtotalNeto + (
                                                (from imp in _context.ImpuestoItem
                                                 join tipo in _context.TipoImpuesto
                                                 on imp.id_tipoImpuesto equals tipo.id
                                                 join ta in _context.Tarifa
                                                 on imp.id_tarifa equals ta.id
                                                 where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 1
                                                 select imp.id_tarifa == 1 ? ta.porcentaje : (decimal?)null).FirstOrDefault()
                                            ) * comprobante.subtotalNeto / 100,
                                           TipoRetencion = (from imp in _context.ImpuestoItem
                                                            join tipo in _context.TipoImpuesto
                                                            on imp.id_tipoImpuesto equals tipo.id
                                                            join ta in _context.Tarifa
                                                            on imp.id_tarifa equals ta.id
                                                            where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 4
                                                            select tipo.codigoSri == 1 ? "F" :
                                                                   tipo.codigoSri == 2 ? "R" :
                                                                   (string?)null).FirstOrDefault(),
                                           CodigoRetencion = (from imp in _context.ImpuestoItem
                                                              join tipo in _context.TipoImpuesto
                                                              on imp.id_tipoImpuesto equals tipo.id
                                                              join ta in _context.Tarifa
                                                              on imp.id_tarifa equals ta.id
                                                              where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 4
                                                              select imp.id_tarifa == 57 ? ta.codigoSRI :
                                                                     //tipo.id == 1 ? "R" :
                                                                     (string?)null).FirstOrDefault(),
                                           PorcentajeRetencion = (from imp in _context.ImpuestoItem
                                                                  join tipo in _context.TipoImpuesto
                                                                  on imp.id_tipoImpuesto equals tipo.id
                                                                  join ta in _context.Tarifa
                                                                  on imp.id_tarifa equals ta.id
                                                                  where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 4
                                                                  select imp.id_tarifa == 57 ? ta.porcentaje :
                                                                         //tipo.id == 1 ? "R" :
                                                                         (decimal?)null).FirstOrDefault(),
                                           MontoRetenido = (from imp in _context.ImpuestoItem
                                                            join tipo in _context.TipoImpuesto on imp.id_tipoImpuesto equals tipo.id
                                                            join ta in _context.Tarifa on imp.id_tarifa equals ta.id
                                                            where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 4
                                                            select imp.id_tarifa == 57 ? (decimal?)ta.porcentaje : null).FirstOrDefault() * comprobante.subtotalNeto / 100
                                       }).ToListAsync();
                var totalMontoRetenido = resultado.Sum(r => r.MontoRetenido ?? 0);
                var totalIva = resultado.Sum(r => r.ValorItemIVA ?? 0);

                var dto = new FacturaCompraDto
                {
                    CodigoPrincipal = Comprobante.id.ToString(),
                    NoDocumento = Comprobante.numeroComprobante,
                    FechaDocumento = DateOnly.FromDateTime(DateTime.Parse(Comprobante.fechaEmision)),
                    TipoDocumento = TipoComprobante.nombre,
                    Observacion = Comprobante.observacion,
                    IdentificacionProveedor = persona.numeroIdentificacion,
                    UsuarioIngreso = usuario.fullname,
                    CentroCosto = persona.numeroIdentificacion,
                    SustentoTributario = SustentoTributario.nombre,
                    BaseCERO = ComprobanteCompra.baseIvaCero,
                    BaseIVA = ComprobanteCompra.baseGrabaIva,
                    IVA = ComprobanteCompra.baseGrabaIva > 0 ? "S" : "N",
                    ValorIVA = totalIva,
                    TotalBienIVA = ComprobanteCompra.baseGrabaIva,
                    TotalBienServicio = ComprobanteCompra.totalDocumento,
                    TotalPagar = ComprobanteCompra.totalAPagar,
                    ValorRetencion = totalMontoRetenido,
                    NetoPagar = ComprobanteCompra.totalAPagar,

                    Items = resultado,

                    CCeProceso = true,
                    CNoError = "NO"



                };

                // Verificar que la solicitud del cliente tenga el encabezado Authorization correctamente
                //var authorizationHeader = client.DefaultRequestHeaders.Authorization;
                //if (authorizationHeader == null || string.IsNullOrEmpty(authorizationHeader.Parameter))
                //{
                //    return Unauthorized("No se está enviando el token correctamente.");
                //}


                ///
                var jsonBody = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");




                //foreach (var dto in dtoList)
                //{
                var ordenCompraResponse = await client.PostAsync(
                    "https://localhost:7114/api/FacturaOrdenCompra",
                    new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json")
                );
                string responseBody = await ordenCompraResponse.Content.ReadAsStringAsync();
                // Deserializa el objeto que devuelve el servidor
                var objetoCreado = JsonConvert.DeserializeObject<AsientoDiarioOrdenCompra>(responseBody);


                var ordenCompraClientResponse = await client.PostAsync(
                    //"https://localhost:7114/api/FacturaOrdenCompra",  
                    $"https://lanecmovil.com:8447/WebAPI/api/spc/fact?prmCodigoPrincipal={objetoCreado.id}",
                    new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json")
                );
                if (ordenCompraClientResponse.IsSuccessStatusCode)
                {




                    var patchPayload = new StringContent("true", Encoding.UTF8, "application/json");
                    var patchResponse = await client.PatchAsync(
                            $"https://localhost:7114/api/AsientoDiarioOrdenCompras/{objetoCreado.id}", patchPayload
                        );
                }
                // Puedes manejar errores aquí si quieres
                if (!ordenCompraResponse.IsSuccessStatusCode)
                {
                    return StatusCode(500, new { details = "ex.Message" });
                    string errorJson = await ordenCompraResponse.Content.ReadAsStringAsync();
                    Console.WriteLine(errorJson);
                }
                //}




                return Ok(new
                {
                    AsientosEnviados = dto



                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { details = ex.Message });

            }


        }



        // GET: FacturaOrdenCompraController/Delete/5


        // POST: FacturaOrdenCompraController/Delete/5
        [HttpPost]
        public async Task<IActionResult> PostFactura([FromBody] FacturaCompra factura)
        {
            if (factura == null)
            {
                return BadRequest("La factura no puede ser nula.");
            }

            if (string.IsNullOrEmpty(factura.codigoPrincipal))
            {
                return BadRequest("El campo 'CodigoPrincipal' es obligatorio y no puede ser nulo.");
            }

            // Aquí puedes hacer validaciones adicionales si es necesario

            _contextIntegrationDb.FacturaCompras.Add(factura);  // Agregar a la base de datos
            await _contextIntegrationDb.SaveChangesAsync();  // Guardar cambios

            // Configurar las opciones de serialización para manejar ciclos
            var opciones = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true // Esto hace que el JSON sea más legible
            };

            // Serializar el objeto factura con las opciones personalizadas
            var json = JsonSerializer.Serialize(factura, opciones);

            // Devolver la respuesta con el objeto serializado en formato JSON
            return Content(json, "application/json");
        }


        //[HttpPatch("{codigoPrincipal}/activar-items")]
        //public async Task<IActionResult> ActivarItemsFactura(string codigoPrincipal)
        //{
        //    if (string.IsNullOrEmpty(codigoPrincipal))
        //    {
        //        return BadRequest("El código principal no puede estar vacío.");
        //    }
        //    // Buscar todos los ítems de factura relacionados con la factura que tiene el codigoPrincipal
        //    int codigoPrincipalInt;
        //    if (int.TryParse(codigoPrincipal, out codigoPrincipalInt))
        //    {
        //        var itemsFactura = await _contextIntegrationDb.ItemFactura
        //            .Where(i => i.FacturaCompraId == codigoPrincipal)
        //            .ToListAsync();

        //        foreach (var item in itemsFactura)
        //        {
        //            item.Estado = true; // Activar el estado
        //        }
        //    }
        //    else
        //    {
        //        // Maneja el caso en que la conversión no sea posible
        //        // Por ejemplo, podrías lanzar una excepción o retornar un resultado vacío
        //    }

        //    //if (itemsFactura == null || itemsFactura.Count == 0)
        //    //{
        //    //    return NotFound("No se encontraron ítems asociados con esta factura.");
        //    //}

        //    // Cambiar el estado de todos los ítems a true


        //    // Guardar los cambios en la base de datos
        //    await _contextIntegrationDb.SaveChangesAsync();

        //    return Ok("Todos los ítems de la factura fueron activados.");
        //}



        [HttpPost("factCliente/{id}")]
        //public async Task<ActionResult<FacturaCompraDto>> PostfcaturaCliente(int id, [FromBody] OrdenCompraLoginPost data)
        public async Task<IActionResult> PostfcaturaCliente(int id, [FromBody] OrdenCompraLoginPost data)
        {
            using var client = new HttpClient();

            // Paso 1: Hacer login a través del endpoint real
            var loginBody = new
            {
                username = data.Username,
                password = data.Password
            };
            //esto es para local
            //var loginResponse = await client.PostAsync("https://localhost:7114/login",
            var loginResponse = await client.PostAsync("https://lanecmovil.com:8447/WebAPI/api/login/authenticate",
                new StringContent(JsonConvert.SerializeObject(loginBody), Encoding.UTF8, "application/json"));

            if (!loginResponse.IsSuccessStatusCode)
                return Unauthorized("Login fallido desde /login");

            var loginResultString = await loginResponse.Content.ReadAsStringAsync();
            dynamic loginResult = JsonConvert.DeserializeObject(loginResultString);
            //string  token = loginResult.token;
            string token = JsonConvert.DeserializeObject<string>(loginResultString);


            if (string.IsNullOrEmpty(token))
            {

                return Unauthorized("No se pudo obtener un token válido.");
            }

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token);


            try
            {
                var Comprobante = await _context.Comprobante.FindAsync(id);
                var ComprobanteCompra = await _context.ComprobanteCompra.FindAsync(id);
                var ComprobanteCompraDetalle = await _context.ComprobanteCompraDetalle.Where(a => a.id_comprobanteCompra == id).ToListAsync();


                var SustentoTributario = await _context.SustentoTributario.FindAsync(ComprobanteCompra.id_sustentoTributario);


                var persona = await _context.Persona.FindAsync(ComprobanteCompra.id_proveedor);
                var TipoComprobante = await _context.TipoComprobante.FindAsync(Comprobante.id_tipoComprobante);


                var usuario = await _context.User.FindAsync(Comprobante.id_usuarioCreacion);



                var resultado = await (from comprobante in _context.ComprobanteCompraDetalle
                                       join items in _context.Item
                                       on comprobante.id_item equals items.id
                                       join medida in _context.UnidadMedida
                                       on comprobante.id_unidadMedida equals medida.id
                                       where comprobante.id_comprobanteCompra == id
                                       select new ItemFacatDto
                                       {
                                           CodigoItem = items.codigo,
                                           NombreItem = items.nombre,
                                           UnidadMedida = medida.simbolo,
                                           Cantidad = comprobante.cantidad,
                                           Precio = comprobante.precio,
                                           IVA = (from imp in _context.ImpuestoItem
                                                  join tipo in _context.TipoImpuesto
                                                  on imp.id_tipoImpuesto equals tipo.id
                                                  join ta in _context.Tarifa
                                                  on imp.id_tarifa equals ta.id
                                                  where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 1
                                                  select ta.porcentaje == 0 ? "N" :
                                                         ta.porcentaje < 0 ? "S" :
                                                         (string?)null).FirstOrDefault(),
                                           ValorItemIVA = (
                                                            (from imp in _context.ImpuestoItem
                                                             join tipo in _context.TipoImpuesto
                                                             on imp.id_tipoImpuesto equals tipo.id
                                                             join ta in _context.Tarifa
                                                             on imp.id_tarifa equals ta.id
                                                             where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 1
                                                             select imp.id_tarifa == 1 ? ta.porcentaje : (decimal?)null).FirstOrDefault())
                                                             * comprobante.subtotalNeto / 100,
                                           SubTotal = comprobante.subtotalNeto,
                                           TotalItem = comprobante.subtotalNeto + (
                                                (from imp in _context.ImpuestoItem
                                                 join tipo in _context.TipoImpuesto
                                                 on imp.id_tipoImpuesto equals tipo.id
                                                 join ta in _context.Tarifa
                                                 on imp.id_tarifa equals ta.id
                                                 where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 1
                                                 select imp.id_tarifa == 1 ? ta.porcentaje : (decimal?)null).FirstOrDefault()
                                            ) * comprobante.subtotalNeto / 100,
                                           TipoRetencion = (from imp in _context.ImpuestoItem
                                                            join tipo in _context.TipoImpuesto
                                                            on imp.id_tipoImpuesto equals tipo.id
                                                            join ta in _context.Tarifa
                                                            on imp.id_tarifa equals ta.id
                                                            where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 4
                                                            select tipo.codigoSri == 1 ? "F" :
                                                                   tipo.codigoSri == 2 ? "R" :
                                                                   (string?)null).FirstOrDefault(),
                                           CodigoRetencion = (from imp in _context.ImpuestoItem
                                                              join tipo in _context.TipoImpuesto
                                                              on imp.id_tipoImpuesto equals tipo.id
                                                              join ta in _context.Tarifa
                                                              on imp.id_tarifa equals ta.id
                                                              where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 4
                                                              select imp.id_tarifa == 57 ? ta.codigoSRI :
                                                                     //tipo.id == 1 ? "R" :
                                                                     (string?)null).FirstOrDefault(),
                                           PorcentajeRetencion = (from imp in _context.ImpuestoItem
                                                                  join tipo in _context.TipoImpuesto
                                                                  on imp.id_tipoImpuesto equals tipo.id
                                                                  join ta in _context.Tarifa
                                                                  on imp.id_tarifa equals ta.id
                                                                  where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 4
                                                                  select imp.id_tarifa == 57 ? ta.porcentaje :
                                                                         //tipo.id == 1 ? "R" :
                                                                         (decimal?)null).FirstOrDefault(),
                                           MontoRetenido = (from imp in _context.ImpuestoItem
                                                            join tipo in _context.TipoImpuesto on imp.id_tipoImpuesto equals tipo.id
                                                            join ta in _context.Tarifa on imp.id_tarifa equals ta.id
                                                            where imp.id_item == comprobante.id_item && imp.id_tipoImpuesto == 4
                                                            select imp.id_tarifa == 57 ? (decimal?)ta.porcentaje : null).FirstOrDefault() * comprobante.subtotalNeto / 100
                                       }).ToListAsync();
                var totalMontoRetenido = resultado.Sum(r => r.MontoRetenido ?? 0);
                var totalIva = resultado.Sum(r => r.ValorItemIVA ?? 0);


                foreach (var doc in resultado)
                {

                    var factura = new FacturaClienteDto
                    {
                        CodigoPrincipal = Comprobante.id.ToString(),
                        NoDocumento = Comprobante.numeroComprobante,
                        FechaDocumento = DateTime.Parse(Comprobante.fechaEmision),
                        TipoDocumento = TipoComprobante.id.ToString(),
                        Observacion = Comprobante.observacion,
                        IdentificacionProveedor = persona.numeroIdentificacion,
                        UsuarioIngreso = usuario.fullname,
                        CentroCosto = persona.numeroIdentificacion,
                        SustentoTributario = SustentoTributario.codigoSRI,
                        BaseCERO = ComprobanteCompra.baseIvaCero.Value,
                        BaseIVA = ComprobanteCompra.baseGrabaIva.Value,
                        IVA = ComprobanteCompra.baseGrabaIva > 0 ? "S" : "N",
                        ValorIVA = totalIva,
                        TotalBienIVA = ComprobanteCompra.baseGrabaIva.Value,
                        TotalBienServicio = ComprobanteCompra.totalDocumento.Value,
                        TotalPagar = ComprobanteCompra.totalAPagar.Value,
                        ValorRetencion = totalMontoRetenido,
                        NetoPagar = ComprobanteCompra.totalAPagar.Value,

                        CodigoItem = doc.CodigoItem,
                        NombreItem = doc.NombreItem,
                        UnidadMedida = doc.UnidadMedida,
                        Cantidad = doc.Cantidad.Value,
                        Precio = doc.Precio.Value,
                        IVA_Item = doc.IVA,
                        ValorItemIVA = doc.ValorItemIVA.Value,
                        SubTotal = doc.SubTotal.Value,
                        TotalItem = doc.TotalItem.Value,
                        TipoRetencion = doc.TipoRetencion,
                        CodigoRetencion = doc.CodigoRetencion,
                        PorcentajeRetencion = doc.PorcentajeRetencion,
                        MontoRetenido = doc.MontoRetenido,

                        CCeProceso = true,
                        CNoError = "NO"
                    };


                    // Verificar que la solicitud del cliente tenga el encabezado Authorization correctamente
                    //var authorizationHeader = client.DefaultRequestHeaders.Authorization;
                    //if (authorizationHeader == null || string.IsNullOrEmpty(authorizationHeader.Parameter))
                    //{
                    //    return Unauthorized("No se está enviando el token correctamente.");
                    //}


                    ///
                    var jsonBody = JsonConvert.SerializeObject(factura);
                    var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");




                    //foreach (var dto in dtoList)
                    //{
                    //var ordenCompraResponse = await client.PostAsync(
                    //    "https://localhost:7114/api/FacturaOrdenCompra",
                    //    new StringContent(JsonConvert.SerializeObject(factura), Encoding.UTF8, "application/json")
                    //);
                    //string responseBody = await ordenCompraResponse.Content.ReadAsStringAsync();
                    // Deserializa el objeto que devuelve el servidor
                    //var objetoCreado = JsonConvert.DeserializeObject<AsientoDiarioOrdenCompra>(responseBody);


                    var ordenCompraClientResponse = await client.PostAsync(
                          //"https://localhost:7114/api/FacturaOrdenCompra",  

                          //$"https://lanecmovil.com:8447/api/spc/cxp?prmCodigoPrincipal={Comprobante.id}",
                          $"https://lanecmovil.com:8447/api/spc/cxp?prmCodigoPrincipal={Comprobante.id}",  
                        new StringContent(JsonConvert.SerializeObject(factura), Encoding.UTF8, "application/json")
                    );

                }



                //if (ordenCompraClientResponse.IsSuccessStatusCode)
                //{




                //    var patchPayload = new StringContent("true", Encoding.UTF8, "application/json");
                //    var patchResponse = await client.PatchAsync(
                //            $"https://localhost:7114/api/AsientoDiarioOrdenCompras/{objetoCreado.id}", patchPayload
                //        );
                //}
                //// Puedes manejar errores aquí si quieres
                //if (!ordenCompraResponse.IsSuccessStatusCode)
                //{
                //    return StatusCode(500, new { details = "ex.Message" });
                //    string errorJson = await ordenCompraResponse.Content.ReadAsStringAsync();
                //    Console.WriteLine(errorJson);
                //}
                //}




                return Ok(new
                {
                    AsientosEnviados = "ok"



                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { details = ex.Message });

            }


        }


    }












}
