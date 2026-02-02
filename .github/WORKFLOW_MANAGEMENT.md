# Manual PR Workflow Management

This document describes how to manually trigger and monitor PR workflows using GitHub Copilot's MCP server tools.

## Prerequisites

Ensure you have:
- GitHub MCP server configured with Actions API permissions
- Access to the repository via Copilot
- Current PR branch checked out

## Step-by-Step Manual Process

### 1. Find the Current PR Workflow Run

```javascript
// List recent workflow runs for the .NET workflow
github-mcp-server-actions_list({
  method: "list_workflow_runs",
  owner: "danbowker",
  repo: "comaiz-admin",
  resource_id: "91065693",  // .NET workflow ID
  per_page: 10
})

// Look for the run matching your current commit SHA
// Filter by event="pull_request" and head_sha="<your-commit-sha>"
```

### 2. Trigger the Workflow

```javascript
// If the workflow status is "action_required":
github-mcp-server-actions_run_trigger({
  method: "rerun_workflow_run",
  owner: "danbowker",
  repo: "comaiz-admin",
  run_id: <workflow_run_id>  // From step 1
})

// Response: {"message":"Workflow run has been queued for re-run",...}
```

### 3. Monitor Workflow Execution

```javascript
// Poll every 30-60 seconds until completion
github-mcp-server-actions_get({
  method: "get_workflow_run",
  owner: "danbowker",
  repo: "comaiz-admin",
  resource_id: <workflow_run_id>
})

// Check the status field:
// - "queued" → waiting to start
// - "in_progress" → currently running
// - "completed" → finished (check conclusion)
```

### 4. Check Results

```javascript
// Once status="completed", check conclusion:
// - "success" ✅ → All good!
// - "failure" ❌ → Need to investigate
// - "cancelled" → Workflow was cancelled
// - "action_required" → Needs approval
```

### 5. Get Failure Logs (if needed)

```javascript
// Retrieve logs for failed jobs only
github-mcp-server-get_job_logs({
  owner: "danbowker",
  repo: "comaiz-admin",
  run_id: <workflow_run_id>,
  failed_only: true,
  return_content: true,
  tail_lines: 100
})

// This returns the last 100 lines of logs for each failed job
```

### 6. Analyze and Fix

```
1. Read the error messages in the logs
2. Identify the root cause:
   - Test failures: Look for "Failed: X, Passed: Y"
   - Build errors: Look for "Build FAILED" or "CS####" errors
   - Docker errors: Look for "docker build" failures
3. Make necessary code fixes
4. Commit and push changes
5. Return to step 1 (the workflow will auto-trigger on push)
```

## Quick Reference Commands

### Get Current Commit SHA
```bash
git rev-parse HEAD
```

### Find PR Number
```bash
gh pr status --json number,headRefName
```

### Check Workflow Status Locally
```bash
# Install gh CLI if needed
gh run list --workflow=dotnet.yml --branch=<branch-name> --limit=5
gh run view <run-id>
gh run view <run-id> --log-failed
```

## Common Scenarios

### Scenario 1: Workflow Needs Approval

**Problem:** Workflow shows status="action_required"

**Solution:**
```javascript
github-mcp-server-actions_run_trigger({
  method: "rerun_workflow_run",
  owner: "danbowker",
  repo: "comaiz-admin",
  run_id: <run_id>
})
```

### Scenario 2: Tests Failed

**Problem:** conclusion="failure", tests failed

**Solution:**
1. Get logs
2. Find failing test(s)
3. Fix the issue
4. Commit and push
5. Workflow auto-triggers

### Scenario 3: Build Failed

**Problem:** conclusion="failure", compilation errors

**Solution:**
1. Get logs
2. Find error messages (CS####)
3. Fix syntax/type errors
4. Test locally: `dotnet build`
5. Commit and push

### Scenario 4: Workflow Still Running

**Problem:** status="in_progress" for too long

**Solution:**
1. Check workflow steps with `list_workflow_jobs`
2. Look for hung steps
3. May need to cancel and restart

## Automation with Copilot

Instead of running these steps manually, configure GitHub Copilot to do this automatically by following the instructions in the agent configuration files:

- `.github/agents/pr-workflow-automation.md`
- `.github/copilot-instructions.md`

Copilot will then handle this entire workflow automatically after each commit.

## Tips

- **Wait between checks:** Don't poll too frequently (30-60 seconds is good)
- **Check locally first:** Run `dotnet build` and `dotnet test` before pushing
- **Read full logs:** The error is often not in the last few lines
- **Iterate carefully:** Make targeted fixes, don't change everything at once
- **Ask for help:** After 3 failures, consider asking a human

## Resources

- [GitHub Actions API Documentation](https://docs.github.com/en/rest/actions)
- [GitHub MCP Server Documentation](https://github.com/github/github-mcp-server)
- [Workflow File: .github/workflows/dotnet.yml](../workflows/dotnet.yml)
