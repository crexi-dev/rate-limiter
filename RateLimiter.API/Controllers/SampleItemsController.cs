using Microsoft.AspNetCore.Mvc;
using RateLimiter.API.Models;

namespace RateLimiter.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SampleItemsController : ControllerBase
{
    private static readonly Dictionary<int, SampleItemModel> _items = new();
    private static int _currentId = 0;

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_items.Values);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        if (_items.TryGetValue(id, out var item))
        {
            return Ok(item);
        }

        return NotFound();
    }

    [HttpPost]
    public IActionResult Create(SampleItemModel item)
    {
        item.Id = ++_currentId;
        _items[item.Id] = item;
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, SampleItemModel item)
    {
        if (_items.ContainsKey(id))
        {
            item.Id = id;
            _items[id] = item;
            return Ok(item);
        }

        return NotFound();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (_items.Remove(id, out var item))
        {
            return Ok(item);
        }

        return NotFound();
    }
}