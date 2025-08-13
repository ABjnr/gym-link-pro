# GymLinkPro

**GymLinkPro** is a web application for managing gym classes, projects, users, registrations, and project resources. Built with ASP.NET Core Razor Pages and Entity Framework Core, it provides a robust platform for gym and project management with role-based access and modern UI patterns.

---

## Features

- **User Management:** Members, Admins, and Trainers with role-based authorization.
- **Gym Classes:** Schedule, edit, and register for gym classes.
- **Class Registration:** Members can register for classes; admins can manage all registrations.
- **Projects:** Create and manage projects, assign members, and manage project links/resources.
- **Project Members:** Assign users to projects with roles (Admin, Member).
- **Project Links:** Manage resources and links related to projects.
- **Cascading Deletes:** Deleting a parent entity (e.g., Project, User, GymClass) will automatically delete related records.
- **Dropdowns for Selection:** User-friendly dropdowns for selecting members, classes, and statuses.
- **Validation:** Client-side and server-side validation using jQuery Validation and Data Annotations.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (or update the connection string for your preferred database)
- Visual Studio 2022 or later (recommended)

---

## Getting Started

1. **Clone the repository:**
    ```sh
    git clone https://github.com/ABjnr/gym-link-pro
    cd GymLinkPro
    ```

2. **Configure the database:**
    - Update the connection string in `appsettings.json` if needed.

3. **Apply database migrations:**
    ```sh
    dotnet ef database update
    ```

4. **Run the application:**
    ```sh
    dotnet run
    ```
    Or use Visual Studio's "Start Debugging" (F5).

5. **Access the app:**
    - Open your browser and go to `https://localhost:5001` (or the port shown in the console).

---

## Project Structure

- `Controllers/` - Razor Pages controllers for views and APIs
- `Models/` - Entity and DTO classes
- `Data/` - Entity Framework Core context and migrations
- `Views/` - Razor Pages views
- `wwwroot/` - Static files (JS, CSS, etc.)

---

## Development Notes

- **Cascading Deletes:** Configured in `ApplicationDbContext` to keep data consistent.
- **Dropdowns:** Member/Class/Status selection uses ViewBag and displays names, not IDs.
- **Status Fields:** Use enums or string lists for consistency (e.g., Pending, Confirmed, Canceled).
- **Role-based Access:** Enforced via `[Authorize]` attributes.
- **Validation:** Uses both client-side (jQuery Validation) and server-side (Data Annotations).

---

## License

This project is licensed under the MIT License. See the [LICENSE](wwwroot/lib/jquery-validation/LICENSE.md) file for details.

---

*For questions or contributions, please open an issue or pull request.*
