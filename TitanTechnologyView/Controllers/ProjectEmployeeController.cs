using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TitanTechnologyView.Models;

namespace TitanTechnologyView.Controllers
{
    public class ProjectEmployeeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "https://localhost:44368/api/ProjectEmployee"; // API Base URL

        public ProjectEmployeeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: List
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiBaseUrl);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to load data.";
                return View(new List<ProjectEmployee>());
            }

            var data = JsonConvert.DeserializeObject<List<ProjectEmployee>>(await response.Content.ReadAsStringAsync());
            return View(data);
        }

        // GET: Add/Edit
        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            if (id == null) return View(new ProjectEmployee());

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not load Project Employee.";
                return RedirectToAction("Index");
            }

            var data = JsonConvert.DeserializeObject<ProjectEmployee>(await response.Content.ReadAsStringAsync());
            return View(data);
        }

        // POST: Save
        [HttpPost]
        public async Task<IActionResult> Save(ProjectEmployee model)
        {
            if (!ModelState.IsValid)
                return View("AddEdit", model);

            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            if (model.Id == null || model.Id == 0) // Add
                response = await client.PostAsync(_apiBaseUrl, content);
            else // Update
                response = await client.PostAsync(_apiBaseUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Save failed!";
                return View("AddEdit", model);
            }

            TempData["Success"] = "Saved successfully!";
            return RedirectToAction("Index");
        }

        // DELETE
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/{id}");

            if (!response.IsSuccessStatusCode)
                TempData["Error"] = "Delete failed!";
            else
                TempData["Success"] = "Deleted successfully!";

            return RedirectToAction("Index");
        }
    }
}
