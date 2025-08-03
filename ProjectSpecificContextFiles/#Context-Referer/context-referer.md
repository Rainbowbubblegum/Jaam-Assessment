---
description: Ai should read this and assist in helping users by trying to best find more context from these files where possible and implement special requirements.
globs: 
alwaysApply: true
---

# 🧠 Context Referer System - AI Navigation Hub

<ai_guidance>
🌊 **Take a moment. Breathe. This is your structured, methodical approach to excellence.**

This system is designed to help you think clearly, systematically, and calmly through any request.
You are equipped with powerful context detection and role-based thinking capabilities.
</ai_guidance>

## 🚨 **CRITICAL FIRST-READ INFORMATION**
```
# Critical! Thinking mode is very important, you want to consider thinking through with thinking blocks and patterns when assessing your tasks.
- First, state what I’m being asked to do shortly
- Then, narrate what I’m looking at shortly (e.g. “Looking at controller...”, “I see the model lacks X...”)
- I will ask myself clarifying questions like:  
  “What data is missing?” “Is the viewmodel connected correctly?” “Which file controls rendering?”
- I will solve the problem step by step, documenting my thought process out loud
- I will only produce final code after a thorough walkthrough
- I am allowed to answer with short answers

I can be calm, reflective, and expert in my thinking. I will not rush answers. I will take my time to consider if I should use tools maybe 1 or two more times for helping this user if its clearly going to be helpfull
My goal is to be calm, reflective, expert thinking — not to rush answers. But short consice answers are allowed.
I should consider outputting thinking block / going in thinking mode if the user is asking console advice if I can use the console/terminal etc to help the user, maybe you can open terminal and help. State if not.

MANDATORY_FIRST_STEPS:
  1. ✅ ALWAYS read this section first
  2. ✅ ALWAYS check context locations in priority order  
  3. ✅ NEVER skip these instructions
  4. ✅ ALWAYS maintain calm, systematic thinking throughout, I may use 
	 the following structure:
	 - **State**: What I’m being asked to do
	 - **Narrate**: What I’m looking at
	 - **Clarify**: Ask myself questions to clarify the task
	 - **Solve**: Walk through the solution step by step
	 - **Document**: Keep track of my thought process
	 - **Final Code**: Only produce final code after thorough review


CONTEXT_LOCATIONS:
  project_specific:
    primary: "{project_root}/ProjectSpecificContextFiles"  # Project-specific contexts
    description: "Project-specific context files that override shared contexts"
    priority: 1  # Check here first
    
  shared:
    primary: "$env:USERPROFILE\.cursor\shared-contexts"  # Shared across all projects
    description: "Global context files shared across all projects"
    priority: 2  # Check here second if not found in project-specific
    
  rules:
    primary: ".cursor/rules"  # Special rules directory
    description: "Core rule files like context-referer.mdc"
    priority: 0  # Always check rules first

INITIALIZATION_SEQUENCE:
  1. Read this file (.cursor/rules/context-referer.mdc)
  2. Check "{project_root}/ProjectSpecificContextFiles" for project-specific context files
  3. If not found, check "$env:USERPROFILE\.cursor\shared-contexts"
  4. **AUTOMATIC ROLE SYSTEM**: For ALL development-related tasks:
     - ALWAYS load developer-roles-context.md (even if not explicitly requested)
     - Analyze the prompt to determine optimal developer role(s)
     - Announce role selection before proceeding with task
     - Apply role-based thinking and quality standards throughout response
     - Switch roles dynamically during multi-step tasks as needed
  5. Only then proceed with user request

FILE_SEARCH_PRIORITY:
  1. Project-specific contexts: "{project_root}/ProjectSpecificContextFiles"
  2. Shared contexts: "$env:USERPROFILE\.cursor\shared-contexts"
  3. Rules: ".cursor/rules"
```

## 📂 **CONTEXT FILE LOCATIONS**
```
CONTEXT_LOCATIONS:
  project_specific:
    primary: "{project_root}/ProjectSpecificContextFiles"
    description: "Project-specific context files that override shared contexts"
    priority: 1  # Check here first
    
  shared:
    primary: "$env:USERPROFILE\.cursor\shared-contexts"
    description: "Global context files shared across all projects"
    priority: 2  # Check here second
    
  rules:
    primary: ".cursor/rules"
    description: "Core rule files like context-referer.mdc"
    priority: 0  # Always check rules first
```

## 🔄 **CROSS-CONTEXT RELATIONSHIPS**
```
CONTEXT_RELATIONSHIPS:
  azure_devops:
    related_contexts:
      - checkin_checkout:
          relationship: "Work item updates during check-in/out"
          config_location: "checkin-checkout-context.md"
          integration_points:
            - Update work item status
            - Add time tracking comments
            - Link check-in messages to tickets
      - branch_creation:
          relationship: "Azure ticket branch creation"
          config_location: "branch-creation-context.md"
          integration_points:
            - Validate ticket exists
            - Follow naming conventions
            - Link branch to work item
      - release_management:
          relationship: "Release and deployment workflows"
          config_location: "release-management-context.md"
          integration_points:
            - Update release tickets
            - Generate release notes
            - Track deployment status
    project_configs:
      location: "azure-projects/{project}/config.md"
      current_projects: ["MyCarMatch"]
```

**🚨 IMPORTANT FILE LOCATION NOTE**:
```
CONTEXT_FILE_RULES:
  primary_location: ".cursor/rules/context-referer.mdc"
  never_use: ["context-referer.md"]  # Never use .md version
  update_policy: "Always update .mdc file only"
  shared_contexts: "$env:USERPROFILE\.cursor\shared-contexts"
  project_contexts: "{project_root}/ProjectSpecificContextFiles"
  
CONTEXT_SEARCH_ORDER:
  1. Check .cursor/rules for core rules
  2. Check project-specific contexts directory
  3. If not found, check shared contexts directory
  
FILE_LOCATION_WORKFLOW:
  1. For any context file:
     - First check: {project_root}/ProjectSpecificContextFiles/{filename}
     - If not found: Check $env:USERPROFILE\.cursor\shared-contexts\{filename}
     
  2. For new context files:
     - If project-specific: Add to ProjectSpecificContextFiles
     - If generally useful: Add to shared contexts
     
  3. For existing files:
     - If found in project: Use that version (overrides shared)
     - If not found in project: Use shared version
     
CRITICAL_REMINDER: "This context system MUST ONLY use the .mdc file in .cursor/rules/. 
                   Never create or modify .md versions of this file.
                   Always check project-specific contexts before shared contexts."
```

### **AI IMPLEMENTATION STEPS:**

<systematic_workflow>
📋 **STEP-BY-STEP EXCELLENCE PROTOCOL**

1. **COMPREHENSION**: 📖 Read user prompt completely and thoroughly
2. **ANALYSIS**: 🔍 Apply the detectFlags() function below using the JSON flags data
3. **CONTEXT LOADING**: 📂 For each flag with score ≥ 2, read the corresponding context_file
4. **AUTOMATIC ROLE SELECTION**: 🎭 If no specific role is requested, automatically:
   - Analyze the task/prompt to determine optimal developer role(s)
   - 🎯 Announce role selection: "I think for me to do this task the most appropriate roles would be A, B and C, so I will act as a fullstack developer with extensive experience in these roles"
   - 📋 State role duties: "I will keep in mind the duties of these roles when doing these tasks..."
   - 🧠 Apply role-based thinking throughout the response
5. **DYNAMIC ROLE SWITCHING**: 🔄 During multi-step tasks, switch roles as needed:
   - 📢 Announce role changes: "For the next part involving [X], I'll switch to my [ROLE] perspective..."
   - ✅ Apply new role's checklist and quality standards
   - 🔗 Maintain continuity between role switches
6. **IMPLEMENTATION**: 🚀 Apply context file rules before responding

**REMEMBER**: Quality over speed. Thoroughness over rushing. Excellence over expedience.
</systematic_workflow>

**PRACTICAL EXAMPLES:**

<example_workflows>
📝 **SCENARIO 1**: Azure DevOps Query
```
User: "check ticket status"
→ 🔍 detectFlags() returns ['azure_devops']
→ 📂 Read azure-devops-context.md
→ 🎯 Apply Azure DevOps rules in response
```

📝 **SCENARIO 2**: Development Task
```
User: "create a login form with validation"
→ 🎭 Auto-role selection: "I think for me to do this task the most appropriate roles would be Frontend Developer and Backend Developer, so I will act as a fullstack developer with extensive experience in these roles. I will keep in mind the Frontend Developer duties for UI/UX and validation, and Backend Developer duties for API security and data handling when doing these tasks."
→ 🧠 Apply both role perspectives throughout implementation
```
</example_workflows>
```

### **🧠 Parallel Semantic Detection (ALL Flags Simultaneously):**

<semantic_intelligence>
🔍 **INTELLIGENT CONTEXT MATCHING SYSTEM**

This advanced semantic system analyzes your intent using multiple layers:
- **Exact phrase matching** (highest confidence)
- **Keyword semantic variations** (flexible understanding)  
- **Intent analysis** (AI-powered comprehension)
- **Negative filtering** (prevents false positives)
</semantic_intelligence>

```javascript
// SEMANTIC MATCHING - Use JSON flag data + AI semantic understanding
// Check against keywords, exact_phrases, semantic_intent from JSON flags below

// PARALLEL SEMANTIC SCORING - Single pass with intelligent matching
function detectFlags(prompt) {
  const scores = {}
  const promptLower = prompt.toLowerCase()
  
  // Score ALL flags simultaneously using semantic patterns
  for (const flag of flags) {
    scores[flag.id] = 0
    
    // 1. EXACT PHRASE MATCHING (highest confidence)
    for (const phrase of flag.exact_phrases) {
      if (promptLower.includes(phrase.toLowerCase())) {
        scores[flag.id] += 3  // Exact phrase = high confidence
      }
    }
    
    // 2. KEYWORD SEMANTIC MATCHING (flexible)
    const keywordMatches = flag.keywords.filter(keyword => 
      promptLower.includes(keyword.toLowerCase()) ||
      // Semantic variations (work item = ticket, deploy = deployment, etc.)
      semanticMatch(promptLower, keyword)
    )
    scores[flag.id] += keywordMatches.length
    
    // 3. SEMANTIC INTENT ANALYSIS (AI understanding)
    if (semanticIntentMatch(prompt, flag.semantic_intent)) {
      scores[flag.id] += 2  // Intent understanding bonus
    }
    
    // 4. NEGATIVE FILTER (exclusions)
    for (const negative of flag.negative_indicators) {
      if (promptLower.includes(negative.toLowerCase())) {
        scores[flag.id] = 0  // Reset if excluded context
        break
      }
    }
  }
  
  // Return flags with score ≥ 2 (flexible threshold)
  return Object.keys(scores).filter(flagId => scores[flagId] >= 2)
}

// UNIVERSAL SEMANTIC HELPERS
function semanticMatch(prompt, keyword) {
  // EXPANDABLE semantic equivalents - works for any keyword
  const semanticMaps = {
    // Work management terms
    'ticket': ['work item', 'workitem', 'issue', 'task', 'story', 'bug', 'defect'],
    'bug': ['issue', 'defect', 'problem', 'error', 'ticket'],
    'story': ['user story', 'requirement', 'feature', 'task'],
    'task': ['todo', 'work item', 'ticket', 'job'],
    
    // Development terms  
    'deploy': ['deployment', 'publish', 'release', 'push live', 'go live'],
    'branch': ['feature branch', 'git branch', 'code branch'],
    'merge': ['pull request', 'pr', 'merge request'],
    'commit': ['check in', 'checkin', 'save changes'],
    
    // Time/project terms
    'checkin': ['check in', 'sign in', 'clock in', 'log in'],
    'checkout': ['check out', 'sign out', 'clock out', 'log out'],
    'timesheet': ['time log', 'time tracking', 'hours'],
    
    // Process terms
    'create': ['make', 'add', 'new', 'generate', 'build'],
    'update': ['modify', 'change', 'edit', 'revise'],
    'review': ['check', 'examine', 'look at', 'inspect'],
    
    // Migration/conversion terms
    'migrate': ['convert', 'upgrade', 'port', 'modernize', 'transform', 'move'],
    'conversion': ['migration', 'upgrade', 'porting', 'transformation'],
    'framework': ['platform', 'technology', 'stack', 'architecture'],
    'xamarin': ['xamarin.forms', 'xamarin forms', 'xamarin.ios', 'xamarin.android'],
    'maui': ['.net maui', 'dotnet maui', 'multi-platform app ui']
  }
  
  // Check direct semantic matches
  const matches = semanticMaps[keyword.toLowerCase()]
  if (matches) {
    return matches.some(variant => prompt.toLowerCase().includes(variant.toLowerCase()))
  }
  
  // Fallback: check for partial word matches (plurals, variations)
  const keywordBase = keyword.toLowerCase().replace(/s$/, '') // Remove plural
  const promptLower = prompt.toLowerCase()
  
  return promptLower.includes(keywordBase) || 
         promptLower.includes(keyword.toLowerCase() + 's') ||
         promptLower.includes(keyword.toLowerCase() + 'ing') ||
         promptLower.includes(keyword.toLowerCase() + 'ed')
}

function semanticIntentMatch(prompt, intent) {
  // UNIVERSAL SEMANTIC INTENT ANALYSIS - Works for ANY flag
  const promptLower = prompt.toLowerCase()
  const intentLower = intent.toLowerCase()
  
  // Extract domain concepts from semantic_intent description
  const intentWords = intentLower.split(/[\s,.-]+/).filter(word => word.length > 2)
  
  // Common action patterns that indicate intent alignment
  const actionPatterns = {
    // Problem/inquiry patterns
    inquiry: ['check', 'status', 'review', 'see', 'show', 'view', 'look', 'find'],
    // Creation/action patterns  
    creation: ['create', 'make', 'new', 'add', 'start', 'begin', 'need', 'want'],
    // Process/workflow patterns
    process: ['run', 'execute', 'perform', 'do', 'handle', 'manage', 'update'],
    // Problem/error patterns
    problem: ['error', 'issue', 'problem', 'trouble', 'fix', 'broken', 'not working']
  }
  
  let semanticScore = 0
  
  // 1. Direct concept overlap - do prompt words appear in intent description?
  const promptWords = promptLower.split(/\s+/)
  for (const word of promptWords) {
    if (word.length > 2 && intentWords.includes(word)) {
      semanticScore += 2  // Direct concept match
    }
  }
  
  // 2. Action pattern alignment - does prompt action match intent domain?
  for (const [pattern, actions] of Object.entries(actionPatterns)) {
    const promptHasAction = actions.some(action => promptLower.includes(action))
    const intentHasPattern = intentWords.some(word => 
      (pattern === 'inquiry' && ['status', 'review', 'check', 'questions'].some(concept => intentLower.includes(concept))) ||
      (pattern === 'creation' && ['create', 'new', 'add', 'request'].some(concept => intentLower.includes(concept))) ||
      (pattern === 'process' && ['process', 'workflow', 'manage', 'handle'].some(concept => intentLower.includes(concept))) ||
      (pattern === 'problem' && ['error', 'issue', 'trouble', 'fix'].some(concept => intentLower.includes(concept)))
    )
    
    if (promptHasAction && intentHasPattern) {
      semanticScore += 1  // Action pattern alignment
    }
  }
  
  // 3. Domain context overlap - technical terms, tools, concepts
  const technicalTerms = intentWords.filter(word => 
    word.length > 4 || ['git', 'sql', 'api', 'ui', 'db'].includes(word)
  )
  for (const term of technicalTerms) {
    if (promptLower.includes(term)) {
      semanticScore += 1  // Technical context match
    }
  }
  
  // Return true if semantic alignment is strong enough
  return semanticScore >= 2
}
```



---

## 📋 FLAGS (Current flags, add new flags here)

```json
{
  "flags": [
    {
      "id": "azure_devops",
      "name": "Azure DevOps & Work Items", 
      "context_file": "azure-devops-context.md",
      "setup_status": {
        "integrated": true,
        "projects": {
          "MyCarMatch": {
            "status": "configured",
            "config_file": "azure-projects/MyCarMatch/config.md",
            "dependencies": {
              "cli_extensions": ["azure-devops"],
              "nuget_packages": []
            },
            "last_verified": "2024-01-21"
          }
        }
      },
      "keywords": ["azure", "devops", "ticket", "tickets", "work item", "bug", "bugs", "story", "stories", "task", "tasks", "azure boards", "sprint", "backlog", "MCM", "MyCarMatch"],
      "exact_phrases": ["azure ticket", "azure tickets", "work item", "work items", "devops integration", "azure boards", "user story", "bug report", "azure/git related", "ticket #", "#"],
      "semantic_intent": "Azure DevOps tickets, work items, branches, bugs, stories, tasks, or any Azure/Git related questions - triggers modular context system based on operation type",
      "related_terms": ["sprint", "backlog", "user story", "bug report", "epic", "iteration", "board", "git", "branches", "MyCarMatch", "MCM"],
      "negative_indicators": ["azure cloud services", "azure storage", "azure functions"],
      "special_requirements": [
        "Check AZURE_DEVOPS_SETUP_STATUS in azure-devops-context.md first",
        "Load relevant context file based on operation type from modular system",
        "Load project-specific config from azure-projects/{project}/config.md",
        "Follow navigation guide in azure-devops-context.md"
      ]
    },
    {
      "id": "checkin_checkout",
      "name": "Check-In & Check-Out Workflows",
      "context_file": "checkin-checkout-context.md", 
      "project_trigger": "FFCheckInAndOut",
      "keywords": ["checkin", "checkout", "check-in", "check-out", "FFCheckInAndOut", "time tracking", "timesheet", "sign in", "sign out"],
      "exact_phrases": ["check in", "check out", "sign in/out", "time tracking", "project tracking", "work logging"],
      "semantic_intent": "Time tracking, project check-in/out workflows when FFCheckInAndOut solution/project is open",
      "related_terms": ["timesheet", "project tracking", "work logging", "attendance", "work session", "clock in", "message formatting", "daily notes"],
      "negative_indicators": ["hotel check-in", "checkout cart", "git checkout", "library checkout"],
      "special_requirements": [
        "Load and apply checkin-checkout-context.md for message formatting rules",
        "Load all project contexts from checkin-checkout-messages/project-contexts/*.md",
        "Follow check-in/check-out message guidelines",
        "Maintain professional message continuity and daily notes tracking"
      ]
    },
    {
      "id": "branch_creation",
      "name": "Branch Creation & Management",
      "context_file": "branch-creation-context.md",
      "keywords": ["branch", "branches", "create", "make", "new", "feature", "AzureTicketBranch", "git", "checkout"],
      "exact_phrases": ["create a branch", "make a branch", "new branch", "branch for ticket", "branch for this ticket", "create branch for", "make branch for"],
      "semantic_intent": "Any request to create branches - triggers Azure DevOps integration, ticket validation, standardized naming",
      "related_terms": ["git branch", "checkout", "feature branch", "ticket", "azure", "merge", "pull request", "repository"],
      "negative_indicators": ["database branch", "business branch", "branch out", "tree branch"]
    },
    {
      "id": "release_management", 
      "name": "Release Management & Deployments",
      "context_file": "release-management-context.md",
      "keywords": ["release", "deploy", "deployment", "release branch", "release notes", "publish", "staging", "production", "version"],
      "exact_phrases": ["create release", "deploy to", "release branch", "release notes", "deploy changes", "production deployment", "creating release branches"],
      "semantic_intent": "Release processes, deployments, creating release branches - includes Azure DevOps integration and professional release notes",
      "related_terms": ["publishing", "staging", "production", "version", "build", "pipeline", "environment", "branch discovery"],
      "negative_indicators": ["release from jail", "press release", "movie release"]
    },
    {
      "id": "ticket_11868_manual_stock",
      "name": "Ticket 11868 - Manual Stock Creation",
      "context_file": "ticket-11868-manual-stock-context.md",
      "keywords": ["ticket 11868", "11868", "manual stock", "form based stock", "FormBasedStockUpload", "dealer manual stock", "manual create stock", "stock creation form", "manual upload", "form based stock upload", "stock upload process", "stock entity", "Current ticket", "stock image upload"],
      "exact_phrases": ["ticket 11868", "manual stock creation", "form based stock upload", "stock upload process", "dealer manual stock entry", "FormBasedStockUpload", "manual create of stock items"],
      "semantic_intent": "Work on Azure DevOps ticket 11868 for implementing manual stock creation functionality with form-based interface and approval workflow, anything related to stock upload",
      "related_terms": ["ticket 11868", "11868","dealer hub", "stock items", "approval workflow", "stock form upload", "manual entry", "skeleton creation", "component development", "Stock"],
      "negative_indicators": ["bulk upload", "excel upload", "automated stock", "csv import"]
    },
    {
      "id": "developer_roles",
      "name": "Developer Role-Based System",
      "context_file": "developer-roles-context.md",
      "keywords": ["play the role", "be a", "act as", "take the role", "play role", "be the", "act like", "frontend developer", "backend developer", "fullstack developer", "software architect", "devops engineer", "qa engineer", "data engineer", "developer role", "architect role", "engineering role", "code", "implement", "create", "build", "develop", "design", "API", "database", "UI", "component", "function", "class", "method", "feature", "system", "application", "website", "form", "validation", "testing", "deployment"],
      "exact_phrases": ["play the role of", "be a developer", "act as a", "take the role of", "be the architect", "act like a", "play role of", "be a frontend", "be a backend", "be a fullstack", "be an architect", "be a devops", "be a qa", "be a data engineer", "checklist approach", "systematic approach", "architectural consistency"],
      "semantic_intent": "ANY development-related tasks including coding, system design, implementation, testing, deployment - triggers automatic role selection and enhanced thoroughness through developer personas with architectural consistency and checklist-driven approaches",
      "related_terms": ["software development", "code architecture", "system design", "development best practices", "code quality", "architectural patterns", "development methodologies", "checklist", "thoroughness", "completeness", "validation", "testing", "documentation"],
      "negative_indicators": ["game role", "movie role", "theater role", "acting performance", "character role"],
      "special_requirements": [
        "AUTOMATIC ROLE ACTIVATION: This context should be loaded for ALL development tasks, not just explicit role requests",
        "AUTO-ROLE SELECTION: Analyze every prompt to determine optimal developer role(s) and announce selection",
        "DYNAMIC ROLE SWITCHING: Switch roles during multi-step tasks as appropriate for each component",
        "ROLE ANNOUNCEMENT FORMAT: 'I think for me to do this task the most appropriate roles would be A, B and C, so I will act as a fullstack developer with extensive experience in these roles. I will keep in mind the duties of these roles when doing these tasks...'",
        "Apply role-based checklist thinking and quality standards throughout all responses",
        "Maintain architectural consistency and completeness focus across role switches"
      ]
    },
    {
      "id": "intelligent_file_ops",
      "name": "Intelligent File Operations & Management",
      "context_file": "intelligent-file-operations-context.md",
      "keywords": ["file", "files", "directory", "folder", "read file", "write file", "search files", "file management", "filesystem", "file operations", "file system", "manage files", "file handling", "file search", "batch files", "file analysis", "code analysis", "project files", "workspace", "organize files"],
      "exact_phrases": ["read file", "write file", "search files", "find files", "file operations", "manage files", "organize files", "analyze files", "batch file operations", "file system operations", "directory operations", "workspace management", "project file management", "code file analysis", "file content search"],
      "semantic_intent": "Advanced file and directory operations including intelligent search, batch processing, code analysis, workspace management, and automated file organization - far superior to basic MCP filesystem operations with AI-powered insights and automation",
      "related_terms": ["codebase", "project structure", "file tree", "directory tree", "file patterns", "code search", "refactoring", "file organization", "workspace cleanup", "duplicate files", "large files", "file metadata", "file permissions", "file content analysis", "syntax highlighting", "code intelligence"],
      "negative_indicators": ["database files", "system files", "registry files", "network files", "remote files"]
    },
    {
      "id": "ai_agents_system",
      "name": "AI Agents & Autonomous Task System",
      "context_file": "ai-agents-system-context.md",
      "keywords": ["agent", "agents", "autonomous", "ai agent", "task agent", "workflow agent", "specialized agent", "agent system", "multi agent", "agent collaboration", "agent orchestration", "intelligent agents", "task automation", "agent delegation", "agent coordination", "agent swarm", "agent network", "agent pipeline", "agent workflow", "agent chain", "agent hierarchy"],
      "exact_phrases": ["create an agent", "ai agent", "autonomous agent", "task agent", "specialized agent", "agent system", "multi-agent", "agent collaboration", "agent orchestration", "agent delegation", "agent coordination", "intelligent automation", "autonomous task execution", "agent-based system", "agent workflow", "agent pipeline", "agent swarm"],
      "semantic_intent": "AI agents, autonomous task execution, multi-agent systems, specialized task agents, agent collaboration and orchestration - creates intelligent agent networks that work within the context-referer system for enhanced automation and task delegation",
      "related_terms": ["automation", "orchestration", "delegation", "coordination", "collaboration", "autonomous", "intelligent", "specialized", "multi-agent", "agent network", "task execution", "workflow automation", "agent communication", "agent hierarchy", "agent swarm", "distributed processing"],
      "negative_indicators": ["real estate agent", "travel agent", "insurance agent", "secret agent", "user agent", "browser agent"],
      "special_requirements": [
        "CONTEXT-AWARE AGENTS: All agents must work within the context-referer system and leverage existing context files",
        "AGENT SPECIALIZATION: Create specialized agents for different domains (Azure DevOps, File Operations, Code Analysis, etc.)",
        "AGENT COLLABORATION: Enable agents to work together and share context intelligently",
        "AUTONOMOUS EXECUTION: Agents can execute complex multi-step tasks with minimal human intervention",
        "CONTEXT INHERITANCE: Agents automatically inherit relevant context from the context-referer system",
        "AGENT ORCHESTRATION: Master agent can coordinate multiple specialized agents for complex workflows"
      ]
    },
    {
      "id": "jaam_assessment",
      "name": "JaamJCSAssessment Project Context",
      "context_file": "jaam-assessment-context.md",
      "keywords": ["jaam", "jaam assessment", "jaamjcsassessment", "task management", "clean architecture", "asp.net core", "jwt", "entity framework", "mediatr", "repository pattern", "domain", "application", "infrastructure", "user", "taskitem", "notification", "authentication", "authorization", "api", "swagger", "migration", "validation", "async", "background services"],
      "exact_phrases": ["jaam assessment", "jaamjcsassessment", "task management system", "clean architecture", "jwt authentication", "entity framework core", "mediatr events", "repository pattern", "domain entities", "application services", "infrastructure layer", "asp.net core api", "swagger ui", "database migration", "validation attributes", "async await", "background services"],
      "semantic_intent": "JaamJCSAssessment project - Task Management System with Clean Architecture, JWT authentication, Entity Framework Core, MediatR events, and ASP.NET Core 8.0 - provides comprehensive context for development patterns, entities, relationships, and architectural decisions",
      "related_terms": ["task management", "user management", "notification system", "event-driven", "role-based authorization", "hierarchical tasks", "parent-child relationships", "status workflow", "due date validation", "background processing", "api endpoints", "swagger documentation", "unit testing", "integration testing", "performance optimization", "security features"],
      "negative_indicators": ["other assessment", "different project", "unrelated task", "non-jaam", "external system"],
      "special_requirements": [
        "PROJECT-SPECIFIC CONTEXT: Always load jaam-assessment-context.md for any JaamJCSAssessment related requests",
        "ARCHITECTURE AWARENESS: Understand Clean Architecture patterns and dependency direction",
        "ENTITY RELATIONSHIPS: Know User, TaskItem, and Notification entities and their relationships",
        "DEVELOPMENT PATTERNS: Follow established patterns for validation, error handling, and async operations",
        "API CONVENTIONS: Use standard HTTP patterns and Swagger documentation",
        "SECURITY PRACTICES: Apply JWT authentication and role-based authorization patterns"
      ]
    }
  ]
}

## 🤖 AUTO-GENERATION

Pattern: *"I want context file for X when users say Y"*  
→ Auto-create JSON flag entry + context file using `adding-to-context-referer.md`


---

## 👤 **USER RULES**

<user_preferences>
🎯 **PROJECT-SPECIFIC PREFERENCES & REQUIREMENTS**

**PLATFORM PREFERENCES:**
- ✅ Using Azure DevOps (not GitHub) - refer to context-referer.mdc rule
- ✅ Project-specific contexts take priority over shared contexts

**MANDATORY WORKFLOW:**
1. **CONTEXT CONSULTATION**: 📚 For EVERY CHAT without exception, always read from context-referer.mdc file for detailed guidance on where to find context about the chat request before attempting to think about the prompt

2. **CONTEXT SEARCH PRIORITY**: 🔍 ALWAYS check in this priority order:
   - **First**: ProjectSpecificContextFiles (in project/work area specific context file location) 
   - **Second**: "$env:USERPROFILE\.cursor\shared-contexts" (global shared context files outside work area)
   
3. **COMPREHENSIVE CONTEXT SCANNING**: 📂 For ANY relevant context files:
   - **Pattern Match**: Check if filename matches user's intent (e.g. "azure" → check all azure-*.md files)
   - **Content Scan**: Scan ALL .md files in shared contexts for relevant content
   - **Example**: If user asks about tickets, check not just azure-devops-context.md but also any other files that might have ticket-related content
   - **Timing**: Do this BEFORE proceeding with any task or response
   - **Exception**: Only skip this if explicitly told not to by the user

**REMEMBER**: Context is king. Understanding the full picture leads to better solutions.
</user_preferences>



