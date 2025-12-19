using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class TenantOwnerRegisterRequest
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string BusinessName { get; set; } = "";   // Used to create Tenant
        public string Subdomain { get; set; } = "";      // Automatically generated or provided
        public int? TemplateId { get; set; }   // 🔥 ADD THIS

    }
}
