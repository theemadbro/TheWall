using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TheWall.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace TheWall.Controllers
{
    public static class SessionExtensions
    {
        // We can call ".SetObjectAsJson" just like our other session set methods, by passing a key and a value
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            // This helper function simply serializes theobject to JSON and stores it as a string in session
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        
        // generic type T is a stand-in indicating that we need to specify the type on retrieval
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            // Upon retrieval the object is deserialized based on the type we specified
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
    public class HomeController : Controller
    {
        private TheWallContext _context;
        public HomeController(TheWallContext context)
        {
            _context = context;
        }



        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            List<CurrentUser> ret = HttpContext.Session.GetObjectFromJson<List<CurrentUser>>("curr");
            if(ret == null)
            {
                CurrentUser newcurr = new CurrentUser();
                newcurr.id = 0;
                ViewBag.CurrentUser = newcurr;
                List<object> temp = new List<object>();
                temp.Add(newcurr);
                HttpContext.Session.SetObjectAsJson("curr", temp);
                return View();
            }
            else if (ret.Count == 0)
            {
                ViewBag.CurrentUser.id = 0;
                return View();
            }
            else
            {
                
                ViewBag.CurrentUser = ret[0];
                return View();
            }
        }

        [HttpPost]
        [Route("register")]
        public IActionResult ProcessReg(Users inp)
        {
            if(ModelState.IsValid)
            {
                PasswordHasher<Users> Hasher = new PasswordHasher<Users>();
                inp.password = Hasher.HashPassword(inp, inp.password);
                _context.Add(inp);
                _context.SaveChanges();
                Users check = _context.users.SingleOrDefault(x => x.email == inp.email);
                CurrentUser newcurr = new CurrentUser();
                newcurr.id = check.id;
                newcurr.name = check.first_name+" "+check.last_name;
                List<object> temp = new List<object>();
                temp.Add(newcurr);
                HttpContext.Session.SetObjectAsJson("curr", temp);
                return RedirectToAction("thewall");
            }
            else
            {
                return View("Index");
            }
        }
        
        [Route("login")]
        public IActionResult ProcessLogin(string inEmail, string inPass)
        {
            List<CurrentUser> ret = HttpContext.Session.GetObjectFromJson<List<CurrentUser>>("curr");
            ViewBag.CurrentUser = ret[0];
            Users check = _context.users.SingleOrDefault(x => x.email == inEmail);
            if (check != null && inPass != null)
            {
                var hasher = new PasswordHasher<Users>();
                if(0 != hasher.VerifyHashedPassword(check, check.password, inPass))
                {
                    CurrentUser curr = new CurrentUser();
                    curr.id = check.id;
                    curr.name = check.first_name+" "+check.last_name;
                    ViewBag.CurrentUser = curr;
                    List<object> temp = new List<object>();
                    temp.Add(curr);
                    HttpContext.Session.SetObjectAsJson("curr", temp);
                    return RedirectToAction("thewall");
                }
                else
                {
                    ViewBag.LogError = "The Email or Password was incorrect.";
                    return View("Index");
                }
                
            }
            else 
            {
                ViewBag.LogError = "Missing Email or Password.";
                return View("Index");
            }
        }

        [HttpGet]
        [Route("thewall")]
        public IActionResult TheWall()
        {
            List<CurrentUser> ret = HttpContext.Session.GetObjectFromJson<List<CurrentUser>>("curr");
            if (ret[0].id == 0)
            {
                return RedirectToAction("");
            }
            else
            {
                ViewBag.CurrentUser = ret[0];
                List<Posts> AllPosts = _context.posts.Include(post => post.User).Include(p => p.Comments).ThenInclude(c => c.User).ToList();
                ViewBag.thewall = AllPosts;
                return View("TheWall");
            }

        }

        [HttpPost]
        [Route("delete/{inpid}")]
        public IActionResult DeletePost(int inpid)
        {
            Posts todelete = _context.posts.Include(x => x.Comments).SingleOrDefault(p => p.id == inpid);
            foreach(var x in todelete.Comments)
            {
                _context.comments.Remove(x);
            }
            _context.posts.Remove(todelete);
            _context.SaveChanges();
            return RedirectToAction("thewall");
        }

        [HttpPost]
        [Route("newpost")]
        public IActionResult NewPost(string postInp)
        {
            List<CurrentUser> ret = HttpContext.Session.GetObjectFromJson<List<CurrentUser>>("curr");
            if (ret.Count == 0)
            {
                return RedirectToAction("");
            }
            else
            {
                Posts newpost = new Posts
                {
                    usersid = ret[0].id,
                    postContent = postInp
                };
                _context.Add(newpost);
                _context.SaveChanges();
                return RedirectToAction("thewall");
            }
        }

        [HttpPost]
        [Route("newcomm")]
        public IActionResult NewComm(string commInp, int postidinp)
        {
            List<CurrentUser> ret = HttpContext.Session.GetObjectFromJson<List<CurrentUser>>("curr");
            if (ret.Count == 0)
            {
                return RedirectToAction("");
            }
            else
            {
                Comments newcomm = new Comments
                {
                    userid = ret[0].id,
                    postid = postidinp,
                    comContent = commInp
                };
                _context.Add(newcomm);
                _context.SaveChanges();
                return RedirectToAction("thewall");
            }
        }



        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
