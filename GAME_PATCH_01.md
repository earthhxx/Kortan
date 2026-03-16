# Game Systems & Technical Specifications

## 1. Character Management (Player Status & Survival)
* **Survival Bars:** Features real-time depleting **Hunger** and **Water** mechanics.
* **Economy System:** Integrated **Money** system for purchasing in-game items.
* **Save/Load System:** Automated data persistence using `PlayerPrefs` to ensure stats and currency are retained after closing the game.
* **Death Logic:** When vital stats reach zero, the character enters a "Dead" state—disabling movement and camera control while unlocking the mouse cursor.

---

## 2. Basic Shop System (Interaction & Shop)
* **Smart ShopItem:** A versatile, single-script architecture using **Enums** to categorize items (e.g., Food, Water).
* **Interaction UI:** Context-sensitive UI that triggers when approaching items, displaying the price and the **[E]** key prompt.
* **Instant Consume:** A "Purchase and Use" mechanic that simultaneously deducts funds and restores the player’s survival stats.

---

## 3. Game Flow & Architecture
* **Main Menu:** Initial interface featuring **Start New Game** (Wipe Save), **Load Game** (Continue), and **Quit**.
* **Game Loop:** A complete gameplay cycle: Start → Play/Purchase → Death → Restart or Return to Menu.
* **DDOL Management:** Utilizes `DontDestroyOnLoad` to ensure the Player and UI persist across different scenes.
* **Singleton/Utility:** Employs a `GameManagerUtils` script to handle scene transitions, memory cleanup, and prevent "duplicate character" bugs.

---

## 4. Controls & Input
* **Custom Movement:** A bespoke movement and camera script supporting **Mouse Delta** inputs and custom **Gravity** physics.
* **Input State Control:** A dynamic system to enable or disable player controls based on game events (e.g., freezing movement upon death).
