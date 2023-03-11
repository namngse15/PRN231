using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class EmpDAO
    {
        public static async Task<List<Employee>> GetEmployees(PaginationParams @params, string? name)
        {
            var emps = await GetAll(name);
            return emps
                .Skip((@params.Page - 1) * @params.ItemsPerPage)
                .Take(@params.ItemsPerPage)
                .ToList();
        }

        public static async Task<List<Employee>> GetAll(string? name)
        {
            var employees = new List<Employee>();
            using (var context = new PRN231DBContext())
            {
                employees = await context.Employees.Include(x => x.Department).
                    Where(x => name == null || x.FullName.Contains(name)).ToListAsync();
            }
            return employees;
        }

        public static async Task<Employee> GetEmployeeById(int? id)
        {
            Employee? employee;
            using (var context = new PRN231DBContext())
            {
                employee = await context
                    .Employees.Include(x => x.Department)
                    .SingleOrDefaultAsync(x => x.EmployeeId == id);
            }
            return employee ?? new();
        }

        public static async Task<int> SaveEmployee(Employee employee)
        {
            using (var context = new PRN231DBContext())
            {
                await context.Employees.AddAsync(employee);
                await context.SaveChangesAsync();
                return employee.EmployeeId;
            }
        }

        public static async Task<bool> UpdateEmployee(Employee employee)
        {
            using (var context = new PRN231DBContext())
            {
                context.Entry<Employee>(employee).State
                = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return await context.SaveChangesAsync() > 0;
            }
        }

        public static async Task<bool> DeleteEmployee(Employee employee)
        {
            using (var context = new PRN231DBContext())
            {
                context.Entry<Employee>(employee).State
                = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                return await context.SaveChangesAsync() > 0;
            }
        }
    }
}
