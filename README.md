# living architecture notes

## Client-only rules to live by
* **No authentication**: clients generate a random, collisony player_id for themselves at startup and use that player_id in all correspondence from the server.
  * At some point, we should query the server for a player_id and use that instead
