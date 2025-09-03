﻿using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TitanTechnologyView.Models;

namespace TitanTechnologyView.Controllers
{
    public class AuthController : Controller
    {

        [HttpGet]
        public IActionResult Register()
        {
            return View(new Register());
        }

        // POST: submit form to the API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = new HttpClient();

            try
            {
                var response = await client.PostAsJsonAsync("https://localhost:44368/api/Register", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Registration successful!";
                    return RedirectToAction(nameof(Login));
                }

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var payload = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", "Validation failed: " + payload);
                    return View(model);
                }

                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"API error ({(int)response.StatusCode}): {error}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error contacting API: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // CALL LOGIN API HERE
        [HttpPost]
        public async Task<IActionResult> LoginWithPassword(string contact_number, string password)
        {
            if (string.IsNullOrWhiteSpace(contact_number) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Contact number and password are required.");
                return RedirectToAction(nameof(Login));
            }

            using var client = new HttpClient();

            var loginRequest = new LoginRequest
            {
                ContactNumber = contact_number,
                Password = password
            };

            try
            {
                var response = await client.PostAsJsonAsync("https://localhost:44368/api/Login/password", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    using var stream = await response.Content.ReadAsStreamAsync();
                    using var doc = await JsonDocument.ParseAsync(stream);

                    // Extract userRole directly from JSON
                    var root = doc.RootElement;
                    var userRole = root.GetProperty("user").GetProperty("userRole").GetString();

                    if (!string.IsNullOrEmpty(userRole))
                    {
                        HttpContext.Response.Cookies.Append("UserRole", userRole, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.UtcNow.AddHours(1)
                        });
                    }

                    TempData["SuccessMessage"] = "Login successful!";
                    return RedirectToAction("Index", "Company");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError("", "Invalid contact number or password.");
                    return RedirectToAction(nameof(Login));
                }

                // Handle other errors
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"API error ({(int)response.StatusCode}): {error}");
                return RedirectToAction(nameof(Login));
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Error contacting API: " + ex.Message);
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unexpected error: " + ex.Message);
                return RedirectToAction(nameof(Login));
            }
        }
    }

    public class LoginRequest
    {
        [JsonPropertyName("contact_number")]
        public string ContactNumber { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

}