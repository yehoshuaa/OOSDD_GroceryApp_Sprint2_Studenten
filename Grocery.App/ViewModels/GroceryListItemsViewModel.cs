using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.App.Views;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    [QueryProperty(nameof(GroceryList), nameof(GroceryList))]
    public partial class GroceryListItemsViewModel : BaseViewModel
    {
        private readonly IGroceryListItemsService _groceryListItemsService;
        private readonly IProductService _productService;
        public ObservableCollection<GroceryListItem> MyGroceryListItems { get; set; } = [];
        public ObservableCollection<Product> AvailableProducts { get; set; } = [];

        [ObservableProperty]
        GroceryList groceryList = new(0, "None", DateOnly.MinValue, "", 0);

        public GroceryListItemsViewModel(IGroceryListItemsService groceryListItemsService, IProductService productService)
        {
            _groceryListItemsService = groceryListItemsService;
            _productService = productService;
            Load(groceryList.Id);
        }

        private void Load(int id)
        {
            MyGroceryListItems.Clear();
            foreach (var item in _groceryListItemsService.GetAllOnGroceryListId(id)) MyGroceryListItems.Add(item);
            GetAvailableProducts();
        }

        private void GetAvailableProducts()
        {
            // leeg de rechterlijst
            AvailableProducts.Clear();

            // haal alle producten op
            var allProducts = _productService.GetAll();

            // pak de ProductId's die al op de boodschappenlijst staan
            var productIdsOnList = MyGroceryListItems
                .Select(i => i.ProductId)
                .ToHashSet();

            // filter: voorraad > 0 én nog niet op de lijst
            foreach (var p in allProducts)
            {
                if (p.Stock > 0 && !productIdsOnList.Contains(p.Id))
                {
                    AvailableProducts.Add(p);
                }
            }
        }

        partial void OnGroceryListChanged(GroceryList value)
        {
            Load(value.Id);
        }

        [RelayCommand]
        public async Task ChangeColor()
        {
            Dictionary<string, object> paramater = new() { { nameof(GroceryList), GroceryList } };
            await Shell.Current.GoToAsync($"{nameof(ChangeColorView)}?Name={GroceryList.Name}", true, paramater);
        }
        [RelayCommand]
        public void AddProduct(Product product)
        {

                // Controleer of het product geldig is
                if (product == null || product.Id <= 0)
                    return;

                // Maak een nieuw GroceryListItem
                var item = new GroceryListItem(
                    id: 0,
                    groceryListId: GroceryList.Id,
                    productId: product.Id,
                    amount: 1
                );

                // Voeg toe via de service
                _groceryListItemsService.Add(item);

                // Voorraad verlagen en opslaan
                product.Stock -= 1;
                if (product.Stock < 0) product.Stock = 0;
                _productService.Update(product);

                // Beschikbare producten updaten
                if (product.Stock == 0)
                    AvailableProducts.Remove(product);

                // Boodschappenlijst vernieuwen
                OnGroceryListChanged(GroceryList);
        }
    }
}
