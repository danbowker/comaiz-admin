# GitHub Copilot Custom Instructions for comaiz-admin

## Automatic PR Workflow Management

When working on pull requests in this repository, GitHub Copilot should follow this automated workflow to ensure all changes pass CI/CD checks:

### Standard PR Workflow

After making any code changes to a pull request:

1. **Commit and Push Changes**
   - Commit all changes with a descriptive message
   - Push changes to the PR branch

2. **Trigger PR Workflow**
   ```
   After pushing, trigger CI immediately using workflow_dispatch — do NOT wait
   for a pull_request_target event run to appear on its own:

   github-mcp-server-actions_run_trigger({
     method: "run_workflow",
     owner: "danbowker",
     repo: "comaiz-admin",
     workflow_id: "91065693",
     ref: "<current-branch-name>"
   })

   This uses the workflow_dispatch trigger which runs immediately without any
   approval requirement, regardless of who created the PR.
   ```

3. **Find the Triggered Run**
   ```
   After triggering, list runs filtered to the current branch to get the run ID:

   github-mcp-server-actions_list({
     method: "list_workflow_runs",
     owner: "danbowker",
     repo: "comaiz-admin",
     resource_id: "91065693",
     per_page: 5
   })
   // Filter results by head_branch == current branch name and event == "workflow_dispatch"
   ```

4. **Monitor Workflow Execution**
   ```
   Poll the workflow run status every 30-60 seconds:
   - Use get_workflow_run to check status
   - Wait until status="completed"
   - Check the conclusion field for result
   ```

5. **Handle Workflow Results**

   **If conclusion="success":**
   - Document the successful run
   - Proceed with next steps or complete the task

   **If conclusion="failure":**
   - Use get_job_logs with failed_only=true to retrieve failure details
   - Analyze the error messages and stack traces
   - Identify the root cause (test failures, build errors, linting issues, etc.)
   - Apply fixes to address the specific failures
   - Commit and push the fixes
   - Return to step 2 (trigger workflow again)
   - Continue iterating until conclusion="success"

### Example API Usage

**Trigger workflow immediately after pushing (preferred method):**
```javascript
github-mcp-server-actions_run_trigger({
  method: "run_workflow",
  owner: "danbowker",
  repo: "comaiz-admin",
  workflow_id: "91065693",
  ref: "<current-branch-name>"
})
```

**List workflow runs for PR branch:**
```javascript
github-mcp-server-actions_list({
  method: "list_workflow_runs",
  owner: "danbowker",
  repo: "comaiz-admin",
  resource_id: "91065693",  // .NET workflow ID
  per_page: 5
})
// Filter by head_branch == current branch to find the latest run
```

**Re-run a failed workflow:**
```javascript
github-mcp-server-actions_run_trigger({
  method: "rerun_workflow_run",
  owner: "danbowker",
  repo: "comaiz-admin",
  run_id: <workflow_run_id>
})
```

**Check workflow status:**
```javascript
github-mcp-server-actions_get({
  method: "get_workflow_run",
  owner: "danbowker",
  repo: "comaiz-admin",
  resource_id: <workflow_run_id>
})
```

**Get failure logs:**
```javascript
github-mcp-server-get_job_logs({
  owner: "danbowker",
  repo: "comaiz-admin",
  run_id: <workflow_run_id>,
  failed_only: true,
  return_content: true,
  tail_lines: 100
})
```

### Workflow Details

**Main .NET Workflow:**
- **Workflow ID:** 91065693
- **File:** `.github/workflows/dotnet.yml`
- **Triggers:** push, pull_request_target, workflow_dispatch
- **Steps:** Build frontend, restore dependencies, build .NET, run tests, build Docker image
- **Note:** `pull_request_target` triggers automatically without approval for same-repo PRs;
  `workflow_dispatch` also triggers immediately and is the preferred way to run CI from Copilot.

**Common Failure Scenarios:**

1. **Test Failures**
   - Review test output in logs
   - Identify failing test(s)
   - Fix code to make tests pass
   - Re-run workflow

2. **Build Errors**
   - Check compilation errors in logs
   - Fix syntax or type errors
   - Re-run workflow

3. **Linting Issues**
   - Review linting warnings/errors
   - Apply code style fixes
   - Re-run workflow

### Best Practices

- **Always verify locally first:** Run `dotnet build` and `dotnet test` before pushing
- **Incremental fixes:** Make small, focused changes to address specific failures
- **Log analysis:** Read the full error context, not just the first error
- **Timeout handling:** If workflow takes too long, check for hung processes or infinite loops
- **Iteration limit:** After 3 failed attempts, ask the user for guidance

### Integration with Copilot Workflow

This pattern should be automatically applied when:
- Working on any PR in this repository
- After using the `report_progress` tool to commit changes
- When explicitly asked to "run tests" or "check CI"
- When fixing issues identified in code reviews

The goal is to ensure that **all PRs have passing CI checks** before being marked as ready for review.
