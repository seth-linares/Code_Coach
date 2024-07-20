namespace SeniorProjBackend.Data
{
    public enum DifficultyLevel
    {
        Easy = 1,
        Medium = 2,
        Hard = 3,
    }

    public enum ProblemCategory
    {
        WarmUps = 1,
        Strings = 2,
        Booleans = 3,
        Lists = 4,
    }

    public class Problem
    {
        public int ProblemID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public ProblemCategory Category { get; set; }

        // Navigation properties
        public virtual List<ProblemLanguage> ProblemLanguages { get; set; }
        public virtual List<UserSubmission> UserSubmissions { get; set; }
        public virtual List<AIConversation> AIConversations { get; set; }
    }
}

/*
{
  "title": "maxMirror",
  "description": "V2UnbGwgc2F5IHRoYXQgYSAibWlycm9yIiBzZWN0aW9uIGluIGFuIGFycmF5IGlzIGEgZ3JvdXAgb2YgY29udGlndW91cyBlbGVtZW50cyBzdWNoIHRoYXQgc29tZXdoZXJlIGluIHRoZSBhcnJheSwgdGhlIHNhbWUgZ3JvdXAgYXBwZWFycyBpbiByZXZlcnNlIG9yZGVyLiBGb3IgZXhhbXBsZSwgdGhlIGxhcmdlc3QgbWlycm9yIHNlY3Rpb24gaW4gezEsIDIsIDMsIDgsIDksIDMsIDIsIDF9IGlzIGxlbmd0aCAzICh0aGUgezEsIDIsIDN9IHBhcnQpLiBSZXR1cm4gdGhlIHNpemUgb2YgdGhlIGxhcmdlc3QgbWlycm9yIHNlY3Rpb24gZm91bmQgaW4gdGhlIGdpdmVuIGFycmF5LgoKbWF4TWlycm9yKFsxLCAyLCAzLCA4LCA5LCAzLCAyLCAxXSkg4oaSIDMKbWF4TWlycm9yKFsxLCAyLCAxLCA0XSkg4oaSIDMKbWF4TWlycm9yKFs3LCAxLCAyLCA5LCA3LCAyLCAxXSkg4oaSIDIK",
  "points": 15,
  "difficulty": "Hard",
  "category": "Lists"
}
*/