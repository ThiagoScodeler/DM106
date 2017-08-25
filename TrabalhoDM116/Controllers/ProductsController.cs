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

namespace TrabalhoDM116.Controllers
{
    public class ProductsController : ApiController
    {
        private TrabalhoDM116Context db = new TrabalhoDM116Context();
        private TrabalhoDM116Context dbToValidationField = new TrabalhoDM116Context();

        // GET: api/Products
        [Authorize(Roles = "ADMIN, USER")]
        public IQueryable<Product> GetProducts()
        {
            return db.Products;
        }

        // GET: api/Products/5
        [Authorize(Roles = "ADMIN, USER")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return BadRequest("Product not Found");
            }

                var option = ValidateCodeModelValue(product);
                switch (option)
                {
                    case 1:
                        return BadRequest("This model is already registered. Please inform other value");

                    case 2:
                        return BadRequest("This code is already registered. Please inform other value");

                }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var option = ValidateCodeModelValue(product);
            switch (option)
            {
                case 1:
                    return BadRequest("This model is already registered. Please inform other value");

                case 2:
                    return BadRequest("This code is already registered. Please inform other value");

            }

            db.Products.Add(product);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.Id == id) > 0;
        }

        private int ValidateCodeModelValue(Product product)
        {
            var findProduct = dbToValidationField.Products.FirstOrDefault(p => p.modelo == product.modelo);
            if (findProduct != null && findProduct.Id != product.Id)
            {
                return 1;
            }
            findProduct = dbToValidationField.Products.FirstOrDefault(p => p.codigo == product.codigo);
            if (findProduct != null && findProduct.Id != product.Id)
            {
                return 2;
            }

            return 0;
        }
    }
}