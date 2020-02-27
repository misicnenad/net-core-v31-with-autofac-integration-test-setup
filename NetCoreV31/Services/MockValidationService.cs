using NetCoreV31.Interfaces;

namespace NetCoreV31.Services
{
    public class MockValidationService : IRequestValidationService
    {
        public bool RequestCanBeProcessed()
        {
            return true;
        }
    }
}
