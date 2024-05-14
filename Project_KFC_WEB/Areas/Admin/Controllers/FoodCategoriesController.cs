using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_KFC_WEB.Models;

namespace Project_KFC_WEB.Areas.Admin.Controllers
{
    public class FoodCategoriesController : Controller
    {
        private KFC_Data db = new KFC_Data();

        // GET: Admin/FoodCategories
        public ActionResult Index(int page = 1, bool isReset = false)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if (!isLogin) return RedirectToAction("Login", "Home", new { isLogin = false });

            if (isReset) Session["isSearchingFoodCategory"] = false;

            List<foodCategory> foodCategories = new List<foodCategory>();
            bool isSearching = Session["isSearchingFoodCategory"] != null ? (bool)Session["isSearchingFoodCategory"] : false;

            var valueSearch = Request.QueryString["valueSearch"];

            foodCategories = db.foodCategories.ToList();

            if (!isSearching)
            {
                if (!string.IsNullOrEmpty(valueSearch))
                {
                    isSearching = true;
                    foodCategories = foodCategories.FindAll(item => item.name.ToLower().Contains(valueSearch.Trim().ToLower()));
                    Session["listFoodCategoty"] = foodCategories;
                }
            }

            if (isSearching)
            {
                foodCategories = Session["listFoodCategoty"] as List<foodCategory>;
                if (foodCategories.ToList().Count() == 0) Session["isSearchingFoodCategory"] = false;
                if (!string.IsNullOrEmpty(valueSearch))
                {
                    foodCategories = foodCategories.FindAll(item => item.name.ToLower().Contains(valueSearch.Trim().ToLower()));
                    Session["listFoodCategoty"] = foodCategories;
                }
            }

            int itemsPerPage = 5;
            int totalItems = foodCategories.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);

            page = Math.Max(1, Math.Min(page, totalPages));

            var startIndex = (page - 1) * itemsPerPage;
            var endIndex = Math.Min(startIndex + itemsPerPage - 1, totalItems - 1);

            List<foodCategory> foodCategoryPage;

            if (startIndex < 0 || startIndex >= totalItems)
            {
                foodCategoryPage = null;
            }
            else
            {
                foodCategoryPage = foodCategories.GetRange(startIndex, endIndex - startIndex + 1);
            }

            ViewBag.currentPage = page;
            Session["currentPageFoodCategory"] = page;
            ViewBag.totalPages = totalPages;


            return View(foodCategoryPage);
        }

        // GET: Admin/FoodCategories/Details/5
        public ActionResult Details(int? id)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if (!isLogin) return RedirectToAction("Login", "Home", new { isLogin = false });

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            foodCategory foodCategory = db.foodCategories.FirstOrDefault((item) => item.id == id);
            if (foodCategory == null)
            {
                return HttpNotFound();
            }
            return View(foodCategory);
        }

        // GET: Admin/FoodCategories/Create
        public ActionResult Create()
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if (!isLogin) return RedirectToAction("Login", "Home", new { isLogin = false });

            return View();
        }

        // POST: Admin/FoodCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,image")] foodCategory foodCategory, HttpPostedFileBase imageFile)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if (!isLogin) return RedirectToAction("Login", "Home", new { isLogin = false });

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(imageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Upload"), fileName);
                    imageFile.SaveAs(path);
                    foodCategory.image = fileName;
                }

                var lastFoodCategory = db.foodCategories.OrderByDescending(fc => fc.id).FirstOrDefault();

                if (lastFoodCategory != null)
                {
                    foodCategory.id = lastFoodCategory.id + 1;
                }
                else
                {
                    foodCategory.id = 1;
                }

                db.foodCategories.Add(foodCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(foodCategory);
        }

        // GET: Admin/FoodCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if (!isLogin) return RedirectToAction("Login", "Home", new { isLogin = false });

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            foodCategory foodCategory = db.foodCategories.FirstOrDefault((item) => item.id == id);
            if (foodCategory == null)
            {
                return HttpNotFound();
            }
            return View(foodCategory);
        }

        // POST: Admin/FoodCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,image")] foodCategory foodCategory, HttpPostedFileBase imageFile)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if (!isLogin) return RedirectToAction("Login", "Home", new { isLogin = false });

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(imageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Upload"), fileName);
                    imageFile.SaveAs(path);
                    foodCategory.image = fileName;
                    db.Entry(foodCategory).State = EntityState.Modified;
                }
                else
                {
                    var existingFoodCategory = db.foodCategories.FirstOrDefault(item => item.id == foodCategory.id);

                    if (existingFoodCategory != null)
                    {
                        existingFoodCategory.name = foodCategory.name;
                        db.Entry(existingFoodCategory).State = EntityState.Modified;
                    }
                    else
                    {
                        return HttpNotFound();
                    }

                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(foodCategory);
        }


        // GET: Admin/FoodCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if (!isLogin) return RedirectToAction("Login", "Home", new { isLogin = false });

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            foodCategory foodCategory = db.foodCategories.FirstOrDefault((item) => item.id == id);
            if (foodCategory == null)
            {
                return HttpNotFound();
            }
            return View(foodCategory);
        }

        // POST: Admin/FoodCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if (!isLogin) return RedirectToAction("Login", "Home", new { isLogin = false });

            foodCategory foodCategory = db.foodCategories.FirstOrDefault((item) => item.id == id);
            db.foodCategories.Remove(foodCategory);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
