using System.Linq;
using DeletingDataService.Data;
using DeletingDataService.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeletingDataService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeletingDataController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DeletingDataController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public ActionResult<string> DeleteUserData([FromBody] string username)
        {
            _context.Devices.RemoveRange(_context.Devices.Where(d=>d.Profile.UserName==username));
            _context.SaveChanges();
            
            _context.Contracts.RemoveRange(_context.Contracts.Where(d=>d.Principal.UserName==username));
            _context.SaveChanges();

            foreach (var item in _context.Contracts.Where(c=>c.Mandatory.UserName==username && c.Status==ContractStatus.Taken))
            {
                item.MandatoryID=null;
                item.Mandatory=null;
                item.Status=ContractStatus.Available;
            }

            var appuser = _context.AppUsers.Single(u => u.Profile.UserName == username);
            _context.AppUsers.Remove(appuser);
            _context.SaveChanges();

            var profile = _context.Profiles.Single(p => p.UserName == username);
            _context.Profiles.Remove(profile);
            _context.SaveChanges();

            return Ok("User data deleted");
        }
    }
}