# Versioning and Release Process

This document describes the versioning strategy and release process for the Comaiz Admin application.

## Overview

The application uses **manual release versioning** based on git tags following [Semantic Versioning](https://semver.org/) (SemVer). The CI/CD pipeline automatically generates version information for all builds, with special handling for tagged releases vs. development builds.

## Version Format

### Release Versions (Tagged)
When you create a git tag, the version matches the tag exactly:
- Tag: `v1.2.3` → Version: `1.2.3`
- Tag: `v2.0.0` → Version: `2.0.0`

### Development Versions (Untagged)
For non-release builds, the version is automatically generated in the format:
- **With existing tags**: `{last-tag}-next.{commits-since-tag}+g{short-sha}`
  - Example: `1.2.3-next.4+gabcdef` (4 commits after v1.2.3)
- **No tags yet**: `0.0.0-dev.{total-commits}+g{short-sha}`
  - Example: `0.0.0-dev.123+gabcdef` (123 total commits)

## Creating a Release

### 1. Decide on Version Number

Follow [Semantic Versioning](https://semver.org/):
- **MAJOR** version (X.0.0): Incompatible API changes
- **MINOR** version (0.X.0): Add functionality (backwards compatible)
- **PATCH** version (0.0.X): Bug fixes (backwards compatible)

### 2. Create and Push Annotated Tag

```bash
# Create an annotated tag
git tag -a v1.2.3 -m "Release 1.2.3: Brief description of changes"

# Push the tag to GitHub
git push origin v1.2.3

# Or push all tags at once
git push --tags
```

**Important**: Use annotated tags (`-a` flag) instead of lightweight tags for releases.

### 3. Trigger Release

When you push a tag, GitHub Actions automatically:
1. Builds the application with the release version
2. Creates a GitHub Release (if configured)
3. Deploys to production environment

## Version Information

### Where Version is Displayed

The application displays version information in multiple places:

1. **UI Footer/Navbar**: Click on the version number to see detailed build information
2. **`/version.json` Endpoint**: Publicly accessible JSON with full version metadata
3. **Build-time Environment Variable**: Available as `REACT_APP_VERSION` in the frontend

### Version Metadata

The `version.json` file contains:

```json
{
  "version": "1.2.3",
  "commit": "abc123def456...",
  "buildTime": "2024-11-14T18:00:00Z",
  "branch": "master"
}
```

- **version**: The semantic version string
- **commit**: Full SHA of the commit that was built
- **buildTime**: ISO 8601 timestamp of when the build was created
- **branch**: The git branch that was built

## CI/CD Integration

### GitHub Actions Workflow

The `.github/workflows/dotnet.yml` workflow includes version generation:

1. **Fetch Tags**: Ensures all tags are available for `git describe`
2. **Generate Version**: Creates version string based on tags and commits
3. **Create version.json**: Writes version metadata to `frontend/public/version.json`
4. **Inject Environment Variable**: Sets `REACT_APP_VERSION` for the build
5. **Build Frontend**: React app is built with the version information

### Workflow Triggers

- **Push to master**: Creates staging build with development version
- **Create Release/Tag**: Creates production build with release version
- **Pull Request**: Creates preview build with development version

## Version Display in UI

The frontend includes a `VersionInfo` component that:
- Displays the version in the navbar (right side)
- Fetches version data from `/version.json`
- Falls back to `REACT_APP_VERSION` if version.json is unavailable
- Shows detailed information on click (commit SHA, build time, branch)

### Component Usage

```tsx
import VersionInfo from './components/VersionInfo';

// In your layout component
<VersionInfo />
```

## Local Development

For local development builds, a default `version.json` is included:

```json
{
  "version": "0.0.0-dev",
  "commit": "local-development",
  "buildTime": "2024-01-01T00:00:00Z",
  "branch": "local"
}
```

You can override this by setting `REACT_APP_VERSION` when building:

```bash
REACT_APP_VERSION="0.0.0-local" npm run build
```

## Checking Current Version

### In Production/Staging

Visit the application and look for the version indicator in the navbar, or:

```bash
curl https://your-domain.com/version.json
```

### In Docker Container

```bash
docker exec <container-name> cat /app/wwwroot/version.json
```

### From Git Repository

```bash
# Get the latest tag
git describe --tags --abbrev=0

# Get the current version string (as CI would generate it)
git describe --tags --long 2>/dev/null || echo "0.0.0-dev.$(git rev-list --count HEAD)+g$(git rev-parse --short HEAD)"
```

## Best Practices

1. **Tag After Merge**: Create release tags after merging to master, not before
2. **Meaningful Messages**: Use descriptive tag messages explaining what's in the release
3. **Test Before Tagging**: Ensure staging/QA is successful before creating a release tag
4. **Semantic Versioning**: Follow SemVer strictly for predictable versioning
5. **Changelog**: Maintain a CHANGELOG.md file alongside version tags
6. **Pre-releases**: Use pre-release versions (e.g., `v1.2.3-beta.1`) for testing

## Troubleshooting

### Version Shows as "0.0.0-dev"

- **Cause**: No tags exist in the repository
- **Solution**: Create the first release tag: `git tag -a v1.0.0 -m "Initial release"`

### Version Not Updating in UI

- **Cause**: Browser cache or version.json not updated
- **Solution**: Hard refresh (Ctrl+Shift+R) or clear cache

### Git Describe Fails in CI

- **Cause**: Shallow clone doesn't include tag history
- **Solution**: The workflow includes `git fetch --tags --force` to resolve this

### Wrong Version After Tagging

- **Cause**: Tag not pushed to remote, or CI ran before tag was pushed
- **Solution**: Ensure tag is pushed: `git push origin <tag-name>`

## Example Release Workflow

```bash
# 1. Ensure you're on master and up to date
git checkout master
git pull origin master

# 2. Review changes since last release
git log v1.2.2..HEAD --oneline

# 3. Create the release tag
git tag -a v1.2.3 -m "Release 1.2.3

- Add user management features
- Fix contract filtering bug
- Update dependencies"

# 4. Push the tag
git push origin v1.2.3

# 5. Verify CI build and deployment
# Check GitHub Actions workflow status
# Verify version in staging/production

# 6. Create release notes
# Go to GitHub Releases and document the changes
```

## Related Documentation

- [Semantic Versioning](https://semver.org/)
- [Git Tagging](https://git-scm.com/book/en/v2/Git-Basics-Tagging)
- [GitHub Actions](https://docs.github.com/en/actions)
- [Deployment Documentation](DEPLOYMENT_CHANGES.md)
