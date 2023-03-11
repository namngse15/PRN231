﻿using BusinessObject.Models;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.RepoImpl
{
    public class ProductRepo : IProductRepo
    {
        public async Task<List<Product>> Products(PaginationParams @params, FilterParams @filter) => await ProductDAO.GetProducts(@params, @filter);

        public async Task<List<Product>> Products(FilterParams @filter) => await ProductDAO.GetAll(@filter);

        public async Task<Product> Product(int? id) => await ProductDAO.GetProductById(id);

        public async Task<bool> Save(Product product) => await ProductDAO.SaveProduct(product);

        public async Task<bool> Save(List<Product> products) => await ProductDAO.SaveProducts(products);

        public async Task<bool> Update(Product product) => await ProductDAO.UpdateProduct(product);

        public async Task<bool> Delete(Product product) => await ProductDAO.DeleteProduct(product);

        public List<Product> GetTopProductsSale(int amount) => ProductDAO.GetTopProductsSale(amount);
    }
}
