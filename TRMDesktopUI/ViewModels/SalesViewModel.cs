﻿using AutoMapper;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Models;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private BindingList<ProductDisplayModel> _products;
        private int _itemQuantity = 1;
        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();
        IProductEndpoint _productEndpoint;
        IConfigHelper _configHelper;
        ISaleEndpoint _saleEndpoint;
        IMapper _mapper;

        public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper, ISaleEndpoint saleEndpoint,
            IMapper mapper)
        {
            _productEndpoint = productEndpoint;
            _configHelper = configHelper;
            _saleEndpoint = saleEndpoint;
            _mapper = mapper;
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
            var products = _mapper.Map<List<ProductDisplayModel>>(productList); //automapper mapps each one individually and returns as a list.
            Products = new BindingList<ProductDisplayModel>(products);
        }

        public BindingList<ProductDisplayModel> Products
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
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        //In larger project, POST the cart value in the sales db. the API calculates it and then sends it to the 
        //front end.
        public BindingList<CartItemDisplayModel> Cart
        {
            get { return _cart; }
            set {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }
        private ProductDisplayModel _selectedProduct;

        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);

            }
        }

        public string SubTotal
        {
            get
            {
              
                return CalculateSubtotal().ToString("C"); // Passing format provider -C , for currency
            }

        }

        private decimal CalculateSubtotal()
        {
            decimal subTotal = 0;
            foreach (var item in Cart)
            {
                subTotal += (item.Product.RetailPrice * item.QuantityInCart);
            }

            return subTotal;
        }

        private decimal CalculateTax()
        {
            decimal taxAmount = 0;
            decimal taxRate = _configHelper.GetTaxRate()/100;
            taxAmount = Cart.Where(x => x.Product.IsTaxable).Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);

            //foreach (var item in Cart)
            //{
            //    if (item.Product.IsTaxable)
            //    {
            //        taxAmount += (item.Product.RetailPrice * item.QuantityInCart * taxRate);
            //    }
            //}

            return taxAmount;
        }
        //TODO: Solve the rounding issues.
        public string Tax
        {
            get
            {
 
                return CalculateTax().ToString("C"); // Passing format provider -C , for currency
            }

        }

        public string Total
        {
            get
            {
                decimal total = CalculateSubtotal() + CalculateTax();
                return total.ToString("C"); // Passing format provider -C , for currency
            }

        }


        public bool CanAddToCart
        {
            get
            {
                bool output = false;

                //Add Check again
                
                if (ItemQuantity>0 && SelectedProduct?.QuantityinStock >= ItemQuantity)
                {
                    output = true;
                }

                return output;
            }
        }

        public void AddToCart()
        {
            CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct); // comparing same type of object having different value.
            if (existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantity;
                //HACK: Its a Hack . should be replaced in a better way.
                //Cart.Remove(existingItem);
                //Cart.Add(existingItem);
            }
            else
            {
                CartItemDisplayModel item = new CartItemDisplayModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);

            }
            SelectedProduct.QuantityinStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Cart);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);

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

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);


        }


        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                if (Cart.Count > 0)
                {
                    output = true;
                }

                return output;
            }
        }

        public async Task CheckOut()
        {
            SaleModel sale = new SaleModel();

            foreach (var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });
            }
           await _saleEndpoint.PostSale(sale);

        }

    }
}
