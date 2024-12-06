using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Abstractions.Interface
{
    public interface IEntitiesBase<T>
    {
        T Id { get; set; }
    }
}
