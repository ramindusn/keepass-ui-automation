#!/usr/bin/env bash
# Checks the PR title and every PR commit against "type(KP-N): description" (KP-N = issue #N).
# The title is checked too because a squash merge turns it into the commit on main.

set -euo pipefail

pattern='^(feat|fix|test|chore|docs|ci|refactor|build|perf)\(KP-[0-9]+\): .+$'
failures=0

check() {
    local what=$1 message=$2

    if [[ $message =~ $pattern ]]; then
        echo "ok    $what: $message"
    else
        echo "FAIL  $what: $message"
        echo "::error title=Commit message format::$what does not follow 'type(KP-N): description' — $message"
        failures=$((failures + 1))
    fi
}

check "PR title" "$PR_TITLE"

while IFS=$'\t' read -r hash subject; do
    check "commit $hash" "$subject"
done < <(git log --no-merges --format='%h%x09%s' "$BASE_SHA..$HEAD_SHA")

if (( failures > 0 )); then
    cat <<'EOF'

Expected format:   type(KP-N): description

  type   feat, fix, test, chore, docs, ci, refactor, build or perf
  KP-N   the task's issue number — KP-3 is issue #3
  e.g.   test(KP-8): reject a wrong master key

To fix the latest commit:   git commit --amend
To fix older commits:       git rebase -i origin/main   (then reword)
Then push the branch again with --force-with-lease.
EOF
    exit 1
fi

echo
echo "All messages follow type(KP-N): description."
