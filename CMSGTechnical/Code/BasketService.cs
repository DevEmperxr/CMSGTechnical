using CMSGTechnical.Domain.Models;
using CMSGTechnical.Mediator.Basket;
using CMSGTechnical.Mediator.Dtos;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace CMSGTechnical.Code
{
    public class BasketChangedEventArgs : EventArgs
    {
        public BasketDto Basket { get; set; }
    }

    public class BasketService
    {
        public event EventHandler<BasketChangedEventArgs> OnChange;
        public BasketDto Basket { get; private set; }

        private readonly IJSRuntime _jsRuntime;

        public BasketService(BasketDto basket, IJSRuntime jsRuntime)
        {
            Basket = basket;
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var storedBasket = await _jsRuntime.InvokeAsync<string>("basketStorage.load");
                if (!string.IsNullOrEmpty(storedBasket))
                {
                    var loadedBasket = JsonSerializer.Deserialize<BasketDto>(storedBasket);
                    if (loadedBasket != null && loadedBasket.MenuItems.Any())
                    {
                        Basket.MenuItems = loadedBasket.MenuItems;
                        OnChange?.Invoke(this, new BasketChangedEventArgs() { Basket = Basket });
                    }
                }
            }
            catch (Exception)
            {
                // If localStorage fails, just continue with empty basket
            }
        }

        public async Task Add(MenuItemDto item)
        {
            Basket.MenuItems.Add(item);
            await SaveToLocalStorage();
            OnChange?.Invoke(this, new BasketChangedEventArgs() { Basket = Basket });
        }

        public async Task Remove(MenuItemDto item)
        {
            Basket.MenuItems.Remove(item);
            await SaveToLocalStorage();
            OnChange?.Invoke(this, new BasketChangedEventArgs() { Basket = Basket });
        }

        private async Task SaveToLocalStorage()
        {
            try
            {
                var json = JsonSerializer.Serialize(Basket);
                await _jsRuntime.InvokeVoidAsync("basketStorage.save", json);
            }
            catch (Exception)
            {
                // If localStorage fails, just continue
            }
        }
    }
}
