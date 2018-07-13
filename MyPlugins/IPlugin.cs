using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPlugins
{
    public interface IPlugin
    {
        string Name { get; set; }
        Task<string> Do(string info);
    }
}
