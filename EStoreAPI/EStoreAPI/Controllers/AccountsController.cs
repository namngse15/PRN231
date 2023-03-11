﻿using AutoMapper;
using DataAccess.RepoImpl;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.Models;
using BusinessObject.Res;
using BusinessObject.Req;
using EStoreAPI.Config;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IConfiguration configuration;
        public static UserRes user = new();
        private IAccountRepo repository = new AccountRepo();
        private IMapper mapper;
        public AccountsController(IMapper _mapper, IConfiguration configuration)
        {
            this.configuration = configuration;
            mapper = _mapper;
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpGet]
        [Route("totalCustomersAccounts")]
        public async Task<IActionResult> GetTotalCustomers()
        {
            var data = await repository.CustomersAccounts();
            return Ok(data.Count());
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpGet]
        [Route("totalEmployeesAccounts")]
        public async Task<IActionResult> GetTotalEmployees()
        {
            var data = await repository.EmployeesAccounts();
            return Ok(data.Count());
        }

        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            if (id is null) return BadRequest();
            var account = await repository.Account(id);
            return account is null ? NotFound() : Ok(mapper.Map<AccRes>(account));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("reset")]
        public async Task<IActionResult> Get(string? email)
        {
            if (email is null) return BadRequest();
            var resetPassword = await repository.Account(email);
            return resetPassword is null ? NotFound() : Ok(MailConfig.SendRecoveryMail(email, resetPassword, configuration));
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> Post(SignUpReq req)
        {
            if (req is null) return BadRequest();
            var isSave = await repository.Save(req);
            if (isSave) return Ok(isSave);
            return Conflict();
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpPost]
        [Route("import-employees")]
        public async Task<IActionResult> Post(IFormFile file)
        {
            var isSave = false;
            var isSend = false;
            if (file is not null)
            {
                List<EmployeeAccount> employees = await ExcelConfig.import(file, "employee");
                employees.ForEach(async e =>
                {
                    isSave = await repository.Save(e);
                    if (isSave) isSend = MailConfig.SendRecoveryMail(e.Email!, e.Password!, configuration);
                });
                if (isSave && isSend) return Ok(isSave);
            }
            return Conflict(); ;
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpPost]
        [Route("add-employee")]
        public async Task<IActionResult> Post(EmployeeAccount employee)
        {
            if (employee is null) return BadRequest();
            var isSave = await repository.Save(employee);
            if (isSave) return Ok(MailConfig.SendRecoveryMail(employee.Email!, employee.Password!, configuration));
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("signin")]
        public async Task<ActionResult<string>> Login(AuthReq request)
        {
            if (request is null) return BadRequest();
            var account = await repository.Account(request);
            user.Account = account;
            user.AccessToken = JWTConfig.CreateToken(user, configuration);
            SetRefreshToken(JWTConfig.GenerateRefreshToken());
            return account is null ? NotFound() : Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("refresh-token")]
        public ActionResult<UserRes> RefreshToken(UserRes u)
        {
            var refreshToken = u.RefreshToken;

            if (!user.RefreshToken!.Equals(refreshToken)) return Unauthorized("Invalid Refresh Token.");
            else if (user.TokenExpires < DateTime.Now) return Unauthorized("Token expired.");
            string token = JWTConfig.CreateToken(user, configuration);
            user.AccessToken = token;
            var newRefreshToken = JWTConfig.GenerateRefreshToken();
            SetRefreshToken(newRefreshToken);
            return Ok(user);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
        }
    }
}
