using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_KFC_WEB.Models;

namespace Project_KFC_WEB.Areas.Admin.Controllers
{
    public class DiscountCodesController : Controller
    {
        private KFC_Data db = new KFC_Data();

        // GET: Admin/DiscountCodes
        public ActionResult Index(int page = 1, bool isReset = false)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if(!isLogin) return RedirectToAction("Login", "Home",new { isLogin = false });

            if (isReset) Session["isSearchingDisCountCode"] = false;

            List<discountCode> discountCodes = new List<discountCode>();
            bool isSearching = Session["isSearchingFoodCategory"] != null ? (bool)Session["isSearchingDisCountCode"] : false;

            var valueSearch = Request.QueryString["valueSearch"];

            discountCodes = db.discountCodes.ToList();

            if (!isSearching)
            {
                if (!string.IsNullOrEmpty(valueSearch))
                {
                    isSearching = true;
                    discountCodes = discountCodes.FindAll(item => item.code.ToLower().Contains(valueSearch.Trim().ToLower()));
                    Session["listDiscountCode"] = discountCodes;
                }
            }

            if (isSearching)
            {
                discountCodes = Session["listDiscountCode"] as List<discountCode>;
                if (discountCodes.ToList().Count() == 0) Session["isSearchingDisCountCode"] = false;
                if (!string.IsNullOrEmpty(valueSearch))
                {
                    discountCodes = discountCodes.FindAll(item => item.code.ToLower().Contains(valueSearch.Trim().ToLower()));
                    Session["listDiscountCode"] = discountCodes;
                }
            }

            int itemsPerPage = 5;
            int totalItems = discountCodes.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);

            page = Math.Max(1, Math.Min(page, totalPages));

            var startIndex = (page - 1) * itemsPerPage;
            var endIndex = Math.Min(startIndex + itemsPerPage - 1, totalItems - 1);

            List<discountCode> discountCodePage;

            if (startIndex < 0 || startIndex >= totalItems)
            {
                discountCodePage = null;
            }
            else
            {
                discountCodePage = discountCodes.GetRange(startIndex, endIndex - startIndex + 1);
            }

            ViewBag.currentPage = page;
            Session["currentPageDiscountCode"] = page;
            ViewBag.totalPages = totalPages;


            return View(discountCodePage);
        }

        // GET: Admin/DiscountCodes/Details/5
        public ActionResult Details(int? id)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if(!isLogin) return RedirectToAction("Login", "Home",new { isLogin = false });

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            discountCode discountCode = db.discountCodes.Find(id);
            if (discountCode == null)
            {
                return HttpNotFound();
            }
            return View(discountCode);
        }

        // GET: Admin/DiscountCodes/Create
        public ActionResult Create()
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if(!isLogin) return RedirectToAction("Login", "Home",new { isLogin = false });

            return View();
        }

        // POST: Admin/DiscountCodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,code,discount")] discountCode discountCode)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if(!isLogin) return RedirectToAction("Login", "Home",new { isLogin = false });

            if (ModelState.IsValid)
            {
                db.discountCodes.Add(discountCode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(discountCode);
        }

        // GET: Admin/DiscountCodes/Edit/5
        public ActionResult Edit(int? id)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if(!isLogin) return RedirectToAction("Login", "Home",new { isLogin = false });

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            discountCode discountCode = db.discountCodes.Find(id);
            if (discountCode == null)
            {
                return HttpNotFound();
            }
            return View(discountCode);
        }

        // POST: Admin/DiscountCodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,code,discount")] discountCode discountCode)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if(!isLogin) return RedirectToAction("Login", "Home",new { isLogin = false });

            if (ModelState.IsValid)
            {
                db.Entry(discountCode).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(discountCode);
        }

        // GET: Admin/DiscountCodes/Delete/5
        public ActionResult Delete(int? id)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if(!isLogin) return RedirectToAction("Login", "Home",new { isLogin = false });

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            discountCode discountCode = db.discountCodes.Find(id);
            if (discountCode == null)
            {
                return HttpNotFound();
            }
            return View(discountCode);
        }

        // POST: Admin/DiscountCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            if(!isLogin) return RedirectToAction("Login", "Home",new { isLogin = false });

            discountCode discountCode = db.discountCodes.Find(id);
            db.discountCodes.Remove(discountCode);
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
