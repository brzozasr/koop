using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Koop.Extensions;
using Koop.models;
using Koop.Models;
using Koop.Models.Repositories;
using Koop.Models.RepositoryModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using NetTopologySuite.Operation.Union;

namespace Koop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private IGenericUnitOfWork _uow;

        public TestController(IGenericUnitOfWork genericUnitOfWork)
        {
            _uow = genericUnitOfWork;
        }
        
        [AllowAnonymous]
        [HttpGet("index")]
        public IActionResult Index()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }

        [AllowAnonymous]
        [HttpGet("NoAuth")]
        public IActionResult NoAuth()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }
        
        [Authorize]
        [HttpGet("Auth")]
        public IActionResult Auth()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }
        
        [Authorize(Policy = "Szymek")]
        [HttpGet("AuthUserName")]
        public IActionResult AuthUserName()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }
        
        [Authorize(Roles = "Koty")]
        [HttpGet("AuthRole")]
        public IActionResult AuthRole()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }

        [HttpGet("products")]
        public IActionResult Products(string orderBy = "name", int start = 1, int count = 10, string orderDir = "asc")
        {
            orderBy = orderBy.ToLower();
            Expression<Func<ProductsShop, object>> order = orderBy switch
            {
                "name" => p => p.ProductName,
                "price" => p => p.Price,
                "blocked" => p => p.Blocked,
                "available" => p => p.Available,
                "unit" => p => p.Unit,
                "amountmax" => p => p.AmountMax,
                "supplierabbr" => p => p.SupplierAbbr,
                _ => p => p.ProductName
            };

            OrderDirection direction = orderDir switch
            {
                "asc" => OrderDirection.Asc,
                "desc" => OrderDirection.Desc,
                _ => OrderDirection.Asc
            };

            return Ok(_uow.ShopRepository().GetProductsShop(order, start, count, direction));
        }

        [HttpGet("product")]
        public IActionResult Product(Guid productId)
        {
            try
            {
                return Ok(_uow.ShopRepository().GetProductById(productId));
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpPost("product/update")]
        public IActionResult UpdateProduct(Product product)
        {
            _uow.ShopRepository().UpdateProduct(product);
            
            try
            {
                _uow.SaveChanges();
                return Ok(new {Message = "Table Product updated successfully."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpDelete("product/remove")]
        public IActionResult RemoveProduct(IEnumerable<Product> products)
        {
            _uow.ShopRepository().RemoveProduct(products);
            
            try
            {
                _uow.SaveChanges();
                return Ok(new {Message = "Entries of Product were removed successfully."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }
        
        [HttpGet("product/categories")]
        public IActionResult GetProductCatgeories(Guid productId)
        {
            try
            {
                return Ok(_uow.ShopRepository().GetProductCategories(productId));
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }
        
        [HttpPost("product/categories/update")]
        public IActionResult UpdateCategories(IEnumerable<ProductCategoriesCombo> productCategoriesCombos)
        {
            _uow.ShopRepository().UpdateProductCategories(productCategoriesCombos);

            try
            {
                _uow.SaveChanges();
                return Ok(new {Message = "Table ProductCategories updated successfully."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }
        
        [HttpDelete("product/categories/remove")]
        public IActionResult RemoveCategories(IEnumerable<ProductCategoriesCombo> productCategoriesCombos)
        {
            _uow.ShopRepository().RemoveProductCategories(productCategoriesCombos);

            try
            {
                _uow.SaveChanges();
                return Ok(new {Message = "Entries of ProductCategories were removed successfully."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }
        
        [HttpGet("product/availQuantities")]
        public IActionResult GetProductAvailQuantities(Guid productId)
        {
            try
            {
                return Ok(_uow.ShopRepository().GetAvailableQuantities(productId));
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpPost("product/availQuantities/update")]
        public IActionResult UpdateAvailQuantities(IEnumerable<AvailableQuantity> availableQuantities)
        {
            _uow.ShopRepository().UpdateAvailableQuantities(availableQuantities);

            try
            {
                _uow.SaveChanges();
                return Ok(new {Message = "Table AvailableQuantities updated successfully."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpDelete("product/availQuantities/remove")]
        public IActionResult RemoveAvailQuantities(IEnumerable<AvailableQuantity> availableQuantities)
        {
            _uow.ShopRepository().RemoveAvailableQuantities(availableQuantities);
            
            try
            {
                _uow.SaveChanges();
                return Ok(new {Message = "Entries of AvailableQuantities were removed successfully."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpGet("allUnits")]
        public IActionResult GetAllUnits()
        {
            try
            {
                return Ok(_uow.Repository<Unit>().GetAll());
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpPost("units/update")]
        public IActionResult UnitsUpdate(IEnumerable<Unit> units)
        {
            _uow.ShopRepository().UpdateUnits(units);
            
            try
            {
                _uow.SaveChanges();
                return Ok(new {Message = "Entries of Unit were updated successfully."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpDelete("units/remove")]
        public IActionResult RemoveUnits(IEnumerable<Unit> units)
        {
            _uow.ShopRepository().RemoveUnits(units);

            try
            {
                _uow.SaveChanges();
                return Ok(new {Message = "Entries of Unit were removed successfully."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpGet("product/{productId}/unit")]
        public IActionResult GetProductUnit(Guid productId)
        {
            try
            {
                return Ok(_uow.Repository<Product>().GetAll().Where(p => p.ProductId == productId).Select(p => p.Unit));
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            try
            {
                return Ok(_uow.Repository<Category>().GetAll());
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpPost("categories/update")]
        public IActionResult UpdateCategories(IEnumerable<Category> categories)
        {
            _uow.ShopRepository().UpdateCategories(categories);
            
            try
            {
                _uow.SaveChanges();
                return Ok(new {Message = "Table Categories updated successfully."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpDelete("categories/remove")]
        public IActionResult RemoveCategories(IEnumerable<Category> categories)
        {
            _uow.ShopRepository().RemoveCategories(categories);
            
            try
            {
                _uow.SaveChanges();
                return Ok(new {Message = "Entries of Categories were removed successfully."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [Authorize]
        [HttpGet("order/make/")]
        public IActionResult MakeOrder(Guid productId, int quantity)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is not null)
            {
                var result = _uow.ShopRepository().MakeOrder(productId, Guid.Parse(userId), quantity);
                
                try
                {
                    _uow.SaveChanges();
                    return Ok(new {result.Message});
                }
                catch (Exception e)
                {
                    return Problem(e.Message, null, 500);
                }
            }
            
            return Problem("Your identity could not be verified.", null, 500);
        }

        [HttpGet("user/{coopId}/order/{orderId}")]
        public IActionResult CoopOrder(Guid coopId, Guid orderId)
        {
            try
            {
                return Ok(_uow.ShopRepository().GetCooperatorOrders(coopId, orderId));
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpPost("orderedItem/{orderedItemId}/setQuantity/{quantity}")]
        public IActionResult UpdateUserOrderQuantity(Guid orderedItemId, int quantity)
        {
            ShopRepositoryResponse response = _uow.ShopRepository().UpdateUserOrderQuantity(orderedItemId, quantity);
            
            if (response.ErrCode == 200)
            {
                try
                {
                    _uow.SaveChanges();
                    return Ok(response);
                }
                catch (Exception e)
                {
                    return Problem(e.Message, null, 500);
                }
            }

            return Problem(response.Message, null, response.ErrCode);
        }

        [HttpPost("orderedItem/{orderedItemId}/remove")]
        public IActionResult RemoveUserOrder(Guid orderedItemId)
        {
            ShopRepositoryReturn response = _uow.ShopRepository().RemoveUserOrder(orderedItemId);
            
            try
            {
                _uow.SaveChanges();
                return Ok(response.ToObject());
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpGet("supplier/{abbr}")]
        public IActionResult Supplier(string abbr)
        {
            return Ok(_uow.ShopRepository().GetSupplier(abbr));
        }

        [HttpPost("user/{userId}/order/{orderId}/setStatus/{statusId}")]
        public IActionResult UpdateUserOrderStatus(Guid orderId, Guid userId, Guid statusId)
        {
            _uow.ShopRepository().UpdateUserOrderStatus(orderId, userId, statusId);
            
            try
            {
                _uow.SaveChanges();
                return Ok(new {Message = "Order status updated successfully."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        
        [HttpGet("supplier/{abbr}/edit")]
        public IActionResult EditSupplier(string abbr)
        {
            return Ok(_uow.ShopRepository().GetSupplier(abbr));
        }
        
        //TODO EWA: refactor!

        // [HttpGet("allsuppliers")]
        // public IActionResult AllSuppliers()
        // {
        //     return Ok(_uow.Repository<Supplier>().GetAll());
        // }
        
        [HttpGet("cooperator/{firstname}+{lastname}/history")]
        public IActionResult UserOrdersHistoryView(string firstName, string lastName)
        {
            return Ok(_uow.ShopRepository().GetUserOrders(firstName, lastName));
        }
        
        [HttpGet("order/baskets")]
        public IActionResult BasketName()
        {
            return Ok(_uow.ShopRepository().GetBaskets());
        }
        
        [HttpGet("bigorders")]
        public IActionResult BigOrders()
        {
            return Ok(_uow.Repository<Order>().GetAll());
        }
    }
}