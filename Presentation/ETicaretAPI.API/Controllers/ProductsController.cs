﻿using ETicaretAPI.Application.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly private IProductWriteRepository _productWriteRepository;
        readonly private IProductReadRepository _productReadRepository;

        public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
        }
        [HttpGet]
        public async Task Get()
        {
           await _productWriteRepository.AddRangeAsync(new()
            {
                new() {Id=Guid.NewGuid(),Name="Product1",Price=120,CreatedDate=DateTime.UtcNow,Stock=15},
                 new() {Id=Guid.NewGuid(),Name="Product2",Price=111,CreatedDate=DateTime.UtcNow,Stock=13},
                  new() {Id=Guid.NewGuid(),Name="Product3",Price=113,CreatedDate=DateTime.UtcNow,Stock=14},
            });
           await _productWriteRepository.SaveChanges();
        }
    }
}