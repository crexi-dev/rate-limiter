using System.ComponentModel.DataAnnotations;

namespace DataBaseLayer.Models
{
	public class Request
	{
		[Key]
		public ulong Id { get; set; }
		public string ClientId { get; set; }
		[Required]
		public DateTimeOffset RequestDatetime { get; set; }

	}
}