using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiC_.Maui.Models
{
    public class EventAggregator
    {
        public event EventHandler AchievementsUpdated;

        public void NotifyAchievementsUpdated()
        {
            AchievementsUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
