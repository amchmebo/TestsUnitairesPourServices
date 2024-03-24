using System;
namespace TestsUnitairesPourServices.Models
{
	public class House
	{
		public House()
		{
		}

		public int Id { get; set; }
		public string Address { get; set; }
		public string OwnerName { get; set; }
		public virtual List<Cat> Cats { get; set; }
	}
}

