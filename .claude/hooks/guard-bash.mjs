#!/usr/bin/env node
// PreToolUse guard for Bash commands. Blocks git/GitHub operations that break the Caterly
// workflow (docs/adr/0002-git-workflow-and-agent-merge-policy.md).
// Exit 2 + stderr = block the tool call and show the reason to Claude.
import { execFileSync } from "node:child_process";
import { readFileSync } from "node:fs";
import path from "node:path";

let input;
try {
  input = JSON.parse(readFileSync(0, "utf8"));
} catch {
  process.exit(0);
}
const cmd = String(input?.tool_input?.command ?? "");
if (!cmd) process.exit(0);

const block = (msg) => {
  process.stderr.write(`Blocked by .claude/hooks/guard-bash.mjs: ${msg}\n`);
  process.exit(2);
};

// Split on shell separators so each sub-command is checked.
const parts = cmd.split(/&&|\|\||;|\n|\|/).map((s) => s.trim()).filter(Boolean);
const protectedBranch = /(^|[\s:/])(main|master)(\s|$)/;

let gitDir = null;
const currentBranch = () => {
  try {
    const args = gitDir ? ["-C", gitDir, "rev-parse", "--abbrev-ref", "HEAD"] : ["rev-parse", "--abbrev-ref", "HEAD"];
    return execFileSync("git", args, { encoding: "utf8" }).trim();
  } catch {
    return "";
  }
};

for (const raw of parts) {
  // Normalize `env X=1 git …`, `cd x && git …` (already split), `git -C dir -c k=v …`.
  let p = raw.replace(/^(?:env\s+)?(?:[A-Za-z_][A-Za-z0-9_]*=\S*\s+)+/, "");
  gitDir = null;
  p = p.replace(/^(\S*\/)?git((?:\s+(?:-C\s+\S+|-c\s+\S+|--no-pager))+)/, (_m, _pre, opts) => {
    const c = opts.match(/-C\s+(\S+)/);
    if (c) gitDir = c[1].replace(/^["']|["']$/g, "");
    return "git";
  });
  p = p.replace(/^(\S*\/)?gh\s+/, "gh ");
  if (/\s--no-verify\b/.test(p) && /^git\b/.test(p)) block("--no-verify is not allowed.");

  if (/^git\s+push\b/.test(p)) {
    if (/\s(--force|-f)(\s|$)/.test(p) || /\s\+\S+/.test(p))
      block("plain force push is not allowed; use --force-with-lease on your own feature branch.");
    if (protectedBranch.test(p.replace(/^git\s+push/, "")))
      block("pushing to main/master is not allowed; open a pull request.");
    if (/--delete\b|\s:\S/.test(p) && /(main|master)/.test(p)) block("deleting main is not allowed.");
    const bare = /^git\s+push(\s+(-u|--set-upstream|--force-with-lease(=\S+)?|origin|-c\s+\S+))*\s*$/.test(p);
    if (bare && ["main", "master"].includes(currentBranch()))
      block("you are on main; create a feature branch first.");
  }

  if (/^git\s+commit\b/.test(p) && ["main", "master"].includes(currentBranch()))
    block("committing on main is not allowed; create a feature branch first.");

  if (/^git\s+branch\s+.*-D\s+(main|master)\b/.test(p)) block("deleting main is not allowed.");
  if (/^git\s+reset\s+--hard\b/.test(p) && ["main", "master"].includes(currentBranch()) && !/origin\/main/.test(p))
    block("hard reset on main is only allowed to origin/main.");

  if (/^gh\s+repo\s+(delete|archive|rename)\b/.test(p)) block("repository admin operations are not allowed.");
  if (/^gh\s+api\b/.test(p) && /branches\/[^/\s]+\/protection|\/rulesets/.test(p) && /-X\s*(DELETE|PUT|PATCH|POST)|--method\s+(DELETE|PUT|PATCH|POST)/i.test(p))
    block("changing branch protection or rulesets is not allowed for agents.");

  if (/owner-approved/.test(p) &&
      (/--add-label\b/.test(p) || /^gh\s+api\b/.test(p) || /^gh\s+label\s+(create|edit|clone)\b/.test(p)))
    block("only the product owner may add or edit the owner-approved label.");

  const merge = p.match(/^gh\s+pr\s+merge\b(.*)$/);
  if (merge) {
    const args = merge[1];
    if (/--admin\b/.test(args)) block("gh pr merge --admin bypasses protection and is not allowed.");
    if (/--(merge|rebase)\b|\s-[mr]\b/.test(args)) block("only --squash merges are allowed.");
    if (!/--squash\b|\s-s\b/.test(args)) block("use gh pr merge <n> --squash --delete-branch.");
    if (/--auto\b/.test(args)) block("--auto skips the agent merge gate; merge explicitly after the gate passes.");
    const num = (args.match(/(?:^|\s)#?(\d+)(?:\s|$)/) || [])[1];
    if (!num) block("specify the PR number explicitly.");
    const root = process.env.CLAUDE_PROJECT_DIR || process.cwd();
    let risk = "high";
    try {
      risk = execFileSync("bash", [path.join(root, ".claude/scripts/pr-risk.sh"), num], { encoding: "utf8" })
        .split("\n")[0].trim();
    } catch {
      block(`could not classify PR #${num} risk; refusing to merge.`);
    }
    if (risk !== "low") {
      let labels = [];
      try {
        labels = JSON.parse(
          execFileSync("gh", ["pr", "view", num, "--json", "labels", "--jq", "[.labels[].name]"], { encoding: "utf8" })
        );
      } catch {
        block(`could not read labels for PR #${num}; refusing to merge.`);
      }
      if (!labels.includes("owner-approved"))
        block(`PR #${num} is high risk and lacks the owner-approved label. Add needs-owner-approval and stop.`);
    }
  }
}
process.exit(0);
