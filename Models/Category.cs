﻿using System.Text.Json.Serialization;

namespace AppleStore.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<Product> Product { get; set; }
    }
}