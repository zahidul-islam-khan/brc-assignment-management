# Bengal Renaissance College — Assignment & Submission Management System

The Assignment Management System is a comprehensive full-stack application designed to streamline educational workflows. It provides a centralized platform for creating, distributing, and grading assignments, ensuring a seamless experience for educational institutions.

## Roles and Features

The system supports three distinct user roles, each with specific permissions:

### Admin
- Create and manage user accounts (Students, Teachers, Admins).
- Configure system-wide settings, including classes and academic groups.
- Oversee all platform activities and manage user statuses.

### Teacher
- Create and publish assignments to specific classes.
- Review and grade student submissions.
- Manage their assigned subjects and track student performance.

### Student
- View active and past assignments assigned to their class.
- Submit work directly through the platform.
- Check grades and feedback from teachers.

## Local Setup

### Requirements
- Node.js (v18 or newer)
- .NET 8.0 SDK
- PostgreSQL (or a Neon/Supabase connection string)

### Running the Backend
1. Navigate to the `backend/` directory.
2. Update `src/BRC.API/appsettings.json` with your PostgreSQL database connection string.
3. Apply database migrations by running: `dotnet ef database update --project src/BRC.Infrastructure --startup-project src/BRC.API`
4. Start the backend server by running: `dotnet run --project src/BRC.API/BRC.API.csproj`
5. The API will be available at `http://localhost:5000`.

### Running the Frontend
1. Navigate to the `frontend/` directory.
2. Install dependencies by running: `npm install`
3. Start the development server by running: `npm run dev`
4. The application will be accessible at `http://localhost:5173`.

## Deployment Guide

### Frontend Deployment (Vercel)
1. Push your repository to GitHub.
2. Log in to Vercel and click "Add New Project".
3. Import your GitHub repository.
4. Set the **Framework Preset** to Vite.
5. Expand the **Build and Output Settings** and set the **Root Directory** to `frontend`.
6. Click Deploy. Vercel will automatically build and host the React application.

### Database Deployment (Neon or Supabase)
1. Create a free account on Neon.tech or Supabase.
2. Create a new PostgreSQL project and copy the provided connection string.
3. This connection string will be used in your backend environment variables.

### Backend Deployment (Azure for Students)
1. Sign up for Azure for Students using your `.edu` email address.
2. In the Azure Portal, create a new "Web App" (Azure App Service).
3. Select `.NET 8 (LTS)` as the runtime stack and Linux as the operating system.
4. Choose the `F1 (Free)` pricing tier.
5. Once created, go to the "Configuration" section of your Web App and add your Neon/Supabase database connection string as an environment variable (e.g., `ConnectionStrings__DefaultConnection`).
6. You can deploy your C# code directly from GitHub Actions by following the automated setup provided in the Azure Deployment Center.

Ensure that the frontend's API base URL is updated to point to your new Azure backend URL once deployed.
