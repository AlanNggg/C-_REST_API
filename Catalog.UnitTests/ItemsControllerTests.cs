using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Api.Controllers;
using Catalog.Api.Dtos;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Catalog.UnitTests;

public class ItemsControllerTests
{
  private readonly Mock<IItemsRepository> repositoryStub = new();
  private readonly Mock<ILogger<ItemsController>> loggerStub = new();
  private readonly Random rand = new();
  [Fact]
  public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
  {
    // RUN ALL TEST : dotnet test

    // Arrange
    // whenever controller invokes GetItemAsync with any Guid, have to return null
    repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
        .ReturnsAsync((Item)null);

    var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

    // Act
    var result = await controller.GetItemAsync(Guid.NewGuid());

    // Assert
    result.Result.Should().BeOfType<NotFoundResult>();
    // Assert.IsType<NotFoundResult>(result.Result);
  }

  [Fact]
  public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
  {
    // Arrange
    var expectedItem = CreateRandomItem();

    // whenever controller invokes GetItemAsync with any Guid, have to return null
    repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
        .ReturnsAsync(expectedItem);


    var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

    // Act
    var result = await controller.GetItemAsync(Guid.NewGuid());

    // Assert
    result.Value.Should().BeEquivalentTo(expectedItem);
    // result.Value.Should().BeEquivalentTo( // compare the value of property
    //     expectedItem,
    //     options => options.ComparingByMembers<Item>() // don't compare dto directly to the item, just focus on the properties each of them have
    // );


    // Assert.IsType<ItemDto>(result.Value);
    // var dto = (result as ActionResult<ItemDto>).Value;
    // Assert.Equal(expectedItem.Id, dto.Id);
    // Assert.Equal(expectedItem.Name, dto.Name);
  }

  [Fact]
  public async Task GetItemsAsync_WithExistingItems_ReturnsAllItems()
  {
    // Arrange
    var expectedItem = new[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };

    // whenever controller invokes GetItemAsync with any Guid, have to return null
    repositoryStub.Setup(repo => repo.GetItemsAsync())
        .ReturnsAsync(expectedItem);


    var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

    // Act
    var actualItems = await controller.GetItemsAsync();

    // Assert
    actualItems.Should().BeEquivalentTo(expectedItem);
    // actualItems.Should().BeEquivalentTo( // compare the value of property
    //      expectedItem,
    //      options => options.ComparingByMembers<Item>() // don't compare dto directly to the item, just focus on the properties each of them have
    // );
  }

  [Fact]
  public async Task GetItemsAsync_WithMatchingItems_ReturnsMatchingItems()
  {
    // Arrange
    var allItem = new[]
    {
        new Item{Name="Potion"},
        new Item{Name="Antidote"},
        new Item{Name="Hi-Potion"}
    };

    var nameToMatch = "Potion";

    // whenever controller invokes GetItemAsync with any Guid, have to return null
    repositoryStub.Setup(repo => repo.GetItemsAsync())
        .ReturnsAsync(allItem);


    var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

    // Act
    IEnumerable<ItemDto> foundItems = await controller.GetItemsAsync(nameToMatch);

    // Assert
    foundItems.Should().OnlyContain(item => item.Name == allItem[0].Name || item.Name == allItem[2].Name);

  }


  [Fact]
  public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
  {
    // Arrange
    var itemToCreate = new CreateItemDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), rand.Next(1, 1000));
    // var itemToCreate = new CreateItemDto
    // {
    //   Name = Guid.NewGuid().ToString(),
    //   Price = rand.Next(1, 1000)
    // };

    var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

    // Act
    var result = await controller.CreateItemAsync(itemToCreate);

    // Assert
    var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDto;
    itemToCreate.Should().BeEquivalentTo(
        createdItem,
        options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
    );
    createdItem.Id.Should().NotBeEmpty();
    createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(1000));
  }

  [Fact]
  public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
  {
    // Arrange
    var existingItem = CreateRandomItem();

    // whenever controller invokes GetItemAsync with any Guid, have to return existingItem
    repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
        .ReturnsAsync(existingItem);

    var itemId = existingItem.Id;
    var itemToUpdate = new UpdateItemDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), existingItem.Price + 3);
    // var itemToUpdate = new UpdateItemDto
    // {
    //   Name = Guid.NewGuid().ToString(),
    //   Price = existingItem.Price + 3
    // };

    var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

    // Act
    var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

    // Assert
    result.Should().BeOfType<NoContentResult>();
  }

  [Fact]
  public async Task DeleteItemAsync_WithExistingItem_ReturnsNoContent()
  {
    // Arrange
    var existingItem = CreateRandomItem();

    repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
        .ReturnsAsync(existingItem);

    var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

    // Act
    var result = await controller.DeleteItemAsync(existingItem.Id);

    // Assert
    result.Should().BeOfType<NoContentResult>();
  }

  private Item CreateRandomItem()
  {
    return new()
    {
      Id = Guid.NewGuid(),
      Name = Guid.NewGuid().ToString(),
      Price = rand.Next(1, 1000),
      CreatedDate = DateTimeOffset.UtcNow
    };
  }
}