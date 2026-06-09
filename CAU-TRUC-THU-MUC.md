# Cấu trúc thư mục — erp-corporation-api-v2

## Cấu trúc thư mục (ALL)

> Chỉ liệt kê **thư mục** (không liệt kê từng file `.cs`, `.json`, `.env`, `.sln`, `.md`…). Bỏ qua `bin/`, `obj/`, `.git/`, `.vs/`. File project (`.csproj`) nằm trong thư mục project tương ứng. Ở root repo còn các **file** solution / tài liệu (không hiện trong cây): `erp-corporation-api-v2.sln`, `README.md`, `.gitignore`, `Cấu trúc dự án.txt`, `CAU-TRUC-THU-MUC.md`, `CHECKLIST-PR.md`, `docs/HE-THONG-PHAN-QUYEN-VA-TO-CHUC.md` (đặc tả RBAC/org/user).

```
erp-corporation-api-v2/
│
├── src/                                            # Thư mục chứa mã nguồn chính
│   ├── API/                                        # [10] Tầng HTTP — Web API host
│   │   ├── Base/                                   # BaseApiController, CrudApiController
│   │   ├── Configuration/
│   │   ├── Controllers/
│   │   │   └── {Module}/                           # Gom controller theo module
│   │   ├── Filters/
│   │   ├── Middlewares/
│   │   └── Properties/
│   │
│   ├── Application/                                # [10] Use case, contract, DTO
│   │   ├── Interfaces/
│   │   │   ├── Repositories/                       # [10] IRepo
│   │   │   │   └── {Module}/                       # Gom IRepo theo module
│   │   │   └── Services/                           # [10] IService
│   │   │       ├── Common/                         # ICrudService (contract CRUD dùng chung)
│   │   │       └── {Module}/                       # Gom IService theo module
│   │   ├── Features/
│   │   │   └── {Module}/                           # [8]  CQRS theo module (tuỳ chọn)
│   │   │       ├── Commands/
│   │   │       ├── Queries/
│   │   │       └── EventHandlers/
│   │   ├── DTOs/
│   │   │   └── {Module}/
│   │   ├── Validators/
│   │   ├── Mappings/                               # AutoMapper Profile theo module
│   │   │   └── {Module}/
│   │   ├── Behaviors/
│   │   └── Common/
│   │       ├── Exceptions/
│   │       └── Models/
│   │
│   ├── Domain/                                     # [10] Lõi nghiệp vụ
│   │   ├── Base/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Enums/
│   │   ├── Events/
│   │   └── Common/
│   │
│   └── Infrastructure/                             # [10] Triển khai kỹ thuật
│       ├── Extensions/                             # [8]  DI manual + scan
│       ├── Security/                               # [7]  Helper bảo mật (static, không DI)
│       ├── Implementations/
│       │   ├── Repositories/                       # [10] Repo
│       │   │   └── {Module}/                       # Đối xứng Application
│       │   └── Services/                           # [10] Service
│       │       └── {Module}/                       # Đối xứng Application
│       ├── Outbox/                                 # [8]  Outbox processor + serializer
│       ├── Persistence/
│       │   ├── Configurations/
│       │   ├── Interceptors/                       # [9]  EF Core Interceptor (Audit tự động)
│       │   └── Outbox/
│       ├── Migrations/
│       └── External/
│           ├── Redis/
│           └── MemoryCache/
│
├── tests/                                          # Thư mục chứa các project kiểm thử (Unit, Integration Tests)
│
├── .github/
│   └── workflows/
```

**Luồng thư mục khi thêm module (ví dụ Order):**

```
src/API/Controllers/
  → src/Application/Interfaces/Services/
  → src/Infrastructure/Implementations/Services/
  → src/Application/Interfaces/Repositories/
  → src/Infrastructure/Implementations/Repositories/
  → src/Infrastructure/Persistence/

src/Domain/Entities/
src/Application/DTOs/{TênModule}/
```

---

Tài liệu mô tả cây thư mục hiện tại, mục đích từng folder, nội dung đặt trong đó và **mức độ quan trọng (1–10)**.

|-----------|-------------------------------------------------------|
| Điểm      | Ý nghĩa                                               |
|-----------|-------------------------------------------------------|
| **10**    | Lõi kiến trúc — không thể thiếu khi vận hành          |
| **8–9**   | Rất quan trọng — hầu hết module ERP đều dùng          |
| **6–7**   | Quan trọng — dùng khi triển khai tính năng tương ứng  |
| **4–5**   | Hữu ích — skeleton / sẽ cần sau                       |
| **1–3**   | Phụ trợ — tiện dev, tài liệu, không ảnh hưởng runtime |
|-----------|-------------------------------------------------------|

**Kiến trúc:** Clean Architecture · .NET 8 · **Service-first** · Repository + Service · EF Core · Outbox (domain events) · MediatR (handlers / Outbox dispatch)

**Luồng phụ thuộc:**

```
API → Application, Infrastructure
Application → Domain
Infrastructure → Application, Domain
Domain → (không phụ thuộc layer khác)
```

**Luồng nghiệp vụ (quy ước team):**

```
Controller → IService → IRepository → Database
```

---

## 1. API — `src/API/`

**Vai trò layer:** Nhận HTTP, trả response, cấu hình host, middleware. **Không** chứa business logic, **không** truy cập DB trực tiếp.

|---------------------------|-------------------------------------------|-----------------------------------------------------------------------|:-------------:|
| Thư mục / file            | Mục đích                                  | Chứa gì                                                               | Quan trọng    |
|---------------------------|-------------------------------------------|-----------------------------------------------------------------------|:-------------:|
| **`src/API/`** (root proj)| Host Web API, `Program.cs`                | `Program.cs`, `appsettings*.json`, `.env`                             | **10**        |
| **`Base/`**               | Class controller dùng chung               | `BaseApiController` — **Service-first**, inject `I*Service`; `CrudApiController` — CRUD mỏng + permission | **8** |
| **`Controllers/`**        | Endpoint theo resource / module           | `*Controller.cs` — gọi `IService` hoặc `IMediator`                    | **10**        |
| **`Configuration/`**      | Cấu hình app, load biến môi trường        | `EnvLoader.cs` (đọc `.env` → map vào `IConfiguration` theo quy ước)   | **9**         |
| **`Middlewares/`**        | Xử lý cross-cutting trên pipeline HTTP    | `ExceptionMiddleware` (ProblemDetails), sau này: logging, rate limit  | **8**         |
| **`Filters/`**            | Action / authorization filter             | `ValidationFilter`, `AuthorizationFilter` (khi có)                    | **6**         |
| **`Properties/`**         | Launch profile VS                         | `launchSettings.json`                                                 | **4**         |
| **`.env`**                | Secret & connection (không commit)        | `CONNECTION_STRING`, JWT, Redis, Mail, MISA…                          | **10**        |
| **`appsettings.json`**    | Cấu hình không nhạy cảm                   | Logging, `AllowedHosts`                                               | **7**         |
| **`API.http`**            | Gọi thử API trong IDE                     | Request mẫu                                                           | **2**         |
|---------------------------|-------------------------------------------|-----------------------------------------------------------------------|:-------------:|

**Không đặt ở src/API:** Entity, DbContext, `IRepository` implement, rule nghiệp vụ phức tạp.

---

## 2. Application — `src/Application/`

**Vai trò layer:** Định nghĩa use case (contract), DTO, validation, MediatR pipeline. **Không** biết HTTP, **không** biết EF/SQL cụ thể.

|-----------------------------------|---------------------------------------|---------------------------------------------------------------|:-------------:|
| Thư mục                           | Mục đích                              | Chứa gì                                                       | Quan trọng    |
|-----------------------------------|---------------------------------------|---------------------------------------------------------------|:-------------:|
| **`src/Application/`** (root)     | Đăng ký DI Application                | `DependencyInjection.cs` — MediatR, FluentValidation          | **10**        |
| **`Interfaces/Repositories/{Module}/`** | **IRepo** — contract truy cập dữ liệu, gom theo module | `IGenericRepository<T>`, `IUnitOfWork` ở root; `{Module}/IOrderRepository`… | **10** |
| **`Interfaces/Services/Common/`** | Contract dùng chung | `ICrudService<TDto, TCreate, TUpdate>` — module CRUD đơn giản kế thừa | **8** |
| **`Interfaces/Services/{Module}/`** | **IService** — contract nghiệp vụ, gom theo module | `{Module}/IOrderService`, `{Module}/ILeaveService`… Controller gọi đây | **10** |
| **`Features/`**                   | CQRS theo module (vertical slice)     | `{Module}/Commands/`, `Queries/`, `EventHandlers/`            | **8**         |
| **`DTOs/`**                       | Data transfer — API contract          | `{Module}/CreateXRequest`, `XDto` — không expose Entity       | **9**         |
| **`Validators/`**                 | FluentValidation                      | `CreateXValidator.cs` — validate trước handler/service        | **8**         |
| **`Mappings/{Module}/`**          | Map Entity ↔ DTO (AutoMapper)         | `*MappingProfile.cs` — Service inject `IMapper`, không map thủ công trong Service | **7** |
| **`Behaviors/`**                  | MediatR pipeline                      | `ValidationBehavior`                                          | **7**         |
| **`Common/Exceptions/`**          | Exception application                 | `NotFoundException`, `ConflictException`                      | **8**         |
| **`Common/Models/`**              | Model dùng chung                      | `PaginatedResult<T>`                                          | **7**         |
|-----------------------------------|---------------------------------------|---------------------------------------------------------------|:-------------:|

### Quy ước `Interfaces/`

```
Interfaces/
├── Repositories/
│   ├── IGenericRepository.cs        ← generic dùng chung (root)
│   ├── IUnitOfWork.cs               ← dùng chung (root)
│   └── {Module}/                    ← gom theo module
│       └── IOrderRepository.cs
├── Services/
│   ├── Common/
│   │   └── ICrudService.cs          ← contract CRUD dùng chung
│   └── {Module}/                    ← gom theo module
│       ├── IOrderService.cs
│       └── IOrderPaymentService.cs
```

|-----------------------|-------------------------------------------|
| Loại chức năng        | Đặt ở                                     |
|-----------------------|-------------------------------------------|
| CRUD đơn giản         | `I*Service : ICrudService<…>` + `CrudApiController` |
| CRUD + logic phụ      | `IService` riêng + `BaseApiController` tùy chỉnh |
| Nghiệp vụ phức tạp    | Method trên `IService` (orchestration)    |
|-----------------------|-------------------------------------------|

**Không đặt ở src/Application:** `DbContext`, class implement repo/service, reference EF Core.

---

## 3. Domain — `src/Domain/`

**Vai trò layer:** Khái niệm nghiệp vụ thuần — entity, rule, event. **Không** phụ thuộc API, EF, HTTP.

|---------------------------|---------------------------------------|-----------------------------------------------------------------------------------------|:-------------:|
| Thư mục                   | Mục đích                              | Chứa gì                                                                                 | Quan trọng    |
|---------------------------|---------------------------------------|-----------------------------------------------------------------------------------------|:-------------:|
| **`src/Domain/`** (root)  | Lõi domain                            | `.csproj` — chỉ `MediatR.Contracts` cho `IDomainEvent`                                  | **10**        |
| **`Base/`**               | Nền tảng entity + Marker Interfaces   | `BaseEntity` (`Id`, DomainEvents); `IAuditable`, `ITracked` (interface đánh dấu audit)  | **10**        |
| **`Entities/`**           | Entity có danh tính (identity)        | `Order`, `Product`… kế thừa `BaseEntity`, implement Interface audit cần thiết           | **10**        |
| **`ValueObjects/`**       | Giá trị không identity, immutable     | `Email`, `Money`, `Address`                                                             | **8**         |
| **`Enums/`**              | Tập giá trị cố định                   | `OrderStatus`, `UserRole`…                                                              | **8**         |
| **`Events/`**             | Domain event — **định nghĩa** sự kiện | `IDomainEvent`, `OrderPlacedEvent`                                                      | **7**         |
| **`Common/`**             | Exception / helper domain             | `DomainException`                                                                       | **7**         |
|---------------------------|---------------------------------------|-----------------------------------------------------------------------------------------|:-------------:|

**Phân biệt event:**

|-----------------------------------------------|---------------------------------------|
| Vị trí                                        | Vai trò                               |
|-----------------------------------------------|---------------------------------------|
| `src/Domain/Events/`                          | Event **đã xảy ra** (record/class)    |
| `src/Application/Features/.../EventHandlers/` | **Xử lý** event (gửi mail, log…)      |
|-----------------------------------------------|---------------------------------------|

**Không đặt ở src/Domain:** DTO API, repository, service implement, `DbContext`.

---

## 4. Infrastructure — `src/Infrastructure/`

**Vai trò layer:** Triển khai kỹ thuật — DB, repo, service, Redis, email, hóa đơn điện tử… **Không** được để layer khác reference trực tiếp (chỉ qua interface Application).

|---------------------------------------|-----------------------------------|-----------------------------------------------------------------------|:-------------:|
| Thư mục                               | Mục đích                          | Chứa gì                                                               | Quan trọng    |
|---------------------------------------|-----------------------------------|-----------------------------------------------------------------------|:-------------:|
| **`src/Infrastructure/`** (root)      | DI Infrastructure                 | `DependencyInjection.cs` — DbContext, UoW                             | **10**        |
| **`Implementations/Repositories/{Module}/`** | **Repo** — implement IRepo, gom theo module | `GenericRepository<T>`, `UnitOfWork` ở root; `{Module}/OrderRepository` | **10** |
| **`Implementations/Services/{Module}/`** | **Service** — implement IService, gom theo module | `{Module}/OrderService`, `{Module}/LeaveService`… | **10** |
| **`Security/`**                         | Helper bảo mật (static, không DI) | `PasswordHasher` — hash/verify mật khẩu; **không** đặt trong `Services/` | **7** |
| **`Persistence/`**                    | EF Core & DbContext               | `AppDbContext`, `DesignTimeDbContextFactory`                          | **10**        |
| **`Persistence/Configurations/`**     | Fluent API mapping entity → bảng  | `OrderConfiguration.cs`                                               | **9**         |
| **`Persistence/Interceptors/`**       | EF Core Interceptor tự động Audit | `AuditSaveChangesInterceptor` — tự gán `CreatedAt/By`, `UpdatedAt/By` | **9**         |
| **`Migrations/`**                     | EF migrations                     | `*_InitialCreate.cs`, snapshot                                        | **9**         |
| **`External/Redis/`**                 | Client / adapter Redis            | Connection wrapper (khi dùng cache)                                   | **6**         |
| **`External/MemoryCache/`**           | In-memory cache adapter           | Wrapper `IMemoryCache` (khi dùng)                                     | **5**         |
|---------------------------------------|-----------------------------------|-----------------------------------------------------------------------|:-------------:|

### Quy ước `Implementations/`

```
Implementations/
├── Repositories/
│   ├── GenericRepository.cs         ← generic dùng chung (root)
│   ├── UnitOfWork.cs                ← dùng chung (root)
│   └── {Module}/                    ← gom theo module, đối xứng Application
│       └── OrderRepository.cs
└── Services/
    └── {Module}/                    ← gom theo module, đối xứng Application
        ├── OrderService.cs
        └── OrderPaymentService.cs
```

**Helper static (không có `I*` interface, không đăng ký DI):** đặt `src/Infrastructure/Security/` — ví dụ `PasswordHasher`. Không đặt trong `Implementations/Services/`.

**Đối xứng với Application (gom theo module):**

|-------------------------------------------------------|--------------------------------------------------------|
| Application                                           | Infrastructure                                         |
|-------------------------------------------------------|--------------------------------------------------------|
| `Interfaces/Repositories/{Module}/IOrderRepository`   | `Implementations/Repositories/{Module}/OrderRepository`|
| `Interfaces/Services/{Module}/IOrderService`          | `Implementations/Services/{Module}/OrderService`       |
|-------------------------------------------------------|--------------------------------------------------------|

**Không đặt ở src/Infrastructure:** Controller, DTO request/response API (trừ binding config).

---

## 5. File & thư mục solution (ngoài các project)

|-------------------------------|---------------------------------------|:-------------:|
| Mục                           | Mục đích                              | Quan trọng    |
|-------------------------------|---------------------------------------|:-------------:|
| `erp-corporation-api-v2.sln`  | Solution quản lý các project          | **9**         |
| `Cấu trúc dự án.txt`          | Spec kiến trúc / blueprint ban đầu    | **6**         |
| `.gitignore`                  | Bỏ qua `bin/`, `obj/`, `.env`         | **8**         |
| Folder ảo `src` trong `.sln`  | Nhóm project trên VS                  | **3**         |
|-------------------------------|---------------------------------------|:-------------:|

---

## 6. Bảng tra nhanh — “File mới đặt đâu?”

|-----------------------|---------------------------------------------------|
| Bạn tạo               | Đặt trong                                         |
|-----------------------|---------------------------------------------------|
| Entity ERP            | `src/Domain/Entities/`                            |
| Enum trạng thái       | `src/Domain/Enums/`                               |
| Value object          | `src/Domain/ValueObjects/`                        |
| Domain event          | `src/Domain/Events/`                              |
| `IOrderRepository`    | `src/Application/Interfaces/Repositories/{Module}/`   |
| `IOrderService`       | `src/Application/Interfaces/Services/{Module}/`       |
| Request/Response API  | `src/Application/DTOs/{Module}/`                      |
| Validator input       | `src/Application/Validators/` hoặc cạnh feature       |
| Command/Query (CQRS)  | `src/Application/Features/{Module}/`                  |
| `OrderRepository`     | `src/Infrastructure/Implementations/Repositories/{Module}/` |
| `OrderService`        | `src/Infrastructure/Implementations/Services/{Module}/`     |
| EF mapping            | `src/Infrastructure/Persistence/Configurations/`      |
| EF Core Interceptor   | `src/Infrastructure/Persistence/Interceptors/`        |
| Marker Interface audit| `src/Domain/Base/` (`IAuditable`, `ITracked`)         |
| Controller            | `src/API/Controllers/{Module}/`                       |
| Middleware HTTP       | `src/API/Middlewares/`                            |
| Đọc `.env`            | `src/API/Configuration/` (EnvLoader auto-map )    |
|-----------------------|---------------------------------------------------|

---

## 7. Mức độ quan trọng — tóm tắt theo layer

|-----------------------|------|--------------------------------|
| Layer                 | OVR  | Ghi chú                        |
|-----------------------|------|--------------------------------|
| **Domain**            | 9–10 | Sai domain → sai toàn hệ thống |
| **Application**       | 8–10 | Contract + use case            |
| **Infrastructure**    | 8–10 | DB & implement                 |
| **API**               | 7–10 | Mỏng, nhưng là cửa vào         |
|-----------------------|------|--------------------------------|

|---------------------------------------------------|:---------------------------------------------:|
| Folder “sẽ đầy” khi code ERP                      | Quan trọng                                    |
|---------------------------------------------------|:---------------------------------------------:|
| `src/Domain/Entities/`                            | **10**                                        |
| `src/Application/Interfaces/Services/`            | **10**                                        |
| `src/Application/Interfaces/Repositories/`        | **10**                                        |
| `src/Infrastructure/Implementations/Services/`    | **10**                                        |
| `src/Infrastructure/Implementations/Repositories/`| **10**                                        |
| `src/API/Controllers/`                            | **10**                                        |
| `src/Application/DTOs/`                           | **9**                                         |
| `src/Application/Features/`                       | **8** (nếu dùng MediatR song song Service)    |
|---------------------------------------------------|:---------------------------------------------:|

|-----------------------------------|-----------------------|
| Folder skeleton (trống, chờ dùng) | Quan trọng hiện tại   |
|-----------------------------------|-----------------------|
| `src/API/Filters/`                | **5**                 |
| `src/Application/Mappings/`       | **6**                 |
| `src/Infrastructure/External/`    | **5–6**               |
| `src/Domain/ValueObjects/`, `Enums/` | **8** khi có module|
|-----------------------------------|-----------------------|

---

## 8. Những điều không nên (tránh rối code)

1. **Controller** gọi thẳng `DbContext` hoặc `OrderRepository` (bỏ qua `IService`).
2. **IService / IRepository** đặt ngoài `src/Application/Interfaces/`.
3. **Implement** repo/service đặt ngoài `src/Infrastructure/Implementations/`.
4. **Entity** expose ra API (luôn qua DTO).
5. **Business rule phức tạp** trong Repository (để ở Service hoặc Domain method).
6. **Secret** trong `appsettings.json` thay vì `.env`.
7. **Tự gán `CreatedAt`/`UpdatedAt` thủ công** trong code nghiệp vụ — để `AuditSaveChangesInterceptor` xử lý tự động.
8. **Dùng `DateTime.UtcNow` trực tiếp** trong Entity/Service — dùng `TimeProvider` (inject qua DI) để dễ test.
9. **Tạo chuỗi kế thừa lớp sâu** cho Entity (`BaseEntity → AuditableEntity → TrackedEntity`) — dùng **Marker Interfaces** (`IAuditable`, `ITracked`) để giữ tính linh hoạt kế thừa.

---

## 9. Trạng thái hiện tại (source base)

|---------------------------------------------------------------|---------------------------------------------------|
| Đã có code                                                    | Chỉ folder / skeleton                             |
|---------------------------------------------------------------|---------------------------------------------------|
| `BaseEntity`, `IDomainEvent`, `DomainException`               | `Entities/`, `Enums/`, `ValueObjects/`            |
| `IGenericRepository`, `IUnitOfWork`                           | `Interfaces/Services/` (chưa có IService cụ thể)  |
| `GenericRepository`, `UnitOfWork`, `AppDbContext`             | `Implementations/Services/`                       |
| `EnvLoader`, `ExceptionMiddleware`                            |                                                   |
| (`ValidationProblemDetails` + `traceId`), `HealthController`  | `Filters/`, `Features/`, `DTOs/`, `Validators/`   |
| Outbox (`OutboxMessages`, `OutboxProcessorHostedService`),    |                                                   |
| DI scan + manual                                              | `Interfaces/Services/` (chưa có IService cụ thể)  |
| Migration `InitialCreate`, `AddOutboxMessages`                | `External/`, entity nghiệp vụ                     |
|---------------------------------------------------------------|---------------------------------------------------|

---

## 10. Quy tắc và luật lệ khi code (phân tầng)

Phần này là **quy ước bắt buộc** (bổ sung mục 8). Cấu trúc theo **tầng** — đọc từ trong ra ngoài cho khớp Dependency Rule.

---

### Phụ thuộc giữa các project (áp dụng mọi code)

- **Domain** không được reference `Application`, `Infrastructure`, `API`.
- **Application** chỉ được reference **Domain** (và package phục vụ use case như MediatR, FluentValidation — không EF).
- **Infrastructure** được reference **Application** + **Domain**; chứa implement interface từ Application.
- **API** được reference **Application** + **Infrastructure**; là host HTTP duy nhất.
- **Không** đảo chiều: ví dụ `Domain` không reference `Infrastructure`; `Application` không reference `Infrastructure`.

---

### Tầng Domain — `src/Domain/`

- `BaseEntity` (`src/Domain/Base/`) chỉ chứa **`Id` (Guid) và `DomainEvents`** — không nhét logic audit vào đây.
- Tính năng audit được khai báo qua **Marker Interfaces** tại `src/Domain/Base/`:
  - `IAuditable` → cột `IsActive`
  - `ICreationTracked` → cột `CreatedAt`, `CreatedBy`
  - `IModificationTracked` → cột `UpdatedAt`, `UpdatedBy`
- Entity **tự do lựa chọn** implement các interface cần thiết, không bị ép kế thừa chuỗi lớp cứng nhắc (tránh xung đột khi cần kế thừa lớp của thư viện bên ngoài như `IdentityUser`).
- Entity kế thừa `BaseEntity`; khi có rule nghiệp vụ, ưu tiên đổi state qua **method** có tên rõ nghĩa thay vì setter public lung tung.
- Value object, enum đặt đúng `ValueObjects/`, `Enums/`; immutable, validate tại chỗ tạo khi cần.
- **Domain event** chỉ **định nghĩa** tại `src/Domain/Events/` — không nhét logic side-effect nặng vào đây.
- `DomainException` và exception nghiệp vụ thuần domain dùng `src/Domain/Common/`.

---

### Tầng Application — `src/Application/`

- **Chỉ interface** hướng ra ngoài persistence: `IRepository` trong `Interfaces/Repositories/`, `IService` trong `Interfaces/Services/` — **không** chứa class `DbContext`, không gọi SQL/EF trực tiếp.
- **DTO** request/response API nằm `DTOs/{Module}/` — **không** trả `Entity` ra contract API.
- **Một module** gom theo tên: ví dụ Order → `DTOs/Orders/`, `IOrderService`, `IOrderRepository` cùng prefix/module cho dễ tìm.
- **Service-first (mặc định)**: Controller → `IService` → `IRepository`. MediatR dùng cho **domain event handlers** (sau Outbox), không gọi từ controller mặc định.
- **AutoMapper**: Profile trong `Mappings/{Module}/`; đăng ký `AddAutoMapper` trong `DependencyInjection.cs`; Service inject `IMapper` — không map thủ công inline.
- **CQRS (tuỳ chọn)**: chỉ khi team chốt case cụ thể; không thay thế Service-first làm luồng chính.
- Validation (FluentValidation), `Behaviors/`, exception application (`Common/Exceptions/`) dùng thống nhất; không duplicate validate mâu thuẫn ở mọi tầng.

---

### Tầng Infrastructure — `src/Infrastructure/`

- **Implement** repository tại `Implementations/Repositories/`; **implement** service tại `Implementations/Services/` — khớp namespace với interface Application.
- **Helper bảo mật static** (hash mật khẩu, v.v.): `Security/` — không đặt trong `Services/` vì không phải class implement `I*Service`.
- **EF Core**: `AppDbContext`, `DesignTimeDbContextFactory` trong `Persistence/`; Fluent mapping trong `Persistence/Configurations/`.
- **Audit tự động (bắt buộc)**: Đăng ký `AuditSaveChangesInterceptor` trong `Persistence/Interceptors/`. Interceptor này kiểm tra entity có implement `ICreationTracked` / `IModificationTracked` hay không và tự động điền `CreatedAt/By`, `UpdatedAt/By` — **không gán thủ công** trong code nghiệp vụ.
- **Thời gian (bắt buộc)**: Dùng `.NET 8 TimeProvider` (đăng ký `services.AddSingleton(TimeProvider.System)`) thay cho `DateTime.UtcNow` trực tiếp. Interceptor và Service đều inject `TimeProvider` qua DI để dễ viết Unit Test.
- **Migration** tạo bằng `dotnet ef`; không sửa lung tung history đã dùng production (trừ quy trình team cho phép).
- **Tích hợp ngoài** (Redis, cache, client HTTP bên thứ ba): ưu tiên `External/` hoặc subfolder rõ tên nhà cung cấp — không trộn với Entity Domain.
- **Outbox**: domain event ghi vào bảng `OutboxMessages` trong cùng transaction với `SaveChanges`; `OutboxProcessorHostedService` publish qua MediatR.
- **DI**: `Infrastructure/Extensions/ServiceRegistrationExtensions` — manual (`IGenericRepository<>`) + scan (`Infrastructure.Implementations` → `Application.Interfaces`).

---

### Tầng API — `src/API/`

- **Controller mỏng**: bind HTTP ↔ DTO, gọi **`IService`** — **không** inject `DbContext`, **không** inject class Repository cụ thể, **không** gọi `IMediator` mặc định.
- **CRUD đơn giản**: kế thừa `CrudApiController<TService, TDto, TCreate, TUpdate>` với `TService : ICrudService<…>`; permission khai báo trong constructor.
- **CRUD + endpoint phụ** (roles, permissions, reset password…): kế thừa `BaseApiController`, gọi `IService` tùy chỉnh.
- **Luồng chuẩn**: `Controller` → `IService` → `IRepository` → persistence. Không bỏ qua Service để gọi Repo từ Controller.
- **Middleware / Filters**: xử lý cross-cutting HTTP (lỗi, auth sau này, v.v.) — không nhét business ERP vào middleware trừ khi thực sự là concern HTTP.

---

### Xuyên tầng — cấu hình, bảo mật, chất lượng

**Cấu hình và bảo mật**

- Secret và connection string: **`src/API/.env`** (hoặc biến môi trường do hạ tầng inject). **Không** commit `.env`.
- Runtime DB: chỉ đọc **`ConnectionStrings:DefaultConnection`** (`EnvLoader` map từ `CONNECTION_STRING`).
- `appsettings*.json`: chỉ giá trị **không nhạy cảm** (logging, allowed hosts, …).
- Lỗi API: `ValidationProblemDetails` (errors theo field) + `extensions.traceId` trong `ExceptionMiddleware`.

**Chất lượng và I/O**

- Dùng `async`/`await` trên luồng request; tránh `.Result` / `.Wait()`.
- Method I/O nên có `CancellationToken` khi phù hợp.
- Lỗi nghiệp vụ: exception đã định nghĩa + middleware map **thống nhất** (ví dụ ProblemDetails).
- **Luôn** validate input (FluentValidation hoặc trong Service) — không tin input client.

**Git và tài liệu**

- Xem diff trước push; không đẩy secret nhầm vào repo.
- Commit theo quy trình team / khi được yêu cầu; message rõ ràng.
- Đổi cấu trúc thư mục quan trọng → cập nhật `CAU-TRUC-THU-MUC.md` (ít nhất mục **Cấu trúc thư mục (ALL)** và phần quy tắc liên quan) **trong cùng PR**.
- Checklist PR: xem **`CHECKLIST-PR.md`**.

---

*Tài liệu phản ánh cấu trúc tại thời điểm tạo. Khi đổi folder, cập nhật file này cho đồng bộ.*
