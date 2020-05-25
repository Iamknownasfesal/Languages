namespace DinoBot.Dal.Models.Items
{
    public class Item : Entity
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string RealName { get; set; }
        public int Price { get; set; }
    }
}
