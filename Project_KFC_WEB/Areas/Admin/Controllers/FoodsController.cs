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
    public class FoodsController : Controller
    {
        private KFC_Data db = new KFC_Data();

        // GET: Admin/Foods
        public ActionResult Index(int page = 1, bool isReset = false)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if (!isLogin) return RedirectToAction("Login", "Home", new { isLogin = false });

            if (isReset) Session["isSearchingFood"] = false;

            List<food> foods = new List<food>();
            bool isSearching = Session["isSearchingFood"] != null ? (bool)Session["isSearchingFood"] : false;

            var selectedCategoryOption = Request.QueryString["selectedCategoryOption"];
            var selectedOptionPrice = Request.QueryString["selectedOptionPrice"];
            var checkSale = Request.QueryString["checkSale"];
            var valueSearch = Request.QueryString["valueSearch"];

            foods = db.foods.Include(c => c.foodCategory).ToList();
            ViewBag.foodCategories = db.foodCategories.ToList();

            if (!isSearching)
            {
                    if (!string.IsNullOrEmpty(selectedOptionPrice) || !string.IsNullOrEmpty(valueSearch) || !string.IsNullOrEmpty(checkSale) || !string.IsNullOrEmpty(selectedCategoryOption))
                    {
                        isSearching = true;
                        Session["isSearchingFood"] = isSearching;
                        foods = SrearchFood(foods, selectedCategoryOption, selectedOptionPrice, valueSearch, checkSale);
                    }
            }

            if (isSearching)
            {
                foods = Session["listFood"] as List<food>;
                if (foods.ToList().Count() == 0) Session["isSearchingFood"] = false;
                foods = SrearchFood(foods, selectedCategoryOption, selectedOptionPrice, valueSearch, checkSale);
            }

            int itemsPerPage = 5;
            int totalItems = foods.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);

            page = Math.Max(1, Math.Min(page, totalPages));

            var startIndex = (page - 1) * itemsPerPage;
            var endIndex = Math.Min(startIndex + itemsPerPage - 1, totalItems - 1);

            List<food> foodPage;

            if (startIndex < 0 || startIndex >= totalItems)
            {
                foodPage = null;
            }
            else
            {
                foodPage = foods.GetRange(startIndex, endIndex - startIndex + 1);
            }

            ViewBag.currentPage = page;
            Session["currentPageFood"] = page;
            ViewBag.totalPages = totalPages;

            return View(foodPage);
        }

        public List<food> SrearchFood(List<food> foods, string selectedCategoryOption, string selectedOptionPrice, string valueSearch, string checkSale)
        {
            if (!string.IsNullOrEmpty(selectedCategoryOption)) foods = foods.FindAll(item => item.foodCategory.name.ToLower().Equals(selectedCategoryOption.Trim().ToLower()));

            if (!string.IsNullOrEmpty(selectedOptionPrice))
            {
                var number = Convert.ToInt32(string.Join("", selectedOptionPrice.Split('-')[0].Trim().Split('.')));

                if (number == 0)
                {
                    foods = foods.FindAll(item => item.price >= 0 && item.price < 10000);
                }
                else if (number == 10000)
                {
                    foods = foods.FindAll(item => item.price >= 10000 && item.price < 50000);
                }
                else if (number == 50000)
                {
                    foods = foods.FindAll(item => item.price >= 50000 && item.price < 100000);
                }
                else if (number == 100000)
                {
                    foods = foods.FindAll(item => item.price >= 100000 && item.price < 500000);

                }
                else if (number == 500000)
                {
                    foods = foods.FindAll(item => item.price >= 500000 && item.price < 1000000);

                }
                else if (number == 1000000)
                {
                    foods = foods.FindAll(item => item.price >= 1000000);

                }
            }

            if (!string.IsNullOrEmpty(valueSearch)) foods = foods.FindAll(item => item.name.ToLower().Contains(valueSearch.Trim().ToLower()));

            if (checkSale != null) foods = checkSale.Contains("on") ? foods.FindAll(item => item.discount > 0) : foods;

            Session["listFood"] = foods;

            return foods;
        }

        // GET: Admin/Foods/Details/5
        public ActionResult Details(int? id)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if (!isLogin) return RedirectToAction("Login", "Home", new { isLogin = false });

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            food food = db.foods.Find(id);
            if (food == null)
            {
                return HttpNotFound();
            }
            return View(food);
        }

        // GET: Admin/Foods/Create
        public ActionResult Create()
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if (!isLogin) return RedirectToAction("Login", "Home", new { isLogin = false });


            ViewBag.idCategory = new SelectList(db.foodCategories, "id", "name");
            return View();
        }

        // POST: Admin/Foods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,idCategory,name,image,price,discount,description,timeSellStart,timeSellEnd")] food food, HttpPostedFileBase imageFile)
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
                    food.image = fileName;
                }

                var lastFood = db.foods.OrderByDescending(f => f.id).FirstOrDefault();

                if (lastFood != null)
                {
                    food.id = lastFood.id + 1;
                }
                else
                {
                    food.id = 1;
                }

                db.foods.Add(food);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(food);
        }

        // GET: Admin/Foods/Edit/5
        public ActionResult Edit(int? id)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if (!isLogin) return RedirectToAction("Login", "Home", new { isLogin = false });

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            food food = db.foods.Find(id);
            if (food == null)
            {
                return HttpNotFound();
            }
            ViewBag.idCategory = new SelectList(db.foodCategories, "id", "name", food.idCategory);
            return View(food);
        }

        // POST: Admin/Foods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,idCategory,name,image,price,discount,description,timeSellStart,timeSellEnd")] food food, HttpPostedFileBase imageFile)
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
                    food.image = fileName;

                    db.Entry(food).State = EntityState.Modified;
                }
                else
                {
                    var existingFood = db.foods.FirstOrDefault(item => item.id == food.id);

                    if (existingFood != null)
                    {
                        existingFood.idCategory = food.idCategory;
                        existingFood.name = food.name;
                        existingFood.price = food.price;
                        existingFood.discount = food.discount;
                        existingFood.description = food.description;
                        existingFood.timeSellStart = food.timeSellStart;
                        existingFood.timeSellEnd = food.timeSellEnd;

                        db.Entry(existingFood).State = EntityState.Modified;
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idCategory = new SelectList(db.foodCategories, "id", "name", food.idCategory);
            return View(food);
        }

        // GET: Admin/Foods/Delete/5
        public ActionResult Delete(int? id)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if (!isLogin) return RedirectToAction("Login", "Home", new { isLogin = false });

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            food food = db.foods.Find(id);
            if (food == null)
            {
                return HttpNotFound();
            }
            return View(food);
        }

        // POST: Admin/Foods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if (!isLogin) return RedirectToAction("Login", "Home", new { isLogin = false });

            food food = db.foods.Find(id);
            db.foods.Remove(food);
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
