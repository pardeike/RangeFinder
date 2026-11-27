# Project workflow

- Run `./scripts/build-quiet.sh -c Release` for a local build.
- Set `RIMWORLD_MOD_DIR` to the RimWorld 1.7 Mods path to deploy and create the ZIP.
- Keep the released 1.6 and older payloads unchanged. Put new resources under `1.7/`.

- Deployment preserves existing older-version DLLs and shared resources. A mismatched frozen payload stops deployment.
