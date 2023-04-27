using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Models
{
    public static class ApplicationSession
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (value == null)
            {

            }
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }

    public class SessionVariable
    {
        public const string UserSession = "UserSession";
        public const string AdminSession = "AdminSession";
    }
    public class UserSession:Error
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string displayname { get; set; }
        public string profilepicture { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int UserType { get; set; }
        public int UserStatus { get; set; }
        public string UserRole { get; set; }
        public List<DropDownKeyValue> pagepermission { get; set; }
        public decimal MyWallet { get; set; }
        public bool IsReferShowed { get; set; }
    }
}
