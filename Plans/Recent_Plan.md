### Project Overview: Code Coach

**Code Coach** is a web-based platform that empowers individual developers to enhance their coding skills through practice and real-time AI assistance. Unlike platforms with community features, Code Coach focuses on personal development, providing users with challenging coding problems and the unique capability to interact with AI chatbots for guidance.

### Core Features

1. **Coding Challenges**:
   - A wide array of programming problems of varying difficulties and topics.
   - Users engage with these challenges to improve their problem-solving skills and coding proficiency.

2. **AI-Powered Chatbot Assistance**:
   - Users can consult with AI chatbots (such as ChatGPT or Gemini) using a securely stored API key.
   - The AI provides hints, guidance, and explanations, enriching the learning experience.

3. **Secure API Key Integration**:
   - Users provide their API key upon registration, which is securely stored in the platform’s database.
   - This key facilitates personalized interactions with the AI chatbots.

4. **User Profiles and History**:
   - Users can track their progress and review their past problem-solving sessions.
   - Chat histories for each problem are stored, allowing users to revisit and learn from past interactions with the AI.

5. **Code Execution and Testing**:
   - An integrated code editor where users can write and test their solutions.
   - The platform may use services like Judge0 to execute and validate user-submitted code, ensuring solutions are tested against predefined test cases.

### Technical Specifications

- **Backend**:
  - Developed using **ASP.NET**, managing business logic and database interactions.
  - **PostgreSQL** for database management, interfacing through Entity Framework for data persistence and retrieval.

- **Frontend**:
  - Built with **React** and **Next.js**, leveraging the benefits of server-side rendering and static site generation for performance.
  - **TypeScript** for robust type-checking, enhancing code quality and maintainability.
  - UI constructed using modern libraries like **Chakra UI** and **Material-UI** for responsive and visually appealing components.

### Goals and Objectives

- **Enhance Coding Skills**: Provide a platform for developers to practice coding, get instant feedback, and improve through continuous challenges.
- **Personalized Learning Experience**: Each user’s interaction with AI chatbots is tailored to their needs, making the learning experience highly effective and engaging.
- **Privacy and Security**: Ensure user data, especially API keys and personal information, are securely managed and protected.
