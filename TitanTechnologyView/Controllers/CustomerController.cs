// Controllers/CustomerController.cs
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using TitanTechnologyView.Models;

namespace TitanTechnologyView.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // API endpoints
        private readonly string _apiOrigin = "https://localhost:44368";            // for files
        private readonly string _apiUrl = "https://localhost:44368/api/Customer"; // for JSON + POST

        public CustomerController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "API call failed: " + response.StatusCode;
                return View(new List<CustomerFormDto>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var customers = JsonConvert.DeserializeObject<List<CustomerFormDto>>(json) ?? new();
            return View(customers);
        }

        [HttpGet]
        public async Task<IActionResult> AddCustomer(int id = 0)
        {
            ViewBag.ApiOrigin = _apiOrigin; // so the view can build absolute links

            if (id == 0)
                return View(new CustomerFormDto());

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiUrl}/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<CustomerFormDto>(json) ?? new CustomerFormDto();
            return View(model);
        }

        // Helper: guess mime type by file extension
        private static string GuessMime(string fileName)
        {
            var ext = Path.GetExtension(fileName)?.ToLowerInvariant();
            return ext switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }

        [HttpPost]
        public async Task<IActionResult> SaveCustomer(CustomerFormDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ApiOrigin = _apiOrigin;
                return View("AddCustomer", model);
            }

            var client = _httpClientFactory.CreateClient();
            using var content = new MultipartFormDataContent();

            // Base fields
            content.Add(new StringContent(model.CustomerId.ToString()), "CustomerId");
            content.Add(new StringContent(model.CompanyCode ?? ""), "CompanyCode");
            content.Add(new StringContent(model.CustomerName ?? ""), "CustomerName");
            content.Add(new StringContent(model.Address ?? ""), "Address");
            content.Add(new StringContent(model.Country ?? ""), "Country");
            content.Add(new StringContent(model.Gstn ?? ""), "Gstn");
            content.Add(new StringContent(model.PanNumber ?? ""), "PanNumber");
            content.Add(new StringContent(model.ContactPersonName ?? ""), "ContactPersonName");
            content.Add(new StringContent(model.ContactPersonNumber ?? ""), "ContactPersonNumber");
            content.Add(new StringContent(model.PaymentTerms ?? ""), "PaymentTerms");

            // local function to attach new-or-existing file
            async Task AttachAsync(string field, IFormFile? newFile, string? existingRelativeUrl)
            {
                if (newFile != null)
                {
                    var sc = new StreamContent(newFile.OpenReadStream());
                    sc.Headers.ContentType = new MediaTypeHeaderValue(newFile.ContentType);
                    content.Add(sc, field, newFile.FileName);
                    return;
                }

                // No new file: if we have an existing path like "/agreements/abc.pdf", fetch it from API and re-attach
                if (!string.IsNullOrWhiteSpace(existingRelativeUrl))
                {
                    var absoluteUrl = existingRelativeUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                        ? existingRelativeUrl
                        : $"{_apiOrigin}{existingRelativeUrl}";

                    var fileBytes = await client.GetByteArrayAsync(absoluteUrl);
                    var fileName = Path.GetFileName(existingRelativeUrl);
                    var ba = new ByteArrayContent(fileBytes);
                    ba.Headers.ContentType = new MediaTypeHeaderValue(GuessMime(fileName));
                    content.Add(ba, field, fileName);
                }
                // else: nothing attached -> API will null it
            }

            await AttachAsync("AgreementFile1", model.AgreementFile1, model.Agreement1);
            await AttachAsync("AgreementFile2", model.AgreementFile2, model.Agreement2);
            await AttachAsync("AgreementFile3", model.AgreementFile3, model.Agreement3);
            await AttachAsync("AgreementFile4", model.AgreementFile4, model.Agreement4);

            // Your API uses POST for both insert/update
            var response = await client.PostAsync(_apiUrl, content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            // on error, redisplay form
            ViewBag.ApiOrigin = _apiOrigin;
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"API Error: {error}");
            return View("AddCustomer", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiUrl}/{id}");
            return RedirectToAction("Index");
        }
    }
}
