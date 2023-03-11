﻿using BusinessObject.Models;
using BusinessObject.Req;
using DataAccess.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataAccess.DAO
{
    public class AccountDAO
    {
        private static readonly string Message = "Employee identity is null";
        public static async Task<List<Account>> GetCustomerAccounts()
        {
            var accounts = new List<Account>();
            using (var context = new PRN231DBContext())
            {
                accounts = await context.Accounts.Where(x => x.Role == 2).ToListAsync();
            }
            return accounts;
        }

        public static async Task<List<Account>> GetEmployeeAccounts()
        {
            var accounts = new List<Account>();
            using (var context = new PRN231DBContext())
            {
                accounts = await context.Accounts.Where(x => x.Role == 1).ToListAsync();
            }
            return accounts;
        }

        public static async Task<Account> GetAccount(int? id)
        {
            Account? account;
            using (var context = new PRN231DBContext())
            {
                account = await context.Accounts.Where(a => a.AccountId == id).FirstOrDefaultAsync(); ;
            }
            return account ?? new();
        }

        public static async Task<Account> GetAccount(AuthReq req)
        {
            Account? account;
            using (var context = new PRN231DBContext())
            {
                account = await context.Accounts.Where(a => a.Email!.Equals(req.Email) && a.Password!.Equals(req.Password)).FirstOrDefaultAsync(); ;
            }
            return account ?? new();
        }

        public static async Task<string?> GetAccount(string? email)
        {
            Account? account;
            using (var context = new PRN231DBContext())
            {
                account = await context.Accounts.Where(a => a.Email!.Equals(email)).FirstOrDefaultAsync(); 
                return account is not null ? RandomUtils.GenerateNewPassword(8) : null;
            }
        }

        public static async Task<bool> SaveCustomer(SignUpReq req)
        {
            req.customer!.CustomerId = RandomUtils.GenerateId(5);
            while (await CustomerDAO.GetCustomerById(req.customer.CustomerId) != null)
            {
                req.customer!.CustomerId = RandomUtils.GenerateId(5);
            }
            using (var context = new PRN231DBContext())
            {
                Account account = new Account
                {
                    Email = req.Email,
                    Password = req.Password,
                    CustomerId = req.customer!.CustomerId,
                    Customer = new Customer()
                    {
                        CustomerId = req.customer!.CustomerId,
                        CompanyName = req.customer!.CompanyName ?? "Notyet",
                        ContactName = req.customer!.ContactName,
                        ContactTitle = req.customer!.ContactTitle,
                        Address = req.customer!.Address
                    },
                    Role = req.Role
                };
                await context.Accounts.AddAsync(account);
                return await context.SaveChangesAsync() > 0;
            }
        }

        public static async Task<bool> SaveEmployee(EmployeeAccount employee)
        {
            var empId = await EmpDAO.SaveEmployee(GetEmployee(employee));
            if (empId == 0) throw new Exception(Message);
            using(var context = new PRN231DBContext())
            {
                Account account = new Account
                {
                    Email = employee.Email,
                    Password = employee.Password,
                    EmployeeId = empId,
                    Role = employee.Role
                };
                await context.Accounts.AddAsync(account);
                return await context.SaveChangesAsync() > 0;
            }
        }

        internal static Employee GetEmployee(EmployeeAccount employee)
        {
            return new Employee
            {
                LastName = employee.LastName,
                FirstName = employee.FirstName,
                BirthDate = employee.BirthDate,
                Address = employee.Address,
                DepartmentId = employee.DepartmentId,
                HireDate = employee.HireDate,
                Title = employee.Title,
                TitleOfCourtesy = employee.TitleOfCourtesy
            };
        }
    }
}
