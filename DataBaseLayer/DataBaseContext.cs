using DataBaseLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataBaseLayer
{
	public class DataBaseContext : Microsoft.EntityFrameworkCore.DbContext
	{
		public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
		{
		}
		public DbSet<Request> Requests { get; set; }
	}
}
