using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class MyFatoorahWebhookDto
    {
        public string Event { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public MyFatoorahWebhookData Data { get; set; } = new();
    }
}
