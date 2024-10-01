using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Packages;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {


        [HttpGet("hello")]
        public string GetString()
        {
            return "gamarjoba";
        }

        [HttpPut("employees")]
        public void PutEmployee(Employee employee)
        {
            PKG_EMP pKG = new();
            pKG.AddEmployee(employee);
        }
        
    }
}
