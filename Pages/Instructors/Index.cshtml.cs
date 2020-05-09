using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MeroUniversity.Data;
using MeroUniversity.Models;
using MeroUniversity.Models.SchoolViewModels;

namespace MeroUniversity.Pages.Instructors
{
    public class IndexModel : PageModel
    {
        private readonly MeroUniversity.Data.SchoolContext _context;

        public IndexModel(MeroUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public InstructorIndexData InstructorData { get; set; }
        public int InstructorID { get; set; }
        public int CourseID { get; set; }
        public async Task OnGetAsync(int?id,int?courseID)
        {
            InstructorData = new InstructorIndexData();
            InstructorData.Instructors = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i=>i.CourseAssignments)
                    .ThenInclude(i=>i.Course)
                        .ThenInclude(i=>i.Department)
                //.Include(i=>i.CourseAssignments)
                //    .ThenInclude(i=>i.Course)
                //        .ThenInclude(i=>i.Enrollments)
                //            .ThenInclude(i=>i.Student)
                //.AsNoTracking()
                //asnotracking is to be commented out because 
                //navigation properties only be explicitly loaded for the tracked entities
                //the commented method uses the eager loading 
                //we are now using explicit loading 
                .OrderBy(i=>i.LastName)
                .ToListAsync();

            if (id != null)
            {
                InstructorID = id.Value;
                Instructor instructor = InstructorData.Instructors
                    //.Single(i => i.ID == id.Value);                    
                    .Where(i => i.ID == id.Value).Single();
                //single can be used directly in place of line above
                InstructorData.Courses = instructor.CourseAssignments
                    .Select(s=>s.Course);
            }

            if (courseID != null)
            {
                courseID = courseID.Value;
                var selectedCourses = InstructorData.Courses
                    .Where(x => x.CourseID == courseID).Single();
                //the code below is used fro the explicit loading
                //i.e. if we rarely want to see enrollments in a course
                //we only load the enrollment data if it's requested
                await _context.Entry(selectedCourses).Collection
                    (x => x.Enrollments).LoadAsync();
                foreach(Enrollment enrollment in selectedCourses.Enrollments)
                {
                    await _context.Entry(enrollment).Reference(
                        x => x.Student).LoadAsync();
                }
                InstructorData.Enrollments = selectedCourses.Enrollments;
                //InstructorData.Enrollments = InstructorData.Courses.
                //    Single(x => x.CourseID == courseID).Enrollments;
            }
        }
    }
}
