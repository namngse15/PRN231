using BusinessObject.Models;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.RepoImpl
{
    public class EmpRepo : IEmpRepo
    {

        public async Task<Employee> Employee(int? id) => await EmpDAO.GetEmployeeById(id);

        public async Task<List<Employee>> Employees(PaginationParams @params, string? name) => await EmpDAO.GetEmployees(@params, name);

        public async Task<List<Employee>> Employees(string? name) => await EmpDAO.GetAll(name);

        public async Task<int> Save(Employee employee) => await EmpDAO.SaveEmployee(employee);

        public async Task<bool> Update(Employee employee) => await EmpDAO.UpdateEmployee(employee);

        public async Task<bool> Delete(Employee employee) => await EmpDAO.DeleteEmployee(employee);
    }
}
