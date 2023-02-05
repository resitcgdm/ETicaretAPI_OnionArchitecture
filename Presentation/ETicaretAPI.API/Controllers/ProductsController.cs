using ETicaretAPI.Application.Abstractions.Storage;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.RequestParameters;

using ETicaretAPI.Application.ViewModels.Products;
using ETicaretAPI.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly private IProductWriteRepository _productWriteRepository;
        readonly private IProductReadRepository _productReadRepository;
        readonly IWebHostEnvironment webHostEnvironment;
      
        
        readonly IFileWriteRepository _fileWriteRepository;
        readonly IFileReadRepository _fileReadRepository;
        readonly IProductImageFileWriteRepository _productImageFileWriteRepository;
        readonly IProductImageFileReadRepository _productImageFileReadRepository;
        readonly IInvoiceFileWriteRepository _invoiceFileWriteRepository;
        readonly IInvoiceFileReadRepository _invoiceFileReadRepository;
        readonly IStorageService _storageService;

        public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository, IWebHostEnvironment webHostEnvironment, IFileWriteRepository fileWriteRepository, IFileReadRepository fileReadRepository, IProductImageFileWriteRepository productImageFileWriteRepository, IProductImageFileReadRepository productImageFileReadRepository, IInvoiceFileWriteRepository invoiceFileWriteRepository, IInvoiceFileReadRepository invoiceFileReadRepository, IStorageService storageService)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            this.webHostEnvironment = webHostEnvironment;


            _fileWriteRepository = fileWriteRepository;
            _fileReadRepository = fileReadRepository;
            _productImageFileWriteRepository = productImageFileWriteRepository;
            _productImageFileReadRepository = productImageFileReadRepository;
            _invoiceFileWriteRepository = invoiceFileWriteRepository;
            _invoiceFileReadRepository = invoiceFileReadRepository;
            _storageService = storageService;
        }
        //[HttpPost]
        //public async Task Add()
        //{
        //   await _productWriteRepository.AddRangeAsync(new()
        //    {
        //        new() {Id=Guid.NewGuid(),Name="Product1",Price=120,CreatedDate=DateTime.UtcNow,Stock=15},
        //         new() {Id=Guid.NewGuid(),Name="Product2",Price=111,CreatedDate=DateTime.UtcNow,Stock=13},
        //          new() {Id=Guid.NewGuid(),Name="Product3",Price=113,CreatedDate=DateTime.UtcNow,Stock=14},
        //    });
        //   await _productWriteRepository.SaveChangesAsync();
        //}
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]Pagination pagination)
        {
            await Task.Delay(1000);
          var totalCount = _productReadRepository.GetAll(false).Count();  
          var products=  _productReadRepository.GetAll(false).Skip(pagination.Size * pagination.Page).Take(pagination.Size).Select(p => new
            {
                p.Id,
                p.Name,
                p.Stock,
                p.Price,
                p.CreatedDate,
                p.UpdatedDate

            }).ToList();
            return Ok(new
            {
                products,
                totalCount
            });
        }

          


        [HttpGet("{id}")]

        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _productReadRepository.GetByIdAsync(id,false));  //tracking mekanizmalarını kapatabiliriz(performans için). Çünkü veritabanında bi değişiklik yapmıyoruz.
        }


        [HttpPost]
        public async Task<IActionResult> Post(VM_Create_Product model)
        {   
            if (ModelState.IsValid)
            {

            }
           await _productWriteRepository.AddAsync(new Product
            {
                Name=model.Name,
                Stock=model.Stock,
                Price=model.Price,
            });
            await _productWriteRepository.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpPut]

        public async Task<IActionResult> Put(VM_Update_Product model)
        {
          Product product=await  _productReadRepository.GetByIdAsync(model.Id);
            product.Stock=model.Stock;
            product.Price=model.Price;
            product.Name = model.Name;
           await _productWriteRepository.SaveChangesAsync();
            return Ok();

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _productWriteRepository.RemoveAsync(id);
            await _productWriteRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Upload()
        {
            //var datas = await _fileService.UploadAsync("resource/product-images", Request.Form.Files); //dosya yükleme hatası veriyor
            //await _productImageFileWriteRepository.AddRangeAsync(datas.Select(d => new ProductImageFile()
            //{

            //    FileName = d.fileName,
            //    Path = d.path
            //}).ToList());
            //await _productImageFileWriteRepository.SaveChangesAsync();


            // wwwroot / resource / product - images
            //string uploadPath = Path.Combine(webHostEnvironment.WebRootPath, "resource/product-images");

            //if (!Directory.Exists(uploadPath))
            //{
            //    Directory.CreateDirectory(uploadPath);
            //}


            //Random r = new();


            //foreach (IFormFile file in Request.Form.Files)
            //{
            //    string fullPath = Path.Combine(uploadPath, $"{r.Next()}{Path.GetExtension(file.FileName)}");

            //    using FileStream fileStream = new(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: false);

            //    await file.CopyToAsync(fileStream);
            //    await fileStream.FlushAsync(); //işlemden sonra filestream'i boşaltır.
            //}
          var datas= await _storageService.UploadAsync("resource/files", Request.Form.Files);
            await _productImageFileWriteRepository.AddRangeAsync(datas.Select(d => new ProductImageFile()
            {

                FileName = d.fileName,
                Path = d.pathOrContainerName,
                Storage = _storageService.StorageName
            }).ToList());
            await _productImageFileWriteRepository.SaveChangesAsync();
            return Ok();

        }

    }
}
