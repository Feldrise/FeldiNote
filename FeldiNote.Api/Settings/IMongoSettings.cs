using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeldiNote.Api.Settings
{
    public interface IMongoSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
