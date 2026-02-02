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
   Use the GitHub MCP server actions API to trigger the PR workflow:
   - List workflow runs to find the latest PR workflow run
   - Look for runs with event="pull_request" and the current head SHA
   - If status is "action_required", use rerun_workflow_run to approve and start it
   ```

3. **Monitor Workflow Execution**
   ```
   Poll the workflow run status every 30-60 seconds:
   - Use get_workflow_run to check status
   - Wait until status="completed"
   - Check the conclusion field for result
   ```

4. **Handle Workflow Results**

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

**List workflow runs for PR:**
```javascript
github-mcp-server-actions_list({
  method: "list_workflow_runs",
  owner: "danbowker",
  repo: "comaiz-admin",
  resource_id: "91065693",  // Workflow ID for .NET workflow
  per_page: 5
})
```

**Find PR workflow run by SHA:**
```bash
# Filter workflow runs to find the one matching current PR's head SHA
# Look for event="pull_request" and status="action_required" or "completed"
```

**Trigger/re-run workflow:**
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
- **Triggers:** push, pull_request, workflow_dispatch
- **Steps:** Build frontend, restore dependencies, build .NET, run tests, build Docker image

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
