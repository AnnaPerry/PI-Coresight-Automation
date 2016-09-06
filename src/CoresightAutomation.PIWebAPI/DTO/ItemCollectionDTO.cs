using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoresightAutomation.PIWebAPI.DTO
{
    public class ItemCollectionDTO<T>
    {
        public List<T> Items { get; set; }

        public static implicit operator List<T>(ItemCollectionDTO<T> ic)
        {
            return ic.Items;
        }
    }
}
