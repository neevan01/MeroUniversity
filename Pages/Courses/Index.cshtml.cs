using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MeroUniversity.Data;
using MeroUniversity.Models;

namespace MeroUniversity.Pages.Courses
{
    public class IndexModel : PageModel
    {
        private readonly MeroUniversity.Data.SchoolContext _context;

        public IndexModel(MeroUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public IList<Course> Courses { get;set; }

        public async Task OnGetAsync()
        {
            Courses = await _context.Courses
                .Include(c => c.Department)
                .AsNoTracking().ToListAsync();
        }
    }
}
