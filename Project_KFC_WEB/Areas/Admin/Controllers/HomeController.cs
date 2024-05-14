using Project_KFC_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_KFC_WEB.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private KFC_Data db = new KFC_Data();

        // GET: Admin/Home
        public ActionResult Index()
        {
            bool isLogin = Session["login"] != null ? (bool)Session["login"] : false;

            
            if (isLogin)
            {
                ViewBag.countUser = db.accounts.ToList().Count();
                ViewBag.countFoodCategory = db.foodCategories.ToList().Count();
                ViewBag.countFood = db.foods.ToList().Count();
                ViewBag.countCart = db.carts.ToList().Count();

                return View();
            }

            return RedirectToAction("Login", new { isLogin = false });
        }

        public ActionResult Login(bool isLogin = true)
        {
            if (!isLogin)
            {
                ViewBag.message = "Đăng nhập thất bại";
            }

            return View();
        }

        [HttpGet]
        public ActionResult CheckLogin()
        {
            var userName = Request.QueryString["inputUserName"];
            var passWord = Request.QueryString["inputPassword"];

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(passWord))
            {
                //var check = db.accounts.ToList().Find(item => item.userName == userName && item.passWord == passWord);

                bool check = userName.Equals("admin") && passWord.Equals("123");

                if (check)
                {
                    Session["login"] = true;
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Login", new { isLogin = false });
                }
            }
            else
            {
                return RedirectToAction("Login", new { isLogin = false });
            }
        }

        public ActionResult CheckLogout()
        {
            Session["login"] = false;

            return RedirectToAction("Login");
        }
    }
}