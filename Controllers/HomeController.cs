using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FormsApp.Models;
using System.Formats.Asn1;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FormsApp.Controllers;

public class HomeController : Controller
{

    public HomeController()
    {

    }
    [HttpGet]
    public IActionResult Index(string searchString,string category)
    {
        var products = Repository.Products ;

        if (!String.IsNullOrEmpty(searchString))
        {
            ViewBag.SearchString = searchString; 
            products = products.Where(p=>p.Name!.ToLower().Contains(searchString.ToLower())).ToList();
        }

        if (!String.IsNullOrEmpty(category) && category != "0")
        {
            products = products.Where(p=>p.CategoryId == int.Parse(category)).ToList();
        }

        // ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name",category);

        var model = new ProductViewModel
        {
            Products = products,
            Categories = Repository.Categories,
            SelectedCategory = category
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product model,IFormFile imageFile)
    {
        var extentions = "";
        if (imageFile !=null)
        {
            var allowedExtentions = new[] {".jpg",".jpeg","png"};
            extentions = Path.GetExtension(imageFile.FileName);
            if (!allowedExtentions.Contains(extentions))
            {
                ModelState.AddModelError("","Geçerli bir resim seçiniz");
            }
        };
        if (ModelState.IsValid)
        {
            var randomfilename  =string.Format($"{Guid.NewGuid().ToString()}{extentions}") ;
            var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/img",randomfilename);

            using (var stream = new FileStream(path,FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            model.Image = randomfilename;
            model.ProductId = Repository.Products.Count + 1;
            Repository.CreateProduct(model);
            return RedirectToAction("Index");
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(model);
       
    }

    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var entity = Repository.Products.FirstOrDefault(p=>p.ProductId == id);
        if (entity == null)
        {
            return NotFound();
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(entity);
    }

    [HttpPost]

    public async Task<IActionResult> Edit(int id ,Product Model , IFormFile? imageFile)
    {
        if (id != Model.ProductId)
        {
            return NotFound();
        }
        if (ModelState.IsValid)
        {
            if (imageFile !=null)
            {
                var extentions = Path.GetExtension(imageFile.FileName);
                var randomfilename  =string.Format($"{Guid.NewGuid().ToString()}{extentions}") ;
                var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/img",randomfilename);

                using (var stream = new FileStream(path,FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                Model.Image = randomfilename;
            };
            Repository.EditProduct(Model);
            return RedirectToAction("Index");
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(Model);
    }

    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var entity = Repository.Products.FirstOrDefault(p=>p.ProductId == id);
        if (entity == null)
        {
            return NotFound();
        }

        return View("DeleteConfirm" , entity);

        

    }

    
}
