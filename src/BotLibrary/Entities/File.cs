using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLibrary.Entities
{
    public class File: BaseEntity
    {
        FileInfo File { get; set; }
        string Description { get; set; }
    }
}
