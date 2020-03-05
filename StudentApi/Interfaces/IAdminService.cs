using StudentApi.Entities.Authorization;
using StudentApi.Helpers;
using StudentApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Interfaces
{
    public interface IAdminService
    {
        Task <OperationResult>AddStudent(string albumNumber, string email, string firstName, string lastName);

        Task<List<Student>> GetAllStudents();
    }
}
