using Microsoft.AspNetCore.Mvc;
using Moq;
using StudentRegister.Api.Controllers;
using StudentRegister.Api.Models;
using StudentRegister.Api.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentRegister.Api.Tests
{
    public class StudentControllerTests
    {
        [Fact]
        public async Task GetStudents_ReturnOkWithStudents()
        {
            //Arrange
            var students = new List<Student>
            {
                new Student { Id = 1, FirstName = "John", LastName = "Doe" },
                new Student { Id = 2, FirstName = "Jane", LastName = "Smith" }
            };
            var studentServiceMock = new Mock<IStudentService>();
            studentServiceMock.Setup(service => service.GetStudentsAsync()).ReturnsAsync(students);
            var controller = new StudentController(studentServiceMock.Object);

            //Act
            var result = await controller.GetStudents();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(students, okResult.Value);
            studentServiceMock.Verify(service => service.GetStudentsAsync(), Times.Once);

        }


        [Fact]
        public async Task SearchStudents_NoResultsReturnsNotFound()
        {
            //Arrange
            var searchTerm = "ST0001920200303";
            var studentServiceMock = new Mock<IStudentService>();
            studentServiceMock.Setup(service => service.SearchStudentsAsync(searchTerm)).ReturnsAsync(new List<Student>());
            var controller = new StudentController(studentServiceMock.Object);

            //Act
            var result = await controller.SearchStudents(searchTerm);

            //Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"No records matching {searchTerm}", notFoundResult.Value);
            studentServiceMock.Verify(service => service.SearchStudentsAsync(searchTerm), Times.Once);
        }

        [Fact]
        public async Task CreateStudent_RetrunsCreatedActionWithId()
        {
            //Arrange
            var student = new Student { Id = 5, FirstName = "John", LastName = "Doe" };
            var studentServiceMock = new Mock<IStudentService>();
            studentServiceMock.Setup(service => service.CreateStudentAsync(student)).ReturnsAsync(student);
            var controller = new StudentController(studentServiceMock.Object);

            //Act
            var result = await controller.CreateStudent(student);

            //Assert
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(StudentController.GetStudentById), created.ActionName);
            Assert.Equal(5, created.RouteValues?["id"]);

        }

        [Fact]
        public async Task GetStudentById_ExistingStudentReturnsOk()
        {
            //Arrange
            var student = new Student { Id = 1, FirstName = "John" };
            var studentServiceMock = new Mock<IStudentService>();
            studentServiceMock.Setup(service => service.GetStudentByIdAsync(1)).ReturnsAsync(student);
            var controller = new StudentController(studentServiceMock.Object);

            //Act
            var result = await controller.GetStudentById(1);


            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(student, okResult.Value);
        }

    }
}
