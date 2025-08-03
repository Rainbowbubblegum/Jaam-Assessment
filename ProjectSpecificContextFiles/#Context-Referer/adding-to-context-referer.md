# AI Instructions: Creating Context Referer Flags

## 🎯 **CORE MISSION**
When a user says: *"I want X to trigger Y context file"* - create a compact flag that reliably detects when that context should be used.

---

## 🔍 **THE STREAMLINED PROCESS**

### **Step 1: Analyze User Request (X)**
Extract from *"I want {X} to trigger {Y.md}"*:
- **Core Subject** - What is X fundamentally about?
- **User Language** - How do people actually say X?
- **Action Words** - What verbs relate to X?

### **Step 2: Read Target Context File (Y.md)**
**CRITICAL**: Read Y.md to understand:
- **What it does** - What problem does it solve?
- **Technical terms** - What specific words appear in the file?
- **Use cases** - What scenarios does it handle?

### **Step 3: Create Compact Flag**
Generate streamlined arrays using our universal scoring system.

---

## 🛠️ **UNIVERSAL SEMANTIC DETECTION**

### **Our Current Detection Logic:**
```javascript
// Uses JSON flag data with universal semantic matching
function detectFlags(prompt) {
  // 1. EXACT PHRASE MATCHING (+3 points)
  // 2. KEYWORD SEMANTIC MATCHING (+1 per match) 
  // 3. SEMANTIC INTENT ANALYSIS (+2 points)
  // 4. NEGATIVE FILTER (reset to 0)
  // Trigger: score ≥ 2 = read context file
}
```

### **Flag Creation Strategy:**
1. **Keywords Array** - Core terms + semantic variations (work item, ticket, etc.)
2. **Exact Phrases** - Realistic user phrases that should trigger
3. **Semantic Intent** - Clear description for universal intent analysis
4. **Negative Indicators** - Exclusions to prevent false positives

---

## 📋 **STREAMLINED EXAMPLE**

### **User Request:**
*"I want database migration requests to trigger database-migration-context.md"*

### **Analysis:**
- **X**: "database migration requests"
- **Y.md content**: Entity Framework, Add-Migration, Update-Database, schema changes
- **User language**: "migrate database", "run migrations", "schema changes"

### **Compact Flag Creation:**
```json
{
  "id": "database_migrations",
  "name": "Database Migrations & Schema Changes",
  "context_file": "database-migration-context.md",
  "keywords": ["database", "migration", "schema", "migrate", "entity", "framework", "ef", "dbcontext"],
  "exact_phrases": ["database migration", "run migrations", "migrate database", "schema changes", "add migration", "update database"],
  "semantic_intent": "Database migration requests, schema updates, and Entity Framework workflows",
  "related_terms": ["sql", "model", "add-migration", "update-database", "rollback", "migration history"],
  "negative_indicators": ["data migration", "server migration", "cloud migration", "user migration"]
}
```

### **How It Works with Universal Detection:**
```javascript
// The universal semantic detection automatically handles:
// 1. Keywords: "database", "migration", "schema" → direct matches
// 2. Semantic variations: "migrate" → "migration", "db" → "database"  
// 3. Intent analysis: "schema changes" matches database intent
// 4. Negative filters: "data migration" excluded by negative indicators

// NO additional detection object needed - uses JSON flag data directly!
```

---

## 🎯 **QUALITY CHECKLIST**

### **Before Adding to context-referer.mdc:**
1. **High-confidence words** - Are they truly domain-specific?
2. **Medium-confidence words** - Do they build toward the intent?
3. **Negative filters** - Do they prevent obvious false positives?
4. **Coverage test** - Would this catch realistic user requests?

### **Test Scenarios:**
```
✅ Should Trigger:
- "I need to migrate the database"
- "Run Entity Framework migrations"  
- "Schema changes not working"

❌ Should NOT Trigger:
- "Migrate users to new system" (data migration)
- "Server migration to cloud" (infrastructure)
```

---

## 🔧 **INTEGRATION STEPS**

### **1. Create the JSON Flag**
Add to the `flags` array in `.cursor\rules\context-referer.mdc`:

```json
{
  "id": "your_flag_id",
  "name": "Human Readable Name",
  "context_file": "your-context-file.md",
  "keywords": ["compact", "array", "format"],
  "exact_phrases": ["realistic", "user", "phrases"],
  "semantic_intent": "Clear description of when to trigger",
  "related_terms": ["supporting", "vocabulary"],
  "negative_indicators": ["exclusion", "words"]
}
```

### **2. Create Context File**
Create `your-context-file.md` with the actual context rules and instructions.

### **3. Test the Flag**
The universal semantic detection will automatically:
- Match keywords and semantic variations
- Analyze semantic intent using AI understanding
- Apply negative filters
- Score and trigger appropriately

**That's it!** No additional detection objects needed.

---

## ⚡ **CURRENT SYSTEM ADVANTAGES**

### **Universal Semantic Detection:**
- **No hardcoded rules** - Works for any flag automatically
- **Semantic flexibility** - Handles variations like "work item" = "ticket"
- **Intent analysis** - AI understands overall request intent
- **Parallel processing** - All flags scored simultaneously

### **Simple Integration:**
- **Just add JSON flag** - No additional detection objects
- **Automatic semantic matching** - Built-in equivalents and variations
- **Expandable** - New flags work immediately with existing system

---

## 🎯 **SUCCESS CRITERIA**

### **Your Flag Should:**
1. **Trigger reliably** - Catch real user requests for Y.md
2. **Avoid false positives** - Not trigger for unrelated requests
3. **Be balanced** - Equal treatment with other flags
4. **Stay compact** - Fit the streamlined system

### **Integration Test:**
After adding your flag, test with realistic prompts to ensure it triggers correctly and reads the right context file.

**Result**: Compact, efficient flags that work with our universal scoring system while maintaining accuracy and coverage. 