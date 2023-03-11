﻿using AutoMapper;
using BusinessObject.Models;
using BusinessObject.Res;
using DataAccess.RepoImpl;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using BusinessObject.Req;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private ICustomerRepo repository = new CustomerRepo();
        private IMapper mapper;
        public CustomersController(IMapper _mapper) => mapper = _mapper;

        [Authorize(Policy = "EmpOnly")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationParams @params, string? name)
        {
            var data = await repository.Customers(@params, name);
            var paginationMetadata = new PaginationMetadata(repository.Customers(name).Result.Count, @params.Page, @params.ItemsPerPage);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(data.Select(mapper.Map<Customer, CusRes>).ToList());
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string? id)
        {
            if (id is null) return BadRequest();
            var customer = await repository.Customer(id);
            return customer is null ? NotFound() : Ok(mapper.Map<CusRes>(customer));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post(CusReq cus)
        {
            if (cus is null) return BadRequest();
            var id = await repository.Save(mapper.Map<Customer>(cus));
            if (id is not null) return Ok(id);
            return Conflict();
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string? id, CusReq req)
        {
            if (id is null) return BadRequest();
            var customer = await repository.Customer(id);
            if (customer is not null) return Ok(await repository.Update(mapper.Map<Customer>(req)));
            return Conflict();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id is null) return BadRequest();
            var customer = await repository.Customer(id);
            if (customer is not null) return Ok(await repository.Delete(customer));
            return Conflict();
        }
    }
}
