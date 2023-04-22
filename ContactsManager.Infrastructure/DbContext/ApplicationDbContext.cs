using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
	public class ApplicationDbContext : DbContext
	{
		public virtual DbSet<Person> Persons { set; get; }
		public virtual DbSet<Country> Countries { set; get; }

		public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Table Builder from Entities
			modelBuilder.Entity<Country>().ToTable("Countries");
			modelBuilder.Entity<Person>().ToTable("Persons");

			// Add seed (initial) data for Countries
			string countriesJSON = System.IO.File.ReadAllText(@"countries.json");
			List<Country>? countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJSON);

			if (countries != null)
			{
				foreach (Country country in countries)
				{
					modelBuilder.Entity<Country>().HasData(country);
				}
			}

			// Add seed (initial) data for Person
			string personsJSON = System.IO.File.ReadAllText(@"persons.json");
			List<Person>? persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJSON);

			if (persons != null)
			{
				foreach (Person person in persons)
				{
					modelBuilder.Entity<Person>().HasData(person);
				}
			}

			// Fluent API
			modelBuilder.Entity<Person>().Property(temp => temp.TIN).HasColumnName("TaxIdentificationNumber").HasColumnType("varchar(8)").HasDefaultValue("ABC12345");

			//modelBuilder.Entity<Person>().HasIndex(temp => temp.TIN).IsUnique();

			// check Constraint
			modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber]) = 8");

			// 1 to Many Table Relations
			//modelBuilder.Entity<Person>(entity =>
			//{
			//	entity.HasOne<Country>(c => c.Country).WithMany(p => p.Persons).HasForeignKey(p => p.CountryId);
			//});
		}

		public List<Person> sp_GetAllPersons()
		{
			return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
		}

		public int sp_InsertPerson(Person person)
		{
			SqlParameter[] parametersSp = new SqlParameter[]
			{
				new SqlParameter("@PersonId", person.PersonId),
				new SqlParameter("@PersonName", person.PersonName),
				new SqlParameter("@Email", person.Email),
				new SqlParameter("@DateOfBirth", person.DateOfBirth),
				new SqlParameter("@Gender", person.Gender),
				new SqlParameter("@CountryId", person.CountryId),
				new SqlParameter("@Address", person.Address),
				new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters),
				new SqlParameter("@TIN", "TEMP"),
			};

			return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonId, @PersonName, @Email, @DateOfBirth,@Gender, @CountryId, @Address, @ReceiveNewsLetters", parametersSp);
		}
	}
}
