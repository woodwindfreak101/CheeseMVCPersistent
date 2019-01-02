using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseMVC.Data;
using CheeseMVC.Models;
using CheeseMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {
        private readonly CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            List<Menu> menus = context.Menus.ToList();
            return View(menus);
        }

        public IActionResult Add()
        {
            AddMenuViewModel addMenuViewModel = new AddMenuViewModel();
            return View(addMenuViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel addMenuViewModel)
        {
            if (ModelState.IsValid)
            {
                // Add the new cheese to my existing cheeses
                Menu newMenu = new Menu
                {
                    Name = addMenuViewModel.Name,
                };

                context.Menus.Add(newMenu);
                context.SaveChanges();

                return Redirect("/Menu/ViewMenu/" + newMenu.ID);
            }


            return View(addMenuViewModel);
        }

        [HttpGet]
        public IActionResult ViewMenu(ViewMenuViewModel viewMenuViewModel, int id)
        {
            List<CheeseMenu> items = context.CheeseMenus
                .Include(item => item.Cheese)
                .Where(cm => cm.MenuID == id)
                .ToList();

            Menu menu = context.Menus.Where(m => m.ID == id).Single();

            viewMenuViewModel.Items = items;
            viewMenuViewModel.Menu = menu;

            return View(viewMenuViewModel);
        }

        [HttpGet]
        public IActionResult AddItem(int id)
        {
            Menu menu = context.Menus.Single(c => c.ID == id);

            IList<Cheese> cheeses = context.Cheeses.ToList();

            AddMenuItemViewModel addMenuItemViewModel = 
                new AddMenuItemViewModel(cheeses, menu);

            return View(addMenuItemViewModel);
        }

        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel addMenuItemViewModel)
        { 
            IList<CheeseMenu> existingItems = context.CheeseMenus
                .Where(cm => cm.CheeseID == addMenuItemViewModel.CheeseID)
                .Where(cm => cm.MenuID == addMenuItemViewModel.MenuID).ToList();

            if (ModelState.IsValid && existingItems.Count == 0)
            {
                CheeseMenu newCheeseMenu = new CheeseMenu
                {
                    CheeseID = addMenuItemViewModel.CheeseID,
                    MenuID = addMenuItemViewModel.MenuID
                };

                context.CheeseMenus.Add(newCheeseMenu);
                context.SaveChanges();
            }
             return Redirect("/Menu/ViewMenu/" + addMenuItemViewModel.MenuID);
        }
    }
}