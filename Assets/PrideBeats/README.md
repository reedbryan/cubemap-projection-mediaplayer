# Pride Beats XR Experience Game Manager

> **XR experience**: This README covers the **PC Unity project** that projects the 360° footage across three walls and orchestrates multi‑headset sessions via OSC.

---

## Overview
This folder is an extention of the [cubemap projection player](https://github.com/reedbryan/cubemap-projection-mediaplayer), used in the **Pride Beats** XR experience.

This project manages the connected headsets via broadcasted **OSC** messages (session lifecycle, calibration, countdown, gameplay timing, feedback).


## Features
- 🧭 **State machine** for operator‑driven sessions
- 🛰️ **OSC broadcast discovery** (`/SessionOpen` with IP)
- 📋 Live **player list** with calibration status for organizing the session
- ⏱️ Countdown & **note‑sequence** timing
- 🧩 Minimal, kiosk‑friendly UI


## State lifecycle
**State 1 — Session Closed**
- Regular 360° media player functionality
- UI shows **Start Session** → transitions to State 2

**State 2 — Session Open**
- Broadcast **`/SessionOpen`** (contains controller IP/port)
- "Start Session" button hidden
- **Player List** visible
- If `<3` players → show "searching for players"
- If `≥3` players → show "session full"
- UI: **Launch Game** → transitions to State 3

**State 3 — Calibrating**
- Broadcast **`/Calibrate`**
- Hide "Launch Game"
- Player list shows calibration progress (e.g., `Player 1: 1/3`)
- When **all** players calibrated → State 4

**State 4 — Game Starting**
- Broadcast **`/StartGame`** with `countdown` and `note sequence`
- Precompute **total duration** = `countdown + sum(sequence)`
- Hide UI except countdown
- When countdown reaches `0` → State 5

**State 5 — Game On**
- Start total time counter
- Start Pride video playback
- Show only user feedback UI (screen effects)
- When total time counter reaches `0` → **State 2** (Session Open)

> Note: You can persist player roster between runs or clear on return to State 2, depending on your flow.


## OSC address space (controller)
**Sends** (broadcast for discovery; unicast or broadcast for control)
| Address | Args | Purpose |
|---|---|---|
| `/SessionOpen` | `ip:string`, `port:int` | Discovery: headsets cache controller endpoint |
| `/Calibrate` | *(none or config)* | Instruct all players to enter calibration |
| `/StartGame` | `countdown:float`, `sequence:[…]` | Start round with timing data |

**Receives** (from each headset)
| Address | Args | Purpose |
|---|---|---|
| `/Player/Hello` | `id:string` | Optional discovery from headset |
| `/Player/Joined` | `id:string`, `model:string` | Add to player list |
| `/Player/Calib` | `id:string`, `done:int(0/1)`, `progress:int` | Update calibration UI; when all `done==1` → State 4 |
| `/Hit` | `id:string`, `t:float`, `noteIdx:int`, `inSync:int(0/1)`, `sig:string` | Register hits; verify signature; drive feedback |
| `/Heartbeat` | `id:string`, `t:float` | Mark player as alive


## Broadcast & networking
- The app computes the **broadcast IP** from the local IP and subnet mask (e.g., `192.168.1.255`) and sends `/SessionOpen` there so any headset on the LAN can hear it.
- Headsets reply **unicast** to the controller IP.
- Keep both directions **UDP** (ExtOSC) for low latency.

**Firewall/ports**
- Default ports: `PC→Headset 9000`, `Headset→PC 9001` (adjust as needed).
- Allow inbound UDP on the Headset→PC port.


## Operator UI & workflow
1. **Session Closed**: pick or cue the 360° video; confirm projection mapping spans the 3 walls.
2. Click **Start Session**.
3. Watch the **Player List** populate as headsets appear. If `<3` players, the UI shows "searching for players"; if `≥3`, "session full".
4. When ready, click **Launch Game** to begin calibration.
5. Track calibration progress (`x/3` per player). When all are done, the app enters **Game Starting**, shows a countdown, and starts playback at `t=0` when the countdown hits zero.
6. During **Game On**, the PC consumes `/Hit` messages to drive on‑screen feedback (e.g., overlays or screen flashes), while the 360° video runs. When the precomputed total time elapses, control returns to **Session Open**.


## Timing & note sequences
- A **sequence** is an ordered list of note durations or timestamps. Provide the same sequence to all headsets via `/StartGame` so they can compute hit windows locally.
- The PC may also compute global accuracy stats, combos, or leaderboards from `/Hit` events.


## Verifying signatures
- On `/Hit`, recompute the signature using the shared secret and compare to `sig`.
- If invalid, discard or mark as untrusted.
- Keep the signature algorithm documented in code and reference it in this README.


## Media & projection pipeline
- Place 360° assets in a known path (e.g., `Assets/StreamingAssets/Video/`).
- Use your existing triple‑wall mapping shaders/materials; document calibration steps for projectors here.


## Demo media in README
Inline controller demo:
```html
<video src="docs/pc-demo.mp4" controls playsinline muted width="720" poster="docs/pc-thumb.jpg"></video>
```
Or a GIF:
```md
![PC Controller Demo](docs/pc-demo.gif)
```


## Troubleshooting
- **Headsets don’t discover session**: verify broadcast address, subnet, and that Wi‑Fi AP allows client‑to‑client.
- **Firewall**: allow inbound UDP on the Headset→PC port.
- **Video lag**: pre‑warm player; ensure decode runs on GPU; avoid disk throttling.
- **Clock drift**: keep all machines on NTP; use countdown zero as the authoritative start.


## Configuration checklist
- [ ] Ports match on PC/headsets
- [ ] Broadcast enabled and correct subnet
- [ ] Signing secret set on both ends
- [ ] Minimum players threshold set to `3` (or configure)
- [ ] Sequence and countdown validated
- [ ] Projectors aligned & gamma matched


## Roadmap / Nice‑to‑haves
- Centralized scoreboard/accuracy view
- Dynamic difficulty scaling per player
- Operator override to skip calibration
- Per‑player color themes
- Save session logs (CSV/JSON)


## Credits & licenses
- 360° footage: Vancouver Pride Parade (credit/license)
- Networking: ExtOSC
- Base projection player: (acknowledge original project)

---

> **Repository layout idea**
>
> - `/Headset/` → README (this AR doc) + Unity project
> - `/PC/` → README (this controller doc) + Unity project
> - `/docs/` → demo videos/gifs, screenshots, diagrams
> - Root README: high‑level overview that links to each sub‑README

