﻿using ProductCatalogManager.Bus;
using ProductCatalogManager.Bus.Models;
using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ProductCatalogManager.Controllers
{
    public class ProductsController : Controller
    {
        private ProductService productService = new ProductService();

        // GET: Products
        public ActionResult Index(string searchString)
        {
            return View(productService.GetProducts(searchString));
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = productService.GetProduct(id.Value);

            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.Photo = GetProductPhotoFromRequest();
                productService.Add(product);
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Product product = productService.GetProduct(id.Value);

            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        // POST: Products/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                product.Photo = GetProductPhotoFromRequest();

                productService.Update(product);

                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Product product = productService.GetProduct(id.Value);

            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            productService.Delete(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                productService.Dispose();
            }

            base.Dispose(disposing);
        }

        public ActionResult RetrieveImage(int id)
        {
            byte[] cover = productService.GetProduct(id).Photo;

            if (cover == null)
                return null;

            return File(cover, "image/jpg");
        }

        private byte[] GetProductPhotoFromRequest()
        {
            HttpPostedFileBase photo = Request.Files["ProductPhoto"];

            if (photo == null)
                return new byte[0];

            BinaryReader reader = new BinaryReader(photo.InputStream);
            return reader.ReadBytes(photo.ContentLength);
        }

        public FileContentResult ExportToExcel()
        {
            var fileContent = productService.ExportAsExcel();
            return new FileContentResult(fileContent, "application/vnd.ms-excel") { FileDownloadName = typeof(Product).Name + DateTime.Now.Ticks + ".xlsx" };
        }
    }
}
