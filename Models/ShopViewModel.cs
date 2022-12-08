using Allup.DAL.Entities;

namespace Allup.Models
{
    public class ShopViewModel
    {
        public Category SelectedCategory { get; set; }  
        public List<Category> Categories { get; set; }
    }
}
