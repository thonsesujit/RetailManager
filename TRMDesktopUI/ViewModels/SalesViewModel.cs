using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private BindingList<ProductModel> _products;
        private int _itemQuantity;
        private BindingList<ProductModel> _cart;
        IProductEndpoint _productEndpoint;

        public SalesViewModel(IProductEndpoint productEndpoint)
        {
            _productEndpoint = productEndpoint;
       
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }

        //to avoid async in constructor. Because constructor is supposed to be fast. 
        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAll();
            Products = new BindingList<ProductModel>(productList);
        }

        public BindingList<ProductModel> Products
        {
            get { return _products; }
            set {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }


        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
            }
        }


        public BindingList<ProductModel> Cart
        {
            get { return _cart; }
            set {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

  

        public string SubTotal
        {
            get {
                //replace with calculations
                return "$0.00";
            }

        }

        public string Tax
        {
            get
            {
                //replace with calculations
                return "$0.00";
            }

        }

        public string Total
        {
            get
            {
                //replace with calculations
                return "$0.00";
            }

        }


        public bool CanAddToCart
        {
            get
            {
                bool output = false;
              
                //Add Checks here.

                return output;
            }
        }

        public void AddToCart()
        {


        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                //Add Checks here.

                return output;
            }
        }

        public void RemoveFromCart()
        {


        }


        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                //Add Checks here.If something in the cart.

                return output;
            }
        }

        public void CheckOut()
        {


        }

    }
}
