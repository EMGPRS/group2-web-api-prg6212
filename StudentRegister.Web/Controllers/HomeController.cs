using Microsoft.AspNetCore.Mvc;
using StudentRegister.Web.Models;
using System.Diagnostics;
using System.Net.Http.Json;

namespace StudentRegister.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _studentApi;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _studentApi = httpClientFactory.CreateClient("StudentApi");
        }

        public async Task<IActionResult> Index(int? editId)
        {
            var students = await _studentApi.GetFromJsonAsync<List<Student>>("api/Student") ?? [];
            var studentToEdit = editId.HasValue
                ? students.FirstOrDefault(student => student.Id == editId.Value)
                : null;

            return View(new StudentRegisterViewModel
            {
                Students = students,
                Student = studentToEdit ?? new Student()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Save(Student student)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            if (student.Id == 0)
            {
                await _studentApi.PostAsJsonAsync("api/Student", student);
            }
            else
            {
                await _studentApi.PutAsJsonAsync($"api/Student/{student.Id}", student);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _studentApi.DeleteAsync($"api/Student/{id}");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
