# SignMap Project - Diagrams Documentation

Thư mục này chứa tất cả các diagram của dự án SignMap.

## 📊 Danh sách Diagrams

### 1. System Architecture Diagram
**File:** `01-system-architecture.md`  
**Mô tả:** Tổng quan kiến trúc hệ thống với microservices, data layer, và external services.

### 2. Database ERD (Entity Relationship Diagram)
**File:** `02-database-erd.md`  
**Mô tả:** Entity Relationship Diagram cho database schema, các mối quan hệ giữa các entities.

### 3. Component Diagram
**File:** `03-component-diagram.md`  
**Mô tả:** Cấu trúc components, dependencies, và responsibilities của từng component.

### 4. Sequence Diagram - Submit Contribution
**File:** `04-sequence-submit-contribution.md`  
**Mô tả:** Flow chi tiết khi user submit contribution, bao gồm AI detection và coin deduction.

### 5. Sequence Diagram - Voting Flow
**File:** `05-sequence-voting-flow.md`  
**Mô tả:** Flow voting với weighted algorithm, tính toán reputation, proximity, và expertise.

### 6. Sequence Diagram - AI Detection
**File:** `06-sequence-ai-detection.md`  
**Mô tả:** Flow AI Vision Service detect và classify traffic signs sử dụng YOLO.

### 7. Sequence Diagram - Payment Flow
**File:** `07-sequence-payment-flow.md`  
**Mô tả:** Flow payment và coin top-up, integration với payment gateway.

### 8. Deployment Diagram
**File:** `08-deployment-diagram.md`  
**Mô tả:** Infrastructure, deployment architecture, scaling strategy.

### 9. Use Case Diagram
**File:** `09-use-case-diagram.md`  
**Mô tả:** Actors, use cases, và mối quan hệ giữa chúng.

### 10. Data Flow Diagram
**File:** `10-data-flow-diagram.md`  
**Mô tả:** Data flow từ Level 0 (Context) đến Level 2 (Process details).

### 11. Class Diagram
**File:** `11-class-diagram.md`  
**Mô tả:** Class diagram cho domain models và service layer pattern.

### 12. State Diagram
**File:** `12-state-diagram.md`  
**Mô tả:** State machines cho Contribution, Payment, User Reputation, và Traffic Sign Status.

## 🛠️ Công cụ xem Diagrams

Các diagram được viết bằng **Mermaid syntax**, có thể xem bằng:

1. **GitHub/GitLab:** Tự động render Mermaid diagrams
2. **VS Code:** Cài extension "Markdown Preview Mermaid Support"
3. **Online Editor:** https://mermaid.live/
4. **Markdown Viewers:** Một số markdown viewers hỗ trợ Mermaid

## 📝 Cách sử dụng

### Xem trong VS Code:
1. Cài extension "Markdown Preview Mermaid Support"
2. Mở file `.md` trong VS Code
3. Nhấn `Ctrl+Shift+V` (Windows) hoặc `Cmd+Shift+V` (Mac) để preview

### Xem online:
1. Copy nội dung file `.md`
2. Paste vào https://mermaid.live/
3. Hoặc upload file lên GitHub/GitLab để tự động render

### Export sang hình ảnh:
1. Dùng https://mermaid.live/ để export PNG/SVG
2. Hoặc dùng Mermaid CLI: `mmdc -i input.md -o output.png`

## 🔄 Cập nhật Diagrams

Khi có thay đổi trong codebase:
1. Cập nhật diagram tương ứng
2. Đảm bảo diagram phản ánh đúng implementation
3. Commit changes cùng với code changes

## 📚 Diagram Types

### Architecture Diagrams
- **System Architecture:** High-level overview
- **Component Diagram:** Detailed component structure
- **Deployment Diagram:** Infrastructure and deployment

### Behavior Diagrams
- **Sequence Diagrams:** Interaction flows
- **Use Case Diagram:** User interactions
- **State Diagrams:** State machines và transitions

### Structure Diagrams
- **Class Diagram:** Domain models và service architecture

### Data Diagrams
- **ERD:** Database schema
- **Data Flow Diagram:** Data movement through system

## 🎨 Color Coding

- **Blue (#4A90E2):** API Gateway, Entry points
- **Red (#FF6B6B):** AI Service, Critical processes
- **Green (#50C878):** Database, Data stores
- **Orange (#FFA500):** Storage, External services
- **Purple (#9B59B6):** Real-time services, SignalR
- **Light Blue (#E3F2FD):** User actors
- **Light Red (#FFEBEE):** Admin actors

## 📖 Tham khảo

- [Mermaid Documentation](https://mermaid.js.org/)
- [Mermaid Live Editor](https://mermaid.live/)
- [UML Diagram Types](https://www.uml-diagrams.org/)

---

**Last Updated:** 2025  
**Maintainer:** Development Team

