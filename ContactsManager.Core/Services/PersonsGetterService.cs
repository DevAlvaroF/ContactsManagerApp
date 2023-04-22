using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using Serilog;
using SerilogTimings;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
	public class PersonsGetterService : IPersonsGetterService
	{
		private readonly IPersonsRepository _personsRepository;
		private readonly ILogger<PersonsGetterService> _logger;
		private readonly IDiagnosticContext _diagnosticContext;
		public PersonsGetterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
		{
			// Add repository abstract layer
			_personsRepository = personsRepository;
			_logger = logger;
			_diagnosticContext = diagnosticContext;
		}


		public async Task<List<PersonResponse>> GetAllPersons()
		{
			List<Person> personResponseList = await _personsRepository.GetAllPersons();

			return personResponseList.Select(person => person.ToPersonResponse()).ToList();


			// OPTIONAL TO USE STORED PROCEDURE
			//return _db.sp_GetAllPersons().Select(person => ConvertPersonToPersonResponse(person)).ToList();
		}

		public async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
		{
			if (personId == null) return null;

			// Fetches 
			Person? finalPerson = await _personsRepository.GetPersonByPersonId(personId.Value);

			if (finalPerson == null) return null;

			return finalPerson.ToPersonResponse();
		}

		public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
		{
			// Logging
			_logger.LogCritical("Warning disabled for Expression<Func<Person,bool>> of the persons repository. LINQ should check it but it could lead to bugs");
			_logger.LogCritical("GetAllPersons of PersonsService");
#pragma warning disable CS8604
#pragma warning disable CS8602
#pragma warning disable CS8629

			List<Person> persons = new List<Person>();
			using (Operation.Time("Time for Filtered Persons from dB"))
			{


				persons = searchBy switch
				{
					nameof(PersonResponse.PersonName) =>
					 await _personsRepository.GetFilteredPersons(temp =>
					 temp.PersonName.Contains(searchString)),

					nameof(PersonResponse.Email) =>
					 await _personsRepository.GetFilteredPersons(temp =>
					 temp.Email.Contains(searchString)),

					nameof(PersonResponse.DateOfBirth) =>
					 await _personsRepository.GetFilteredPersons(temp =>
					 EF.Functions.Like(temp.DateOfBirth.Value.ToString(), $"%{searchString}%")),


					nameof(PersonResponse.Gender) =>
					 await _personsRepository.GetFilteredPersons(temp =>
					 EF.Functions.Like(temp.Gender, $"%{searchString}%")),

					nameof(PersonResponse.CountryId) =>
					 await _personsRepository.GetFilteredPersons(temp =>
					 temp.Country.CountryName.Contains(searchString)),

					nameof(PersonResponse.Address) =>
					await _personsRepository.GetFilteredPersons(temp =>
					temp.Address.Contains(searchString)),

					nameof(PersonResponse.ReceiveNewsLetters) =>
					await _personsRepository.GetFilteredPersons(temp =>
					EF.Functions.Like(temp.ReceiveNewsLetters.ToString(), $"%{searchString}%")),

					_ => await _personsRepository.GetAllPersons()
				};

			};

#pragma warning restore CS8604
#pragma warning restore CS8602
#pragma warning restore CS8629

			// Passing through Serilog to the diagnostic context (seq) 
			_diagnosticContext.Set("Persons", persons);

			// Add Execution timig

			return persons.Select(temp => temp.ToPersonResponse()).ToList();


		}


		public async Task<MemoryStream> GetPersonsCSV()
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream);

			// Configuring CSV Writer
			CsvConfiguration csvConfiguration = new CsvConfiguration(cultureInfo: System.Globalization.CultureInfo.InvariantCulture);

			// Culture Info csvWriter
			CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);

			// Writes Headers based on Class Properties
			csvWriter.WriteField(nameof(PersonResponse.PersonName));
			csvWriter.WriteField(nameof(PersonResponse.Email));
			csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
			csvWriter.WriteField(nameof(PersonResponse.Age));
			csvWriter.WriteField(nameof(PersonResponse.Gender));
			csvWriter.WriteField(nameof(PersonResponse.Country));
			csvWriter.WriteField(nameof(PersonResponse.Address));
			csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));

			// Move to next line
			csvWriter.NextRecord();
			csvWriter.Flush();
			// Load Data
			List<PersonResponse> persons = await GetAllPersons();

			// Data Rows (loops through all)
			foreach (PersonResponse person in persons)
			{
				csvWriter.WriteField(person.PersonName);
				csvWriter.WriteField(person.Email);
				if (person.DateOfBirth.HasValue)
					csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
				else
					csvWriter.WriteField("");
				csvWriter.WriteField(person.Age);
				csvWriter.WriteField(person.Gender);
				csvWriter.WriteField(person.Country);
				csvWriter.WriteField(person.Address);
				csvWriter.WriteField(person.ReceiveNewsLetters);
				csvWriter.NextRecord();
				csvWriter.Flush();
			}

			// Flow Data into memory stream

			// Move memory stream to 0
			memoryStream.Position = 0;

			return memoryStream;
		}

		public async Task<MemoryStream> GetPersonsExcel()
		{
			// Generate Memory Stream
			MemoryStream memoryStream = new MemoryStream();

			// Load Data
			List<PersonResponse> persons = await GetAllPersons();

			// Open Excel package temporarily
			using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
			{
				// Add Sheet to Workbook
				ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");

				//Add Headers based on class properties
				worksheet.Cells["A1"].Value = nameof(PersonResponse.PersonName);
				worksheet.Cells["B1"].Value = nameof(PersonResponse.Email);
				worksheet.Cells["C1"].Value = nameof(PersonResponse.DateOfBirth);
				worksheet.Cells["D1"].Value = nameof(PersonResponse.Age);
				worksheet.Cells["E1"].Value = nameof(PersonResponse.Gender);
				worksheet.Cells["F1"].Value = nameof(PersonResponse.Country);
				worksheet.Cells["G1"].Value = nameof(PersonResponse.Address);
				worksheet.Cells["H1"].Value = nameof(PersonResponse.ReceiveNewsLetters);

				// Format Headers
				using (ExcelRange headerCells = worksheet.Cells["A1:H1"])
				{
					// Pattern Type
					headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

					// BG color
					headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

					// Font
					headerCells.Style.Font.Bold = true;
				}

				// Add cell content
				int row = 2;
				foreach (PersonResponse person in persons)
				{
					worksheet.Cells[$"A{row}"].Value = person.PersonName;
					worksheet.Cells[$"B{row}"].Value = person.Email;
					if (person.DateOfBirth.HasValue)
						worksheet.Cells[$"C{row}"].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
					else
						worksheet.Cells[$"C{row}"].Value = "";
					worksheet.Cells[$"D{row}"].Value = person.Age;
					worksheet.Cells[$"E{row}"].Value = person.Gender;
					worksheet.Cells[$"F{row}"].Value = person.Country;
					worksheet.Cells[$"G{row}"].Value = person.Address;
					worksheet.Cells[$"h{row}"].Value = person.ReceiveNewsLetters;
					row++;
				}
				// Resize columns based on method
				worksheet.Cells[$"A1:H{row}"].AutoFitColumns();

				// Save async
				await excelPackage.SaveAsync();
			}

			memoryStream.Position = 0;

			return memoryStream;

		}
	}
}

