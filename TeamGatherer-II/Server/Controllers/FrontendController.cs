using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using TeamGatherer.Shared;

namespace TeamGatherer.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FrontendController : ControllerBase
    {
        public Task<StaffConfig> GetStaffConfigAsync([FromServices]IOptions<StaffConfig> options)
        {
            return Task.FromResult(options.Value);
        }
    }
}
