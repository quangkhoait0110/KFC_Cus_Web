using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_KFC_WEB.Models;
using System.Data.Entity;

namespace Project_KFC_WEB.Controllers
{
    public class HomeController : Controller
    {
        KFC_Data db = new KFC_Data();

        public ActionResult Index() 
        {
            List<cart> carts = new List<cart>(); 
            if(Session["cartUser"] as List<cart> == null)
            {
                Session["cartUser"] = new List<cart>();
            }
            else
            {
                carts = Session["cartUser"] as List<cart>;
            } 

            ViewBag.foodCategories = db.foodCategories.ToList();
            ViewBag.Length = db.foodCategories.ToList().Count();
            ViewBag.quantityCart = carts.Count;

            return View(db.foods.ToList());
        }

        public ActionResult Menu(int index = 0)
        {
            List<cart> carts = new List<cart>();
            if (Session["cartUser"] as List<cart> == null)
            {
                Session["cartUser"] = new List<cart>();
            }
            else
            {
                carts = Session["cartUser"] as List<cart>;
            }

            ViewBag.foodCategories = db.foodCategories.ToList();
            ViewBag.Length = db.foodCategories.ToList().Count();
            ViewBag.currentIndex = index;
            ViewBag.quantityCart = carts.Count;

            return View(db.foods.ToList());
        }

        public ActionResult FoodDetail(int id = 1)
        {
            List<cart> carts = new List<cart>();
            if (Session["cartUser"] as List<cart> == null)
            {
                Session["cartUser"] = new List<cart>();
            }
            else
            {
                carts = Session["cartUser"] as List<cart>;
            }

            var idCategory = db.foods.ToList().FirstOrDefault(item => item.id == id).idCategory;
            ViewBag.category = db.foodCategories.ToList().FirstOrDefault(item => item.id == idCategory);
            ViewBag.index = db.foodCategories.ToList().FindIndex(item => item.id == idCategory);
            ViewBag.quantityCart = carts.Count;

            var discount = db.foods.ToList().FirstOrDefault(item => item.id == id).discount;
            ViewBag.discount =  discount != null && discount > 0 ? discount : -1 ;

            return View(db.foods.ToList().FirstOrDefault( item => item.id == id));
        }

        public ActionResult Cart(int page = 1, bool success = false, bool checkUser = true)
        {
            List<cart> carts = Session["cartUser"] as List<cart>;
            ViewBag.listFood = db.foods.ToList();
            ViewBag.quantityCart = carts == null ? 0 : carts.Count;
            ViewBag.foodCategories = db.foodCategories.ToList();
            ViewBag.Length = db.foodCategories.ToList().Count();

            if (success) ViewBag.success = success;

            if (!checkUser) ViewBag.error = "Bạn chưa đăng nhập";

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
                Session["currentPageCartUser"] = page;
                ViewBag.totalPages = totalPages;
            }

            ViewBag.cartPage = cartPage;

            return View(carts);
        }

        public ActionResult InsertCart()
        {
            List<cart> carts = new List<cart>();
            if (Session["cartUser"] as List<cart> == null)
            {
                Session["cartUser"] = new List<cart>();
            }
            else
            {
                carts = Session["cartUser"] as List<cart>;
            }

            string userName = Session["userName"] == null ? null : Session["userName"] as string;
            
            if (userName == null)
            {
                return RedirectToAction("Cart", "Home", new { checkUser = false});
            }

            foreach (var item in carts)
            {
                var cartLast = db.carts.ToList().Find(cart => cart.idFood == item.idFood && cart.userName == userName);

                if (cartLast != null)
                {
                    // update quantity
                    cartLast.quantity = cartLast.quantity + item.quantity;

                    db.Entry(cartLast).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    // new cart
                    item.id = -1;
                    item.userName = userName;
                    item.food = null;
                    item.account = null;

                    db.carts.Add(item);
                    db.SaveChanges();
                }
                
            }
            

            Session["cartUser"] = null;
            Session["discount"] = null;

            return RedirectToAction("Cart", new { success = true});
        }

        public ActionResult UpdateCart(int? id, int? quantity, bool plus)
        {
            List<cart> carts = new List<cart>();
            if (Session["cartUser"] as List<cart> == null)
            {
                return RedirectToAction("Cart");
            }
            else
            {
                carts = Session["cartUser"] as List<cart>;
            }

            var checkCart = carts.Find(item => item.idFood == id);

            if (checkCart != null && quantity > 0) {
                if (plus)
                {
                    carts[carts.FindIndex(item => item.idFood == checkCart.idFood)].quantity = checkCart.quantity + 1;
                }else
                {
                    if(checkCart.quantity - 1  != 0)
                    {
                        carts[carts.FindIndex(item => item.idFood == checkCart.idFood)].quantity = checkCart.quantity - 1;
                    }
                }
            }

            Session["cartUser"] = carts;

            return RedirectToAction("Cart", new { page = (int)Session["currentPageCartUser"] });
        }

        public ActionResult AddCart(int? id, string view = "Index" ,int quantity = 1)
        {
            int index = 1;
            if(id != null)
            {
                List<cart> carts = new List<cart>();
                if (Session["cartUser"] as List<cart> == null)
                {
                    Session["cartUser"] = new List<cart>();
                }
                else
                {
                    carts = Session["cartUser"] as List<cart>;
                }

                var userName = Session["userName"] == null ? "admin" : Session["userName"] as string;
                var checkCart = carts.Find(item => item.idFood == id);

                if (checkCart == null)
                {
                    cart cart = new cart();
                    cart.idFood = id;
                    cart.userName = userName;
                    cart.quantity = quantity;
                    cart.food = db.foods.ToList().Find(item => item.id == id);
                    cart.account = db.accounts.ToList().Find(item => item.userName == userName);

                    carts.Add(cart);
                }
                else
                {
                    carts[carts.FindIndex(item => item.id == checkCart.id)].quantity = checkCart.quantity + quantity;
                }

                

                Session["cartUser"] = carts;

                var idCategory = db.foods.ToList().FirstOrDefault(item => item.id == id).idCategory;
                index = db.foodCategories.ToList().FindIndex(item => item.id == idCategory);
            }

            if (!view.ToLower().Contains("index")) view = "Menu";

            return RedirectToAction(view, new { index = index});
        }

        public ActionResult DeleteCart(int? id)
        {
            if (id != null)
            {
                List<cart> carts = new List<cart>();
                if (Session["cartUser"] as List<cart> == null)
                {
                    Session["cartUser"] = new List<cart>();
                }
                else
                {
                    carts = Session["cartUser"] as List<cart>;
                }

                var checkCart = carts.Find(item => item.idFood == id);

                if (checkCart != null)
                {
                    carts.Remove(checkCart);
                }

                Session["cartUser"] = carts;
            }


            return RedirectToAction("Cart" , new { page = (int) Session["currentPageCartUser"] });
        }


        public ActionResult CheckDiscount()
        {
            var discount = Request.QueryString["discount"];

            var code = db.discountCodes.ToList().Find(item => item.code == discount);

            if (code != null) Session["discount"] = code.discount;

            return RedirectToAction("Cart", "Home");
        }

    }
}