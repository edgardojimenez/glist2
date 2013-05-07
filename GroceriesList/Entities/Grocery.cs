using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroceriesList.Entities {

    public partial class Grocery {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual Product Product { get; set; }
    }
}