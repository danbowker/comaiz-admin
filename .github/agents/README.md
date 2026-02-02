# Automated PR Workflow Management

This directory contains custom agent configurations for GitHub Copilot that enable automatic CI/CD workflow management.

## What This Does

When GitHub Copilot makes changes to a pull request, it will automatically:

1. ✅ **Trigger the PR workflow** after committing changes
2. 🔍 **Monitor the workflow** until completion
3. 📊 **Check for success or failure**
4. 🔧 **Apply fixes** if the workflow fails
5. 🔄 **Iterate** until all tests pass

## Files

- **`pr-workflow-automation.md`** - Main agent configuration defining the workflow pattern
- **`../copilot-instructions.md`** - Repository-wide Copilot instructions

## How It Works

### Automatic Triggering

After each commit, Copilot will:
1. Find the PR workflow run using the GitHub MCP server
2. Trigger it if status is "action_required"
3. Poll for completion every 30-60 seconds

### Failure Handling

If the workflow fails:
1. Retrieves detailed logs for failed jobs
2. Analyzes error messages and stack traces
3. Identifies the root cause (test failure, build error, etc.)
4. Applies targeted fixes
5. Commits and re-triggers the workflow
6. Continues until success (max 3 iterations)

### Example Workflow

```
Copilot makes code changes
    ↓
Commits with report_progress
    ↓
Triggers PR workflow (run_id: 12345)
    ↓
Monitors: status="in_progress" → "completed"
    ↓
Checks: conclusion="failure"
    ↓
Gets logs: "Test 'LoginTest' failed: Expected 200, got 401"
    ↓
Analyzes: Authentication issue
    ↓
Fixes: Updates auth token generation
    ↓
Commits fix
    ↓
Triggers workflow again (run_id: 12346)
    ↓
Monitors: status="completed"
    ↓
Checks: conclusion="success" ✅
    ↓
Done!
```

## API Access Required

This automation requires the GitHub MCP server to be configured with Actions API permissions:

- `actions:read` - Read workflow runs and jobs
- `actions:write` - Trigger workflow runs
- `contents:read` - Read repository contents
- `pull_requests:read` - Read PR information

## Configuration

### Repository Settings

The main .NET workflow (`dotnet.yml`) is configured to:
- Trigger on `pull_request` events
- Trigger on `workflow_dispatch` (manual runs)
- Require approval for first-time contributors (security)

### Workflow ID

The .NET workflow ID is: **91065693**

This is hardcoded in the agent instructions but can be updated if needed.

## Customization

To modify the automation behavior, edit:

1. **`pr-workflow-automation.md`** - Change workflow logic, iteration limits, timeout values
2. **`../copilot-instructions.md`** - Add repository-specific patterns or best practices

## Testing

To test this automation:

1. Make a breaking change to the code
2. Use `report_progress` to commit
3. Observe Copilot triggering and monitoring the workflow
4. Watch as it retrieves logs and applies fixes
5. Verify it iterates until the workflow passes

## Limitations

- Maximum 3 automatic fix iterations (prevents infinite loops)
- Requires MCP server with Actions API access
- Only monitors the main .NET workflow
- Manual intervention needed for complex failures

## Benefits

✅ Faster PR turnaround - No waiting for manual CI checks
✅ Automatic fix attempts - Common issues resolved immediately  
✅ Consistent process - Same workflow every time
✅ Learning from failures - Copilot improves with each iteration
✅ Developer productivity - Focus on features, not CI babysitting

## Future Enhancements

- [ ] Support for multiple workflows
- [ ] Parallel workflow execution
- [ ] Smart retry with exponential backoff
- [ ] Failure pattern learning
- [ ] Integration with code review comments
- [ ] Slack/email notifications on repeated failures
