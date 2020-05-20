using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.Models
{
    public class ProductDisplayModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal RetailPrice { get; set; }
        private int _quantityinStock;

        public int QuantityinStock
        {
            get { return _quantityinStock; }
            set { 
                _quantityinStock = value;
                CallPropertyChanged(nameof(QuantityinStock));
            }
        }

        public bool IsTaxable { get; set; }




        //I need to fire this event whenever the property changes
        public event PropertyChangedEventHandler PropertyChanged;
        public void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }





}
