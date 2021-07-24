using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Business
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }
        public string name { get; private set; }

        public Category(string name)
        {
            this.name = name;
        }
        public String getName()
        {
            return name;
        }

        public bool Equals(Category nc)
        {
            return this.name.Equals(nc.name);
        }
    }
}