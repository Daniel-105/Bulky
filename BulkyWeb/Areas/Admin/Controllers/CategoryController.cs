using Bulky.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Bulky.Models;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Bulky.Utility;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        // declaring the variable that holds the connection to the database
        // allowing me to later use this variable intead of applying method on the class AplicationDbContext
        private readonly IUnitOfWork _unitOfWork;
        // Constructor (it has the same name as the controller class)
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        // Defaul is of the type GET
        public IActionResult Create()
        {
            // Returns the file with the name Create.cshtml in the Views_Category folder
            return View();
        }

        [HttpPost] // Defining that this action is of the type POST
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Display Order cannot match the Catelory name");
            }
            if (ModelState.IsValid)
            {
                // Stages the post
                _unitOfWork.Category.Add(obj);
                // Push the post to the database
                _unitOfWork.Save();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            Category categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            // Returns the file with the name Create.cshtml in the Views_Category folder
            return View(categoryFromDb);
        }
        [HttpPost] // Defining that this action is of the type POST
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                // Stages the post
                _unitOfWork.Category.Update(obj);
                // Push the post to the database
                _unitOfWork.Save();
                TempData["success"] = "Category Updated Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            Category categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            // Returns the file with the name Create.cshtml in the Views_Category folder
            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")] // Defining that this action is of the type POST
        // Also defining that the ActionName is "Delete" even though we gave it a different name
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            // Stages the post
            _unitOfWork.Category.Remove(obj);
            // Push the post to the database
            _unitOfWork.Save();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
