using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using TitanTechnologyView.Models;

namespace TitanTechnologyView.Controllers
{
    public class VendorController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiOrigin = "https://localhost:44368";            
        private readonly string _apiUrl = "https://localhost:44368/api/Vendor";

        public VendorController(IHttpClientFactory httpClientFactory)
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
                return View(new List<VendorMaster>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var vendors = JsonConvert.DeserializeObject<List<VendorMaster>>(json) ?? new();
            return View(vendors);
        }

        [HttpGet]
        public async Task<IActionResult> VendorForm(int? id = 0)
        {
            ViewBag.ApiOrigin = _apiOrigin;

            if (id == null || id == 0)
                return View(new VendorDto());

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiUrl}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<VendorDto>(json);
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
        public async Task<IActionResult> SaveVendor(VendorDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ApiOrigin = _apiOrigin;
                return View("VendorForm", model);
            }

            var client = _httpClientFactory.CreateClient();
            using var form = new MultipartFormDataContent();

            // Add all basic string fields
            form.Add(new StringContent(model.VendorId?.ToString() ?? "0"), "VendorId");
            form.Add(new StringContent(model.VendorName ?? ""), "VendorName");
            form.Add(new StringContent(model.CompanyCode ?? ""), "CompanyCode");
            form.Add(new StringContent(model.Address ?? ""), "Address");
            form.Add(new StringContent(model.Gstn ?? ""), "Gstn");
            form.Add(new StringContent(model.PanNumber ?? ""), "PanNumber");
            form.Add(new StringContent(model.ContactPersonName ?? ""), "ContactPersonName");
            form.Add(new StringContent(model.ContactPersonNumber ?? ""), "ContactPersonNumber");
            form.Add(new StringContent(model.PaymentTerms ?? ""), "PaymentTerms");
            form.Add(new StringContent(model.AccountHolderName ?? ""), "AccountHolderName"); 
            form.Add(new StringContent(model.AccountNumber1 ?? ""), "AccountNumber1"); 
            form.Add(new StringContent(model.IfscCode ?? ""), "IfscCode"); 
            form.Add(new StringContent(model.BankAccountName ?? ""), "BankAccountName");

            // Attach files
            async Task AttachFile(string field, IFormFile? newFile, string? existingRelativeUrl)
            {
                if (newFile != null)
                {
                    var sc = new StreamContent(newFile.OpenReadStream());
                    sc.Headers.ContentType = new MediaTypeHeaderValue(newFile.ContentType);
                    form.Add(sc, field, newFile.FileName);
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
                    form.Add(ba, field, fileName);
                }
                // else: nothing attached -> API will null it
            }

            await AttachFile("GstnUpload", model.GstnUpload, model.ExistingGstnUpload);
            await AttachFile("PanUpload", model.PanUpload, model.ExistingPanUpload);
            await AttachFile("CancelledCheque", model.CancelledCheque, model.ExistingCancelledCheque);
            await AttachFile("Agreement1", model.Agreement1, model.ExistingAgreement1);
            await AttachFile("Agreement2", model.Agreement2, model.ExistingAgreement2);
            await AttachFile("Agreement3", model.Agreement3, model.ExistingAgreement3);
            await AttachFile("Agreement4", model.Agreement4, model.ExistingAgreement4);

            var response = await client.PostAsync(_apiUrl, form);
 
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            // on error, redisplay form
            ViewBag.ApiOrigin = _apiOrigin;
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"API Error: {error}");
            return View("VendorForm", model);
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


//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
//using System.Net.Http.Headers;
//using TitanTechnologyView.Models;

//namespace TitanTechnologyView.Controllers
//{
//    public class VendorController : Controller
//    {
//        private readonly IWebHostEnvironment _env;
//        private readonly IHttpClientFactory _httpClientFactory;
//        private readonly string _apiUrl = "https://localhost:44368/api/Vendor";

//        public VendorController(IWebHostEnvironment env, IHttpClientFactory httpClientFactory)
//        {
//            _env = env;
//            _httpClientFactory = httpClientFactory;
//        }

//        [HttpGet]
//        public async Task<IActionResult> Index()
//        {
//            var client = _httpClientFactory.CreateClient();
//            var response = await client.GetAsync(_apiUrl);

//            if (!response.IsSuccessStatusCode)
//            {
//                ViewBag.Error = "API call failed: " + response.StatusCode;
//                return View(new List<VendorMaster>());
//            }

//            var json = await response.Content.ReadAsStringAsync();
//            var vendors = JsonConvert.DeserializeObject<List<VendorMaster>>(json);
//            return View(vendors);
//        }

//        [HttpGet]
//        public async Task<IActionResult> AddVendorAsync(int? id)
//        {
//            if (id == null || id == 0)
//                return View(new VendorDto());

//            var client = _httpClientFactory.CreateClient();
//            var response = await client.GetAsync($"{_apiUrl}/{id}");

//            if (!response.IsSuccessStatusCode)
//                return RedirectToAction("Index");

//            var vendor = await response.Content.ReadFromJsonAsync<VendorDto>();
//            return View(vendor);
//        }

//        [HttpPost]
//        public async Task<IActionResult> SaveVendor(VendorDto model)
//        {
//            if (!ModelState.IsValid)
//                return View("AddVendor", model);

//            var client = _httpClientFactory.CreateClient();
//            using var form = new MultipartFormDataContent();

//            // Basic fields
//            form.Add(new StringContent(model.VendorId?.ToString() ?? "0"), nameof(model.VendorId));
//            form.Add(new StringContent(model.VendorName ?? ""), nameof(model.VendorName));
//            form.Add(new StringContent(model.CompanyCode ?? ""), nameof(model.CompanyCode));
//            form.Add(new StringContent(model.Address ?? ""), nameof(model.Address));
//            form.Add(new StringContent(model.Gstn ?? ""), nameof(model.Gstn));
//            form.Add(new StringContent(model.PanNumber ?? ""), nameof(model.PanNumber));
//            form.Add(new StringContent(model.ContactPersonName ?? ""), nameof(model.ContactPersonName));
//            form.Add(new StringContent(model.ContactPersonNumber ?? ""), nameof(model.ContactPersonNumber));
//            form.Add(new StringContent(model.PaymentTerms ?? ""), nameof(model.PaymentTerms));
//            form.Add(new StringContent(model.AccountHolderName ?? ""), nameof(model.AccountHolderName));
//            form.Add(new StringContent(model.AccountNumber1 ?? ""), nameof(model.AccountNumber1));
//            form.Add(new StringContent(model.IfscCode ?? ""), nameof(model.IfscCode));
//            form.Add(new StringContent(model.BankAccountName ?? ""), nameof(model.BankAccountName));

//            var uploadRoot = Path.Combine(_env.WebRootPath, "uploads", "vendors");
//            if (!Directory.Exists(uploadRoot))
//                Directory.CreateDirectory(uploadRoot);


//            // 🟢 Helper function for files
//            async Task<string> SaveFile(IFormFile? file, string existingFile)
//            {
//                if (file != null && file.Length > 0)
//                {
//                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
//                    var filePath = Path.Combine(uploadRoot, fileName);
//                    using (var stream = new FileStream(filePath, FileMode.Create))
//                    {
//                        await file.CopyToAsync(stream);
//                    }
//                    return $"/uploads/vendors/{fileName}";
//                }
//                return existingFile; // keep old if new not uploaded
//            }

//            model.ExistingGstnUpload = await SaveFile(model.GstnUpload, model.ExistingGstnUpload);
//            model.ExistingPanUpload = await SaveFile(model.PanUpload, model.ExistingPanUpload);
//            model.ExistingCancelledCheque = await SaveFile(model.CancelledCheque, model.ExistingCancelledCheque);
//            model.ExistingAgreement1 = await SaveFile(model.Agreement1, model.ExistingAgreement1);
//            model.ExistingAgreement2 = await SaveFile(model.Agreement2, model.ExistingAgreement2);
//            model.ExistingAgreement3 = await SaveFile(model.Agreement3, model.ExistingAgreement3);
//            model.ExistingAgreement4 = await SaveFile(model.Agreement4, model.ExistingAgreement4);

//            HttpResponseMessage response;
//            if (model.VendorId == 0) // Add
//                response = await client.PostAsJsonAsync(_apiUrl, model);
//            else // Update
//                response = await client.PutAsJsonAsync($"{_apiUrl}/{model.VendorId}", model);

//            if (response.IsSuccessStatusCode)
//            {
//                TempData["Success"] = model.VendorId == 0 ? "Vendor added successfully!" : "Vendor updated successfully!";
//                return RedirectToAction("Index");
//            }

//            TempData["Error"] = "Error: " + await response.Content.ReadAsStringAsync();
//            return View("AddVendor", model);
//        }

//        [HttpGet]
//        public async Task<IActionResult> EditVendor(int id)
//        {
//            var client = _httpClientFactory.CreateClient();
//            var response = await client.GetAsync($"{_apiUrl}/{id}");

//            if (!response.IsSuccessStatusCode)
//            {
//                TempData["Error"] = "Vendor not found!";
//                return RedirectToAction("Index");
//            }

//            var json = await response.Content.ReadAsStringAsync();
//            var vendor = JsonConvert.DeserializeObject<VendorMaster>(json);

//            var dto = new VendorDto
//            {
//                VendorId = vendor.VendorId,
//                VendorName = vendor.VendorName,
//                CompanyCode = vendor.CompanyCode,
//                Address = vendor.Address,
//                Gstn = vendor.Gstn,
//                PanNumber = vendor.PanNumber,
//                ContactPersonName = vendor.ContactPersonName,
//                ContactPersonNumber = vendor.ContactPersonNumber,
//                PaymentTerms = vendor.PaymentTerms,
//                AccountHolderName = vendor.AccountHolderName,
//                AccountNumber1 = vendor.AccountNumber1,
//                IfscCode = vendor.IfscCode,
//                BankAccountName = vendor.BankAccountName,
//                ExistingGstnUpload = vendor.GstnUpload,
//                ExistingPanUpload = vendor.PanUpload,
//                ExistingCancelledCheque = vendor.CancelledCheque,
//                ExistingAgreement1 = vendor.Agreement1,
//                ExistingAgreement2 = vendor.Agreement2,
//                ExistingAgreement3 = vendor.Agreement3,
//                ExistingAgreement4 = vendor.Agreement4
//            };

//            return View("AddVendor", dto); // reuse same form
//        }

//        [HttpGet]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var client = _httpClientFactory.CreateClient();
//            await client.DeleteAsync($"{_apiUrl}/{id}");
//            return RedirectToAction("Index");
//        }
//    }
//}
