using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Accountant.WebApplication.Commons
{
    public class CookiesManager
    {
        public static void AddCookie(HttpRequest Request, HttpResponse Response, string key, object value, bool httpOnly = true, DateTime? expirationDate = null)
        {
            RemoveCookie(Request, Response, key);

            var json = ObjectToJson(value);
            var encryptedData = EncryptData(json);

            var cookieOptions = new CookieOptions()
            {
                IsEssential = true,
                HttpOnly = httpOnly,
                Expires = expirationDate ?? DateTime.Now.AddDays(7),
                SameSite = SameSiteMode.None,
            };

            Response.Cookies.Append(key, encryptedData, cookieOptions);
        }

        public static T GetCookie<T>(HttpRequest Request, string key)
        {
            var json = Request.Cookies[key];

            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }

            var decryptedData = DecryptData(json);

            return JsonToObject<T>(decryptedData);
        }

        public static void RemoveCookie(HttpRequest Request, HttpResponse Response, string key)
        {
            var json = Request.Cookies[key];

            if (!string.IsNullOrWhiteSpace(json))
            {
                Response.Cookies.Delete(key);
            }
        }

        private static string ObjectToJson(object objectValue)
        {
            var jsonString = JsonSerializer.Serialize(objectValue);
            return jsonString;
        }

        private static T JsonToObject<T>(string json)
        {
            var objectValue = JsonSerializer.Deserialize<T>(json);
            return objectValue;
        }

        private static string EncryptData(string data)
        {
            return Encryption.Encrypt(data);
        }

        private static string DecryptData(string data)
        {
            return Encryption.Decrypt(data);
        }
    }
}
