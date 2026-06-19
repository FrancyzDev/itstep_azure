using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemsController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Item>>> GetAll()
    {
        var items = await _itemService.GetAllItemsAsync();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Item>> GetById(int id)
    {
        var item = await _itemService.GetItemByIdAsync(id);
        if (item == null)
            return NotFound($"Елемент з ID {id} не знайдено");
        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Item>> Create([FromForm] ItemCreateDto itemCreateDto)
    {
        if (string.IsNullOrWhiteSpace(itemCreateDto.Name))
            return BadRequest("Назва товару обов'язкова");

        if (itemCreateDto.Price <= 0)
            return BadRequest("Ціна має бути більше 0");

        try
        {
            var item = new Item
            {
                Name = itemCreateDto.Name,
                Price = itemCreateDto.Price
            };

            var createdItem = await _itemService.CreateItemAsync(item, itemCreateDto.ImageFile);
            return CreatedAtAction(nameof(GetById), new { id = createdItem.Id }, createdItem);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _itemService.DeleteItemAsync(id);
        if (!deleted)
            return NotFound($"Елемент з ID {id} не знайдено");

        return NoContent();
    }
}