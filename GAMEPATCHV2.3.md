# 🚀 Patch Notes: PATCH 2.3

## 🇹🇭 [TH] รายละเอียดการอัปเดต

### 1. ระบบควบคุมและมุมกล้องอัจฉริยะ (Movement & Camera System)
* **สลับโหมดได้ดั่งใจ:** ระบบ `SettingController` ที่ปรับสลับกล้องแบบ **First Person** (มุมมองบุคคลที่ 1) กับ **Isometric** (มุมมองเฉียง 45 องศา) ได้แบบ Real-time
* **เดิน 2 สไตล์:** สลับการบังคับตัวละครระหว่างใช้ปุ่ม **WASD** กับใช้เมาส์คลิกเดิน (**Click-to-Move**)
* **สมองกลนำทาง (NavMesh):** อัปเกรดให้ตัวละครฉลาดขึ้น สามารถคำนวณเส้นทางเดินอ้อมตึกหรือสิ่งกีดขวางได้เอง แถมรองรับการ "คลิกค้างแล้วลาก" ทั้งบนเมาส์ PC และจอทัชสกรีนมือถือ

### 2. ระบบเอาชีวิตรอดและสเตตัส (Survival & Player Stats)
* **หลอดพลังชีวิตครบวงจร:** จัดการค่าความหิว, น้ำ, ความหนาว, และเงิน ผ่าน `PlayerManager`
* **แฟชั่นกันหนาว:** ระบบเสื้อกันหนาวที่มีการคำนวณ "วันหมดอายุและพัง" อิงตามเวลาภายในเกม
* **ระบบ Game Over:** จัดการตอนตายอย่างเป็นระบบ (แก้บั๊กศพเดินได้เรียบร้อย!) พร้อมระบบรื้อฟื้นคืนชีพ (`ResetStatusForNewGame`) ที่โหลดเลือดและค่าต่างๆ กลับมา 100%

### 3. ระบบกระเป๋าและร้านค้า (Inventory & Shopping)
* **กระเป๋าสไลด์เมาส์:** ระบบ `InventoryManager` ที่ฉลาดพอจะรู้ว่าเปิดกระเป๋าแล้วต้องโชว์เมาส์ให้คลิกไอเทม และปิดแล้วต้องกลับไปเป็นโหมดตามมุมกล้อง
* **มินิมาร์ท 7-11:** ระบบหยิบของใส่ตะกร้า (**CartUI**), นำไปจ่ายเงินที่จุดแคชเชียร์ (**Checkout**), และมีระบบรักษาความสะอาด "เทตะกร้าทิ้ง" อัตโนมัติเวลาเดินออกจากร้าน

### 4. ระบบจัดการฉากและการวาร์ป (Scene Teleportation)
* **พอร์ทัลข้ามมิติ:** ระบบ `TeleportPortal` รับส่งตัวละครข้ามแผนที่ และ `SceneEntryDetector` ที่คอยจับตัวละคร (DDOL) ไปวางตรงจุดเกิดใหม่แบบเป๊ะๆ
* **ย้ายร่างไร้บั๊ก:** จัดการปิด-เปิดสมองกล `NavMeshAgent` และ `CharacterController` ตอนข้ามฉาก เพื่อป้องกันอาการตัวแข็งหรือเดินติดบั๊ก

### 5. ระบบบันทึกข้อมูล (Save & Auto-Save)
* **บันทึกทุกย่างก้าว:** จัดเก็บข้อมูลสถานะผู้เล่น, เงิน, ของในกระเป๋า, การตั้งค่ากล้อง, และเวลาในเกมลงเครื่อง
* **เซฟอัจฉริยะ:** ระบบ **Auto-Save** ที่ทำงานทันทีที่คุณ "พับจอหนี" (**Pause**) หรือ "ปิดเกมทิ้ง" (**Quit**)

---

## 🇺🇸 [EN] Update Details

### 1. Movement & Camera System
* **Seamless Toggle:** Integrated `SettingController` allowing real-time switching between **First Person** and **Isometric** (45-degree) camera views.
* **Dual Movement Styles:** Supports both **WASD** keyboard controls and **Click-to-Move** mouse navigation.
* **Smart Navigation (NavMesh):** Enhanced pathfinding logic for obstacle avoidance. Now supports "Click and Drag" navigation for both PC mice and mobile touchscreens.

### 2. Survival & Player Stats
* **Comprehensive Vital Signs:** Centralized management of Hunger, Hydration, Temperature, and Currency via `PlayerManager`.
* **Thermal Gear:** Winter clothing system featuring durability and "Expiration/Breakage" mechanics based on in-game time.
* **Robust Game Over System:** Refined death handling (fixed "walking corpse" bugs!) with a `ResetStatusForNewGame` function to restore all stats to 100% upon respawn.

### 3. Inventory & Shopping
* **Contextual Cursor:** `InventoryManager` automatically toggles cursor visibility between item interaction (UI) and camera-look modes.
* **Convenience Store Mechanics:** Added "Add to Cart" (**CartUI**) and **Checkout** systems. Includes an automated "Clear Basket" cleanup feature when leaving the store area.

### 4. Scene Teleportation
* **Interdimensional Portals:** Utilizes `TeleportPortal` and `SceneEntryDetector` to accurately position persistent characters (DDOL) across different maps.
* **Bug-Free Transitions:** Automated toggling of `NavMeshAgent` and `CharacterController` during scene loads to prevent character freezing or clipping issues.

### 5. Save & Auto-Save System
* **Complete Data Logging:** Locally saves player status, currency, inventory contents, camera preferences, and in-game timestamps.
* **Smart Auto-Save:** Triggers immediate data persistence whenever the game is **Paused** or **Quit**.