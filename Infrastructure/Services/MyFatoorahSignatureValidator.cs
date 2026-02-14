using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class MyFatoorahSignatureValidator : IPaymentSignatureValidator
    {
        public bool Validate(string rawJson, string signature, string secretKey)
        {
            if (string.IsNullOrEmpty(secretKey))
                return false;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawJson));
            var computedSignature = Convert.ToBase64String(hash);

            return computedSignature == signature;
        }
    }

}
