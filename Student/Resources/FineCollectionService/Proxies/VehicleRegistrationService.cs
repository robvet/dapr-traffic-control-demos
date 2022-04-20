using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FineCollectionService.Models;

namespace FineCollectionService.Proxies
{
    public class VehicleRegistrationService
    {
        // You still reference httpClient, but underneath the hood, it uses Dapr Service Invocation building block to communicate, due to regisration in Startup
        private HttpClient _httpClient;

        public VehicleRegistrationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<VehicleInfo> GetVehicleInfo(string licenseNumber)
        {
            // Dapr call -- note we remove the domain and port.
            return await _httpClient.GetFromJsonAsync<VehicleInfo>($"/vehicleinfo/{licenseNumber}");
            // Non-Dapr code.
            //return await _httpClient.GetFromJsonAsync<VehicleInfo>($"http://localhost:6002/vehicleinfo/{licenseNumber}");
        }       
    }
}