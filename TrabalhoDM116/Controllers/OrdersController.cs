using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using TrabalhoDM116.Models;
using TrabalhoDM116.br.com.correios.ws;
using TrabalhoDM116.CRMClient;
using System.Globalization;

namespace TrabalhoDM116.Controllers
{
    [Authorize]
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        private TrabalhoDM116Context db = new TrabalhoDM116Context();

        // GET: api/Orders
        [Authorize(Roles = "ADMIN")]
        public List<Order> GetOrders()
        {
            //return db.Orders;
            return db.Orders.Include(order => order.OrderItems).ToList();
        }

        // GET: api/Orders/5
        [Authorize(Roles = "ADMIN, USER")]
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return BadRequest("Order not found!");
            }
            else if ((User.IsInRole("ADMIN") || (User.Identity.Name == order.email)))
            {
                return Ok(order);
            }
            else
            {
                return BadRequest("This user does not have rights to access this order");
            }

        }

        // GET: api/Orders/byemail?email={email}
        [Authorize(Roles = "ADMIN, USER")]
        [ResponseType(typeof(Order))]
        [HttpGet]
        [Route("byemail")]
        public IHttpActionResult GetOrderByEmail(string email)
        {
            var orders = db.Orders.Where(p => p.email == email);
            if (orders == null)
            {
                return BadRequest("Order not found!");
            }
            else if ((User.IsInRole("ADMIN") || (User.Identity.Name == orders.First().email)))
            {
                return Ok(orders.Include(order => order.OrderItems).ToList());
            }
            else
            {
                return BadRequest("This user does not have rights to access this order");
            }

        }

        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Orders
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            order.email = User.Identity.Name;
            order.dataOrder = DateTime.Now;
            order.dataEntrega = DateTime.Now;
            order.status = "novo";
            order.precoTotal = 0;
            order.pesoTotal = 0;
            order.precoFrete = 0;

            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.Id }, order);
        }

        // Put: api/Orders/closeOrder?id={id}
        [Authorize(Roles = "ADMIN, USER")]
        [ResponseType(typeof(Order))]
        [HttpPut]
        [Route("closeOrder")]
        public IHttpActionResult PutCloseOrder(int id)
        {
            var order = db.Orders.Find(id);

            if (order != null)
            {
                if (User.IsInRole("ADMIN") || (User.Identity.Name == order.email))
                {
                    if (order.precoFrete != 0)
                    {
                        order.status = "fechado";
                        db.Entry(order).State = EntityState.Modified;
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!OrderExists(id))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                        return Ok("Order closed");
                    }
                    else
                    {
                        return BadRequest("Shipment not yet calculated");
                    }
                }
                else
                {
                    return BadRequest("This user does not have rights to access this order");
                }
            }
            else
            {
                return BadRequest("Order not found");
            }
        }

        // DELETE: api/Orders/5
        [Authorize(Roles = "ADMIN, USER")]
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return BadRequest("Order not Found");
            }
            else if ((User.IsInRole("ADMIN")) || (User.Identity.Name == order.email))
            {
                db.Orders.Remove(order);
                db.SaveChanges();

                return Ok(order);
            }
            else
            {
                return BadRequest("This user does not have rights to access this order");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.Id == id) > 0;
        }


        // PUT: api/Pedidos/calcShipment?id={id}
        [Authorize(Roles = "ADMIN, USER")]
        [ResponseType(typeof(Order))]
        [HttpPut]
        [Route("calcShipment")]
        public IHttpActionResult PutCalcShipment(int id)
        {
            Order orders = db.Orders.Find(id);
            if (orders != null)
            {
                if (((User.IsInRole("ADMIN")) || orders.email == User.Identity.Name))
                {
                    if ((orders.status == "novo") && (orders.precoFrete == 0))
                    {
                        try
                        {
                            string cep = FindCep(orders.email);
                            if (cep.Equals("FAULT"))
                            {
                                return BadRequest("Impossible to Access CRM service");
                            }
                            else if (cep.Equals("NOCUSTOMER"))
                            {
                                return BadRequest("User is not registered");
                            }
                            else
                            {
                                List<OrderItem> item = orders.OrderItems.ToList();

                                decimal largura = 0;
                                decimal altura = 0;
                                decimal comprimento = 0;
                                decimal diametro = 0;
                                decimal peso = 0;
                                decimal preco = 0;

                                if (item.Count != 0)
                                {
                                    for (int i = 0; i < item.Count(); i++)
                                    {
                                        altura = altura + (item[i].product.altura * item[i].quantidade);
                                        peso = peso + (item[i].product.peso * item[i].quantidade);
                                        preco = preco + (item[i].product.preco * item[i].quantidade);

                                        if (item[i].product.comprimento > comprimento)
                                        {
                                            comprimento = item[i].product.comprimento;
                                        }
                                        if (item[i].product.largura > largura)
                                        {
                                            largura = item[i].product.largura;
                                        }
                                        if (item[i].product.diametro > diametro)
                                        {
                                            diametro = item[i].product.diametro;
                                        }
                                    }

                                    CalcPrecoPrazoWS correios = new CalcPrecoPrazoWS();
                                    cResultado resultado = correios.CalcPrecoPrazo("", "", "40010", "37540000", cep,
                                        peso.ToString(), 1, comprimento, altura, largura, diametro, "N", preco, "S");

                                    if (resultado.Servicos[0].Erro.Equals("0"))
                                    {
                                        //String shipment;
                                        //shipment = "Valor Frete: " + resultado.Servicos[0].Valor + " Prazo de Entrega: " + resultado.Servicos[0].PrazoEntrega + " dias";

                                        NumberFormatInfo numberFormatInfo = new CultureInfo("pt-BR", false).NumberFormat;

                                        orders.precoTotal = decimal.Parse(resultado.Servicos[0].Valor, numberFormatInfo) + preco;
                                        orders.pesoTotal = peso;
                                        orders.precoFrete = decimal.Parse(resultado.Servicos[0].Valor, numberFormatInfo);
                                        orders.dataEntrega = DateTime.Now.AddDays(Convert.ToDouble(resultado.Servicos[0].PrazoEntrega));

                                        db.Entry(orders).State = EntityState.Modified;

                                        try
                                        {
                                            db.SaveChanges();
                                        }
                                        catch (DbUpdateConcurrencyException)
                                        {
                                            if (!OrderExists(id))
                                            {
                                                return NotFound();
                                            }
                                            else
                                            {
                                                throw;
                                            }
                                        }
                                        return Ok(orders);
                                    }
                                    else
                                    {
                                        return BadRequest("Correios Message Error: " + resultado.Servicos[0].Erro + " Error: " + resultado.Servicos[0].MsgErro);
                                    }
                                }
                                else
                                {
                                    return BadRequest("No itens in this order");
                                }
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                    else
                    {
                        return BadRequest("This order has a STATUS different than novo");
                    }
                }
                else
                {
                    return BadRequest("This user does not have rights to access this order");
                }
            }
            else
            {
                return BadRequest("Order not Found");
            }
        }

        public string FindCep(string email)
        {
            try
            {
                CRMRestClient crmClient = new CRMRestClient();
                Customer customer = crmClient.GetCustomerByEmail(User.Identity.Name);
                if (customer == null)
                {
                    return "NOCUSTOMER";
                }
                return customer.zip;

            }
            catch (Exception)
            {
                return "FAULT";
            }

        }

    }
}