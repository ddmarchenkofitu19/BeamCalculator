using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamCalculator.Views.Flyout
{
    public class FlyoutMenuItem
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string IconSource { get; set; }

        public FlyoutMenuItemType ItemType { get; set; }

        public Type TargetType { get; set; }

        public DevicePlatform[] OnPlatforms { get; set; }
    }

    public enum FlyoutMenuItemType
    {
        NavPage,
        DetailPage,
        Popup
    }
}
