using BusinessObject.Models;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.RepoImpl
{
    public class CustomerRepo : ICustomerRepo
    {
        public async Task<Customer> Customer(string? id) => await CustomerDAO.GetCustomerById(id);

        public async Task<List<Customer>> Customers(PaginationParams @params, string? name) => await CustomerDAO.GetCustomers(@params, name);

        public async Task<List<Customer>> Customers(string? name) => await CustomerDAO.GetAll(name);

        public async Task<bool> Delete(Customer customer) => await CustomerDAO.DeleteCustomer(customer);

        public async Task<string> Save(Customer customer) => await CustomerDAO.SaveCustomer(customer);

        public async Task<bool> Update(Customer customer) => await CustomerDAO.UpdateCustomer(customer);
    }
}
