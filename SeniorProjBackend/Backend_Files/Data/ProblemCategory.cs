namespace SeniorProjBackend.Data
{
    public class ProblemCategory
    {
        /*
         7.  **ProblemCategories Junction Table**:
    
            *   `ProblemCategoryID` (Primary Key, INT).
            *   `ProblemID` (INT, Foreign Key, references `Problems.ProblemID`).
            *   `CategoryID` (INT, Foreign Key, references `Categories.CategoryID`).
         */
        public int ProblemCategoryID { get; set; }
        public int ProblemID { get; set; } // Foreign Key; Problems.ProblemID
        public int CategoryID { get; set; } // Foreign Key; Categories.CategoryID

        // Navigation Properties
        public Problem Problem { get; set; }
        public Category Category { get; set; }

    }
}
