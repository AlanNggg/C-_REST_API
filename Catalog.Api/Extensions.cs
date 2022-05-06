using Catalog.Api.Dtos;
using Catalog.Api.Entities;

namespace Catalog.Api
{
  public static class Extension
  {
    public static ItemDto AsDto(this Item item)
    {
      // must create via constructor
      return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
    }
  }
}