using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TitanTechnologyView.Models;

namespace TitanTechnologyView.Controllers
{
    public class TimesheetController : Controller
    {
        private readonly string apiBaseUrl = "https://localhost:44368/api/Timesheet";

        // Show Add/Edit Form
        public IActionResult AddEdit(int? id)
        {
            Timesheet model = new Timesheet();

            if (id != null && id > 0)
            {
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync($"{apiBaseUrl}/{id}").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var json = response.Content.ReadAsStringAsync().Result;
                        model = JsonConvert.DeserializeObject<Timesheet>(json);
                    }
                }
            }

            return View(model);
        }

        // Handle Save (Add or Update)
        [HttpPost]
        public IActionResult SaveTimesheet(Timesheet model)
        {
            using (var client = new HttpClient())
            {
                string jsonData = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(apiBaseUrl, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Error saving data");
                    return View("AddEdit", model);
                }
            }
        }

        // Show List Page
        public IActionResult Index()
        {
            List<Timesheet> list = new List<Timesheet>();

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(apiBaseUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    list = JsonConvert.DeserializeObject<List<Timesheet>>(json);
                }
            }

            return View(list);
        }

        public IActionResult Delete(int id)
        {
            using (var client = new HttpClient())
            {
                var response = client.DeleteAsync($"{apiBaseUrl}/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }
    }
}
