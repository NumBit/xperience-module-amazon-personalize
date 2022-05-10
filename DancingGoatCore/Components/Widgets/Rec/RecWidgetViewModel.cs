using System.Collections.Generic;

using DancingGoat.Models;

namespace DancingGoat.Widgets
{
    /// <summary>
    /// View model for Articles widget.
    /// </summary>
    public class RecWidgetViewModel
    {
        /// <summary>
        /// Latest articles to display.
        /// </summary>
        public IEnumerable<string> Recommendations { get; set; }
    }
}