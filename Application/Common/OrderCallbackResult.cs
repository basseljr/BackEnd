using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public class OrderCallbackResult
    {
        public bool IsSuccess { get; set; }
        public bool IsAlreadyProcessed { get; set; }
        public Order? Order { get; set; }
        public string? ErrorMessage { get; set; }

        public static OrderCallbackResult Success(Order order) =>
            new() { IsSuccess = true, Order = order };

        public static OrderCallbackResult Failed(string? error = null) =>
            new() { IsSuccess = false, ErrorMessage = error };

        public static OrderCallbackResult AlreadyProcessed(Order order) =>
            new() { IsSuccess = true, IsAlreadyProcessed = true, Order = order };
    }

}
