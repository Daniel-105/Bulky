using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;

namespace BulkyWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class ProductController : Controller
    {

        // declaring the variable that holds the connection to the database
        // allowing me to later use this variable intead of applying method on the class AplicationDbContext
        private readonly IUnitOfWork _unitOfWork;
        // injecting this built in function of .net core.
        private readonly IWebHostEnvironment _webHostEnviroment;
        // Constructor (it has the same name as the controller class)
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnviroment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnviroment = webHostEnviroment;
        }
        public IActionResult Index()
        {
            List<Product> objCategoryList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(objCategoryList);
        }

        public IActionResult Upsert(int? id)
        {

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            // When there is no id in the URL
            if (id == null || id == 0)
            {
                // for Create
                return View(productVM);
            }
            //When there is an Id in the URL
            else
            {
                // for update
                productVM.Product = _unitOfWork.Product.Get(x => x.Id == id);
                return View(productVM);
            }
        }

        [HttpPost] // Defining that this action is of the type POST
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                // associating the path for the wwwroot (/) with the variable wwwRootPath
                string wwwRootPath = _webHostEnviroment.WebRootPath;
                if (file != null)
                {
                    //Creating a new Guid and conserting it to a string 
                    // Then concatnate that with the extention of the file (.jpeg, .pdf, etc...)
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    // Check if there is an image loaded
                    if (!string.IsNullOrEmpty(productVM.Product.ImageURL))
                    {
                        //Delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageURL.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageURL = @"\images\product\" + fileName;
                }

                if (productVM.Product.Id == 0)
                {
                    // Stages the post
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                // Push the post to the database
                _unitOfWork.Save();
                TempData["success"] = "Category Updated Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }

        //public IActionResult Delete(int? id)
        //{
        //    if (id == 0 || id == null)
        //    {
        //        return NotFound();
        //    }
        //    Product productFromDb = _unitOfWork.Product.Get(x => x.Id == id);
        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    // Returns the file with the name Create.cshtml in the Views_Category folder
        //    return View(productFromDb);
        //}
        //[HttpPost] // Defining that this action is of the type POST
        //public IActionResult Delete(Product? obj)
        //{
        //    if (obj.Id == 0 || obj.Id == null)
        //    {
        //        NotFound();
        //    }
        //        _unitOfWork.Product.Remove(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Category Updated Successfully";
        //        return RedirectToAction("Index");
        //    return View();
        //}

        #region API CALLs
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            //Delete the old image
            var oldImagePath = Path.Combine(_webHostEnviroment.WebRootPath, productToBeDeleted.ImageURL.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfull" });
        }
    }

    #endregion
}
//}
