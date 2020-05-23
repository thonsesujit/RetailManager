using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TRMDataManager.Models
{
    /// <summary>
    /// Getting all user roles
    /// </summary>
    public class ApplicationUserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public Dictionary<string, string> Roles { get; set; } = new Dictionary<string, string>();
    }
}