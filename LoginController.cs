using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StateScholarshipProject.Models;

namespace StateScholarshipProject.Controllers
{
    public class LoginController : Controller
    {
        StateScholarshipEntities db;
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {
            using (db = new StateScholarshipEntities())
            {
                var checkUSer = db.Registration1.Where(d => d.User_id == loginModel.UserId && d.user_password == loginModel.Password && d.User_category == loginModel.UserCategory).FirstOrDefault();

                if (checkUSer != null)
                {
                    if (loginModel.UserCategory == "Scholarship Provider")
                    {
                        Session["userid"] = checkUSer.User_id.ToString();
                        return View("SchlorshipProvider");
                    }

                    else if (loginModel.UserCategory == "Admin")
                    {
                        StateScholarshipEntities4 con = new StateScholarshipEntities4();
                        var count = con.Helps.Count(x => x.Solved == null);

                        ViewBag.Data = count;
                        return View("SchlorshipProvider");
                    }
                    

                    else
                    {

                        //Session["userid"] = checkUSer.User_id.ToString();
                        //return RedirectToAction("HelpCheckRequest");


                        TempData["userId"] = checkUSer.User_id.ToString();
                        return View("Student");
                    }
                }
                else
                {
                    ModelState.Clear();
                    ViewBag.RejectMessage = "Invalid user Id or Password";
                    return View("login");
                }

            }

        }

        StateScholarshipEntities4 context = new StateScholarshipEntities4();
        public ActionResult HelpCheckRequest(Help help)
        {
            
            var userid = int.Parse(TempData["userId"].ToString());
            return View(context.Helps.Where(x=>x.UserId == userid).ToList());
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPassword forgotPassword)
        {
            using(db= new StateScholarshipEntities())
            {

                var UserIdCheck = db.Registration1.Where(d => d.User_id == forgotPassword.UserID &&  d.FavouritePet == forgotPassword.FavouritePet && d.FavouriteMovie == forgotPassword.FavouriteMovie && d.LuckyNumber == forgotPassword.LuckyNumber).FirstOrDefault();
                if(UserIdCheck != null)
                {

                  Session["userid"] = UserIdCheck.User_id.ToString();
                  return RedirectToAction("PasswordReset");
                }
            }
            return View("ForgotPassword");
        }

        [HttpGet]
        public ActionResult PasswordReset()
        {
            if (Session["userid"].ToString() != null)
            {
                return View();
            }

            else
            {
                return RedirectToAction("Login");
            }
          
        }



        [HttpPost]
        public ActionResult PasswordReset(PasswordResetModel passwordReset)
        {
            using(db = new StateScholarshipEntities())
            {
                  string id = Session["userid"].ToString();

                    if (id != null)
                    {
                        if(passwordReset.NewPassword != null && passwordReset.ConfirmPassword != null)
                        {
                            int userid = int.Parse(Session["userid"].ToString());

                            var UserIdCheck = db.Registration1.Where(d => d.User_id == userid).FirstOrDefault();

                            UserIdCheck.user_password = passwordReset.NewPassword;

                            db.SaveChanges();

                            ViewBag.SuccessMessage = "Password Updated successful";
                        }

                    else
                    {
                        ViewBag.Message = "Please enter Details";
                    }
                        
                    }

                    else
                    {
                        return RedirectToAction("Login");
                    }
                
                return View();
            }
            
        }

        public ActionResult Student(LoginModel model)
        {
            Session["userid"] = model.UserId.ToString();
            return RedirectToAction("RequestView", "Help");
        }

        [HttpGet]
        public ActionResult ForgotID()
        {

            return View();
        }

        [HttpPost]
        public ActionResult ForgotID(ForgotUserID userID)
        {
            using(db = new StateScholarshipEntities())
            {
                var EmailCheck = db.Registration1.Where(d => d.Email == userID.Email && d.FavouritePet == userID.FavouritePet && d.FavouriteMovie == userID.FavouriteMovie && d.LuckyNumber == userID.LuckyNumber).FirstOrDefault();
                if (EmailCheck != null)
                {
                    var id = (from Registration1 in db.Registration1
                              where Registration1.Email.Equals(userID.Email)
                                      select Registration1.User_id).SingleOrDefault();

                    ViewBag.SuccessMessage = "User Id :" + id;
                }


                else
                {
                    ModelState.Clear();
                    ViewBag.RejectMessage = "Invalid Information given";
                   
                }
                return View();

            }
           
        }
    }
}