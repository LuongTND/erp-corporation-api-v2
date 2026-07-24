# Git Convention

## 1. Branch Naming

- **Format:** `<type>/<short-description>`
- **Types:**
  - `feat/`: New feature
  - `fix/`: Bug fix
  - `hotfix/`: Urgent production fix
  - `chore/`: Maintenance, setup, dependencies
  - `refactor/`: Code improvement
- **Example:** `feat/POS-123-update-payment-ui`

## 2. Commit Message Format

- **Structure:** `<type>(<scope>):[<Role>] <subject>`
- **Rules:**
  - `subject`: Imperative mood (e.g., "add" not "added"). Lowercase. No period at the end.
  - `scope`: Optional. Specifies the module (e.g., `api`, `ui`, `auth`, `db`).
- **Types:**
  - `feat`: New feature
  - `fix`: Bug fix
  - `refactor`: Code change that neither fixes a bug nor adds a feature
  - `perf`: Code change that improves performance
  - `style`: Formatting, missing semi colons, etc. (no code change)
  - `docs`: Documentation only changes
  - `chore`: Updating build tasks, package manager configs, etc.
- **Examples:**
  - `feat(auth): add JWT login endpoint`
  - `fix(ui): resolve overflow issue on mobile cart`
  - `chore: update dependencies`
