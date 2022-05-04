namespace Catalog.Entities
{
  public record Item
  {
    /*
    init:
      Item item = new() { Id = Guid.NewGuid() };  creation expression
    NOT:
      item.Id = Guid.NewGuid();
    */
    public Guid Id { get; init; } // init-only allow you use a creation expression
    public string Name { get; init; }
    public decimal Price { get; init; }
    public DateTimeOffset CreatedDate { get; init; }
  } 
}