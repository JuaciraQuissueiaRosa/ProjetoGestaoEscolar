Management System – Service-Oriented Architecture

This project was developed as part of the software development unit, with a focus on implementing a service-oriented architecture using .NET Framework 4.8 for the API, 
WPF with .NET 8 for the client application, and SQL Server hosted on Azure for data persistence.

⸻

Technical Architecture

The solution is structured in three main layers:

	1.	Database (SQL Server - Azure)
Relational model with multiple tables, designed to support all operations of the system.

	2.	API (ASP.NET Web API - .NET Framework 4.8)
RESTful API built using LINQ to SQL, responsible for data exposure and business logic.

	3.	Client Application (WPF - .NET 8)
Desktop interface that consumes the API exclusively for all operations, built with modern WPF features.

⸻

Technologies Used
	•	C#
	•	WPF (.NET 8)
	•	ASP.NET Web API (.NET Framework 4.8)
	•	LINQ to SQL
	•	SQL Server (Azure)
	•	RESTful Services
	•	Azure App Services / SQL Database
	•	Git & GitHub

⸻
Key Features
	•	Full CRUD for all entities
	•	Clean and intuitive graphical user interface
	•	Data validation and error handling
	•	HTTPClient communication between client and API
	•	Proper separation of concerns (Data, Service, Presentation layers)
	•	API and database hosted on Microsoft Azure

⸻

How to Run the Project
	1.	Clone the repository:
git clone https://github.com/JuaciraQuissueiaRosa/ProjetoGestaoEscolar.git

	2.	API:
	•	Requirements: Visual Studio 2022, .NET Framework 4.8
	•	Open the API project and run it locally or access the deployed version.
	•	Live API URL
	3.	Client (WPF):
	•	Requirements: .NET 8 SDK
	•	Open the WPF project in Visual Studio or compatible IDE.
	•	Run the project; it will connect automatically to the API hosted on Azure : (https://schoolapi.azurewebsites.net)

⸻

## 📹 Video Demonstration

[![Watch the demo](https://drive.google.com/file/d/1GbhuXt0oX2ObPKf28s5_gNfQOcCj4Y0I/view?usp=sharing))


⸻

Author & Version

Name: Juacira Rosa
Date: 27/ May/ 2025
Version: 1.0.0

⸻

Final Notes

This project showcases my ability to design and implement a real-world, service-oriented system following object-oriented programming principles. 
It is part of my professional portfolio and reflects the skills I am prepared to apply in a Junior Developer position.
