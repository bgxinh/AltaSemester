using AltaSemester.Data.Abstractions.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Abstractions
{
    public abstract class EntityAuditBase<T> : EntitiesBase<T>, ISoftDelete
    {
        public bool IsDelete { get; set; }
        public DateTimeOffset DeleteDate { get; set; }
    }
}
