### Core Tables

1.  **Users Table**:
   *   `UserID` (Primary Key, INT): Unique identifier for each user.
   *   `Username` (VARCHAR): User's chosen username.
   *   `PasswordHash` (VARCHAR): Hash of the user's password for secure storage.
   *   `EmailAddress` (VARCHAR): User's email address.
   *   `TwoFactorEnabled` (Boolean): Indicates if two-factor authentication is enabled.
   *   `SecretKey` (VARCHAR, optional): Encrypted  secret key used for generating 2FA codes. Null or empty if 2FA is not enabled.
   *   `TotalScore` (INT): Accumulated score from problem-solving activities.
   *   `Bio` (VARCHAR, optional): Short biography or user description.
   *   `ProfilePictureURL` (VARCHAR): URL to the user's profile picture, has default value
   *   `RegistrationDate` (DateTime): Date and time when the user registered.
   *   `LastActiveDate` (DateTime): Date and time when the user was last active.
   *   `Rank` (VARCHAR): User's rank, calculated from `TotalScore`.
   *   `RankIconURL` (VARCHAR): URL to an icon/image representing the user's rank, has default value
   *   `ActiveStreak` (INT, optional): Number of consecutive days the user has been active.

2.  **Problems Table**:
    *   `ProblemID` (Primary Key, INT): Unique identifier for each problem.
    *   `Title` (NVARCHAR): Title of the problem.
    *   `Description` (NVARCHAR(MAX)): Detailed description of the problem.
    *   `DifficultyScore` (INT): ELO/Kyu score indicating the problem's difficulty.
    *   `IsActive` (Boolean): Indicates whether the problem is active and available to users.
    *   `LastModifiedDate` (DateTime): Date and time when the problem was last modified.
    *   `TestCodeFileName` (NVARCHAR): Filename of the file containing the test code for the problem.

3.  **UserSubmissions Table**:   
    *   `SubmissionID` (Primary Key, INT).
    *   `UserID` (INT, Foreign Key, references `Users.UserID`).
    *   `ProblemID` (INT, Foreign Key, references `Problems.ProblemID`).
    *   `SubmittedCode` (VARCHAR(MAX)).
    *   `SubmissionTime` (DateTime).
    *   `IsSuccessful` (Boolean).
    *   `ScoreAwarded` (INT).
    *   `ExecutionTime` (INT, optional): Time taken to execute the solution.
    *   `MemoryUsage` (INT, optional): Memory used by the solution.
    *   `LanguageID` (INT, Foreign Key, references `Languages.LanguageID`): Programming language used in the submission.



### Normalized Tables for Language and Category

4.  **Languages Table**:
    
    *   `LanguageID` (Primary Key, INT).
    *   `LanguageName` (VARCHAR).
5.  **Categories Table**:
    
    *   `CategoryID` (Primary Key, INT).
    *   `CategoryName` (VARCHAR).
6.  **ProblemLanguages Junction Table**:
    
    *   `ProblemLanguageID` (Primary Key, INT).
    *   `ProblemID` (INT, Foreign Key, references `Problems.ProblemID`).
    *   `LanguageID` (INT, Foreign Key, references `Languages.LanguageID`).
7.  **ProblemCategories Junction Table**:
    
    *   `ProblemCategoryID` (Primary Key, INT).
    *   `ProblemID` (INT, Foreign Key, references `Problems.ProblemID`).
    *   `CategoryID` (INT, Foreign Key, references `Categories.CategoryID`).

### Additional Tables

 8.  **UserPreferences Table**:
    
    *   `UserPreferenceID` (Primary Key, INT).
    *   `UserID` (INT, Foreign Key, references `Users.UserID`).
    *   `PreferenceKey` (VARCHAR) represents the type or name of the preference (e.g., "Language", "Theme", "Notification", etc.)
    *   `PreferenceValue` (VARCHAR) holds the value of the preference (e.g., "en" for English language, "dark" for dark theme, "true" for enabling notifications, etc.)

9.  **Feedback Table**:
    
    *   `FeedbackID` (Primary Key, INT).
    *   `UserID` (INT, Foreign Key, references `Users.UserID`).
    *   `ProblemID` (INT, Foreign Key, references `Problems.ProblemID`, optional).
    *   `FeedbackText` (VARCHAR(MAX)).
    *   `SubmissionTime` (DateTime).

10. **AIConversations Table**:

    *   `ConversationID` (Primary Key, INT): Unique identifier for each conversation.
    *   `UserID` (INT, Foreign Key, references `Users.UserID`): Identifier of the user who had the conversation.
    *   `ProblemID` (INT, Foreign Key, references `Problems.ProblemID`): Identifier of the problem related to the conversation.
    *   `Timestamp` (DateTime): Date and time when the conversation took place.
    *   `ConversationContent` (NVARCHAR(MAX)): The entire conversation text, possibly in a structured format like JSON to preserve the flow of the conversation.
    *   `IsCompleted` (Boolean): Indicates whether the problem was solved in this session or if the conversation is ongoing.

11. **RecoveryCodes Table**:

    *   `RecoveryCodeID` (Primary Key, INT): Unique identifier for each recovery code.
    *   `UserID` (INT, Foreign Key, references `Users.UserID`): The user to whom the recovery code belongs.
    *   `Code` (VARCHAR): Hashed recovery code.
    *   `CreationDate` (DateTime): The date and time when the recovery code was generated.

12. **APIKeys Table** (New):
    * `APIKeyID` (Primary Key, INT): Unique identifier for each API key.
    * `UserID` (INT, Foreign Key, references `Users.UserID`): The user to whom the API key belongs.
    * `KeyType` (VARCHAR): Type of the API key (e.g., 'ChatGPT', 'Gemini').
    * `KeyValue` (VARCHAR): The actual API key, encrypted for security.
    * `Permissions` (VARCHAR, optional): Describes the scope or permissions granted by the API key.
    * `CreatedAt` (DateTime): Timestamp of when the key was created.
    * `ExpiresAt` (DateTime, optional): Timestamp of when the key expires, if applicable.

13. **AuditLogs Table** (New):
    * `AuditLogID` (Primary Key, INT): Unique identifier for each log entry.
    * `UserID` (INT, Foreign Key, references `Users.UserID`, nullable): The user associated with the event, if applicable.
    * `EventType` (VARCHAR): The type of event logged (e.g., login, 2FA verification, API key usage).
    * `Details` (VARCHAR(MAX)): Detailed information about the event, potentially stored as JSON for structured data.
    * `Timestamp` (DateTime): The date and time when the event occurred.




### Relationships and Connections

1. **Users to UserSubmissions (One-to-Many)**:
    - A single user can submit multiple solutions, represented by multiple entries in the `UserSubmissions` table.
    - The `UserID` in `UserSubmissions` serves as a foreign key linking back to the `UserID` in the `Users` table.
    - This relationship allows for tracking all submissions made by a specific user.

2. **Problems to UserSubmissions (One-to-Many)**:
    - Each problem can have multiple attempts or submissions by various users, reflected in the `UserSubmissions` table.
    - The `ProblemID` in `UserSubmissions` is a foreign key that references the `ProblemID` in the `Problems` table.
    - Submissions are thus associated with the specific problems they attempt to solve.

3. **Problems to Languages via ProblemLanguages (Many-to-Many)**:
    - Problems can be solved in multiple programming languages, necessitating a many-to-many relationship.
    - This is managed through the `ProblemLanguages` junction table, which contains `ProblemID` and `LanguageID` to link problems and languages.
    - Enables association of multiple languages with each problem, facilitating flexibility in problem-solving approaches.

4. **Problems to Categories via ProblemCategories (Many-to-Many)**:
    - A single problem can fall under multiple categories, such as "Algorithms" and "Data Structures."
    - Managed through the `ProblemCategories` junction table, linking problems to categories via `ProblemID` and `CategoryID`.
    - This setup allows problems to be categorized in a versatile manner, enhancing discoverability and organization.

5. **Users to Feedback (One-to-Many)**:
    - Users can provide feedback on multiple problems or platform features, tracked through the `Feedback` table.
    - The `UserID` in `Feedback` is a foreign key that references the `Users` table, identifying the source of each feedback entry.
    - Facilitates collection of user insights and experiences for continuous improvement.

6. **Problems to Feedback (One-to-Many)**:
    - Each problem can accumulate feedback from various users, providing valuable insights into its difficulty, clarity, or potential issues.
    - The `ProblemID` in `Feedback` links feedback entries back to the specific problems they address.
    - Essential for curating and refining problem content based on user input.

7. **Users to UserPreferences (One-to-Many)**:
    - Users can have multiple preference settings, allowing customization of their experience on the platform.
    - Each entry in the `UserPreferences` table is linked to a user via the `UserID` foreign key.
    - Supports personalization of user interactions and settings within the platform.

8. **Users to AIConversations (One-to-Many)**:
    - Users can engage in multiple conversations with an AI tutor or assistant, with each conversation stored for reference.
    - The `UserID` in `AIConversations` references the user involved in each conversation.
    - Enables tracking and analysis of AI interactions, supporting adaptive learning and assistance.

9. **Problems to AIConversations (One-to-Many)**:
    - AI conversations can be related to specific problems, offering targeted assistance or discussion on problem-solving strategies.
    - The `ProblemID` in `AIConversations` links these conversations to the relevant problems.
    - Enhances the educational value of conversations by associating them with specific learning objectives or challenges.

10. **Users to RecoveryCodes (One-to-Many)**:
    - Each user can generate multiple recovery codes as part of account security measures.
    - Recovery codes are linked to users through the `UserID` in the `RecoveryCodes` table.
    - Ensures users have a method to regain access to their accounts in case of lost credentials.

11. **Users to APIKeys (One-to-Many)**:
    - Users can possess multiple API keys, each with potentially different scopes or permissions for accessing platform functionalities programmatically.
    - The `APIKeys` table records each key along with the `UserID` of the owner, facilitating control and management of API access.

12. **Users to AuditLogs (One-to-Many)**:
    - Audit logs track significant actions or events associated with users, such as logins, changes to user profiles, or security-related activities.
    - Each entry in the `AuditLogs` table is associated with a user via the `UserID`, enabling accountability and monitoring of user actions on the platform.

### Additional Notes

*   **Junction Tables for Many-to-Many Relationships**: In the case of `ProblemLanguages` and `ProblemCategories`, the use of junction tables is a normalization technique to efficiently manage many-to-many relationships without data redundancy.
*   **Referential Integrity**: Foreign keys ensure referential integrity, meaning that all references from one table to another are valid and consistent.
*   **Querying and Data Retrieval**: These relationships facilitate complex queries, such as fetching all problems attempted by a user, or all languages a problem is applicable in, which are essential for the functionality of your platform.
*   **TwoFactorEnabled and SecretKey in Users Table**: These new columns facilitate the management of 2FA. The `SecretKey` is used for generating OTPs and should be encrypted for security.
*   **RecoveryCodes Table**: This new table is crucial for managing 2FA recovery codes. Each code is unique and should be securely stored. The table allows for the generation of new codes when one is used or when the user requests new ones.
