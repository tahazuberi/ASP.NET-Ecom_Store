using NexusMarketing.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NexusMarketing.Controllers
{
    public class UserController : Controller
    {
        NexusMarketingEntities2 db = new NexusMarketingEntities2();
        // GET: User
        public ActionResult Index(int? page)
        {
            int pagesize = 3, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_category.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_category> cat = list.ToPagedList(pageindex, pagesize);

            return View(cat);

        }
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(tbl_user us, HttpPostedFileBase file)
        {
            string path = uploadimage(file);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image Could not be Uploaded";
            }
            else
            {
                tbl_user u = new tbl_user();
                u.u_name = us.u_name;
                u.u_password = us.u_password;
                u.u_contact = us.u_contact;
                u.u_image = path;

                db.tbl_user.Add(u);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View();
        }

        public string uploadimage(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();

            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));

                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                else
                {
                    Response.Write("<script> alert ('Only Jpg, Jpeg and Png files are Acceptable') </script>");
                }
            }
            else
            {
                Response.Write("<script> alert('Please Select a File') </script>");
                path = "-1";
            }
            return path;
        }
        [HttpGet]
        public ActionResult UserLogin()
        {

            return View();
        }
        [HttpPost]
        public ActionResult UserLogin(tbl_user us)
        {
            tbl_user u = db.tbl_user.Where(x => x.u_name == us.u_name && x.u_password == us.u_password).SingleOrDefault();
            if (u != null)
            {
                Session["u_name"] = u.u_name;
                Session["u_id"] = u.u_id;
                return RedirectToAction("UserPannel");
            }
            else
            {
                ViewBag.error = "Invalid User Name or Password";

            }
            return View();
        }

        public ActionResult UserPannel()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Add_Product()
        {
            List<tbl_category> dep_list = db.tbl_category.ToList();
            ViewBag.cat_list = new SelectList(dep_list, "cat_id", "cat_name");
            return View();
        }

        [HttpPost]
        public ActionResult Add_Product(tbl_product pro,HttpPostedFileBase file)
        {
            string path = uploadimage(file);

            if (path.Equals("-1"))
            {
                ViewBag.error = "Image Could Not Be Upload";
            }
            else
            {
                List<tbl_category> dep_list = db.tbl_category.ToList();
                ViewBag.cat_list = new SelectList(dep_list, "cat_id", "cat_name");

                tbl_product p = new tbl_product();
                p.pro_name = pro.pro_name;
                p.pro_price = pro.pro_price;
                p.pro_description = pro.pro_description;
                p.pro_image = path;
                p.cat_id_fk = pro.cat_id_fk;
                db.tbl_product.Add(p);
                db.SaveChanges();

                return RedirectToAction("Add_Product");

            }

            return View();
        }

        public ActionResult View_Products(int?id , int ? page)
        {
            int pagesize = 3, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.cat_id_fk == id).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<tbl_product> pro = list.ToPagedList(pageindex, pagesize);

            return View(pro);
        }

    }
}