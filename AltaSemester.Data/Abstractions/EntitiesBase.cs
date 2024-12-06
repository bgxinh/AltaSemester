using AltaSemester.Data.Abstractions.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Abstractions
{
    public abstract class EntitiesBase<T> : IEntitiesBase<T>
    {
        public T Id { get; set; }
    }
}
