using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using TitanTechnologyView.Models;

namespace TitanTechnologyView.Controllers
{
    public class ProjectMasterController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl = "https://localhost:44368/api/ProjectMaster";

        public ProjectMasterController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // List
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "API call failed: " + response.StatusCode;
                return View(new List<Timesheet>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var timesheets = JsonConvert.DeserializeObject<List<Timesheet>>(json) ?? new();
            return View(timesheets);
        }

        // Add / Edit form
        [HttpGet]
        public async Task<IActionResult> AddEdit(string? projectCode)
        {
            if (string.IsNullOrEmpty(projectCode))
                return View(new ProjectMaster()); // Add form

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiUrl}/{projectCode}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<ProjectMaster>(json) ?? new ProjectMaster();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProjectMaster model)
        {
            model.ProjectCode = model.ProjectCode?.Trim();

            if (!ModelState.IsValid)
                return View("AddEdit", model);

            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_apiUrl, content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"API Error: {error}");
            return View("AddEdit", model);
        }


        // Delete
        [HttpGet]
        public async Task<IActionResult> Delete(string projectCode)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiUrl}/{projectCode}");
            return RedirectToAction("Index");
        }
    }
}
