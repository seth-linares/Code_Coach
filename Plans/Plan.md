# Project Plan

## Project Overview

Develop a web application integrating an AI chatbot for problem-solving and secure code execution within a sandbox environment. The application features a TypeScript/React frontend and an ASP.NET backend, with Microsoft SQL Server for data persistence. AWS services are utilized for deployment and operational needs, focusing on security, scalability, and user experience.

## Technical Components and Features

#### 1. **Frontend**:
   - **Technology Stack**: TypeScript and React for building a dynamic and responsive UI.
   - **Page Generation**: Use React Router for dynamic page generation, supporting various coding problems with templates.
   - **Secure Storage of API Keys**: Modify the approach for storing API keys by moving from local storage to a secure, server-side solution. This adjustment aims to mitigate the risk associated with cross-site scripting (XSS) attacks. Implement a secure API key management feature within the application's user profile section, allowing users to add, delete, and manage their API keys with ease.
   - **Content Security Policy (CSP)**: Implement CSP headers to prevent XSS attacks by restricting the resources the browser is allowed to load.
   - **User Authentication**: Incorporate user authentication with TOTP-based 2FA using Google Authenticator for enhanced security.
   

#### 2. **Backend**:
   - **Architecture**: Two ASP.NET Core applications; one for general backend services (user management, problem management, submissions, audit logging) and another for the Code Sandbox environment.
   - **Code Sandbox Security**: Secure the execution of user-submitted code in Docker containers with network isolation, resource limitation, and regular vulnerability scanning of Docker images.
   - **Database Integration**: Use Microsoft SQL Server for storing user data, problems, submissions, and more, with considerations for future scalability and performance optimization.
   - **API Key Management**: Develop a new set of backend services dedicated to managing API keys. This includes endpoints for adding, removing, updating, and selecting API keys. Each key can be associated with different services (e.g., Google, OpenAI) and models (e.g., GPT-3.5, GPT-4), and users can assign custom names to each key for easy identification.

#### 3. **Database**:
   - **Schema**: Tables for users, problems, submissions, API keys, languages, categories, user preferences, feedback, AI conversations, recovery codes, and audit logs. Adjustments include a new table for securely storing API keys.
      - **API Keys Table**: Introduce a new table to store API keys securely. The schema for this table will include columns for the API key ID, user ID (linking to the users table), service name, model type, key name (user-assigned for easy reference), the encrypted API key, creation date, and an optional expiration date.

#### 4. **AWS Services**:
   - **EC2 Instances**: Host backend applications and the database, initially on the same instance for cost-effectiveness, with separation considered for future scalability.
   - **S3**: Store and serve static web content.
   - **Certificate Manager**: Manage SSL/TLS certificates for HTTPS.
   - **KMS**: Secure cryptographic keys, including TOTP secrets and the user's API Keys.
   - **WAF**: Protect against web exploits and integrate CAPTCHA for additional security.
   - **Route 53**: Manage DNS and routing for the application, ensuring reliable access and performance.

#### 5. **Security and Compliance**:
   - **HTTPS Enforcement**: Use exclusively for secure communication.
   - **AWS Security**: Utilize AWS security features, including WAF, KMS, and IAM, to enhance the overall security posture of the application.
      - **Data Encryption**: Secure sensitive data at rest using AWS KMS (e.g., API keys, TOTP secrets).

#### 6. **Deployment and Operational Considerations**:
   - **Docker Containers**: For the secure and isolated execution of user-submitted code.
   - **Audit Logging**: Enhanced logging for monitoring and analysis, including user authentication events and 2FA interactions, stored in JSON format.
   - **No Use of AWS RDS, CI/CD, or Elastic Beanstalk**: Opt for simplicity and cost-effectiveness by initially hosting the database on the same EC2 instance as the backend.

#### 7. **Data Handling and UI/UX Design**:
   - **Dynamic Content Loading**: For an engaging and responsive user experience.
   - **Accessibility and Usability**: Design with a focus on user engagement and accessibility, ensuring a wide audience can effectively use the platform.
   - **API Key Management UI**: Design and implement a user-friendly interface for managing API keys within the application. This interface should allow users to easily add new keys, assign names to them, select different keys for different tasks, and switch between them as needed.

### Audit Logging and Security Measures Enhancements:

- **Comprehensive Audit Logging**: Extend audit logging to include actions related to API key management, such as the addition, deletion, and selection of keys, ensuring that all sensitive actions are traceable.

