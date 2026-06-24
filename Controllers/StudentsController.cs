using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _context;
    public StudentsController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll() => 
        Ok(await _context.Students.ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create(Student s)
    {
        _context.Students.Add(s);
        await _context.SaveChangesAsync();
        return Ok(s);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var s = await _context.Students.FindAsync(id);
        if (s == null) return NotFound();
        _context.Students.Remove(s);
        await _context.SaveChangesAsync();
        return Ok();
    }
}