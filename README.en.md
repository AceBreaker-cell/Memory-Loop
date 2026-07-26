# Memory Loop
A source code for my own original game, a visual novel game kinda thing with the style of 2D pixel art. 

<div align="center">

# 🏠 The Recurring Day

[Download source code here! 👋](https://drive.google.com/file/d/1w-Y3Wjmb8MIVBdesMe1MyjM75DPAJzlK/view?usp=sharing)

[Or Play it online! 🎮](https://acebreaker-cell.itch.io/memory-loop)

### *Every identical day is how you refuse to forget.*

[![Bahasa](https://img.shields.io/badge/🇮🇩_Switch_to-Indonesian-red?style=for-the-badge)](README.md)
[![Language](https://img.shields.io/badge/🇬🇧_Language-English-blue?style=for-the-badge)](README.en.md)

![Unity](https://img.shields.io/badge/Unity-6000.4.0f1-000000?style=flat-square&logo=unity)
![Genre](https://img.shields.io/badge/Genre-2D_Pixel_Art_Narrative-8B5E3C?style=flat-square)
![Status](https://img.shields.io/badge/Status-Final_Exam_Project-6A4C93?style=flat-square)
![Platform](https://img.shields.io/badge/Platform-PC-1e88e5?style=flat-square)

</div>

---

<p align="center">
  <img width="1577" height="881" alt="Screenshot 2026-07-25 231738" src="https://github.com/user-attachments/assets/516bd0d4-5c56-47fc-9e31-585685d5cb2f" />
  <img width="1578" height="888" alt="Screenshot 2026-07-25 201436" src="https://github.com/user-attachments/assets/a2cb30c2-1e31-4565-bdfe-a1394ecbe792" />
</p>

<details>
<summary><b>🔍 Tutorial setup</b></p></summary>

## ✅ Before You Start

Make sure you have:

- **[Unity Hub](https://unity.com/download)** installed
- **Unity Editor `6000.4.0f1`** (or a close 6.x version) installed via Unity Hub
  - Open Unity Hub → **Installs** tab → **Install Editor** → pick the matching version
- A bit of free disk space (a few GB for the Editor + project files)

---

## 🗂️ [Method 1] Download as ZIP (Easiest, No Git Needed)

**Step 1: Download**

Go to the GitHub repository page [(link in the README)]((https://drive.google.com/file/d/1w-Y3Wjmb8MIVBdesMe1MyjM75DPAJzlK/view?usp=sharing)), click the green **`<> Code`** button, then click **Download ZIP**.

**Step 2: Extract the ZIP**

Right-click the downloaded `.zip` file → **Extract All** (Windows) or double-click it (Mac). Use any archive tool (WinRAR, 7-Zip, or the built-in one) if needed.

> ⚠️ After extracting, you'll usually get a folder like `HariYangTerusBerulang-main`. **Open that folder** — inside it you should see subfolders named `Assets`, `Packages`, and `ProjectSettings`. That's the actual Unity project root you need for the next step.

**Step 3: Open Unity Hub**

1. Launch **Unity Hub**
2. Go to the **Projects** tab
3. Click **Add** → **Add project from disk**
4. Browse and select the extracted folder (the one containing `Assets`, `Packages`, `ProjectSettings`)
5. Click **Add Project** / **Select Folder**

**Step 4: Open the Project**

Click the project entry that just appeared in your Unity Hub list. Unity will start importing all assets — **this can take a few minutes on the first launch**, especially while it compiles shaders. Just let it finish, don't force close.

**Step 5: Play!**

Once the Editor is open:
1. In the **Project** window, go to `Assets/Scenes/`
2. Double-click **`Main Menu`** to open that scene
3. Press the ▶️ **Play** button at the top of the Editor

🎉 That's it you're in the game!

---

## 🌱 [Method 2] Clone via Git (For Updates & Contributors)

If you want to keep your copy easily updatable, or plan to contribute, cloning with Git is better than downloading a ZIP. Here's the full walkthrough even if you've never used Git before:

**Step 1: Install Git**

Download and install Git from **[git-scm.com](https://git-scm.com/downloads)**. Just click through the installer with default options.

**Step 2: Open a Terminal**

- **Windows:** Right-click on your Desktop or in a folder → **Open in Terminal** (or search "Command Prompt" / "Git Bash" in the Start Menu)
- **Mac:** Open the **Terminal** app (search via Spotlight)

**Step 3: Clone the Repository**

Navigate to a folder where you want the project to live, then run:

```bash
git clone https://github.com/AceBreaker-Cell/Memory-Loop.git
```

This will create a new folder with the full project inside it.

**Step 4: Add to Unity Hub**

Same as Method 1, Step 3–5:
1. Open **Unity Hub** → **Projects** → **Add** → **Add project from disk**
2. Select the cloned folder
3. Open the project, wait for it to import
4. Open `Main Menu` scene → press **Play**

**Bonus Getting future updates:**
Whenever the repo gets updated, just open a terminal inside your cloned folder and run:
```bash
git pull
```
This will fetch the latest changes without needing to re-download anything.

---

## 🛠️ Editing the Project

Feel free to explore, modify, and experiment with the project! A few tips:

- All gameplay scripts live in `Assets/Scripts/`
- All scenes (Main Menu, Loop 0–3, Final Loop, Endings) live in `Assets/Scenes/`
- Make sure your Unity Editor version matches (or is close to) `6000.4.0f1` to avoid compatibility warnings

---

## ❗ Troubleshooting

| Problem | Solution |
|---|---|
| Unity Hub says "Unity version not found" | Install the matching version via Unity Hub → Installs → Install Editor |
| Pink/missing textures after opening | Let the project finish importing fully, then restart Unity |
| Project won't open / stuck loading | Make sure you selected the folder that directly contains `Assets`, not a folder above or below it |
| Everything is very slow on first open | Normal — Unity is compiling shaders and importing assets the first time. Subsequent opens will be much faster |

---

## 📜 Credits & License

This project was created by **Muhammad Aziz Syah Dani** as a final exam project for the *Introduction to Game Programming* course And **Fajri Aulia** for the story concept of the game, with visual assets by **Nagita Syahira Putri** and **Muhammad Zaki Daisa Ammar**.

You're welcome to download, play, and modify this project for learning purposes **please credit the original creator if you share, showcase, or build upon this work.**

Some asset files in this project exceed GitHub's 25 MB upload limit, so the full source code (with all assets included) is hosted on Google Drive instead of directly in this repo.

➡️ [Download the complete project here:](https://drive.google.com/file/d/1w-Y3Wjmb8MIVBdesMe1MyjM75DPAJzlK/view?usp=sharing) 

Please use this Drive link and follow Method 1 (Download ZIP) in the setup guide the GitHub repo alone may be missing some larger files.

<div align="center">

**Copyright © Albatany 2026**

*Happy playing! 🎮*

</div>

</details>

---

## 📖 About the Game

**The Recurring Day** (Indonesian title: *Hari yang Terus Berulang*) is a 2D pixel art narrative adventure with light exploration and emotional puzzle elements. Built in Unity as a final exam project for the **Introduction to Game Programming** course.

The game explores themes of **homecoming, regret, memory, and acceptance** wrapped in a visual style that starts warm and gradually turns haunting.

> You play as **Mono**, an office worker who finally returns to his childhood home after being too caught up in work to visit for a long time. His mother welcomes him warmly, just like always.
>
> But the next day... is the same day.
> And the day after that... is the same, too.

As the day keeps repeating, Mono begins to notice something is wrong a clock that has stopped, a torn family photo, and a mother who starts behaving strangely. Players must explore, talk, and choose carefully, as every decision shapes how this story ultimately ends.

---

## 🎮 Gameplay Mechanics

### Exploration & Interaction
Freely explore Mono's childhood home from the front yard, living room, kitchen, to the bedroom. Use **Arrow Keys / A-D** to move, and **E / Space** to interact with objects or talk to other characters.

### Branching Dialogue
Every conversation with Mother offers different response options. The words you choose don't just change how characters react they also quietly **shape the direction of the story and which ending you'll receive**.

### The Loop System
The story unfolds across several *loops* (repeated days), each with a distinct atmosphere and intensity:

| Loop | Atmosphere | What Happens |
|---|---|---|
| **Loop 0** | Warm, normal | First day home everything feels fine |
| **Loop 1** | Slightly grim | Déjà vu begins, the wall clock has stopped |
| **Loop 2** | Darker | Family photo is damaged, a puzzle to find photo pieces begins |
| **Loop 3** | Very grim | Cracks start appearing, Mother's behavior grows stranger |
| **Final Loop** | Story climax | The truth is finally revealed |

### Photo Piece Puzzle
In Loop 2, you must explore the house to collect **family photo pieces** scattered across various rooms. The more pieces you collect, the more hidden rooms become unlocked.

### Hidden Emotion System
Behind the scenes, the game tracks three invisible emotional values: **Denial**, **Regret**, and **Acceptance**. Every dialogue choice you make adds to one of these values and whichever is highest by the end of the game determines which ending you'll receive.

### Memory Items
Objects you find and inspect are stored as *memory items* small fragments of Mono and his mother's past that slowly piece together the truth.

---

## 🌅 [Endings] Four Different Conclusions

<details>
<summary><b>⚠️ Click to expand contains story SPOILERS</b></summary>

<br>

The ending you receive is determined by the pattern of your dialogue choices throughout the game, not by a single decision at the end.

### 🌤️ [Ending A] *Acceptance*
Mono finally lets go. His mother isn't truly gone she becomes a memory he can carry without being trapped by it. The house slowly brightens, the loop stops, and a real morning finally arrives.

*Obtained by: frequently choosing honest and open dialogue, and collecting most of the memory items.*

### 🌫️ [Ending B] *Denial*
Mono refuses to accept the truth. He wakes up again in front of the house but this time, even the main menu appears broken. The lingering impression: this loop isn't over, and perhaps never will be.

*Obtained by: frequently choosing avoidant dialogue and ignoring signs of the truth.*

### 🍂 [Ending C] *Regret*
Mono realizes the truth but hasn't fully made peace with it. He leaves the house with an incomplete family photo an ending that feels bitter and unresolved.

*Obtained by: a mix of honest and avoidant dialogue choices, with the photo pieces left incomplete.*

### ✨ [Secret Ending] *Memory Album Complete*
If all memory pieces are successfully collected, a short epilogue unlocks: Mono returns to the house on a different day not to be trapped again, but to tidy the house and keep his mother's memory in peace. This time, he comes to say goodbye.

*Obtained by: collecting all memory items and leaning toward acceptance dialogue in the final moments.*

</details>

---

## 💭 The Message Behind the Story

This game was born from a simple question: *what's left behind when we're too busy to come home?*

**The Recurring Day** isn't simply about a curse or horror it's about how denial can trap someone into reliving the same day, rather than confronting loss. It's a reminder that time with the people we love can never truly be "postponed for later."

Sometimes, the bravest way to love someone is to be brave enough to say goodbye.

---

## 🕹️ Controls

| Action | Key |
|---|---|
| Move Left / Right | `←` `→` or `A` `D` |
| Interact / Talk | `E` or `Space` |
| Advance Dialogue | `E` / `Space` / `Enter` |
| Open Inventory | UI Button (top-right corner) |
| Pause | UI Button (top-right corner) |

---

## 🛠️ Built With

- **Engine:** Unity 6 (6000.4.0f1)
- **Language:** C#
- **Rendering:** Universal Render Pipeline (URP), 2D
- **UI:** TextMeshPro
- **Visual Style:** 2D Pixel Art, inspired by *A Space for the Unbound*

---

## 👥 Development Team

This project was made as a final exam assignment for the **Introduction to Game Programming** course by **Kelompok Ganjil**.

| Role | Name |
|---|---|
| 🧠 **Game Concept** | Fajri Aulia |
| 🎮 **Game Design, Programming, Narrative & Direction** | Muhammad Aziz Syah Dani |
| 🎨 **Visual Assets** | Nagita Syahira Putri |
| 🎨 **Visual Assets** | Muhammad Zaki Daisa Ammar |

<div align="center">

*Made wholeheartedly by one person who never stopped trying.*

</div>

---

<div align="center">

**"I'm home, Mom."**

</div>
