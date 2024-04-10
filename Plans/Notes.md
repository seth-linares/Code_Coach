# Logging and Auditing

<!-- Kestrel will be our web server that will listen for HTTP/S requests.  -->
<!-- Audit logs will be stored locally on the VM we are hosting Kestral on. 

    Example of File Logging with **Serilog**:
    var logger = new LoggerConfiguration()
    .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger(); 

    -->

<!--  -->

<!-- WHEN LOGGING FORMAT IN JSON FOR EASY ANALYSIS LATER  
     Useful when we need to consider indexing and looking at frequently used columns -->

<!-- With the logs we get how are we going to analyze them?
        - Batch processing: For log files, you might use batch processing (e.g., with Apache Hadoop or Spark) to analyze the data. 
          This is typically done at regular intervals (like nightly). 
        - Scheduled Queries: running scheduled queries or using a tool like Apache Kafka for stream processing.
        - Dashboarding: Have some sort of tool to enable easy dashboarding/graphing. I assume this would be done using python just for ease of use if we want to
          graph the data altough maybe it can make a dashboard too. -->

<!--  CCPA and GDPR regulation on data collection will need to be considered. I believe as long as our logging doesn't contain personal info it should be safe
      but we need to double check. -->

<!-- Cookies, auth tokens, refresh tokens, how to handle persistent sessions for SSO,  -->


### Captcha 

<!-- CAPTCHA JavaScript API integration Info

Customize the placement and characteristics of the CAPTCHA puzzle for your end users by integrating AWS WAF CAPTCHA into your client applications. -->

<script type="text/javascript" src="https://93f53941fd9c.us-east-1.captcha-sdk.awswaf.com/93f53941fd9c/jsapi.js" defer></script>

### Enforcing Security Policies Based on 2FA Status

1.  **Backend Enforcement**:
    
    *   **API-Level Checks**: The most secure way to enforce 2FA restrictions is at the backend. Before processing a request, especially for sensitive operations, check the user's 2FA status.
    *   **Example**: In an API method, you might have a conditional check like `if (user.TwoFactorEnabled) { // proceed with operation } else { // deny access }`.
    *   **Advantages**: This method ensures that even if a frontend bypass is attempted, the backend security policy remains intact.
2.  **Frontend Behavior**:
    
    *   While backend enforcement is crucial, the frontend can also play a role in enhancing user experience.
    *   **UI Adjustments**: Based on the user's 2FA status, you can show or hide certain options. For example, features requiring 2FA can be greyed out or hidden if 2FA is disabled.
    *   **Prevent Unnecessary Requests**: Before making an API call for a 2FA-protected action, the frontend can check the user's 2FA status and either proceed or show a message indicating that the action requires 2FA.
    *   **Example**: When a user clicks a link or button for a sensitive feature, the frontend can first check (perhaps using a local state or context) if 2FA is enabled for the user and act accordingly.



<!-- Is it possible to use indexing on certain keys or columns? What would that look like? How would we determine which ones to index? -->
<!-- Example index: CREATE INDEX idx_userid ON Users(UserID); -->


<!-- In the DB, do we need to store session information? Like if a user is still logged in or something? -->



<!-- We should put this at the end if at all way too much hassle -->
## Process for setting up SSO
### 1\. Register Your Application with Google

*   **Create a Project in Google Developer Console**: Before you can use Google's SSO, you need to create a project in the Google Developer Console.
*   **Configure the OAuth consent screen**: Provide the necessary information about your application, like the app name, user support email, and developer contact information.
*   **Create OAuth 2.0 Credentials**: Create credentials (Client ID and Client Secret) for your web application. You will need to provide the authorized redirect URIs, which are the endpoints in your application where users will be redirected after they have authenticated with Google.

### 2\. Implementing Google Sign-In in Your ASP.NET Application

*   **Install Required Libraries**: Make sure your ASP.NET project has the necessary libraries for OAuth 2.0 authentication. You might use Google's libraries or generic OAuth 2.0 libraries.
*   **Authentication Flow**: Implement the OAuth 2.0 flow to redirect users to Google's sign-in page, handle the redirect back to your application with an authorization code, and exchange this code for an access token and a refresh token from Google.
*   **Fetch User Profile**: Use the access token to fetch the user profile from Google's UserInfo endpoint.

### 3\. Integrating User Data with Your Database

*   **User Identification**: Once you have the user's information from Google, check if the user already exists in your database. Typically, you would use an email address as a unique identifier.
*   **Account Creation/Update**: If the user is new, create a new user record in your database. If the user exists but the information has changed, update their record.
*   **Session Management**: Generate a session or a token (like JWT) for the user on your side. This will be used to maintain the user's logged-in status.

### 4\. Session Management for Prolonged Periods

*   **Persistent Login**: Implement persistent login functionality by using refresh tokens or long-lived session tokens. Store these tokens securely on the client-side (e.g., HttpOnly cookies) and on your server.
*   **Refresh Tokens**: Use refresh tokens to obtain new access tokens when the current token expires without requiring the user to log in again. Ensure you securely store and manage refresh tokens, as they can potentially allow long-term access to user accounts.
*   **Token Expiry and Renewal**: Implement logic in your application to automatically refresh the access token using the refresh token before it expires, ensuring the user remains logged in.
*   **Security Considerations**: Implement security measures such as HTTPS, token revocation on logout, monitoring for unusual activity, and regular audits of token usage.

### 5\. Testing and Validation

*   **Test the Login Flow**: Thoroughly test the login flow in different scenarios, including first-time logins, returning users, token expiry, and manual logout.
*   **Validate Session Handling**: Ensure that your session handling logic correctly maintains user sessions across different sessions and properly expires sessions when necessary.

### 6\. Comply with Privacy and Legal Requirements

*   **User Consent**: Make sure to obtain and manage user consent for data processing in compliance with relevant laws (e.g., GDPR, CCPA).
*   **Data Handling and Security**: Implement robust security measures to protect user data and comply with privacy laws.


## Database Schema
### Core Tables

1.  **Users Table**:
    *   `UserID` (Primary Key, INT): Unique identifier for each user.
    *   `Username` (VARCHAR): User's chosen username.
    *   `PasswordHash` (VARCHAR): Hash of the user's password for secure storage.
    *   `EmailAddress` (VARCHAR): User's email address.
    *   `TwoFactorEnabled` (Boolean): Indicates if two-factor authentication is enabled. 
    <!-- might force enable 2FA, or just totally disable -->
    <!-- if we keep 2fa, we need to be able to store a recovery key -->
    *   `TotalScore` (INT): Accumulated score from problem-solving activities.
    *   `Bio` (VARCHAR, optional): Short biography or user description.
    *   `ProfilePictureURL` (VARCHAR, optional): URL to the user's profile picture. 
    <!-- Figure out how we will handle uploading of PFPs. Perhaps we could have a preset bank of images and let users pick 
         Also, need to set a URL to a default pfp and then have the ability to update it. -->
    *   `RegistrationDate` (DateTime): Date and time when the user registered.
    *   `LastActiveDate` (DateTime): Date and time when the user was last active.
    *   `Rank` (VARCHAR, optional): User's rank, calculated from `TotalScore`.
    *   `RankIconURL` (VARCHAR, optional): URL to an icon/image representing the user's rank.
    <!-- probably don't need rank or rank icon, should be able to be calced on the front end -->
    *   `ActiveStreak` (INT, optional): Number of consecutive days the user has been active.


    <!-- MAKE A TABLE FOR RECOVERY CODES-->
    <!-- For recovery codes, we should likely hand out 2-3 and then once a code is used, INVALIDATE and REPLACE all of them -->
    <!-- Once used, we should also either bring the user to a page to collect their new codes or email them a link to a page containing their new codes.
         In addition, we need to reset their 2FA because if they are using a recovery code it implies that their 2FA is compromised or lost. -->
    <!-- ^^ implies we need to have discrete methods or set of methods to set up 2FA and regen recovery codes -->
    <!-- I suppose this would mean we would have some chain of web pages that would handle authentication needs so that we can easily chain users along these processes -->



    <!-- It might make sense to make a 2FA table for secret keys for security, but probably doesn't matter unless we decide to allow multiple forms of 2FA then we need to -->


    <!-- 1.  GoogleAuthenticator can create the secret key, QR Code (comes as url and can be made into a popup or modal), ability to actually validate the TOTP code -->

    <!-- 2.  If a user loses their 2FA device, we should have a recovery key system in place. This would likely be a separate table with a foreign key to the user table. 
         We would need to store the recovery key in a secure manner, likely hashed and salted. We would also need to provide a way for the user to regenerate a new recovery key and reconfigure google auth. 
         We would also need to log this action in some form of audit log and provide the user with an email notifying them. -->

    <!-- 3.  If a user disabled 2FA we need to delete any secret key info and if using a separate recovery key table, we will need to delete any of those entries. 
         We will also need to likely prompt the user on the front end, record this action in some form of audit log, and provide the user with an email notifying them. -->

    <!-- 3a. The requirements to disable 2FA should likely be a pain in the butt so it would likely prompt us to re-auth by using 2FA again or perhaps sending an email, but 2FA re-auth seems easy enough. 
          If we provide an email notif, we should embed a link to perhaps some kind of verification system where they would provide the recovery code to verify it's them disputing the shut off of 2FA and allow
          them to regenerate a password along with a new secret key and reconfigure google auth. Also log that this user claimed to be hacked/compromised for future reference -->

    <!-- 3b. If 2fa is disabled, we can update the session by calling something like :

                userManager.UpdateSecurityStampAsync(user) 
                
            This would be importand in updating any relevant variables or check for 2FA status, especially if we lock certain features behind a check for 2FA-->






    
    


2.  **Problems Table**:
    
    *   `ProblemID` (Primary Key, INT): Unique identifier for each problem.
    *   `Title` (VARCHAR): Title of the problem.
    *   `Description` (VARCHAR(MAX)): Detailed description of the problem.
    *   `DifficultyScore` (INT): ELO/Kyu score indicating the problem's difficulty.
    *   `IsActive` (Boolean): Indicates whether the problem is active and available to users.
    *   `LastModifiedDate` (DateTime): Date and time when the problem was last modified.
    *   `TestCodeFileName` (VARCHAR): Filename of the file containing the test code for the problem.
    <!-- potentially add number of times completed / number of attempts -->

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
    *   `LanguageUsed` (VARCHAR): Programming language used in the submission.

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
    *   `PreferenceType` (VARCHAR).
    *   `PreferenceValue` (VARCHAR).
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
    *   `ConversationContent` (TEXT): The entire conversation text, possibly in a structured format like JSON to preserve the flow of the conversation.
    *   `IsCompleted` (Boolean, optional): Indicates whether the problem was solved in this session or if the conversation is ongoing.
<!-- I need to figure out exactly how we will make the conversations available! -->


### Relationships and Connections

1.  **Users-UserSubmissions** (One-to-Many):
    
    *   Each `User` can make multiple `UserSubmissions`.
    *   Represented by the `UserID` foreign key in `UserSubmissions`, referencing `UserID` in `Users`.
    *   Allows tracking of which user made each submission.
2.  **Problems-UserSubmissions** (One-to-Many):
    
    *   Each `Problem` can have multiple `UserSubmissions`.
    *   The `ProblemID` foreign key in `UserSubmissions` references `ProblemID` in `Problems`.
    *   Associates submissions with the problems they are attempting to solve.
3.  **Problems-ProblemLanguages** (Many-to-Many):
    
    *   Problems can be applicable in multiple programming languages.
    *   Managed via `ProblemLanguages` junction table with `ProblemID` and `LanguageID`.
    *   Associates multiple languages with each problem.
4.  **Problems-ProblemCategories** (Many-to-Many):
    
    *   A problem can belong to multiple categories.
    *   Managed via `ProblemCategories` junction table with `ProblemID` and `CategoryID`.
    *   Facilitates categorizing problems into various types or topics.
5.  **Users-Feedback** (One-to-Many):
    
    *   Each user can provide multiple pieces of feedback.
    *   `UserID` foreign key in `Feedback` references `UserID` in `Users`.
    *   Tracks which user provided specific feedback.
6.  **Problems-Feedback** (One-to-Many):
    
    *   Each problem can receive feedback from multiple users.
    *   `ProblemID` foreign key in `Feedback` references `ProblemID` in `Problems`.
    *   Accumulates feedback on individual problems.
7.  **Users-UserPreferences** (One-to-Many):
    
    *   Each user can have multiple preference settings.
    *   `UserID` foreign key in `UserPreferences` references `UserID` in `Users`.
    *   Stores and retrieves individual user preferences.
8.  **Users-AIConversations** (One-to-Many):
    
    *   Each user can have multiple conversations with the AI tutor.
    *   `UserID` foreign key in `AIConversations` references `UserID` in `Users`.
    *   Enables tracking and revisiting of AI tutor conversations by users.
9.  **Problems-AIConversations** (One-to-Many):
    
    *   Each problem can have multiple AI tutor conversations associated with it.
    *   `ProblemID` foreign key in `AIConversations` references `ProblemID` in `Problems`.
    *   Links AI tutor conversations to specific problems.


### Additional Notes

*   **Junction Tables for Many-to-Many Relationships**: In the case of `ProblemLanguages` and `ProblemCategories`, the use of junction tables is a normalization technique to efficiently manage many-to-many relationships without data redundancy.
*   **Referential Integrity**: Foreign keys ensure referential integrity, meaning that all references from one table to another are valid and consistent.
*   **Querying and Data Retrieval**: These relationships facilitate complex queries, such as fetching all problems attempted by a user, or all languages a problem is applicable in, which are essential for the functionality of your platform.



<!-- Set up KMS to handle the recovery codes/secret information.
     S3 could possibly work for handling the items like profile pics and such. -->