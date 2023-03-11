﻿using BusinessObject.Models;
using BusinessObject.Req;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IAccountRepo
    {
        Task<List<Account>> CustomersAccounts();

        Task<List<Account>> EmployeesAccounts();

        Task<Account> Account(int? id);

        Task<Account> Account(AuthReq req);

        Task<string?> Account(string? email);

        Task<bool> Save(SignUpReq req);

        Task<bool> Save(EmployeeAccount employee);

    }
}
