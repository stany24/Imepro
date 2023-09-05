using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryData
{
    public class NewUrlEventArgs : EventArgs
    {
        public Url Url;

        public NewUrlEventArgs(Url url) { Url = url; }
    }
}
