# AwesomeDotNet
A simple web API built with Microsoft's .NET Core framework.
### Objective
1. Build a simple web API with the .NET Core framework.
2. Implement JWT authentication.
3. Learn LINQ method syntax.
4. Use EF Core and a MySQL database provider to scaffold a real database
5. Practice with C# and SQL schemas.

### Current API

The API provides access to a very simple LMS (learning management system) database.

| API | Description | Request body | Response body |
|-----|-------------|--------------|-------------|
| POST /api/users/authenticate | JWT Authentication | User | User + JWT |
| POST /api/users/register | Create and authenticate a new user | User | User + JWT |
| GET /api/users | Get all users | None | Array of users |
| GET /api/users/{id} | Get a user by ID | None | User |
| GET /api/courses | Get all courses | None | User |
| GET /api/courses/{id} | Get a course by ID | None | Course |
| GET /api/courses/name/{name} | Get a course by the specified name | None | Course |
| POST /api/courses/add | Add a new course | Course | Course |
| DELETE /api/courses/{id} | Delete a course | None | Course |
| GET api/teachers | Get all teachers | None | Teacher |
| GET api/teachers/{id} | Get a teacher by ID | None | Teacher |
| GET api/teachers/age/{age} | Get all teachers older than the given age | None | Array of teachers |
| DELETE api/teachers/{id} | Remove a teacher | None | Teacher |
| GET api/students | Get a student by ID | None | Student |
| GET api/students/courses/{name} | Get all students enrolled in the given course | None | Array of students |
|GET api/students/grades/{grade} | Get all students with the specified letter grade | None | Array of students |

### MySQL Schema Summary
| Table | Description |
|---|---|
| User | A user may have one of two roles: student or teacher |
| Course | A course is taught by a single teacher but may contain multiple students |
| Enrollment | Students are enrolled in one or more courses and receive a grade for each course |
