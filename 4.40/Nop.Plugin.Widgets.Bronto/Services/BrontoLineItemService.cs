using System.Threading.Tasks;
using Nop.Plugin.Widgets.Bronto.Domain;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Core;
using System;
using Nop.Services.Seo;
using Nop.Services.Media;
using Nop.Services.Common;
using Nop.Core.Domain.Catalog;
using System.Linq;

namespace Nop.Plugin.Widgets.Bronto.Services
{
    public class BrontoLineItemService : IBrontoLineItemService
    {
        private readonly ICategoryService _categoryService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;

        public BrontoLineItemService(
            ICategoryService categoryService,
            IGenericAttributeService genericAttributeService,
            IPictureService pictureService,
            IProductService productService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService
        ) {
            _categoryService = categoryService;
            _genericAttributeService = genericAttributeService;
            _pictureService = pictureService;
            _productService = productService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
        }

        public async Task<IList<BrontoLineItem>> GetBrontoLineItemsAsync(IList<ShoppingCartItem> cart)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));

            var result = new List<BrontoLineItem>();
            foreach (var cartItem in cart)
            {
                result.Add(await GetBrontoLineItemAsync(cartItem.ProductId, cartItem.Quantity));
            }

            return result;
        }

        public async Task<IList<BrontoLineItem>> GetBrontoLineItemsAsync(IList<OrderItem> orderItems)
        {
            if (orderItems == null) throw new ArgumentNullException(nameof(orderItems));

            var result = new List<BrontoLineItem>();
            foreach (var orderItem in orderItems)
            {
                result.Add(await GetBrontoLineItemAsync(orderItem.ProductId, orderItem.Quantity));
            }

            return result;
        }

        private async Task<BrontoLineItem> GetBrontoLineItemAsync(int productId, int quantity)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            var productUrl = (await _storeContext.GetCurrentStoreAsync()).Url +
                             await _urlRecordService.GetSeNameAsync<Product>(product);
            var unitPrice = product.OldPrice != 0.0M ?
                                product.OldPrice :
                                product.Price;

            var brontoDescription = await _genericAttributeService.GetAttributeAsync<string>(
                product,
                "BrontoDescription"
            );
            var shortDescription = brontoDescription ?? product.ShortDescription;

            var productPictures = await _pictureService.GetPicturesByProductIdAsync(product.Id);
            var productPictureUrl = productPictures.Any() ?
                                        await _pictureService.GetPictureUrlAsync(productPictures[0].Id) :
                                        "";
            return new BrontoLineItem()
            {
                Sku = product.Sku,
                Name = product.Name,
                Description = shortDescription,
                Category = await GetProductCategoryBreadcrumbAsync(product),
                Other = "", //not currently in use
                UnitPrice = unitPrice,
                SalePrice = product.Price,
                Quantity = quantity,
                TotalPrice = product.Price * quantity,
                ImageUrl = productPictureUrl,
                ProductUrl = productUrl
            };
        }

        private async Task<string> GetProductCategoryBreadcrumbAsync(Product product)
        {
            var productCategory = (await _categoryService.GetProductCategoriesByProductIdAsync(product.Id)).FirstOrDefault();
            if (productCategory == null) return "";

            var category = await _categoryService.GetCategoryByIdAsync(productCategory.CategoryId);
            var categoryBreadcrumb = await _categoryService.GetFormattedBreadCrumbAsync(category, null, ">");
            return categoryBreadcrumb;
        }
    }
}
