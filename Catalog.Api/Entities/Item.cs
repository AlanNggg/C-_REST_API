namespace Catalog.Api.Entities
{
  public class Item
  {
    /*
    init: init-only allow you use a creation expression
      Item item = new() { Id = Guid.NewGuid() };  creation expression
    NOT:
      item.Id = Guid.NewGuid();
    */
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
  }
}