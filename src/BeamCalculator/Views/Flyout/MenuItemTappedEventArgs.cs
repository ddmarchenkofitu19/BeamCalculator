using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamCalculator.Views.Flyout
{
    public class MenuItemTappedEventArgs
    {
        public MenuItemTappedEventArgs(FlyoutMenuItem item) { ItemTapped = item; }
        public FlyoutMenuItem ItemTapped { get; }
    }
}
