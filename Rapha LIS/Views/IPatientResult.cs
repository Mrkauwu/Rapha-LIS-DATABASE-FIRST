using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Views
{
    public interface IPatientResult
    {
        string ResultSearchQuery { get; set; }

        bool EditResult { get; set; }

        event EventHandler ResultSearchRequested;
        event EventHandler ResultActionRequested;
        void BindPatientResult(BindingSource resultBindingSource);
        void Show();
    }
}
