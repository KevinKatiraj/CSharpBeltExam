using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CSharpBeltKevin.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CSharpBeltKevin.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private MyContext _context;
    public HomeController(ILogger<HomeController> logger , MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
      {
        
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Register");
        }
        int id = (int)HttpContext.Session.GetInt32("userId");
                //Marrim gjithe perdoruesit e tjere
        List<Request> req =  _context.Requests.Include(e=>e.Reciver).Include(e=>e.Sender).Where(e => e.ReciverId == id).Where(e => e.Accepted == false).ToList();
        //List me request
        List<User> LIST4= _context.Users.Include(e=>e.Requests).Where(e=> e.UserId != id).Where(e=>(e.Requests.Any(f=> f.SenderId == id) == false) && (e.Requests.Any(f=> f.ReciverId == id) == false) ).ToList();
        //list me miqte
        List<Request> miqte =_context.Requests.Where(e => (e.SenderId == id) || (e.ReciverId == id)).Include(e=>e.Reciver).Include(e=>e.Sender).Where(e=>e.Accepted ==true).ToList();
        //Filtorjme listen e gjithe userave ne menyre qe miqte dhe ata qe u kemi nisur ose na kane nisur 
        //request te mos dalin tek users e tjere.
        for (int i = 0; i < LIST4.Count-1; i++)
        {
            var test22= LIST4[i].Requests.Except(req);
            
            for (int j = 0; j < req.Count; j++)
            {
                if ((req.Count > 0) && (req[j].SenderId == LIST4[i].UserId || req[j].ReciverId == LIST4[i].UserId ))
            {
                if(LIST4.Count > 0)
                LIST4.Remove(LIST4[i]);
            }
            }
            if  (miqte.Count > 0)
            for (int z = 0; z < miqte.Count; z++)
            {
               
                if ( (miqte[z].SenderId == LIST4[i].UserId || miqte[z].ReciverId == LIST4[i].UserId) )
            {   
                if(LIST4.Count > 0)
                LIST4.Remove(LIST4[i]);
            }
            }
            
        }
        //shfaqim gjith requests
        ViewBag.requests = _context.Requests.Include(e=>e.Reciver).Include(e=>e.Sender).Where(e => e.ReciverId == id).Where(e => e.Accepted == false).ToList();
        // lista e filtruar ruhet ne viewbag
        ViewBag.perdoruesit= LIST4;
        //Marr te loguarin me te dhena
        ViewBag.iLoguari = _context.Users.FirstOrDefault(e => e.UserId == id);
        // shfaq gjith miqte
        ViewBag.miqte = _context.Requests.Where(e => (e.SenderId == id) || (e.ReciverId == id)).Include(e=>e.Reciver).Include(e=>e.Sender).Where(e=>e.Accepted ==true).ToList();
        return View();
    }

    [HttpGet("Register")]
    
    public IActionResult Register()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return View();
        }
        return RedirectToAction("Index");
    }

    [HttpPost("Register")]
    public IActionResult Register(User user)
    {
        
        if (ModelState.IsValid)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email already in use!");

                return View();
            }
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            user.Password = Hasher.HashPassword(user, user.Password);
            _context.Users.Add(user);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("userId", user.UserId);
            return RedirectToAction("Index");
        }
        return View();
    }

    [HttpPost("Login")]
    public IActionResult LoginSubmit(LoginUser userSubmission)
    {
        if (ModelState.IsValid)
        {
            var userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
            if (userInDb == null)
            {
                ModelState.AddModelError("User", "Invalid UserName/Password");
                return View("Register");
            }

            var hasher = new PasswordHasher<LoginUser>();

            var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

            if (result == 0)
            {
                ModelState.AddModelError("Password", "Invalid Password");
                return View("Register");
            }
            HttpContext.Session.SetInt32("userId", userInDb.UserId);

            return RedirectToAction("Index");
        }

        return View("Register");
    }

    [HttpGet("Logout")]
    public IActionResult Logout()
    {

        HttpContext.Session.Clear();
        return RedirectToAction("register");
    }

    [HttpGet("AllUser")]
    public IActionResult AllUser()
    {
        int id = (int)HttpContext.Session.GetInt32("userId");
        //Marrim gjithe perdoruesit e tjere
        List<Request> req =  _context.Requests.Include(e=>e.Reciver).Include(e=>e.Sender).Where(e => e.ReciverId == id).Where(e => e.Accepted == false).ToList();
        //List me request
        List<User> LIST4= _context.Users.Include(e=>e.Requests).Where(e=> e.UserId != id).Where(e=>(e.Requests.Any(f=> f.SenderId == id) == false) && (e.Requests.Any(f=> f.ReciverId == id) == false) ).ToList();
        //list me miqte
        List<Request> miqte =_context.Requests.Where(e => (e.SenderId == id) || (e.ReciverId == id)).Include(e=>e.Reciver).Include(e=>e.Sender).Where(e=>e.Accepted ==true).ToList();
        //Filtorjme listen e gjithe userave ne menyre qe miqte dhe ata qe u kemi nisur ose na kane nisur 
        //request te mos dalin tek users e tjere.
        for (int i = 0; i < LIST4.Count-1; i++)
        {
            var test22= LIST4[i].Requests.Except(req);
            
            for (int j = 0; j < req.Count; j++)
            {
                if ((req.Count > 0) && (req[j].SenderId == LIST4[i].UserId || req[j].ReciverId == LIST4[i].UserId ))
            {
                if(LIST4.Count > 0)
                LIST4.Remove(LIST4[i]);
            }
            }
            if  (miqte.Count > 0)
            for (int z = 0; z < miqte.Count; z++)
            {
               
                if ( (miqte[z].SenderId == LIST4[i].UserId || miqte[z].ReciverId == LIST4[i].UserId) )
            {   
                if(LIST4.Count > 0)
                LIST4.Remove(LIST4[i]);
            }
            }
        }

        //shfaqim gjith requests
        ViewBag.requests = _context.Requests.Include(e=>e.Reciver).Include(e=>e.Sender).Where(e => e.ReciverId == id).Where(e => e.Accepted == false).ToList();
        // lista e filtruar ruhet ne viewbag
        ViewBag.perdoruesit= LIST4;
        //Marr te loguarin me te dhena
        ViewBag.iLoguari = _context.Users.FirstOrDefault(e => e.UserId == id);
        // shfaq gjith miqte
        ViewBag.miqte = _context.Requests.Where(e => (e.SenderId == id) || (e.ReciverId == id)).Include(e=>e.Reciver).Include(e=>e.Sender).Where(e=>e.Accepted ==true).ToList();
        return View();
    }

    [HttpGet("AcceptR/{id}")]
    public IActionResult AcceptR(int id)
    {
        
        Request requestii = _context.Requests.First(e => e.RequestId == id);
        requestii.Accepted=true;
        // _context.Remove(hiqFans);
        _context.SaveChanges();
        return RedirectToAction("index");
    }


    [HttpGet("DeclineR/{id}")]
    public IActionResult Decline(int id)
    {
        
        Request requestii = _context.Requests.First(e => e.RequestId == id);
         _context.Remove(requestii);
        _context.SaveChanges();
        return RedirectToAction("index");
    }

    [HttpGet("Connect/{id}")]
    public IActionResult SendR(int id)
    {
        int idFromSession = (int)HttpContext.Session.GetInt32("userId");
        Request newRequest = new Models.Request()
        {
            SenderId = idFromSession,
            ReciverId = id,
        };
        _context.Requests.Add(newRequest);
        _context.SaveChanges();
        // User dbUser = _context.Users.Include(e=>e.Requests).First(e=> e.UserId == idFromSession);
        // dbUser.Requests.Add(newRequest);
        _context.SaveChanges();
        return RedirectToAction("index");
    }

    [HttpGet("User/{id}")]
    public IActionResult UserId(int id)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Register");
        }

        ViewBag.marrNgaDb = _context.Users.ToList();
        User marrNgaDb= _context.Users.FirstOrDefault(e => e.UserId == id);
        return View("UserId", marrNgaDb);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
