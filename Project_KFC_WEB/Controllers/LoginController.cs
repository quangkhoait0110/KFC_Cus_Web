using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_KFC_WEB.Models;

namespace Project_KFC_WEB.Controllers
{
    public class LoginController : Controller
    {
        KFC_Data db = new KFC_Data();

        // GET: Login
        public ActionResult Index(int check = 0)
        {
            if(check == 2)
            {
                ViewBag.error = "Sai tài khoản mật khẩu";
            }

            bool isLogin = Session["isLoginUser"] == null ? false : (bool) Session["isLoginUser"];

            if (isLogin) return RedirectToAction("Profile", "Login");

            ViewBag.foodCategories = db.foodCategories.ToList();
            ViewBag.Length = db.foodCategories.ToList().Count();

            return View();
        }

        public ActionResult Logout()
        {
            Session["userName"] = null;
            Session["isLoginUser"] = null;

            return RedirectToAction("Index", "Login");
        }

        public ActionResult Profile(int? result = 3, int page = 1)
        {

            bool isLogin = Session["isLoginUser"] == null ? false : (bool)Session["isLoginUser"];

            if (!isLogin) return RedirectToAction("Index", "Login");

            if(result == 0)
            {
                ViewBag.message = "Nhập sai mật khẩu!! Đổi mật khẩu thất bại";
            }
            else if(result == 2)
            {
                ViewBag.message = "Mật khẩu mới cần nhập trùng nhau";
            }
            else if(result == 1)
            {
                ViewBag.message = "Thay đổi thành công";
            }


            var userName = (string) Session["userName"];
            account acc = new account();

            if (!string.IsNullOrEmpty(userName))
            {
                acc =  db.accounts.ToList().Find(item => item.userName == userName);
            }

            ViewBag.foodCategories = db.foodCategories.ToList();
            ViewBag.Length = db.foodCategories.ToList().Count();

            var carts = db.carts.ToList().FindAll(item => item.userName == acc.userName);
            List<cart> cartPage = carts;

            if (carts != null && carts.Count() > 0)
            {
                //page
                int itemsPerPage = 3;
                int totalItems = carts.Count();
                int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);

                page = Math.Max(1, Math.Min(page, totalPages));

                var startIndex = (page - 1) * itemsPerPage;
                var endIndex = Math.Min(startIndex + itemsPerPage - 1, totalItems - 1);


                if (startIndex < 0 || startIndex >= totalItems)
                {
                    cartPage = null;
                }
                else
                {
                    cartPage = carts.GetRange(startIndex, endIndex - startIndex + 1);
                }

                ViewBag.currentPage = page;
                Session["currentPageCartProfile"] = page;
                ViewBag.totalPages = totalPages;
            }

            ViewBag.cartPage = cartPage.Count > 0 ? cartPage : null;

            return View(acc);
        }

        public ActionResult Register(bool check = true)
        {
            if(!check)
            {
                ViewBag.error = "Đăng kí thất bại";
            }

            ViewBag.foodCategories = db.foodCategories.ToList();
            ViewBag.Length = db.foodCategories.ToList().Count();

            return View();
        }

        public ActionResult CheckLogin()
        {
            var userName = Request.QueryString["emailLogin"];
            var passWord = Request.QueryString["passLogin"];

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord))
            {
                return RedirectToAction("Index", "Login", new { check = 2 });
            }
            else
            {
                if (db.accounts.ToList().Find(item => item.userName == userName && item.passWord == passWord) == null)
                {
                    return RedirectToAction("Index", "Login", new { check = 2 });
                }
                else
                {
                    Session["userName"] = userName;
                    Session["isLoginUser"] = true;

                    return RedirectToAction("Index", "Home");
                }

            }
        }

        public ActionResult CheckRegister()
        {
            var userName = Request.QueryString["emailLogin"];
            var passWord = Request.QueryString["passLogin"];

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord))
            {
                return RedirectToAction("Index", "Login", new { check = 2 });
            }
            else
            {
                try
                {
                    account acc = new account();

                    acc.userName = userName;
                    acc.passWord = passWord;
                    acc.carts = null;

                    db.accounts.Add(acc);
                    db.SaveChanges();

                    Session["userName"] = userName;
                    Session["isLoginUser"] = true;

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception)
                {
                    return RedirectToAction("Register", "Login", new { check  = false});
                }

                
            }

        }

        public ActionResult ChangePassword()
        {
            var passwordLast = Request.QueryString["passwordLast"];
            var passwordNew1 = Request.QueryString["passwordNew1"];
            var passwordNew2 = Request.QueryString["passwordNew2"];
            var useName = (string) Session["userName"];
            int reslut = 0;

            if (!string.IsNullOrEmpty(passwordLast) || !string.IsNullOrEmpty(passwordNew1) || !string.IsNullOrEmpty(passwordNew2))
            {
                if (passwordNew1.Equals(passwordNew2))
                {
                    var acc = db.accounts.ToList().Find(item => item.userName == useName && item.passWord == passwordLast);

                    if(acc != null)
                    {
                        acc.passWord = passwordNew1;

                        db.Entry(acc).State = EntityState.Modified;
                        db.SaveChanges();

                        reslut = 1;
                    }
                    else
                    {
                        reslut = 0;
                    }
                }
                else
                {
                    reslut = 2;

                }
            }

            return RedirectToAction("Profile", "Login", new { result =  reslut});
        }

        public ActionResult ChangeProfile()
        {
            var name = Request.QueryString["name"];
            var address = Request.QueryString["address"];
            var phone = Request.QueryString["phone"];
            var useName = (string)Session["userName"];
            int reslut = 0;

            if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(address) || !string.IsNullOrEmpty(phone))
            {
                var acc = db.accounts.ToList().Find(item => item.userName == useName);

                if (acc != null)
                {
                    acc.phone = phone;
                    acc.name = name;
                    acc.address = address;


                db.Entry(acc).State = EntityState.Modified;
                    db.SaveChanges();

                    reslut = 1;
                }
            }

            return RedirectToAction("Profile", "Login", new { result = reslut });
        }
    }
}