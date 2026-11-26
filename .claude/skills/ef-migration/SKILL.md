---
name: ef-migration
description: Add and review an EF Core migration for Caterly safely — test-first for constraints, generate the migration, review the idempotent SQL for destructive or locking operations, and document it for the PR. Use whenever an entity configuration or schema changes, or when asked to "add a migration", "change the schema" or "add a column/index/constraint".
---

# EF Core migration (safe by default)

Migrations make a PR **high risk** (`pr-risk.sh`), so the owner must approve it.

1. **Test first.** Write the failing PostgreSQL integration test for the behavior the schema
   must guarantee: the unique constraint rejects a duplicate active selection, the tenant
   filter hides another tenant's rows, the concurrency token rejects a stale update, `numeric`
   keeps exact money.
2. **Change the configuration** (`IEntityTypeConfiguration<T>`), not the domain type's
   attributes. Conventions: `snake_case` names if established, `TenantId` first in composite
   indexes, money `numeric(18,2)` + currency, rates `numeric(18,4)`, `timestamptz`.
3. **Generate the migration** (from `backend/`):
   `dotnet ef migrations add <PascalCaseIntent> -p src/Caterly.Infrastructure -s src/Caterly.Api`
   One migration per logical schema change, named after the intent
   (`AddMealSelectionActiveUniqueIndex`), not `Update1`.
4. **Review the SQL:**
   `dotnet ef migrations script --idempotent -p src/Caterly.Infrastructure -s src/Caterly.Api`
   Stop and ask the owner if you see any of these:
   - `DROP TABLE`, `DROP COLUMN` or a column type narrowing (data loss)
   - a rename that EF generated as drop + add
   - `NOT NULL` added to an existing populated column without a default or backfill
   - a large-table index created without `CONCURRENTLY` (needs a manual SQL migration)
   - changes to tables of issued invoices, payments or finalized snapshots (history must not
     be rewritten)
5. **Backfill.** Data moves go in the same migration only if they are small and idempotent.
   Otherwise, plan them separately with the architect.
6. **Verify.** Run the integration tests until they are green. Never run `dotnet ef database
   update` against a non-local database.
7. **PR.** Under "Database / migrations", give the migration name, a summary, whether it is
   destructive (yes or no), whether it needs a backfill, and the rollback approach.
