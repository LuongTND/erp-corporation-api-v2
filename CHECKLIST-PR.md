# Checklist Pull Request — erp-corporation-api-v2

## Kiến trúc & luồng

- [ ] Controller **Service-first**: chỉ gọi `I*Service`, không inject `DbContext` / repository cụ thể
- [ ] Không gọi `IMediator` từ controller (trừ khi team chốt ngoại lệ có ghi chú)
- [ ] Business rule phức tạp nằm ở Service hoặc Domain, không ở Repository

## Cấu hình

- [ ] Secret / connection string trong `API/.env` (không commit `.env`)
- [ ] Runtime chỉ đọc `ConnectionStrings:DefaultConnection` (EnvLoader map từ `.env`)

## API & lỗi

- [ ] Lỗi validation trả `ValidationProblemDetails` (errors theo field)
- [ ] Mọi `ProblemDetails` có `extensions.traceId`

## Persistence & events

- [ ] Domain event quan trọng đi qua **Outbox** (không publish trực tiếp sau `SaveChanges`)
- [ ] Migration mới chạy được trên DB dev (nếu đổi schema)

## DI

- [ ] Interface trong `Application/Interfaces/*`, implement trong `Infrastructure/Implementations/*`
- [ ] Đăng ký thủ công cho binding đặc biệt; scan chỉ trong `Infrastructure.Implementations`

## Tài liệu

- [ ] Đổi cấu trúc thư mục / quy tắc → cập nhật `CAU-TRUC-THU-MUC.md` **cùng PR**

## Build

- [ ] `dotnet build` không lỗi
- [ ] Smoke test endpoint chính (Swagger / `API.http`)
