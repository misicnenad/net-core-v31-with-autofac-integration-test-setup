using NetCoreV31.Interfaces;

using System;

namespace NetCoreV31.Services
{
    public class RequestValidationService : IRequestValidationService
    {
        public bool RequestCanBeProcessed()
        {
            var random = new Random();
            return random.Next(10) < 5;
        }
    }
}