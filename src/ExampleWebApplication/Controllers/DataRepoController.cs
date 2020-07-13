using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ExampleWebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataRepoController : Controller
    {
        private readonly IDataRepo _dataRepo;

        public DataRepoController(IDataRepo dataRepo)
        {
            _dataRepo = dataRepo;
        }

        public IActionResult Index()
        {
            return Ok(_dataRepo.GetNames());
        }
    }
}
