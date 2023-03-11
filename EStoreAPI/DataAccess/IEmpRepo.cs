using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IEmpRepo
    {
        Task<List<Employee>> Employees(PaginationParams @params, string? name);
        Task<List<Employee>> Employees(string? name);
        Task<Employee> Employee(int? id);
        Task<int> Save(Employee employee);
        Task<bool> Update(Employee employee);
        Task<bool> Delete(Employee employee);
    }
}
