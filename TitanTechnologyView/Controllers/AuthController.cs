using Microsoft.AspNetCore.Mvc;

namespace TitanTechnologyView.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password, string userType)
        {
            // Just a sample logic (replace with DB authentication)
            if (email == "admin@gmail.com" && password == "Passwoord@12345" && userType == "Admin")
            {
                return RedirectToAction("AdminDashboard", "Dashboard");
            }
            else if (userType == "Vendor")
            {
                return RedirectToAction("VendorDashboard", "Dashboard");
            }
            else if (userType == "Customer")
            {
                return RedirectToAction("CustomerDashboard", "Dashboard");
            }
            else if (userType == "Employee")
            {
                return RedirectToAction("EmployeeDashboard", "Dashboard");
            }
            else
            {
                ViewBag.Error = "Invalid login details!";
                return View();
            }
        }

    }
}
