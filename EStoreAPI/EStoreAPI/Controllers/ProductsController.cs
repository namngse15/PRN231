﻿using AutoMapper;
using BusinessObject.Models;
using BusinessObject.Req;
using BusinessObject.Res;
using ClosedXML.Excel;
using DataAccess;
using DataAccess.RepoImpl;
using EStoreAPI.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IProductRepo repository = new ProductRepo();
        private IMapper mapper;
        public ProductsController(IMapper _mapper) => mapper = _mapper;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationParams @params, [FromQuery] FilterParams @filter)
        {
            var data = await repository.Products(@params, @filter);
            var paginationMetadata = new PaginationMetadata(repository.Products(@filter).Result.Count, @params.Page, @params.ItemsPerPage);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(data.Select(mapper.Map<Product, ProductRes>).ToList());
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sale")]
        public IActionResult Get(int amount)
            => Ok(repository.GetTopProductsSale(amount).Select(mapper.Map<Product, ProductRes>).ToList());

        [AllowAnonymous]
        [HttpGet]
        [Route("totalProducts")]
        public async Task<IActionResult> Get([FromQuery] FilterParams @filter)
        {
            var data = await repository.Products(@filter);
            return Ok(data.Count);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            if (id is null) return BadRequest();
            var product = await repository.Product(id);
            return product is null ? NotFound() : Ok(mapper.Map<ProductRes>(product));
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpPost]
        public async Task<IActionResult> Post(ProductReq product)
        {
            if (product is null) return BadRequest();
            var isSave = await repository.Save(mapper.Map<Product>(product));
            if (isSave) return Ok(isSave);
            return Conflict();
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpPost]
        [Route("import")]
        public async Task<IActionResult> Post(IFormFile? file)
        {
            if (file is not null)
            {
                var isSave = await repository.Save(await ExcelConfig.import(file));
                if (isSave) return Ok(isSave);
            }
            return BadRequest();
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpPost]
        [Route("export")]
        public async Task<IActionResult> Post(FilterParams @filter)
        {
            var products = await repository.Products(@filter);
            using (var workbook = new XLWorkbook())
            {
                ExcelConfig.export(products, workbook);
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(
                       content,
                       "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       "products.xlsx");
                }
            }
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int? id, ProductReq req)
        {
            if (id is null) return BadRequest();
            var product = await repository.Product(id);
            if (product is not null) return Ok(await repository.Update(mapper.Map<Product>(req)));
            return Conflict();
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();
            var product = await repository.Product(id);
            if (product is not null) return Ok(await repository.Delete(product));
            return Conflict();
        }
    }
}
